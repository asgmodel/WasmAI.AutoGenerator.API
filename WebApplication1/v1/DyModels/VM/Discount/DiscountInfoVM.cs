using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// Discount  property for VM Info.
    /// </summary>
    public class DiscountInfoVM : ITVM
    {
        ///
        public string? Id { get; set; }
    }
}