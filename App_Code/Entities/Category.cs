using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a product category.
    /// </summary>
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVisible { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        public Category() { }

        public Category(int categoryId, string name)
        {
            CategoryId = categoryId;
            Name = name;
        }

        public override string ToString()
        {
            return $"Category: {Name}";
        }
    }
}
