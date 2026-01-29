using AutoGenerator.Conditions;
using v1.Validators.Conditions;
using System;
using v1.BPR.Layers.Base;

namespace v1.Validators
{
    public interface IConditionChecker : IBaseConditionChecker
    {
        public ITFactoryInjector Injector { get; }
    }
}