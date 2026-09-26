using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaApp
{
    internal class DatabaseHelper
    {
        private OleDbConnection connection; //variable
        private string connectionString = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source =|DataDirectory|\agenda_DB.mdb";



        public DatabaseHelper()
        {
            connection = new OleDbConnection(connectionString);
        }

        //Listing the entire Agenda 
        public DataTable List()
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM Agenda";
            OleDbDataAdapter adapter = new OleDbDataAdapter(query, connectionString);
            adapter.Fill(dt);
            return dt;

        }


    }
}
