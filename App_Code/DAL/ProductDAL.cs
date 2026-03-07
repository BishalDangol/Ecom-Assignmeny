using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.DAL
{
    /// <summary>
    /// Handles database operations for Products.
    /// </summary>
    public class ProductDAL
    {
        /// <summary>
        /// Retrieves all visible products.
        /// </summary>
        public List<Product> GetAll()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM products WHERE is_visible = 1 ORDER BY created_at DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(MapToEntity(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Log error (logging utility could be added)
                    throw new Exception("Error retrieving products from database", ex);
                }
                finally
                {
                    DatabaseHelper.CloseConnection(conn);
                }
            }
            return products;
        }

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        public Product GetById(int productId)
        {
            Product product = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM products WHERE product_id = @ProductId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = MapToEntity(reader);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving product by ID", ex);
                }
            }
            return product;
        }

        /// <summary>
        /// Inserts a new product into the database.
        /// </summary>
        public int Insert(Product product)
        {
            int newId = 0;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO products (vendor_id, category_id, name, description, price, stock, image_url, is_visible, is_featured, made_in_nepal_certified, created_at) 
                                VALUES (@VendorId, @CategoryId, @Name, @Description, @Price, @Stock, @ImageUrl, @IsVisible, @IsFeatured, @MadeInNepalCertified, @CreatedAt);
                                SELECT SCOPE_IDENTITY();";
                
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@VendorId", product.VendorId);
                cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Description", product.Description);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@Stock", product.Stock);
                cmd.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
                cmd.Parameters.AddWithValue("@IsVisible", product.IsVisible);
                cmd.Parameters.AddWithValue("@IsFeatured", product.IsFeatured);
                cmd.Parameters.AddWithValue("@MadeInNepalCertified", product.MadeInNepalCertified);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    newId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error inserting product", ex);
                }
            }
            return newId;
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        public bool Update(Product product)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"UPDATE products SET vendor_id = @VendorId, category_id = @CategoryId, name = @Name, 
                                description = @Description, price = @Price, stock = @Stock, image_url = @ImageUrl, 
                                is_visible = @IsVisible, is_featured = @IsFeatured, made_in_nepal_certified = @MadeInNepalCertified 
                                WHERE product_id = @ProductId";
                
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@VendorId", product.VendorId);
                cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Description", product.Description);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@Stock", product.Stock);
                cmd.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
                cmd.Parameters.AddWithValue("@IsVisible", product.IsVisible);
                cmd.Parameters.AddWithValue("@IsFeatured", product.IsFeatured);
                cmd.Parameters.AddWithValue("@MadeInNepalCertified", product.MadeInNepalCertified);
                cmd.Parameters.AddWithValue("@ProductId", product.ProductId);

                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error updating product", ex);
                }
            }
        }

        /// <summary>
        /// Updates the stock quantity of a product.
        /// </summary>
        public bool UpdateStock(int productId, int quantityChange)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "UPDATE products SET stock = stock + @Change WHERE product_id = @ProductId AND (stock + @Change) >= 0";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Change", quantityChange);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error updating product stock", ex);
                }
            }
        }
        
        /// <summary>
        /// Searches products by keyword, category, and price range.
        /// </summary>
        public List<Product> Search(string keyword, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM products WHERE is_visible = 1";
                if (!string.IsNullOrEmpty(keyword)) query += " AND (name LIKE @Keyword OR description LIKE @Keyword)";
                if (categoryId.HasValue) query += " AND category_id = @CategoryId";
                if (minPrice.HasValue) query += " AND price >= @MinPrice";
                if (maxPrice.HasValue) query += " AND price <= @MaxPrice";
                
                SqlCommand cmd = new SqlCommand(query, conn);
                if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                if (categoryId.HasValue) cmd.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                if (minPrice.HasValue) cmd.Parameters.AddWithValue("@MinPrice", minPrice.Value);
                if (maxPrice.HasValue) cmd.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);

                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(MapToEntity(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error searching products", ex);
                }
            }
            return products;
        }

        /// <summary>
        /// Helper method to map a SqlDataReader row to a Product entity.
        /// </summary>
        private Product MapToEntity(SqlDataReader reader)
        {
            return new Product
            {
                ProductId = Convert.ToInt32(reader["product_id"]),
                VendorId = Convert.ToInt32(reader["vendor_id"]),
                CategoryId = Convert.ToInt32(reader["category_id"]),
                Name = reader["name"].ToString(),
                Description = reader["description"].ToString(),
                Price = Convert.ToDecimal(reader["price"]),
                Stock = Convert.ToInt32(reader["stock"]),
                ImageUrl = reader["image_url"].ToString(),
                IsVisible = Convert.ToBoolean(reader["is_visible"]),
                IsFeatured = Convert.ToBoolean(reader["is_featured"]),
                MadeInNepalCertified = Convert.ToBoolean(reader["made_in_nepal_certified"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }
    }
}
