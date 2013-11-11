using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

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

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            int rCnt;
            int lastrCnt = HitungBaris();

            object misValue = System.Reflection.Missing.Value;

            Excel.Application xlApp = new ApplicationClass();
            Workbook xlWorkBook = xlApp.Workbooks.Open(textBox1.Text, 0,
                true, 5, "", "", true, XlPlatform.xlWindows,
                "\t", false, false, 0, true, 1, 0);
            Worksheet xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            Range range = xlWorkSheet.UsedRange;

            _connection.Open();
            SqlTransaction transaction = _connection.BeginTransaction();
            for (rCnt = 2; rCnt <= lastrCnt; rCnt++) //Baris
            {
                //try
                //{
                //    _connect.MasukkanData("UPDATE [KASDA].[dbo].[ANGKAS_DTL] set tot_angkas = " +
                //        (range.Cells[rCnt, 2] as Range).Value2 + ", tot_sblm_pak = " +
                //        (range.Cells[rCnt, 1] as Range).Value2 + " where id_rinci_rs = '" +
                //        (range.Cells[rCnt, 4] as Range).Value2 + "'", _connection, transaction);
                //    _connect.MasukkanData("INSERT into [REALANGGAR].[dbo].[T_PAK] values (getdate(), " +
                //        (range.Cells[rCnt, 1] as Range).Value2 + ", " +
                //        (range.Cells[rCnt, 2] as Range).Value2 + ", " +
                //        (range.Cells[rCnt, 3] as Range).Value2 + ", " +
                //        (range.Cells[rCnt, 4] as Range).Value2 + ")", _connection, transaction);
                //}
                //catch (SqlException sqlEx)
                //{
                //    MessageBox.Show(@"Terjadi kesalahan dgn pesan : " + sqlEx.Message);
                //    transaction.Rollback();
                //    return;
                //}
                if (Convert.ToString(((Range) range.Cells[lastrCnt, 2]).Value2) == "")
                {
                    MessageBox.Show("Test " + ((Range) range.Cells[lastrCnt, 12]).Value2);
                }
            }
            transaction.Commit();
            _connection.Close();

            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button2.Enabled = true;
        } 
    }
}
