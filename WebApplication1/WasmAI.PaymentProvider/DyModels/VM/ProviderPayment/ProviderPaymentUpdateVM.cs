using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
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