using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; // Needed for Path.GetFileNameWithoutExtension

public class ComprehensiveCodeMerger
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
    public ComprehensiveCodeMerger(string oldCode, string newCode, string oldFilePath, string newFilePath, CSharpParseOptions parseOptions = null)
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
        var diagnostics = tree.GetDiagnostics()
                              .Where(d => d.Severity == DiagnosticSeverity.Error)
                              .ToList();

        if (diagnostics.Any())
        {
            var errors = string.Join("\n", diagnostics.Select(d => $"{label} Error: {d.GetMessage()} at {d.Location.GetLineSpan()}"));
            throw new InvalidOperationException(errors);
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
        // Ensure both roots are compilation units for consistent processing
        if (_oldRoot is not CompilationUnitSyntax oldUnit || _newRoot is not CompilationUnitSyntax newUnit)
        {
            throw new ArgumentException("Both code inputs must represent valid C# compilation units.");
        }

        SyntaxNode mergedRoot;

        // Check if the code is from the same logical file
        if (_oldFileName.Equals(_newFileName, StringComparison.OrdinalIgnoreCase))
        {
            // Case: Same file - Assume new code is an evolution/improvement of the old code.
            // Apply the comprehensive merge logic (additions, modifications/replacements, deletions).
            Console.WriteLine($"Merging '{_newFileName}.cs' as a development of '{_oldFileName}.cs'.");
            mergedRoot = MergeCompilationUnits(oldUnit, newUnit);
        }
        else
        {
            // Case: Different files - Combine the contents based on member identity.
            // The same comprehensive merge logic is used, which will merge identically named
            // top-level members (namespaces, types) and add unique ones.
            // Note: This might lead to unexpected results or require conflict resolution
            // if identically named types/members in different files are not intended to be merged.
            // A more advanced merger might handle this case differently (e.g., report conflicts,
            // only add members if no match exists, etc.).
            Console.WriteLine($"Merging content from '{_newFileName}.cs' into '{_oldFileName}.cs'.");
            mergedRoot = MergeCompilationUnits(oldUnit, newUnit);
        }


        // Normalize whitespace for clean formatting of the final output
        return mergedRoot.NormalizeWhitespace().ToFullString();
    }

    // --- Recursive Merge Logic (Same as previous version) ---
    private CompilationUnitSyntax MergeCompilationUnits(CompilationUnitSyntax oldUnit, CompilationUnitSyntax newUnit)
    {
        var mergedUnit = oldUnit;

        // 1. Merge Usings
        mergedUnit = MergeUsings(mergedUnit, newUnit);

        // 2. Merge Global Attributes (attributes directly on the assembly/module before any namespace/type)
        mergedUnit = MergeGlobalAttributes(mergedUnit, newUnit);

        // 3. Merge Members at the root level (Namespaces, Type Declarations)
        // This is the recursive step for the root's direct members
        mergedUnit = (CompilationUnitSyntax)MergeMembersInContainer(mergedUnit, newUnit);

        return mergedUnit;
    }

    private CompilationUnitSyntax MergeUsings(CompilationUnitSyntax oldUnit, CompilationUnitSyntax newUnit)
    {
        var oldUsings = oldUnit.Usings;
        var newUsings = newUnit.Usings;

        // Use a HashSet to track usings we have to avoid duplicates based on their structure
        // Use NormalizeWhitespace().ToFullString() for robust comparison including trivia differences
        var mergedUsingsSet = new HashSet<string>(oldUsings.Select(u => u.NormalizeWhitespace().ToFullString()));

        var usingsToAdd = new List<UsingDirectiveSyntax>();
        foreach (var newUsing in newUsings)
        {
            // Add new using only if its normalized string representation is not already present
            if (mergedUsingsSet.Add(newUsing.NormalizeWhitespace().ToFullString()))
            {
                usingsToAdd.Add(newUsing);
            }
        }

        if (usingsToAdd.Any())
        {
            // Add the new usings to the old unit. AddUsings handles positioning.
            return oldUnit.AddUsings(usingsToAdd.ToArray());
        }

        return oldUnit; // No new usings to add
    }

    private CompilationUnitSyntax MergeGlobalAttributes(CompilationUnitSyntax oldUnit, CompilationUnitSyntax newUnit)
    {
        // More sophisticated merging logic would be needed for attributes themselves (e.g., merge attributes with the same target/name)
        // For simplicity here, we'll merge the lists. We'll take new global attributes and add any old non-global attributes.
        // This effectively replaces old global attributes with new global attributes, and keeps old non-global ones.

        var newGlobalAttributes = newUnit.AttributeLists.Where(al => al.Target?.Identifier.Kind() is SyntaxKind.AssemblyKeyword or SyntaxKind.ModuleKeyword).ToList();
        var oldNonGlobalAttributes = oldUnit.AttributeLists.Where(al => al.Target?.Identifier.Kind() is not (SyntaxKind.AssemblyKeyword or SyntaxKind.ModuleKeyword)).ToList();

        // Combine new global attributes with old non-global attributes
        var combinedAttributeLists = new List<AttributeListSyntax>();
        combinedAttributeLists.AddRange(newGlobalAttributes);
        combinedAttributeLists.AddRange(oldNonGlobalAttributes);

        // If the combined list is different from the original old list, update.
        // Checking list equivalence by converting to string is one way, or by comparing counts and content.
        // Simple check: if the combined list is different from the old unit's list.
        //if (!SyntaxFactory.AreEquivalent(oldUnit.AttributeLists.NormalizeWhitespace(), SyntaxFactory.List(combinedAttributeLists).NormalizeWhitespace()))
        //{
        //    return oldUnit.WithAttributeLists(SyntaxFactory.List(combinedAttributeLists));
        //}

        return oldUnit; // No changes to global attributes based on this logic
    }


    // Recursively merges members within a container node (CompilationUnit, Namespace, TypeDeclaration, EnumDeclaration)
    private SyntaxNode MergeMembersInContainer(SyntaxNode oldContainer, SyntaxNode newContainer)
    {
        // Handle Enum members specifically as their list type is different
        if (oldContainer is EnumDeclarationSyntax oldEnum && newContainer is EnumDeclarationSyntax newEnum)
        {
            return MergeEnumMembers(oldEnum, newEnum);
        }

        // Get members list from the container node for standard MemberDeclarationSyntax types
        var oldMembersList = GetStandardMembers(oldContainer);
        var newMembersList = GetStandardMembers(newContainer);

        // If container is not supported or has no standard members list, return the old one
        if (oldMembersList == default || newMembersList == default)
        {
            return oldContainer;
        }

        SyntaxNode updatedContainer = oldContainer;

        // Create a dictionary of old members by their identity for quick lookup during the ADDITION/MODIFICATION phase
        // Using ILookup to handle potential identity clashes gracefully (e.g., overloaded methods)
        var oldMemberLookup = oldMembersList
            .Select(m => new { Member = m, Identity = GetMemberIdentity(m) })
            .Where(item => item.Identity != null) // Only map members we can identify
            .ToLookup(item => item.Identity, item => item.Member);


        // Track which old members are matched by a new member (for deletion detection)
        var matchedOldMembers = new HashSet<MemberDeclarationSyntax>();

        // --- Pass 1: Process New Members (Additions and Modifications) ---
        // Iterate through the members in the NEW container
        foreach (var newMember in newMembersList)
        {
            string identity = GetMemberIdentity(newMember);

            // Skip members we cannot identify for merging
            if (identity == null)
            {
                // Consider logging or reporting skipped member types
                continue;
            }

            // Find potential corresponding members in the OLD container using the identity
            var oldCandidates = oldMemberLookup.Contains(identity) ? oldMemberLookup[identity].ToList() : new List<MemberDeclarationSyntax>();

            if (oldCandidates.Any())
            {
                // Found one or more corresponding members in the old code with the same identity.

                MemberDeclarationSyntax oldMemberToReplace = null;
                bool equivalentFound = false;

                // Try to find an exactly equivalent member or the best candidate for replacement
                foreach (var oldCandidate in oldCandidates)
                {
                    // Only consider candidates of the same structural kind (Method vs Method, Class vs Class)
                    if (oldCandidate.Kind() == newMember.Kind())
                    {
                        if (SyntaxFactory.AreEquivalent(oldCandidate, newMember))
                        {
                            // Found an equivalent member, no change needed for this new member.
                            oldMemberToReplace = oldCandidate; // Mark as matched
                            equivalentFound = true;
                            break; // Found exact match, stop checking candidates
                        }
                        else
                        {
                            // Found a member of the same kind with the same identity, but different content.
                            // This is a candidate for replacement if no exact equivalent is found later.
                            oldMemberToReplace = oldCandidate; // Keep track of a differing match
                        }
                    }
                }

                if (oldMemberToReplace != null)
                {
                    // Mark the specific old member node that matched (or would be replaced) as handled
                    matchedOldMembers.Add(oldMemberToReplace);

                    // If no exact equivalent was found, but a differing match was found (oldMemberToReplace is not equivalent)
                    if (!equivalentFound) // Same as checking !SyntaxFactory.AreEquivalent(oldMemberToReplace, newMember) if oldMemberToReplace is the differing one
                    {
                        // Members are different, need to merge or replace

                        bool isOldContainer = IsContainerType(oldMemberToReplace);
                        bool isNewContainer = IsContainerType(newMember);

                        if (isOldContainer && isNewContainer)
                        {
                            // Both are containers (Namespace, Class, Struct, etc.), recursively merge their contents
                            var mergedChildContainer = MergeMembersInContainer(oldMemberToReplace, newMember);
                            // Replace the old child container node with the merged one in the parent container
                            updatedContainer = updatedContainer.ReplaceNode(oldMemberToReplace, mergedChildContainer);
                        }
                        else if (!isOldContainer && !isNewContainer && oldMemberToReplace.Kind() == newMember.Kind())
                        {
                            // Both are non-containers of the same kind (method, property, field, etc.)
                            // Replace the old member node with the new member node.
                            // This implicitly merges attributes, modifiers, bodies etc by taking the new version.
                            updatedContainer = updatedContainer.ReplaceNode(oldMemberToReplace, newMember);
                        }
                        else
                        {
                            // Identity match but different kinds or a mix of container/non-container.
                            // This indicates a complex change (e.g., method changed to property with same name).
                            // In this simplified merger, we don't automatically merge this; the old node is kept.
                            // Console.WriteLine($"Conflict/Complex Change: Identity match '{identity}' but different kinds ({oldMemberToReplace.Kind()} vs {newMember.Kind()}). Keeping old node, skipping new node addition.");
                        }
                    }
                    // If equivalentFound is true, oldMemberToReplace was equivalent, already marked as matched, no replacement needed.
                }
                else
                {
                    // Identity matched one or more old members, but *none* of them were of the same Kind as the new member.
                    // This new member is treated as an addition, because it doesn't structurally match any existing member of its kind.
                    // Add the new member to the container.
                    updatedContainer = AddStandardMember(updatedContainer, newMember);
                }

            }
            else
            {
                // The NEW member does not exist in the OLD container (based on identity).
                // Add the new member to the container.
                updatedContainer = AddStandardMember(updatedContainer, newMember);
            }
        }

        // --- Pass 2: Process Old Members (Deletions) ---
        // Create a dictionary of new members by their identity for quick lookup during the DELETION phase
        var newMemberLookupForDeletion = newMembersList
             .Select(m => new { Member = m, Identity = GetMemberIdentity(m) })
             .Where(item => item.Identity != null)
             .ToLookup(item => item.Identity, item => item.Member);


        var oldMembersToDelete = new List<MemberDeclarationSyntax>();

        // Iterate through the members in the OLD container to find deletions
        foreach (var oldMember in oldMembersList)
        {
            string identity = GetMemberIdentity(oldMember);

            // If identity is null, we didn't handle this type for matching, so don't try to delete it automatically
            if (identity == null) continue;

            // Try to find a corresponding member in the NEW container with the same identity AND kind
            var newCandidates = newMemberLookupForDeletion.Contains(identity) ? newMemberLookupForDeletion[identity].ToList() : new List<MemberDeclarationSyntax>();

            bool foundMatchInNew = newCandidates.Any(newCandidate => newCandidate.Kind() == oldMember.Kind());

            // If no match found in the NEW code with the same identity AND kind, this old member is considered deleted.
            if (!foundMatchInNew)
            {
                oldMembersToDelete.Add(oldMember);
            }
            // Note: If identity matches but Kind differs, it's not considered a "match" for deletion purposes here,
            // so the old member is kept unless its identity matches a new member of the *same* kind.
            // This prevents deleting an old method if a property with the same name was added.
        }


        // Remove the unmatched old members from the container
        if (oldMembersToDelete.Any())
        {
            updatedContainer = updatedContainer.RemoveNodes(oldMembersToDelete, SyntaxRemoveOptions.KeepExteriorTrivia);
        }

        return updatedContainer;
    }

    // Helper to get the list of standard MemberDeclarationSyntax members from a container
    private SyntaxList<MemberDeclarationSyntax> GetStandardMembers(SyntaxNode container)
    {
        return container switch
        {
            CompilationUnitSyntax unit => unit.Members,
            NamespaceDeclarationSyntax ns => ns.Members,
          //  BaseTypeDeclarationSyntax type => type.Members, // Class, Struct, Interface, Record
            // EnumDeclarationSyntax handled separately in MergeMembersInContainer
            _ => default // Return default for unsupported container types
        };
    }

    // Helper to add a standard MemberDeclarationSyntax member to a container node (returns a new container node)
    private SyntaxNode AddStandardMember(SyntaxNode container, MemberDeclarationSyntax memberToAdd)
    {
        // Could add logic here to insert in a specific order (e.g., alphabetical, by kind)
        // For simplicity, AddMembers usually adds to the end.
        return container switch
        {
            CompilationUnitSyntax unit => unit.AddMembers(memberToAdd),
            NamespaceDeclarationSyntax ns => ns.AddMembers(memberToAdd),
           // BaseTypeDeclarationSyntax type => type.AddAttributeLists(memberToAdd), // Class, Struct, Interface, Record
            _ => container // Return original container for unsupported types
        };
    }

    // Specific merger for Enum members (EnumMemberDeclarationSyntax)
    private EnumDeclarationSyntax MergeEnumMembers(EnumDeclarationSyntax oldEnum, EnumDeclarationSyntax newEnum)
    {
        var oldMembers = oldEnum.Members; // SeparatedSyntaxList<EnumMemberDeclarationSyntax>
        var newMembers = newEnum.Members; // SeparatedSyntaxList<EnumMemberDeclarationSyntax>

        EnumDeclarationSyntax updatedEnum = oldEnum;

        // Map new members by identity (identifier text) for addition/modification pass
        var newMemberMap = newMembers.ToDictionary(m => m.Identifier.Text);

        // Track matched old members for deletion
        var matchedOldMembers = new HashSet<EnumMemberDeclarationSyntax>();

        // --- Pass 1: Process New Enum Members (Additions and Modifications) ---
        foreach (var newMember in newMembers)
        {
            var identity = newMember.Identifier.Text;

            // Find corresponding member in old enum
            if (newMemberMap.TryGetValue(identity, out var newMemberToCheckInOld)) // Lookup new member in NEW map (this is backwards)
            {
                // Corrected lookup in OLD members
                var oldMember = oldMembers.FirstOrDefault(m => m.Identifier.Text == identity);

                if (oldMember != null)
                {
                    // Found a corresponding member
                    matchedOldMembers.Add(oldMember); // Mark as matched

                    // Check if they are different
                    if (!SyntaxFactory.AreEquivalent(oldMember, newMember))
                    {
                        // Members are different, replace the old one with the new one
                        updatedEnum = updatedEnum.ReplaceNode(oldMember, newMember);
                    }
                    // If equivalent, do nothing
                }
                else
                {
                    // The new member does not exist in the old enum, add it.
                    updatedEnum = updatedEnum.AddMembers(newMember);
                }
            }
            else
            {
                // This case should not happen if iterating newMembers and using its own identity, but belt-and-suspenders.
                // If for some reason GetMemberIdentity returned null or was complex for EnumMember, handle it.
                // Console.WriteLine($"Warning: Could not identify new enum member: {newMember.Identifier.Text}. Skipping.");
            }
        }

        // --- Pass 2: Process Old Enum Members (Deletions) ---
        // Map new members by identity (identifier text) for deletion pass
        var newMemberMapForDeletion = newMembers.ToDictionary(m => m.Identifier.Text);

        var oldMembersToDelete = new List<EnumMemberDeclarationSyntax>();

        // Iterate through the members in the OLD enum
        foreach (var oldMember in oldMembers)
        {
            var identity = oldMember.Identifier.Text;

            // If the old member's identity is NOT found in the new member map, it's deleted.
            if (!newMemberMapForDeletion.ContainsKey(identity))
            {
                oldMembersToDelete.Add(oldMember);
            }
        }


        // Remove the unmatched old members
        if (oldMembersToDelete.Any())
        {
            // RemoveNodes works on SeparatedSyntaxList too
            updatedEnum = updatedEnum.RemoveNodes(oldMembersToDelete, SyntaxRemoveOptions.KeepExteriorTrivia);
        }

        // Optional: Could also merge attributes on the EnumDeclarationSyntax itself here if needed
        // e.g. if (!SyntaxFactory.AreEquivalent(oldEnum.AttributeLists, newEnum.AttributeLists))
        //      updatedEnum = updatedEnum.WithAttributeLists(newEnum.AttributeLists);


        return updatedEnum;
    }


    // Helper to check if a node is a container type that holds members we recurse into
    private bool IsContainerType(SyntaxNode node)
    {
        // Note: CompilationUnitSyntax and EnumDeclarationSyntax are handled as special cases or base cases for recursion.
        // This checks for types that contain MemberDeclarationSyntax lists.
        return node is NamespaceDeclarationSyntax ||
               node is BaseTypeDeclarationSyntax; // Includes Class, Struct, Interface, Record
    }


    // Generates a unique identifier string for a member for matching purposes.
    // Includes the Kind() to differentiate members with the same name but different types.
    // This is a heuristic and might need refinement for complex cases (e.g., generics, operators,
    // explicit interface implementations, multiple variable fields).
    private string GetMemberIdentity(SyntaxNode member)
    {
        // Handle EnumMemberDeclarationSyntax separately as it's not a MemberDeclarationSyntax
        if (member is EnumMemberDeclarationSyntax enumMember)
        {
            return enumMember.Identifier.Text + "!" + SyntaxKind.EnumMemberDeclaration; // Use "!" as separator to avoid name/kind clashes
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
                    baseIdentity = ((DelegateDeclarationSyntax)memberDeclaration).Identifier.Text + ((DelegateDeclarationSyntax)memberDeclaration).ParameterList.ToString(); // Include parameters
                    break;
                case SyntaxKind.MethodDeclaration:
                    baseIdentity = ((MethodDeclarationSyntax)memberDeclaration).Identifier.Text + ((MethodDeclarationSyntax)memberDeclaration).ParameterList.ToString() + GetMethodGenericParametersString(((MethodDeclarationSyntax)memberDeclaration).TypeParameterList); // Include parameters and generics
                    break;
                case SyntaxKind.ConstructorDeclaration:
                    baseIdentity = ((ConstructorDeclarationSyntax)memberDeclaration).ParameterList.ToString(); // Only parameters needed, name is fixed
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
                    // For events, might need to be more robust, but name is common.
                    // EventFieldDeclaration can declare multiple variables, using first one is a simplification.
                    var eventDecl = memberDeclaration as EventDeclarationSyntax;
                    var eventFieldDecl = memberDeclaration as EventFieldDeclarationSyntax;
                    baseIdentity = eventDecl?.Identifier.Text ?? eventFieldDecl?.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                    break;
                case SyntaxKind.FieldDeclaration:
                    // FieldDeclaration can declare multiple variables, using first one is a simplification.
                    baseIdentity = ((FieldDeclarationSyntax)memberDeclaration).Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                    break;
                case SyntaxKind.OperatorDeclaration:
                    baseIdentity = ((OperatorDeclarationSyntax)memberDeclaration).OperatorToken.ValueText + ((OperatorDeclarationSyntax)memberDeclaration).ParameterList.ToString(); // Operator token + parameters
                    break;
                case SyntaxKind.ConversionOperatorDeclaration:
                    baseIdentity = ((ConversionOperatorDeclarationSyntax)memberDeclaration).Type.ToString() + ((ConversionOperatorDeclarationSyntax)memberDeclaration).ParameterList.ToString(); // Return type + parameters
                    break;
                // Add cases for other member types as needed

                default:
                    return null; // Return null for unhandled member types
            }
            // Append kind to ensure uniqueness across different member types with same base name
            return baseIdentity + "!" + memberDeclaration.Kind();
        }

        // For nodes that are not MemberDeclarationSyntax or EnumMemberDeclarationSyntax (like CompilationUnit, UsingDirective, AttributeList)
        // these should be handled by specific merge logic, not this generic member merging loop.
        return null;
    }

    // Helper to get a string representation of method generic type parameters
    private string GetMethodGenericParametersString(TypeParameterListSyntax typeParameterList)
    {
        if (typeParameterList == null) return string.Empty;
        // Include generic parameter names for identity
        return "<" + string.Join(",", typeParameterList.Parameters.Select(p => p.Identifier.Text)) + ">";
    }

    // Note on Conflicts:
    // This implementation resolves conflicts by prioritizing the 'new' version when a member is found to be different.
    // If identity matches but Kind differs, the old node is kept and the new one is not added by the recursive merger.
    // True text-level conflict detection within member bodies (e.g., same line modified differently)
    // is not performed and would require a different approach (e.g., text diffing combined with AST).
}