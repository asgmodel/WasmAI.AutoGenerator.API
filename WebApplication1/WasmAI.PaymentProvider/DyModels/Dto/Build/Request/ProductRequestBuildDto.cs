using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.Dto.Build.Requests
{
    public class ProductRequestBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"product_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// Name property for DTO.
        /// </summary>
        public String? Name { get; set; }
        /// <summary>
        /// Description property for DTO.
        /// </summary>
        public String? Description { get; set; }
        /// <summary>
        /// IsActive property for DTO.
        /// </summary>
        public Boolean IsActive { get; set; }
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// ProviderId property for DTO.
        /// </summary>
        public String? ProviderId { get; set; }
        public ProviderPaymentRequestBuildDto? Provider { get; set; }
        public ICollection<PricePlanRequestBuildDto>? PricePlans { get; set; }
    }
}