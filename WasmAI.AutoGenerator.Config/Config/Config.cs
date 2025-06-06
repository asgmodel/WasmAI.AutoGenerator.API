


using AutoGenerator.ApiFolder;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System.Reflection;
using System.Security.Cryptography;
using static System.Formats.Asn1.AsnWriter;

namespace AutoGenerator.Config
{

     
    public static class AutoConfigall
    {
       
        public static void AddAutoScope(this IServiceCollection serviceCollection, Assembly? assembly)
        {

            var scopes = assembly.GetTypes().Where(t => typeof(ITScope).IsAssignableFrom(t) ).AsParallel().ToList();
            var Iscopeshare = scopes.Where(t => typeof(ITBaseShareRepository).IsAssignableFrom(t) && t.IsInterface).AsParallel().ToList();
            var cscopeshare = scopes.Where(t => typeof(ITBaseShareRepository).IsAssignableFrom(t) && t.IsClass).AsParallel().ToList();
              foreach (var Iscope in Iscopeshare)
            {

                var cscope= cscopeshare.Where(t => Iscope.IsAssignableFrom(t)).FirstOrDefault();
                if(cscope != null)
                {
                    serviceCollection.AddScoped(Iscope, cscope);
                }
                else
                {

                }
               
            }

            var Iscopeservis = scopes.Where(t => typeof(ITBaseService).IsAssignableFrom(t) && t.IsInterface).AsParallel().ToList();
            var cscopeservis = scopes.Where(t => typeof(ITBaseService).IsAssignableFrom(t) && t.IsClass).AsParallel().ToList();
            foreach (var Iscope in Iscopeservis)
            {
                if(!Iscope.Name.Contains("IUse"))
                {
                    continue;
                }
                var cscope = cscopeservis.Where(t => Iscope.IsAssignableFrom(t)).FirstOrDefault();
                if (cscope != null)
                {
                    serviceCollection.AddScoped(Iscope, cscope);
                }
            }


            var cscopebuilders= scopes.Where(t => typeof(ITBuilder).IsAssignableFrom(t) && t.IsClass).AsParallel().ToList();

            foreach(var b in cscopebuilders)
            {
                serviceCollection.AddScoped(b);





            }



            //serviceCollection.AddHttpContextAccessor();

            //var  usercliems= assembly.GetTypes().Where(t => typeof(ITClaimsHelper).IsAssignableFrom(t)).AsParallel().ToList();












        }

        public static void AddAutoSingleton(this IServiceCollection serviceCollection, Assembly? assembly)
        {
          
            var singletons = assembly.GetTypes().Where(t => typeof(ITSingleton).IsAssignableFrom(t) && t.IsClass).ToList();
            foreach (var singleton in singletons)
            {
                serviceCollection.AddSingleton(singleton);
            }



           
        }

        public static void AddAutoTransient(this IServiceCollection serviceCollection, Assembly? assembly)
        {
         
            var transients = assembly.GetTypes().Where(t => typeof(ITTransient).IsAssignableFrom(t) && t.IsClass).ToList();
            foreach (var transient in transients)
            {
                serviceCollection.AddTransient(transient);
            }
        }

    }

    public class MappingConfig : Profile
    {
        public static bool CheckIgnoreAutomateMapper(Type type)
        {
            var attribute = type.GetCustomAttribute<IgnoreAutomateMapperAttribute>();
            return attribute != null && attribute.IgnoreMapping;
        }
        public static  int CountMap { get; set; } = 0;
        public MappingConfig()
        {
            var assemblyModels = ApiFolderInfo.AssemblyModels;
            var assembly = ApiFolderInfo.AssemblyShare;

            var models = assemblyModels.GetTypes().Where(t => typeof(ITModel).IsAssignableFrom(t) && t.IsClass).ToList();
            var dtos = assembly.GetTypes().Where(t => typeof(ITBuildDto).IsAssignableFrom(t) && t.IsClass&&t.Name.Contains("BuildDto")).ToList();
            var dtosShare = assembly.GetTypes().Where(t => typeof(ITShareDto).IsAssignableFrom(t) && t.IsClass&& t.Name.Contains("ShareDto")).ToList();
            var vms = assembly.GetTypes().Where(t => typeof(ITVM).IsAssignableFrom(t) && t.IsClass).ToList();
            var dsos = assembly.GetTypes().Where(t => typeof(ITDso).IsAssignableFrom(t) && t.IsClass&& t.Name.Contains("Dso")).ToList();

            // 1. Map Models <-> DTOs
            int c = 0;
            foreach (var model in models.Where(m => !CheckIgnoreAutomateMapper(m)))
            {

                var sh = dtosShare.Where(s => s.Name.Contains(model.Name));
                var ds = dsos.Where(d => d.Name.Contains(model.Name));

                foreach (var dto in dtos.Where(d => d.Name.Contains(model.Name)))
                {
                    AddTwoWayMap(model, dto);

                   
                    if (!CheckIgnoreAutomateMapper(dto))
                    {
                        foreach (var share in sh)
                        {
                            CreateMap(dto, share);
                            CountMap++;

                            foreach (var dso in ds)
                            {
                                CreateMap(share, dso);
                                CountMap++;
                                c++;
                            }
                        }
                    }
                }
            }

            // 2. Map DSO <-> VM
            foreach (var dso in dsos.Where(d => !CheckIgnoreAutomateMapper(d)))
            {

                var name = models.Where(m => dso.Name.Contains(m.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.Name;
                foreach (var vm in vms.Where(v => !CheckIgnoreAutomateMapper(v) && v.Name.Contains(name, StringComparison.OrdinalIgnoreCase)))
                {


                    AddTwoWayMap(dso, vm);


                }
            }


            MapRepToReq("BuildDto", dtos);
            MapRepToReq("ShareDto", dtosShare);
            MapRepToReq("Dso", dsos);




            // 3. Map DTO <-> VM
            foreach (var dto in dtos.Where(d => !CheckIgnoreAutomateMapper(d)))
            {

                var name = models.Where(m => dto.Name.Contains(m.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.Name;

                foreach (var vm in vms.Where(v => !CheckIgnoreAutomateMapper(v) && v.Name.Contains(name, StringComparison.OrdinalIgnoreCase)))
                {
                    AddTwoWayMap(dto, vm);
                }
            }

            // 4. Map Share <-> VM
            foreach (var share in dtosShare.Where(s => !CheckIgnoreAutomateMapper(s)))
            {
                var name = models.Where(m => share.Name.Contains(m.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.Name;
                foreach (var vm in vms.Where(v => !CheckIgnoreAutomateMapper(v) && v.Name.Contains(name, StringComparison.OrdinalIgnoreCase)))
                {
                    AddTwoWayMap(share, vm);
                }
            }
        }
        private void MapRepToReq(string tag, List<Type> types, string tagreq = "Request", string tagrep = "Response")
        {


            foreach (var typereq in types.Where(d => d.Name.Contains($"{tagreq}{tag}")))
            {

                var typerep = types.Where(d => d.Name.Contains($"{tagrep}{tag}") && d.Name.Contains(typereq.Name.Replace($"{tagreq}{tag}", ""))).FirstOrDefault();
                if (typerep != null)
                {
                    CreateMap(typerep, typereq);
                    CountMap++;

                }



            }
        }
        private void AddTwoWayMap(Type source, Type destination)
        {
            CreateMap(source, destination).AfterMap((src, dest, context) =>
            {
                HelperTranslation.MapToProcessAfter(src, dest, context);
            });

            CountMap+=2;
            CreateMap(destination, source).AfterMap((src, dest, context) =>
            {
                HelperTranslation.MapToProcessAfter(src, dest, context);
            });
        }
    }

}