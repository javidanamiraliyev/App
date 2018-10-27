using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class New : Form
    {
        public New()
        {
            InitializeComponent();
            dateBirth.CustomFormat = "dd.MM.yyyy";
        }

        public static string filepath;
        private static string alias="";

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textFirstname.Text = "";
            textLastname.Text = "";
            dateBirth.Text = "";
            textAlias.Text = "";
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Upload picture";
            theDialog.Filter = "Image files|*.jpg;";
            theDialog.InitialDirectory = @"%USERPROFILE%\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                labelPicture.ForeColor = Color.Green;
                labelPicture.Visible = true;
                filepath = theDialog.FileName;
                
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            filepath = "";
            
            if (textFirstname.Text == "" || textFirstname.Text == " ")
            {
                return;
            }
            if (textLastname.Text == "" || textLastname.Text == " ")
            {
                return;
            }
            if (textAlias.Text == "" || textAlias.Text == " ")
            {
                return;
            }
            if (alias == "") return;

            if (filepath != "") File.Copy(filepath, AppDomain.CurrentDomain.BaseDirectory + "Resources\\" + textAlias.Text + Path.GetExtension(filepath));
            Functions.Register(textFirstname.Text, textLastname.Text, comboGender.Text, dateBirth.Text, textAlias.Text);
            DialogResult success = MessageBox.Show("You have been successfully registered!", "Success", MessageBoxButtons.OK);
            if (success == DialogResult.OK)
            {
                Close();
            }

        }

        private void textAlias_TextChanged(object sender, EventArgs e)
        {
            label10.Visible = false;
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            label12.Visible = false;
        }

        private void textAlias_TextChanged_1(object sender, EventArgs e)
        {
            label10.Visible = false;
            MySqlConnectionStringBuilder dbInfo = new MySqlConnectionStringBuilder();
            dbInfo.Server = Properties.Settings.Default.servername;
            dbInfo.UserID = Properties.Settings.Default.userid;
            dbInfo.Password = Properties.Settings.Default.password;
            dbInfo.Port = UInt32.Parse(Properties.Settings.Default.port);
            dbInfo.Database = Properties.Settings.Default.database;
            dbInfo.SslMode = (MySqlSslMode)Properties.Settings.Default.sslmode;

            MySqlConnection dbConn = new MySqlConnection(dbInfo.ToString());
            dbConn.Open();
            MySqlCommand dbComm;
            if (textAlias.Text != "")
            {
                dbComm = new MySqlCommand("select count(`Alias`) from `people` where `Alias`='" + textAlias.Text + "';", dbConn);
                MySqlDataReader reader = dbComm.ExecuteReader();
                while (reader.Read())
                {


                    if (Int32.Parse(reader[0].ToString()) > 0)
                    {
                        label10.Text = "Not available";
                        label10.ForeColor = Color.Red;
                        label10.Visible = true;
                        alias = "";
                    }
                    else
                    {
                        label10.Text = "Available";
                        label10.ForeColor = Color.Green;
                        label10.Visible = true;
                        alias = textAlias.Text;
                    }
                }
            }
        }
    }
}
