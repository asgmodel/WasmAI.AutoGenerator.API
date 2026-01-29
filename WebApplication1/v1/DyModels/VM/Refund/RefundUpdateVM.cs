using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// Refund  property for VM Update.
    /// </summary>
    public class RefundUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public RefundCreateVM? Body { get; set; }
    }
}