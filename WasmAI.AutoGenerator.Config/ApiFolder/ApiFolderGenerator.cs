
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.PortableExecutable;
namespace AutoGenerator.ApiFolder;


public  class ApiFolderInfo
{


    public static FolderNode? ROOT { get; set; }
    public static string? AbsolutePath;

    public static Type? TypeContext { get; set; }

    public static Type? TypeIdentityUser { get; set; }


    public static  Assembly? AssemblyShare { get;  set; }

    public static Assembly? AssemblyModels { get; set; }


    public static bool IsBPR { get; set; }=false;




}

public class ApiFolderBuildOptions
{
    /// <summary>
    /// The root path of the project. If null or empty, the current directory will be used.
    /// </summary>
    public string? ProjectPath { get; set; }

    /// <summary>
    /// The name of the root folder to be generated. Defaults to "Api".
    /// </summary>
    public string NameRoot { get; set; } = "Api";

   

    /// <summary>
    /// Optional callback invoked when folders are created.
    /// </summary>
    public Action<object?, FolderEventArgs?>? OnCreateFolders { get; set; }

    /// <summary>
    /// Optional callback invoked when files are created.
    /// </summary>
    public Action<object?, FileEventArgs?>? OnCreateFiles { get; set; }
}

public class ApiFolderGenerator
{










    public static void Build(ApiFolderBuildOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        string projectPath = string.IsNullOrEmpty(options.ProjectPath)
            ? Directory.GetCurrentDirectory().Split("bin")[0]
            : options.ProjectPath;

        string jsonFilePath = Path.Combine(projectPath, projectPath); // هذا يبدو خطأ من الأصل!

        // تحقق من أن الملف موجود
        if (!Directory.Exists(projectPath))
        {
            Console.WriteLine($"❌ The provided project path does not exist: {projectPath}");
            return;
        }

        var folderReader = new FolderStructureReader();

        if (options.OnCreateFolders != null)
            folderReader.FolderCreated += (sender, args) => options.OnCreateFolders(sender, args);

        if (options.OnCreateFiles != null)
            folderReader.FileCreating += (sender, args) => options.OnCreateFiles(sender, args);

        folderReader.LoadFromJson(jsonFilePath);

        var root = folderReader.BuildFolderTree(options.NameRoot ?? "Api");

        ApiFolderInfo.ROOT = root;
        ApiFolderInfo.AbsolutePath = projectPath;

        folderReader.PrintFolderTree(root);
        folderReader.CreateFolders(projectPath, root);
        folderReader.OnAfterCreatedFolders(projectPath, root);

        Console.WriteLine("✅ All folders have been created successfully!");
    }



}


