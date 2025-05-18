using AutoGenerator.ApiFolder;
using AutoGenerator.Helper.Translation;
using AutoGenerator.TM;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text;


namespace AutoGenerator.Code;

public class ServiceGenerator : BaseGenerator, ITGenerator
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

    class ActionServ
    {

        public string Name { get; set; }
        public Func<string, string> ActionM { get; set; }
    }

        public static void GenerateAll(string type, string subtype, string NamespaceName, string pathfile)
        {


        var assembly = ApiFolderInfo.AssemblyModels;


        var models = assembly.GetTypes().Where(t => typeof(ITModel).IsAssignableFrom(t) && t.IsClass).ToList();




            var root = ApiFolderInfo.ROOT.Name;

            var funcs = new List<ActionServ>() {
            new(){ActionM=createTIBS ,Name="IT"}  ,
            new(){ActionM=createTIUseS ,Name="IUse"}  ,
             new(){ActionM=createTCS ,Name=""}
             };


            NamespaceName = $"{root}.Services.{subtype}";


            foreach (var model in models)
            {
                CreateFolder(pathfile, $"{model.Name}");

                foreach (var func in funcs)
                {
                    var options = new GenerationOptions($"{model.Name}{type}", model)
                    {
                        NamespaceName = NamespaceName,
                        Template = func.ActionM(model.Name)
                                    ,
                        Usings = new List<string>
                        {
                            "AutoGenerator",
                            "AutoMapper",

                            "Microsoft.Extensions.Logging",
                            "System.Collections.Generic",

                            "AutoGenerator.Services.Base",
                            $"{root}.DyModels.Dso.Requests",
                            $"{root}.DyModels.Dso.Responses",
                            ApiFolderInfo.TypeIdentityUser.Namespace,
                            $"{root}.DyModels.Dto.Share.Requests",
                            $"{root}.DyModels.Dto.Share.Responses",


                            $"{root}.Repositories.Share",
                            "System.Linq.Expressions",
                            $"{root}.Repositories.Builder",
                            "AutoGenerator.Repositories.Base",
                            "AutoGenerator.Helper"



                        }


                    };








                    ITGenerator generator = new ServiceGenerator();
                    generator.Generate(options);

                    string jsonFile = Path.Combine(pathfile, $"{model.Name}/{func.Name}{model.Name}Service.cs");
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


        private static string createTIBS(string className)
        {


            return $@"
public interface I{className}Service<TServiceRequestDso, TServiceResponseDso> 
    where TServiceRequestDso : class 
    where TServiceResponseDso : class 
{{ 
}} 

";
        }


    private static string getInterfaceIBaseBPRServiceLayer()
    {

        return ApiFolderInfo.IsBPR ? "IBaseBPRServiceLayer" : "IBasePublicRepository";

    }
    private static string createTIUseS(string className)
        {


            return $@"
                public interface IUse{className}Service : I{className}Service<{className}RequestDso, {className}ResponseDso>, IBaseService 
                               ,{getInterfaceIBaseBPRServiceLayer()}<{className}RequestDso, {className}ResponseDso>
               
                {{ 
                }} 


";
        }

        private static string createTCS(string className)
        {
            return TmService.GetTmService(className);

      
        }




        private static string[] UseRepositorys = new string[] { "Builder", "Share" };
        public static void GeneratWithFolder(FolderEventArgs e)
        {

            GenerateAll(e.Node.Name, e.Node.Name, e.Node.Name, e.FullPath);

            //GenerateAll(e.Node.Name, node.Name, node.Name, e.FullPath);




        }



    }


