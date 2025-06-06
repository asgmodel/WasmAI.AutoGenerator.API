using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.Dto.Build.Requests
{
    public class CheckoutSessionRequestBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"checkoutsession_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// ExternalId property for DTO.
        /// </summary>
        public String? ExternalId { get; set; }
        /// <summary>
        /// ProviderName property for DTO.
        /// </summary>
        public String? ProviderName { get; set; }
        /// <summary>
        /// CustomerId property for DTO.
        /// </summary>
        public String? CustomerId { get; set; }
        public CustomerRequestBuildDto? Customer { get; set; }
        /// <summary>
        /// Status property for DTO.
        /// </summary>
        public String? Status { get; set; }
        /// <summary>
        /// Mode property for DTO.
        /// </summary>
        public String? Mode { get; set; }
        /// <summary>
        /// AmountTotal property for DTO.
        /// </summary>
        public Nullable<Decimal> AmountTotal { get; set; }
        /// <summary>
        /// Currency property for DTO.
        /// </summary>
        public String? Currency { get; set; }
        /// <summary>
        /// SuccessUrl property for DTO.
        /// </summary>
        public String? SuccessUrl { get; set; }
        /// <summary>
        /// CancelUrl property for DTO.
        /// </summary>
        public String? CancelUrl { get; set; }
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// ExpiresAt property for DTO.
        /// </summary>
        public Nullable<DateTime> ExpiresAt { get; set; }
        /// <summary>
        /// CompletedAt property for DTO.
        /// </summary>
        public Nullable<DateTime> CompletedAt { get; set; }
    }
}