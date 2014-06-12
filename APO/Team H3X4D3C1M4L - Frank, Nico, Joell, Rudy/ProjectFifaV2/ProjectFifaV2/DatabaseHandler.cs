using System;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using System.Data;

namespace ProjectFifaV2
{
    class DatabaseHandler
    {
        private SqlCeConnection con;

        public DatabaseHandler()
        {
            con = new SqlCeConnection(@"Data Source=.\DB.sdf");
        }

        public void TestConnection()
        {
            bool open = false;

            try
            {
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nPlease report this bug to S.Dijks@rocwb.nl");
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    open = true;
                }
                con.Close();
            }

            if (!open)
            {
                Application.Exit();
            }
        }

        public void OpenConnectionToDB()
        {
            con.Open();
        }

        public void CloseConnectionToDB()
        {
            con.Close();
        }

        public System.Data.DataTable FillDT(string query)
        {
            TestConnection();
            OpenConnectionToDB();

            SqlCeDataAdapter dataAdapter = new SqlCeDataAdapter(query, GetCon());
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            
            CloseConnectionToDB();

            return dt;
        }

        public SqlCeConnection GetCon()
        {
            return con;
        }
    }
}
