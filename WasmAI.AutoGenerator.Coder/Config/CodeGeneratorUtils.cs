// ===== File: CodeGeneratorUtils.cs (Same) =====
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoGenerator.Code.v1;

public static class CodeGeneratorUtils
{
    // Correctly handle Nullable types and add '?' for string
    public static string GetPropertyTypeName(Type propertyType)
    {
        if (propertyType == null) return "object?"; // Handle null type defensively

        if (propertyType.IsGenericType)
        {
            var genericArguments = propertyType.GetGenericArguments();
            if (propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(propertyType);
                // Recursively get the name of the underlying type and append '?'
                return underlyingType != null ? GetPropertyTypeName(underlyingType) + "?" : propertyType.Name;
            }

            var baseTypeName = propertyType.Name.Substring(0, propertyType.Name.IndexOf('`'));
            var genericArgNames = genericArguments.Select(GetPropertyTypeName);
            return $"{baseTypeName}<{string.Join(", ", genericArgNames)}>";
        }
        else if (propertyType == typeof(string))
        {
            return "string?"; // Treat string as nullable by default
        }
        else if (propertyType.IsPrimitive || propertyType.IsValueType)
        {
            var alias = SyntaxFacts.GetKeywordKind(propertyType.Name);
            if (alias != SyntaxKind.None && SyntaxFacts.IsPredefinedType(alias))
            {
                return SyntaxFacts.GetText(alias);
            }
            return propertyType.Name;
        }
        else // Reference types (object, class, interface, delegate, array)
        {
            var alias = SyntaxFacts.GetKeywordKind(propertyType.Name);
            if (alias != SyntaxKind.None)
            {
                return SyntaxFacts.GetText(alias);
            }

            if (propertyType.IsArray)
            {
                var elementType = propertyType.GetElementType();
                return $"{GetPropertyTypeName(elementType)}[]";
            }

            return propertyType.Name;
        }
    }


    public static bool IsCollection(this Type type)
    {
        if (type == null) return false;
        return type != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
    }

    public static string ApplyTemplate(string template, Dictionary<string, string> replacements)
    {
        // Use String.Replace is simple but can have issues with partial matches.
        // Regex.Replace would be more robust for exact {Key} matching.
        foreach (var replacement in replacements)
        {
            template = template.Replace($"{{{replacement.Key}}}", replacement.Value);
        }
        return template;
    }

    // Generates a unique identifier string for a member for matching purposes.
    public static string GetMemberIdentity(SyntaxNode member)
    {
        if (member == null) return null;

        // Handle EnumMemberDeclarationSyntax separately as it's not a MemberDeclarationSyntax
        if (member is EnumMemberDeclarationSyntax enumMember)
        {
            return enumMember.Identifier.Text + "!" + SyntaxKind.EnumMemberDeclaration; // Use "!" as separator
        }

        // Handle standard MemberDeclarationSyntax types
        if (member is MemberDeclarationSyntax memberDeclaration)
        {
            string baseIdentity;
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    baseIdentity = ((NamespaceDeclarationSyntax)memberDeclaration).Name.ToString();
                    break;
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.EnumDeclaration:
                    baseIdentity = ((BaseTypeDeclarationSyntax)memberDeclaration).Identifier.Text;
                    break;
                case SyntaxKind.DelegateDeclaration:
                    baseIdentity = ((DelegateDeclarationSyntax)memberDeclaration).Identifier.Text + ((DelegateDeclarationSyntax)memberDeclaration).ParameterList.ToString() + GetMethodGenericParametersString(((DelegateDeclarationSyntax)memberDeclaration).TypeParameterList); // Include parameters and generics
                    break;
                case SyntaxKind.MethodDeclaration:
                    baseIdentity = ((MethodDeclarationSyntax)memberDeclaration).Identifier.Text + ((MethodDeclarationSyntax)memberDeclaration).ParameterList.ToString() + GetMethodGenericParametersString(((MethodDeclarationSyntax)memberDeclaration).TypeParameterList); // Include parameters and generics
                    break;
                case SyntaxKind.ConstructorDeclaration:
                    baseIdentity = ((ConstructorDeclarationSyntax)memberDeclaration).ParameterList.ToString(); // Only parameters needed
                    break;
                case SyntaxKind.DestructorDeclaration:
                    baseIdentity = ((DestructorDeclarationSyntax)memberDeclaration).Identifier.Text; // Name is fixed ~TypeName
                    break;
                case SyntaxKind.PropertyDeclaration:
                    baseIdentity = ((PropertyDeclarationSyntax)memberDeclaration).Identifier.Text;
                    break;
                case SyntaxKind.IndexerDeclaration:
                    baseIdentity = ((IndexerDeclarationSyntax)memberDeclaration).ParameterList.ToString(); // Match by parameters
                    break;
                case SyntaxKind.EventDeclaration: // Event with add/remove accessors
                case SyntaxKind.EventFieldDeclaration: // Event declared as a field
                    var eventDecl = memberDeclaration as EventDeclarationSyntax;
                    var eventFieldDecl = memberDeclaration as EventFieldDeclarationSyntax;
                    baseIdentity = eventDecl?.Identifier.Text ?? eventFieldDecl?.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                    break;
                case SyntaxKind.FieldDeclaration:
                    baseIdentity = ((FieldDeclarationSyntax)memberDeclaration).Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                    break;
                case SyntaxKind.OperatorDeclaration:
                    baseIdentity = ((OperatorDeclarationSyntax)memberDeclaration).OperatorToken.ValueText + ((OperatorDeclarationSyntax)memberDeclaration).ParameterList.ToString(); // Operator token + parameters
                    break;
                case SyntaxKind.ConversionOperatorDeclaration:
                    baseIdentity = ((ConversionOperatorDeclarationSyntax)memberDeclaration).Type.ToString() + ((ConversionOperatorDeclarationSyntax)memberDeclaration).ParameterList.ToString(); // Return type + parameters
                    break;
                default:
                    return null; // Unhandled member types
            }
            return baseIdentity + "!" + memberDeclaration.Kind();
        }

        return null; // Not a member declaration
    }

    // Helper to get a string representation of method generic type parameters
    private static string GetMethodGenericParametersString(TypeParameterListSyntax typeParameterList)
    {
        if (typeParameterList == null) return string.Empty;
        return "<" + string.Join(",", typeParameterList.Parameters.Select(p => p.Identifier.Text)) + ">";
    }

    // Helper to generate identity for TypeSyntax (used for base class/interface matching)
    // This is a very basic identity using the string representation.
    public static string GetTypeIdentity(TypeSyntax type)
    {
        if (type == null) return null;
        return type.NormalizeWhitespace().ToFullString(); // Includes generics, but not namespaces
    }

    // Helper to parse a string snippet into a SyntaxNode (useful for getting identities from template parts)
    // Be cautious with this, it expects valid C# snippets that can be placed inside a type.
    // Returns the first member declaration found in the snippet.
    public static MemberDeclarationSyntax ParseMemberSnippet(string snippet)
    {
        if (string.IsNullOrWhiteSpace(snippet)) return null;
        // Wrap in a dummy class to parse member declarations
        var wrappedCode = $"class Dummy {{ {snippet} }}";
        var tree = CSharpSyntaxTree.ParseText(wrappedCode);
        // Return the first member inside the dummy class
        return tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault()?.Members.FirstOrDefault();
    }
}

