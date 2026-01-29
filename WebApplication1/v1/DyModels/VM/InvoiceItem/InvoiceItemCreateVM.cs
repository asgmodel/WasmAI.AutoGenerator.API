using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// InvoiceItem  property for VM Create.
    /// </summary>
    public class InvoiceItemCreateVM : ITVM
    {
        ///
        public String? Description { get; set; }
        ///
        public Decimal Amount { get; set; }
        ///
        public Int32 Quantity { get; set; }
        ///
        public Decimal Total { get; set; }
        ///
        public String? InvoiceId { get; set; }
    // public InvoiceCreateVM? Invoice { get; set; }
    }
}