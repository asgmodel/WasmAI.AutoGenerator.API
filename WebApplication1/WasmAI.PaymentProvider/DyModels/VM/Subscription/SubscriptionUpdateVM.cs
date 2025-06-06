using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Subscription  property for VM Update.
    /// </summary>
    public class SubscriptionUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public SubscriptionCreateVM? Body { get; set; }
    }
}