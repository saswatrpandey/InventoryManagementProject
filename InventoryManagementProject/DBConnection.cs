using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementProject
{
    public class DBConnection
    {
        public static SqlConnection DBConnect()
        {
            var connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\inventoryManagement.mdf;Integrated Security=True";
            SqlConnection con = new SqlConnection(connectionString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            return con;
        }
        public static DataTable GetTableByQuery(string sqlQuery)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = DBConnect();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void ExecutingNonQuery(string sqlQuery)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = DBConnect();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
