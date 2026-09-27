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
    public partial class frmAdd : Form
    {
        DatabaseHelper dbHelper=new DatabaseHelper();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public frmAdd()
        {
            InitializeComponent();
            date.MinDate = DateTime.Now;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtMessage.Text != "")
            {
               dbHelper.AddNote(date.Value,txtMessage.Text);
                this.Hide();
            }
            else
            {
                MessageBox.Show("Please fill in the message field","Warning",   MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
        }

        private void frmAdd_MouseDown(object sender, MouseEventArgs e)
        {


            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

     
    }
}

