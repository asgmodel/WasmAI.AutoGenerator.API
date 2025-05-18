using AutoGenerator.ApiFolder;
using AutoGenerator.Helper.Translation;
using AutoGenerator.TM;
using AutoMapper.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Reflection;
using System.Text;


namespace AutoGenerator.Code;

public class DtoGenerator : BaseGenerator, ITGenerator
{





    public static string getPTrns(string name)
    {
        return $@"
            public TranslationData? {name} {{ get; set; }}=new();";
    }


    public static string getTostr(string name)
    {
        return $@"
            public string? {name} {{ get; set; }}";
    }

    public static string getTampBuildRepo(string name, string type, string tag)
    {
        return $@"
                public  class {name}{type}{tag} : BaseBuilderRepository<{name}, {name}BuildRequestDto, {name}BuildResponseDto>, I{name}BuilderRepository<{name}BuildRequestDto, {name}BuildResponseDto>
              ";
    }
    public new string Generate(GenerationOptions options)
    {



        string generatedCode = base.Generate(options);



        return generatedCode;
    }

    public new void SaveToFile(string filePath)
    {


        base.SaveToFile(filePath);
    }


    public void GenrateandSave(GenerationOptions options, string path)
    {

        Generate(options);
        SaveToFile(options.ClassName);
    }




    public static void GenerateAll(string type, string subtype, string NamespaceName, string pathfile)
    {


        var assembly = ApiFolderInfo.AssemblyModels;

        var models = assembly.GetTypes().Where(t => typeof(ITModel).IsAssignableFrom(t) && t.IsClass).ToList();


        bool isbuild = subtype == "Build";
        Type type1 = isbuild ? typeof(ITBuildDto) : typeof(ITShareDto);
        var root = ApiFolderInfo.ROOT.Name;
       



		foreach (var model in models)
        {
            var options = new GenerationOptions($"{model.Name}{NamespaceName}{subtype}{type}", model)
            {
                NamespaceName = $"{root}.DyModels.{type}.{subtype}.{NamespaceName}s",
                AdditionalCode = @"",
                Interfaces = new List<Type> { type1 },
                Usings = new List<string> {  "AutoGenerator" ,"AutoGenerator.Helper.Translation" , model.Namespace}

            };
			options.SubType = subtype;


			if (isbuild)
            {
                options.Usings.Add($"AutoGenerator.Config");
            }
            if (!isbuild)
            {
                options.BaseClass = $"{model.Name}{NamespaceName}Build{type}";

                options.Usings.Add($"{root}.DyModels.{type}.Build.{NamespaceName}s");
               




			}

            else
                options.AdditionalCode += GenerateDtoProperties(options.Properties, models, $"{NamespaceName}{subtype}{type}", NamespaceName,model.Name);


            options.Properties = new List<PropertyInfo>().ToArray();
            ITGenerator generator = new DtoGenerator();
            generator.Generate(options);

            string jsonFile = Path.Combine(pathfile, $"{subtype}/{NamespaceName}/{options.ClassName}.cs");
            generator.SaveToFile(jsonFile);

            Console.WriteLine($"✅ {options.ClassName} has been created successfully!");
        }


        //if (root == null)
        //{
        //    root = new FolderNode("DyModels");
        //    ApiFolderGenerator.ROOT.Children.Add(root);





        //var generator = new DtoGenerator();
        //string reqDtofile= Path.Combine(projectPath, $"{options.ClassName}.cs");

        //generator.GenrateandSave(options, );

    }


    public static void GeneratWithFolder(FolderEventArgs e)
    {
        foreach (var node in e.Node.Children)
        {
            foreach (var child in node.Children)
            {
                GenerateAll(e.Node.Name, node.Name, child.Name, e.FullPath);
            }


        }
    }

    public static string GenerateDtoProperties(PropertyInfo[] properties, List<Type> models, string end,string subtype,string namemodel="")
    {
        var propertyDeclarations = new StringBuilder();

        foreach (var prop in properties)
        {
            // إذا كان النوع من ضمن القائمة models
            if (models.Contains(prop.PropertyType))
            {
                propertyDeclarations.AppendLine($@"
        public {prop.PropertyType.Name}{end}? {prop.Name} {{ get; set; }}");
            }
            // إذا كانت الخاصية من نوع Collection
            else if (prop.PropertyType.IsCollection())
            {
                if (prop.PropertyType.IsArray)
                {
                    var elementType = prop.PropertyType.GetElementType();
                    var typeName = models.Any(m => m.Name == elementType.Name) ? $"{elementType.Name}{end}" : elementType.Name;

                    propertyDeclarations.AppendLine($@"
        public {typeName}[]? {prop.Name} {{ get; set; }}");
                }
                else
                {

                    var genericArguments = prop.PropertyType.GenericTypeArguments;
                    var typeNames = genericArguments.Select(t => models.Any(m => m.Name == t.Name) ? $"{t.Name}{end}" : t.Name);

                    propertyDeclarations.AppendLine($@"
        public ICollection<{string.Join(", ", typeNames)}>? {prop.Name} {{ get; set; }}");
                }

            }
            // إذا كان لديها `ToTranslationAttribute`
            else if (prop.GetCustomAttributes<ToTranslationAttribute>().Any())
            {
                propertyDeclarations.AppendLine(subtype != "ResponseFilter"? getPTrns(prop.Name):getTostr(prop.Name));
            }
            else if (prop.Name.ToLower() == "id" && prop.PropertyType == typeof(string))
            {

                propertyDeclarations.AppendLine($@"
                  public string? Id {{ get; set; }}=$""{namemodel.ToLower()}_{{Guid.NewGuid().ToString()}}"";
");              


            }
            // الحالات الأخرى (الافتراضية)
            else
            {
                propertyDeclarations.AppendLine($@"
        /// <summary>
        /// {prop.Name} property for DTO.
        /// </summary>
        public {CodeGeneratorUtils.GetPropertyTypeName(prop.PropertyType)} {prop.Name} {{ get; set; }}");
            }
        }

        if(subtype== "ResponseFilter")
            propertyDeclarations.AppendLine($@"
           [FilterLGEnabled]
           public string? Lg {{get; set; }}");

            return propertyDeclarations.ToString();
    }



}
