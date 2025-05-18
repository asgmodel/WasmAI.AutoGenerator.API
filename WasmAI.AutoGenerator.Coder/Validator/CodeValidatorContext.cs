using AutoGenerator.ApiFolder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WasmAI.AutoGenerator.Code;
using WasmAI.ConditionChecker.Base;
using WasmAI.ConditionChecker.Validators;

public enum ValidatorType
{
    ClassValidator,
    PropertyValidator,
    MethodValidator,
    VariableValidator,
    EnumValidator,
    NamespaceValidator
}

public class CodeValidatorContext<TCode, EValidator> : BaseValidatorContext<TCode, EValidator>
    where TCode : class
   
    where EValidator : Enum
{
    public CodeValidatorContext(ICodeConditionChecker checker) : base(checker) { }

    // قسمة تحقق من نوع كود معين
    protected virtual Task<ConditionResult> ValidateCodeElementAsync(string codeText, ValidatorType validatorType)
    {
        return validatorType switch
        {
            ValidatorType.ClassValidator => ValidateClassesAsync(codeText),
            ValidatorType.PropertyValidator => ValidatePropertiesAsync(codeText),
            ValidatorType.MethodValidator => ValidateMethodsAsync(codeText),
            ValidatorType.NamespaceValidator => ValidateNamespacesAsync(codeText),
            ValidatorType.VariableValidator => ValidateVariablesAsync(codeText),
            ValidatorType.EnumValidator => ValidateEnumsAsync(codeText),
            _ => throw new NotImplementedException("ValidatorType not implemented.")
        };
    }

    // تحقق من الكلاسات
    public virtual async Task<ConditionResult> ValidateClassesAsync(string codeText)
    {
        return await Task.Run(() =>
        {
            var root = CSharpSyntaxTree.ParseText(codeText).GetRoot();
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            if (!classes.Any())
                return ConditionResult.ToFailure(null, "No classes found.");

            foreach (var cls in classes)
            {
                if (string.IsNullOrWhiteSpace(cls.Identifier.Text))
                    return ConditionResult.ToFailure(cls, "Class with missing name found.");
            }

            return ConditionResult.ToSuccess(classes, "All classes are valid.");
        });
    }



    // تحقق من الخصائص
    public virtual async Task<ConditionResult> ValidatePropertiesAsync(string codeText)
    {
        return await Task.Run(() =>
        {
            var root = CSharpSyntaxTree.ParseText(codeText).GetRoot();
            var properties = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

            foreach (var prop in properties)
            {
                if (string.IsNullOrWhiteSpace(prop.Identifier.Text))
                    return ConditionResult.ToFailure(prop, "Property with missing name found.");
                if (prop.Type == null)
                    return ConditionResult.ToFailure(prop, $"Property '{prop.Identifier.Text}' has no type.");
            }

            return ConditionResult.ToSuccess(properties, "All properties are valid.");
        });
    }

    // تحقق من الدوال
    public virtual async Task<ConditionResult> ValidateMethodsAsync(string codeText)
    {
        return await Task.Run(() =>
        {
            var root = CSharpSyntaxTree.ParseText(codeText).GetRoot();
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

            if (!methods.Any())
                return ConditionResult.ToFailure(null, "No methods found.");

            foreach (var method in methods)
            {
                if (string.IsNullOrWhiteSpace(method.Identifier.Text))
                    return ConditionResult.ToFailure(method, "Method with missing name found.");
                if (method.ReturnType == null)
                    return ConditionResult.ToFailure(method, $"Method '{method.Identifier.Text}' is missing a return type.");
                if (method.Body == null && method.ExpressionBody == null)
                    return ConditionResult.ToFailure(method, $"Method '{method.Identifier.Text}' has no body.");
            }

            return ConditionResult.ToSuccess(methods, "All methods are valid.");
        });
    }

    // تحقق من النيم سبيس
    public virtual async Task<ConditionResult> ValidateNamespacesAsync(string codeText)
    {
        return await Task.Run(() =>
        {
            var root = CSharpSyntaxTree.ParseText(codeText).GetRoot();
            var namespaces = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();

            if (!namespaces.Any())
                return ConditionResult.ToFailure(null, "No namespace found.");

            foreach (var ns in namespaces)
            {
                if (string.IsNullOrWhiteSpace(ns.Name.ToString()))
                    return ConditionResult.ToFailure(ns, "Namespace with missing name found.");
            }

            return ConditionResult.ToSuccess(namespaces, "All namespaces are valid.");
        });
    }

    // تحقق من المتغيرات
    public virtual async Task<ConditionResult> ValidateVariablesAsync(string codeText)
    {
        return await Task.Run(() =>
        {
            var root = CSharpSyntaxTree.ParseText(codeText).GetRoot();
            var variables = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList();

            foreach (var variable in variables)
            {
                if (string.IsNullOrWhiteSpace(variable.Identifier.Text))
                    return ConditionResult.ToFailure(variable, "Variable with missing name found.");
            }

            return ConditionResult.ToSuccess(variables, "All variables are valid.");
        });
    }

    // تحقق من الإنمز
    public virtual async Task<ConditionResult> ValidateEnumsAsync(string codeText)
    {
        return await Task.Run(() =>
        {
            var root = CSharpSyntaxTree.ParseText(codeText).GetRoot();
            var enums = root.DescendantNodes().OfType<EnumDeclarationSyntax>().ToList();

            if (!enums.Any())
                return ConditionResult.ToFailure(null, "No enums found.");

            foreach (var enumItem in enums)
            {
                if (string.IsNullOrWhiteSpace(enumItem.Identifier.Text))
                    return ConditionResult.ToFailure(enumItem, "Enum with missing name found.");
            }

            return ConditionResult.ToSuccess(enums, "All enums are valid.");
        });
    }

    // استبدال الجزء الذي تم تغييره
    public virtual string ReplaceChangedPart(string originalCode, string generatedCode)
    {
        return originalCode == generatedCode ? originalCode : generatedCode;
    }


    protected override Task<TCode> GetModel(string? id)
    {
        return Task.FromResult(default(TCode));
    }

    public virtual IEnumerable<Type>? GetModels<TModel>()
    {
        var assembly = ApiFolderInfo.AssemblyModels;
        return assembly?.GetTypes().Where(t => typeof(TModel).IsAssignableFrom(t) && t.IsClass).ToList();
    }
}