using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hotel_accommodation_System
{
    class Connection
    {
        private SqlConnection con;
        private SqlCommand cmd;
        private SqlDataAdapter da;


        public static readonly string ConnectionString =
    @"Data Source=YOURDATASOURCE;Initial Catalog=HotelAccomodationSystem;Integrated Security=True;TrustServerCertificate=True";


        public Connection() // Default Constructor
        {
            con = new SqlConnection(ConnectionString);
        }
        public void openConnection()
        {
            con.Open();
        }
        public void closeConnection()
        {
            con.Close();
        }

        public DataTable getData(string a)
        {
            openConnection();
            da = new SqlDataAdapter(a, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            closeConnection();
            return dt;
        }
        public void executeQuery(string query)
        {
            try
            {
                openConnection();
                cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                closeConnection();
            }
        }
    }
}
