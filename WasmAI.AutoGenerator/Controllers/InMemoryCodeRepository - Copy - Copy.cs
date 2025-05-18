// Assuming CodeIdentifier is in this namespace
using AutoGenerator.Custom.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;


namespace AutoGenerator.Code.Services
{
    public class UserClaim
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public string ProjectId { get; set; } = "";

        // يمكنك إضافة خصائص أخرى حسب الحاجة
    }
    public class SessionTokenReader
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionTokenReader(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("JWT_TOKEN");
        }

        public UserClaim GetUserClaim()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return null;

            return TokenService.GetUserClaim(token);
        }
    }

    public class TokenService
    {
        private static readonly string _keyjwt = "SuperSecretJwtKeyThatIsLongEnough123456789"; // 
        private readonly string token;
        public TokenService(string token)
        {
            this.token = token;
        }



        public static UserClaim GetUserClaim(string token)
        {
            var tokenService = new TokenService(token);
            return tokenService.GetUserClaimFromToken();
        }
        public UserClaim GetUserClaimFromToken()
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_keyjwt));
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // إذا كنت بحاجة للتحقق من المصدر، يمكن ضبطه إلى true
                    ValidateAudience = false, // إذا كنت بحاجة للتحقق من الجمهور، يمكن ضبطه إلى true
                    ValidateLifetime = true,
                    IssuerSigningKey = key
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // إنشاء كائن UserClaim من Claims
                var userClaim = new UserClaim
                {
                    UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Username = principal.Identity.Name,
                    Email = principal.FindFirst(ClaimTypes.Email)?.Value,
                    Role = principal.FindFirst(ClaimTypes.Role)?.Value,

                    ProjectId = principal.FindFirst("ProjectId")?.Value
                };

                return userClaim;
            }
            catch (Exception ex)
            {
                return null; // إذا فشل فك التوكن أو كان غير صالح
            }
        }



    }


    public class CodeIdentifier
    {
        /// <summary>
        /// The unique identifier of the code (e.g., a GUID string).
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The file path where the code is or should be stored.
        /// </summary>
        public string? PathFile { get; set; }

        // You could add a constructor if needed, but object initializer syntax is common.
        // public CodeIdentifier(string? id = null, string? pathFile = null)
        // {
        //     Id = id;
        //     PathFile = pathFile;
        // }
    }

    public class CodeGeneratorM: CodeGenerator
    {
        public bool IsChanged { get; set; } = false;


        public CodeGeneratorM(CodeGenerator code) {


            // Copy properties from the original CodeGenerator to this new instance
            Id = code.Id;
            Name = code.Name;
            Type = code.Type;
            SubType = code.SubType;
            PathFile = code.PathFile;
            Code = code.Code;
            NamespaceName = code.NamespaceName;
            AdditionalCode = code.AdditionalCode;
            Usings = code.Usings;
            BaseClass = code.BaseClass;
            CreatedAt = code.CreatedAt;
            ProjectId = code.ProjectId;
            Project = code.Project;
            CodeHistories = code.CodeHistories;


        }
    }
    public class InMemoryCodeGeneratorRepository
    {
        // Dictionary key will be the resolved identifier (GUID string or normalized path)
        // Dictionary now stores CodeGenerator directly as requested.
        private readonly ConcurrentDictionary<string, CodeGeneratorM> _codes;

        private readonly string smodels;
        // Static property usage is generally discouraged, consider making this instance-based if possible
        public static string PathModels { get; set; } = string.Empty;

        // Constructor updated to accept ICollection<CodeGenerator>
        public InMemoryCodeGeneratorRepository(ICollection<CodeGenerator>? initialCodes = null, string? pathmodels = null)
        {
            // Handle the smodels part from the original code
            if (!string.IsNullOrWhiteSpace(pathmodels))
            {
                PathModels = pathmodels; // Update static property if provided
                // Use a temporary file name or ensure uniqueness if running multiple instances
                string tempCombineFilePath = Path.Combine(Path.GetTempPath(), "InMemoryCodeRepository_CombinedModels.txt");
                try
                {
                    CombineCSFiles(PathModels, tempCombineFilePath);
                    smodels = File.ReadAllText(tempCombineFilePath);
                    // Clean up the temporary file
                    // File.Delete(tempCombineFilePath); // Keep for debugging if needed
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error combining CS files from {PathModels}: {ex.Message}");
                    smodels = string.Empty; // Initialize to empty on failure
                }
            }
            else
            {
                smodels = string.Empty; // Initialize to empty if no pathmodels provided
            }


            _codes = new ConcurrentDictionary<string, CodeGeneratorM>(StringComparer.OrdinalIgnoreCase); // Case-insensitive key comparison
            Load(initialCodes);


        }


        public void  Load(ICollection<CodeGenerator>? initialCodes)
        {
            foreach (var code in initialCodes ?? Enumerable.Empty<CodeGenerator>())
            {



                var codegenerator = new CodeGeneratorM(code);

                // Add it to the dictionary using the resolved identifier
                _codes.TryAdd(ResolveIdentifierToKey(new CodeIdentifier { Id = code.Id, PathFile = code.PathFile }) ?? string.Empty, codegenerator);
                // Note: You might want to handle duplicates or existing keys based on your requirements
            }
        }
      

        public string GetSModels()
        {
            return smodels;
        }

        // Static method to combine CS files - Kept as in original
        public static void CombineCSFiles(string folderPath, string outputFilePath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"The specified folder does not exist: {folderPath}");
                // Create an empty output file to avoid errors later if the folder is missing
                try { File.WriteAllText(outputFilePath, string.Empty); } catch { } // Suppress errors on empty file write
                return;
            }

            string[] csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            try
            {
                // Ensure the output directory exists
                string? outputDir = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrWhiteSpace(outputDir) && !Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                using (StreamWriter writer = new StreamWriter(outputFilePath, false, Encoding.UTF8))
                {
                    foreach (string file in csFiles)
                    {
                        writer.WriteLine($"// ===== File: {Path.GetFileName(file)} =====");
                        try
                        {
                            string content = File.ReadAllText(file, Encoding.UTF8);
                            writer.WriteLine(content);
                            writer.WriteLine(); // line break between files
                        }
                        catch (Exception fileEx)
                        {
                            Console.WriteLine($"Error reading file {file}: {fileEx.Message}");
                            writer.WriteLine($"// Error reading file: {fileEx.Message}");
                            writer.WriteLine();
                        }
                    }
                }

                Console.WriteLine($"Combined {csFiles.Length} files into: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error combining files to {outputFilePath}: {ex.Message}");
                // Ensure output file exists even on error
                if (!File.Exists(outputFilePath))
                {
                    try { File.WriteAllText(outputFilePath, string.Empty); } catch { }
                }
            }
        }

        // Resolves an identifier (Id or Path) to the consistent key used in the dictionary
        public string? ResolveIdentifierToKey(CodeIdentifier identifier)
        {
            if (identifier == null) return null;

            // Prefer Id if it looks like a valid GUID
            if (!string.IsNullOrWhiteSpace(identifier.Id) && Guid.TryParse(identifier.Id.Trim(), out _))
            {
                // Convert to lowercase invariant string for consistent keying
                return identifier.Id.Trim().ToLowerInvariant();
            }

            // If Id is not a GUID or is missing, try PathFile
            if (!string.IsNullOrWhiteSpace(identifier.PathFile))
            {
                try
                {
                    // Normalize path to a consistent format (full path, forward slashes, lowercase)
                    return Path.GetFullPath(identifier.PathFile.Trim()).Replace('\\', '/').ToLowerInvariant();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not normalize path '{identifier.PathFile}': {ex.Message}");
                    return null; // Path is invalid
                }
            }

            // No valid identifier found
            return null;
        }


        // Add or update a CodeGenerator in the repository
        // Accepts and stores CodeGenerator directly, returns the added/updated CodeGenerator
        public CodeGeneratorM AddOrUpdate(CodeGeneratorM generator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            // Use Id or PathFile to create a CodeIdentifier for key resolution
            var identifier = new CodeIdentifier { Id = generator.Id, PathFile = generator.PathFile };
            string? key = ResolveIdentifierToKey(identifier);

            if (string.IsNullOrWhiteSpace(key))
            {
                // If CodeGenerator has neither a valid Id nor a valid PathFile, it cannot be keyed.
                throw new ArgumentException($"CodeGenerator '{generator.Name ?? "Unnamed"}' (Id: {generator.Id}, Path: {generator.PathFile}) must have a valid Id (GUID) or PathFile to be added or updated.", nameof(generator));
            }

            // Normalize PathFile property within the CodeGenerator for internal consistency, if present
            // Note: Normalizing the property on the object itself *might* affect other references
            // if the object is shared. Storing a copy or using CodeVM avoids this.
            // Since we're storing CodeGenerator directly, proceed with caution or normalize path *only* for the key.
            // Let's normalize the property on the object here.
            if (!string.IsNullOrWhiteSpace(generator.PathFile))
            {
                try { generator.PathFile = Path.GetFullPath(generator.PathFile).Replace('\\', '/'); }
                catch { /* Ignore path normalization errors on the property */ }
            }


            // Add or update the CodeGenerator in the dictionary using the resolved key
            // The factory function handles the case where the key already exists
            // Since CodeGenerator has no IsChanged, we cannot mark it here.
            var resultGenerator = _codes.AddOrUpdate(key, generator, (existingKey, existingVal) =>
            {
                // This block is executed if the key already exists (it's an update)
                // We are replacing the existing object reference with the new 'generator' object.
                // If you wanted to merge properties instead of replacing, you would copy
                // properties from 'generator' to 'existingVal' here.
                // Example (if merging):
                // existingVal.Name = generator.Name;
                // existingVal.Code = generator.Code;
                // ... and so on for all properties you want to update.
                // For simplicity, we will replace the object reference:
                Console.WriteLine($"Updating key '{key}'. Replacing existing object reference.");
                return generator; // Replace existing item with the new one
            });
            resultGenerator.IsChanged = true;
            // The resultGenerator is the object that was added or updated in the dictionary.
            return resultGenerator;
        }

        // save all changes to the file where PathFile is defined
        // IMPORTANT: Without IsChanged, this will attempt to save ALL codes with a PathFile.
        public void SaveChanges()
        {
            Console.WriteLine("SaveChanges called. Attempting to save all codes with a defined PathFile...");
            foreach (var code in _codes.Values) // Iterating over CodeGenerator objects
            {
                // IMPORTANT: We can no longer check an IsChanged flag on the CodeGenerator object.
                // We must assume any code with a PathFile *might* need saving, or implement
                // a separate tracking mechanism. The simplest approach (implemented here)
                // is to attempt to save everything that *can* be saved (has a PathFile).
                if (!string.IsNullOrWhiteSpace(code.PathFile))
                {
                    try
                    {
                        // Note: Passing code.Code and code.PathFile directly.
                        CodeSaveValidator.ValidationCode(code.Code ?? string.Empty);
                        // We cannot set an IsChanged flag on 'code' because CodeGenerator doesn't have one.
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving code to file {code.PathFile}: {ex.Message}");
                        // The object remains in the dictionary. No way to mark it as failed-to-save.
                    }
                }
            }
            Console.WriteLine("SaveChanges finished.");
        }

        // Try to get a CodeGenerator by ID (GUID string) or PathFile string
        public bool TryGet(string identifierString, out CodeGeneratorM? generator)
        {
            if (string.IsNullOrWhiteSpace(identifierString))
            {
                generator = null;
                return false;
            }

            // Create an identifier object from the input string
            CodeIdentifier identifier;
            if (Guid.TryParse(identifierString.Trim(), out _))
            {
                // If it looks like a GUID, treat it as an Id
                identifier = new CodeIdentifier { Id = identifierString.Trim() };
            }
            else
            {
                // Otherwise, treat it as a PathFile
                identifier = new CodeIdentifier { PathFile = identifierString.Trim() };
            }

            // Resolve the string identifier to a potential dictionary key
            string? lookupKey = ResolveIdentifierToKey(identifier);

            if (string.IsNullOrWhiteSpace(lookupKey))
            {
                generator = null;
                return false; // Could not resolve the identifier to a valid key
            }

            // Try to get the CodeGenerator using the resolved key
            return _codes.TryGetValue(lookupKey, out generator);
        }

        // Try to remove a CodeGenerator by ID (GUID string) or PathFile string
        public bool TryRemove(string identifierString, out CodeGeneratorM? removedGenerator)
        {
            if (string.IsNullOrWhiteSpace(identifierString))
            {
                removedGenerator = null;
                return false;
            }

            // Create an identifier object from the input string
            CodeIdentifier identifier;
            if (Guid.TryParse(identifierString.Trim(), out _))
            {
                // If it looks like a GUID, treat it as an Id
                identifier = new CodeIdentifier { Id = identifierString.Trim() };
            }
            else
            {
                // Otherwise, treat it as a PathFile
                identifier = new CodeIdentifier { PathFile = identifierString.Trim() };
            }

            // Resolve the string identifier to a potential dictionary key
            string? lookupKey = ResolveIdentifierToKey(identifier);

            if (string.IsNullOrWhiteSpace(lookupKey))
            {
                removedGenerator = null;
                return false; // Could not resolve the identifier to a valid key
            }

            // Try to remove the CodeGenerator using the resolved key
            return _codes.TryRemove(lookupKey, out removedGenerator);
        }


        // Get all CodeGenerator objects
        public IEnumerable<CodeGeneratorM> GetAll()
        {
            // Return all CodeGenerator objects directly
            return _codes.Values.AsEnumerable();
        }


        public IEnumerable<CodeGenerator> GetCodesChanged()
        {
            // Filter CodeGenerator objects that are marked as changed
            return _codes.Values.Where(gen => gen.IsChanged);
        }

        // Get CodeGenerator objects by CodeGenerator.Name
        public IEnumerable<CodeGeneratorM> GetByType(string name) // Renamed parameter from 'type' to 'name' for clarity based on usage (c.Name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Enumerable.Empty<CodeGeneratorM>();
            // Filter CodeGenerator objects by Name
            return _codes.Values
                         .Where(gen => gen.Type?.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase) ?? false);
        }

        // Get CodeGenerator objects where Name is "ValidatorGenerator"
        public IEnumerable<CodeGeneratorM> GetValidatorCodes()
        {
            // Filter CodeGenerator objects
            return _codes.Values
                         .Where(gen => gen.Type?.Equals("ValidatorGenerator", StringComparison.OrdinalIgnoreCase) ?? false);
        }

        // Get CodeGenerator objects where Name matches the name of a given T type
        // Assuming T is just a marker type whose name is used for filtering CodeGenerator.Name.
        public IEnumerable<CodeGeneratorM> GetCodes<T>()
        {
            var typeName = typeof(T).Name;
            // Filter CodeGenerator objects by Name
            return _codes.Values
                          .Where(gen => gen.Type?.Equals(typeName, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        // Gets CodeGenerator objects that represent validator contexts based on code content
        public IEnumerable<CodeGeneratorM> GetValdatorContexts()
        {
            // Assuming CodeGenerator.Code contains the necessary content for this check
            // Filter CodeGenerator objects based on Code content
            return _codes.Values
                         .Where(gen => gen.Code != null && gen.Code.Contains("ValidatorContext : ValidatorContext<"));
        }

        // Combines the Code property of all CodeGenerators matching a given T type name
        public string GetRawCodes<T>()
        {
            var typeName = typeof(T).Name;
            var codes = GetCodes<T>(); // Use the method that returns CodeGenerator
            if (codes == null || !codes.Any()) return string.Empty;

            var sb = new StringBuilder();
            foreach (var code in codes) // Iterating over CodeGenerator objects
            {
                sb.AppendLine($"// --- Code for: {code.Name} ({code.Id}) ---");
                sb.AppendLine($"// Path: {code.PathFile}");
                sb.AppendLine(code.Code);
                sb.AppendLine("==========================="); // Separator
                sb.AppendLine(); // Extra line break
            }
            return sb.ToString();
        }
    }
}