using AutoGenerator.Code.Services;
using AutoGenerator.Code.VM;
using AutoGenerator.Code.VM.v1;
using AutoGenerator.Custom.Models;
using AutoGenerator.Data;
using AutoGenerator.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CategorySubType = AutoGenerator.Code.VM.v1.CategorySubType;

namespace AutoGenerator.Custom.Data
{

    public class CodeFilterCriteria
    {
        public string? Keyword { get; set; }
        public string? Category { get; set; }
        public string? SubType { get; set; }
        public string? PathFile { get; set; }
        public string? Name { get; set; }
        public string? NamespaceName { get; set; }
        public string? UsingKeyword { get; set; }
        public string? ExactUsing { get; set; }
        public string? BaseClass { get; set; }
        public bool? HasAdditionalCode { get; set; }
        public bool? HasUsings { get; set; }
        public bool? WithoutPathFile { get; set; }
        public bool? WithoutBaseClass { get; set; }
        public DateTimeOffset? CreatedAfter { get; set; }
        public string? NameStartsWith { get; set; }
        public string? PathFileEndsWith { get; set; }
        public int? UsingCount { get; set; }
        public string? RegexPattern { get; set; }
    }
    public record BulkDeleteResult(string Id, bool IsSuccess, string? ErrorMessage = null);

        public interface ICodeGeneratorRepository
        {
            Task<List<CodeGenerator>> GetAllAsync();
            Task<CodeGenerator?> GetByIdAsync(string id);
            Task CreateAsync(CodeGenerator item);
            Task<bool> UpdateAsync(CodeGenerator item);
            Task<bool> DeleteAsync(string id);
            Task<List<BulkDeleteResult>> BulkDeleteAsync(BulkDeleteRequest request);

            Task<CodeGenerator?> GetByPathFileAsync(string pathFile);
            Task SwapCodesAsync(string id1, string id2);
            Task<(List<CodeGenerator> Items, int TotalCount)> GetFilteredAndPagedAsync(CodeFilterCriteria criteria, int page, int pageSize);
            Task< MetadataLists> GetMetadataListsAsync();
            Task<CountSummaries> GetCountSummariesAsync(string? usingLine = null);

            Task<List<CodeGenerator>?> GetCodesBy(string type);
            Task<List<CodeGenerator>?> GetCodesBy<T>() where T : class;

            Task<string?> GetSModels();

            Task<string> GetRawCodes<T>() where T : class;

            CodeHistory AddOrUpdateCodeHistory(CodeGenerator code, string state = "change");
            Task<bool> SaveChanges();
        }
    
    public class CodeRepository : TBaseRepository<ApplicationCodeUser, IdentityRole, string, CodeGenerator>, ICodeGeneratorRepository
    {
        private readonly CodeDataContext _context;
        private readonly ILogger _logger;
        private readonly UserClaim _userClaim;

        public CodeRepository(CodeDataContext db, ILoggerFactory logger, UserClaim userClaim) : base(db, logger)
        {
            _userClaim = userClaim;
            _context = db;
            _logger = logger.CreateLogger<CodeRepository>();
        }

        public async Task<List<CodeGenerator>?> GetCodesBy(string type)
        {
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("GetCodesBy: UserClaim or ProjectId is null/empty.");
                return null;
            }

            var ms = await _context.Set<CodeGenerator>()
                                   .Where(x => x.Type == type && x.ProjectId == _userClaim.ProjectId)
                                   .ToListAsync();

            _logger.LogInformation($"GetCodesBy({type}): Found {ms?.Count ?? 0} items.");
            return ms;
        }

        public Task<List<CodeGenerator>?> GetCodesBy<T>() where T : class
        {
            _logger.LogInformation($"GetCodesBy<{typeof(T).Name}>()");
            return Task.Run(async () =>
            {
                var allOfType = await GetCodesBy(typeof(T).Name);
                if (allOfType == null) return null;
                _logger.LogInformation($"GetCodesBy<{typeof(T).Name}>(): Found {allOfType.Count(item => item is T)} items of type T.");
                return allOfType;
            });
        }

        public async Task<string?> GetSModels()
        {
            _logger.LogInformation("Fetching 'model' codes for SModels string.");
            var models = await GetCodesBy("model");

            if (models == null || !models.Any())
            {
                _logger.LogInformation("No 'model' codes found for SModels.");
                return null;
            }

            var sb = new StringBuilder();
            foreach (var model in models)
            {
                if (!string.IsNullOrWhiteSpace(model.Code))
                {
                    sb.AppendLine(model.Code);
                    sb.AppendLine();
                }
            }
            _logger.LogInformation($"Generated SModels string ({sb.Length} characters).");
            return sb.ToString();
        }

        public async Task<string> GetRawCodes<T>() where T : class
        {
            _logger.LogInformation($"Fetching raw codes for type {typeof(T).Name}.");
            var codes = await GetCodesBy<T>();
            if (codes == null || !codes.Any())
            {
                _logger.LogInformation($"No codes found for type {typeof(T).Name}.");
                return string.Empty;
            }
            var sb = new StringBuilder();
            foreach (var code in codes)
            {
                if (!string.IsNullOrWhiteSpace(code.Code))
                {
                    sb.AppendLine(code.Code);
                    sb.AppendLine("===========================");
                }
            }
            _logger.LogInformation($"Generated raw codes string for type {typeof(T).Name} ({sb.Length} characters).");
            return sb.ToString();
        }

        public async Task<bool> SaveChanges()
        {
            _logger.LogInformation("Executing SaveChanges logic based on history.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("SaveChanges: UserClaim or ProjectId is null/empty, cannot save.");
                return false;
            }

            var codesWithChanges = await _context.CodeGenerators
                 .Where(x => x.ProjectId == _userClaim.ProjectId)
                 .Include(x => x.CodeHistories.Where(ch => ch.CodeId == "change"))
                 .Where(x => x.CodeHistories.Any())
                 .ToListAsync();

            _logger.LogInformation($"Found {codesWithChanges.Count} CodeGenerators with pending 'change' history.");

            bool overallSuccess = true;

            foreach (var code in codesWithChanges)
            {
                foreach (var codeHistory in code.CodeHistories)
                {
                    _logger.LogDebug($"Applying history change for CodeGenerator ID {code.Id} from history ID {codeHistory.Id}.");
                    try
                    {
                        code.Code = codeHistory.CodeSnapshot;
                        codeHistory.CodeId = "done";
                        _context.CodeHistories.Update(codeHistory);
                        _context.Set<CodeGenerator>().Update(code);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error applying history or marking as done for CodeGenerator ID {code.Id}.");
                        overallSuccess = false;
                    }
                }
            }

            try
            {
                int changesSaved = await _context.SaveChangesAsync();
                _logger.LogInformation($"Finished SaveChanges process. Persisted {changesSaved} changes to the database.");
                return overallSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during final SaveChangesAsync after processing history.");
                return false;
            }
        }

        public CodeHistory AddOrUpdateCodeHistory(CodeGenerator code, string state = "change")
        {
            _logger.LogInformation($"Adding code history for CodeGenerator ID {code.Id} with state '{state}'.");
            var codeHistory = new CodeHistory
            {
                CodeId = state,
                CodeGeneratorId = code.Id,
                CodeSnapshot = code.Code,
                CreatedAt = DateTime.UtcNow,
            };

            _context.CodeHistories.Add(codeHistory);

            try
            {
                _context.SaveChanges();
                _logger.LogInformation($"Code history added and saved immediately for ID {code.Id}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving code history immediately for ID {code.Id}.");
                throw;
            }

            return codeHistory;
        }

        public async Task<List<CodeGenerator>> GetAllAsync()
        {
            _logger.LogInformation("GetAllAsync: Fetching all CodeGenerators for project.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("GetAllAsync: UserClaim or ProjectId is null/empty.");
                return new List<CodeGenerator>();
            }
            return await _context.Set<CodeGenerator>().Where(x => x.ProjectId == _userClaim.ProjectId).ToListAsync();
        }

        public async Task<CodeGenerator?> GetByIdAsync(string id)
        {
            _logger.LogInformation($"GetByIdAsync: Fetching CodeGenerator item with Id: {id} for project {_userClaim?.ProjectId}.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId)) return null;
            return await _context.Set<CodeGenerator>()
                                 .Where(c => c.ProjectId == _userClaim.ProjectId && c.Id == id)
                                 .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(CodeGenerator item)
        {
            _logger.LogInformation($"CreateAsync: Creating CodeGenerator item with Id: {item.Id} for project {_userClaim?.ProjectId}.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("CreateAsync: UserClaim or ProjectId is null/empty. Cannot proceed.");
                throw new InvalidOperationException("Cannot create item without project context.");
            }
            item.ProjectId = _userClaim.ProjectId; // Set ProjectId

            await _context.Set<CodeGenerator>().AddAsync(item);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"CreateAsync: Created CodeGenerator item with Id: {item.Id}");
        }

        public async Task<bool> UpdateAsync(CodeGenerator item)
        {
            _logger.LogInformation($"UpdateAsync: Updating CodeGenerator item with Id: {item.Id} for project {_userClaim?.ProjectId}.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("UpdateAsync: UserClaim or ProjectId is null/empty. Cannot proceed.");
                return false;
            }
            var existing = await _context.Set<CodeGenerator>().Where(c => c.ProjectId == _userClaim.ProjectId && c.Id == item.Id).FirstOrDefaultAsync();
            if (existing == null)
            {
                _logger.LogWarning($"UpdateAsync: Item with Id {item.Id} not found in project {_userClaim.ProjectId} for update.");
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(item); // Simple update, assumes CodeGenerator properties match DB columns
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"UpdateAsync: Updated CodeGenerator item with Id: {item.Id}");
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"UpdateAsync: Concurrency conflict updating CodeGenerator item with Id: {item.Id}");
                // Handle concurrency resolution here or let caller catch
                return false; // Indicate update failed, likely due to concurrency
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateAsync: Error updating CodeGenerator item with Id: {item.Id}");
                throw; // Re-throw other exceptions
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            _logger.LogInformation($"DeleteAsync: Deleting CodeGenerator item with Id: {id} for project {_userClaim?.ProjectId}.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("DeleteAsync: UserClaim or ProjectId is null/empty. Cannot proceed.");
                return false;
            }
            var existing = await _context.Set<CodeGenerator>().Where(c => c.ProjectId == _userClaim.ProjectId && c.Id == id).FirstOrDefaultAsync();
            if (existing == null)
            {
                _logger.LogInformation($"DeleteAsync: Item with Id {id} not found in project {_userClaim.ProjectId} for deletion.");
                return false;
            }

            _context.Set<CodeGenerator>().Remove(existing);
            try
            {
                int changes = await _context.SaveChangesAsync();
                _logger.LogInformation($"DeleteAsync: Deleted CodeGenerator item with Id: {id}. Changes saved: {changes}");
                return changes > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteAsync: Error deleting CodeGenerator item with Id: {id}");
                throw;
            }
        }

        public async Task<List<BulkDeleteResult>> BulkDeleteAsync(BulkDeleteRequest request)
        {
            _logger.LogInformation("BulkDeleteAsync: Performing bulk delete for CodeGenerators.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("BulkDeleteAsync: UserClaim or ProjectId is null/empty. Cannot proceed.");
                return request.Ids?.Select(id => new BulkDeleteResult(id, false, "Missing project context.")).ToList()
                       ?? new List<BulkDeleteResult>();
            }

            var query = _context.Set<CodeGenerator>().Where(c => c.ProjectId == _userClaim.ProjectId).AsQueryable();

            if (request.Ids != null && request.Ids.Any())
            {
                query = query.Where(c => request.Ids.Contains(c.Id));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(request.Category))
                {
                    query = query.Where(c => !string.IsNullOrWhiteSpace(c.Type) && c.Type.Equals(request.Category, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrWhiteSpace(request.SubType))
                {
                    query = query.Where(c => !string.IsNullOrWhiteSpace(c.SubType) && c.SubType.Equals(request.SubType, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrWhiteSpace(request.PathFile))
                {
                    query = query.Where(c => !string.IsNullOrWhiteSpace(c.PathFile) && c.PathFile.Equals(request.PathFile, StringComparison.OrdinalIgnoreCase));
                }
            }

            var itemsToDelete = await query.ToListAsync();
            var results = new List<BulkDeleteResult>();

            if (!itemsToDelete.Any())
            {
                _logger.LogInformation("BulkDeleteAsync: No items found matching criteria for deletion.");
                return results;
            }

            _logger.LogInformation($"BulkDeleteAsync: Found {itemsToDelete.Count} items to delete.");

            _context.Set<CodeGenerator>().RemoveRange(itemsToDelete); // Stage all for deletion

            try
            {
                int changes = await _context.SaveChangesAsync(); // Save all deletions in one go
                _logger.LogInformation($"BulkDeleteAsync: SaveChangesAsync completed. {changes} entities affected.");
                // Assume success for all items that were fetched if SaveChangesAsync succeeds.
                // More granular tracking would iterate itemsToDelete *after* savechanges and check state.
                return itemsToDelete.Select(item => new BulkDeleteResult(item.Id, true)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BulkDeleteAsync: Error during SaveChangesAsync for bulk delete.");
                // If the batch save fails, assume all *in this batch* failed, or return partial success if possible.
                // Returning failures for all attempted items in case of batch save failure is a safe approach.
                return itemsToDelete.Select(item => new BulkDeleteResult(item.Id, false, $"Batch save failed: {ex.Message}")).ToList();
            }
        }

        public async Task<CodeGenerator?> GetByPathFileAsync(string pathFile)
        {
            _logger.LogInformation($"GetByPathFileAsync: Fetching CodeGenerator item with PathFile: {pathFile} for project {_userClaim?.ProjectId ?? "N/A"}.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId)) return null;
            return await _context.Set<CodeGenerator>()
                                 .Where(c => c.ProjectId == _userClaim.ProjectId && c.PathFile == pathFile)
                                 .FirstOrDefaultAsync();
        }

        public async Task SwapCodesAsync(string id1, string id2)
        {
            _logger.LogInformation($"SwapCodesAsync: Swapping code between CodeGenerators {id1} and {id2} for project {_userClaim?.ProjectId ?? "N/A"}.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("SwapCodesAsync: UserClaim or ProjectId is null/empty. Cannot proceed.");
                throw new InvalidOperationException("Cannot perform swap without project context.");
            }

            var item1Task = _context.Set<CodeGenerator>().Where(c => c.ProjectId == _userClaim.ProjectId && c.Id == id1).FirstOrDefaultAsync();
            var item2Task = _context.Set<CodeGenerator>().Where(c => c.ProjectId == _userClaim.ProjectId && c.Id == id2).FirstOrDefaultAsync();

            await Task.WhenAll(item1Task, item2Task);

            var item1 = item1Task.Result;
            var item2 = item2Task.Result;

            if (item1 == null) throw new InvalidOperationException($"Source item with Id '{id1}' not found for project {_userClaim.ProjectId}.");
            if (item2 == null) throw new InvalidOperationException($"Target item with Id '{id2}' not found for project {_userClaim.ProjectId}.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tempCode = item1.Code;
                item1.Code = item2.Code;
                item2.Code = tempCode;

                _context.Set<CodeGenerator>().Update(item1);
                _context.Set<CodeGenerator>().Update(item2);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation($"SwapCodesAsync: Successfully swapped code between {id1} and {id2}.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"SwapCodesAsync: Transaction rolled back due to error swapping code between {id1} and {id2}.");
                throw new InvalidOperationException($"Failed to swap code transactionally: {ex.Message}", ex);
            }
        }

        public async Task<(List<CodeGenerator> Items, int TotalCount)> GetFilteredAndPagedAsync(CodeFilterCriteria criteria, int page, int pageSize)
        {
            _logger.LogInformation($"GetFilteredAndPagedAsync: Filtering and paging CodeGenerators (Page {page}, Size {pageSize}).");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("GetFilteredAndPagedAsync: UserClaim or ProjectId is null/empty.");
                return (new List<CodeGenerator>(), 0);
            }

            var query = _context.Set<CodeGenerator>().Where(c => c.ProjectId == _userClaim.ProjectId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                query = query.Where(c =>
                    (!string.IsNullOrWhiteSpace(c.Name) && c.Name.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.Type) && c.Type.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.Code) && c.Code.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.SubType) && c.SubType.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.PathFile) && c.PathFile.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.NamespaceName) && c.NamespaceName.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.AdditionalCode) && c.AdditionalCode.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.BaseClass) && c.BaseClass.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrWhiteSpace(criteria.Category))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.Type) && c.Type.Equals(criteria.Category, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(criteria.SubType))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.SubType) && c.SubType.Equals(criteria.SubType, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(criteria.PathFile))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.PathFile) && c.PathFile.Equals(criteria.PathFile, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(criteria.Name))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.Name) && c.Name.Equals(criteria.Name, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(criteria.NamespaceName))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.NamespaceName) && c.NamespaceName.Equals(criteria.NamespaceName, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(criteria.BaseClass))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.BaseClass) && c.BaseClass.Equals(criteria.BaseClass, StringComparison.OrdinalIgnoreCase));
            }

           

            if (criteria.HasAdditionalCode.HasValue)
            {
                query = query.Where(c => criteria.HasAdditionalCode.Value ? !string.IsNullOrWhiteSpace(c.AdditionalCode) : string.IsNullOrWhiteSpace(c.AdditionalCode));
            }
          
            if (criteria.WithoutPathFile.HasValue)
            {
                query = query.Where(c => criteria.WithoutPathFile.Value ? string.IsNullOrWhiteSpace(c.PathFile) : !string.IsNullOrWhiteSpace(c.PathFile));
            }
            if (criteria.WithoutBaseClass.HasValue)
            {
                query = query.Where(c => criteria.WithoutBaseClass.Value ? string.IsNullOrWhiteSpace(c.BaseClass) : !string.IsNullOrWhiteSpace(c.BaseClass));
            }

            if (criteria.CreatedAfter.HasValue)
            {
                query = query.Where(c => c.CreatedAt > criteria.CreatedAfter.Value);
            }

            if (!string.IsNullOrWhiteSpace(criteria.NameStartsWith))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.Name) && c.Name.StartsWith(criteria.NameStartsWith, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(criteria.PathFileEndsWith))
            {
                query = query.Where(c => !string.IsNullOrWhiteSpace(c.PathFile) && c.PathFile.EndsWith(criteria.PathFileEndsWith, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(criteria.RegexPattern))
            {
                try
                {
                    var regex = new Regex(criteria.RegexPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                    query = query.AsEnumerable().Where(c => !string.IsNullOrWhiteSpace(c.Code) && regex.IsMatch(c.Code)).AsQueryable();
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException($"Invalid regex pattern provided: {criteria.RegexPattern}");
                }
            }

            query = query.OrderBy(c => c.Name ?? "")
                         .ThenBy(c => c.Id);

            var totalCount = await query.CountAsync();
            _logger.LogInformation($"GetFilteredAndPagedAsync: Found {totalCount} matching items.");

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            _logger.LogInformation($"GetFilteredAndPagedAsync: Returning {items.Count} items for page {page}.");

            return (items, totalCount);
        }

        public async Task< MetadataLists> GetMetadataListsAsync()
        {
            _logger.LogInformation("GetMetadataListsAsync: Fetching CodeGenerator metadata lists for project.");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("GetMetadataListsAsync: UserClaim or ProjectId is null/empty.");
                return new MetadataLists();
            }

            var allCodes = await _context.Set<CodeGenerator>().Where(c => c.ProjectId == _userClaim.ProjectId).ToListAsync();

            var metadata = new MetadataLists
            {
                Categories = allCodes
                            .Where(c => !string.IsNullOrWhiteSpace(c.Type))
                            .Select(c => c.Type!)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(s => s)
                            .ToList(),

                Namespaces = allCodes
                            .Where(c => !string.IsNullOrWhiteSpace(c.NamespaceName))
                            .Select(c => c.NamespaceName!)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(s => s)
                            .ToList(),

                PathFiles = allCodes
                           .Where(c => !string.IsNullOrWhiteSpace(c.PathFile))
                           .Select(c => c.PathFile!)
                           .Distinct(StringComparer.OrdinalIgnoreCase)
                           .OrderBy(s => s)
                           .ToList(),

                Ids = allCodes.Select(c => c.Id!).OrderBy(s => s).ToList(),
                Names = allCodes.Where(c => !string.IsNullOrWhiteSpace(c.Name)).Select(c => c.Name!).OrderBy(s => s).ToList(),

                BaseClasses = allCodes
                            .Where(c => !string.IsNullOrWhiteSpace(c.BaseClass))
                            .Select(c => c.BaseClass!)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(s => s)
                            .ToList(),

               

                CategorySubTypes = allCodes
                                    .Where(c => !string.IsNullOrWhiteSpace(c.Type))
                                    .GroupBy(c => c.Type!)
                                    .Select(g => new CategorySubType
                                    {
                                        Category = g.Key,
                                        SubTypes = g.Where(item => !string.IsNullOrWhiteSpace(item.SubType))
                                                     .Select(item => item.SubType!)
                                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                                     .OrderBy(s => s)
                                                     .ToList()
                                    })
                                    .OrderBy(cs => cs.Category, StringComparer.OrdinalIgnoreCase)
                                    .ToList()
            };

            _logger.LogInformation($"GetMetadataListsAsync: Finished fetching metadata. Categories: {metadata.Categories.Count}, Namespaces: {metadata.Namespaces.Count}, etc.");
            return metadata;
        }

        public async Task<CountSummaries> GetCountSummariesAsync(string? usingLine = null)
        {
            _logger.LogInformation($"GetCountSummariesAsync: Fetching CodeGenerator count summaries for project (UsingLine: {usingLine ?? "None"}).");
            if (string.IsNullOrEmpty(_userClaim?.ProjectId))
            {
                _logger.LogWarning("GetCountSummariesAsync: UserClaim or ProjectId is null/empty.");
                var emptySummaries = new CountSummaries();
                if (!string.IsNullOrWhiteSpace(usingLine)) emptySummaries.SpecificUsingLine = usingLine;
                return emptySummaries;
            }

            var query = _context.Set<CodeGenerator>().Where(c => c.ProjectId == _userClaim.ProjectId);

            var totalCountTask = query.CountAsync();
            var totalLinesTask = query.SumAsync(c => string.IsNullOrWhiteSpace(c.Code) ? 0 : c.Code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length);
            var countByCategoryTask = query.GroupBy(c => c.Type ?? "Unknown").Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);
            var countByNamespaceTask = query.GroupBy(c => c.NamespaceName ?? "Unknown").Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);
            var countByBaseClassTask = query.GroupBy(c => c.BaseClass ?? "None").Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);

            await Task.WhenAll(totalCountTask, totalLinesTask, countByCategoryTask, countByNamespaceTask, countByBaseClassTask);

            var summaries = new CountSummaries
            {
                TotalCount = totalCountTask.Result,
                TotalLinesOfCode = totalLinesTask.Result,
                CountByCategory = countByCategoryTask.Result,
                CountByNamespace = countByNamespaceTask.Result,
                CountByBaseClass = countByBaseClassTask.Result,
            };

            if (!string.IsNullOrWhiteSpace(usingLine))
            {
                var cleanUsingLine = usingLine.Trim();
                var codesWithUsings = await query.Where(c => c.Usings != null).ToListAsync();
               
                summaries.SpecificUsingLine = usingLine;
                _logger.LogInformation($"GetCountSummariesAsync: Count for using line '{usingLine}': {summaries.CountForSpecificUsing}");
            }

            _logger.LogInformation($"GetCountSummariesAsync: Finished fetching counts. Total: {summaries.TotalCount}, Lines: {summaries.TotalLinesOfCode}, etc.");
            return summaries;
        }
    }
}