using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Payment  property for VM Create.
    /// </summary>
    public class PaymentCreateVM : ITVM
    {
        ///
        public String? ExternalId { get; set; }
        ///
        public String? CustomerId { get; set; }
        // public CustomerCreateVM? Customer { get; set; }
        ///
        public Decimal Amount { get; set; }
        ///
        public String? Currency { get; set; }
        ///
        public String? Description { get; set; }
        ///
        public String? Status { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public Nullable<DateTime> ProcessedAt { get; set; }
        ///
        public String? FailureReason { get; set; }
        //
        public List<RefundCreateVM>? Refunds { get; set; }
    }
}