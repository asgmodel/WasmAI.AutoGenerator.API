using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Product  property for VM Update.
    /// </summary>
    public class ProductUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public ProductCreateVM? Body { get; set; }
    }
}