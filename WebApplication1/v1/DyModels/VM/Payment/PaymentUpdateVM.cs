using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// Payment  property for VM Update.
    /// </summary>
    public class PaymentUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public PaymentCreateVM? Body { get; set; }
    }
}