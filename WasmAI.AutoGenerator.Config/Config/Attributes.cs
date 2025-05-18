
using System;
using System.Reflection;

namespace AutoGenerator.Config
{




    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class MapEnabledAttribute : Attribute
    {
        public bool IsMapped { get; set; }

        // Constructor to define whether mapping is enabled or not.
        public MapEnabledAttribute(bool isMapped = true)
        {
            IsMapped = isMapped;
        }
    }

    [AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ValidatorEnabledAttribute : Attribute
    {
        public bool IsValidatorped { get; set; }

        // Constructor to define whether Validatorping is enabled or not.
        public ValidatorEnabledAttribute(bool isValidatorped = true)
        {
            IsValidatorped = isValidatorped;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SchedulerEnabledAttribute : Attribute
    {
        public bool IsSchedulerped { get; set; }

        // Constructor to define whether Schedulerping is enabled or not.
        public SchedulerEnabledAttribute(bool isSchedulerped = true)
        {
            IsSchedulerped = isSchedulerped;
        }
    }


    [AttributeUsage(AttributeTargets.Property , Inherited = false, AllowMultiple = false)]
    public class FilterLGEnabledAttribute : Attribute
    {
        public bool IsEnable { get; set; }

        // Constructor to define whether mapping is enabled or not.
        public FilterLGEnabledAttribute(bool isenable = true)
        {
            IsEnable = IsEnable;
        }
    }



    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class IgnoreAutomateMapperAttribute : Attribute
    {
        public bool IgnoreMapping { get; set; }

        // Constructor with default value as true
        public IgnoreAutomateMapperAttribute(bool ignoreMapping = true)
        {
            IgnoreMapping = ignoreMapping;
        }
    }



    public  class GlobalAttribute
    {

        public static bool CheckFilterLGEnabled(Type type)
        {
            var attribute = type.GetCustomAttribute<FilterLGEnabledAttribute>();

            // Return true if the attribute exists and IgnoreMapping is true, otherwise false
            return attribute != null && attribute.IsEnable;
        }

    }

}
