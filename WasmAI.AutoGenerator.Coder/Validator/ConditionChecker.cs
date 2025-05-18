using WasmAI.ConditionChecker.Checker;

namespace WasmAI.AutoGenerator.Code
{
    public interface ICodeConditionChecker : IBaseConditionChecker
    {
      
    }
    public class  CodeConditionChecker:BaseConditionChecker, ICodeConditionChecker
    {
        
        public CodeConditionChecker() : base()
        {
            
        }
    }
  
}
