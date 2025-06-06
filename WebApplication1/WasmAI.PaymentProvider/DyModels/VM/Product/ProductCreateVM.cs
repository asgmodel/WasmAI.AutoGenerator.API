using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Product  property for VM Create.
    /// </summary>
    public class ProductCreateVM : ITVM
    {
        ///
        public String? Name { get; set; }
        ///
        public String? Description { get; set; }
        ///
        public Boolean IsActive { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public String? ProviderId { get; set; }
        // public ProviderPaymentCreateVM? Provider { get; set; }
        //
        public List<PricePlanCreateVM>? PricePlans { get; set; }
    }
}