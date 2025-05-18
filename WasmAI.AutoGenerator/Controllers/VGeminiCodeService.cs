using GenerativeAI;
using GenerativeAI.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoGenerator.Code.Services
{
    public class VGeminiCodeService
    {
        private readonly GoogleAi _googleAI;
        private readonly GenerativeModel _generativeModel;
        private readonly List<Content> _initialContents;

        public VGeminiCodeService(string apiKey = "AIzaSyALy9VxGquNEppI_YtSjPbKK06JyKALnT4")
        {
            _googleAI = new GoogleAi(apiKey);
            _generativeModel = _googleAI.CreateGenerativeModel("models/gemini-1.5-flash");

            string instructionText = @"You are a C# code generation expert.
Your primary goal is to generate clean, functional, and production-ready C# code based on the user's request.
Always return ONLY the raw C# code. Do not include any extra text, explanations, comments outside of the code itself, or markdown code fences (```csharp / ```).";

            _initialContents = new List<Content>()
            {
                new Content()
                {
                    Role = "user",
                    Parts = new List<Part>()
                    {
                        new Part() { Text = instructionText }
                    }
                }
            };
        }

        private string ExtractNotes(string code)
        {
            var notes = new List<string>();
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;
            foreach (var line in code.Split('\n'))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("//&&") && trimmed.EndsWith("//"))
                {
                    notes.Add(trimmed.Substring(4, trimmed.Length - 6).Trim());
                }
            }
            return string.Join("\n", notes);
        }

        private string StripCodeFences(string responseText)
        {
            if (string.IsNullOrWhiteSpace(responseText)) return string.Empty;
            var cleaned = responseText.Replace("```csharp", "").Replace("```", "").Trim();
            cleaned = Regex.Replace(cleaned, @"^\s*$", "", RegexOptions.Multiline);
            return cleaned;
        }

        private bool IsValueType(string typeName)
        {
            if (typeName.EndsWith("?")) return false;
            switch (typeName.Trim())
            {
                case "bool":
                case "byte":
                case "sbyte":
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                case "float":
                case "double":
                case "decimal":
                case "char":
                case "DateTime":
                case "DateTimeOffset":
                case "TimeSpan":
                case "Guid":
                    return true;
                default:
                    return false;
            }
        }

        public async Task<string> MergeCodesAsync2(string code1, string code2)
        {
            string notesCombined = string.Join("\n", new[] { ExtractNotes(code1), ExtractNotes(code2) }.Where(n => !string.IsNullOrWhiteSpace(n)));

            string prompt = string.IsNullOrWhiteSpace(notesCombined)
                ? @$"
You will be given two separate C# code snippets.
Your task is to intelligently merge them into a single clean, optimized, and functional C# code block.
Do not repeat duplicate functionality.
Preserve all important logic from both.
Only return the final merged code — no extra text or symbols.

First Code:
{code1}

Second Code:
{code2}"
                : @$"
You will be given two separate C# code snippets and additional notes embedded inside special comment markers //&&...//. These notes are important and should guide how the code should be merged.

Your task is to intelligently merge the codes into a single clean, optimized, and functional C# code block.
Follow the instructions provided inside the notes.
Do not repeat duplicate functionality. Preserve important logic from both.

Important rules:
- If two methods have the same route attribute (e.g., [HttpGet(""{{id}}"", Name = ""..."")]), prefer the one from the **second code** (newer).
- Follow all instructions provided inside the notes.
- Do not repeat duplicate functionality.
- Preserve important logic from both codes.
- Only return the final merged code — no extra text or symbols.

Developer Notes:
{notesCombined}

First Code:
{code1}

Second Code:
{code2}";

            var requestContents = new List<Content>(_initialContents);
            requestContents.Add(new Content() { Role = "user", Parts = new List<Part>() { new Part() { Text = prompt } } });
            var chatSession = _generativeModel.StartChat(requestContents);
            var response = await chatSession.GenerateContentAsync("");
            return StripCodeFences(response.Text);
        }

        public async Task<string> GenerateCodeAsync(string codeRequest)
        {
            var requestContents = new List<Content>(_initialContents);
            var taskInstruction = @"Generate C# code based on the following detailed description.
Return only the raw C# code.";
            requestContents.Add(new Content() { Role = "user", Parts = new List<Part>() { new Part() { Text = taskInstruction }, new Part() { Text = codeRequest } } });

            var chatSession = _generativeModel.StartChat(requestContents);
            var response = await chatSession.GenerateContentAsync("");
            return StripCodeFences(response.Text);
        }

        public async Task<string> MergeCodesAsync(string code1, string code2)
        {
            var requestContents = new List<Content>(_initialContents);
            requestContents.Add(new Content()
            {
                Role = "user",
                Parts = new List<Part>()
                {
                    new Part() { Text = @$"
You will be given two separate C# code snippets.
Your task is to intelligently merge them into a single clean, optimized, and functional C# code block.
Do not repeat duplicate functionality.
Preserve all important logic from both.
Only return the final merged code — no extra text or symbols.

First Code:
{code1}

Second Code:
{code2}" }
                }
            });

            var chatSession = _generativeModel.StartChat(requestContents);
            var response = await chatSession.GenerateContentAsync("");
            return StripCodeFences(response.Text);
        }

        public async Task<string> GenerateValidatorFromModelAsync(string modelName, string modelStructure, string templateInstructions)
        {
            string prompt = $@"
You are an expert C# backend engineer specializing in code generation and validator patterns.
Given the following:

Model Name:
{modelName}

Model Structure (C# Class Definition):
{modelStructure}

Validator Template Pattern Requirements:
{templateInstructions}

Your task is to:
- Generate a complete C# validator class named '{modelName}ValidatorContext'.
- Include the corresponding public enum '{modelName}ValidatorStates'.
- Implement the constructor, GetModel method, and validation methods exactly as described in the instructions.
- Register condition validators using attributes.
- Follow best practices in naming and structure.
- Return only valid C# code without explanation, comments, or markdown formatting.

IMPORTANT:
- Use async methods for validation where logic requires async operations (like GetModel) or simply return Task.FromResult for synchronous logic. Follow the signature requirement.
- Include necessary using statements.
- Do not add any boilerplate explanation or notes.
";

            var requestContents = new List<Content>(_initialContents);
            requestContents.Add(new Content() { Role = "user", Parts = new List<Part>() { new Part() { Text = prompt } } });
            var chatSession = _generativeModel.StartChat(requestContents);
            var response = await chatSession.GenerateContentAsync("");
            return StripCodeFences(response.Text);
        }


        public async Task<string> GenerateValidatorCodeForModelAsync(string modelStructure)
        {
            string modelName = "";
            List<(string Type, string Name)> properties = new List<(string Type, string Name)>();

            var classNameMatch = Regex.Match(modelStructure, @"\s*(\w+)\s+class\s+(\w+)", RegexOptions.Singleline);
            if (classNameMatch.Success && classNameMatch.Groups.Count > 2)
            {
                modelName = classNameMatch.Groups[2].Value;
            }
            else
            {
                classNameMatch = Regex.Match(modelStructure, @"class\s+(\w+)\s*[{|\r|\n]", RegexOptions.Singleline);
                if (classNameMatch.Success && classNameMatch.Groups.Count > 1)
                {
                    modelName = classNameMatch.Groups[1].Value;
                }
                else
                {
                    return "// Error: Could not parse model name from the provided structure.";
                }
            }

            var propertyMatches = Regex.Matches(modelStructure, @"public\s+(\w+|\w+\?|[\w<>,\?\s]+)\s+(\w+)\s*{\s*get;\s*set;\s*}", RegexOptions.Multiline);

            foreach (Match match in propertyMatches)
            {
                string propType = match.Groups[1].Value.Trim();
                string propName = match.Groups[2].Value.Trim();
                if (propType.Contains(modelName) || propType.EndsWith(modelName))
                {
                    if (!propType.StartsWith("ICollection<", StringComparison.OrdinalIgnoreCase) && !propType.StartsWith("List<", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }
                if (IsValueType(propType) && !propType.EndsWith("?"))
                {
                    continue;
                }
                properties.Add((propType, propName));
            }

            if (!properties.Any() && modelName == "")
            {
                return "// Error: Could not parse model name or any public properties from the provided structure.";
            }

            StringBuilder instructionsBuilder = new StringBuilder();
            instructionsBuilder.AppendLine($"Generate a C# Validator class named '{modelName}ValidatorContext' for the '{modelName}' model.");
            instructionsBuilder.AppendLine($"The class must inherit from 'ValidatorContext<{modelName}, {modelName}ValidatorStates>' and implement 'ITValidator'.");
            instructionsBuilder.AppendLine("Include a constructor 'public " + modelName + "ValidatorContext(IConditionChecker checker) : base(checker)'.");

            string modelFieldName = "_" + char.ToLowerInvariant(modelName.FirstOrDefault());
            if (modelName.Length > 1) modelFieldName += modelName.Substring(1);
            else if (modelName.Length == 1) modelFieldName = "_" + char.ToLowerInvariant(modelName[0]);
            else modelFieldName = "_model";

            instructionsBuilder.AppendLine($"Include a private {modelName}? {modelFieldName} field for caching.");
            instructionsBuilder.AppendLine($"Include the protected override async Task<{modelName}?> GetModel(string? id) method with caching logic using the private field '{modelFieldName}'.");
            instructionsBuilder.AppendLine("Ensure the InitializeConditions method is empty, as registration is via attributes.");
            instructionsBuilder.AppendLine();

            instructionsBuilder.AppendLine($"Generate a public enum named '{modelName}ValidatorStates'. This enum must contain a state entry for EACH PUBLIC PROPERTY that has a generated validation rule below.");
            instructionsBuilder.AppendLine("Follow the pattern 'HasPropertyName' for most properties.");
            instructionsBuilder.AppendLine("For boolean properties, use the pattern 'IsPropertyName'.");
            instructionsBuilder.AppendLine("Include a generic state 'IsFound' for checking model existence by Id.");
            instructionsBuilder.AppendLine();

            instructionsBuilder.AppendLine("Generate validation methods and attributes based on the following rules:");

            instructionsBuilder.AppendLine($"- State: IsFound");
            instructionsBuilder.AppendLine($"  - Corresponding Enum State: IsFound");
            instructionsBuilder.AppendLine($"  - Validation Method Name: Validate{modelName}Found");
            instructionsBuilder.AppendLine($"  - DataFilter Type: DataFilter<string, {modelName}>");
            instructionsBuilder.AppendLine($"  - Attribute: '[RegisterConditionValidator(typeof({modelName}ValidatorStates), {modelName}ValidatorStates.IsFound, \"{modelName} is not found\")]'");
            instructionsBuilder.AppendLine($"  - Logic: Check if f.Share is NOT NULL. Return SuccessAsync(f.Share) if valid, FailureAsync(\"{modelName} is not found\") if invalid.");
            instructionsBuilder.AppendLine();

            foreach (var prop in properties)
            {
                string stateName = "";
                string errorMessage = "";
                string validationLogic = "";
                string dataFilterType = prop.Type;
                bool generateValidator = false;
                string validateMethodName = $"Validate{prop.Name}";

                if (prop.Type.Contains("string", StringComparison.OrdinalIgnoreCase))
                {
                    if (prop.Name.EndsWith("Id"))
                    {
                        stateName = $"Has{prop.Name}";
                        errorMessage = $"{prop.Name} is required.";
                        validationLogic = $"bool valid = !string.IsNullOrWhiteSpace(f.Share?.{prop.Name}); return valid ? ConditionResult.ToSuccessAsync(f.Share?.{prop.Name}) : ConditionResult.ToFailureAsync(f.Share?.{prop.Name}, \"{errorMessage}\");";
                        dataFilterType = "string";
                        generateValidator = true;
                    }
                    else if (prop.Name.Contains("Url") || prop.Name.Contains("Path"))
                    {
                        stateName = $"HasValid{prop.Name}";
                        errorMessage = $"{prop.Name} is invalid or missing.";
                        validationLogic = $"bool valid = Uri.TryCreate(f.Share?.{prop.Name}, UriKind.Absolute, out _); return valid ? ConditionResult.ToSuccessAsync(f.Share?.{prop.Name}) : ConditionResult.ToFailureAsync(f.Share?.{prop.Name}, \"{errorMessage}\");";
                        dataFilterType = "string";
                        generateValidator = true;
                    }
                    else if (prop.Name == "Token")
                    {
                        stateName = $"Has{prop.Name}IfExists";
                        errorMessage = $"{prop.Name} cannot be empty if provided.";
                        validationLogic = $"var token = f.Share?.{prop.Name};\nbool valid = token == null || !string.IsNullOrWhiteSpace(token); return valid ? ConditionResult.ToSuccessAsync(token) : ConditionResult.ToFailureAsync(\"{errorMessage}\");";
                        dataFilterType = "string?";
                        generateValidator = true;
                    }
                    else
                    {
                        stateName = $"Has{prop.Name}";
                        errorMessage = $"{prop.Name} is required.";
                        validationLogic = $"bool valid = !string.IsNullOrWhiteSpace(f.Share?.{prop.Name}); return valid ? ConditionResult.ToSuccessAsync(f.Share?.{prop.Name}) : ConditionResult.ToFailureAsync(f.Share?.{prop.Name}, \"{errorMessage}\");";
                        dataFilterType = "string";
                        generateValidator = true;
                    }
                }
                else if (prop.Type.Contains("bool", StringComparison.OrdinalIgnoreCase))
                {
                    stateName = $"Is{prop.Name}";
                    errorMessage = "";
                    validationLogic = $"return Task.FromResult(ConditionResult.ToSuccess(f.Share?.{prop.Name} ?? false));";
                    dataFilterType = "bool";
                    generateValidator = true;
                }
                else if (prop.Type.StartsWith("ICollection<", StringComparison.OrdinalIgnoreCase) || prop.Type.StartsWith("List<", StringComparison.OrdinalIgnoreCase))
                {
                    var itemTypeMatch = Regex.Match(prop.Type, @"<([\w<>,\?\s]+)>");
                    string itemType = itemTypeMatch.Success ? itemTypeMatch.Groups[1].Value.Trim() : "object";

                    stateName = $"Has{prop.Name}";
                    errorMessage = $"No {prop.Name} defined or collection is empty.";
                    validationLogic = $"bool valid = f.Share?.{prop.Name} != null && f.Share.{prop.Name}.Any(); return valid ? ConditionResult.ToSuccessAsync(f.Share?.{prop.Name}) : ConditionResult.ToFailureAsync(f.Share?.{prop.Name}, \"{errorMessage}\");";
                    dataFilterType = $"object";
                    generateValidator = true;
                }
                else if (prop.Type.Contains("DateTime", StringComparison.OrdinalIgnoreCase) || prop.Type.Contains("TimeSpan", StringComparison.OrdinalIgnoreCase) || prop.Type.Contains("Guid", StringComparison.OrdinalIgnoreCase)
                         || prop.Type.Contains("int", StringComparison.OrdinalIgnoreCase) || prop.Type.Contains("long", StringComparison.OrdinalIgnoreCase) || prop.Type.Contains("decimal", StringComparison.OrdinalIgnoreCase))
                {
                    if (prop.Type.EndsWith("?"))
                    {
                        stateName = $"Has{prop.Name}";
                        errorMessage = $"{prop.Name} is required.";
                        validationLogic = $"bool valid = f.Share?.{prop.Name} != null; return valid ? ConditionResult.ToSuccessAsync(f.Share?.{prop.Name}) : ConditionResult.ToFailureAsync(\"{errorMessage}\");";
                        dataFilterType = prop.Type;
                        generateValidator = true;
                    }
                }

                if (generateValidator)
                {
                    instructionsBuilder.AppendLine($"- Property: {prop.Name} (Type: {prop.Type})");
                    instructionsBuilder.AppendLine($"  - Corresponding Enum State: {stateName}");
                    instructionsBuilder.AppendLine($"  - Validation Method Name: {validateMethodName}");
                    instructionsBuilder.AppendLine($"  - DataFilter Type: DataFilter<{dataFilterType}, {modelName}>");
                    instructionsBuilder.AppendLine($"  - Attribute: '[RegisterConditionValidator(typeof({modelName}ValidatorStates), {modelName}ValidatorStates.{stateName}, \"{errorMessage}\")]'");
                    instructionsBuilder.AppendLine($"  - Logic: Implement validation logic: {validationLogic.Replace("\n", " ").Trim()}");
                    instructionsBuilder.AppendLine();
                }
            }

            instructionsBuilder.AppendLine("Ensure all generated code adheres to the specified template pattern.");
            instructionsBuilder.AppendLine("Include necessary using statements (e.g., AutoGenerator.Conditions, V1.Validators.Conditions, the model's namespace, System, System.Threading.Tasks, System.Linq, System.Collections.Generic, System.Text, System.Text.RegularExpressions).");
            instructionsBuilder.AppendLine("// Assume the model's namespace is accessible (e.g., using Api.SM.Models;)");
            instructionsBuilder.AppendLine("Your response MUST be ONLY the complete C# code for the '" + modelName + "ValidatorContext' class and the enum, without any surrounding text or markdown.");

            string finalTemplateInstructions = instructionsBuilder.ToString();

            var requestContents = new List<Content>(_initialContents);
            string prompt = $@"
You are an expert C# backend engineer specializing in code generation and validator patterns.
Given the following:

Model Name:
{modelName}

Model Structure (C# Class Definition):
{modelStructure}

Validator Template Pattern Requirements:
{finalTemplateInstructions}

Your task is to:
- Generate a complete C# validator class named '{modelName}ValidatorContext'.
- Include the corresponding public enum '{modelName}ValidatorStates'.
- Implement the constructor, GetModel method, and validation methods exactly as described in the instructions.
- Register condition validators using attributes.
- Follow best practices in naming and structure.
- Return only valid C# code without explanation, comments, or markdown formatting.

IMPORTANT:
- Use async methods for validation where logic requires async operations (like GetModel) or simply return Task.FromResult for synchronous logic. Follow the signature requirement.
- Include necessary using statements based on the template pattern (e.g., from AutoGenerator, your project's Validator.Conditions).
- Do not add any boilerplate explanation or notes.
";

            requestContents.Add(new Content() { Role = "user", Parts = new List<Part>() { new Part() { Text = prompt } } });
            var chatSession = _generativeModel.StartChat(requestContents);
            var response = await chatSession.GenerateContentAsync("");
            return StripCodeFences(response.Text);
        }

        public async Task<string> GenerateValidatorFromTextWithExamplesAsync(string description, List<string> exampleValidatorCodes)
        {
            StringBuilder promptBuilder = new StringBuilder();
            promptBuilder.AppendLine(@"You are a C# backend engineer specializing in code generation and validator patterns.");
            promptBuilder.AppendLine(@"Your task is to generate a C# Validator class based on the user's description, strictly following the architectural pattern shown in the provided examples.");
            promptBuilder.AppendLine(@"Adhere to the structure using ValidatorContext<TModel, TState>, [RegisterConditionValidator] attributes, async validation methods, and the standard naming conventions observed in the examples.");
            promptBuilder.AppendLine(@"Return ONLY the raw C# code for the validator class and its corresponding enum. Do not include any extra text, explanations, comments outside of the code itself, or markdown code fences (```csharp / ```).");
            promptBuilder.AppendLine();

            if (exampleValidatorCodes != null && exampleValidatorCodes.Any())
            {
                promptBuilder.AppendLine($"Review these {exampleValidatorCodes.Count} examples of the desired Validator architectural pattern:");
                promptBuilder.AppendLine("--- Examples ---");
                foreach (var exampleCode in exampleValidatorCodes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList())
                {
                    promptBuilder.AppendLine(exampleCode);
                    promptBuilder.AppendLine("--- End Example ---");
                }
                promptBuilder.AppendLine("----------------");
                promptBuilder.AppendLine();
            }
            else
            {
                promptBuilder.AppendLine("No specific examples provided. Generate a validator following the general ValidatorContext pattern:");
                promptBuilder.AppendLine("- Class inheriting ValidatorContext<TModel, TState> and ITValidator.");
                promptBuilder.AppendLine("- Public enum TState.");
                promptBuilder.AppendLine("- Constructor taking IConditionChecker.");
                promptBuilder.AppendLine("- Use [RegisterConditionValidator] attributes for validation methods.");
                promptBuilder.AppendLine("- Validation methods should be private async Task<ConditionResult> and take DataFilter<TProp, TModel>.");
                promptBuilder.AppendLine("- Include a GetModel override if applicable.");
                promptBuilder.AppendLine("- Infer the model name (TModel) from the description if possible, otherwise use a placeholder like 'GenericModel'.");
                promptBuilder.AppendLine();
            }

            promptBuilder.AppendLine("Generate a C# Validator class based on the following description:");
            promptBuilder.AppendLine("--- Validator Description ---");
            promptBuilder.AppendLine(description);
            promptBuilder.AppendLine("---------------------------");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("IMPORTANT:");
            promptBuilder.AppendLine("- Return ONLY the complete C# code for the validator class and its enum.");
            promptBuilder.AppendLine("- Include necessary using statements (infer from common patterns and the template, e.g., AutoGenerator.Conditions, AutoGenerator).");
            promptBuilder.AppendLine("- Follow the template structure (ValidatorContext, enum, attributes, async methods, DataFilter signature).");
            promptBuilder.AppendLine("- Do NOT include any surrounding text, explanations, or markdown formatting.");
            promptBuilder.AppendLine("- Ensure the generated code is syntactically correct C#.");
            promptBuilder.AppendLine("- Do NOT generate any example usage or Main method.");

            string prompt = promptBuilder.ToString();

            var requestContents = new List<Content>(_initialContents);
            requestContents.Add(new Content() { Role = "user", Parts = new List<Part>() { new Part() { Text = prompt } } });
            var chatSession = _generativeModel.StartChat(requestContents);
            var response = await chatSession.GenerateContentAsync("");
            return StripCodeFences(response.Text);
        }
    }
}