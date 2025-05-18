using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

public class SyntaxTreeValidator
{
    private List<string> _errors = new List<string>();

    public List<string> Errors => _errors;

    // دالة للتحقق من نوع العقدة
    public bool IsNodeOfType(SyntaxNode node, SyntaxKind kind)
    {
        return node.Kind() == kind;
    }

    // دالة لاستخراج موقع العقدة
    public string GetLocation(SyntaxNode node)
    {
        var location = node.GetLocation();
        return location.GetLineSpan().ToString();
    }

    // دالة للتحقق من الشجرة النحوية بالكامل
    public List<string> ValidateSyntaxTree(SyntaxNode rootNode)
    {
        ValidateNodeWithErrors(rootNode);
        return _errors;
    }

    // دالة للتحقق من العقدة مع إضافة الأخطاء إلى القائمة
    private void ValidateNodeWithErrors(SyntaxNode node)
    {
        if (node == null) return;

        // فحص الأخطاء بناءً على نوع العقدة
        switch (node.Kind())
        {
            case SyntaxKind.MethodDeclaration:
                ValidateMethodDeclaration((MethodDeclarationSyntax)node);
                break;
            case SyntaxKind.ClassDeclaration:
                ValidateClassDeclaration((ClassDeclarationSyntax)node);
                break;
            case SyntaxKind.NamespaceDeclaration:
                ValidateNamespaceDeclaration((NamespaceDeclarationSyntax)node);
                break;
            case SyntaxKind.IncompleteMember:
                CheckForIncompleteMembers(node);
                break;
            default:
                // تحقق من العقد الفرعية
                foreach (var child in node.ChildNodes())
                {
                    ValidateNodeWithErrors(child);
                }
                break;
        }
    }

    // دالة للتحقق من طريقة MethodDeclaration
    private void ValidateMethodDeclaration(MethodDeclarationSyntax methodNode)
    {
        // تحقق من اسم الطريقة
        if (string.IsNullOrEmpty(methodNode.Identifier.Text))
        {
            _errors.Add($"Method declaration is missing a name at {GetLocation(methodNode)}.");
        }

        // تحقق من نوع الإرجاع
        if (methodNode.ReturnType.IsMissing)
        {
            _errors.Add($"Method return type is missing at {GetLocation(methodNode)}.");
        }
    }

    // دالة للتحقق من إعلان الفئة ClassDeclaration
    private void ValidateClassDeclaration(ClassDeclarationSyntax classNode)
    {
        // تحقق من اسم الفئة
        if (string.IsNullOrEmpty(classNode.Identifier.Text))
        {
            _errors.Add($"Class declaration is missing a name at {GetLocation(classNode)}.");
        }
    }

    // دالة للتحقق من إعلان الـ Namespace
    private void ValidateNamespaceDeclaration(NamespaceDeclarationSyntax namespaceNode)
    {
        // تحقق من اسم النيمسبيس
        if (string.IsNullOrEmpty(namespaceNode.Name.ToString()))
        {
            _errors.Add($"Namespace declaration is missing a name at {GetLocation(namespaceNode)}.");
        }
    }

    // دالة للتحقق من وجود أعضاء ناقصة
    private void CheckForIncompleteMembers(SyntaxNode node)
    {
        _errors.Add($"Incomplete member declaration found at {GetLocation(node)}.");
    }

    // دالة لفحص العقد المتكررة
    public bool IsRedundantNode(SyntaxNode node, List<SyntaxNode> visitedNodes)
    {
        foreach (var visitedNode in visitedNodes)
        {
            if (visitedNode.IsEquivalentTo(node))
            {
                return true; // العقدة متكررة
            }
        }
        visitedNodes.Add(node);
        return false;
    }

    // دالة لتطبيق التحقق من العقد في الشجرة النحوية
    public void AnalyzeSyntaxTree(SyntaxNode rootNode)
    {
        var visitedNodes = new List<SyntaxNode>();
        ValidateNodeWithErrors(rootNode);

        // طباعة الأخطاء
        foreach (var error in _errors)
        {
            Console.WriteLine(error);
        }
    }
}
