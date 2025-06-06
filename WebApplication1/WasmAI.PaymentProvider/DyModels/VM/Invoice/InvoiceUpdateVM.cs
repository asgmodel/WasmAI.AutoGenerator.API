using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
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