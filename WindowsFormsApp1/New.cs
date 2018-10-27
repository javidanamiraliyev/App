using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            textID.Text = Functions.Auto_Increment();
            dateBirth.CustomFormat = "dd.MM.yyyy";
        }

        public static string filepath;
        bool canRegister;

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textName.Text = "";
            textEmail.Text = "";
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
                try
                {
                    labelPicture.ForeColor = Color.Green;
                    labelPicture.Visible = true;
                    filepath = theDialog.FileName;
                    labelPicture.Text = Path.GetFileName(filepath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            filepath = "";
            MySqlConnectionStringBuilder dbInfo = new MySqlConnectionStringBuilder();
            dbInfo.Server = Properties.Settings.Default.servername;
            dbInfo.UserID = Properties.Settings.Default.userid;
            dbInfo.Password = Properties.Settings.Default.password;
            dbInfo.Port = UInt32.Parse(Properties.Settings.Default.port);
            dbInfo.Database = Properties.Settings.Default.database;
            dbInfo.SslMode = MySqlSslMode.None;

            MySqlConnection dbConn = new MySqlConnection(dbInfo.ToString());
            dbConn.Open();
            MySqlCommand dbComm;
            if (textAlias.Text != "")
            {
                dbComm = new MySqlCommand("SELECT Alias FROM people WHERE Alias = '" + textAlias.Text + "';", dbConn);
                MySqlDataReader reader = dbComm.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        label10.Visible = true;
                        return;
                    }
                }
            }
            if (textName.Text == "" || textName.Text == " ")
            {
                return;
            }
            if (textLastname.Text == "" || textLastname.Text == " ")
            {
                return;
            }
            if (!Functions.IsValidEmail(textEmail.Text))
            {
                label13.Visible = true;
                return;
            }
            if (!Functions.isDate(dateBirth.Text))
            {
                label6.Visible = true;
                return;
            }
            if (textAlias.Text == "" || textAlias.Text == " ")
            {
                return;
            }
            Console.WriteLine(dateBirth.Text);

            if (filepath != "") File.Copy(filepath, @"C:\Users\javidana\source\repos\WindowsFormsApp1\WindowsFormsApp1\Resources\" + textAlias.Text + Path.GetExtension(filepath));
            Functions.Register(textName.Text, textLastname.Text, comboGender.Text, textEmail.Text, dateBirth.Text, textAlias.Text);
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

        private void textEmail_TextChanged(object sender, EventArgs e)
        {
            label13.Visible = false;
        }
    }
}
