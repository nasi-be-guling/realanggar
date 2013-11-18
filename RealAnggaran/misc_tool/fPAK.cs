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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // ====================================================
            // | Commplete code snipet ada di berkas fBacaForm.cs |
            // ====================================================

            //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            //int rCnt;
            //int lastrCnt = HitungBaris();

            //object misValue = System.Reflection.Missing.Value;

            //Excel.Application xlApp = new ApplicationClass();
            //Workbook xlWorkBook = xlApp.Workbooks.Open(textBox1.Text, 0,
            //    true, 5, "", "", true, XlPlatform.xlWindows,
            //    "\t", false, false, 0, true, 1, 0);
            ////Worksheet xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //Sheets sheets = xlWorkBook.Worksheets;

            //Worksheet worksheet = (Worksheet) sheets.get_Item(1);

            //Range range = worksheet.UsedRange;

            //_connection.Open();
            //SqlTransaction transaction = _connection.BeginTransaction();
            //for (rCnt = 2; rCnt <= lastrCnt; rCnt++) //Baris
            //{
            //    var value2 = ((Range) range.Cells[rCnt, 9]).Value2; // digit terakhir kode rekening = menandakan bahwa ini rekening yg dipakai
            //    var value3 = ((Range) range.Cells[rCnt, 2]).Value2; // kode panggil
            //    var value4 = ((Range) range.Cells[rCnt, 10]).Value2; // uraian
            //    var vPPTK = ((Range) range.Cells[rCnt, 1]).Value2; // pptk

            //    if (value2 != null && value3 != null && vPPTK == null) // cek apabila ada pptk yg tidak diisi
            //    {
            //        MessageBox.Show(@"PPTK pada baris ke : " + rCnt + @" KOSONG");
            //        xlWorkBook.Close(true, misValue, misValue);
            //        xlApp.Quit();

            //        releaseObject(worksheet);
            //        releaseObject(xlWorkBook);
            //        releaseObject(xlApp);

            //        xlApp = null;

            //        return;
            //    }

            //    if (value2 != null && value3 == null && vPPTK != null) // cek apabila ada kode panggil yg tidak diisi
            //    {
            //        MessageBox.Show(@"KODE PANGGIL pada baris ke : " + rCnt + @" KOSONG");
            //        xlWorkBook.Close(true, misValue, misValue);
            //        xlApp.Quit();

            //        releaseObject(worksheet);
            //        releaseObject(xlWorkBook);
            //        releaseObject(xlApp);


            //        xlApp = null;
            //        return;
            //    }
            //    //try
            //    //{
            //    //    _connect.MasukkanData("UPDATE [KASDA].[dbo].[ANGKAS_DTL] set tot_angkas = " +
            //    //        (range.Cells[rCnt, 2] as Range).Value2 + ", tot_sblm_pak = " +
            //    //        (range.Cells[rCnt, 1] as Range).Value2 + " where id_rinci_rs = '" +
            //    //        (range.Cells[rCnt, 4] as Range).Value2 + "'", _connection, transaction);
            //    //    _connect.MasukkanData("INSERT into [REALANGGAR].[dbo].[T_PAK] values (getdate(), " +
            //    //        (range.Cells[rCnt, 1] as Range).Value2 + ", " +
            //    //        (range.Cells[rCnt, 2] as Range).Value2 + ", " +
            //    //        (range.Cells[rCnt, 3] as Range).Value2 + ", " +
            //    //        (range.Cells[rCnt, 4] as Range).Value2 + ")", _connection, transaction);
            //    //}
            //    //catch (SqlException sqlEx)
            //    //{
            //    //    MessageBox.Show(@"Terjadi kesalahan dgn pesan : " + sqlEx.Message);
            //    //    transaction.Rollback();
            //    //    return;
            //    //}
            //    //if (value2 != null && value3 != null &&
            //    //    !CekIfKeyNotFound(_connection, transaction, value3.ToString().Trim()))
            //    //{
            //    //    MessageBox.Show("Test : " + value3);
            //    //}
            //    //else
            //    //{
            //    //    if (value4.ToString().Contains(@"*"))
            //    //        MessageBox.Show("subsidi : " + rCnt);
            //    //}
            //}
            //transaction.Commit();
            //_connection.Close();
            //xlWorkBook.Close(true, misValue, misValue);
            //xlApp.Quit();

            //releaseObject(worksheet);
            //releaseObject(xlWorkBook);
            //releaseObject(xlApp);

            //Marshal.ReleaseComObject(range);
            //Marshal.ReleaseComObject(worksheet);
            //Marshal.ReleaseComObject(sheets);
            //Marshal.ReleaseComObject(xlWorkBook);
            //Marshal.ReleaseComObject(xlApp);
            
            //xlApp = null;
            
            //var fileName = string.Format("{0}\\fileNameHere", Directory.GetCurrentDirectory());
            //var connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", fileName);
  
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
                        object cellValue = currentWorksheet.Cells[theRows, 1].Value;

                        MessageBox.Show(cellValue.ToString());
                    }
                }
            }
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
