using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// Discount  property for VM Update.
    /// </summary>
    public class DiscountUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public DiscountCreateVM? Body { get; set; }
    }
}