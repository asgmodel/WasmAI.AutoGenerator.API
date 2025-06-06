using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// InvoiceItem  property for VM Info.
    /// </summary>
    public class InvoiceItemInfoVM : ITVM
    {
        ///
        public string? Id { get; set; }
    }
}