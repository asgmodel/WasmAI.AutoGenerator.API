using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// ActivityLog  property for VM Create.
    /// </summary>
    public class ActivityLogCreateVM : ITVM
    {
        ///
        public String? EntityName { get; set; }
        ///
        public String? EntityId { get; set; }
        ///
        public String? Action { get; set; }
        ///
        public String? Description { get; set; }
        ///
        public DateTime Timestamp { get; set; }
        ///
        public String? UserId { get; set; }
        ///
        public String? AdditionalData { get; set; }
    }
}