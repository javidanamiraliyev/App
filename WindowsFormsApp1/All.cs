using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class All : Form
    {
        public static string showID;
        public static string deleteID;
        public All()
        {
            InitializeComponent();
            try
            {
                Functions.LoadData(this.dataGridView1);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not connect to database!");
            }
        }

        private void All_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns[0].Width = 50;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (radioName.Checked)
            {
                try
                {
                    MySqlConnectionStringBuilder dbInfo = new MySqlConnectionStringBuilder();
                    dbInfo.Server = Properties.Settings.Default.servername;
                    dbInfo.UserID = Properties.Settings.Default.userid;
                    dbInfo.Password = Properties.Settings.Default.password;
                    dbInfo.Port = UInt32.Parse(Properties.Settings.Default.port);
                    dbInfo.Database = Properties.Settings.Default.database;
                    dbInfo.SslMode = (MySqlSslMode)Properties.Settings.Default.sslmode;

                    MySqlConnection dbConn = new MySqlConnection(dbInfo.ToString());
                    dbConn.Open();
                    MySqlDataAdapter dataAdapter;
                    dataAdapter = new MySqlDataAdapter("SELECT * FROM `all_people` WHERE Firstname LIKE '" + textSearch.Text + "%'", dbConn);

                    DataTable table = new DataTable();
                    dataAdapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
                catch (Exception)
                {

                }
                
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            try
            {
                MySqlConnectionStringBuilder dbInfo = new MySqlConnectionStringBuilder();
                dbInfo.Server = Properties.Settings.Default.servername;
                dbInfo.UserID = Properties.Settings.Default.userid;
                dbInfo.Password = Properties.Settings.Default.password;
                dbInfo.Port = UInt32.Parse(Properties.Settings.Default.port);
                dbInfo.Database = Properties.Settings.Default.database;
                dbInfo.SslMode = MySqlSslMode.None;

                MySqlConnection dbConn = new MySqlConnection(dbInfo.ToString());
                dbConn.Open();
                MySqlDataAdapter dataAdapter;
                dataAdapter = new MySqlDataAdapter("SELECT * FROM `all_people` WHERE `Firstname` LIKE '" + textSearch.Text + "%' OR `Lastname` LIKE '" + textSearch.Text + "%' OR `Alias` LIKE '" + textSearch.Text + "%' OR `Date of Birth` LIKE '" + textSearch.Text + "%'", dbConn);

                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                dataGridView1.DataSource = table;
            }
            catch (Exception)
            {

            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                showID = dataGridView1[0, e.RowIndex].Value.ToString();
                UserInfo userInfo = new UserInfo();
                userInfo.ShowDialog();
                Functions.LoadData(this.dataGridView1);
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong!");
            }
            
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    MySqlConnectionStringBuilder dbInfo = new MySqlConnectionStringBuilder();
                    dbInfo.Server = Properties.Settings.Default.servername;
                    dbInfo.UserID = Properties.Settings.Default.userid;
                    dbInfo.Password = Properties.Settings.Default.password;
                    dbInfo.Port = UInt32.Parse(Properties.Settings.Default.port);
                    dbInfo.Database = Properties.Settings.Default.database;
                    dbInfo.SslMode = MySqlSslMode.None;

                    MySqlConnection dbConn = new MySqlConnection(dbInfo.ToString());
                    MySqlCommand dbComm = new MySqlCommand("UPDATE people SET `Status` = 'Deactive' WHERE ID = " + deleteID, dbConn);
                    dbConn.Open();
                    int a = dbComm.ExecuteNonQuery();
                    if (a != 0)
                    {
                        this.DialogResult = DialogResult.OK;
                        Functions.LoadData(dataGridView1);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong!");
                }
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            deleteID = dataGridView1[0, e.RowIndex].Value.ToString();
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            New reg = new New();
            reg.ShowDialog();
            Functions.LoadData(dataGridView1);
        }

        private void radioMore_CheckedChanged(object sender, EventArgs e)
        {
            if (radioMore.Checked) buttonSearch.Enabled = true;
            else buttonSearch.Enabled = false;
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Console.WriteLine(folderBrowserDialog1.SelectedPath);
                    Functions.Export(@folderBrowserDialog1.SelectedPath);
                    MessageBox.Show("Exported to " + folderBrowserDialog1.SelectedPath + String.Format("ddMMyyyyHHmmss", DateTime.Now)+".xls");
                }
                catch(Exception)
                {
                    MessageBox.Show("Something went wrong!", "Error");
                }
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Functions.LoadData(dataGridView1);
        }
    }
}
