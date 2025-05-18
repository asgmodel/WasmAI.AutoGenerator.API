using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutoGenerator.Code.VM.v1
{
    public class CodeVM
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? SubType { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? PathFile { get; set; }
        public string? NamespaceName { get; set; }
        public string AdditionalCode { get; set; } = string.Empty;
        public List<string>? Usings { get; set; }
        public string? BaseClass { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Type? TypeModel { get; set; } 

        public  bool IsChanged { get; set; } = false;
    }

    public class SwapCodeRequest
    {
        public CodeIdentifier? Source { get; set; }
        public CodeIdentifier? Target { get; set; }
    }

    public class CodeIdentifier
    {
        public string? Id { get; set; }
        public string? PathFile { get; set; }
    }

    public class MergeCodeRequest
    {
        public string NewCode { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? FilePath2 { get; set; }
    }

    public class ExtendedProblemDetails : ProblemDetails
    {
        public new IDictionary<string, object?> Extensions { get; set; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    public class CategorySubType
    {
        public string Category { get; set; } = string.Empty;
        public List<string> SubTypes { get; set; } = new List<string>();
    }

    public class MetadataLists
    {
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Namespaces { get; set; } = new List<string>();
        public List<string> PathFiles { get; set; } = new List<string>();
        public List<string> Ids { get; set; } = new List<string>();
        public List<string> Names { get; set; } = new List<string>();
        public List<string> BaseClasses { get; set; } = new List<string>();
        public List<string> UniqueUsings { get; set; } = new List<string>();
        public List<CategorySubType> CategorySubTypes { get; set; } = new List<CategorySubType>();
    }

    public class CountSummaries
    {
        public int TotalCount { get; set; }
        public int TotalLinesOfCode { get; set; }
        public Dictionary<string, int> CountByCategory { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> CountByNamespace { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> CountByBaseClass { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public string? SpecificUsingLine { get; set; }
        public int? CountForSpecificUsing { get; set; }
    }

    public class BulkDeleteRequest
    {
        public List<string>? Ids { get; set; }
        public string? Category { get; set; }
        public string? SubType { get; set; }
        public string? PathFile { get; set; }
    }

    public class TextGenerationRequest
    {
        public string? Description { get; set; } = "";
        public string? TargetModelName { get; set; }
    }

    public class ModelStructureRequest
    {
        public string ModelStructure { get; set; } = string.Empty;
    }
}