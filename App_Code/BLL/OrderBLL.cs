using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Saja.DAL;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.BLL
{
    /// <summary>
    /// Implements complex business rules for Orders.
    /// </summary>
    public class OrderBLL
    {
        private OrderDAL orderDAL;
        private ProductDAL productDAL;
        private DistrictDAL districtDAL;
        private CouponBLL couponBLL;

        public OrderBLL()
        {
            orderDAL = new OrderDAL();
            productDAL = new ProductDAL();
            districtDAL = new DistrictDAL();
            couponBLL = new CouponBLL();
        }

        /// <summary>
        /// Processes a full checkout. Uses a transaction to ensure atomicity.
        /// </summary>
        public OperationResult CreateOrder(int memberId, List<OrderItem> items, int districtId, string address1, string recipient, string phone, int paymentMethodId, string couponCode = null)
        {
            if (items == null || items.Count == 0)
                return OperationResult.Failed("Cart is empty.");

            decimal subtotal = 0;
            foreach (var item in items)
            {
                Product p = productDAL.GetById(item.ProductId);
                if (p == null || p.Stock < item.Quantity)
                    return OperationResult.Failed($"Product '{p?.Name ?? "Unknown"}' is out of stock.");
                
                item.Price = p.Price;
                item.Subtotal = p.Price * item.Quantity;
                item.VendorId = p.VendorId;
                subtotal += item.Subtotal;
            }

            // Calculate Delivery Charge based on District
            District district = districtDAL.GetById(districtId);
            decimal deliveryCharge = district?.DeliveryCharge ?? 150; // Default if not found

            // Handle Coupon
            decimal discount = 0;
            Coupon coupon = null;
            if (!string.IsNullOrEmpty(couponCode))
            {
                var couponResult = couponBLL.ValidateAndApply(couponCode, subtotal, memberId);
                if (couponResult.Success)
                {
                    coupon = (Coupon)couponResult.Data;
                    discount = CalculateDiscount(subtotal, coupon);
                }
            }

            decimal total = subtotal - discount + deliveryCharge;

            // Database Transaction
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                DatabaseHelper.OpenConnection(conn);
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // 1. Create Order Master
                    Order order = new Order
                    {
                        MemberId = memberId,
                        OrderCode = "SAJA-" + DateTime.Now.Ticks.ToString().Substring(10),
                        OrderDate = DateTime.Now,
                        Status = "Pending",
                        PaymentMethodId = paymentMethodId,
                        CouponId = coupon?.CouponId,
                        Subtotal = subtotal,
                        DiscountAmount = discount,
                        DeliveryCharge = deliveryCharge,
                        TotalAmount = total,
                        PaymentStatus = "Unpaid"
                    };

                    int orderId = orderDAL.InsertOrder(order, trans);
                    
                    // 2. Create Order Items and Update Stock
                    foreach (var item in items)
                    {
                        item.OrderId = orderId;
                        orderDAL.InsertOrderItem(item, trans);
                        
                        // Deduct Stock
                        if (!productDAL.UpdateStock(item.ProductId, -item.Quantity))
                        {
                            throw new Exception("Failed to update stock for Product ID " + item.ProductId);
                        }
                    }

                    // 3. Create Order Address
                    OrderAddress oAddr = new OrderAddress
                    {
                        OrderId = orderId,
                        RecipientName = recipient,
                        Phone = phone,
                        AddressLine1 = address1,
                        DistrictId = districtId
                    };
                    orderDAL.InsertOrderAddress(oAddr, trans);

                    // 4. Update Coupon Usage if applied
                    if (coupon != null)
                    {
                        new CouponDAL().IncrementUsage(coupon.CouponId, trans);
                    }

                    trans.Commit();
                    return OperationResult.Succeeded("Order placed successfully. Order Code: " + order.OrderCode, orderId);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return OperationResult.Failed("Checkout failed: " + ex.Message);
                }
                finally
                {
                    DatabaseHelper.CloseConnection(conn);
                }
            }
        }

        private decimal CalculateDiscount(decimal subtotal, Coupon coupon)
        {
            if (coupon.DiscountType == "Percentage")
                return subtotal * (coupon.DiscountValue / 100);
            else
                return coupon.DiscountValue;
        }
    }
}
