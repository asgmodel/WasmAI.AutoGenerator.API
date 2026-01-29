using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// Payment  property for VM Info.
    /// </summary>
    public class PaymentInfoVM : ITVM
    {
        ///
        public string? Id { get; set; }
    }
}