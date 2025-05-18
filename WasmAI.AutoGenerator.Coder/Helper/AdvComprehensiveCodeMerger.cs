using GenerativeAI;
using GenerativeAI.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class advGeminiCodeService
{
    private readonly GoogleAi _googleAI;
    private readonly GenerativeModel _generativeModel;
    private readonly List<Content> _contents; // Consider thread safety if used concurrently

    public advGeminiCodeService(string apiKey /* Consider secure configuration */ = "YOUR_DEFAULT_API_KEY") // Changed default key
    {
        // IMPORTANT: Avoid hardcoding API keys in production. Use configuration or environment variables.
        _googleAI = new GoogleAi(apiKey);
        _generativeModel = _googleAI.CreateGenerativeModel("models/gemini-1.5-flash");

        // Initial instruction for the model
        string instructionText = @"You are a C# code generation expert.
Your primary goal is to generate clean, functional, and production-ready C# code based on the provided model structure and instructions.
Always return ONLY the raw C# code. Do not include any extra text, explanations, comments outside of the code itself, or markdown code fences (```csharp / ```).";

        _contents = new List<Content>()
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

    // ... (Other methods like ExtractNotes, MergeCodesAsync2, GenerateCodeAsync, MergeCodesAsync remain here)

    // Helper to clean code fences (can be applied to all generation methods)
    private string StripCodeFences(string responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText)) return string.Empty;
        var cleaned = responseText.Replace("```csharp", "").Replace("```", "").Trim();
        // Also handle potential extra newlines if necessary
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"^\s*$", "", System.Text.RegularExpressions.RegexOptions.Multiline);
        return cleaned;
    }


    /// <summary>
    /// Generates a Validator class for a given C# model structure based on a template pattern using Gemini.
    /// </summary>
    /// <param name="modelName">The name of the model class (e.g., "Folder").</param>
    /// <param name="modelStructure">The C# code definition of the model class.</param>
    /// <param name="templateInstructions">Specific instructions on validation rules to apply based on the model properties.</param>
    /// <returns>The generated C# validator code as a string.</returns>
    public async Task<string> GenerateValidatorFromModelAsync(string modelName, string modelStructure, string templateInstructions)
    {
        // Crafting the specific prompt for validator generation based on the user's template
        string prompt = $@"
Generate a C# Validator class for the model '{modelName}' with the following structure:

Model Name: {modelName}

Model Structure (C# Class Definition):
{modelStructure}

Validator Template Pattern Requirements:
- The class must be named '{modelName}ValidatorContext'.
- It must inherit from 'ValidatorContext<{modelName}, {modelName}ValidatorStates>'.
- It must implement 'ITValidator'.
- It needs a constructor taking 'IConditionChecker checker'.
- Generate a public enum named '{modelName}ValidatorStates'. This enum should contain a state entry for EACH PUBLIC PROPERTY in the model structure (e.g., 'HasPropertyName').
- For EACH PUBLIC PROPERTY in the Model Structure, create a corresponding private async method to perform validation.
    - The method signature should be 'private async Task<ConditionResult> ValidatePropertyName(DataFilter<PropertyType, {modelName}> f)'. Use the actual PropertyType from the Model Structure.
    - Each validation method must use the '[RegisterConditionValidator(typeof({modelName}ValidatorStates), {modelName}ValidatorStates.HasPropertyName, ""Error message"")]' attribute.
    - The 'InitializeConditions' method should register each validation method using 'RegisterCondition({modelName}ValidatorStates.HasPropertyName, ValidatePropertyName);'.
- Implement the 'GetModel' protected async method, ensuring it correctly handles caching if needed, similar to the provided template example.
- Apply the following specific validation rules based on property types, as detailed in Template Instructions:
{templateInstructions}

Your response MUST be ONLY the C# code for the '{modelName}ValidatorContext' class, including the enum, using statements (if necessary based on common patterns like AutoGenerator.Conditions), constructors, methods, and attributes. Do NOT include any surrounding text, explanations, or markdown.

Example Validation Logic Based on Common Types (Guideline for AI):
- string properties: Check for null or whitespace.
- string properties (potential URLs): Use Uri.TryCreate.
- bool properties: Often just check if the property exists or is default.
- Collection properties (like ICollection<FileEntity>): Check if the collection is null or empty if required, or validate each item within the collection based on specific instructions.

Ensure all generated code adheres to the specified template pattern and the Template Instructions provided below.

Template Instructions:
{templateInstructions}
";

        // Add the crafted prompt to the chat history and generate the response
        _contents.Add(new Content()
        {
            Role = "user",
            Parts = new List<Part>() { new Part() { Text = prompt } }
        });

        // Start a new chat session or continue the existing one.
        // Using a new session might give a cleaner slate for each generation,
        // or continuing might provide better context if subsequent generations
        // depend on previous ones (less likely for independent validators).
        // Let's stick to starting a new one for clarity per generation request.
        var chatSession = _generativeModel.StartChat(new List<Content>(_contents)); // Clone contents to avoid side effects on the shared _contents list if used concurrently
        var response = await chatSession.GenerateContentAsync("");

        // Clean the response from markdown and extra spaces
        var generatedCode = StripCodeFences(response.Text);

        // Optional: Log the prompt and response for debugging
        // Console.WriteLine("--- PROMPT ---");
        // Console.WriteLine(prompt);
        // Console.WriteLine("--- RESPONSE ---");
        // Console.WriteLine(generatedCode);

        return generatedCode;
    }

    // ... (Add or modify other methods like GenerateCodeFromTextAsync if needed, applying StripCodeFences)
    public async Task<string> GenerateCodeFromTextAsync(string description)
    {
        var instruction = @"You are a C# code generation expert.
Your task is to write C# code based on the user's text description.
Only return the C# code, no comments, no explanation, and no extra symbols.";

        // Create a new list of contents for this specific request to avoid state issues
        var requestContents = new List<Content>(_contents);
        requestContents.Add(new Content()
        {
            Role = "user",
            Parts = new List<Part>()
            {
                new Part() { Text = instruction },
                new Part() { Text = description }
            }
        });

        var chatSession = _generativeModel.StartChat(requestContents); // Use request-specific contents
        var response = await chatSession.GenerateContentAsync("");

        // تنظيف النتائج من أي علامات لغة أو تعليمات تنسيق
        string generatedCode = StripCodeFences(response.Text);
        return generatedCode;
    }
}