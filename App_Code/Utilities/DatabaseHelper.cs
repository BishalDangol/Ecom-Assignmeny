using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Saja.Utilities
{
    /// <summary>
    /// Helper class for managing database connections and common operations.
    /// </summary>
    public static class DatabaseHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        /// <summary>
        /// Gets a new SQL connection.
        /// </summary>
        /// <returns>SqlConnection object</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Opens the provided connection if it's closed.
        /// </summary>
        /// <param name="conn">The connection to open</param>
        public static void OpenConnection(SqlConnection conn)
        {
            if (conn != null && conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
        }

        /// <summary>
        /// Closes the provided connection if it's open.
        /// </summary>
        /// <param name="conn">The connection to close</param>
        public static void CloseConnection(SqlConnection conn)
        {
            if (conn != null && conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
        }
    }
}
