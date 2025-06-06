using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.Dto.Build.Requests
{
    public class PricePlanRequestBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"priceplan_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// ExternalId property for DTO.
        /// </summary>
        public String? ExternalId { get; set; }
        /// <summary>
        /// Name property for DTO.
        /// </summary>
        public String? Name { get; set; }
        /// <summary>
        /// Amount property for DTO.
        /// </summary>
        public Decimal Amount { get; set; }
        /// <summary>
        /// Currency property for DTO.
        /// </summary>
        public String? Currency { get; set; }
        /// <summary>
        /// Interval property for DTO.
        /// </summary>
        public String? Interval { get; set; }
        /// <summary>
        /// IntervalCount property for DTO.
        /// </summary>
        public Nullable<Int32> IntervalCount { get; set; }
        /// <summary>
        /// UsageType property for DTO.
        /// </summary>
        public String? UsageType { get; set; }
        /// <summary>
        /// IsActive property for DTO.
        /// </summary>
        public Boolean IsActive { get; set; }
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// ProductId property for DTO.
        /// </summary>
        public String? ProductId { get; set; }
        public ProductRequestBuildDto? Product { get; set; }
    }
}