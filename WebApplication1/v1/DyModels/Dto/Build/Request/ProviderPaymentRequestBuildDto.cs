using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.Dto.Build.Requests
{
    public class ProviderPaymentRequestBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"providerpayment_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// Name property for DTO.
        /// </summary>
        public String? Name { get; set; }
        /// <summary>
        /// Description property for DTO.
        /// </summary>
        public String? Description { get; set; }
        public ICollection<ProductRequestBuildDto>? Products { get; set; }
    }
}