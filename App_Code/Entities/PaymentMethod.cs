using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a payment method (e.g., eSewa, Khalti, COD).
    /// </summary>
    public class PaymentMethod
    {
        public int PaymentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public decimal ProcessingFee { get; set; }
        public int DisplayOrder { get; set; }

        public PaymentMethod() { }

        public PaymentMethod(int paymentId, string name)
        {
            PaymentId = paymentId;
            Name = name;
        }

        public override string ToString()
        {
            return $"PaymentMethod: {Name}";
        }
    }
}
