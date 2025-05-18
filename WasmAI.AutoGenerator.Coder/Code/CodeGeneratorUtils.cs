using Microsoft.CodeAnalysis;


namespace AutoGenerator.Code;

public static class CodeGeneratorUtils
{
    public static string GetPropertyTypeName(Type propertyType)
    {
        if (propertyType.IsGenericType)
        {
            var genericArguments = propertyType.GetGenericArguments();
            return $"{propertyType.Name.Substring(0, propertyType.Name.IndexOf('`'))}<{string.Join(", ", genericArguments.Select(GetPropertyTypeName))}>";
        }
        else if (propertyType.IsNullableType())
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            return underlyingType != null ? underlyingType.Name : propertyType.Name;
        }
        else if (propertyType == typeof(string))
            return propertyType.Name+"?";
        else
        {
            return propertyType.Name;
        }
    }

    public static bool IsNullableType(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    // تطبيق القالب على النص
    public static string ApplyTemplate(string template, Dictionary<string, string> replacements)
    {
        foreach (var replacement in replacements)
        {
            template = template.Replace($"{{{replacement.Key}}}", replacement.Value);
        }
        return template;
    }
}
