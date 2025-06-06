using AutoGenerator.Conditions;
using WasmAI.PaymentProvider.Validators.Conditions;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Validators
{
    public interface IConditionChecker : IBaseConditionChecker
    {
        public ITFactoryInjector Injector { get; }
    }
}