using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// WebhookEvent  property for VM Update.
    /// </summary>
    public class WebhookEventUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public WebhookEventCreateVM? Body { get; set; }
    }
}