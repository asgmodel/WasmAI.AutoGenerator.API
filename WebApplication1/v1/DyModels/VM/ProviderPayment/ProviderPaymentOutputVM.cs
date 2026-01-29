using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// ProviderPayment  property for VM Output.
    /// </summary>
    public class ProviderPaymentOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
        ///
        public String? Name { get; set; }
        ///
        public String? Description { get; set; }
        //
        public List<ProductOutputVM>? Products { get; set; }
    }
}