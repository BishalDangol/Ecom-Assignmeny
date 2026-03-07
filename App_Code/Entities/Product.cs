using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a product sold by a vendor.
    /// </summary>
    public class Product
    {
        public int ProductId { get; set; }
        public int VendorId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVisible { get; set; }
        public bool IsFeatured { get; set; }
        public bool MadeInNepalCertified { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Vendor Vendor { get; set; }
        public Category Category { get; set; }

        public Product() { }

        public Product(int productId, string name, decimal price, int stock)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            Stock = stock;
        }

        public override string ToString()
        {
            return $"Product: {Name} (ID: {ProductId}, Price: NPR {Price})";
        }
    }
}
