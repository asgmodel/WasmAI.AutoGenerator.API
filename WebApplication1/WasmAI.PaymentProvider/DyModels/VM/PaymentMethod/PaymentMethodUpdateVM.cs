using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// PaymentMethod  property for VM Update.
    /// </summary>
    public class PaymentMethodUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public PaymentMethodCreateVM? Body { get; set; }
    }
}