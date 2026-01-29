using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
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