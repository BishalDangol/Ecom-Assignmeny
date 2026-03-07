using System;
using System.Collections.Generic;
using Saja.DAL;
using Saja.Entities;

namespace Saja.BLL
{
    public class WishlistBLL
    {
        private WishlistDAL wishlistDAL;

        public WishlistBLL()
        {
            wishlistDAL = new WishlistDAL();
        }

        public List<Wishlist> GetUserWishlist(int memberId)
        {
            return wishlistDAL.GetByMember(memberId);
        }

        public OperationResult AddToWishlist(int memberId, int productId)
        {
            if (memberId <= 0 || productId <= 0)
                return OperationResult.Failed("User must be logged in.");

            if (wishlistDAL.Insert(memberId, productId))
                return OperationResult.Succeeded("Added to wishlist.");
            
            return OperationResult.Failed("Product is already in your wishlist.");
        }

        public OperationResult RemoveFromWishlist(int memberId, int productId)
        {
            if (wishlistDAL.Delete(memberId, productId))
                return OperationResult.Succeeded("Removed from wishlist.");
            
            return OperationResult.Failed("Could not remove item.");
        }
    }
}
