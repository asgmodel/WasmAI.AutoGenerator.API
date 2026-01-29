using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// Invoice  property for VM Update.
    /// </summary>
    public class InvoiceUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public InvoiceCreateVM? Body { get; set; }
    }
}