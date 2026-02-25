using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelVTIntegrationProject
{
    public partial class Form1 : Form
    {
        SqlConnection sqlConnection = new SqlConnection(
            @"Data Source=DESKTOP-H6VMCSI\SQLEXPRESS;Initial Catalog=ProjectsVT;Integrated Security=True;Trust Server Certificate=True");

        public Form1()
        {
            InitializeComponent();
        }

        private void btnVTdenOku_Click(object sender, EventArgs e)
        {
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = true;

            Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            string[] columnNames = { "PersonalsNo", "Ad", "Soyad", "Rayon", "Şəhər" };
            Excel.Range range;
            for (int i = 0; i < columnNames.Length; i++)
            {
                range = worksheet.Cells[1, i + 1];
                range.Value2 = columnNames[i];
            }
            try
            {
                sqlConnection.Open();
                string sqlQuery = "Select  PersonalsNo, Ad, Soyad, Rayon, Şəhər from Personal";
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                int row = 2; // Start from the second row, as the first row is for column names
                while (sqlDataReader.Read())
                {
                    string personalsNo = sqlDataReader[0].ToString();
                    string ad = sqlDataReader[1].ToString();
                    string soyad = sqlDataReader[2].ToString();
                    string rayon = sqlDataReader[3].ToString();
                    string seher = sqlDataReader[4].ToString();
                    richTextBox1.Text = richTextBox1.Text + personalsNo + " " + ad + " " + soyad + " " + rayon + " " + seher + "\n";

                    range = worksheet.Cells[row, 1];
                    range.Value2 = personalsNo;
                    range = worksheet.Cells[row, 2];
                    range.Value2 = ad;
                    range = worksheet.Cells[row, 3];
                    range.Value2 = soyad;
                    range = worksheet.Cells[row, 4];
                    range.Value2 = rayon;
                    range = worksheet.Cells[row, 5];
                    range.Value2 = seher;
                    row++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sql Qury sırasında problem oldu, Xəta Kodu:SQLREAD01 \n" + ex.ToString());
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Excel.Application excelApp;
            Excel.Workbook excelworkbook;
            Excel.Worksheet excelworksheet;
            Excel.Range range;
            int rowCount = 0;
            int colmnCount = 0;
            excelApp = new Excel.Application();
            excelworkbook = excelApp.Workbooks.Open("D:\\Csharp-Sqlserver-Projects\\ExcelVTIntegrationProject\\test.xlsx");
            excelworksheet = excelworkbook.Worksheets.get_Item(1);
            range = excelworksheet.UsedRange;
            richTextBox2.Clear();

            for (rowCount = 2; rowCount <= range.Rows.Count; rowCount++)
            {
                ArrayList arrayList = new ArrayList();
                for (colmnCount = 1; colmnCount <= range.Columns.Count; colmnCount++)
                {
                    string readCell = Convert.ToString((range.Cells[rowCount, colmnCount] as Excel.Range).Value2);
                    richTextBox2.Text = richTextBox2.Text + readCell + "  ";
                    arrayList.Add(readCell);
                }
                richTextBox2.Text = richTextBox2.Text + "\n";

                try
                {
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand(
     "Insert into Personal (PersonalsNo, Ad, Soyad, Rayon, Şəhər) values (@PersonalsNo, @Ad, @Soyad, @Rayon, @Şəhər)", sqlConnection);

                    sqlCommand.Parameters.AddWithValue("@PersonalsNo", arrayList[0]); // Excel-in birinci sütunu
                    sqlCommand.Parameters.AddWithValue("@Ad", arrayList[1]);
                    sqlCommand.Parameters.AddWithValue("@Soyad", arrayList[2]);
                    sqlCommand.Parameters.AddWithValue("@Rayon", arrayList[3]);
                    sqlCommand.Parameters.AddWithValue("@Şəhər", arrayList[4]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sql Qury sırasında problem oldu, Xəta Kodu:SQLREAD02 \n" + ex.ToString());
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
            excelApp.Quit();
            ReleaseObject(excelworksheet);
            ReleaseObject(excelworkbook);
            ReleaseObject(excelApp);
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
