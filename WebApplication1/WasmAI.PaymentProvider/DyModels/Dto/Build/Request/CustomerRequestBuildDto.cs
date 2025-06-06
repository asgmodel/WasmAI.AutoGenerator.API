using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.Dto.Build.Requests
{
    public class CustomerRequestBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"customer_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// ExternalId property for DTO.
        /// </summary>
        public String? ExternalId { get; set; }
        /// <summary>
        /// Name property for DTO.
        /// </summary>
        public String? Name { get; set; }
        /// <summary>
        /// Email property for DTO.
        /// </summary>
        public String? Email { get; set; }
        /// <summary>
        /// PhoneNumber property for DTO.
        /// </summary>
        public String? PhoneNumber { get; set; }
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        public ICollection<PaymentMethodRequestBuildDto>? PaymentMethods { get; set; }
        public ICollection<SubscriptionRequestBuildDto>? Subscriptions { get; set; }
        public ICollection<InvoiceRequestBuildDto>? Invoices { get; set; }
        public ICollection<DiscountRequestBuildDto>? Discounts { get; set; }
        public ICollection<CheckoutSessionRequestBuildDto>? CheckoutSessions { get; set; }
    }
}