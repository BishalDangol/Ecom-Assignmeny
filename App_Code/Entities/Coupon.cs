using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a discount coupon.
    /// </summary>
    public class Coupon
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; } // Percentage or Fixed
        public decimal DiscountValue { get; set; }
        public decimal MinPurchase { get; set; }
        public int MaxUsage { get; set; }
        public int UsageCount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public Coupon() { }

        public Coupon(int couponId, string code, decimal discountValue)
        {
            CouponId = couponId;
            Code = code;
            DiscountValue = discountValue;
        }

        public override string ToString()
        {
            return $"Coupon: {Code} ({DiscountValue} {DiscountType})";
        }
    }
}
