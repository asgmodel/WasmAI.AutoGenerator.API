namespace AutoGenerator.Conditions
{



    public interface IBaseConditionChecker: WasmAI.ConditionChecker.Checker.IBaseConditionChecker
    {

    }


    public abstract class BaseConditionChecker : WasmAI.ConditionChecker.Checker.BaseConditionChecker, IBaseConditionChecker
    {
        public BaseConditionChecker() { }
   
    }


}





