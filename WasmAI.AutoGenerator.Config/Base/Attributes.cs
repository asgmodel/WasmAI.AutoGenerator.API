
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoGenerator
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FolderGeneratorAttribute : Attribute
    {
        public string FolderName { get; }

        public FolderGeneratorAttribute(string folderName)
        {
            FolderName = folderName;
        }
    }


    [AttributeUsage( AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ToTranslationAttribute : Attribute
    {
        public bool IsEnable { get; set; }

        // Constructor with default value as true
        public ToTranslationAttribute(bool isenable = true)
        {
            IsEnable = isenable;
        }


        public static bool CheckTo(Type type)
        {
            var attribute = type.GetCustomAttribute<ToTranslationAttribute>();

            // Return true if the attribute exists and IgnoreMapping is true, otherwise false
            return attribute != null && attribute.IsEnable;
        }
    }

}

