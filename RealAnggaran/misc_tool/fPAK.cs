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
using ExcelLibrary.SpreadSheet;
using OfficeOpenXml;
using QiHe.CodeLib;
using ExcelLibrary.CompoundDocumentFormat;
using ExcelLibrary.BinaryFileFormat;
using ExcelLibrary.BinaryDrawingFormat;

namespace RealAnggaran.misc_tool
{
    public partial class FPak : Form
    {
        readonly CKonek _connect = new CKonek();
        CAlat _tools = new CAlat();
        readonly SqlConnection _connection;
        private string _query1;
        private string _query2;

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

        private void button2_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
            button2.Enabled = false;
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
            #region CONTOH IMPLEMENTASI EPPLUS DAN EXCELLIBRARY
            /* ======================================================================================================
             * | Implementasi baca excel dengan 2 metode                                                            |
             * |    #1 EEPLUS = hanya bisa membaca open office document *.xlsx                                      |
             * |    #2 ExcelLibrary = hanya bisa membaca open office document *.xls                                 |
             * |                                                                                                    |
             * | Metode untuk membuka file, menggunakan FileStream instead of FileInfo, karena menggunakan FileInfo |
             * | akan menghasilkan error apabila file yg akan dibaca masih dipergunakan oleh program lainnya        |
             * ======================================================================================================
             * */

            //FileInfo fileInfo = new FileInfo(textBox1.Text); // ==> metode lama, menghasilkan System.IO.IOException was unhandled by user code
            //     Message=The process cannot access the file 'D:\Book1.xlsx' because it is being used by another process.
            //FileStream logFileStream = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            //ExcelPackage paket = new ExcelPackage(logFileStream);
            //ExcelWorkbook workBook = paket.Workbook;

            //if (workBook != null)
            //{
            //    if (workBook.Worksheets.Count > 0)
            //    {
            //        ExcelWorksheet currentWorksheet = workBook.Worksheets.First();
            //        for (int theRows = 2; theRows <= currentWorksheet.Dimension.End.Row; theRows++)
            //        {

            //            object cellPPTK = currentWorksheet.Cells[theRows, 1].Value;
            //            object cellKodePanggil = currentWorksheet.Cells[theRows, 2].Value;
            //            object cellDigitTerakhir = currentWorksheet.Cells[theRows, 9].Value;
            //            object celltest = currentWorksheet.Cells[theRows, 10].Value;

            //            if (string.IsNullOrEmpty(NullToString(cellKodePanggil)) &&
            //                 (!string.IsNullOrEmpty(NullToString(cellDigitTerakhir))))
            //            {
            //                MessageBox.Show(@"KODE PANGGIL tidak dilengkapi pada baris ke - " + theRows);
            //                e.Cancel = true;
            //                _connection.Close();
            //                return;
            //            }

            //            if (string.IsNullOrEmpty(NullToString(cellPPTK)) &&
            //                (!string.IsNullOrEmpty(NullToString(cellKodePanggil)) &&
            //                 (!string.IsNullOrEmpty(NullToString(cellDigitTerakhir)))))
            //            {
            //                MessageBox.Show(@"PPTK tidak dilengkapi pada baris ke - " + theRows);
            //                e.Cancel = true;
            //                _connection.Close();
            //                return;
            //            }

            //            if (CekIfKeyNotFound(_connection, transaction, NullToString(cellKodePanggil)) &&
            //                !string.IsNullOrEmpty(NullToString(cellPPTK)) &&
            //                !string.IsNullOrEmpty(NullToString(cellKodePanggil)) &&
            //                !string.IsNullOrEmpty(NullToString(cellDigitTerakhir)))
            //            {
            //                MessageBox.Show("update pada baris : " + theRows);
            //            }
            //            else if (!CekIfKeyNotFound(_connection, transaction, NullToString(cellKodePanggil)) &&
            //                !string.IsNullOrEmpty(NullToString(cellPPTK)) &&
            //                !string.IsNullOrEmpty(NullToString(cellKodePanggil)) &&
            //                !string.IsNullOrEmpty(NullToString(cellDigitTerakhir)))
            //            {
            //                MessageBox.Show("insert pada baris : " + theRows);
            //            }
            //        }
            //    }
            //}

            //create new xls file
            //string file = "C:\\newdoc.xls";
            //Workbook workbook = new Workbook();
            //Worksheet worksheet = new Worksheet("First Sheet");
            //worksheet.Cells[0, 1] = new Cell((short)1);
            //worksheet.Cells[2, 0] = new Cell(9999999);
            //worksheet.Cells[3, 3] = new Cell((decimal)3.45);
            //worksheet.Cells[2, 2] = new Cell("Text string");
            //worksheet.Cells[2, 4] = new Cell("Second string");
            //worksheet.Cells[4, 0] = new Cell(32764.5, "#,##0.00");
            //worksheet.Cells[5, 1] = new Cell(DateTime.Now, @"YYYY\-MM\-DD");
            //worksheet.Cells.ColumnWidth[0, 1] = 3000;
            //workbook.Worksheets.Add(worksheet);
            //workbook.Save(file);
            ////========================================= 
            #endregion

            FileStream logFileStream = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Workbook book = Workbook.Load(logFileStream);
            Worksheet sheet = book.Worksheets[0];

            #region example using ExcelLibrary
            // traverse cells => melakukan perjalanan by cells
            //foreach (Pair<Pair<int, int>, Cell> cell in sheet.Cells)
            //{
            //    //dgvCells[cell.Left.Right, cell.Left.Left].Value = cell.Right.Value;
            //    MessageBox.Show(cell.Left.Right.ToString() + " ... " + cell.Left.Left.ToString());
            //}

            // traverse rows by Index => melakukan perjalanan by rows
            //for (int rowIndex = sheet.Cells.FirstRowIndex;
            //       rowIndex <= sheet.Cells.LastRowIndex; rowIndex++)
            //{
            //    Row row = sheet.Cells.GetRow(rowIndex);
            //    for (int colIndex = row.FirstColIndex;
            //       colIndex <= row.LastColIndex; colIndex++)
            //    {
            //        Cell cell = row.GetCell(colIndex);
            //        MessageBox.Show(cell.ToString());
            //    }
            //} 
            #endregion

            _connection.Open();
            SqlTransaction transaction = _connection.BeginTransaction();

            for (int rowIndex = sheet.Cells.FirstRowIndex + 1;
                   rowIndex <= sheet.Cells.LastRowIndex; rowIndex++)
            {
                Cell cellPPTK = sheet.Cells[rowIndex, 0];
                Cell cellKodePanggil = sheet.Cells[rowIndex, 1];
                Cell cellDigitTerakhir = sheet.Cells[rowIndex, 8];
                Cell cellSebelum = sheet.Cells[rowIndex, 10];
                Cell cellSesudah = sheet.Cells[rowIndex, 11];
                Cell cellUraian = sheet.Cells[rowIndex, 9];
                Cell cellDigit1 = sheet.Cells[rowIndex, 2];
                Cell cellDigit2 = sheet.Cells[rowIndex, 3];
                Cell cellDigit3 = sheet.Cells[rowIndex, 4];
                Cell cellDigit4 = sheet.Cells[rowIndex, 5];
                Cell cellDigit5 = sheet.Cells[rowIndex, 6];
                Cell cellDigit6 = sheet.Cells[rowIndex, 7];

                MessageBox.Show(_tools.NullToString(cellPPTK) + "|" + _tools.NullToString(cellKodePanggil) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "|" +
                    Convert.ToInt16(_tools.NullToNumber(cellDigit2)) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigit3)) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigit4)) + "|" +
                    Convert.ToInt16(_tools.NullToNumber(cellDigit5)) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) + "|" +
                    _tools.NullToString(cellUraian) + "|" + _tools.NullToNumber(cellSebelum) + "|" + _tools.NullToNumber(cellSesudah));

                int actualRow = rowIndex + 1;

                if (string.IsNullOrEmpty(_tools.NullToString(cellKodePanggil)) &&
                     (!string.IsNullOrEmpty(_tools.NullToString(cellDigitTerakhir))))
                {
                    MessageBox.Show(@"KODE PANGGIL tidak dilengkapi pada baris ke - " + actualRow);
                    e.Cancel = true;
                    _connection.Close();
                    return;
                }

                if (string.IsNullOrEmpty(_tools.NullToString(cellPPTK)) &&
                    (!string.IsNullOrEmpty(_tools.NullToString(cellKodePanggil)) &&
                     (!string.IsNullOrEmpty(_tools.NullToString(cellDigitTerakhir)))))
                {
                    MessageBox.Show(@"PPTK tidak dilengkapi pada baris ke - " + actualRow);
                    e.Cancel = true;
                    _connection.Close();
                    return;
                }

                if (CekIfKeyNotFound(_connection, transaction, _tools.NullToString(cellKodePanggil)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellPPTK)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellKodePanggil)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellDigitTerakhir)))
                {
                    _query1 = "update";

                    if (_tools.NullToString(cellUraian).Contains("*"))
                        _query2 = "update kasda..angkas_dtl set p = '', k = '', ";
                    else
                        _query2 = "fungional";
                    try
                    {
                        _connect.MasukkanData(_query1, _connection, transaction);
                        _connect.MasukkanData(_query2, _connection, transaction);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(@"Terjadi Kesalahan SQL, kode : " + ex.Message);
                        transaction.Rollback();
                        return;
                    }
                    transaction.Commit();
                }
                else if (!CekIfKeyNotFound(_connection, transaction, _tools.NullToString(cellKodePanggil)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellPPTK)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellKodePanggil)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellDigitTerakhir)))
                {
                    _query1 = "";
                    _query2 = "";
                    try
                    {
                        _connect.MasukkanData(_query1, _connection, transaction);
                        _connect.MasukkanData(_query2, _connection, transaction);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(@"Terjadi Kesalahan SQL, kode : " + ex.Message);
                        transaction.Rollback();
                        return;
                    }
                    transaction.Commit();
                }
            }
            _connection.Close();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button2.Enabled = true;
        }
        /// <summary>
        /// created         : nov-08-2013
        /// creator         : Putu
        /// name            : cekIfKeyNotFound
        /// description     : fungsi untuk mencek apabila value yg dimasukan ada atau tidak pada database
        ///                 : digunakan pada proses insert (bisa value tidak ditemukan) / update (bila terdapat value)   
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
    }
}
