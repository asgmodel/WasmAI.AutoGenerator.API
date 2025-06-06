using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.Dto.Build.Responses
{
    public class PaymentMethodResponseBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"paymentmethod_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// ExternalId property for DTO.
        /// </summary>
        public String? ExternalId { get; set; }
        /// <summary>
        /// Type property for DTO.
        /// </summary>
        public String? Type { get; set; }
        /// <summary>
        /// CardBrand property for DTO.
        /// </summary>
        public String? CardBrand { get; set; }
        /// <summary>
        /// Last4 property for DTO.
        /// </summary>
        public String? Last4 { get; set; }
        /// <summary>
        /// Expiry property for DTO.
        /// </summary>
        public Nullable<DateTime> Expiry { get; set; }
        /// <summary>
        /// IsDefault property for DTO.
        /// </summary>
        public Boolean IsDefault { get; set; }
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// CustomerId property for DTO.
        /// </summary>
        public String? CustomerId { get; set; }
        public CustomerResponseBuildDto? Customer { get; set; }
    }
}