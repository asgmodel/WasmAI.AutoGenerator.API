using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// ProviderPayment  property for VM Update.
    /// </summary>
    public class ProviderPaymentUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public ProviderPaymentCreateVM? Body { get; set; }
    }
}