using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// InvoiceItem  property for VM Output.
    /// </summary>
    public class InvoiceItemOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
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
        public InvoiceOutputVM? Invoice { get; set; }
    }
}