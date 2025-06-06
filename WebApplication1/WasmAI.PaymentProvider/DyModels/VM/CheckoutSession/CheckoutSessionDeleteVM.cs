using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// CheckoutSession  property for VM Delete.
    /// </summary>
    public class CheckoutSessionDeleteVM : ITVM
    {
        ///
        public string? Id { get; set; }
    }
}