


using AutoGenerator.ApiFolder;
using AutoGenerator.Code.Config;
using AutoGenerator.Code.Services;
using AutoGenerator.Custom;
using AutoGenerator.Custom.ApiClient;
using AutoGenerator.Custom.Models;
using AutoGenerator.Data;
using GenerativeAI.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using Wasm.AutoGenerator.Config;
using Wasm.AutoGenerator.ConfigApi;

namespace AutoGenerator
{

 

    
    public class AutoGeneratorCustomApiOptions
    {


        public string  ?Token { get; set; }



        public LoginRequest? LoginRequest { get; set; }



        public string[]? Arags { get; set; }


        public string? ProjectId { get; set; } 


        public string? PathModels { get; set; } = "";

        public string? PathDataContext { get; set; } = "";

        public IDictionary<string, string>? OtherPathFiles { get; set; } 
        public string? DbConnectionStringCode { get; set; } 

        public string? UrlApi { get; set; } = "https://wasmcoder.runasp.net/";


    }
        public class AutoBuilderApiCoreOption
    {

        public string? ProjectPath { get; set; }
        public string? ProjectName { get; set; } = "";

        public string NameRootApi { get; set; } = "Api";

        //public bool  IsAutoBuild { get; set; } = true;
        public bool IsMapper { get; set; } = true;

        public Type? TypeContext { get; set; }

        public Assembly? Assembly { get; set; }
        public Assembly? AssemblyModels { get; set; }

        public string? DbConnectionString { get; set; }
        public string[]? Arags { get; set; }

        public string? PathModels { get; set; }= "";




    }


    public static class ConfigAddAutoBuilderApiCore
    {
        private static AutoBuilderApiCoreOption CoreOption=new AutoBuilderApiCoreOption();
        public static IServiceCollection AddAutoBuilderApiCore<TContext, TUser>(
           this IServiceCollection serviceCollection,
           AutoBuilderApiCoreOption option)
            where TContext : class
            where TUser : class

        {


            var options = new AutoBuilderCoreOption()
            {
                ProjectName = option.ProjectName,
                IsMapper = option.IsMapper,
                TypeContext = option.TypeContext,
                Assembly = option.Assembly,
                AssemblyModels = option.AssemblyModels,
                DbConnectionString = option.DbConnectionString,
                Arags = option.Arags,
                ApiFolderBuildOptions = new ApiFolderBuildOptions()
                {
                    NameRoot = option.NameRootApi,
                    ProjectPath = option.ProjectPath,
                    OnCreateFolders = (sender, e) =>
                    {
                        if (e != null)
                        {
                            ConfigCode.OnCreateFolders(sender, e);
                        }
                    },
                }
            };


            options.Arags=cmdargs(option.Arags);
            InMemoryCodeRepository.PathModels = option.PathModels ?? "";
            CoreOption = option;

            return serviceCollection.AddAutoBuilderCore<TContext, TUser>(options);
        }
         


        private static string[] cmdargs(string[] arrgs)
        {
            var art = arrgs.ToList();

            if (arrgs != null && arrgs.Length > 0)
            {
                if (arrgs[0]== "generate" &&arrgs.Length>1)
                {


                    if (arrgs[1] == "/m")
                    {
                       BaseGenerator.UseMG = true;
                        art.Remove("/m");
                    }
                    else if (arrgs[1] == "/m/ai")
                    {

                        BaseGenerator.UseMG = true;
                        BaseGenerator.UseAI = true;
                        art.Remove("/m/ai");
                    }
                    if(arrgs.Contains("/bpr"))
                    {

                     
                        art.Remove("/bpr");

                        ApiFolderInfo.IsBPR=true;
                    }
                    
                }
                else if (arrgs[0] == "fetch")
                {

                    if (arrgs.Length > 1)
                    {
                        if (arrgs[1] == "/m")
                        {
                            BaseGenerator.UseMG = true;
                            art.Remove("/m");
                        
                        }
                        else if (arrgs[1] == "/m/ai")
                        {
                            BaseGenerator.UseMG = true;
                            BaseGenerator.UseAI = true;
                            art.Remove("/m/ai");
                        }
                    }
                }
               
            }
            return art.ToArray();
        }


        public static WebApplication UseWasmAutoGenerator(this WebApplication app, AutoGeneratorCustomApiOptions? options = null, bool isLocal = true)

        {



            return app;

        }



        public static  async Task<WebApplication> UseAutoGeneratorCustomLocalApi(this WebApplication app, AutoGeneratorCustomApiOptions? options=null)
        {


            //    app.UseMiddleware<AutoGeneratorCustomApiMiddleware>(options.Token);

            if(BaseGenerator.TGenerators.Count>0)
            {
                await DbInitializer.SeedData(app.Services, options,CoreOption);
            }


            

            return app;
        }


        public static IServiceCollection AddAutoDbContext<TContext, TUser>(this IServiceCollection services, string connectionString)
       
            where TContext : DbContext
            where TUser : class


        {
            string connectionString2 = connectionString;
            services.AddDbContext<TContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlServer(connectionString2);
            });
            services.AddIdentity<TUser, IdentityRole<string>>().AddEntityFrameworkStores<TContext>().AddDefaultTokenProviders();
            return services;
        }
        public static IServiceCollection AddAutoGeneratorCodeCustomApi(this IServiceCollection serviceCollection, AutoGeneratorCustomApiOptions? options = null)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null.");
            if(options.Arags!=null)
                 cmdargs(options.Arags);


            if(options.DbConnectionStringCode != null)
                 serviceCollection.AddDbCodeContextCustom(options.DbConnectionStringCode);
            else if(options.Token != null&&options.UrlApi!=null)
            {
                ApiWasmCoderService.GetProject(options).Wait();
            }


           

            return serviceCollection;
        }
        public static  WebApplication UseAutoGeneratorCustomApi(this WebApplication app, AutoGeneratorCustomApiOptions? options = null,bool isLocal=true)
        {

            if (options == null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null.");

            if (BaseGenerator.TGenerators.Count > 0)
            {

                if (options.DbConnectionStringCode != null)
                {
                     DbInitializer.SeedData(app.Services, options, CoreOption).Wait();
                }
                else if (  options.UrlApi != null)
                {
                     ApiWasmCoderService.PushProject(options, CoreOption).Wait();
                }

                
               
            }


            return app;
        }
    }


}