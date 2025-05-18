using AutoGenerator.ApiFolder;
using AutoGenerator.Helper.Translation;
using AutoMapper.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text;


namespace AutoGenerator.Code;

public class VMGenerator : BaseGenerator, ITGenerator
{

    public new string Generate(GenerationOptions options)
    {

        string generatedCode = base.Generate(options);



        return generatedCode;
    }

    public new void SaveToFile(string filePath)
    {


        base.SaveToFile(filePath);
    }


    public static void GenerateAll(string type, string subtype, string NamespaceName, string pathfile)
    {


        var assembly = ApiFolderInfo.AssemblyModels;


        var models = assembly.GetTypes().Where(t => typeof(ITModel).IsAssignableFrom(t) && t.IsClass).ToList();

        





       
        StringBuilder  temp= new StringBuilder();
        var root = ApiFolderInfo.ROOT.Name;


        foreach (var model in models)

        {


            CreateFolder(pathfile, $"{model.Name}");

            NamespaceName = $"{root}.DyModels.{type}s";
            foreach (var subvm in UseVM)
            {

                if (subvm == "Output")
                {
                    var prortyappend = GenerateDtoProperties(model.GetProperties(), models, $"{subvm}{type}", isOutput: true);
                    temp.AppendLine(GetTemplateVM(null, subvm, model.Name, prortyappend));

                }
                else if (subvm == "Create" )
                {
                    var prortyappend = GenerateDtoProperties(model.GetProperties(), models, $"{subvm}{type}", isOutput: false);
                    temp.AppendLine(GetTemplateVM(null, subvm, model.Name, prortyappend));
                }
                else
                    temp.AppendLine(GetTemplateVM(null, subvm, model.Name));



                var options = new GenerationOptions($"{model.Name}{type}", model, isProperties: false)
                {
                    NamespaceName = NamespaceName,
                    Template = temp.ToString()
                                ,
                    Usings = new List<string>
                        {
                            "AutoGenerator",
                            
                            "AutoGenerator.Helper.Translation",
                             model.Namespace






                        }


                };

                ITGenerator generator = new VMGenerator();
                generator.Generate(options);



                string jsonFile = Path.Combine(pathfile, $"{model.Name}/{model.Name}{subvm}{type}.cs");
                generator.SaveToFile(jsonFile);
                temp.Clear();
                Console.WriteLine($"✅ {options.ClassName} has been created successfully!");
            }
        }



    }

    private static void CreateFolder(string path,string namemodel)
    {
       
            string folderPath = Path.Combine(path, namemodel);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        
    }
    
    private static string GetTemplateVM(List<string> usings, string nameSpace, string className,string append="")
    {
        // Initialize a StringBuilder to accumulate the using statements.
        StringBuilder usingStatements = new StringBuilder();

    
        StringBuilder pros= new StringBuilder();

        if (!string.IsNullOrEmpty(append))
            pros.AppendLine(append);
        else
        {
            

            
                pros.AppendLine(@" 
         ///
                public string?  Id { get; set; }");

                
                if (nameSpace == "Filter")
                pros.AppendLine(@"
          ///
   
       
                public string?  Lg { get; set; }");
        }
        if (nameSpace == "Update")
        {

            pros.AppendLine($@" 
         ///
                public {className}CreateVM?  Body {{ get; set; }}");


        }



        return @$"


           /// <summary>
                 /// {className}  property for VM {nameSpace}.
           /// </summary>
        public class {className}{nameSpace}VM :ITVM  
        {{
              
                  {pros.ToString()}
   
                
        }}



     ";
    }





    private static string[] UseVM = new string[] { "Create", "Output", "Update", "Delete", "Info","Filter" };
    public static void GeneratWithFolder(FolderEventArgs e)
    {


            GenerateAll(e.Node.Name, e.Node.Name, e.Node.Name, e.FullPath);
          
            //GenerateAll(e.Node.Name, node.Name, node.Name, e.FullPath);



      
    }


    public static string GenerateDtoProperties(PropertyInfo[] properties, List<Type> models, string end,bool isOutput=false,bool isupdaute=false)
    {
        var propertyDeclarations = new StringBuilder();

        foreach (var prop in properties)
        {

            if ((!isOutput&&!isupdaute) && prop.Name.ToLower() == "id") {  continue; }
            // إذا كان النوع من ضمن القائمة models
            if (models.Contains(prop.PropertyType))
            {

                if (!isOutput)
                    propertyDeclarations.AppendLine($@"
                   // public {prop.PropertyType.Name}{end}? {prop.Name} {{ get; set; }}");


                else
                {
                    propertyDeclarations.AppendLine($@"
                   public {prop.PropertyType.Name}{end}? {prop.Name} {{ get; set; }}");

                }
            }

            // إذا كانت الخاصية من نوع Collection
            else if (prop.PropertyType.IsCollection())
            {
                if (prop.PropertyType.IsArray)
                {
                    var elementType = prop.PropertyType.GetElementType();
                    var typeName = models.Any(m => m.Name == elementType.Name) ? $"{elementType.Name}{end}" : elementType.Name;

                    propertyDeclarations.AppendLine($@"
        //
        public {typeName}[]? {prop.Name} {{ get; set; }}");
                }
                else
                {

                    var genericArguments = prop.PropertyType.GenericTypeArguments;
                    var typeNames = genericArguments.Select(t => models.Any(m => m.Name == t.Name) ? $"{t.Name}{end}" : t.Name);

                    propertyDeclarations.AppendLine($@"
        //
        public List<{string.Join(", ", typeNames)}>? {prop.Name} {{ get; set; }}");
                }

            }
            // إذا كان لديها `ToTranslationAttribute`
            else if (prop.GetCustomAttributes<ToTranslationAttribute>().Any())
            {
                propertyDeclarations.AppendLine(isOutput ? getproString(prop.Name) : getproITranslationData(prop.Name));
            }
            // الحالات الأخرى (الافتراضية)
            else
            {
                propertyDeclarations.AppendLine($@"
        ///
        public {CodeGeneratorUtils.GetPropertyTypeName(prop.PropertyType)} {prop.Name} {{ get; set; }}");
            }
        }

        return propertyDeclarations.ToString();
    }


    public static string getproString(string name)
    {
        return $@"
            //
            public string? {name} {{ get; set; }}";
    }

    public static string getproITranslationData(string name)
    {
        return $@"
            //
            public TranslationData? {name} {{ get; set; }}";
    }
}

