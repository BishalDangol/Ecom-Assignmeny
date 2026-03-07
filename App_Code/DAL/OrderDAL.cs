using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.DAL
{
    /// <summary>
    /// Handles database operations for Orders and related entities.
    /// </summary>
    public class OrderDAL
    {
        /// <summary>
        /// Inserts a new order and returns the generated ID.
        /// </summary>
        public int InsertOrder(Order order, SqlTransaction transaction = null)
        {
            string query = @"INSERT INTO orders (member_id, order_code, order_date, status, payment_method_id, coupon_id, subtotal, discount_amount, delivery_charge, total_amount, payment_status, created_at) 
                            VALUES (@MemberId, @OrderCode, @OrderDate, @Status, @PaymentMethodId, @CouponId, @Subtotal, @DiscountAmount, @DeliveryCharge, @TotalAmount, @PaymentStatus, @CreatedAt);
                            SELECT SCOPE_IDENTITY();";

            SqlConnection conn = transaction?.Connection ?? DatabaseHelper.GetConnection();
            SqlCommand cmd = new SqlCommand(query, conn, transaction);
            
            cmd.Parameters.AddWithValue("@MemberId", order.MemberId);
            cmd.Parameters.AddWithValue("@OrderCode", order.OrderCode);
            cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            cmd.Parameters.AddWithValue("@Status", order.Status);
            cmd.Parameters.AddWithValue("@PaymentMethodId", order.PaymentMethodId);
            cmd.Parameters.AddWithValue("@CouponId", (object)order.CouponId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Subtotal", order.Subtotal);
            cmd.Parameters.AddWithValue("@DiscountAmount", order.DiscountAmount);
            cmd.Parameters.AddWithValue("@DeliveryCharge", order.DeliveryCharge);
            cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
            cmd.Parameters.AddWithValue("@PaymentStatus", order.PaymentStatus);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            if (transaction == null) DatabaseHelper.OpenConnection(conn);
            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            if (transaction == null) DatabaseHelper.CloseConnection(conn);
            
            return newId;
        }

        /// <summary>
        /// Inserts an order item.
        /// </summary>
        public void InsertOrderItem(OrderItem item, SqlTransaction transaction)
        {
            string query = @"INSERT INTO order_items (order_id, product_id, vendor_id, quantity, price, subtotal) 
                            VALUES (@OrderId, @ProductId, @VendorId, @Quantity, @Price, @Subtotal)";
            
            SqlCommand cmd = new SqlCommand(query, transaction.Connection, transaction);
            cmd.Parameters.AddWithValue("@OrderId", item.OrderId);
            cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
            cmd.Parameters.AddWithValue("@VendorId", item.VendorId);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Subtotal", item.Subtotal);
            
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserts an order address snapshot.
        /// </summary>
        public void InsertOrderAddress(OrderAddress address, SqlTransaction transaction)
        {
            string query = @"INSERT INTO order_addresses (order_id, recipient_name, phone, address_line_1, address_line_2, district_id, postal_code) 
                            VALUES (@OrderId, @RecipientName, @Phone, @AddressLine1, @AddressLine2, @DistrictId, @PostalCode)";
            
            SqlCommand cmd = new SqlCommand(query, transaction.Connection, transaction);
            cmd.Parameters.AddWithValue("@OrderId", address.OrderId);
            cmd.Parameters.AddWithValue("@RecipientName", address.RecipientName);
            cmd.Parameters.AddWithValue("@Phone", address.Phone);
            cmd.Parameters.AddWithValue("@AddressLine1", address.AddressLine1);
            cmd.Parameters.AddWithValue("@AddressLine2", address.AddressLine2);
            cmd.Parameters.AddWithValue("@DistrictId", address.DistrictId);
            cmd.Parameters.AddWithValue("@PostalCode", address.PostalCode);
            
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves orders for a specific member.
        /// </summary>
        public List<Order> GetByMember(int memberId)
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM orders WHERE member_id = @MemberId ORDER BY created_at DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MemberId", memberId);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(MapToEntity(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving member orders", ex);
                }
            }
            return orders;
        }

        private Order MapToEntity(SqlDataReader reader)
        {
            return new Order
            {
                OrderId = Convert.ToInt32(reader["order_id"]),
                MemberId = Convert.ToInt32(reader["member_id"]),
                OrderCode = reader["order_code"].ToString(),
                OrderDate = Convert.ToDateTime(reader["order_date"]),
                Status = reader["status"].ToString(),
                PaymentMethodId = Convert.ToInt32(reader["payment_method_id"]),
                CouponId = reader["coupon_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["coupon_id"]),
                Subtotal = Convert.ToDecimal(reader["subtotal"]),
                DiscountAmount = Convert.ToDecimal(reader["discount_amount"]),
                DeliveryCharge = Convert.ToDecimal(reader["delivery_charge"]),
                TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                PaymentStatus = reader["payment_status"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }
    }
}
