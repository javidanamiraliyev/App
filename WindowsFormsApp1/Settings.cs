using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            textServer.Text = Properties.Settings.Default.servername.ToString();
            textDB.Text = Properties.Settings.Default.database;
            textUsername.Text = Properties.Settings.Default.userid;
            textPort.Text = Properties.Settings.Default.port;
            textPassword.Text = Properties.Settings.Default.password;
            switch (Properties.Settings.Default.sslmode)
            {
                case 0: comboSSL.Text = "None";
                    break;
                case 1: comboSSL.Text = "Required";
                    break;
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            MySqlConnectionStringBuilder dbInfo = new MySqlConnectionStringBuilder();
            dbInfo.Server = textServer.Text;
            dbInfo.UserID = textUsername.Text;
            dbInfo.Password = textPassword.Text;
            dbInfo.Port = UInt32.Parse(textPort.Text);
            dbInfo.Database = textDB.Text;
            switch (comboSSL.Text)
            {
                case "None": dbInfo.SslMode = 0;
                    break;
                case "Required": dbInfo.SslMode = (MySqlSslMode)1;
                    break;
                default:dbInfo.SslMode = 0;
                    break;
            }
            
            using (MySqlConnection conn = new MySqlConnection(dbInfo.ToString()))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Test connection succeeded!", "Test connection", MessageBoxButtons.OK);
                    buttonSave.Enabled = true;
                }
                catch(MySqlException ex)
                {
                    MessageBox.Show("Test connection is unsuccessfull!", "Test connection", MessageBoxButtons.OK);
                }
            }
        }

        private void maskedTextBox1_Click(object sender, EventArgs e)
        {
            char[] ch = { '0', '1', '2', '3', '4', '5', '7', '8', '9' };
            this.textPort.Select(textPort.Text.LastIndexOfAny(ch) + 1, 0);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.servername = textServer.Text;
            Properties.Settings.Default.userid = textUsername.Text;
            Properties.Settings.Default.password = textPassword.Text;
            Properties.Settings.Default.port = textPort.Text;
            Properties.Settings.Default.database = textDB.Text;
            switch (comboSSL.Text)
            {
                case "None" : Properties.Settings.Default.sslmode = 0;
                    break;
                case "Required": Properties.Settings.Default.sslmode = 1;
                    break;
                default: Properties.Settings.Default.sslmode = 0;
                    break;
            }
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void textChanged(object sender, EventArgs e)
        {
            buttonSave.Enabled = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
