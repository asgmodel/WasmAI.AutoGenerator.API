using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Refund  property for VM Output.
    /// </summary>
    public class RefundOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
        ///
        public String? ExternalId { get; set; }
        ///
        public String? PaymentId { get; set; }
        public PaymentOutputVM? Payment { get; set; }
        ///
        public Decimal Amount { get; set; }
        ///
        public String? Currency { get; set; }
        ///
        public String? Status { get; set; }
        ///
        public String? Reason { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public Nullable<DateTime> ProcessedAt { get; set; }
    }
}