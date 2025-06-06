using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// PricePlan  property for VM Delete.
    /// </summary>
    public class PricePlanDeleteVM : ITVM
    {
        ///
        public string? Id { get; set; }
    }
}