// ===== File: GenerationOptions.cs =====
using AutoGenerator.Code;
using Microsoft.CodeAnalysis.CSharp.Syntax; // Added for TypeSyntax
using System.Reflection;
using System; // Added for Type
using System.Collections.Generic; // Added for List

// Ensure this is in the correct namespace
namespace AutoGenerator.Code.v1;

public class GenerationOptions
{
    public string ClassName { get; set; }
    public Type SourceType { get; } // Represents the source type (e.g., a Model) driving generation
    public string NamespaceName { get; set; } = "GeneratedClasses"; // The target namespace for the generated code
    public string AdditionalCode { get; set; } = ""; // Any extra code snippets to be included (e.g., manual members)

    // Store types for interfaces the generated type should implement
    public List<Type> Interfaces { get; set; } = new List<Type>();

    // Store the Type of the base class the generated type should inherit from
    public Type BaseClassType { get; set; } = null;

    // List of properties from the SourceType. The generator decides how to use these.
    public PropertyInfo[] Properties { get; set; }

    // --- Validation Expectations ---
    // List of member identities (string representation like "MethodName!(MethodDeclaration)" )
    // expected to be found directly within the main generated type.
    // Populated by specific Generators based on their templates/logic.
    public List<string> ExpectedMemberIdentities { get; set; } = new List<string>();

    // List of TypeSyntax identities (string representation like "BaseType<T>")
    // expected to be found in the BaseList of the main generated type.
    // Populated by specific Generators based on BaseClassType and Interfaces.
    public List<string> ExpectedBaseListTypeIdentities { get; set; } = new List<string>();

    // --- Template Configuration ---
    // List of using directives (namespace strings) to be included
    public List<string> Usings { get; set; } = new List<string>();

    // The template string used by BaseGenerator.
    // Should include placeholders like {ClassName}, {NamespaceName}, {BaseList}, {Properties}, {AdditionalCode}.
    public string Template { get; set; } = @"
// Default Template

using System; // Default using if not in options.Usings

namespace {NamespaceName}
{{
    public class {ClassName} {BaseList}
    {{
        {Properties} // Placeholder for properties added by BaseGenerator or specific generator
        {AdditionalCode} // Placeholder for additional code (like manually generated members)
    }}
}}
    ";

    /// <summary>
    /// Initializes a new instance of the GenerationOptions class.
    /// </summary>
    /// <param name="className">The desired name for the main generated class/type.</param>
    /// <param name="sourceType">The source Type (e.g., a Model) used to drive generation.</param>
    /// <param name="isProperties">If true, properties from the SourceType are automatically retrieved and stored in the Properties array.</param>
    public GenerationOptions(string className, Type sourceType, bool isProperties = true)
    {
        ClassName = className;
        SourceType = sourceType;

        if (isProperties && sourceType != null) // Add null check for sourceType
        {
            Properties = sourceType.GetProperties();
            // Note: Identities for these properties should be added by the specific generator
            // if they are generated via AdditionalCode/Template, or by BaseGenerator
            // if it adds them based on this Properties list (which it currently doesn't do beyond the string replacement).
            // Let's assume specific generators handle adding these identities when they decide to generate properties.
        }
        else
        {
            Properties = new PropertyInfo[0]; // Ensure Properties is not null
        }
    }

    // Note: The 'BaseClass' string property is now deprecated in favor of 'BaseClassType' Type property.
    // The template placeholder '{BaseClass}' is still supported for backward compatibility
    // but '{BaseList}' which includes both base class and interfaces is preferred in newer templates.
    // If using BaseClassType, the Template property should use {BaseList}.
    // Keeping BaseClass string property for compatibility if older templates use it directly.
    // public string BaseClass { get; set; } = null; // Deprecated in favor of BaseClassType, kept for template compatibility
}