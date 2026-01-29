using AutoGenerator;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Conditions;
using v1.Validators.Conditions;
using WasmAI.PaymentProvider.Models;
using v1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;
using System;
using v1.BPR.Layers.Base;

namespace v1.Validators
{
    public class PricePlanValidatorContext : ValidatorContext<PricePlan, PricePlanValidatorStates>, ITValidator
    {
        public PricePlanValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public  enum  PricePlanValidatorStates //
    { IsActive ,  IsFull ,  IsValid ,  //
    }

}