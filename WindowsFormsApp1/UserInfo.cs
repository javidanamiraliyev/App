using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
                textFirstname.Text = reader["FirstName"].ToString();
                textLastname.Text = reader["Lastname"].ToString();
                comboGender.Text = reader["Gender"].ToString();
                textCompany.Text = reader["Company"].ToString();
                textPosition.Text = reader["Position"].ToString();
                textEmail.Text = reader["Email"].ToString();
                textAddress.Text = reader["Address"].ToString();
                textPhone.Text = reader["Phone"].ToString();
                dateBirth.Value = DateTime.ParseExact(reader["DateOfBirth"].ToString(), "dd.MM.yyyy",CultureInfo.InvariantCulture);
                textAlias.Text = reader["Alias"].ToString();
                if(File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent( AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString())+"\\Resources\\" + textAlias.Text + ".jpg")){
                    using(FileStream stream = File.Open(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString()) + "\\Resources\\" + textAlias.Text + ".jpg", FileMode.Open, FileAccess.Read))
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
                    if(File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString()) + "\\Resources\\" + textAlias.Text + ".jpg"))
                    File.Delete(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString()) + "\\Resources\\" + textAlias.Text + ".jpg");
                    File.Copy(theDialog.FileName, Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString()) + "\\Resources\\" + textAlias.Text + ".jpg");
                    pictureBox.Image = Image.FromFile(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString())+"\\Resources\\" + this.textAlias.Text + Path.GetExtension(theDialog.FileName));
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
            string newFirstname,newLastname, newGender, newCompany, newPosition, newEmail, newAddress, newPhone, newDateOfBirth;
            if (textFirstname.Text != "" && textLastname.Text!="")
            {
                newFirstname = textFirstname.Text;
                newLastname = textLastname.Text;
            }
            else
            {
                MessageBox.Show("Check the required fields!");
                return;
            }
            newDateOfBirth = dateBirth.Text;
            newEmail = textEmail.Text;
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
                    "SET Firstname='"+newFirstname+"', Lastname='"+newLastname+"', Gender='"+newGender+"',Company='"+newCompany+"',Position='"+newPosition+"', Email='"+newEmail+"',Address='"+newAddress+"',Phone='"+newPhone+"',DateOfBirth='"+newDateOfBirth+"' WHERE ID='"+textID.Text+"';" ,dbConn);
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
