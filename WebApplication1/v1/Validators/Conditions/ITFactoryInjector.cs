using AutoGenerator;
using AutoGenerator.Conditions;
using AutoMapper;
using WasmAI.PaymentProvider.Data;
using System;
using v1.BPR.Layers.Base;

namespace v1.Validators.Conditions
{
    public interface ITFactoryInjector : ITBaseFactoryInjector
    {
        public PaymentDbContext Context { get; }
    }
}