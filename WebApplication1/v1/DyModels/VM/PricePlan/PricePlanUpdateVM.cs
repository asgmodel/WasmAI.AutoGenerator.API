using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// PricePlan  property for VM Update.
    /// </summary>
    public class PricePlanUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public PricePlanCreateVM? Body { get; set; }
    }
}