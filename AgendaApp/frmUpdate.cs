using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgendaApp
{
    public partial class frmUpdate : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public int id;
        DatabaseHelper dbHelper = new DatabaseHelper();
        public frmUpdate()
        {
            InitializeComponent();

        }


        private void frmUpdate_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            date.MinDate = DateTime.Now;
            DataTable dt = dbHelper.GetById(id);
            if (dt.Rows.Count > 0)
            {
                txtMessage.Text = dt.Rows[0]["Message"].ToString();
                date.Value = Convert.ToDateTime(dt.Rows[0]["message_date"]);
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMessage.Text))
            {
                dbHelper.UpdateNote(id, date.Value, txtMessage.Text);
                this.Hide();
            }
            else
            {
                MessageBox.Show("The message field is required!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
               
            }
        }
    }
}