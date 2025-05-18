using AutoGenerator;
using AutoGenerator.ApiFolder;
using AutoGenerator.Custom.ApiClient;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System.Collections.Generic;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Wasm.AutoGenerator.ConfigApi
{
    public class ApiWasmCoderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl ;

        public ApiWasmCoderService(string? baseurl)
        {
             baseurl = baseurl ?? throw new ArgumentNullException(nameof(baseurl));


            _httpClient = new HttpClient();
            _baseUrl = baseurl;

        }

        public  HttpClient CreateHttpClient( )
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };

            // يمكنك إضافة إعدادات إضافية هنا إن أردت مثل Headers أو Timeout
            // client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        public async Task<List<CodeGenerator>?> GetCodesProjectAsync(string token ="")
        {
            try
            {
               
                CodeGeneratorClient codeGeneratorClient = new CodeGeneratorClient(CreateHttpClient());
                var  res=await codeGeneratorClient.GetCodesProjectAsync(token);


                if (res == null)
                {
                    Console.WriteLine("No data found.");
                    return null;
                }
                return res.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }


         private static   async Task<bool> GetProject(string token,string urlpathapi)
        {

            var client = new ApiWasmCoderService(urlpathapi);

            var result =await client.GetCodesProjectAsync(token);

            if (result == null)
            {
                Console.WriteLine("No data found.");
                return false;
            }
            foreach (var code in result)
            {
               

                //

               var pathfile=code.PathFile;

                // save folders  and save file 

                CreateFileWithFolders(pathfile, code.Code);









            }



            return true;


        }
        public static  Task<bool> GetProject(AutoGeneratorCustomApiOptions options)
        {

            
            return GetProject(options.Token,options.UrlApi);


        }

        private  async Task<OutLoginRequest?> Login(LoginRequest? request)
        {

            var auth = new AuthCodeClient(CreateHttpClient());

            try
            {
              var res=await  auth.LoginAsync(request);

                return res;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }

        }

        public static async Task<bool> PushProject(AutoGeneratorCustomApiOptions optionapi, AutoBuilderApiCoreOption coreOption)
        {
            var client = new ApiWasmCoderService(optionapi.UrlApi);
            var res = await client.pushProject(optionapi, coreOption);

            return res;

        }

        private async Task<bool> pushProject(AutoGeneratorCustomApiOptions optionapi, AutoBuilderApiCoreOption coreOption)
        {
            OutLoginRequest outLogin;
            try
            {
             
                outLogin = await Login(optionapi.LoginRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            if(coreOption.NameRootApi== "")
            {

                coreOption.NameRootApi = optionapi.ProjectId ?? "Api";

            }
           

            try
            {
                var project = new ProjectCreateVM
                {
                    Name = coreOption.NameRootApi,

                    Type = "Project",
                    SectionId = outLogin.SectionId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CodeGenerators = new List<CodeGeneratorCreateVM>(),
                    Description = "Description of Project 1",


                };


                var codes = new List<CodeGeneratorCreateVM>();

                var models = CombineCSFiles(project, optionapi.PathModels);
                if (models != null)
                {
                    codes.AddRange(models);
                }

                var datacontext = CombineCSFiles(project, optionapi.PathDataContext, "Data");

                if (datacontext != null)
                {
                    codes.AddRange(datacontext);
                }





                var Generators = BaseGenerator.TGenerators;

                string basepath = ApiFolderInfo.ROOT.Name;

                foreach (var generator in Generators)
                {

                    var path = generator.GetFilePath();
                    var items = path.Split(basepath);

                    // spit the path to get the folder name
                    var subpath = basepath + items[1];

                    // name file in subpath
                    var fileName = Path.GetFileName(subpath);



                    var options = generator.Options;





                    var codeGenerator = new CodeGeneratorCreateVM
                    {

                        Name = fileName,
                        Type = generator.GetType().Name,
                        Code = generator.GetCode() ?? string.Empty,
                        PathFile = subpath,
                        NamespaceName = options.NamespaceName,
                        AdditionalCode = options.AdditionalCode ?? string.Empty,
                        Usings = options.Usings.ToString(),
                        SubType = options.SubType,

                        BaseClass = options.SourceType.Name,
                        CreatedAt = DateTimeOffset.UtcNow,




                    };


                    codes.Add(codeGenerator);
                    // Add the code generator to the context


                }


                project.CodeGenerators = codes;
                var projectClient = new ProjectClient(CreateHttpClient());

                var newp = await projectClient.CreateProjectAsync(project);


                SaveProjectToJsonFile(newp, $"autogeneratorprojectsoptionsapi.json", outLogin.Token);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

            return true;


        }
        public static void SaveProjectToJsonFile(ProjectOutputVM project, string filePath,string token)
        {
            try
            {
                var newProject = new Dictionary<string, string>
        {
            { "Id", project.Id },
            { "Name", project.Name },
            { "Type", project.Type },
            { "CreatedAt", project.CreatedAt.ToString("o") }, // بصيغة ISO
            { "SectionId", project.SectionId },
           { "Token", token },
        };

                List<Dictionary<string, string>> allProjects = new();

                if (File.Exists(filePath))
                {
                    string existingJson = File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        allProjects = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(existingJson)
                                      ?? new List<Dictionary<string, string>>();
                    }
                }

                allProjects.Add(newProject);

                string json = JsonSerializer.Serialize(allProjects, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving project to JSON: {ex.Message}");
            }
        }
        public static  List<CodeGeneratorCreateVM>? CombineCSFiles(ProjectCreateVM project, string folderPath, string tag = "Models")
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("The specified folder does not exist.");
                return null;
            }
            List <CodeGeneratorCreateVM> codeGenerators = new List<CodeGeneratorCreateVM>();
            string[] csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            foreach (string file in csFiles)
            {

                string content = System.IO.File.ReadAllText(file);


                var fileName = Path.GetFileName(file);
                var codeGenerator = new CodeGeneratorCreateVM
                {

                    // Id = Guid.NewGuid().ToString(),
                    Name = fileName,
                    Type = "model",
                    Code = content,
                    PathFile = $"{project.Name}/{tag}/{fileName}",
                    NamespaceName = "---",
                    AdditionalCode = "----",
                    Usings = "----",

                    CreatedAt = DateTimeOffset.UtcNow,



                };
                codeGenerators.Add(codeGenerator);


                // Write the content of the file to the output file


            }
            return codeGenerators;
            


        }

        static   bool mergfiles(string filepath,string content,bool useai=false)
        {

            if (string.IsNullOrWhiteSpace(filepath))
            {
                Console.WriteLine("File path is null or empty.");
                return false;
            }
            // no found file  
            if (!File.Exists(filepath))
            {
                
                return false;
            }

            try
            {
                var merger = new ComprehensiveCodeMerger3();
               var generatedCode = merger.MergeCode(File.ReadAllText(filepath), content, "old_version.cs", "new_version.cs", useai);
                File.WriteAllText(filepath, generatedCode);
                Console.WriteLine($"Merged code saved to {filepath}");
              
                return true;
            }


            catch (Exception ex)
            {
                Console.WriteLine($"Error merging code: {ex.Message}");
                // Handle the exception as needed
            }
            return false;
        }

        public static void CreateFileWithFolders(string inputPath, string content)
        {


            if (string.IsNullOrWhiteSpace(inputPath))
            {
                Console.WriteLine("File path is null or empty.");
                return;
            }
             var  ismrg= mergfiles(inputPath, content,BaseGenerator.UseAI);

            if (ismrg)
            {
                Console.WriteLine($"Merged code saved to {inputPath}");
                return;
            }
            // توحيد الفواصل إلى '/' أولاً ثم تحويلها إلى الفاصل الصحيح للنظام
            string cleanedPath = inputPath.Replace("\\", "/").Replace("/", Path.DirectorySeparatorChar.ToString());


            // استخراج مسار المجلد
            string directoryPath = Path.GetDirectoryName(cleanedPath);

            if (!string.IsNullOrEmpty(directoryPath))
            {
                // إنشاء المجلدات إذا لم تكن موجودة
                Directory.CreateDirectory(directoryPath);
            }

            // إنشاء أو استبدال الملف بالمحتوى المطلوب
            File.WriteAllText(cleanedPath, content);

            Console.WriteLine($"✅ Created file at: {cleanedPath}");
        }
    }
}

