using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// ApplicationUser  property for VM Update.
    /// </summary>
    public class ApplicationUserUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public ApplicationUserCreateVM? Body { get; set; }
    }
}