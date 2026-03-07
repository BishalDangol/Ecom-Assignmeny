using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents an item within an order.
    /// </summary>
    public class OrderItem
    {
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int VendorId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }

        public OrderItem() { }

        public OrderItem(int itemId, int productId, int quantity, decimal price)
        {
            ItemId = itemId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            Subtotal = quantity * price;
        }

        public override string ToString()
        {
            return $"OrderItem: ProductID {ProductId}, Qty: {Quantity}, Price: {Price}";
        }
    }
}
