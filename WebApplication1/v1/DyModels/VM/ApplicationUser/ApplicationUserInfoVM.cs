using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.VMs
{
    /// <summary>
    /// ApplicationUser  property for VM Info.
    /// </summary>
    public class ApplicationUserInfoVM : ITVM
    {
        ///
        public string? Id { get; set; }
    }
}