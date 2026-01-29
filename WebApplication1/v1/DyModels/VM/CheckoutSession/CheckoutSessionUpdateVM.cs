using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// CheckoutSession  property for VM Update.
    /// </summary>
    public class CheckoutSessionUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public CheckoutSessionCreateVM? Body { get; set; }
    }
}