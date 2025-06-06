using AutoGenerator.Conditions;
using WasmAI.PaymentProvider.Validators.Conditions;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Validators
{
    public class ConditionChecker : BaseConditionChecker, IConditionChecker
    {
        private readonly ITFactoryInjector _injector;
        public ITFactoryInjector Injector => _injector;

        public ConditionChecker(ITFactoryInjector injector) : base()
        {
        }
    // الدوال السابقة تبقى كما هي
    }
}