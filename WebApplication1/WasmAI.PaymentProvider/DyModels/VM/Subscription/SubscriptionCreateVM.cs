using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Subscription  property for VM Create.
    /// </summary>
    public class SubscriptionCreateVM : ITVM
    {
        ///
        public String? ExternalId { get; set; }
        ///
        public String? CustomerId { get; set; }
        // public CustomerCreateVM? Customer { get; set; }
        ///
        public String? PricePlanExternalId { get; set; }
        ///
        public String? Status { get; set; }
        ///
        public DateTime StartDate { get; set; }
        ///
        public Nullable<DateTime> EndDate { get; set; }
        ///
        public Nullable<DateTime> TrialEndDate { get; set; }
        ///
        public Boolean CancelAtPeriodEnd { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        //
        public List<SubscriptionItemCreateVM>? Items { get; set; }
    }
}