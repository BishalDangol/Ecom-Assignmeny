using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.DAL
{
    public class DistrictDAL
    {
        public List<District> GetAll()
        {
            List<District> districts = new List<District>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM districts WHERE is_active = 1 ORDER BY name";
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            districts.Add(new District
                            {
                                DistrictId = Convert.ToInt32(reader["district_id"]),
                                Name = reader["name"].ToString(),
                                DeliveryCharge = Convert.ToDecimal(reader["delivery_charge"]),
                                DeliveryDays = Convert.ToInt32(reader["delivery_days"])
                            });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving districts", ex);
                }
            }
            return districts;
        }

        public District GetById(int districtId)
        {
            District district = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM districts WHERE district_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", districtId);
                try
                {
                    DatabaseHelper.OpenConnection(conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            district = new District
                            {
                                DistrictId = Convert.ToInt32(reader["district_id"]),
                                Name = reader["name"].ToString(),
                                DeliveryCharge = Convert.ToDecimal(reader["delivery_charge"]),
                                DeliveryDays = Convert.ToInt32(reader["delivery_days"])
                            };
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error retrieving district by ID", ex);
                }
            }
            return district;
        }
    }
}
