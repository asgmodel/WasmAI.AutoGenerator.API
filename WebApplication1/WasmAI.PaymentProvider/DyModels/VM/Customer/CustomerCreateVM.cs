using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Customer  property for VM Create.
    /// </summary>
    public class CustomerCreateVM : ITVM
    {
        ///
        public String? ExternalId { get; set; }
        ///
        public String? Name { get; set; }
        ///
        public String? Email { get; set; }
        ///
        public String? PhoneNumber { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        //
        public List<PaymentMethodCreateVM>? PaymentMethods { get; set; }
        //
        public List<SubscriptionCreateVM>? Subscriptions { get; set; }
        //
        public List<InvoiceCreateVM>? Invoices { get; set; }
        //
        public List<DiscountCreateVM>? Discounts { get; set; }
        //
        public List<CheckoutSessionCreateVM>? CheckoutSessions { get; set; }
    }
}