using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// Product  property for VM Output.
    /// </summary>
    public class ProductOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
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
        public ProviderPaymentOutputVM? Provider { get; set; }
        //
        public List<PricePlanOutputVM>? PricePlans { get; set; }
    }
}