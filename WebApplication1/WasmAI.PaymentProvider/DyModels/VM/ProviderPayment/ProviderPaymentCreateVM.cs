using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// ProviderPayment  property for VM Create.
    /// </summary>
    public class ProviderPaymentCreateVM : ITVM
    {
        ///
        public String? Name { get; set; }
        ///
        public String? Description { get; set; }
        //
        public List<ProductCreateVM>? Products { get; set; }
    }
}