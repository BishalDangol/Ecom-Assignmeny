using System;
using Saja.DAL;
using Saja.Entities;

namespace Saja.BLL
{
    public class CouponBLL
    {
        private CouponDAL couponDAL;

        public CouponBLL()
        {
            couponDAL = new CouponDAL();
        }

        /// <summary>
        /// Validates if a coupon can be applied to the current purchase.
        /// </summary>
        public OperationResult ValidateAndApply(string code, decimal currentSubtotal, int memberId)
        {
            if (string.IsNullOrWhiteSpace(code))
                return OperationResult.Failed("Coupon code is required.");

            Coupon coupon = couponDAL.GetByCode(code.Trim());

            if (coupon == null)
                return OperationResult.Failed("Invalid coupon code.");

            if (!coupon.IsActive)
                return OperationResult.Failed("This coupon is no longer active.");

            DateTime now = DateTime.Now;
            if (now < coupon.ValidFrom || now > coupon.ValidUntil)
                return OperationResult.Failed("This coupon has expired.");

            if (coupon.UsageCount >= coupon.MaxUsage)
                return OperationResult.Failed("This coupon has reached its maximum usage limit.");

            if (currentSubtotal < coupon.MinPurchase)
                return OperationResult.Failed($"A minimum purchase of NPR {coupon.MinPurchase} is required to use this coupon.");

            // Success
            return OperationResult.Succeeded("Coupon applied!", coupon);
        }
    }
}
