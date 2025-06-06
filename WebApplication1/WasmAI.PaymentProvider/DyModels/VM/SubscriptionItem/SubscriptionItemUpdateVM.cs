using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// SubscriptionItem  property for VM Update.
    /// </summary>
    public class SubscriptionItemUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public SubscriptionItemCreateVM? Body { get; set; }
    }
}