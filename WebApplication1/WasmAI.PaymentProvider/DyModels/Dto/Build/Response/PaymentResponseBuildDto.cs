using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.Dto.Build.Responses
{
    public class PaymentResponseBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"payment_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// ExternalId property for DTO.
        /// </summary>
        public String? ExternalId { get; set; }
        /// <summary>
        /// CustomerId property for DTO.
        /// </summary>
        public String? CustomerId { get; set; }
        public CustomerResponseBuildDto? Customer { get; set; }
        /// <summary>
        /// Amount property for DTO.
        /// </summary>
        public Decimal Amount { get; set; }
        /// <summary>
        /// Currency property for DTO.
        /// </summary>
        public String? Currency { get; set; }
        /// <summary>
        /// Description property for DTO.
        /// </summary>
        public String? Description { get; set; }
        /// <summary>
        /// Status property for DTO.
        /// </summary>
        public String? Status { get; set; }
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// ProcessedAt property for DTO.
        /// </summary>
        public Nullable<DateTime> ProcessedAt { get; set; }
        /// <summary>
        /// FailureReason property for DTO.
        /// </summary>
        public String? FailureReason { get; set; }
        public ICollection<RefundResponseBuildDto>? Refunds { get; set; }
    }
}