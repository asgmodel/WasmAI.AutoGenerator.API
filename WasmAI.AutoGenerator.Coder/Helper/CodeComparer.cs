using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

public class ComprehensiveCodeMerger3
{
    private readonly CSharpParseOptions _parseOptions;

    private static readonly  GeminiCodeService  geminiCodeService = new GeminiCodeService();

    private static readonly AdvGeminiCodeService advgeminiCodeService = new AdvGeminiCodeService();

    public ComprehensiveCodeMerger3(CSharpParseOptions parseOptions = null)
    {
        _parseOptions = parseOptions ?? new CSharpParseOptions(LanguageVersion.Latest);
    }

    private SyntaxTree ParseCode(string code, string filePath, string label)
    {
        var tree = CSharpSyntaxTree.ParseText(code, _parseOptions, path: filePath);
        ValidateCode(tree, label);
        return tree;
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

    public string MergeCode(string oldCode, string newCode, string oldFilePath = "old.cs", string newFilePath = "new.cs",bool isusedai=true)
    {

        var oldTree = ParseCode(oldCode, oldFilePath, "Old Code");
        var newTree = ParseCode(newCode, newFilePath, "New Code");

        var oldRoot = (CompilationUnitSyntax)oldTree.GetRoot();
        var newRoot = (CompilationUnitSyntax)newTree.GetRoot();
        if (isusedai)
        {
            try
            {

                var txtcode = geminiCodeService.MergeCodesAsync2(oldCode, newCode).Result;

                var newTree2 = ParseCode(txtcode, newFilePath, "Merged Code");

                ValidateCode(newTree2, "Merged Code");
                return txtcode;

            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error during merge: {ex.Message}");

            }
        }

        Console.WriteLine($"Starting merge of '{Path.GetFileName(newFilePath)}' into '{Path.GetFileName(oldFilePath)}'...");

        var mergedRoot = MergeCompilationUnits(oldRoot, newRoot);

        return mergedRoot.NormalizeWhitespace().ToFullString();
    }

    public async Task<string> MergeCodeValidator(string modelName, string modelStructure, string templateInstructions, string newFilePath = "new.cs", bool isusedai = true)
    {

     
     
        {
            try
            {

                var txtcode =await advgeminiCodeService.GenerateValidatorFromModelAsync(modelName, modelStructure, templateInstructions);

                var newTree2 = ParseCode(txtcode, newFilePath, "Merged Code");

                ValidateCode(newTree2, "Merged Code");
                return txtcode;

            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error during merge: {ex.Message}");

            }
            return string.Empty;
        }

  
      

    }

    private CompilationUnitSyntax MergeCompilationUnits(CompilationUnitSyntax oldUnit, CompilationUnitSyntax newUnit)
    {
        var mergedUsings = MergeUsings(oldUnit.Usings, newUnit.Usings);
        var mergedAttributeLists = MergeAttributeLists(oldUnit.AttributeLists, newUnit.AttributeLists);
        var mergedMembers = MergeMembers(oldUnit.Members, newUnit.Members);

        return SyntaxFactory.CompilationUnit()
            .WithUsings(mergedUsings)
            .WithAttributeLists(mergedAttributeLists)
            .WithMembers(mergedMembers)
            .WithEndOfFileToken(oldUnit.EndOfFileToken); // Preserve EOF token from old or new
    }

    private SyntaxList<UsingDirectiveSyntax> MergeUsings(SyntaxList<UsingDirectiveSyntax> oldUsings, SyntaxList<UsingDirectiveSyntax> newUsings)
    {
        var set = new HashSet<string>(oldUsings.Select(u => u.NormalizeWhitespace().ToFullString()));
        var combinedUsings = oldUsings.ToList();

        foreach (var newUsing in newUsings)
        {
            if (set.Add(newUsing.NormalizeWhitespace().ToFullString()))
            {
                combinedUsings.Add(newUsing);
            }
        }
        // Sort usings for consistency, optional
        // combinedUsings.Sort((u1, u2) => u1.Name.ToString().CompareTo(u2.Name.ToString()));
        return SyntaxFactory.List(combinedUsings);
    }

    private SyntaxList<AttributeListSyntax> MergeAttributeLists(SyntaxList<AttributeListSyntax> oldAttributes, SyntaxList<AttributeListSyntax> newAttributes)
    {
        // Merging global attributes (assembly/module)
        var set = new HashSet<string>(oldAttributes.Select(a => a.NormalizeWhitespace().ToFullString()));
        var combinedAttributes = oldAttributes.ToList();

        foreach (var newAttrList in newAttributes)
        {
            if (set.Add(newAttrList.NormalizeWhitespace().ToFullString()))
            {
                combinedAttributes.Add(newAttrList);
            }
        }
        return SyntaxFactory.List(combinedAttributes);
    }

    private SyntaxList<MemberDeclarationSyntax> MergeMembers(SyntaxList<MemberDeclarationSyntax> oldMembers, SyntaxList<MemberDeclarationSyntax> newMembers)
    {
        var memberMap = new Dictionary<string, MemberDeclarationSyntax>();

        // Add old members first
        foreach (var oldMember in oldMembers)
        {
            var key = GetTopLevelMemberKey(oldMember);
            if (key != null && !memberMap.ContainsKey(key)) // Should not happen if keys are unique
            {
                memberMap[key] = oldMember;
            }
        }

        // Merge or add new members
        foreach (var newMember in newMembers)
        {
            var key = GetTopLevelMemberKey(newMember);
            if (key == null) continue; // Skip unknown member types or members without a clear key

            if (memberMap.TryGetValue(key, out var existingMember))
            {
                // Member exists, attempt to merge based on type
                memberMap[key] = MergeSpecificMember(existingMember, newMember);
            }
            else
            {
                // New member, add it
                memberMap[key] = newMember;
            }
        }

        return SyntaxFactory.List(memberMap.Values);
    }

    private MemberDeclarationSyntax MergeSpecificMember(MemberDeclarationSyntax oldMember, MemberDeclarationSyntax newMember)
    {
        // If types don't match, prefer new (could be a refactor)
        if (oldMember.Kind() != newMember.Kind())
        {
            Console.WriteLine($"Warning: Member '{GetTopLevelMemberKey(oldMember)}' changed kind. Taking new version.");
            return newMember;
        }

        return (oldMember, newMember) switch
        {
            (ClassDeclarationSyntax o, ClassDeclarationSyntax n) => MergeClassDeclarations(o, n),
            (StructDeclarationSyntax o, StructDeclarationSyntax n) => MergeStructDeclarations(o, n),
            (InterfaceDeclarationSyntax o, InterfaceDeclarationSyntax n) => MergeInterfaceDeclarations(o, n),
            (EnumDeclarationSyntax o, EnumDeclarationSyntax n) => MergeEnumDeclarations(o, n),
            (NamespaceDeclarationSyntax o, NamespaceDeclarationSyntax n) => MergeNamespaceDeclarations(o, n),
            // For other types (delegates, etc.), if they have the same key, the new one overwrites.
            // This is because their "content" is usually just their declaration.
            _ => newMember
        };
    }

    private string GetTopLevelMemberKey(MemberDeclarationSyntax member)
    {
        return member switch
        {
            BaseTypeDeclarationSyntax typeDecl => $"{typeDecl.Kind()}:{typeDecl.Identifier.ValueText}", // Class, Struct, Interface, Enum
            NamespaceDeclarationSyntax nsDecl => $"namespace:{nsDecl.Name.ToString()}",
            DelegateDeclarationSyntax delegateDecl => $"delegate:{delegateDecl.Identifier.ValueText}:{delegateDecl.ReturnType.ToString()}({string.Join(",", delegateDecl.ParameterList.Parameters.Select(p => p.Type?.ToString()))})",
            GlobalStatementSyntax globalStmt => $"global:{globalStmt.Statement.ToString().GetHashCode()}", // Less precise, but best effort
            FileScopedNamespaceDeclarationSyntax fsnsDecl => $"filescopednamespace:{fsnsDecl.Name.ToString()}",
            _ => $"unknownToplevel:{member.Kind()}:{member.ToString().GetHashCode()}" // Fallback
        };
    }

    // --- Specific Merger Functions ---

    private ClassDeclarationSyntax MergeClassDeclarations(ClassDeclarationSyntax oldClass, ClassDeclarationSyntax newClass)
    {
        Console.WriteLine($"Merging class: {oldClass.Identifier.ValueText}");
        var mergedBaseList = MergeBaseList(oldClass.BaseList, newClass.BaseList);
        var mergedTypeParameters = MergeTypeParameterList(oldClass.TypeParameterList, newClass.TypeParameterList);
        var mergedConstraintClauses = MergeConstraintClauses(oldClass.ConstraintClauses, newClass.ConstraintClauses);
        var mergedAttributeLists = MergeAttributeLists(oldClass.AttributeLists, newClass.AttributeLists); // Attributes on the class itself
        var mergedModifiers = MergeModifiers(oldClass.Modifiers, newClass.Modifiers);

        var mergedMembers = MergeClassInternalMembers(oldClass.Members, newClass.Members);

        return newClass // Start with new class structure, apply merged parts
            .WithIdentifier(oldClass.Identifier) // Keep old name if that's the policy, or newClass.Identifier
            .WithAttributeLists(mergedAttributeLists)
            .WithModifiers(mergedModifiers)
            .WithBaseList(mergedBaseList)
            .WithTypeParameterList(mergedTypeParameters)
            .WithConstraintClauses(mergedConstraintClauses)
            .WithMembers(mergedMembers);
    }

    private StructDeclarationSyntax MergeStructDeclarations(StructDeclarationSyntax oldStruct, StructDeclarationSyntax newStruct)
    {
        Console.WriteLine($"Merging struct: {oldStruct.Identifier.ValueText}");
        // Similar logic to MergeClassDeclarations
        var mergedBaseList = MergeBaseList(oldStruct.BaseList, newStruct.BaseList);
        var mergedTypeParameters = MergeTypeParameterList(oldStruct.TypeParameterList, newStruct.TypeParameterList);
        var mergedConstraintClauses = MergeConstraintClauses(oldStruct.ConstraintClauses, newStruct.ConstraintClauses);
        var mergedAttributeLists = MergeAttributeLists(oldStruct.AttributeLists, newStruct.AttributeLists);
        var mergedModifiers = MergeModifiers(oldStruct.Modifiers, newStruct.Modifiers);

        var mergedMembers = MergeClassInternalMembers(oldStruct.Members, newStruct.Members); // Re-use class member merging logic

        return newStruct
            .WithIdentifier(oldStruct.Identifier)
            .WithAttributeLists(mergedAttributeLists)
            .WithModifiers(mergedModifiers)
            .WithBaseList(mergedBaseList)
            .WithTypeParameterList(mergedTypeParameters)
            .WithConstraintClauses(mergedConstraintClauses)
            .WithMembers(mergedMembers);
    }

    private InterfaceDeclarationSyntax MergeInterfaceDeclarations(InterfaceDeclarationSyntax oldInterface, InterfaceDeclarationSyntax newInterface)
    {
        Console.WriteLine($"Merging interface: {oldInterface.Identifier.ValueText}");
        var mergedBaseList = MergeBaseList(oldInterface.BaseList, newInterface.BaseList);
        var mergedTypeParameters = MergeTypeParameterList(oldInterface.TypeParameterList, newInterface.TypeParameterList);
        var mergedConstraintClauses = MergeConstraintClauses(oldInterface.ConstraintClauses, newInterface.ConstraintClauses);
        var mergedAttributeLists = MergeAttributeLists(oldInterface.AttributeLists, newInterface.AttributeLists);
        var mergedModifiers = MergeModifiers(oldInterface.Modifiers, newInterface.Modifiers);

        // Interfaces can also have methods, properties, events.
        var mergedMembers = MergeClassInternalMembers(oldInterface.Members, newInterface.Members); // Re-use

        return newInterface
            .WithIdentifier(oldInterface.Identifier)
            .WithAttributeLists(mergedAttributeLists)
            .WithModifiers(mergedModifiers)
            .WithBaseList(mergedBaseList)
            .WithTypeParameterList(mergedTypeParameters)
            .WithConstraintClauses(mergedConstraintClauses)
            .WithMembers(mergedMembers);
    }

    private EnumDeclarationSyntax MergeEnumDeclarations(EnumDeclarationSyntax oldEnum, EnumDeclarationSyntax newEnum)
    {
        Console.WriteLine($"Merging enum: {oldEnum.Identifier.ValueText}");
        var memberMap = new Dictionary<string, EnumMemberDeclarationSyntax>();

        foreach (var member in oldEnum.Members.Concat(newEnum.Members))
        {
            memberMap[member.Identifier.ValueText] = member; // New overwrites old if same name
        }

        var mergedAttributeLists = MergeAttributeLists(oldEnum.AttributeLists, newEnum.AttributeLists);
        var mergedModifiers = MergeModifiers(oldEnum.Modifiers, newEnum.Modifiers);
        var mergedBaseList = oldEnum.BaseList ?? newEnum.BaseList; // Enum base is optional (int by default)

        return newEnum
            .WithIdentifier(oldEnum.Identifier)
            .WithAttributeLists(mergedAttributeLists)
            .WithModifiers(mergedModifiers)
            .WithBaseList(mergedBaseList)
            .WithMembers(SyntaxFactory.SeparatedList(memberMap.Values));
    }

    private NamespaceDeclarationSyntax MergeNamespaceDeclarations(NamespaceDeclarationSyntax oldNs, NamespaceDeclarationSyntax newNs)
    {
        Console.WriteLine($"Merging namespace: {oldNs.Name}");
        // Namespaces merge their contents (usings, externs, members)
        var mergedUsings = MergeUsings(oldNs.Usings, newNs.Usings);
        var mergedExterns = MergeExterns(oldNs.Externs, newNs.Externs); // Handle extern alias directives
        var mergedMembers = MergeMembers(oldNs.Members, newNs.Members); // Recursive call for members inside namespace

        return SyntaxFactory.NamespaceDeclaration(oldNs.Name) // Or newNs.Name if policy changes
            .WithUsings(mergedUsings)
            .WithExterns(mergedExterns)
            .WithMembers(mergedMembers);
    }

    private SyntaxList<ExternAliasDirectiveSyntax> MergeExterns(SyntaxList<ExternAliasDirectiveSyntax> oldExterns, SyntaxList<ExternAliasDirectiveSyntax> newExterns)
    {
        var set = new HashSet<string>(oldExterns.Select(e => e.Identifier.ValueText));
        var combined = oldExterns.ToList();
        foreach (var newExtern in newExterns)
        {
            if (set.Add(newExtern.Identifier.ValueText))
            {
                combined.Add(newExtern);
            }
        }
        return SyntaxFactory.List(combined);
    }
    // (داخل كلاس ComprehensiveCodeMerger)
    // (داخل كلاس ComprehensiveCodeMerger)

    private SyntaxList<MemberDeclarationSyntax> MergeClassInternalMembers(SyntaxList<MemberDeclarationSyntax> oldMembers, SyntaxList<MemberDeclarationSyntax> newMembers)
    {
        var memberMap = new Dictionary<string, MemberDeclarationSyntax>();
        var allConstructors = new List<ConstructorDeclarationSyntax>();
        var otherOldMembers = new List<MemberDeclarationSyntax>();
        var otherNewMembers = new List<MemberDeclarationSyntax>();

        // 1. Separate constructors from other members for both old and new lists
        foreach (var member in oldMembers)
        {
            if (member is ConstructorDeclarationSyntax ctor)
                allConstructors.Add(ctor); // Mark as from old or add source info if needed
            else
                otherOldMembers.Add(member);
        }
        foreach (var member in newMembers)
        {
            if (member is ConstructorDeclarationSyntax ctor)
                allConstructors.Add(ctor); // Mark as from new
            else
                otherNewMembers.Add(member);
        }

        // 2. Process other members (non-constructors) using the existing map logic
        foreach (var oldMember in otherOldMembers)
        {
            var key = GetInternalMemberSignature(oldMember);
            if (key != null)
            {
                memberMap[key] = oldMember;
            }
        }

        foreach (var newMember in otherNewMembers)
        {
            var key = GetInternalMemberSignature(newMember);
            if (key == null) continue;

            if (memberMap.TryGetValue(key, out var existingMember))
            {
                // Handle merging for non-constructor members as before
                if (existingMember is ClassDeclarationSyntax ocd && newMember is ClassDeclarationSyntax ncd)
                    memberMap[key] = MergeClassDeclarations(ocd, ncd);
                // ... (add other specific member type merges: Struct, Interface, Enum)
                else
                    memberMap[key] = newMember; // New overwrites old for simple members
            }
            else
            {
                memberMap[key] = newMember;
            }
        }

        // 3. Select the single best constructor
        ConstructorDeclarationSyntax bestConstructor = null;
        if (allConstructors.Any())
        {
            // Sort by parameter count descending, then by source (new preferred over old for ties)
            // To prefer "new" for ties in param count, we need to know the source.
            // Let's assume for simplicity for now: if a new version of a constructor exists (same signature),
            // it would have replaced the old one in the `allConstructors` list if we added new ones last
            // and used a dictionary for constructors temporarily keyed by full signature.
            // Or, a simpler approach:
            var sortedConstructors = allConstructors
                .OrderByDescending(c => c.ParameterList.Parameters.Count)
                //.ThenBy(c => IsFromNewCollection(c, newMembers)) // Requires tracking source
                .ToList();

            // A more direct way if new always "updates" old for the same signature:
            // First, get unique constructors, preferring new ones if signatures match
            var uniqueConstructorsMap = new Dictionary<string, ConstructorDeclarationSyntax>();
            foreach (var ctor in oldMembers.OfType<ConstructorDeclarationSyntax>())
            {
                uniqueConstructorsMap[GetInternalMemberSignature(ctor)] = ctor;
            }
            foreach (var ctor in newMembers.OfType<ConstructorDeclarationSyntax>()) // New overwrites if signature matches
            {
                uniqueConstructorsMap[GetInternalMemberSignature(ctor)] = ctor;
            }

            if (uniqueConstructorsMap.Any())
            {
                bestConstructor = uniqueConstructorsMap.Values
                   .OrderByDescending(c => c.ParameterList.Parameters.Count)
                   .ThenByDescending(c => newMembers.Contains(c)) // Prefer new if param count is same
                   .FirstOrDefault();
            }


            if (bestConstructor != null)
            {
                Console.WriteLine($"Selected single constructor for class: {GetInternalMemberSignature(bestConstructor)} with {bestConstructor.ParameterList.Parameters.Count} parameters.");
            }
        }

        // 4. Combine the best constructor (if any) with other merged members
        var finalMembers = memberMap.Values.ToList();
        if (bestConstructor != null)
        {
            // Ensure it's not already added if GetInternalMemberSignature for constructors was used in memberMap
            // (which it shouldn't be if we separated them)
            finalMembers.Insert(0, bestConstructor); // Add constructor, typically at the beginning
        }

        return SyntaxFactory.List(finalMembers);
    }

    // Helper to check if a constructor instance came from the new collection (for tie-breaking)
    // This is a simplified check. A more robust way is to tag them during collection.
    private bool IsFromNewCollection(ConstructorDeclarationSyntax ctor, SyntaxList<MemberDeclarationSyntax> newMemberList)
    {
        // This relies on object reference equality. If Roslyn creates new instances even for identical code,
        // this might not be reliable. A better way is to tag with an enum Old/New.
        return newMemberList.Contains(ctor);
    }

    // GetInternalMemberSignature for ConstructorDeclarationSyntax must still be precise
    // to distinguish overloads if they were temporarily stored in uniqueConstructorsMap.
    // $"C:{c.Identifier.ValueText}({string.Join(",", c.ParameterList.Parameters.Select(p => $"{p.Type?.ToString() ?? "var"} {p.Identifier.ValueText}"))})",
    private string GetInternalMemberSignature(MemberDeclarationSyntax member)
    {
        // More detailed signature for members inside a type
        return member switch
        {
            MethodDeclarationSyntax m => $"M:{m.Identifier.ValueText}:{m.ReturnType.ToString()}({string.Join(",", m.ParameterList.Parameters.Select(p => $"{p.Type?.ToString() ?? "var"} {p.Identifier.ValueText}"))}){(m.TypeParameterList?.ToString() ?? "")}",
            PropertyDeclarationSyntax p => $"P:{p.Identifier.ValueText}:{p.Type.ToString()}",
            FieldDeclarationSyntax f => $"F:{string.Join(",", f.Declaration.Variables.Select(v => v.Identifier.ValueText))}:{f.Declaration.Type.ToString()}",
            ConstructorDeclarationSyntax c => $"C:{c.Identifier.ValueText}({string.Join(",", c.ParameterList.Parameters.Select(p => $"{p.Type?.ToString() ?? "var"} {p.Identifier.ValueText}"))})",
            EventFieldDeclarationSyntax ef => $"EF:{string.Join(",", ef.Declaration.Variables.Select(v => v.Identifier.ValueText))}", // Event field
            EventDeclarationSyntax ed => $"E:{ed.Identifier.ValueText}", // Event with accessors
            IndexerDeclarationSyntax i => $"I:this({string.Join(",", i.ParameterList.Parameters.Select(p => $"{p.Type?.ToString() ?? "var"} {p.Identifier.ValueText}"))}):{i.Type.ToString()}",
            OperatorDeclarationSyntax o => $"OP:{o.OperatorToken.ValueText}({string.Join(",", o.ParameterList.Parameters.Select(p => $"{p.Type?.ToString() ?? "var"} {p.Identifier.ValueText}"))}):{o.ReturnType.ToString()}",
            ConversionOperatorDeclarationSyntax co => $"COP:{co.ImplicitOrExplicitKeyword.ValueText} operator {co.Type.ToString()}({string.Join(",", co.ParameterList.Parameters.Select(p => $"{p.Type?.ToString() ?? "var"} {p.Identifier.ValueText}"))})",
            ClassDeclarationSyntax nestedClass => $"NESTED_CLASS:{nestedClass.Identifier.ValueText}", // Key for merging, actual merge is recursive
            StructDeclarationSyntax nestedStruct => $"NESTED_STRUCT:{nestedStruct.Identifier.ValueText}",
            InterfaceDeclarationSyntax nestedIface => $"NESTED_IFACE:{nestedIface.Identifier.ValueText}",
            EnumDeclarationSyntax nestedEnum => $"NESTED_ENUM:{nestedEnum.Identifier.ValueText}",
            DelegateDeclarationSyntax nestedDelegate => $"NESTED_DELEGATE:{nestedDelegate.Identifier.ValueText}",
            _ => $"unknownInternal:{member.Kind()}:{member.ToString().GetHashCode()}" // Fallback
        };
    }

    // --- Helper methods for merging specific parts of declarations ---
    private BaseListSyntax MergeBaseList(BaseListSyntax oldList, BaseListSyntax newList)
    {
        if (oldList == null && newList == null) return null;
        if (newList == null) return oldList; // Prefer new if available
        if (oldList == null) return newList;

        var set = new HashSet<string>(oldList.Types.Select(t => t.Type.ToString()));
        var combinedTypes = oldList.Types.ToList();
        foreach (var newType in newList.Types)
        {
            if (set.Add(newType.Type.ToString()))
            {
                combinedTypes.Add(newType);
            }
        }
        return SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(combinedTypes));
    }

    private TypeParameterListSyntax MergeTypeParameterList(TypeParameterListSyntax oldList, TypeParameterListSyntax newList)
    {
        if (oldList == null && newList == null) return null;
        // Simple strategy: prefer new list if it exists, otherwise old. More complex merging could be done.
        return newList ?? oldList;
    }

    private SyntaxList<TypeParameterConstraintClauseSyntax> MergeConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> oldClauses, SyntaxList<TypeParameterConstraintClauseSyntax> newClauses)
    {
        // Simple strategy: prefer new clauses if they exist.
        if (newClauses.Any()) return newClauses;
        return oldClauses;
    }

    private SyntaxTokenList MergeModifiers(SyntaxTokenList oldModifiers, SyntaxTokenList newModifiers)
    {
        var set = new HashSet<string>(oldModifiers.Select(m => m.ValueText));
        var combined = oldModifiers.ToList();
        foreach (var newMod in newModifiers)
        {
            if (set.Add(newMod.ValueText)) // ValueText for keywords like "public", "static"
            {
                combined.Add(newMod);
            }
        }
        // Potentially sort them by typical order (public, static, abstract, virtual...) - more complex
        return SyntaxFactory.TokenList(combined);
    }
}
