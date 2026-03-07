using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a customer order.
    /// </summary>
    public class Order
    {
        public int OrderId { get; set; }
        public int MemberId { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public int PaymentMethodId { get; set; }
        public int? CouponId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        public Order() { }

        public Order(int orderId, string orderCode, decimal totalAmount)
        {
            OrderId = orderId;
            OrderCode = orderCode;
            TotalAmount = totalAmount;
        }

        public override string ToString()
        {
            return $"Order: {OrderCode} (ID: {OrderId}, Total: NPR {TotalAmount})";
        }
    }
}
