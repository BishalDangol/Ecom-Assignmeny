using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.DAL
{
    /// <summary>
    /// Handles database operations for Categories.
    /// </summary>
    public class CategoryDAL
    {
        public List<Category> GetAll()
        {
            List<Category> categories = new List<Category>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM categories WHERE is_visible = 1 ORDER BY display_order";
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryId = Convert.ToInt32(reader["category_id"]),
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                ImageUrl = reader["image_url"].ToString(),
                                IsVisible = Convert.ToBoolean(reader["is_visible"]),
                                DisplayOrder = Convert.ToInt32(reader["display_order"]),
                                CreatedAt = Convert.ToDateTime(reader["created_at"])
                            });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving categories", ex);
                }
            }
            return categories;
        }

        public Category GetById(int categoryId)
        {
            Category category = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM categories WHERE category_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", categoryId);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            category = new Category
                            {
                                CategoryId = Convert.ToInt32(reader["category_id"]),
                                Name = reader["name"].ToString()
                            };
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving category by ID", ex);
                }
            }
            return category;
        }
    }
}
