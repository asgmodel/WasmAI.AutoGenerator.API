using AutoGenerator.ApiFolder;
using AutoGenerator.Config;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AutoGenerator
{
    /// <summary>
    /// Options used to configure AutoBuilder API Core.
    /// </summary>
    public class AutoBuilderCoreOption
    {
        public string? ProjectName { get; set; } = string.Empty;

        public bool IsMapper { get; set; } = true;

        public Type? TypeContext { get; set; }

        public Assembly? Assembly { get; set; }

        public Assembly? AssemblyModels { get; set; }

        public string? DbConnectionString { get; set; }

        public string[]? Arags { get; set; }

        public ApiFolderBuildOptions? ApiFolderBuildOptions { get; set; }
    }

    /// <summary>
    /// Extension methods for configuring Auto API builder services.
    /// </summary>
    public static class ConfigServices
    {
        /// <summary>
        /// Entry method to add API builder core with DbContext and Identity user types.
        /// </summary>
        public static IServiceCollection AddAutoBuilderCore<TContext, TUser>(
            this IServiceCollection serviceCollection,
            AutoBuilderCoreOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            ApiFolderInfo.TypeContext = typeof(TContext);
            ApiFolderInfo.TypeIdentityUser = typeof(TUser);
            return serviceCollection.AddAutoBuilderCore<TUser>(option);
        }

        /// <summary>
        /// Internal method to determine whether to generate APIs or register services.
        /// </summary>
        public static IServiceCollection AddAutoBuilderCore<TModel>(
            this IServiceCollection serviceCollection,
            AutoBuilderCoreOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            var args = option.Arags ?? Array.Empty<string>();

            ApiFolderInfo.AssemblyModels = typeof(TModel).Assembly;
            ApiFolderInfo.AssemblyShare = option.Assembly;
            

            if (args.Length > 0 && args[0].ToLowerInvariant().Contains("generate"))
            {
                // Ensure build options are initialized
                if (option.ApiFolderBuildOptions == null)
                    option.ApiFolderBuildOptions = new ApiFolderBuildOptions();

                // Generate APIs for each folder root provided
                if (args.Length > 1)
                {
                    for (int i = 1; i < args.Length; i++)
                    {
                        option.ApiFolderBuildOptions.NameRoot = args[i];
                        serviceCollection.AddAutoCodeGenerator(option.ApiFolderBuildOptions);
                    }
                }
                else
                {
                    serviceCollection.AddAutoCodeGenerator(option.ApiFolderBuildOptions);
                }
            }
            else
            {
                // Otherwise, just register services
                serviceCollection.AddAutoServicesApiCore(option);
            }

            return serviceCollection;
        }

        /// <summary>
        /// Registers DI services (Scoped, Transient, Singleton) and AutoMapper if needed.
        /// </summary>
        public static void AddAutoServicesApiCore(
            this IServiceCollection serviceCollection,
            AutoBuilderCoreOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            if (option.IsMapper)
            {
                serviceCollection.AddAutoMapper(typeof(MappingConfig));
            }

            if (option.Assembly != null)
            {
                serviceCollection.AddAutoScope(option.Assembly);
                serviceCollection.AddAutoTransient(option.Assembly);
                serviceCollection.AddAutoSingleton(option.Assembly);
            }
        }

        /// <summary>
        /// Triggers code generation for WASM API folders and files.
        /// </summary>
        public static void AddAutoCodeGenerator(
            this IServiceCollection serviceCollection,
            ApiFolderBuildOptions option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            ApiFolderGenerator.Build(option);
        }
    }
}
