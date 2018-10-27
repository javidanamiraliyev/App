using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class UserInfo : Form
    {
        public UserInfo()
        {
            InitializeComponent();
            dateBirth.CustomFormat = "dd.MM.yyyy";
            MySqlConnectionStringBuilder dbInfo = new MySqlConnectionStringBuilder();
            dbInfo.Server = Properties.Settings.Default.servername;
            dbInfo.UserID = Properties.Settings.Default.userid;
            dbInfo.Password = Properties.Settings.Default.password;
            dbInfo.Port = UInt32.Parse(Properties.Settings.Default.port);
            dbInfo.Database = Properties.Settings.Default.database;
            dbInfo.SslMode = MySqlSslMode.None;

            MySqlConnection dbConn = new MySqlConnection(dbInfo.ToString());
            MySqlCommand dbComm = new MySqlCommand("SELECT * FROM people WHERE ID = '" +All.showID+ "';", dbConn);
            dbConn.Open();
            MySqlDataReader reader = dbComm.ExecuteReader();
            while (reader.Read())
            {
                textID.Text = reader["ID"].ToString();
                textName.Text = reader["Name"].ToString();
                textLastname.Text = reader["Lastname"].ToString();
                comboGender.Text = reader["Gender"].ToString();
                textCompany.Text = reader["Company"].ToString();
                textPosition.Text = reader["Position"].ToString();
                textEmail.Text = reader["email"].ToString();
                textAddress.Text = reader["address"].ToString();
                textPhone.Text = reader["Phone"].ToString();
                dateBirth.Text = reader["DateOfBirth"].ToString();
                textAlias.Text = reader["Alias"].ToString();
                if(File.Exists(@"C:\Users\javidana\source\repos\WindowsFormsApp1\WindowsFormsApp1\Resources\" + textAlias.Text + ".jpg")){
                    using(FileStream stream = new FileStream(@"C:\Users\javidana\source\repos\WindowsFormsApp1\WindowsFormsApp1\Resources\" + textAlias.Text + ".jpg", FileMode.Open, FileAccess.Read))
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }
                }
                else
                {
                    pictureBox.Image = Properties.Resources.DefaultPicture;
                }
                this.Text = textAlias.Text;
            }
        }

        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        
        private void buttonUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Upload picture";
            theDialog.Filter = "Image files|*.jpg;";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(theDialog.FileName);
                try
                {
                    if(File.Exists(@"C:\Users\javidana\source\repos\WindowsFormsApp1\WindowsFormsApp1\Resources\" + textAlias.Text + ".jpg"))
                    File.Delete(@"C:\Users\javidana\source\repos\WindowsFormsApp1\WindowsFormsApp1\Resources\" + textAlias.Text + ".jpg");
                    File.Copy(theDialog.FileName, @"C:\Users\javidana\source\repos\WindowsFormsApp1\WindowsFormsApp1\Resources\" + textAlias.Text + ".jpg");
                    pictureBox.Image = Image.FromFile(@"C:\Users\javidana\source\repos\WindowsFormsApp1\WindowsFormsApp1\Resources\" + this.textAlias.Text + Path.GetExtension(theDialog.FileName));
                    MessageBox.Show("Picture was uploaded successfully", "Success", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong!");
                }
            }

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string newName, newGender, newCompany, newPosition, newEmail, newAddress, newPhone, newDateOfBirth;
            if (textName.Text != "" &&textName.Text!=" " &&textAlias.Text!="" && textEmail.Text != "" && dateBirth.Text != "")
            {
                newName = textName.Text;
                newEmail = textEmail.Text;
                newDateOfBirth = dateBirth.Text;
            }
            else
            {
                MessageBox.Show("Check the required fields!");
                return;
            }
            newGender = comboGender.Text;
            newAddress = textAddress.Text;
            newPhone = textPhone.Text;
            newCompany = textCompany.Text;
            newPosition = textPosition.Text;
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
                MySqlCommand dbComm = new MySqlCommand("UPDATE people "+
                    "SET Name='"+newName+"', Gender='"+newGender+"',Company='"+newCompany+"',Position='"+newPosition+"', email='"+newEmail+"',address='"+newAddress+"',Phone='"+newPhone+"',DateOfBirth='"+newDateOfBirth+"' WHERE ID='"+textID.Text+"';" ,dbConn);
                int a = dbComm.ExecuteNonQuery();
                if (a != 0)
                {
                    MessageBox.Show("Successfully updated!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something went wrong!");
            }
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
                    MySqlCommand dbComm = new MySqlCommand("UPDATE people SET `Status` = 'Inactive' WHERE ID = " + textID.Text, dbConn);
                    dbConn.Open();
                    int a = dbComm.ExecuteNonQuery();
                    if (a != 0)
                    {
                        MessageBox.Show("Record deleted!");
                        this.DialogResult = DialogResult.OK;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong!");
                }
            }
        }

        private void Phone_Click(object sender, EventArgs e)
        {
            char[] ch = { '0', '1', '2', '3', '4', '5', '7', '8', '9', '+' };
            this.textPhone.Select(textPhone.Text.LastIndexOfAny(ch)+1,0);
        }
    }
}
