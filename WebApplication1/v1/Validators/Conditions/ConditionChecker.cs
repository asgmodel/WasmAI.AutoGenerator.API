using AutoGenerator.Conditions;
using v1.Validators.Conditions;
using System;
using v1.BPR.Layers.Base;

namespace v1.Validators
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