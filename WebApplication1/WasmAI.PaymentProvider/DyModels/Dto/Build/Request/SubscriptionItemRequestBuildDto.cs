using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.Dto.Build.Requests
{
    public class SubscriptionItemRequestBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"subscriptionitem_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// ExternalId property for DTO.
        /// </summary>
        public String? ExternalId { get; set; }
        /// <summary>
        /// SubscriptionId property for DTO.
        /// </summary>
        public String? SubscriptionId { get; set; }
        public SubscriptionRequestBuildDto? Subscription { get; set; }
        /// <summary>
        /// PricePlanExternalId property for DTO.
        /// </summary>
        public String? PricePlanExternalId { get; set; }
        /// <summary>
        /// Quantity property for DTO.
        /// </summary>
        public Int32 Quantity { get; set; }
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}