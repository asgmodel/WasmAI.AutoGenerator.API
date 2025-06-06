using AutoGenerator;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Conditions;
using WasmAI.PaymentProvider.Validators.Conditions;
using WasmAI.PaymentProvider.Models;
using WasmAI.PaymentProvider.Validators.Conditions;
using WasmAI.ConditionChecker.Base;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Validators
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