using AutoGenerator.ApiFolder;
using AutoGenerator.Helper.Translation;
using AutoGenerator.TM;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Reflection;
using System.Text;

namespace AutoGenerator.Code;

public class RepositoryGenerator : BaseGenerator, ITGenerator
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
        public Func<string, string,string> ActionM { get; set; }

        
    }

    private static readonly string TAG = "Repositories";
    public static void GenerateBaseRep(string root,string type,string subtype,string pathfile)
    {


        

     
        //base
        ITGenerator generator = new RepositoryGenerator();

        var usings = new List<string>()
        {

            "AutoGenerator.Repositories.Base",
            ApiFolderInfo.TypeContext.Namespace,
            ApiFolderInfo.TypeIdentityUser.Namespace,
            "Microsoft.AspNetCore.Identity"

        };

        var options = new GenerationOptions("BaseRepository",typeof(RepositoryGenerator),isProperties:false)
        {

            NamespaceName = $"{root}.{type}.Base",
            Template = TmBaseRepository.GetTmBaseRepository("BaseRepository"),
            Usings = usings
        };

        var generatedCode = generator.Generate(options);

        string jsonFile = Path.Combine(pathfile, $"{subtype}/BaseRepository.cs");
        generator.SaveToFile(jsonFile);

        Console.WriteLine($"✅ {options.ClassName} has been created successfully!");

        usings = new List<string>()
        {

           
            ApiFolderInfo.TypeContext.Namespace,
            ApiFolderInfo.TypeIdentityUser.Namespace,
            "AutoMapper",
            "AutoGenerator",
            "AutoGenerator.Repositories.Builder"

        };
        ITGenerator generator2 = new RepositoryGenerator();
        var options2 = new GenerationOptions("BaseBuilderRepository", typeof(RepositoryGenerator), isProperties: false)
        {

            NamespaceName = $"{root}.{type}.Base",
            Template = TmBaseRepository.GetTmBaseBuilderRepository("BaseBuilderRepository"),
            Usings = usings
        };

         generatedCode = generator2.Generate(options2);

        jsonFile = Path.Combine(pathfile, $"{subtype}/BaseBuilderRepository.cs");
        generator2.SaveToFile(jsonFile);

        Console.WriteLine($"✅ {options2.ClassName} has been created successfully!");







    }
    public static void GenerateAll(string root,string type, string subtype, string NamespaceName, string pathfile)
    {


        var assembly = ApiFolderInfo.AssemblyModels;
        var nameContext = ApiFolderInfo.TypeContext.Name;

        var models = assembly.GetTypes().Where(t => typeof(ITModel).IsAssignableFrom(t) && t.IsClass).ToList();




        var funcs = subtype != "Share" ?
            new List<ActionServ>() {
             new(){ActionM=createTIRBuild,Name="I"},
             new(){ActionM=createTIRbodyBuild,Name=""}
            } :
         new List<ActionServ>() {
             new(){ActionM=createTIRShare,Name="I"},
             new(){ActionM=createTIBodyShare,Name=""}
            };

       



        NamespaceName = $"{root}.{TAG}.{subtype}";

        foreach (var model in models)
        {


            CreateFolder(pathfile, $"{subtype}/{model.Name}");
            foreach (var func in funcs)
            {
                var options = new GenerationOptions($"{model.Name}{type}", model)
                {
                    NamespaceName = NamespaceName,
                    Template =func.ActionM(model.Name,type) ,
                    Usings = new List<string>
                        {
                            
                            "AutoMapper",
                            ApiFolderInfo.TypeContext.Namespace,
                            ApiFolderInfo.TypeIdentityUser.Namespace,
                       
                           $"{root}.{type}.Base",
                           $"AutoGenerator.{TAG}.Builder",
                           $"{root}.DyModels.Dto.Build.Requests",
                           $"{root}.DyModels.Dto.Build.Responses",
                           "AutoGenerator",
                           "AutoGenerator.Repositories.Base"


                        }


                };
                options.SubType = subtype;



                if (subtype == "Share")
                {

                    options.Usings.AddRange(new List<string> {



                            "AutoGenerator",
                            $"{root}.{TAG}.Builder",
                            $"AutoGenerator.{TAG}.Share",
                            "System.Linq.Expressions",
                           $"AutoGenerator.{TAG}.Base",
                            "AutoGenerator.Helper",
                           $"{root}.DyModels.Dto.Share.Requests",
                           $"{root}.DyModels.Dto.Share.Responses",
                });

                }




                    ITGenerator generator = new RepositoryGenerator();
                    generator.Generate(options);

                    string jsonFile = Path.Combine(pathfile, $"{subtype}/{model.Name}/{func.Name}{model.Name}{subtype}Repository.cs");
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
    private  static string getInterfaceIBPRLayer()
    {

        return ApiFolderInfo.IsBPR ? "IBPRLayer" : "IBasePublicRepository";

	}

    static string createTIRBuild(string className, string type)
    {

        return $@"  /// <summary>
                 /// {className} interface property for BuilderRepository.
           /// </summary>
         public interface I{className}BuilderRepository<TBuildRequestDto,TBuildResponseDto>  
             : {getInterfaceIBPRLayer()}<TBuildRequestDto,TBuildResponseDto> //
             where TBuildRequestDto : class  //
             where TBuildResponseDto : class //
         {{
             // Define methods or properties specific to the builder interface.
         }}
         ";
    }

    static string createTIRbodyBuild(string className, string type)
    {




        return $@"  /// <summary>
                 /// {className} class property for BuilderRepository.
           /// </summary>
         //
         

        public class {className}BuilderRepository   
               : BaseBuilderRepository<{className},{className}RequestBuildDto,{className}ResponseBuildDto>,  
                 I{className}BuilderRepository<{className}RequestBuildDto,{className}ResponseBuildDto>,ITBuilder
         {{
                       /// <summary>
                         /// Constructor for {className}BuilderRepository.
                   /// </summary>


             public {className}BuilderRepository({ApiFolderInfo.TypeContext.Name} dbContext,
                                               IMapper mapper, ILoggerFactory logger) 
                 : base(dbContext, mapper, logger) // Initialize  constructor.
             {{
                 // Initialize necessary fields or call base constructor.
                ///
                /// 

       
                /// 
             }}


         //

          // Add additional methods or properties as needed.
         }}
         ";
    }

	private static string getInterfaceIBaseBPRShareLayer()
	{

		return ApiFolderInfo.IsBPR ? "IBaseBPRShareLayer" : "IBasePublicRepository";

	}
	static string createTIRShare(string className ,string type)
    {

        return $@"
                 /// <summary>
                /// {className} interface for {type}Repository.
                /// </summary>
               public interface I{className}ShareRepository 
                                : IBaseShareRepository<{className}RequestShareDto, {className}ResponseShareDto> //
                               
                               ,{getInterfaceIBaseBPRShareLayer()}<{className}RequestShareDto, {className}ResponseShareDto>

                               //  يمكنك  التزويد بكل  دوال   طبقة Builder   ببوابات  الطبقة   هذه نفسها      
                               //,I{className}BuilderRepository<{className}RequestShareDto, {className}ResponseShareDto>
                        {{
                            // Define methods or properties specific to the share repository interface.
                        }}";
            }



    static string createTIBodyShare(string className, string type)
    {

        return TmShareRepository.GetTmShareRepository(className);
    }

    private static string[] UseRepositories = new string[] { "Builder", "Share" };
    public static void GeneratWithFolder(FolderEventArgs e)
    {
        if (ApiFolderInfo.IsBPR)
        {
            var bbrpath = e.FullPath.Replace("Repositories", "");
            CreateFolder(bbrpath, "BPRLayers");
            CreateFolder($"{ bbrpath}/BPRLayers", "Base");


            TmBaseLayers.GenerateGroupedBaseLayerFiles($"{bbrpath}/BPRLayers/Base", ApiFolderInfo.ROOT.Name);
          //  File.WriteAllText($"{e.FullPath.Replace("Repositories", "")}/Layers/Base.cs", TmBaseLayers.GetTmBaseLayers(""));

        }

        foreach (var node in e.Node.Children)
        {
            var root = ApiFolderInfo.ROOT.Name;

            if (UseRepositories.Contains(node.Name))

                GenerateAll(root, e.Node.Name, node.Name, node.Name, e.FullPath);
            else
                GenerateBaseRep(root, e.Node.Name, node.Name, e.FullPath);
            //GenerateAll(e.Node.Name, node.Name, node.Name, e.FullPath);



        }
    }



}

