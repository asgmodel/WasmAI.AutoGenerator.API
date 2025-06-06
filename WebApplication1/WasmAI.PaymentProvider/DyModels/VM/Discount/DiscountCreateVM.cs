using AutoGenerator;
using AutoGenerator.Helper.Translation;
using WasmAI.PaymentProvider.Models;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.DyModels.VMs
{
    /// <summary>
    /// Discount  property for VM Create.
    /// </summary>
    public class DiscountCreateVM : ITVM
    {
        ///
        public String? CouponCode { get; set; }
        ///
        public String? ExternalId { get; set; }
        ///
        public Nullable<Decimal> AmountOff { get; set; }
        ///
        public Nullable<Int32> PercentOff { get; set; }
        ///
        public Nullable<DateTime> ExpiryDate { get; set; }
        ///
        public Nullable<Int32> MaxRedemptions { get; set; }
        ///
        public Int32 TimesRedeemed { get; set; }
        ///
        public Boolean IsActive { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public String? CustomerId { get; set; }
    // public CustomerCreateVM? Customer { get; set; }
    }
}