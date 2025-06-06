using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using AutoGenerator.Config;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.Dto.Build.Requests
{
    public class ActivityLogRequestBuildDto : ITBuildDto
    {
        public string? Id { get; set; } = $"activitylog_{Guid.NewGuid().ToString()}";
        /// <summary>
        /// EntityName property for DTO.
        /// </summary>
        public String? EntityName { get; set; }
        /// <summary>
        /// EntityId property for DTO.
        /// </summary>
        public String? EntityId { get; set; }
        /// <summary>
        /// Action property for DTO.
        /// </summary>
        public String? Action { get; set; }
        /// <summary>
        /// Description property for DTO.
        /// </summary>
        public String? Description { get; set; }
        /// <summary>
        /// Timestamp property for DTO.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// UserId property for DTO.
        /// </summary>
        public String? UserId { get; set; }
        /// <summary>
        /// AdditionalData property for DTO.
        /// </summary>
        public String? AdditionalData { get; set; }
    }
}