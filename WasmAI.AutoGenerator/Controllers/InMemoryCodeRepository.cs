using AutoGenerator.ApiFolder;
using AutoGenerator.Code;
using AutoGenerator.Code.VM.v1;
using AutoGenerator.Custom.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoGenerator.Code.Services
{
    public class InMemoryCodeRepository
    {
        private readonly ConcurrentDictionary<string, CodeVM> _codes = new ConcurrentDictionary<string, CodeVM>();

        private readonly string smodels ;
        public static  string PathModels { get; set; } = string.Empty;
        public InMemoryCodeRepository(ICollection<ITGenerator>? generators = null,string? pathmodels=null)
        {
            var gs = generators ?? BaseGenerator.TGenerators ;

            CombineCSFiles(PathModels, "InMemoryCodeRepository_ttt.txt");
            smodels=File.ReadAllText("InMemoryCodeRepository_ttt.txt");
            LoadInitialCodes(gs);

        }

      

        public string GetSModels()
        {
            return smodels;
        }
        public static void CombineCSFiles(string folderPath, string outputFilePath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("The specified folder does not exist.");
                return;
            }

            string[] csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            using (StreamWriter writer = new StreamWriter(outputFilePath, false))
            {
                foreach (string file in csFiles)
                {
                    writer.WriteLine($"// ===== File: {Path.GetFileName(file)} =====");
                    string content = File.ReadAllText(file);
                    writer.WriteLine(content);
                    writer.WriteLine(); // line break between files
                }
            }

            Console.WriteLine($"Combined {csFiles.Length} files into: {outputFilePath}");
        }
        private void LoadInitialCodes(ICollection<ITGenerator>? generators)
        {
            if (generators == null) return;

            foreach (var generator in generators)
            {
                try
                {
                    var options = generator.Options;
                    List<string>? usingsList = null;
              
                    
                     if (options.Usings is List<string> usingsFromList)
                    {
                        usingsList = usingsFromList
                                     .Where(u => !string.IsNullOrWhiteSpace(u))
                                     .Select(u => u.Trim())
                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                     .ToList();
                    }
                    if (usingsList != null && !usingsList.Any()) usingsList = null;

                    string? FilePath = generator.GetFilePath();
                   

                    var code = new CodeVM
                    {
                        Id = FilePath,
                        Name = generator.GetType().Name,
                        Type = options.Type ?? "Unknown",
                        Code = generator.GetCode() ?? string.Empty,
                        PathFile = FilePath,
                        NamespaceName = options.NamespaceName,
                        AdditionalCode = options.AdditionalCode ?? string.Empty,
                        Usings = usingsList,
                        SubType = options.SubType,
                        BaseClass = options.BaseClass,
                        TypeModel = options.SourceType,
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    if (!string.IsNullOrWhiteSpace(code.Id))
                    {
                        _codes.AddOrUpdate(code.Id, code, (key, existingVal) => code);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing code from generator {generator.GetType().Name}: {ex.Message}");
                }
            }
        }

        public CodeVM AddOrUpdate(CodeVM code)
        {
            if (code == null || string.IsNullOrWhiteSpace(code.Id)) throw new ArgumentException("CodeVM must have a valid Id.", nameof(code));

            code.Name ??= string.Empty;
            code.Type ??= string.Empty;
            code.Code ??= string.Empty;
            code.AdditionalCode ??= string.Empty;

            if (!string.IsNullOrWhiteSpace(code.PathFile))
            {
                try { code.PathFile = Path.GetFullPath(code.PathFile).Replace('\\', '/'); }
                catch { } // Ignore path normalization errors on save
            }

            if (code.Usings != null)
            {
                code.Usings = code.Usings
                                    .Where(u => !string.IsNullOrWhiteSpace(u))
                                    .Select(u => u.Trim())
                                    .Distinct(StringComparer.OrdinalIgnoreCase)
                                    .ToList();
                if (!code.Usings.Any()) code.Usings = null;
            }

            code.IsChanged = true;
            var result = _codes.AddOrUpdate(code.Id, code, (key, existingVal) => code);
            return result;
        }

        // save all changes to the file
        public void SaveChanges()
        {
            foreach (var code in _codes.Values)
            {
                if (code.IsChanged && !string.IsNullOrWhiteSpace(code.PathFile))
                {
                    try
                    {
                        CodeSaveValidator.ValidateAndSave(code.Code, code.PathFile);
                        code.IsChanged = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving code to file {code.PathFile}: {ex.Message}");
                    }
                }
            }
        }

        public bool TryGet(string id, out CodeVM? code)
        {
            if (string.IsNullOrWhiteSpace(id)) { code = null; return false; }
            string lookupId = id;
            if (Guid.TryParse(id, out Guid guid)) lookupId = guid.ToString().ToLowerInvariant();
            else if (id.Contains('/') || id.Contains('\\')) { try { lookupId = Path.GetFullPath(id).Replace('\\', '/').ToLowerInvariant(); } catch { } }
            else { lookupId = id.Trim(); }
            return _codes.TryGetValue(lookupId, out code);
        }

        public bool TryRemove(string id, out CodeVM? code)
        {
            if (string.IsNullOrWhiteSpace(id)) { code = null; return false; }
            string lookupId = id;
            if (Guid.TryParse(id, out Guid guid)) lookupId = guid.ToString().ToLowerInvariant();
            else if (id.Contains('/') || id.Contains('\\')) { try { lookupId = Path.GetFullPath(id).Replace('\\', '/').ToLowerInvariant(); } catch { } }
            else { lookupId = id.Trim(); }
            return _codes.TryRemove(lookupId, out code);
        }

        public string? ResolveIdentifierToKey(CodeIdentifier identifier)
        {
            if (identifier == null) return null;
            if (!string.IsNullOrWhiteSpace(identifier.Id))
            {
                if (Guid.TryParse(identifier.Id, out Guid guid)) return guid.ToString().ToLowerInvariant();
                return identifier.Id.Trim();
            }
            if (!string.IsNullOrWhiteSpace(identifier.PathFile))
            {
                try { return Path.GetFullPath(identifier.PathFile).Replace('\\', '/').ToLowerInvariant(); }
                catch { return null; }
            }
            return null;
        }

        public IEnumerable<CodeVM> GetAll()
        {
            return _codes.Values.AsEnumerable();
        }

        public IEnumerable<CodeVM> GetByType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return Enumerable.Empty<CodeVM>();
            return _codes.Values.Where(c => c.Name?.Equals(type, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        public IEnumerable<CodeVM> GetValidatorCodes()
        {
            return GetByType("ValidatorGenerator");
        }

        public IEnumerable<CodeVM> GetCodes<T>()
            where T : ITGenerator
        {

            var typeName = typeof(T).Name;
            return GetByType(typeName);
        }

     


        public IEnumerable<CodeVM> GetValdatorContexts()
        {
            var validatorCodes = GetValidatorCodes();
            var validatorContexts = new List<CodeVM>();
            foreach (var validator in validatorCodes)
            {
                if(validator.Code.Contains("ValidatorContext : ValidatorContext<"))

                        validatorContexts.Add(validator);
            }
            return validatorContexts;
        }

        public string GetRawCodes<T>()
            where T : ITGenerator
        {
            var typeName = typeof(T).Name;
            var codes = GetByType(typeName);
            if (codes == null || !codes.Any()) return string.Empty;
            var sb = new StringBuilder();
            foreach (var code in codes)
            {
                sb.AppendLine(code.Code);
                sb.AppendLine("===========================");

            }
            return sb.ToString();
        }

    }
}