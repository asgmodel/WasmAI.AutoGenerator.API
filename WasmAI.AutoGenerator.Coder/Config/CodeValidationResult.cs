// ===== File: CodeValidationResult.cs (Same) =====
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CodeValidationResult
{
    public bool IsValid { get; private set; }
    public List<string> Errors { get; }
    public List<string> Warnings { get; }

    public CodeValidationResult()
    {
        Errors = new List<string>();
        Warnings = new List<string>();
        IsValid = true; // Assume valid until an error is added
    }

    public void AddError(string message)
    {
        Errors.Add(message);
        IsValid = false; // Mark as invalid if any error is added
    }

    public void AddWarning(string message)
    {
        Warnings.Add(message);
    }

    public override string ToString()
    {
        if (IsValid && !Warnings.Any())
        {
            return "Validation Successful: No issues found.";
        }

        var sb = new StringBuilder();
        if (!IsValid)
        {
            sb.AppendLine("Validation Failed:");
            foreach (var error in Errors)
            {
                sb.AppendLine($"- ERROR: {error}");
            }
        }
        if (Warnings.Any())
        {
            if (sb.Length > 0) sb.AppendLine(); // Add blank line if there were errors
            sb.AppendLine("Validation Warnings:");
            foreach (var warning in Warnings)
            {
                sb.AppendLine($"- WARNING: {warning}");
            }
        }

        return sb.ToString();
    }
}