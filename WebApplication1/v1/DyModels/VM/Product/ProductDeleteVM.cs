using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// Product  property for VM Delete.
    /// </summary>
    public class ProductDeleteVM : ITVM
    {
        ///
        public string? Id { get; set; }
    }
}