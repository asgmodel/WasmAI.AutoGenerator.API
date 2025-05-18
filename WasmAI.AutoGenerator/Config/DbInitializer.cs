using AutoGenerator;
using AutoGenerator.ApiFolder;
using AutoGenerator.Custom.Data;
using AutoGenerator.Custom.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Wasm.AutoGenerator.Config
{
    public class UserData
    {
        public string? UserName { get; set; } = "anas";

        public string? Password { get; set; }
        public string? Email { get; set; }


    }
    public static class DbInitializer
    {
        
        public static async Task SeedData(IServiceProvider serviceProvider, AutoGeneratorCustomApiOptions apioptions, AutoBuilderApiCoreOption coreOption, bool islocal=true)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<CodeDataContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationCodeUser>>();
            // user find
            if (islocal)
            {
                context.Database.Migrate();
                var user = await EnsureDefaultUserAsync(userManager,new UserData()
                {
                    Email= apioptions.LoginRequest.Username,
                    Password = apioptions.LoginRequest.Password,
                    UserName = apioptions.LoginRequest.Username
                });
                var sec= await EnsureDefaultSectionAsync(context, user);
                await RigsterProject(sec, context,apioptions,coreOption);
            }
            
        }

        public static async Task CombineCSFiles(Project project, CodeDataContext context, string folderPath, string outputFilePath,string tag= "Models")
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("The specified folder does not exist.");
                return;
            }

            string[] csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            using (StreamWriter writer = new StreamWriter(outputFilePath, false))
            {
                foreach (string file in csFiles)
                {
                  
                    string content = File.ReadAllText(file);


                    var fileName = Path.GetFileName(file);
                    var codeGenerator = new CodeGenerator
                    {

                        // Id = Guid.NewGuid().ToString(),
                        Name = fileName,
                        Type = "model",
                        Code = content,
                        PathFile = $"{project.Name}/{tag}/{fileName}",
                        NamespaceName = "---",
                        AdditionalCode ="----",
                        Usings = "----",

                        CreatedAt = DateTimeOffset.UtcNow,
                        ProjectId = project.Id,



                    };

                   await context.CodeGenerators.AddAsync(codeGenerator);
                   await context.SaveChangesAsync();
                    // Write the content of the file to the output file
                    await writer.WriteLineAsync(content);

                }
            }

           
        }
        public static async Task<bool> RigsterProject(Section section, CodeDataContext  context, AutoGeneratorCustomApiOptions apioptions, AutoBuilderApiCoreOption coreOption)
        {



            // create project

            var project = new Project
            {
                Id = Guid.NewGuid().ToString(),
                Name = coreOption.NameRootApi,
                Description = "Description of Project 1",
                Type = "Project",

                SectionId = section.Id,
                CreatedAt = DateTime.Now
            };


            project=context.Projects.Add(project).Entity;
            await context.SaveChangesAsync();
            await CombineCSFiles(project, context, apioptions.PathModels, "temPDAT.txt");
            await CombineCSFiles(project, context, apioptions.PathDataContext, "temPDAT.txt","Data");

            SaveProjectToJsonFile(project, $"autogeneratorprojectsoptions.json");


            var Generators = BaseGenerator.TGenerators;

            string basepath =ApiFolderInfo.ROOT.Name;
            var  codes= new List<CodeGenerator>();
            foreach (var generator in Generators)
            {
               
                var path = generator.GetFilePath();
                var items = path.Split(basepath);
               
                // spit the path to get the folder name
                var subpath = basepath + items[1];

                // name file in subpath
                var fileName = Path.GetFileName(subpath);



                var options = generator.Options;





                var codeGenerator = new CodeGenerator
                {

                   // Id = Guid.NewGuid().ToString(),
                    Name = fileName,
                    Type = generator.GetType().Name,
                    Code = generator.GetCode() ?? string.Empty,
                    PathFile = subpath,
                    NamespaceName = options.NamespaceName,
                    AdditionalCode = options.AdditionalCode ?? string.Empty,
                    Usings =options.Usings.ToString(),
                    SubType = options.SubType,
                    
                    BaseClass = options.SourceType.Name,
                    CreatedAt = DateTimeOffset.UtcNow,
                    ProjectId= project.Id,



                };

                
            codes.Add(codeGenerator);
                // Add the code generator to the context
               

            }

            // Add the code generator to the context

           await  context.AddRangeAsync(codes);
           await context.SaveChangesAsync();

            return true;


        }
        public static async Task<ApplicationCodeUser> EnsureDefaultUserAsync(
      UserManager<ApplicationCodeUser> userManager,
     UserData userInput)
        {
            var user = await userManager.FindByNameAsync(userInput.UserName)
                    ?? await userManager.FindByEmailAsync(userInput.Email);

            if (user != null)
            {
                // تحقق من كلمة المرور
                var isPasswordValid = await userManager.CheckPasswordAsync(user, userInput.Password);
                if (!isPasswordValid)
                {
                    throw new Exception("User already exists but the password is incorrect.");
                }

                return user; // المستخدم موجود وكلمة المرور صحيحة
            }
            else
            {

                // إذا لم يكن المستخدم موجودًا، قم بإنشاء مستخدم جديد
                user = new ApplicationCodeUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userInput.UserName,
                    Email = userInput.Email,
                    EmailConfirmed = true // تأكيد البريد الإلكتروني
                };
                // إنشاء المستخدم
                var result = await userManager.CreateAsync(user, userInput.Password);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return user;
            }
        }


        public static void SaveProjectToJsonFile(Project project, string filePath)
        {
            try
            {
                var newProject = new Dictionary<string, string>
        {
            { "Id", project.Id },
            { "Name", project.Name },
            { "Type", project.Type },
            { "CreatedAt", project.CreatedAt.ToString("o") }, // بصيغة ISO
            { "SectionId", project.SectionId }
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

        public static async Task<Section> EnsureDefaultSectionAsync(CodeDataContext context, ApplicationCodeUser user)
        {
            if (true)
            {
                var section = new Section
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Main Section",
                    Description = "Primary Section",
                    ApplicationCodeUserId = user.Id,
                    CreatedAt = DateTime.Now
                };

              var sec=  context.Sections.Add(section);
                await context.SaveChangesAsync();

                return sec.Entity;
            }

            return context.Sections.First(); // أو أعد null حسب الحاجة
        }
        public static async Task<Section> EnsureDefaultSectionAsync(CodeDataContext context, string userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return await EnsureDefaultSectionAsync(context, user);
        }



    }
}

