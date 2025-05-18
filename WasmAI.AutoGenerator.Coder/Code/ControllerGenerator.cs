using AutoGenerator.ApiFolder;
using AutoGenerator.Helper.Translation;
using AutoGenerator.TM;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text;


namespace AutoGenerator.Code;

public class ControllerGenerator : BaseGenerator, ITGenerator
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


    public static void GenerateAll( string type, string subtype, string NamespaceName, string pathfile)
    {


        var assembly = ApiFolderInfo.AssemblyModels;


        var models = assembly.GetTypes().Where(t => typeof(ITModel).IsAssignableFrom(t) && t.IsClass).ToList();





        var root = ApiFolderInfo.ROOT.Name;

        NamespaceName = $"{root}.Controllers.{subtype}";

        foreach (var model in models)
        {
            var options = new GenerationOptions($"{model.Name}{type}", model)
            {
                NamespaceName = NamespaceName,
                Template = TmController.GetTemplateController(null, subtype, model.Name)
                            ,
                Usings = new List<string>
                        {

                            "AutoMapper",

                            "Microsoft.Extensions.Logging",
                            "System.Collections.Generic",

                            $"{root}.Services.Services",
                            "Microsoft.AspNetCore.Mvc",
                            $"{root}.DyModels.VMs",
                            "System.Linq.Expressions",
                             $"{root}.DyModels.Dso.Requests",
                            $"{root}.DyModels.Dso.Responses",
                            "AutoGenerator.Helper.Translation"









                        }


            };








            ITGenerator generator = new ControllerGenerator();
            generator.Generate(options);

            string jsonFile = Path.Combine(pathfile, $"{subtype}/{model.Name}Controller.cs");
            generator.SaveToFile(jsonFile);

            Console.WriteLine($"✅ {options.ClassName} has been created successfully!");
        }


    }

    private static string[] UseControllers = new string[] { "Api" };
    public static void GeneratWithFolder(FolderEventArgs e)
    {

        foreach (var node in e.Node.Children)
                 if(UseControllers.Contains(node.Name))
                 GenerateAll(e.Node.Name, node.Name, e.Node.Name, e.FullPath);
          
            //GenerateAll(e.Node.Name, node.Name, node.Name, e.FullPath);



      
    }



}

