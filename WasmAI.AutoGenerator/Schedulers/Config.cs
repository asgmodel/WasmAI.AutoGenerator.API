
using AutoGenerator.Data;
using AutoGenerator.Schedulers;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using System.Collections.Generic;
using System.Reflection;
using WasmAI.ConditionChecker.Checker;

namespace AutoGenerator.Schedulers
{
    public class OptionScheduler
    {
        
        public Assembly? Assembly { get; set; }


        public  string ? DbConnectionString { get; set; }



    }





    public  static class ConfigScheduler
    {


        public static Dictionary<Type, JobOptions> getJobOptions(IBaseConditionChecker checker, Assembly assembly)
        {

            var typesjobs = assembly.GetTypes()
          .Where(t => t.IsClass && !t.IsAbstract && typeof(ITJob).IsAssignableFrom(t))
          .AsParallel()
          .ToList();

            Dictionary<Type, JobOptions> jobs = new();

            foreach (var type in typesjobs)
            {

                var instance = Activator.CreateInstance(type, checker) as BaseJob;


                jobs[type] = instance.Options;

            }
            return jobs;
        }

        public static void AddAutoScheduler(this IServiceCollection serviceCollection, OptionScheduler? option = null)
        {

            serviceCollection.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
               
            });

            serviceCollection.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


      

            var typesjobs =option.Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ITJob).IsAssignableFrom(t))
                .AsParallel()
                .ToList();

            foreach (var type in typesjobs)
            {

               


                serviceCollection.AddScoped(type);


            }


            serviceCollection.AddHangfire(config =>
                config.UseSqlServerStorage(option.DbConnectionString)); // √Ê √Ì „“Êœ  Œ“Ì‰ ¬Œ—


        }


        public async static void UseSchedulerDashboard(this WebApplication app, OptionScheduler? option = null)
        {

            app.UseHangfireDashboard();

        }


            //public async static void UseSchedulersCore(this WebApplication app, OptionScheduler? option=null)
            //{
            //    using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //    {





            //        var schedulerJobProvider = new SchedulerJobProvider(scope.ServiceProvider.GetRequiredService<ISchedulerFactory>(), jobOptions);

            //        await schedulerJobProvider.StartAsync();
            //    }

            //}
            //public static Dictionary<string,JobOptions> getJobOptions(DataContext context, Assembly assembly)
            //{



            //    var typesjobs = assembly.GetTypes()
            //        .Where(t => t.IsClass && !t.IsAbstract && typeof(ITJob).IsAssignableFrom(t))
            //        .AsParallel()
            //        .ToList();

            //    var jobOptions = new Dictionary<string, JobOptions>();
            //    foreach (var type in typesjobs)
            //    {
            //        var instance = Activator.CreateInstance(type, context) as ITJob;
            //        if (instance != null)
            //        {
            //            var jobOption = instance.Options;
            //            jobOption.JobType = type;
            //            jobOptions.Add(jobOption.JobName, jobOption);
            //        }


            //    }



            //    return jobOptions;
















            //}
        }


}
       
