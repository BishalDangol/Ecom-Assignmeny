using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.DAL
{
    public class VendorDAL
    {
        public Vendor GetById(int vendorId)
        {
            Vendor vendor = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM vendors WHERE vendor_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", vendorId);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vendor = new Vendor
                            {
                                VendorId = Convert.ToInt32(reader["vendor_id"]),
                                BusinessName = reader["business_name"].ToString(),
                                Verified = Convert.ToBoolean(reader["verified"]),
                                CommissionRate = Convert.ToDecimal(reader["commission_rate"])
                            };
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving vendor", ex);
                }
            }
            return vendor;
        }

        public List<Vendor> GetAll()
        {
            List<Vendor> vendors = new List<Vendor>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM vendors ORDER BY business_name";
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vendors.Add(new Vendor
                            {
                                VendorId = Convert.ToInt32(reader["vendor_id"]),
                                BusinessName = reader["business_name"].ToString(),
                                Verified = Convert.ToBoolean(reader["verified"])
                            });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving all vendors", ex);
                }
            }
            return vendors;
        }
    }
}
