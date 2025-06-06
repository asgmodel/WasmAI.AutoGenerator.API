using AutoGenerator;
using AutoGenerator.Conditions;
using AutoMapper;
using WasmAI.PaymentProvider.Data;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Validators.Conditions
{
    public interface ITFactoryInjector : ITBaseFactoryInjector
    {
        public PaymentDbContext Context { get; }
    }
}