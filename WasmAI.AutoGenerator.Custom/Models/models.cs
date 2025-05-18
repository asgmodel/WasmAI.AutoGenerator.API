

using Microsoft.AspNetCore.Identity;

namespace AutoGenerator.Custom.Models
{
    public class ApplicationCodeUser : IdentityUser<string>,ITModel
    {
        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }

    public class Section : ITModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
      
        public string ApplicationCodeUserId { get; set; } = null!;
        public ApplicationCodeUser ApplicationCodeUser { get; set; } = null!;

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }

    public class Project : ITModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!; // Project or Library
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string SectionId { get; set; } = null!;
        public Section Section { get; set; } = null!;

      
        public ICollection<Folder> Folders { get; set; } = new List<Folder>();
    }

    public class Folder : ITModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string ProjectId { get; set; } = null!;
        public Project Project { get; set; } = null!;

        public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
    }

    public class FileEntity:ITModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public  string Content{ get; set; } = null!;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public string FolderId { get; set; } = null!;
        public Folder Folder { get; set; } = null!;
    }

    public class CodeGenerator : ITModel
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString(); // معرف فريد للكود
        public string? Name { get; set; } // اسم الكود (مثلاً اسم الفئة أو الواجهة)
        public string? Type { get; set; } // نوع الكود (يمثل الفئة الرئيسية مثل Class, Interface, Enum)
        public string? SubType { get; set; } // نوع فرعي (مثلاً DTO, Service, Controller)
        public string? PathFile { get; set; } // المسار المتوقع للملف الناتج

        public string? Code { get; set; } // محتوى الكود الفعلي (النص البرمجي)

        public string? NamespaceName { get; set; } // مساحة الاسم التي ينتمي إليها الكود
        public string AdditionalCode { get; set; } = string.Empty; // كود إضافي قد يتم إلحاقه

        [ToTranslation]
        public string? Usings { get; set; }  // قائمة بأسطر الـ using المطلوبة
        public string? BaseClass { get; set; } // اسم الفئة الأساسية التي يرث منها (إن وجدت)

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; // وقت إنشاء الكود
        public string ProjectId { get; set; } = null!;
        public Project Project { get; set; } = null!;


        public ICollection<CodeHistory>? CodeHistories { get; set; }// تاريخ الكود (النسخ السابقة)

        // يمكنك إضافة أي خصائص أخرى هنا حسب الحاجة
    }




    public class CodeHistory : ITModel
    {
        public string? Id { get; set; }= Guid.NewGuid().ToString();
        public string? CodeId { get; set; } 
        public string? CodeSnapshot { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public string? CodeGeneratorId { get; set; } 
        public CodeGenerator? CodeGenerator { get; set; } 

    }

}