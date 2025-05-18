using System.Reflection;

namespace AutoGenerator.Code;

public class GenerationOptions
{
   
    public string ClassName { get; set; }
    public Type SourceType { get; }
    public string? SubType { get; set; }

    public string? Type { get; set; }
    public string NamespaceName { get; set; } = "GeneratedClasses";
    public string AdditionalCode { get; set; } = "";
    public List<Type> Interfaces { get; set; } = new List<Type>();

    public PropertyInfo[] Properties { get; set; } 

    public GenerationOptions(string className, Type sourceType,bool isProperties=true)
    {
        ClassName = className;
        SourceType = sourceType;
        if(isProperties)
             Properties =sourceType.GetProperties();

    }

    public List<string> Usings { get; set; } = new List<string>();
    public string BaseClass { get; set; } = null;
    public string Template { get; set; } = @"
        public class {ClassName} {BaseClass} {Interfaces}
        {
            {Properties}
            {AdditionalCode}
        }
    ";
}
