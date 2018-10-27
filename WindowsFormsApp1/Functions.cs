using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


namespace WindowsFormsApp1
{
    class Functions
    {
        
        public static bool isDate(string date)
        {
            DateTime temp;            
            return DateTime.TryParse(date, out temp);
        }
        
        public static void Export(string foldername)
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
            
            int i = 0, j = 0;
            string data = null;
            Excel.Application xlApp;
            Excel.Workbook xlWorkbook;
            Excel.Worksheet xlWorksheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            xlWorkbook = xlApp.Workbooks.Add(misValue);
            xlWorksheet = (Excel.Worksheet)xlWorkbook.Worksheets.get_Item(1);

            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM people", dbConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            
            int rows = ds.Tables["people"].Rows.Count;
            int columns = ds.Tables["people"].Columns.Count;
            for (i = 0; i <= rows - 1; i++)
            {
                for (j = 0; j <= columns - 1; j++)
                {
                    data = ds.Tables["people"].Rows[i].ItemArray[j].ToString();
                    xlWorksheet.Cells[i + 1, j + 1] = data;
                }
            }

            string filename = foldername + "\\" + String.Format("ddMMyyyyHHmmss",DateTime.Now);
            xlWorkbook.SaveAs(filename + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkbook.Close(true, misValue, misValue);
            xlApp.Quit();

            releaseObject(xlWorksheet);
            releaseObject(xlWorkbook);
            releaseObject(xlApp);

        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        public static string timeConversion(string time12)
        {
            int h12 = Convert.ToInt32(time12.Substring(0, 2));
            string am_pm = time12.Substring(8, 2);
            string h24 = "00";
            if (am_pm == "AM" && h12 == 12)
            {
                h24 = "00";
            }
            if (am_pm == "PM" && h12 == 12)
            {
                h24 = "12";
            }
            if (am_pm == "AM" && h12 < 12)
            {
                h24 = "0" + h12.ToString();
            }
            if (am_pm == "PM" && h12 < 12)
            {
                h24 = (h12 + 12).ToString();
            }
            return h24 + time12.Substring(2, 6);
        }

        public static string Auto_Increment()
        {
            string ID = "";
            MySqlConnectionStringBuilder dbInfo = new MySqlConnectionStringBuilder();
            dbInfo.Server = Properties.Settings.Default.servername;
            dbInfo.UserID = Properties.Settings.Default.userid;
            dbInfo.Password = Properties.Settings.Default.password;
            dbInfo.Port = UInt32.Parse(Properties.Settings.Default.port);
            dbInfo.Database = Properties.Settings.Default.database;
            dbInfo.SslMode = MySqlSslMode.None;

            MySqlConnection dbConn = new MySqlConnection(dbInfo.ToString());
            MySqlCommand dbComm = new MySqlCommand("SELECT `AUTO_INCREMENT` FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'sys' AND TABLE_NAME = 'people'; ",dbConn);
            dbConn.Open();
            MySqlDataReader reader = dbComm.ExecuteReader();
            while (reader.Read())
            {
                ID = reader.GetString(0);
            }
            return ID;
        }

        public static bool IsValidEmail(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Return true if strIn is in valid email format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static void Register(string Name, string Lastname, string Gender, string Email, string DateOfBirth, string Alias)
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
            MySqlCommand dbComm = new MySqlCommand("INSERT INTO people(`Name`,`Lastname`,`Gender`,`Email`,`DateOfBirth`,`Alias`) VALUES(@Name, @Lastname, @Gender, @Email, @DateOfBirth, @Alias);", dbConn);
            dbComm.Parameters.AddWithValue("@Name", Name);
            dbComm.Parameters.AddWithValue("@Lastname", Lastname);
            dbComm.Parameters.AddWithValue("@Gender", Gender);
            dbComm.Parameters.AddWithValue("@Email", Email);
            dbComm.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            dbComm.Parameters.AddWithValue("@Alias", Alias);

            MySqlDataReader reader = dbComm.ExecuteReader();
        }

        public static void LoadData(DataGridView dataGridView)
        {
            MySqlConnectionStringBuilder dbInfo = new MySqlConnectionStringBuilder();
            dbInfo.Server = Properties.Settings.Default.servername;
            dbInfo.UserID = Properties.Settings.Default.userid;
            dbInfo.Password = Properties.Settings.Default.password;
            dbInfo.Port = uint.Parse(Properties.Settings.Default.port);
            dbInfo.Database = Properties.Settings.Default.database;
            dbInfo.SslMode = MySqlSslMode.None;

            MySqlConnection dbConn = new MySqlConnection(dbInfo.ToString());
            dbConn.Open();

            MySqlDataAdapter dataAdapter = new MySqlDataAdapter("SELECT * FROM `all_people`", dbConn);
            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            dataGridView.DataSource = table;
        }
    }
}
