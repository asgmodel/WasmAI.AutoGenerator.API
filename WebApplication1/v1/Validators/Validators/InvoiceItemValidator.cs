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
    public class InvoiceItemValidatorContext : ValidatorContext<InvoiceItem, InvoiceItemValidatorStates>, ITValidator
    {
        public InvoiceItemValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public  enum  InvoiceItemValidatorStates //
    { IsActive ,  IsFull ,  IsValid ,  //
    }

}