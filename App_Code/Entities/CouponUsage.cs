using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents the record of a coupon being used in an order.
    /// </summary>
    public class CouponUsage
    {
        public int UsageId { get; set; }
        public int CouponId { get; set; }
        public int OrderId { get; set; }
        public int MemberId { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime UsedAt { get; set; }

        public CouponUsage() { }

        public override string ToString()
        {
            return $"CouponUsage: Coupon {CouponId}, Order {OrderId}, Discount: {DiscountAmount}";
        }
    }
}
