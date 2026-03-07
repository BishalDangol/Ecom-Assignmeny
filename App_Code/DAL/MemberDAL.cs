using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.DAL
{
    /// <summary>
    /// Handles database operations for Members (Customers).
    /// </summary>
    public class MemberDAL
    {
        public Member GetByUsername(string username)
        {
            Member member = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM members WHERE username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            member = MapToEntity(reader);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving member by username", ex);
                }
            }
            return member;
        }

        public Member GetById(int memberId)
        {
            Member member = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM members WHERE member_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", memberId);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            member = MapToEntity(reader);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving member by ID", ex);
                }
            }
            return member;
        }

        public int Insert(Member member)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO members (username, email, password, full_name, phone, created_at) 
                                VALUES (@Username, @Email, @Password, @FullName, @Phone, @CreatedAt);
                                SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", member.Username);
                cmd.Parameters.AddWithValue("@Email", member.Email);
                cmd.Parameters.AddWithValue("@Password", member.Password);
                cmd.Parameters.AddWithValue("@FullName", member.FullName);
                cmd.Parameters.AddWithValue("@Phone", member.Phone);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error registering member", ex);
                }
            }
        }

        public bool UpdateToken(int memberId, string token, DateTime? expires)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "UPDATE members SET persistent_token = @Token, token_expires = @Expires WHERE member_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Token", (object)token ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Expires", (object)expires ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", memberId);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error updating member token", ex);
                }
            }
        }

        private Member MapToEntity(SqlDataReader reader)
        {
            return new Member
            {
                MemberId = Convert.ToInt32(reader["member_id"]),
                Username = reader["username"].ToString(),
                Email = reader["email"].ToString(),
                Password = reader["password"].ToString(),
                FullName = reader["full_name"].ToString(),
                Phone = reader["phone"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                PersistentToken = reader["persistent_token"].ToString(),
                TokenExpires = reader["token_expires"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["token_expires"])
            };
        }
    }
}
