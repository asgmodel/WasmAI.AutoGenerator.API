


using AutoGenerator.ApiFolder;
using AutoGenerator.Custom.Data;
using AutoGenerator.Custom.Models;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AutoGenerator.Custom
{

     
    public static class AutoConfigall
    {


        public static IServiceCollection AddDbCodeContextCustom(this IServiceCollection services, string connectionString)
        {

            services.AddDbContext<CodeDataContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddIdentity<ApplicationCodeUser, IdentityRole<string>>()
                .AddEntityFrameworkStores<CodeDataContext>()  
                .AddDefaultTokenProviders();



            // Add other services as needed
            return services;



        }
     
    }

  

}