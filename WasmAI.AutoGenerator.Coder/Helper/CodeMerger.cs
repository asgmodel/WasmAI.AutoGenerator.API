using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; // Needed for Path.GetFileNameWithoutExtension

public class ComprehensiveCodeMerger2
{
    private SyntaxNode _oldRoot;
    private SyntaxNode _newRoot;
    private string _oldFileName;
    private string _newFileName;

    /// <summary>
    /// Initializes a new instance of the ComprehensiveCodeMerger class.
    /// </summary>
    /// <param name="oldCode">The original C# code string.</param>
    /// <param name="newCode">The new C# code string.</param>
    /// <param name="oldFilePath">The file path for the old code (used for identifying the source).</param>
    /// <param name="newFilePath">The file path for the new code (used for identifying the source).</param>
    /// <param name="parseOptions">Optional C# parse options.</param>
    public ComprehensiveCodeMerger2(string oldCode, string newCode, string oldFilePath, string newFilePath, CSharpParseOptions parseOptions = null)
    {
        parseOptions ??= new CSharpParseOptions(LanguageVersion.Latest);

        _oldFileName = Path.GetFileNameWithoutExtension(oldFilePath);
        _newFileName = Path.GetFileNameWithoutExtension(newFilePath);

        var oldTree = CSharpSyntaxTree.ParseText(oldCode, parseOptions, path: oldFilePath);
        var newTree = CSharpSyntaxTree.ParseText(newCode, parseOptions, path: newFilePath);

        ValidateCode(oldTree, "Old Code");
        ValidateCode(newTree, "New Code");

        _oldRoot = oldTree.GetRoot();
        _newRoot = newTree.GetRoot();
    }

    private void ValidateCode(SyntaxTree tree, string label)
    {
        var errors = tree.GetDiagnostics()
                              .Where(d => d.Severity == DiagnosticSeverity.Error)
                              .ToList();

        if (errors.Any())
        {
            var errorMessages = string.Join("\n", errors.Select(d => $"{label} Error: {d.GetMessage()} at {d.Location.GetLineSpan()}"));
            throw new InvalidOperationException($"Syntax errors found:\n{errorMessages}");
        }
    }
    /// <summary>
    /// Merges the new code into the old code based on structural comparison.
    /// If file names are the same, it assumes the new code is a development of the old code,
    /// applying additions, modifications (replacing old with new), and deletions.
    /// If file names are different, the same comprehensive merge logic is applied,
    /// which effectively combines the contents based on member identity.
    /// </summary>
    /// <returns>The merged code string.</returns>
    public string MergeCode()
    {
        if (_oldRoot is not CompilationUnitSyntax oldUnit || _newRoot is not CompilationUnitSyntax newUnit)
        {
            throw new ArgumentException("Both code inputs must represent valid C# compilation units.");
        }

        SyntaxNode mergedRoot;

        if (_oldFileName.Equals(_newFileName, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Merging '{_newFileName}.cs' as a development of '{_oldFileName}.cs'.");
            mergedRoot = MergeCompilationUnits(oldUnit, newUnit);
        }
        else
        {
            Console.WriteLine($"Merging content from '{_newFileName}.cs' into '{_oldFileName}.cs'.");
            mergedRoot = MergeCompilationUnits(oldUnit, newUnit);
        }

        return mergedRoot.NormalizeWhitespace().ToFullString();
    }

    private CompilationUnitSyntax MergeCompilationUnits(CompilationUnitSyntax oldUnit, CompilationUnitSyntax newUnit)
    {
        var mergedUnit = oldUnit;

        mergedUnit = MergeUsings(mergedUnit, newUnit);
        mergedUnit = MergeGlobalAttributes(mergedUnit, newUnit);
        mergedUnit = (CompilationUnitSyntax)MergeMembersInContainer(mergedUnit, newUnit);

        return mergedUnit;
    }

    private CompilationUnitSyntax MergeUsings(CompilationUnitSyntax oldUnit, CompilationUnitSyntax newUnit)
    {
        var oldUsings = oldUnit.Usings;
        var newUsings = newUnit.Usings;
        var mergedUsingsSet = new HashSet<string>(oldUsings.Select(u => u.NormalizeWhitespace().ToFullString()));
        var usingsToAdd = new List<UsingDirectiveSyntax>();

        foreach (var newUsing in newUsings)
        {
            if (mergedUsingsSet.Add(newUsing.NormalizeWhitespace().ToFullString()))
            {
                usingsToAdd.Add(newUsing);
            }
        }

        return usingsToAdd.Any() ? oldUnit.AddUsings(usingsToAdd.ToArray()) : oldUnit;
    }

    private CompilationUnitSyntax MergeGlobalAttributes(CompilationUnitSyntax oldUnit, CompilationUnitSyntax newUnit)
    {
        var oldGlobalAttrLists = oldUnit.AttributeLists
            .Where(al => al.Target?.Identifier.Kind() is SyntaxKind.AssemblyKeyword or SyntaxKind.ModuleKeyword)
            .ToList();
        var newGlobalAttrLists = newUnit.AttributeLists
            .Where(al => al.Target?.Identifier.Kind() is SyntaxKind.AssemblyKeyword or SyntaxKind.ModuleKeyword)
            .ToList();

        var mergedAttributeLists = oldUnit.AttributeLists
            .Where(al => al.Target?.Identifier.Kind() is not (SyntaxKind.AssemblyKeyword or SyntaxKind.ModuleKeyword))
            .ToList(); // Start with non-global attributes from old

        var existingGlobalAttrStrings = new HashSet<string>();

        // Add old global attributes first, tracking them
        foreach (var oldAttrList in oldGlobalAttrLists)
        {
            if (existingGlobalAttrStrings.Add(oldAttrList.NormalizeWhitespace().ToFullString()))
            {
                mergedAttributeLists.Add(oldAttrList);
            }
        }

        // Add new global attributes that are not already present (from old or new)
        foreach (var newAttrList in newGlobalAttrLists)
        {
            if (existingGlobalAttrStrings.Add(newAttrList.NormalizeWhitespace().ToFullString()))
            {
                mergedAttributeLists.Add(newAttrList);
            }
        }

        var finalAttributeListsSyntax = SyntaxFactory.List(mergedAttributeLists);

        bool listsAreEquivalent = oldUnit.AttributeLists.Count == finalAttributeListsSyntax.Count;
        if (listsAreEquivalent)
        {
            for (int i = 0; i < oldUnit.AttributeLists.Count; i++)
            {
                if (!oldUnit.AttributeLists[i].IsEquivalentTo(finalAttributeListsSyntax[i], topLevel: false))
                {
                    listsAreEquivalent = false;
                    break;
                }
            }
        }

        if (!listsAreEquivalent)
        {
            return oldUnit.WithAttributeLists(finalAttributeListsSyntax);
        }
        return oldUnit;
    }

    private SyntaxNode MergeMembersInContainer(SyntaxNode oldContainer, SyntaxNode newContainer)
    {
        if (oldContainer is EnumDeclarationSyntax oldEnum && newContainer is EnumDeclarationSyntax newEnum)
        {
            return MergeEnumMembers(oldEnum, newEnum);
        }

        var oldMembersList = GetStandardMembers(oldContainer);
        var newMembersList = GetStandardMembers(newContainer);

        if (oldMembersList == default || newMembersList == default)
        {
            return oldContainer;
        }

        SyntaxNode updatedContainer = oldContainer;
        var oldMemberLookup = oldMembersList
            .Select(m => new { Member = m, Identity = GetMemberIdentity(m) })
            .Where(item => item.Identity != null)
            .ToLookup(item => item.Identity, item => item.Member);

        var matchedOldMembers = new HashSet<MemberDeclarationSyntax>();

        foreach (var newMember in newMembersList)
        {
            string identity = GetMemberIdentity(newMember);
            if (identity == null) continue;

            var oldCandidates = oldMemberLookup.Contains(identity) ? oldMemberLookup[identity].ToList() : new List<MemberDeclarationSyntax>();

            if (oldCandidates.Any())
            {
                MemberDeclarationSyntax oldMemberToReplace = null;
                bool equivalentFound = false;

                foreach (var oldCandidate in oldCandidates)
                {
                    if (oldCandidate.Kind() == newMember.Kind())
                    {
                        if (SyntaxFactory.AreEquivalent(oldCandidate, newMember))
                        {
                            oldMemberToReplace = oldCandidate;
                            equivalentFound = true;
                            break;
                        }
                        else
                        {
                            oldMemberToReplace = oldCandidate;
                        }
                    }
                }

                if (oldMemberToReplace != null) // A candidate of the same kind was found
                {
                    matchedOldMembers.Add(oldMemberToReplace);
                    if (!equivalentFound) // Same Kind and Identity, but different content
                    {
                        if (IsContainerType(oldMemberToReplace)) // Both are containers
                        {
                            var mergedChildContainer = MergeMembersInContainer(oldMemberToReplace, newMember);
                            updatedContainer = updatedContainer.ReplaceNode(oldMemberToReplace, mergedChildContainer);
                        }
                        else // Both are non-containers (method, property, field, etc.)
                        {
                            updatedContainer = updatedContainer.ReplaceNode(oldMemberToReplace, newMember);
                        }
                    }
                }
                else // Identity matched, but NO old candidate was of the SAME KIND as newMember
                {
                    updatedContainer = AddStandardMember(updatedContainer, newMember);
                }
            }
            else // New member (no identity match in old)
            {
                updatedContainer = AddStandardMember(updatedContainer, newMember);
            }
        }

        var newMemberLookupForDeletion = newMembersList
             .Select(m => new { Member = m, Identity = GetMemberIdentity(m) })
             .Where(item => item.Identity != null)
             .ToLookup(item => item.Identity, item => item.Member);

        var oldMembersToDelete = new List<MemberDeclarationSyntax>();
        foreach (var oldMember in oldMembersList)
        {
            string identity = GetMemberIdentity(oldMember);
            if (identity == null) continue;

            var newCandidates = newMemberLookupForDeletion.Contains(identity) ? newMemberLookupForDeletion[identity].ToList() : new List<MemberDeclarationSyntax>();
            bool foundMatchInNewOfSameKind = newCandidates.Any(nc => nc.Kind() == oldMember.Kind());

            if (!foundMatchInNewOfSameKind)
            {
                oldMembersToDelete.Add(oldMember);
            }
        }

        if (oldMembersToDelete.Any())
        {
            updatedContainer = updatedContainer.RemoveNodes(oldMembersToDelete, SyntaxRemoveOptions.KeepExteriorTrivia);
        }

        return updatedContainer;
    }

    private SyntaxList<MemberDeclarationSyntax> GetStandardMembers(SyntaxNode container)
    {
        return container switch
        {
            CompilationUnitSyntax unit => unit.Members,
            NamespaceDeclarationSyntax ns => ns.Members,
            _ => default
        };
    }

    private SyntaxNode AddStandardMember(SyntaxNode container, MemberDeclarationSyntax memberToAdd)
    {
        return container switch
        {
            CompilationUnitSyntax unit => unit.AddMembers(memberToAdd),
            NamespaceDeclarationSyntax ns => ns.AddMembers(memberToAdd),
            //BaseTypeDeclarationSyntax type => type.AddModifiers(memberToAdd), // Class, Struct, Interface, Record
            _ => container
        };
    }

    private EnumDeclarationSyntax MergeEnumMembers(EnumDeclarationSyntax oldEnum, EnumDeclarationSyntax newEnum)
    {
        EnumDeclarationSyntax updatedEnum = oldEnum;

        var oldMembersOriginal = oldEnum.Members.ToList();
        var newMembersList = newEnum.Members.ToList();

        var oldMemberMapById = oldMembersOriginal.ToDictionary(m => m.Identifier.Text);
        var matchedOldMemberIdentities = new HashSet<string>();

        // Pass 1: Add new members or modify existing ones
        foreach (var newMember in newMembersList)
        {
            var identity = newMember.Identifier.Text;
            if (oldMemberMapById.TryGetValue(identity, out var originalOldMember))
            {
                matchedOldMemberIdentities.Add(identity);
                // Find the current version of this old member in updatedEnum (it might have been affected by prior ops)
                var currentOldMemberNodeInUpdatedEnum = updatedEnum.Members.FirstOrDefault(m => m.Identifier.Text == identity);
                if (currentOldMemberNodeInUpdatedEnum != null && !SyntaxFactory.AreEquivalent(currentOldMemberNodeInUpdatedEnum, newMember))
                {
                    updatedEnum = updatedEnum.ReplaceNode(currentOldMemberNodeInUpdatedEnum, newMember);
                }
            }
            else
            {
                updatedEnum = updatedEnum.AddMembers(newMember);
            }
        }

        // Pass 2: Delete old members not present in the new enum
        var membersToDelete = new List<EnumMemberDeclarationSyntax>();
        var newMemberIdentities = new HashSet<string>(newMembersList.Select(m => m.Identifier.Text));

        foreach (var originalOldMember in oldMembersOriginal)
        {
            if (!newMemberIdentities.Contains(originalOldMember.Identifier.Text))
            {
                // This original old member is not in the new list, so it needs to be deleted from 'updatedEnum'.
                // Find its current representation in 'updatedEnum'.
                var nodeToDeleteInUpdatedEnum = updatedEnum.Members.FirstOrDefault(m => m.Identifier.Text == originalOldMember.Identifier.Text);
                if (nodeToDeleteInUpdatedEnum != null)
                {
                    membersToDelete.Add(nodeToDeleteInUpdatedEnum);
                }
            }
        }

        if (membersToDelete.Any())
        {
            // Ensure distinct nodes if somehow duplicates were added (though unlikely for simple enums)
            updatedEnum = updatedEnum.RemoveNodes(membersToDelete.Distinct(), SyntaxRemoveOptions.KeepExteriorTrivia);
        }

        // Merge attributes and modifiers for the EnumDeclarationSyntax itself
        if (!SyntaxFactory.AreEquivalent(oldEnum.AttributeLists, newEnum.AttributeLists))
            updatedEnum = updatedEnum.WithAttributeLists(newEnum.AttributeLists);
        if (!SyntaxFactory.AreEquivalent(oldEnum.Modifiers, newEnum.Modifiers))
            updatedEnum = updatedEnum.WithModifiers(newEnum.Modifiers);
        // Could also compare/merge Identifier if necessary, though less common for enums to be renamed during merge.

        return updatedEnum;
    }

    private bool IsContainerType(SyntaxNode node)
    {
        return node is NamespaceDeclarationSyntax ||
               node is BaseTypeDeclarationSyntax; // Class, Struct, Interface, Record
    }

    private string GetNormalizedParameterListString(ParameterListSyntax parameterList)
    {
        if (parameterList == null) return "()";
        return parameterList.NormalizeWhitespace().ToFullString();
    }

    private string GetNormalizedTypeParameterListString(TypeParameterListSyntax typeParameterList)
    {
        if (typeParameterList == null || !typeParameterList.Parameters.Any()) return string.Empty;
        return typeParameterList.NormalizeWhitespace().ToFullString();
    }

    private string GetMemberIdentity(SyntaxNode member)
    {
        if (member is EnumMemberDeclarationSyntax enumMember)
        {
            return enumMember.Identifier.Text + "!" + SyntaxKind.EnumMemberDeclaration;
        }

        if (member is MemberDeclarationSyntax memberDeclaration)
        {
            string baseIdentity="";
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    baseIdentity = ((NamespaceDeclarationSyntax)memberDeclaration).Name.ToString();
                    break;
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.RecordDeclaration: // Covers record struct as well
                case SyntaxKind.EnumDeclaration:
                    baseIdentity = ((BaseTypeDeclarationSyntax)memberDeclaration).Identifier.Text;
                    break;
                case SyntaxKind.DelegateDeclaration:
                    var delegateDecl = (DelegateDeclarationSyntax)memberDeclaration;
                    baseIdentity = delegateDecl.Identifier.Text +
                                   GetNormalizedParameterListString(delegateDecl.ParameterList) +
                                   GetNormalizedTypeParameterListString(delegateDecl.TypeParameterList);
                    break;
                case SyntaxKind.MethodDeclaration:
                    var methodDecl = (MethodDeclarationSyntax)memberDeclaration;
                    var methodName = methodDecl.Identifier.Text;
                    if (methodDecl.ExplicitInterfaceSpecifier != null)
                    {
                        methodName = methodDecl.ExplicitInterfaceSpecifier.Name.ToString() + "." + methodName;
                    }
                    baseIdentity = methodName +
                                   GetNormalizedParameterListString(methodDecl.ParameterList) +
                                   GetNormalizedTypeParameterListString(methodDecl.TypeParameterList);
                    break;
                case SyntaxKind.ConstructorDeclaration:
                    var ctorDecl = (ConstructorDeclarationSyntax)memberDeclaration;
                    baseIdentity = (ctorDecl.Parent is TypeDeclarationSyntax typeDecl ? typeDecl.Identifier.Text : "ctor") + // Include type name for clarity, though parameters usually suffice
                                   GetNormalizedParameterListString(ctorDecl.ParameterList);
                    break;
                case SyntaxKind.DestructorDeclaration:
                    var destructorDecl = (DestructorDeclarationSyntax)memberDeclaration;
                    baseIdentity = (destructorDecl.Parent is TypeDeclarationSyntax parentTypeDestructor ? parentTypeDestructor.Identifier.Text : "dtor") + // Include type name for clarity
                                    destructorDecl.Identifier.Text; // Name is fixed ~TypeName
                    break;
                case SyntaxKind.PropertyDeclaration:
                    baseIdentity = ((PropertyDeclarationSyntax)memberDeclaration).Identifier.Text;
                    // Could add ExplicitInterfaceSpecifier here too if properties can have it directly
                    var propDecl = (PropertyDeclarationSyntax)memberDeclaration;
                    if (propDecl.ExplicitInterfaceSpecifier != null)
                    {
                        baseIdentity = propDecl.ExplicitInterfaceSpecifier.Name.ToString() + "." + baseIdentity;
                    }
                    break;
                case SyntaxKind.IndexerDeclaration:
                    var indexerDecl = (IndexerDeclarationSyntax)memberDeclaration;
                   // baseIdentity = "this" + GetNormalizedParameterListString(indexerDecl.ParameterList);
                    if (indexerDecl.ExplicitInterfaceSpecifier != null)
                    {
                     //   baseIdentity = indexerDecl.ExplicitInterfaceSpecifier.Name.ToString() + "." + baseIdentity;
                    }
                    break;
                case SyntaxKind.EventDeclaration: // Event with add/remove accessors
                    var eventDecl = (EventDeclarationSyntax)memberDeclaration;
                    baseIdentity = eventDecl.Identifier.Text;
                    if (eventDecl.ExplicitInterfaceSpecifier != null)
                    {
                        baseIdentity = eventDecl.ExplicitInterfaceSpecifier.Name.ToString() + "." + baseIdentity;
                    }
                    break;
                case SyntaxKind.EventFieldDeclaration: // Event declared as a field
                    // Simplification: uses first variable. Multiple event fields in one decl is rare.
                    baseIdentity = ((EventFieldDeclarationSyntax)memberDeclaration).Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                    break;
                case SyntaxKind.FieldDeclaration:
                    // Simplification: uses first variable.
                    baseIdentity = ((FieldDeclarationSyntax)memberDeclaration).Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                    break;
                case SyntaxKind.OperatorDeclaration:
                    var opDecl = (OperatorDeclarationSyntax)memberDeclaration;
                    baseIdentity = opDecl.OperatorToken.ValueText + GetNormalizedParameterListString(opDecl.ParameterList);
                    break;
                case SyntaxKind.ConversionOperatorDeclaration:
                    var convOpDecl = (ConversionOperatorDeclarationSyntax)memberDeclaration;
                    baseIdentity = (convOpDecl.ImplicitOrExplicitKeyword.IsKind(SyntaxKind.ImplicitKeyword) ? "implicit " : "explicit ") +
                                   "operator " +
                                   convOpDecl.Type.NormalizeWhitespace().ToFullString() +
                                   GetNormalizedParameterListString(convOpDecl.ParameterList);
                    break;
                default:
                    // For unhandled specific member types, try to get a name if possible, or log/return null.
                    // This could be a MemberDeclarationSyntax not yet explicitly handled.
                    // A simple fallback could be member.ToString(), but it might not be unique or stable.
                    Console.WriteLine($"Warning: Unhandled member type for identity generation: {memberDeclaration.Kind()} - {memberDeclaration.GetType().Name}");
                    return null;
            }
            return baseIdentity + "!" + memberDeclaration.Kind();
        }
        return null;
    }
}