using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// WebhookEvent  property for VM Output.
    /// </summary>
    public class WebhookEventOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
        ///
        public String? EventId { get; set; }
        ///
        public String? EventType { get; set; }
        ///
        public String? Payload { get; set; }
        ///
        public String? ProviderName { get; set; }
        ///
        public DateTime ReceivedAt { get; set; }
        ///
        public Boolean Processed { get; set; }
        ///
        public Nullable<DateTime> ProcessedAt { get; set; }
        ///
        public String? ProcessingError { get; set; }
    }
}