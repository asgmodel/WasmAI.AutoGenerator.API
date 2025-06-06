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
    public class RefundValidatorContext : ValidatorContext<Refund, RefundValidatorStates>, ITValidator
    {
        public RefundValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public  enum  RefundValidatorStates //
    { IsActive ,  IsFull ,  IsValid ,  //
    }

}