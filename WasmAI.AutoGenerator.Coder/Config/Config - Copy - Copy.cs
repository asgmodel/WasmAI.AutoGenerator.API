
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoGenerator.Code.v1;


public class CodeSyntaxTreeValidator
{
    private CodeValidationResult _result;
    private GenerationOptions _options; // Store options for specific validation
    private SyntaxTree _syntaxTree; // Store tree to get diagnostics

    /// <summary>
    /// Validates a syntax tree against general C# rules and optionally against GenerationOptions.
    /// </summary>
    /// <param name="tree">The syntax tree to validate.</param>
    /// <param name="options">Optional generation options used to create this code (for specific checks).</param>
    /// <returns>A CodeValidationResult detailing errors and warnings.</returns>
    public CodeValidationResult Validate(SyntaxTree tree, GenerationOptions options = null)
    {
        _result = new CodeValidationResult();
        _options = options; // Store options
        _syntaxTree = tree; // Store tree
        if (tree == null)
        {
            _result.AddError("Cannot validate a null syntax tree.");
            return _result;
        }

        var root = tree.GetRoot();

        // --- Pass 1: Roslyn's Built-in Diagnostics ---
        // Capture syntax errors and initial warnings found by the parser
        var diagnostics = tree.GetDiagnostics().Where(d => d.Severity >= DiagnosticSeverity.Warning).ToList();
        foreach (var diagnostic in diagnostics)
        {
            var message = $"Roslyn Diagnostic ({diagnostic.Severity}): {diagnostic.GetMessage()} at {diagnostic.Location.GetLineSpan()}";
            if (diagnostic.Severity == DiagnosticSeverity.Error)
                _result.AddError(message);
            else if (diagnostic.Severity == DiagnosticSeverity.Warning)
                _result.AddWarning(message);
        }

        // If there are critical Roslyn syntax errors, further structural validation might be unreliable.
        // You might choose to stop here if _result.IsValid is false after this step.
        if (!_result.IsValid && _result.Errors.Any()) // Check specifically for errors
        {
            // Optional: Stop here if basic syntax is broken
            // return _result;
        }


        // --- Pass 2: General Structural Validation (Manual) ---
        // Checks for logical structural issues not caught by basic syntax parsing
        ValidateNode(root); // Recursive structural checks


        // --- Pass 3: Specific Generated Code Validation (if options provided) ---
        if (_options != null)
        {
            ValidateSpecificGeneratedCode(root);
        }

        return _result;
    }

    // --- General Syntax Tree Validation (Recursive) ---
    // Checks for common structural issues across any C# code
    private void ValidateNode(SyntaxNode node)
    {
        if (node == null) return;

        // Apply specific validation logic based on node type (SyntaxKind)
        switch (node.Kind())
        {
            case SyntaxKind.CompilationUnit:
                // Handled by explicit call below to ensure members are recursed
                break;
            case SyntaxKind.NamespaceDeclaration:
                ValidateNamespaceDeclaration((NamespaceDeclarationSyntax)node);
                // Recursion handled inside ValidateNamespaceDeclaration
                return;
            case SyntaxKind.ClassDeclaration:
            case SyntaxKind.StructDeclaration:
            case SyntaxKind.InterfaceDeclaration:
            case SyntaxKind.RecordDeclaration:
            case SyntaxKind.EnumDeclaration:
                ValidateTypeDeclaration((BaseTypeDeclarationSyntax)node);
                // Recursion handled inside ValidateTypeDeclaration
                return;
            case SyntaxKind.MethodDeclaration:
                ValidateMethodDeclaration((MethodDeclarationSyntax)node);
                break;
            case SyntaxKind.PropertyDeclaration:
                ValidatePropertyDeclaration((PropertyDeclarationSyntax)node);
                break;
            case SyntaxKind.FieldDeclaration:
                ValidateFieldDeclaration((FieldDeclarationSyntax)node);
                break;
            case SyntaxKind.EnumMemberDeclaration:
                ValidateEnumMemberDeclaration((EnumMemberDeclarationSyntax)node);
                break;
            case SyntaxKind.DelegateDeclaration:
                ValidateDelegateDeclaration((DelegateDeclarationSyntax)node);
                break;
            case SyntaxKind.ConstructorDeclaration:
                ValidateConstructorDeclaration((ConstructorDeclarationSyntax)node);
                break;
            case SyntaxKind.IndexerDeclaration:
                ValidateIndexerDeclaration((IndexerDeclarationSyntax)node);
                break;
            case SyntaxKind.OperatorDeclaration:
                ValidateOperatorDeclaration((OperatorDeclarationSyntax)node);
                break;
            case SyntaxKind.ConversionOperatorDeclaration:
                ValidateConversionOperatorDeclaration((ConversionOperatorDeclarationSyntax)node);
                break;
            case SyntaxKind.EventDeclaration:
                ValidateEventDeclaration((EventDeclarationSyntax)node);
                break;
            case SyntaxKind.EventFieldDeclaration:
                ValidateEventFieldDeclaration((EventFieldDeclarationSyntax)node);
                break;
            case SyntaxKind.IncompleteMember:
                _result.AddError($"Incomplete member declaration found at {GetLocation(node)}. This indicates a parsing issue.");
                // Continue recursion to find more issues
                break;

            // Default case to ensure recursion continues for unhandled or non-specific nodes
            default:
                // Could log unhandled kinds for debugging validator coverage
                // Console.WriteLine($"DEBUG: Unhandled node kind in general validation: {node.Kind()} at {GetLocation(node)}");
                break; // Just recurse
        }

        // Recurse into children for nodes not handled with a return
        // Manual recursion calls inside Validate* methods for container types prevent double recursion.
        // For non-container types, recurse here.
        if (node.Kind() != SyntaxKind.CompilationUnit &&
           node.Kind() != SyntaxKind.NamespaceDeclaration &&
           node.Kind() != SyntaxKind.ClassDeclaration &&
           node.Kind() != SyntaxKind.StructDeclaration &&
           node.Kind() != SyntaxKind.InterfaceDeclaration &&
           node.Kind() != SyntaxKind.RecordDeclaration &&
           node.Kind() != SyntaxKind.EnumDeclaration)
        {
            foreach (var child in node.ChildNodes())
            {
                ValidateNode(child);
            }
        }
        else if (node.Kind() == SyntaxKind.CompilationUnit)
        {
            // Special case for CompilationUnit - its members are handled by recursion,
            // but other top-level children like Usings, AttributeLists are also visited.
            foreach (var child in ((CompilationUnitSyntax)node).Usings) ValidateNode(child);
            foreach (var child in ((CompilationUnitSyntax)node).AttributeLists) ValidateNode(child);
            // Members recursion is manual in ValidateCompilationUnit
        }
    }

    // --- Specific Validation Methods for General C# Structure ---
    // These methods also handle recursion into their members/children


    private void ValidateCompilationUnit(CompilationUnitSyntax node)
    {
        if (!node.Members.Any() && !node.Usings.Any() && !node.AttributeLists.Any())
        {
            // Allow empty files
        }
        // Recurse into root members
        foreach (var member in node.Members)
        {
            ValidateNode(member); // Calls ValidateNamespaceDeclaration, ValidateTypeDeclaration etc.
        }
        // Usings and AttributeLists recursion is handled in the default case or explicit CU handling.
    }

    private void ValidateNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        // Name check handled by Roslyn parser diagnostics
        if (!node.Members.Any())
        {
            _result.AddWarning($"Namespace '{node.Name}' is empty at {GetLocation(node)}.");
        }
        // Recurse into members within the namespace
        foreach (var member in node.Members)
        {
            ValidateNode(member); // Calls ValidateTypeDeclaration etc.
        }
    }

    private void ValidateTypeDeclaration(BaseTypeDeclarationSyntax node)
    {
        // Name check handled by Roslyn parser diagnostics
        // Check for empty types (optional)
        if (node is TypeDeclarationSyntax typeNode && !typeNode.Members.Any() && !typeNode.AttributeLists.Any() && typeNode.BaseList == null)
        {
            // This might be valid for marker classes/structs, so make it a warning or conditional.
            // _result.AddWarning($"{node.Kind()} '{node.Identifier.Text}' is very sparse (no members, attributes, or base types) at {GetLocation(node)}.");
        }
        // Recurse into members within the type
        if (node is TypeDeclarationSyntax typeNodeWithMembers) // Class, Struct, Interface, Record
        {
            foreach (var member in typeNodeWithMembers.Members)
            {
                ValidateNode(member); // Calls ValidateMethodDeclaration, ValidatePropertyDeclaration etc.
            }
        }
        else if (node is EnumDeclarationSyntax enumNode)
        {
            // Validate enum members if necessary (e.g., check for duplicates - Roslyn does this)
            foreach (var member in enumNode.Members) // Members is SeparatedSyntaxList<EnumMemberDeclarationSyntax>
            {
                ValidateNode(member); // Calls ValidateEnumMemberDeclaration
            }
        }
        // BaseList is a child and will be visited by recursive calls
        // AttributeLists are children and will be visited
    }

    private void ValidateMethodDeclaration(MethodDeclarationSyntax node)
    {
        // Name, return type, body presence (conditional) checks handled by Roslyn diagnostics
        // ParameterList, Body/ExpressionBody, AttributeLists are children and will be visited
    }

    private void ValidatePropertyDeclaration(PropertyDeclarationSyntax node)
    {
        // Name, type checks handled by Roslyn diagnostics
        // AccessorList, ExpressionBody, Initializer, AttributeLists are children and will be visited
    }

    private void ValidateFieldDeclaration(FieldDeclarationSyntax node)
    {
        // Declaration (type, variables) checks handled by Roslyn diagnostics
        // Variable declarators within Declaration will be visited
        // AttributeLists are children and will be visited
    }

    private void ValidateEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
    {
        // Name check handled by Roslyn parser diagnostics
        // EqualsValueClause (for explicit value) is a child and will be visited
        // AttributeLists are children and will be visited
    }

    private void ValidateDelegateDeclaration(DelegateDeclarationSyntax node)
    {
        // Name, return type, parameter list checks handled by Roslyn diagnostics
        // ParameterList, AttributeLists are children and will be visited
    }

    private void ValidateConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        // Name (implicit), body/expression body checks handled by Roslyn diagnostics
        // ParameterList, Initializer, AttributeLists are children and will be visited
    }

    private void ValidateIndexerDeclaration(IndexerDeclarationSyntax node)
    {
        // Type, parameter list, accessor list checks handled by Roslyn diagnostics
        // ParameterList, AccessorList, ExpressionBody, AttributeLists are children and will be visited
    }

    private void ValidateOperatorDeclaration(OperatorDeclarationSyntax node)
    {
        // Return type, parameter list, body checks handled by Roslyn diagnostics
        // ParameterList, Body/ExpressionBody, AttributeLists are children and will be visited
    }

    private void ValidateConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
    {
        // Target type, parameter list (count), body checks handled by Roslyn diagnostics
        // ParameterList, Body/ExpressionBody, AttributeLists are children and will be visited
    }

    private void ValidateEventDeclaration(EventDeclarationSyntax node)
    {
        // Name, type checks handled by Roslyn diagnostics
        // AttributeLists are children and will be visited
    }

    private void ValidateEventFieldDeclaration(EventFieldDeclarationSyntax node)
    {
        // Declaration (type, variables) checks handled by Roslyn diagnostics
        // Variable declarators within Declaration will be visited
        // AttributeLists are children and will be visited
    }


    // Helper to get location string for reporting
    private string GetLocation(SyntaxNode node)
    {
        if (node == null || node.SyntaxTree == null) return "[Unknown Location]";
        try
        {
            var lineSpan = node.GetLocation().GetLineSpan();
            return $"Line {lineSpan.StartLinePosition.Line + 1}, Col {lineSpan.StartLinePosition.Character + 1}";
        }
        catch
        {
            return "[Unknown Location (Error getting line span)]";
        }
    }


    // --- Specific Generated Code Validation (Uses Options) ---
    // This method orchestrates checks based on the GenerationOptions blueprint
    private void ValidateSpecificGeneratedCode(SyntaxNode root)
    {
        // Find the main type declaration generated by these options
        BaseTypeDeclarationSyntax mainTypeDeclaration = null;
        // Search for the main type in the expected namespace, or anywhere if namespace is not specified
        if (!string.IsNullOrWhiteSpace(_options.NamespaceName))
        {
            var targetNamespace = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>()
                                .FirstOrDefault(ns => ns.Name.ToString() == _options.NamespaceName);

            if (targetNamespace != null)
            {
                mainTypeDeclaration = targetNamespace.Members.OfType<BaseTypeDeclarationSyntax>()
                                          .FirstOrDefault(t => t.Identifier.Text == _options.ClassName);
            }
            // If targetNamespace is null, mainTypeDeclaration remains null, error reported below.
        }
        else
        {
            // If no specific namespace expected, find the type anywhere
            mainTypeDeclaration = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
                                      .FirstOrDefault(t => t.Identifier.Text == _options.ClassName);
        }


        if (mainTypeDeclaration == null)
        {
            // If options expected a type but it's not found, it's an error
            if (!string.IsNullOrWhiteSpace(_options.ClassName))
            {
                _result.AddError($"Main generated type '{_options.ClassName}' not found in the code. Expected namespace: '{_options.NamespaceName ?? "(any)"}'.");
            }
            return; // Cannot perform type-specific checks if the type isn't found
        }

        // --- Check Base Types and Interfaces ---
        ValidateBaseList(mainTypeDeclaration, _options);

        // --- Check Expected Members ---
        ValidateExpectedMembers(mainTypeDeclaration, _options);


        // --- Add Generator-Specific Attribute Checks based on Naming/Conventions ---
        // This is a heuristic way to add checks based on the *likely* generator type.
        // You can extend this switch statement with specific checks for different generator types.

        var mainTypeName = _options.ClassName;
        var mainTypeKind = mainTypeDeclaration.Kind();

        // Example: Controller-specific checks
        if (mainTypeName.EndsWith("Controller") && mainTypeKind == SyntaxKind.ClassDeclaration)
        {
            ValidateControllerSpecifics((ClassDeclarationSyntax)mainTypeDeclaration, _options);
        }
        // Example: Validator-specific checks
        else if (mainTypeName.EndsWith("Validator") && mainTypeKind == SyntaxKind.ClassDeclaration)
        {
            ValidateValidatorSpecifics((ClassDeclarationSyntax)mainTypeDeclaration, _options);
        }
        // Example: DTO/VM specific checks (assuming they are classes)
        else if ((mainTypeName.EndsWith("Dto") || mainTypeName.EndsWith("VM")) && mainTypeKind == SyntaxKind.ClassDeclaration)
        {
            ValidateDtoVmSpecifics((ClassDeclarationSyntax)mainTypeDeclaration, _options);
        }
        // Add checks for other generator types (Repository, Scheduler, etc.)
        else if (mainTypeName.EndsWith("Repository") && mainTypeKind == SyntaxKind.ClassDeclaration)
        {
            ValidateRepositorySpecifics((ClassDeclarationSyntax)mainTypeDeclaration, _options);
        }
        else if (mainTypeName.EndsWith("Job") && mainTypeKind == SyntaxKind.ClassDeclaration)
        {
            ValidateSchedulerSpecifics((ClassDeclarationSyntax)mainTypeDeclaration, _options);
        }
        // Add checks for interfaces like IController, IService, IRepository if they are generated


    }

    // --- Helper Validation Methods for Specific Aspects ---

    private void ValidateBaseList(BaseTypeDeclarationSyntax node, GenerationOptions options)
    {
        if (options.ExpectedBaseListTypeIdentities != null && options.ExpectedBaseListTypeIdentities.Any())
        {
            var actualBaseListTypeIdentities = node.BaseList?.Types
                                                .Select(bt => CodeGeneratorUtils.GetTypeIdentity(bt.Type))
                                                .Where(id => id != null)
                                                .ToList() ?? new List<string>();

            foreach (var expectedIdentity in options.ExpectedBaseListTypeIdentities)
            {
                if (!actualBaseListTypeIdentities.Contains(expectedIdentity))
                {
                    var typeName = expectedIdentity.Split('!').FirstOrDefault() ?? expectedIdentity; // Simplified guess
                    _result.AddError($"{node.Kind()} '{node.Identifier.Text}' is missing expected base type or interface '{typeName}' (identity: '{expectedIdentity}') at {GetLocation(node)}.");
                }
            }
        }
    }

    private void ValidateExpectedMembers(BaseTypeDeclarationSyntax node, GenerationOptions options)
    {
        // Get all members directly owned by the node type
        SyntaxList<MemberDeclarationSyntax> nodeMembers;
        if (node is TypeDeclarationSyntax typeWithMembers) nodeMembers = typeWithMembers.Members;
        else if (node is EnumDeclarationSyntax enumNode) nodeMembers = SyntaxFactory.List<MemberDeclarationSyntax>(enumNode.Members); // Cast EnumMemberDeclarationSyntax to MemberDeclarationSyntax
        else nodeMembers = SyntaxFactory.List<MemberDeclarationSyntax>();


        if (options.ExpectedMemberIdentities != null && options.ExpectedMemberIdentities.Any())
        {
            var actualMemberIdentities = nodeMembers
                                            .Select(m => CodeGeneratorUtils.GetMemberIdentity(m))
                                            .Where(id => id != null)
                                            .ToList();

            foreach (var expectedIdentity in options.ExpectedMemberIdentities)
            {
                if (!actualMemberIdentities.Contains(expectedIdentity))
                {
                    var parts = expectedIdentity.Split('!');
                    var memberName = parts.FirstOrDefault() ?? "UnknownMember";
                    var memberKind = parts.Skip(1).FirstOrDefault() ?? "UnknownKind";

                    _result.AddError($"{node.Kind()} '{node.Identifier.Text}' is missing expected member '{memberName}' ({memberKind}) (identity: '{expectedIdentity}') at {GetLocation(node)}.");
                }
            }
        }
    }


    // --- Specific Generator Type Validation Methods ---

    private void ValidateControllerSpecifics(ClassDeclarationSyntax node, GenerationOptions options)
    {
             // Add checks for expected HTTP method attributes on methods if needed
        // This would require parsing method identities and checking for specific attributes.
        // Example: Check if methods in ExpectedMemberIdentities have corresponding attributes
    }

    private void ValidateValidatorSpecifics(ClassDeclarationSyntax node, GenerationOptions options)
    {
        // Check if it inherits from BaseValidator<,> - Covered by ValidateBaseList
        // Check if it implements ITValidator - Covered by ValidateBaseList

        // Check for InitializeConditions method - Covered by ValidateExpectedMembers

        // Check for the nested Enum - Covered by ValidateExpectedMembers

        // Check for the constructor - Covered by ValidateExpectedMembers

        // Add other validator-specific checks if needed (e.g., checks related to rule registration inside InitializeConditions body? - requires semantic analysis or pattern matching inside method body, very complex)
    }

    private void ValidateDtoVmSpecifics(ClassDeclarationSyntax node, GenerationOptions options)
    {
        // Check if it implements ITDso, ITBuildDto, ITShareDto, ITVM etc. - Covered by ValidateBaseList

        // Check for properties - Covered by ValidateExpectedMembers (checking expected identities)

        // Example Rule: Check if all properties have a getter (or both get/set/init)
        // This is a rule that might need to be configurable.
        foreach (var member in node.Members.OfType<PropertyDeclarationSyntax>())
        {
            // If a property is expected (its identity is in ExpectedMemberIdentities), check its accessors.
            // Or check all properties found in the generated DTO/VM.
            // Let's check all properties found, assuming DTO/VM properties should follow certain patterns.
            if (member.AccessorList == null || !member.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration))
            {
                _result.AddError($"DTO/VM property '{member.Identifier.Text}' is missing a 'get' accessor at {GetLocation(member)}.");
            }
            // Optional: Check for set/init accessor too depending on DTO/VM type/convention.
            // if (!member.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration || a.Kind() == SyntaxKind.InitAccessorDeclaration))
            // {
            //     _result.AddWarning($"DTO/VM property '{member.Identifier.Text}' is missing a 'set' or 'init' accessor at {GetLocation(member)}.");
            // }

        }
        // Check for Id property presence based on VM subtype (Create, Output, etc.) - Covered by ValidateExpectedMembers
    }

    private void ValidateRepositorySpecifics(ClassDeclarationSyntax node, GenerationOptions options)
    {
        // Check base classes/interfaces - Covered by ValidateBaseList
        // Check constructor - Covered by ValidateExpectedMembers
        // Check standard methods like CountAsync, GetByIdAsync, etc. - Covered by ValidateExpectedMembers
        // Add repository-specific checks if needed (e.g., check for DbContext injection in constructor) - requires inspecting constructor parameters and types, more complex.
    }

    private void ValidateSchedulerSpecifics(ClassDeclarationSyntax node, GenerationOptions options)
    {
        // Check base class (BaseJob) - Covered by ValidateBaseList
        // Check Execute method - Covered by ValidateExpectedMembers
        // Check InitializeJobOptions method - Covered by ValidateExpectedMembers
        // Add scheduler-specific checks if needed (e.g., check if JobOptions are set in InitializeJobOptions body) - requires inspecting method body, complex.
    }


}