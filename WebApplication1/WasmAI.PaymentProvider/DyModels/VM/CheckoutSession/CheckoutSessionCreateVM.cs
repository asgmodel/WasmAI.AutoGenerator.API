using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// CheckoutSession  property for VM Create.
    /// </summary>
    public class CheckoutSessionCreateVM : ITVM
    {
        ///
        public String? ExternalId { get; set; }
        ///
        public String? ProviderName { get; set; }
        ///
        public String? CustomerId { get; set; }
        // public CustomerCreateVM? Customer { get; set; }
        ///
        public String? Status { get; set; }
        ///
        public String? Mode { get; set; }
        ///
        public Nullable<Decimal> AmountTotal { get; set; }
        ///
        public String? Currency { get; set; }
        ///
        public String? SuccessUrl { get; set; }
        ///
        public String? CancelUrl { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public Nullable<DateTime> ExpiresAt { get; set; }
        ///
        public Nullable<DateTime> CompletedAt { get; set; }
    }
}