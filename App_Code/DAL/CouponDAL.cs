using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.DAL
{
    public class CouponDAL
    {
        public Coupon GetByCode(string code)
        {
            Coupon coupon = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM coupons WHERE code = @Code AND is_active = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Code", code);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            coupon = new Coupon
                            {
                                CouponId = Convert.ToInt32(reader["coupon_id"]),
                                Code = reader["code"].ToString(),
                                DiscountType = reader["discount_type"].ToString(),
                                DiscountValue = Convert.ToDecimal(reader["discount_value"]),
                                MinPurchase = Convert.ToDecimal(reader["min_purchase"]),
                                MaxUsage = Convert.ToInt32(reader["max_usage"]),
                                UsageCount = Convert.ToInt32(reader["usage_count"]),
                                ValidFrom = Convert.ToDateTime(reader["valid_from"]),
                                ValidUntil = Convert.ToDateTime(reader["valid_until"]),
                                IsActive = Convert.ToBoolean(reader["is_active"])
                            };
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving coupon", ex);
                }
            }
            return coupon;
        }

        public void IncrementUsage(int couponId, SqlTransaction transaction)
        {
            string query = "UPDATE coupons SET usage_count = usage_count + 1 WHERE coupon_id = @Id";
            SqlCommand cmd = new SqlCommand(query, transaction.Connection, transaction);
            cmd.Parameters.AddWithValue("@Id", couponId);
            cmd.ExecuteNonQuery();
        }
    }
}
