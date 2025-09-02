using Microsoft.Data.SqlClient; 
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
            for (int i = 0; i <columnNames.Length; i++)
            {
                 range  = worksheet.Cells[1, i + 1];
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
    }
}
