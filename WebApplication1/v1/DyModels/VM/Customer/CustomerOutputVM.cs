using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// Customer  property for VM Output.
    /// </summary>
    public class CustomerOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
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
        public List<PaymentMethodOutputVM>? PaymentMethods { get; set; }
        //
        public List<SubscriptionOutputVM>? Subscriptions { get; set; }
        //
        public List<InvoiceOutputVM>? Invoices { get; set; }
        //
        public List<DiscountOutputVM>? Discounts { get; set; }
        //
        public List<CheckoutSessionOutputVM>? CheckoutSessions { get; set; }
    }
}