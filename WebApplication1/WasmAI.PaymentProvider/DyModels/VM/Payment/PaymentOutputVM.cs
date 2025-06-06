using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Payment  property for VM Output.
    /// </summary>
    public class PaymentOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
        ///
        public String? ExternalId { get; set; }
        ///
        public String? CustomerId { get; set; }
        public CustomerOutputVM? Customer { get; set; }
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
        public List<RefundOutputVM>? Refunds { get; set; }
    }
}