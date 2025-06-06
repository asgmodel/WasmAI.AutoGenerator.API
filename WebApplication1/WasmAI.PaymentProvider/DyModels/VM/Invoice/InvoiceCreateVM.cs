using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Invoice  property for VM Create.
    /// </summary>
    public class InvoiceCreateVM : ITVM
    {
        ///
        public String? ExternalId { get; set; }
        ///
        public String? CustomerId { get; set; }
        // public CustomerCreateVM? Customer { get; set; }
        ///
        public String? Currency { get; set; }
        ///
        public Decimal TotalAmount { get; set; }
        ///
        public String? Status { get; set; }
        ///
        public DateTime InvoiceDate { get; set; }
        ///
        public Nullable<DateTime> DueDate { get; set; }
        ///
        public Nullable<DateTime> PaidAt { get; set; }
        ///
        public String? Description { get; set; }
        ///
        public String? Number { get; set; }
        //
        public List<InvoiceItemCreateVM>? Items { get; set; }
    }
}