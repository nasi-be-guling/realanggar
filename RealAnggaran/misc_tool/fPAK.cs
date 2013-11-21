using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;

namespace RealAnggaran.misc_tool
{
    public partial class FPak : Form
    {
        readonly CKonek _connect = new CKonek();
        readonly SqlConnection _connection;

        public FPak()
        {
            _connection = _connect.KonekDb();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private int HitungBaris(bool isbaris = true)
        {
            Excel.Application xlApp = new ApplicationClass();
            Workbook xlWorkBook = xlApp.Workbooks.Open(textBox1.Text, 0,
                true, 5, "", "", true, XlPlatform.xlWindows,
                "\t", false, false, 0, true, 1, 0);
            Worksheet xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            Range last = xlWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
            Range range = xlWorkSheet.get_Range("A1", last);

            if (isbaris)
                return last.Row;
           
            return last.Column;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
            button2.Enabled = false;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Unable to release the Object " + ex);
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// Implementasi EEPLUS
        /// -----------------------------------------------------------------------------------
        /// | WARNING : PERHATIAN                                                             |
        /// | TARGET FRAMEWORK HARUS MENGGUNAKAN FULL FRAMEWORK DOT NET, BUKAN CLIENT PROFILE |
        /// | CONTOH .NET FRAMEWORK 3.5 BUKAN .NET FRAMEWORK 3.5 CLIENT PROFILE               |
        /// ---------------------- ------------------------------------------------------------
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            FileInfo excelFile = new FileInfo(textBox1.Text);
            ExcelPackage paket = new ExcelPackage(excelFile);
            ExcelWorkbook workBook = paket.Workbook;

            if (workBook != null)
            {
                if (workBook.Worksheets.Count > 0)
                {
                    ExcelWorksheet currentWorksheet = workBook.Worksheets.First();
                    for (int theRows = 1; theRows <= currentWorksheet.Dimension.End.Row; theRows++)
                    {
                        
                        object cellPPTK = currentWorksheet.Cells[theRows, 1].Value;
                        object cellKodePanggil = currentWorksheet.Cells[theRows, 2].Value;
                        object cellDigitTerakhir = currentWorksheet.Cells[theRows, 9].Value;
                        object celltest = currentWorksheet.Cells[theRows, 10].Value;

                        if (string.IsNullOrEmpty(NullToString(cellKodePanggil)) &&
                             (!string.IsNullOrEmpty(NullToString(cellDigitTerakhir))))
                        {
                            MessageBox.Show(@"KODE PANGGIL tidak dilengkapi pada baris ke - " + theRows);
                            e.Cancel = true;
                            return;
                        }

                        if (string.IsNullOrEmpty(NullToString(cellPPTK)) &&
                            (!string.IsNullOrEmpty(NullToString(cellKodePanggil)) &&
                             (!string.IsNullOrEmpty(NullToString(cellDigitTerakhir)))))
                        {
                            MessageBox.Show(@"PPTK tidak dilengkapi pada baris ke - " + theRows);
                            e.Cancel = true;
                            return;
                        }


                    }
                }
            }
        }

        private string NullToString(object value)
        {
            // Value.ToString() allows for Value being DBNull, but will also convert int, double, etc.
            return value == null ? "" : value.ToString();

            // If this is not what you want then this form may suit you better, handles 'Null' and DBNull otherwise tries a straight cast
            // which will throw if Value isn't actually a string object.
            //return Value == null || Value == DBNull.Value ? "" : (string)Value;
        }

        //private void updatedb()
        //{
            
        //}

        //private void 

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button2.Enabled = true;
        }
        /// <summary>
        /// created         : nov-08-2013
        /// creator         : Putu
        /// name            : cekIfKeyNotFound
        /// description     : fungsi untuk mencek apabila value yg dimasukan ada atau tidak pada database
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="keystring"></param>
        /// <returns>jika nilai bool true maka update</returns>
        private bool CekIfKeyNotFound(SqlConnection sqlConnection, SqlTransaction transaction, string keystring)
        {
            SqlDataReader reader =
                _connect.MembacaData(
                    "SELECT     a.Id_Rinci_RS, REALANGGAR.dbo.A_REKENING.Id_Reken, REALANGGAR.dbo.A_REKENING.Kd_Reken " +
                    "FROM         KASDA..AKD_RINCIAN a INNER JOIN " +
                    "REALANGGAR.dbo.A_REKENING ON a.Id_Rinci_RS = REALANGGAR.dbo.A_REKENING.Kd_Reken " +
                    "WHERE     (REPLACE(REPLACE(REPLACE(a.Id_Rinci_RS, ' ', ''), CHAR(10), ''), CHAR(13), '') = '" + keystring + "') ", sqlConnection, transaction);

            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }

        private void FPak_Load(object sender, EventArgs e)
        {
            //foreach (Process proc in Process.GetProcessesByName("EXCEL"))
            //{
            //    if (proc.MainWindowTitle == "")
            //        proc.Kill();
            //}
        }
    }
}
