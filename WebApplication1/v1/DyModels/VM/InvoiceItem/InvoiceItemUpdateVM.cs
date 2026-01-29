using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
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