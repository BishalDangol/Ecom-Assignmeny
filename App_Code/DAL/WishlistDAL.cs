using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.DAL
{
    public class WishlistDAL
    {
        public List<Wishlist> GetByMember(int memberId)
        {
            List<Wishlist> wishlist = new List<Wishlist>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT w.*, p.name as ProductName, p.price as ProductPrice, p.image_url as ProductImage 
                                FROM wishlists w 
                                JOIN products p ON w.product_id = p.product_id 
                                WHERE w.member_id = @MemberId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MemberId", memberId);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            wishlist.Add(new Wishlist
                            {
                                WishlistId = Convert.ToInt32(reader["wishlist_id"]),
                                MemberId = Convert.ToInt32(reader["member_id"]),
                                ProductId = Convert.ToInt32(reader["product_id"]),
                                AddedAt = Convert.ToDateTime(reader["added_at"]),
                                Product = new Product { 
                                    ProductId = Convert.ToInt32(reader["product_id"]),
                                    Name = reader["ProductName"].ToString(),
                                    Price = Convert.ToDecimal(reader["ProductPrice"]),
                                    ImageUrl = reader["ProductImage"].ToString()
                                }
                            });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving wishlist", ex);
                }
            }
            return wishlist;
        }

        public bool Insert(int memberId, int productId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "IF NOT EXISTS (SELECT 1 FROM wishlists WHERE member_id = @MId AND product_id = @PId) " +
                               "INSERT INTO wishlists (member_id, product_id, added_at) VALUES (@MId, @PId, @Date)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MId", memberId);
                cmd.Parameters.AddWithValue("@PId", productId);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error adding to wishlist", ex);
                }
            }
        }

        public bool Delete(int memberId, int productId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "DELETE FROM wishlists WHERE member_id = @MId AND product_id = @PId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MId", memberId);
                cmd.Parameters.AddWithValue("@PId", productId);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error removing from wishlist", ex);
                }
            }
        }
    }
}
