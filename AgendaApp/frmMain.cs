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
    public partial class frmMain : Form
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        DatabaseHelper dbHelper = new DatabaseHelper();

        private void List()
        {
            dgvMain.DataSource = dbHelper.List();
        }
        public frmMain()
        {
            InitializeComponent();

            ContextMenuStrip trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, (s, e) => { this.Show(); this.Activate(); });
            trayMenu.Items.Add("Exit", null, (s, e) => { notifyIcon1.Visible = false; Environment.Exit(0); });
            notifyIcon1.ContextMenuStrip = trayMenu;
            notifyIcon1.BalloonTipClicked += (s, e) => { this.Show(); this.Activate(); };
            List();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
        "Yes: Exit completely\nNo: Keep running in the tray",
        "Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                notifyIcon1.Visible = false;
                Environment.Exit(0);
            }
            else if (dialogResult == DialogResult.No)
            {
                this.Hide();
            }

        }

        private void frmMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvMain.SelectedRows.Count > 0)
            {
                frmUpdate frm = new frmUpdate();
                frm.id = int.Parse(dgvMain.SelectedRows[0].Cells["ID"].Value.ToString());
                frm.ShowDialog();
                List();
            }
            else
            {
                MessageBox.Show("Please select a note to update!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAdd frm = new frmAdd();
            frm.ShowDialog();
            List();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            dgvMain.Columns[0].HeaderText = "ID";

            dgvMain.Columns[1].HeaderText = "Message";

            dgvMain.Columns[2].HeaderText = "Date";

            dgvMain.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMain.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a message to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int clickedNoteId = int.Parse(dgvMain.SelectedRows[0].Cells["ID"].Value.ToString());


            DialogResult result = MessageBox.Show("Are you sure you want to delete it? ", "Confirmation", MessageBoxButtons.YesNo,
              MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                dbHelper.DeleteNote(clickedNoteId);
                List();
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime until = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            DataTable due = dbHelper.GetDueNotes(until);

            if (due.Rows.Count == 0)
            {
                return;
            }
            timer1.Stop();

            string text = "";

            foreach (DataRow row in due.Rows)
            {
                DateTime time = Convert.ToDateTime(row["message_date"]);
                text += time.ToString("HH:mm") + " - " + row["message"].ToString() + "\n";
            }
            if (this.Visible)
                MessageBox.Show(text + " \nThese notes have been deleted.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                notifyIcon1.ShowBalloonTip(5000, "Reminder", text, ToolTipIcon.Info);

            dbHelper.DeleteDueNotes(until);
            List();
            timer1.Start();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.Activate();
        }


    }
}

