


using AutoGenerator.ApiFolder;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AutoGenerator.Code.Config
{
    


   

    public  enum BPRLayerGenType
    {
         
        BuildDto,
        ShareDto ,
        Dso ,
        All
       
    }
    public  class BPRLAYERGENERATOR
    {
        public  static  BPRLayerGenType  GenType { get; set; } = BPRLayerGenType.Dso;

        public  static  bool IsGenerateBuilderDto { get; set; } = false;

        public static bool IsGenerateShareDto { get; set; } = false;

        public static bool IsGenerateDso { get; set; } = true;

        public static void SetGenType(BPRLayerGenType genType)
        {
            GenType = genType;

            FolderStructureReader.TMFolderStructure= GetFolderStractRoot(genType);

        }
       private  static string GetFolderStractRoot(BPRLayerGenType genType)
        {
            if(genType == BPRLayerGenType.BuildDto)
           
            return @"
{
  ""Controllers"": [ ""Api"", ""Auth"", ""Admin"" ],
  ""Repositories"": [ ""Base"", ""Builder""],
 
  ""DyModels"": [
    {
      ""VM"": [],
      ""Dto"": {
        ""Build"": [ ""Request"", ""Response""  ]
        
      }
     
    }
  ],

  ""Validators"": [ ""Conditions"" ],
  ""Schedulers"": []
}";
            else if (genType == BPRLayerGenType.ShareDto)

                return @"
{
  ""Controllers"": [ ""Api"", ""Auth"", ""Admin"" ],
  ""Repositories"": [ ""Base"", ""Builder"", ""Share"" ],
  
  ""DyModels"": [
    {
      ""VM"": [],
      ""Dto"": {
        ""Build"": [ ""Request"", ""Response""  ],
        ""Share"": [ ""Request"", ""Response"" ]
      }
     
    }
  ],

  ""Validators"": [ ""Conditions"" ],
  ""Schedulers"": []
}";
           


            return @"
{
  ""Controllers"": [ ""Api"", ""Auth"", ""Admin"" ],
  ""Repositories"": [ ""Base"", ""Builder"", ""Share"" ],
  ""Services"": [ ""Email"", ""Logging"" ],
  ""DyModels"": [
    {
      ""VM"": [],
      ""Dto"": {
        ""Build"": [ ""Request"", ""Response""  ],
        ""Share"": [ ""Request"", ""Response"" ]
      },
      ""Dso"": [ ""Request"", ""Response"" ]
    }
  ],

  ""Validators"": [ ""Conditions"" ],
  ""Schedulers"": []
}";
        }

    }

    public class ConfigCode
    {
       
        private static Dictionary<string, Action<FolderEventArgs>>? _folderGenerators;

        private static void LoadFolderGenerators()
        {
            _folderGenerators = new Dictionary<string, Action<FolderEventArgs>>();

            var types = Assembly.GetExecutingAssembly().GetTypes(); // أو استخدم AssemblyShare
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<FolderGeneratorAttribute>();
                if (attr != null)
                {
                    var method = type.GetMethod("GeneratWithFolder", BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        var action = (Action<FolderEventArgs>)Delegate.CreateDelegate(typeof(Action<FolderEventArgs>), method);
                        _folderGenerators[attr.FolderName] = action;
                    }
                }
            }
        }


        public static void OnCreateFolders(object? sender, FolderEventArgs e)
        {
            if (e.Node.Name == "Dto")
            {


                DtoGenerator.GeneratWithFolder(e);


            }
            else if (e.Node.Name == "Dso")
            {
               
                DsoGenerator.GeneratWithFolder(e);
            }

            else if (e.Node.Name == "Repositories")
            {
                RepositoryGenerator.GeneratWithFolder(e);
            }

            else if (e.Node.Name == "Services")
            {
                ServiceGenerator.GeneratWithFolder(e);
            }
            else if (e.Node.Name == "Controllers")
            {
                ControllerGenerator.GeneratWithFolder(e);
            }
            else if (e.Node.Name == "VM")
            {
                VMGenerator.GeneratWithFolder(e);
            }
            else if (e.Node.Name == "Validators")
            {

                ValidatorGenerator.GeneratWithFolder(e);
            }

            else if (e.Node.Name == "Schedulers")
            {
                SchedulerGenerator.GeneratWithFolder(e);

            }
        }



    }

  

}