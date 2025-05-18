using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.IO;
using System.Linq;

public static class CodeSaveValidator
{
    // يقوم بإنشاء الشجرة التركيبية (Syntax Tree) والتحقق من الأخطاء وحفظ الكود إذا كان خاليًا من الأخطاء
    public static ValidationResult ValidateAndSave(string code, string filePath, CSharpParseOptions parseOptions = null)
    {
        try
        {
            // إذا كانت الخيارات فارغة، استخدم الخيارات الافتراضية
            parseOptions ??= new CSharpParseOptions(LanguageVersion.Latest);

            var syntaxTree = CSharpSyntaxTree.ParseText(code, parseOptions);

            // فحص الأخطاء في الشجرة التركيبية
            var validationResult = ValidateCode(syntaxTree); // استخدمنا validationResult هنا
            if (!validationResult.IsSuccess)
            {
                return ValidationResult.Fail(validationResult.ErrorMessage);
            }

            // إذا لم توجد أخطاء، نقوم بحفظ الكود في الملف
            SaveCodeToFile(code, filePath);
            return ValidationResult.Success("Code saved successfully.");
        }
        catch (Exception ex)
        {
            return ValidationResult.Fail($"Error occurred: {ex.Message}");
        }
    }

    public static  ValidationResult ValidationCode(string code)
    {
       var  parseOptions = new CSharpParseOptions(LanguageVersion.Latest);

        var syntaxTree = CSharpSyntaxTree.ParseText(code, parseOptions);
        return ValidateCode(syntaxTree);
    }
    // فحص الشجرة التركيبية للتحقق من وجود أخطاء
    private static ValidationResult ValidateCode(SyntaxTree tree)
    {
        var diagnostics = tree.GetDiagnostics()
                              .Where(d => d.Severity == DiagnosticSeverity.Error)
                              .ToList();

        if (diagnostics.Any())
        {
            var errors = string.Join("\n", diagnostics.Select(d => $"Error: {d.GetMessage()} at {d.Location.GetLineSpan()}"));
            return ValidationResult.Fail(errors);
        }

        return ValidationResult.Success();
    }

    // حفظ الكود في الملف إذا كان خاليًا من الأخطاء
    private static void SaveCodeToFile(string code, string filePath)
    {
        try
        {
            // إذا كان الملف موجودًا، نقوم بإعادة الكتابة عليه
            File.WriteAllText(filePath, code);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error saving code to file: {ex.Message}");
        }
    }
}

// نوع ValidationResult لاحتواء النتيجة (نجاح أو فشل)
public class ValidationResult
{
    public bool IsSuccess { get; private set; }
    public string  ErrorMessage { get; private set; }

    public string Code { get;  set; }

    private ValidationResult(bool isSuccess,string code="" ,string errorMessage = null)
    {
        IsSuccess = isSuccess;
        Code = code;
        ErrorMessage = errorMessage;
    }

    // ميثود لإنشاء نتيجة نجاح
    public static ValidationResult Success(string code="",string message = null)
    {
        return new ValidationResult(true, code, message);
    }

    // ميثود لإنشاء نتيجة فشل
    public static ValidationResult Fail(string errorMessage, string code = "")
    {
        return new ValidationResult(false, errorMessage);
    }
}
