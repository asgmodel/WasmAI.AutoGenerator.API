

using WasmAI.ConditionChecker.Validators;

namespace AutoGenerator.Conditions
{

   
    public interface ITValidator : WasmAI.ConditionChecker.Validators.ITValidator
    {
        
    }

    public abstract class BaseValidator<TContext, EValidator> : BaseValidator<EValidator>, IValidator<TContext>, ITValidator
        where TContext : class
        where EValidator : Enum
    {
        protected BaseValidator(IBaseConditionChecker checker) : base(checker)
        {
        }
    }
    public abstract class BaseValidatorContext<TContext, EValidator> : WasmAI.ConditionChecker.Validators.BaseValidatorContext<TContext, EValidator>
    where TContext : class
    where EValidator : Enum

    {
        protected BaseValidatorContext(IBaseConditionChecker checker) : base(checker)
        {
        }
    }



}