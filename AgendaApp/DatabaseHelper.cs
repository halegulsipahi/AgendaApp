using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        //Getting the notes by ID
        public DataTable GetById(int id)
        {
            DataTable dt = new DataTable();
            using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM Agenda  WHERE ID=@id", connection))
            {
                cmd.Parameters.AddWithValue("id", id);
                connection.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

            }
            connection.Close();
            return dt;

        }

        //Adding a new note by ID
        public void AddNote(DateTime date, string message)
        {
            date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
            string query = "INSERT INTO Agenda ([message],[message_date]) VALUES (?, ?)";
            OleDbCommand cmd = new OleDbCommand(query, connection);

            cmd.Parameters.Add("@message", OleDbType.LongVarWChar).Value = message;
            cmd.Parameters.Add("@message_date", OleDbType.Date).Value = date;

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                System.Windows.Forms.MessageBox.Show("Added!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        //Updating Notes
        public void UpdateNote(int id, DateTime date, string message)
        {
            date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
            string query = "UPDATE Agenda SET message_date=@date, message=@message WHERE ID=@id ";
            OleDbCommand cmd = new OleDbCommand(query, connection);

            cmd.Parameters.Add("@message_date", OleDbType.Date).Value = date;
            cmd.Parameters.Add("@message", OleDbType.LongVarWChar).Value = message;
            cmd.Parameters.Add("@id", OleDbType.Integer).Value = id;

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                System.Windows.Forms.MessageBox.Show("Updated!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

        }

        //Deleting Notes by ID
        public void DeleteNote(int id)
        {

            string query = "DELETE * FROM Agenda WHERE ID=@id";
            OleDbCommand cmd = new OleDbCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            bool success = false;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                success = true;

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
            if (success)
            {
                System.Windows.Forms.MessageBox.Show("Deleted!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        //Listing all the dates in the table.
        public List<DateTime> GetAllNoteDates()
        {
            List<DateTime> list = new List<DateTime>();
            string query = "SELECT message_date FROM Agenda";

            using (OleDbCommand cmd = new OleDbCommand(query, connection))
            {
                connection.Open();

                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["message_date"] != DBNull.Value && DateTime.TryParse(reader["message_date"].ToString(), out DateTime dbDate))
                        {
                            list.Add(dbDate);
                        }
                    }

                }
                connection.Close();
            }
            return list;

            /* OR
             * public List<DateTime> GetAllNoteDates()
            {
                List<DateTime> dates = new List<DateTime>();
                DataTable dt = List();

                foreach (DataRow row in dt.Rows)
                {
                    dates.Add(Convert.ToDateTime(row["message_date"]));
                }

                return dates;
            } */
        }

        // Retrieving the message from a specific date.

        public string GetMessage(DateTime date)
        {
            string message = "";
            string query = "SELECT message FROM Agenda WHERE message_date=@message_date";
            using (OleDbCommand cmd = new OleDbCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@message_date", date);
                connection.Open();

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    message = result.ToString();
                }

            }
            connection.Close();
            return message;

        }

        // Getting the notes whose time has come.
        public DataTable GetDueNotes(DateTime until)
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM Agenda WHERE message_date<=? ORDER BY message_date";
            using (OleDbCommand cmd = new OleDbCommand(query, connection))
            {
                cmd.Parameters.Add("@until", OleDbType.Date).Value = until;
                try
                {
                    connection.Open();
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);
                }

                finally { connection.Close(); }
            }
            return dt;
        }

        //Deleting the notes whose time has come.
        public int DeleteDueNotes(DateTime until)
        {
            int deleted = 0;
            string query = "DELETE FROM Agenda WHERE message_date<=?";
            using (OleDbCommand cmd = new OleDbCommand(query, connection))
            {
                cmd.Parameters.Add("@until", OleDbType.Date).Value = until;
                try
                {
                    connection.Open();
                    deleted = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                }
                finally { connection.Close(); }
            }
            return deleted;
        }

    }
}
