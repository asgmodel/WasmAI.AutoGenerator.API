using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace AutoGenerator.Code.VM
{

    public  class NodeInfo
    {

    }

    public class MergeResultProblem : ProblemDetails
    {
        public List<string> MergedResults { get; set; } = new();
    }
    // ✅ 1. فيو موديل لتمثيل بيانات الكود الواحد
    public class CodeVM
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString(); // معرف فريد للكود
        public string? Name { get; set; } // اسم الكود (مثلاً اسم الفئة أو الواجهة)
        public string? Type { get; set; } // نوع الكود (يمثل الفئة الرئيسية مثل Class, Interface, Enum)
        public string? SubType { get; set; } // نوع فرعي (مثلاً DTO, Service, Controller)
        public string? PathFile { get; set; } // المسار المتوقع للملف الناتج

        public string? Code { get; set; } // محتوى الكود الفعلي (النص البرمجي)

        public string? NamespaceName { get; set; } // مساحة الاسم التي ينتمي إليها الكود
        public string AdditionalCode { get; set; } = string.Empty; // كود إضافي قد يتم إلحاقه
        public List<string>? Usings { get; set; } = new List<string>(); // قائمة بأسطر الـ using المطلوبة
        public string? BaseClass { get; set; } // اسم الفئة الأساسية التي يرث منها (إن وجدت)

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; // وقت إنشاء الكود


        // يمكنك إضافة أي خصائص أخرى هنا حسب الحاجة
    }

    // ✅ 2. فيو موديل لطلب تبديل الكود بين ملفين
    public class SwapCodeRequest
    {
        // معرف الملف المصدر للتبديل
        // استخدام null! لإبلاغ المترجم بأن هذه الخاصية يجب أن يتم تهيئتها خارج الفئة (يفترض الـ Controller)
        public CodeIdentifier Source { get; set; } = null!;

        // معرف الملف الهدف للتبديل
        public CodeIdentifier Target { get; set; } = null!;
    }

    // ✅ 3. فئة مساعدة لتمثيل معرف الكود (تستخدم داخل SwapCodeRequest)
    public class CodeIdentifier
    {
        // الـ ID الخاص بكائن CodeVM المراد الإشارة إليه
        public string Id { get; set; } = null!;

        // يمكن إضافة خصائص أخرى هنا إذا كان التحديد يتم بغير الـ Id (مثل PathFile)
        // public string? PathFile { get; set; }
    }

    // ✅ 4. فيو موديل لطلب دمج الكود لملف واحد أو مجموعة ملفات (لكل ملف في القائمة)
    public class MergeCodeRequest
    {
        // مسار الملف الهدف الذي سيتم دمج الكود الجديد فيه. يستخدم أيضاً للبحث عن CodeVM في الـ dictionary.
        public string FilePath { get; set; } = null!;

        // محتوى الكود القديم للملف. يمكن جلبه من CodeVM إذا لم يتم توفيره.
        public string OldCode { get; set; } = null!;

        // محتوى الكود الجديد الذي سيتم دمجه مع الكود القديم. هذا هو الكود الذي تم إنشاؤه بواسطة مولد الكود.
        public string NewCode { get; set; } = null!;

        // مسار ملف ثانوي اختياري. قد تستخدمه أداة ComprehensiveCodeMerger للمقارنة أو السياق.
        public string? FilePath2 { get; set; }
    }

    // ✅ 5. فيو موديل لتفاصيل المشكلة الموسعة (للاستجابات التي تحتوي على تفاصيل أخطاء إضافية)
    // يرث من ProblemDetails القياسي في ASP.NET Core
    //public class ExtendedProblemDetails : ProblemDetails
    //{
    //    // قام ProblemDetails القياسي في الإصدارات الحديثة بالفعل بتضمين Extensions
    //    // الاحتفاظ بهذا هنا يضمن تهيئة قاموس Extensions تلقائياً
    //    // استخدام new يخفي الخاصية الموجودة في الفئة الأساسية ويستخدم هذا التعريف بدلاً منها.
    //    public new IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>();
    //}

    // ✅ 6. فيو موديل مساعد لعرض تجميعات الفئة والنوع الفرعي
    // تستخدم في استجابة GetAllCategorySubTypes
    public class CategorySubType
    {
        // اسم الفئة (يمثل Type في CodeVM)
        public string Category { get; set; } = string.Empty;

        // قائمة بالأنواع الفرعية الفريدة المرتبطة بهذه الفئة
        public List<string> SubTypes { get; set; } = new List<string>();
    }

}