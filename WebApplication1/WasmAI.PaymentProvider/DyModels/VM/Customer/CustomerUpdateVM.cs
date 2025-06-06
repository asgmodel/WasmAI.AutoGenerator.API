using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Customer  property for VM Update.
    /// </summary>
    public class CustomerUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public CustomerCreateVM? Body { get; set; }
    }
}