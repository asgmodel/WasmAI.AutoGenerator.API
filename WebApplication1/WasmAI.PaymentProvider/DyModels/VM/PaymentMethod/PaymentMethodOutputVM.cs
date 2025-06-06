using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// PaymentMethod  property for VM Output.
    /// </summary>
    public class PaymentMethodOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
        ///
        public String? ExternalId { get; set; }
        ///
        public String? Type { get; set; }
        ///
        public String? CardBrand { get; set; }
        ///
        public String? Last4 { get; set; }
        ///
        public Nullable<DateTime> Expiry { get; set; }
        ///
        public Boolean IsDefault { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public String? CustomerId { get; set; }
        public CustomerOutputVM? Customer { get; set; }
    }
}