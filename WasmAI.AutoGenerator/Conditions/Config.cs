

using System.Reflection;



namespace AutoGenerator.Conditions
{
   


    public  static class  BaseConfigValidator
    {
      
        public static void Register(IBaseConditionChecker checker,Assembly assembly)
        {
            


            var validators = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ITValidator).IsAssignableFrom(t))
              
                .ToList();

            foreach (var validator in validators)
            {
                var instance = Activator.CreateInstance(validator, checker) as ITValidator;
                //instance?.Register(checker);
            }

            














        }
    }


}
       
