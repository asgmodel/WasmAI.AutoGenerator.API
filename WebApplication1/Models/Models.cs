using AutoGenerator;
using AutoGenerator.Config;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasmAI.PaymentProvider.Models
{

       public class ApplicationUser : IdentityUser, ITModel
    {

    }
    public class ProviderPayment: ITModel
    {
        public string Id { get; set; } 
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string? Description { get; set; }

        public List<Product> Products { get; set; } = new();
    }
    public class CheckoutSession: ITModel 
    {
        [Key]
        [MaxLength(100)]
        public string Id { get; set; }   // المفتاح الأساسي للجلسة داخل نظامك كـ نص

        [Required]
        [MaxLength(200)]
        public string ExternalId { get; set; } // معرف الجلسة في المزود (مثل cs_xxx في Stripe Checkout)

        [Required]
        [MaxLength(100)]
        public string ProviderName { get; set; } // اسم المزود (Stripe, PayPal)

        // FK للعميل الذي بدأ الجلسة
        public string  CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;

        [Required]
        [MaxLength(50)] // open, complete, expired
        public string Status { get; set; }

        [Required]
        [MaxLength(50)] // payment, subscription, setup
        public string Mode { get; set; }

        public decimal? AmountTotal { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        [Required]
        [MaxLength(500)]
        public string SuccessUrl { get; set; }

        [Required]
        [MaxLength(500)]
        public string CancelUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public DateTime? CompletedAt { get; set; }

       

        // يمكن إضافة Metadata
        // public string? Metadata { get; set; }
    }

    public class Product: ITModel
    {
        public string Id { get; set; } 

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string  ProviderId { get; set; }
        public ProviderPayment Provider { get; set; } = default!;

        public List<PricePlan> PricePlans { get; set; } = new();
    }

    public class PricePlan: ITModel
    {
        public string Id { get; set; } 
        [Required]
        [MaxLength(200)]
        public string ExternalId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public decimal Amount { get; set; }
        [Required]
        [MaxLength(10)]
        public string Currency { get; set; }
        [Required]
        [MaxLength(50)]
        public string Interval { get; set; }

        public int? IntervalCount { get; set; }
        [MaxLength(50)]
        public string? UsageType { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string  ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }

    public class Customer: ITModel
    {
        public string Id { get; set; } 
        [MaxLength(200)]
        public string ExternalId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        [MaxLength(254)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<PaymentMethod> PaymentMethods { get; set; } = new();
        public List<Subscription> Subscriptions { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();
        public List<Discount> Discounts { get; set; } = new();
        public ICollection<CheckoutSession> CheckoutSessions { get; set; } = new List<CheckoutSession>();

    }

    public class PaymentMethod: ITModel
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string ExternalId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        [MaxLength(50)]
        public string? CardBrand { get; set; }
        [MaxLength(4)]
        public string? Last4 { get; set; }
        public DateTime? Expiry { get; set; }
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string  CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
    }

    public class Payment: ITModel
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string ExternalId { get; set; }

        public string  CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;

        public decimal Amount { get; set; }
        [Required]
        [MaxLength(10)]
        public string Currency { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        [MaxLength(500)]
        public string? FailureReason { get; set; }

        public List<Refund> Refunds { get; set; } = new();
    }

    public class Refund: ITModel
    {
        public string Id { get; set; }
        [MaxLength(200)]
        public string ExternalId { get; set; }

        public string  PaymentId { get; set; }
        public Payment Payment { get; set; } = default!;

        public decimal Amount { get; set; }
        [Required]
        [MaxLength(10)]
        public string Currency { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
        [MaxLength(500)]
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }

    public class Invoice: ITModel
    {
        public string Id { get; set; } 
        [Required]
        [MaxLength(200)]
        public string ExternalId { get; set; }

        public string  CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; }
        public decimal TotalAmount { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? PaidAt { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(200)]
        public string? Number { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();
    }

    public class InvoiceItem: ITModel
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Total { get; set; }

        public string  InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = default!;
    }

    public class Subscription: ITModel
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string ExternalId { get; set; }

        public string  CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string PricePlanExternalId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<SubscriptionItem> Items { get; set; } = new();
    }

    public class SubscriptionItem: ITModel
    {
        public string Id { get; set; } 
        [Required]
        [MaxLength(200)]
        public string ExternalId { get; set; }

        public string  SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string PricePlanExternalId { get; set; }

        public int Quantity { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [SchedulerEnabled(true)]
    public class Discount: ITModel
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string CouponCode { get; set; }
        [Required]
        [MaxLength(200)]
        public string ExternalId { get; set; }

        public decimal? AmountOff { get; set; }
        public int? PercentOff { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int? MaxRedemptions { get; set; }
        public int TimesRedeemed { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string ? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }

    public class WebhookEvent: ITModel
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string EventId { get; set; }
        [Required]
        [MaxLength(100)]
        public string EventType { get; set; }
        [Required]
        public string Payload { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProviderName { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public bool Processed { get; set; } = false;
        public DateTime? ProcessedAt { get; set; }
        [MaxLength(500)]
        public string? ProcessingError { get; set; }
    }

    public class ActivityLog: ITModel
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; }
        [Required]
        [MaxLength(200)]
        public string EntityId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Action { get; set; }
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [MaxLength(200)]
        public string? UserId { get; set; }
        [MaxLength(200)]
        public string? AdditionalData { get; set; }
    }
}