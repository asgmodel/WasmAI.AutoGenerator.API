using AutoGenerator.ApiFolder;
using AutoGenerator.Config;
using AutoGenerator.TM;
using Microsoft.CodeAnalysis;
using System.Reflection;


namespace AutoGenerator.Code;

public class ValidatorGenerator : BaseGenerator, ITGenerator
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

    public static void CinfgGenerate(string type, string subtype, string NamespaceName, string pathfile)
    {



        


        var root = ApiFolderInfo.ROOT.Name;
        ITGenerator generator = new ValidatorGenerator();

        NamespaceName = $"{root}.Validators";
        var option1 = new GenerationOptions("t", typeof(ValidatorGenerator), isProperties: false)
        {
            NamespaceName = $"{NamespaceName}.Conditions",
            Template = TmValidators.GetTmConfigValidator("")
                               ,
            Usings = new List<string>
                        {
                            "AutoGenerator",

                            "AutoGenerator.Conditions",

                            "System.Reflection"

                        }


        };

        //

        generator.Generate(option1);

        string jsonFile = Path.Combine(pathfile, $"{subtype}/Config.cs");
        generator.SaveToFile(jsonFile);

        Console.WriteLine($"✅ {option1.ClassName} has been created successfully!");




    }


    public static void CinfgValidatorContext(string type, string subtype, string NamespaceName, string pathfile)
    {

        var root = ApiFolderInfo.ROOT.Name;
        ITGenerator generator = new ValidatorGenerator();

        NamespaceName = $"{root}.Validators";
        var option1 = new GenerationOptions("t", typeof(ValidatorGenerator), isProperties: false)
        {
            NamespaceName = $"{NamespaceName}",
            Template = TmValidators.GetTmIValidatorContext("")
                               ,
            Usings = new List<string>
                        {
                            "AutoGenerator",

                            "AutoGenerator.Conditions",
                            $"{root}.Validators.Conditions"



                        }


        };

        //

        generator.Generate(option1);

        string jsonFile = Path.Combine(pathfile, $"{subtype}/ValidatorContext.cs");
        generator.SaveToFile(jsonFile);

        Console.WriteLine($"✅ {option1.ClassName} has been created successfully!");




    }
    public static void InjoctorGenerate(string type, string subtype, string NamespaceName, string pathfile)
    {

        var root = ApiFolderInfo.ROOT.Name;
        ITGenerator generator = new ValidatorGenerator();

        NamespaceName = $"{root}.Validators";
        var option1 = new GenerationOptions("TFactoryInjector", typeof(ValidatorGenerator), isProperties: false)
        {
            NamespaceName = $"{NamespaceName}.Conditions",
            Template = TmValidators.GetTmTFactoryInjector("")
                               ,
            Usings = new List<string>
                        {
                            "AutoGenerator",

                            "AutoGenerator.Conditions",
                            "AutoGenerator.Notifications",
                            "AutoMapper",
                            ApiFolderInfo.TypeContext.Namespace

                        }


        };

        //

        generator.Generate(option1);


        string jsonFile = Path.Combine(pathfile, $"{subtype}/TFactoryInjector.cs");
        generator.SaveToFile(jsonFile);

        Console.WriteLine($"✅ {option1.ClassName} has been created successfully!");


        var option2 = new GenerationOptions("ITFactoryInjector", typeof(ValidatorGenerator), isProperties: false)
        {
            NamespaceName = $"{NamespaceName}.Conditions",
            Template = TmValidators.GetTmITFactoryInjector("")
                             ,
            Usings = new List<string>
                        {
                            "AutoGenerator",

                            "AutoGenerator.Conditions",

                            "AutoMapper",
                            ApiFolderInfo.TypeContext.Namespace

                        }


        };

        //
        ITGenerator generator2 = new ValidatorGenerator();
        generator2.Generate(option2);

        jsonFile = Path.Combine(pathfile, $"{subtype}/ITFactoryInjector.cs");
        generator2.SaveToFile(jsonFile);

        Console.WriteLine($"✅ {option2.ClassName} has been created successfully!");


    }


    public static void CheckerGenerate(string type, string subtype, string NamespaceName, string pathfile)
    {

        var root = ApiFolderInfo.ROOT.Name;
        ITGenerator generator = new ValidatorGenerator();

        NamespaceName = $"{root}.Validators";
        var option1 = new GenerationOptions("ConditionChecker", typeof(ValidatorGenerator), isProperties: false)
        {
            NamespaceName = NamespaceName,
            Template = TmValidators.GetTmConditionChecker("")
                               ,
            Usings = new List<string>
                        {


                            "AutoGenerator.Conditions",

                           $"{root}.Validators.Conditions",


                        }


        };

        //

        generator.Generate(option1);

        string jsonFile = Path.Combine(pathfile, $"{subtype}/ConditionChecker.cs");
        generator.SaveToFile(jsonFile);

        Console.WriteLine($"✅ {option1.ClassName} has been created successfully!");


        var option2 = new GenerationOptions("IConditionChecker", typeof(ValidatorGenerator), isProperties: false)
        {
            NamespaceName = $"{NamespaceName}",
            Template = TmValidators.GetTmIConditionChecker("")
                             ,
            Usings = new List<string>
                        {


                            "AutoGenerator.Conditions",

                            $"{root}.Validators.Conditions",

                        }


        };

        //
        ITGenerator generator2 = new ValidatorGenerator();
        generator2.Generate(option2);

        jsonFile = Path.Combine(pathfile, $"{subtype}/IConditionChecker.cs");
        generator2.SaveToFile(jsonFile);

        Console.WriteLine($"✅ {option2.ClassName} has been created successfully!");


    }





    public static void GenerateAll(string type, string subtype, string NamespaceName, string pathfile)
    {


        var assembly = ApiFolderInfo.AssemblyModels;


        var models = assembly.GetTypes().Where(t => typeof(ITModel).IsAssignableFrom(t) && t.IsClass).ToList();




        var root = ApiFolderInfo.ROOT.Name;




        NamespaceName = $"{root}.Validators";


        foreach (var model in models)
        {







            var attribute = model.GetCustomAttribute<ValidatorEnabledAttribute>();

            // Return true if the attribute exists and IgnoreMapping is true, otherwise false

            if (!(attribute != null && attribute.IsValidatorped))
            {


                var options = new GenerationOptions(model.Name, model, isProperties: false)
                {
                    NamespaceName = NamespaceName,
                    Template = createVoldetor(model.Name)
                               ,
                    Usings = new List<string>
                        {
                            "AutoGenerator",

                            "AutoGenerator.Helper.Translation",
                            "AutoGenerator.Conditions",
                            
                            $"{NamespaceName}.Conditions",
                            $"{model.Namespace}",
                            $"{root}.Validators.Conditions",
                            $"WasmAI.ConditionChecker.Base",







                        }


                };

                ITGenerator generator = new ValidatorGenerator();
                generator.Generate(options);

                string jsonFile = Path.Combine(pathfile, $"{subtype}/{model.Name}Validator.cs");
                generator.SaveToFile(jsonFile);

                Console.WriteLine($"✅ {options.ClassName} has been created successfully!");
            }
        }
    }











    private static void CreateFolder(string path, string namemodel)
    {

        string folderPath = Path.Combine(path, namemodel);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

    }


    private static string createVoldetor(string className)
    {


        return TmValidators.GetTmValidatorContext(className);
    }





    public static void GeneratWithFolder(FolderEventArgs e)
    {

        CinfgGenerate(e.Node.Name, "Conditions", e.Node.Name, e.FullPath);
        CinfgValidatorContext(e.Node.Name, "Conditions", e.Node.Name, e.FullPath);

        InjoctorGenerate(e.Node.Name, "Conditions", e.Node.Name, e.FullPath);

        CheckerGenerate(e.Node.Name, "Conditions", e.Node.Name, e.FullPath);



        GenerateAll(e.Node.Name, "Validators", e.Node.Name, e.FullPath);

        //GenerateAll(e.Node.Name, node.Name, node.Name, e.FullPath);




    }



}


