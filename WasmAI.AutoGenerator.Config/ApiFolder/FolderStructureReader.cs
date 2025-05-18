using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace AutoGenerator.ApiFolder
{



    using System;

    public class FolderEventArgs : EventArgs
    {
        public FolderNode Node { get; }
        public string FullPath { get; }

        public FolderEventArgs(FolderNode node, string fullPath)
        {
            Node = node;
            FullPath = fullPath;
        }
    }

    public class FileEventArgs : EventArgs
    {
        public FolderNode Node { get; }
        public string FullPath { get; }

        public FileEventArgs(FolderNode node, string fullPath)
        {
            Node = node;
            FullPath = fullPath;
        }
    }


    public class FolderNode
    {
        private  string? _relativePath;

        public string? RelativePath
        {
            get { return _relativePath; }
            set { _relativePath = value; }
        }

        public string Name { get; set; }

        public List<FolderNode>? Children { get; set; }

        public FolderNode(string name, string? relativePath)
        {
            Name = name;
            Children = new List<FolderNode>();
            _relativePath = relativePath;
        }

        // البحث عن مجلد بناءً على الاسم
        public FolderNode? FindNode(string name)
        {
            if (Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return this;

            foreach (var child in Children ?? Enumerable.Empty<FolderNode>())
            {
                var foundNode = child.FindNode(name);
                if (foundNode != null)
                    return foundNode;
            }

            return null;
        }

        // إضافة مجلد جديد كطفل
        public void AddChild(FolderNode child)
        {
            Children?.Add(child);
        }

        // حذف مجلد بناءً على الاسم
        public bool RemoveChild(string name)
        {
            var nodeToRemove = FindNode(name);
            if (nodeToRemove != null && nodeToRemove != this)
            {
                return Children?.Remove(nodeToRemove) ?? false;
            }
            return false;
        }

        // الحصول على جميع الأسماء في الشجرة
        public List<string> GetAllNames()
        {
            var names = new List<string> { Name };
            foreach (var child in Children ?? Enumerable.Empty<FolderNode>())
            {
                names.AddRange(child.GetAllNames());
            }
            return names;
        }

        // الحصول على جميع المسارات النسبية في الشجرة
        public List<string> GetAllRelativePaths()
        {
            var paths = new List<string> { _relativePath ?? string.Empty };
            foreach (var child in Children ?? Enumerable.Empty<FolderNode>())
            {
                paths.AddRange(child.GetAllRelativePaths());
            }
            return paths;
        }

        // التحقق إذا كان المجلد يحتوي على أطفال
        public bool HasChildren()
        {
            return Children != null && Children.Any();
        }
    }

    public class FolderStructureReader
    {
        private dynamic? folderStructure;


        private static string TMFolderStructure = @"
{
  ""Controllers"": [ ""Api"", ""Auth"", ""Admin"" ],
  ""Repositories"": [ ""Base"", ""Builder"", ""Share"" ],
  ""Services"": [ ""Email"", ""Logging"" ],
  ""DyModels"": [
    {
      ""VM"": [],
      ""Dto"": {
        ""Build"": [ ""Request"", ""Response"", ""ResponseFilter"" ],
        ""Share"": [ ""Request"", ""Response"", ""ResponseFilter"" ]
      },
      ""Dso"": [ ""Request"", ""Response"", ""ResponseFilter"" ]
    }
  ],
  ""Config"": [ ""Mappers"", ""Scopes"", ""Singletons"", ""Transients"" ],
  ""Models"": [],
  ""Builders"": [ ""Db"" ],
  ""Helper"": [],
  ""Data"": [],
  ""Enums"": [],
  ""Validators"": [ ""Conditions"" ],
  ""Schedulers"": []
}";


        /// <summary>
        /// تحميل الهيكلية من ملف JSON.
        /// </summary>
        public void LoadFromJson(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    string json = System.IO.File.ReadAllText(filePath);
                    folderStructure = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                }
                else
                {
                   
                    folderStructure = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(TMFolderStructure);



                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ حدث خطأ أثناء تحميل الهيكلية: {ex.Message}");
            }
        }

        /// <summary>
        /// تحويل الهيكلية إلى شجرة FolderNode.
        /// </summary>
        public FolderNode? BuildFolderTree(string folderName, dynamic structure = null)
        {
            structure = structure ?? folderStructure;
            FolderNode? node = new FolderNode(folderName, null);

            if (structure is Newtonsoft.Json.Linq.JObject)
            {
                foreach (var property in ((Newtonsoft.Json.Linq.JObject)structure).Properties())
                {
                    FolderNode? childNode = BuildFolderTree(property.Name, property.Value);
                    node.Children?.Add(childNode);
                }
            }
            else if (structure is Newtonsoft.Json.Linq.JArray)
            {
                foreach (var item in (Newtonsoft.Json.Linq.JArray)structure)
                {
                    if (item.Type == Newtonsoft.Json.Linq.JTokenType.String)
                    {
                        FolderNode? childNode = new FolderNode(item.ToString(), null);
                        node?.Children?.Add(childNode);
                    }
                    else if (item is Newtonsoft.Json.Linq.JObject)
                    {
                        foreach (var property in ((Newtonsoft.Json.Linq.JObject)item).Properties())
                        {
                            FolderNode? childNode = BuildFolderTree(property.Name, property.Value);
                            node?.Children?.Add(childNode);
                        }
                    }
                }
            }
            else if (structure is Newtonsoft.Json.Linq.JValue)
            {
                node?.Children?.Add(new FolderNode(structure.ToString(), null));
            }

            return node;
        }

        /// <summary>
        /// طباعة شجرة المجلدات للتأكد من صحتها.
        /// </summary>
        public void PrintFolderTree(FolderNode node, string indent = "")
        {
            Console.WriteLine(indent + node.Name);
            foreach (var child in node.Children)
            {
                PrintFolderTree(child, indent + "  ");
            }
        }

        public event EventHandler<FolderEventArgs>? FolderCreated;
        public event EventHandler<FolderEventArgs>? FolderCreating;

        // ✅ حدث عند إنشاء ملف
        public event EventHandler<FileEventArgs>? FileCreating;

        /// <summary>
        /// إنشاء المجلدات على النظام وإضافة ملف Base.cs داخل كل منها.
        /// </summary>
        public void CreateFolders(string basePath, FolderNode node)
        {
            try
            {
                string folderPath = Path.Combine(basePath, node.Name);

                // 🔥 تشغيل الحدث عند إنشاء المجلد
               
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    Console.WriteLine($"📁 تم إنشاء المجلد: {folderPath}");
                }
                FolderCreating?.Invoke(this, new FolderEventArgs(node, folderPath));



                if (node.Children == null || node.Children.Count == 0)
                {
                    // ✅ إنشاء ملف Base.cs داخل المجلد
                    string baseFilePath = Path.Combine(folderPath, "README.md");

                    // 🔥 تشغيل الحدث عند إنشاء الملف
                    FileCreating?.Invoke(this, new FileEventArgs(node, folderPath));

                    if (!File.Exists(baseFilePath))
                    {
                        var parent = folderPath.Split("\\");
                        if (parent.Length > 1)
                        {
                            var nameSpace = $"{parent[parent.Length - 2]}.{parent[parent.Length - 1]}";
                            File.WriteAllText(baseFilePath, GetBaseClassTemplate($"Base{node.Name}{parent[parent.Length - 2]}", nameSpace));
                            Console.WriteLine($"📝 تم إنشاء الملف: {baseFilePath}");
                        }
                    }
                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        CreateFolders(folderPath, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {ex.Message}");
            }
        }



        public void OnAfterCreatedFolders(string basePath, FolderNode node)
        {
            try
            {
                string folderPath = Path.Combine(basePath, node.Name);

               

               
                FolderCreated?.Invoke(this, new FolderEventArgs(node, folderPath));



                if (node.Children == null || node.Children.Count == 0)
                {
                  
                  
                           
                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        OnAfterCreatedFolders(folderPath, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ حدث خطأ أثناء إنشاء المجلدات والملفات: {ex.Message}");
            }
        }

        /// <summary>
        /// </summary>
        private string GetBaseClassTemplate(string className, string nameSpace)
        {
            return $@"using System;

namespace {nameSpace}
{{
    public class {className}
    {{
        public {className}()
        {{
            Console.WriteLine(""Base class initialized in {className}"");
        }}
    }}
}}";
        }
    }



}
