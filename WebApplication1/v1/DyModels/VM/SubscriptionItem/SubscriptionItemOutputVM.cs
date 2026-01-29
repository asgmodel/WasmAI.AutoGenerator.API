using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// SubscriptionItem  property for VM Output.
    /// </summary>
    public class SubscriptionItemOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
        ///
        public String? ExternalId { get; set; }
        ///
        public String? SubscriptionId { get; set; }
        public SubscriptionOutputVM? Subscription { get; set; }
        ///
        public String? PricePlanExternalId { get; set; }
        ///
        public Int32 Quantity { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
    }
}