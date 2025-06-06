using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// InvoiceItem  property for VM Update.
    /// </summary>
    public class InvoiceItemUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public InvoiceItemCreateVM? Body { get; set; }
    }
}