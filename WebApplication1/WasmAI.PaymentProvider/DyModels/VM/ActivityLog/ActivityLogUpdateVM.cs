using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// ActivityLog  property for VM Update.
    /// </summary>
    public class ActivityLogUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public ActivityLogCreateVM? Body { get; set; }
    }
}