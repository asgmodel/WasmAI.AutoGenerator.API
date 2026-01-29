using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using v1.BPR.Layers.Base;

namespace v1.DyModels.Dto.Build.Requests
{
    public class InvoiceItemRequestBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"invoiceitem_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// Description property for DTO.
        /// </summary>
        public String? Description { get; set; }
        /// <summary>
        /// Amount property for DTO.
        /// </summary>
        public Decimal Amount { get; set; }
        /// <summary>
        /// Quantity property for DTO.
        /// </summary>
        public Int32 Quantity { get; set; }
        /// <summary>
        /// Total property for DTO.
        /// </summary>
        public Decimal Total { get; set; }
        /// <summary>
        /// InvoiceId property for DTO.
        /// </summary>
        public String? InvoiceId { get; set; }
        public InvoiceRequestBuildDto? Invoice { get; set; }
    }
}