using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.Dto.Build.Requests
{
    public class SubscriptionRequestBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"subscription_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// ExternalId property for DTO.
        /// </summary>
        public String? ExternalId { get; set; }
        /// <summary>
        /// CustomerId property for DTO.
        /// </summary>
        public String? CustomerId { get; set; }
        public CustomerRequestBuildDto? Customer { get; set; }
        /// <summary>
        /// PricePlanExternalId property for DTO.
        /// </summary>
        public String? PricePlanExternalId { get; set; }
        /// <summary>
        /// Status property for DTO.
        /// </summary>
        public String? Status { get; set; }
        /// <summary>
        /// StartDate property for DTO.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// EndDate property for DTO.
        /// </summary>
        public Nullable<DateTime> EndDate { get; set; }
        /// <summary>
        /// TrialEndDate property for DTO.
        /// </summary>
        public Nullable<DateTime> TrialEndDate { get; set; }
        /// <summary>
        /// CancelAtPeriodEnd property for DTO.
        /// </summary>
        public Boolean CancelAtPeriodEnd { get; set; }
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        public ICollection<SubscriptionItemRequestBuildDto>? Items { get; set; }
    }
}