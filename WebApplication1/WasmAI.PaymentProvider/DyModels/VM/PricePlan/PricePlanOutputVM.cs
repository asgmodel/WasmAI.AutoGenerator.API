using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// PricePlan  property for VM Output.
    /// </summary>
    public class PricePlanOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
        ///
        public String? ExternalId { get; set; }
        ///
        public String? Name { get; set; }
        ///
        public Decimal Amount { get; set; }
        ///
        public String? Currency { get; set; }
        ///
        public String? Interval { get; set; }
        ///
        public Nullable<Int32> IntervalCount { get; set; }
        ///
        public String? UsageType { get; set; }
        ///
        public Boolean IsActive { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public String? ProductId { get; set; }
        public ProductOutputVM? Product { get; set; }
    }
}