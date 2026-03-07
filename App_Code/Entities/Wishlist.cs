using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a member's wishlist item.
    /// </summary>
    public class Wishlist
    {
        public int WishlistId { get; set; }
        public int MemberId { get; set; }
        public int ProductId { get; set; }
        public DateTime AddedAt { get; set; }

        // Navigation property
        public Product Product { get; set; }

        public Wishlist() { }

        public Wishlist(int memberId, int productId)
        {
            MemberId = memberId;
            ProductId = productId;
            AddedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Wishlist Item: Member {MemberId}, Product {ProductId}";
        }
    }
}
