using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using ExcelLibrary.SpreadSheet;
using RealAnggaran.Properties;

namespace RealAnggaran.misc_tool
{
    public partial class FPak : Form
    {
        readonly CKonek _connect = new CKonek();
        readonly CAlat _tools = new CAlat();
        readonly SqlConnection _connection;
        private string _queryRekening;
        private string _queryKasda;
        private string _queryAngkas;

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

        private void Bantai()
        {
            backgroundWorker1.CancelAsync();
            backgroundWorker1.Dispose();
            backgroundWorker1 = null;
            GC.Collect();
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
            decimal incrementAngkas = HitungAngkas();
            string idAngkas = GetidAngkas();
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

            Workbook book;
            try
            {
                FileStream logFileStream = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite);
                book = Workbook.Load(logFileStream);
            }
            catch
            {
                MessageBox.Show(Resources.FPak_backgroundWorker1_DoWork_);
                return;
            }         
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

            #region useful garbage

            //_queryRekening = "update realanggar..a_rekening set program = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "', kegiatan = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit2)) +
            //           "', uraian = '" + _tools.NullToString(cellUraian) + "', kode_kelompok = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit3)) + "', kode_jenis = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit4)) +
            //           "', kode_objek = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit5)) + "', kode_rincian = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "', idktg_blj = '" + Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) +
            //           "', formatpanjang = '" + _tools.NullToString(cellDigit1) + "." + _tools.NullToString(cellDigit2) + "." + _tools.NullToString(cellDigit3) + _tools.NullToString(cellDigit4) +
            //            _tools.NullToString(cellDigit5) + "." + _tools.NullToString(cellDigit6) + "." + _tools.NullToString(cellDigitTerakhir) + "' where kd_reken = '" + _tools.NullToString(cellKodePanggil) + "'";

            //_queryKasda = "update kasda..akd_rincian set kode_kelompok = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit3)) + "', kode_jenis = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit4)) + "', kode_obyek = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit5)) +
            //    "', kode_rincian = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "', idktg_blj = '" + Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) + "', uraian = '" + _tools.NullToString(cellUraian) +
            //    "', p = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "', k = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit2)) + "', formatpanjang = '" + _tools.NullToString(cellDigit1) + "." + _tools.NullToString(cellDigit2) + "." + _tools.NullToString(cellDigit3) + _tools.NullToString(cellDigit4) +
            //     _tools.NullToString(cellDigit5) + "." + _tools.NullToString(cellDigit6) + "." + _tools.NullToString(cellDigitTerakhir) + "' where id_rinci_rs = '" + _tools.NullToString(cellKodePanggil) + "'";


            //if (_tools.NullToString(cellUraian).Contains("*"))
            //    _queryAngkas = "update kasda..angkas_dtl set p = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "', k = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit2)) +
            //        "', tsubsi = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) +
            //        ", tsub_pak = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", tsub_sblm_pak = " + _tools.NullToNumber(cellSebelum) + ", " +
            //        "tot_angkas = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", tot_sblm_pak = " + _tools.NullToNumber(cellSebelum) + ", " +
            //        "bantu = '" + _tools.NullToString(cellUraian) + "', kode_kelompok = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit3)) +
            //        "', kode_jenis = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit4)) + "', kode_obyek = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit5)) +
            //        "', kode_rincian = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "', idktg_blj = '" + Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) +
            //        "' where id_rinci_rs = '" + _tools.NullToString(cellKodePanggil) + "'";
            //else
            //    _queryAngkas = "update kasda..angkas_dtl set p = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "', k = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit2)) +
            //        "', tfungsi = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) +
            //        ", tfung_pak = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", tfung_sblm_pak = " + _tools.NullToNumber(cellSebelum) + ", " +
            //        "tot_angkas = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", tot_sblm_pak = " + _tools.NullToNumber(cellSebelum) + ", " +
            //        "bantu = '" + _tools.NullToString(cellUraian) + "', kode_kelompok = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit3)) +
            //        "', kode_jenis = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit4)) + "', kode_obyek = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit5)) +
            //        "', kode_rincian = '" + Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "', idktg_blj = '" + Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) +
            //        "' where id_rinci_rs = '" + _tools.NullToString(cellKodePanggil) + "'"; 

            //MessageBox.Show(_tools.NullToString(cellPPTK) + "|" + _tools.NullToString(cellKodePanggil) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "|" +
            //    Convert.ToInt16(_tools.NullToNumber(cellDigit2)) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigit3)) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigit4)) + "|" +
            //    Convert.ToInt16(_tools.NullToNumber(cellDigit5)) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "|" + Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) + "|" +
            //    _tools.NullToString(cellUraian) + "|" + _tools.NullToNumber(cellSebelum) + "|" + _tools.NullToNumber(cellSesudah));

            #endregion

            _connection.Open();
            SqlTransaction transaction = _connection.BeginTransaction();

            for (int rowIndex = sheet.Cells.FirstRowIndex + 2;
                   rowIndex <= sheet.Cells.LastRowIndex; rowIndex++)
            {
                // for checking excel file status : the signature
                Cell cellFileSignature = sheet.Cells[0, 1];

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

                int actualRow = rowIndex + 1;
             
                if (_tools.NullToString(cellFileSignature) != "fixed" ||
                    string.IsNullOrEmpty(_tools.NullToString(cellFileSignature)))
                {
                    MessageBox.Show(Resources.FPak_backgroundWorker1_DoWork_1);
                    _connection.Close();
                    e.Cancel = true;
                    Bantai();
                    return;
                }

                if (string.IsNullOrEmpty(_tools.NullToString(cellKodePanggil)) &&
                     (!string.IsNullOrEmpty(_tools.NullToString(cellDigitTerakhir))))
                {
                    MessageBox.Show(@"KODE PANGGIL tidak dilengkapi pada baris ke - " + actualRow);
                    _connection.Close();
                    e.Cancel = true;
                    Bantai();
                    return;
                }

                if (string.IsNullOrEmpty(_tools.NullToString(cellPPTK)) &&
                    (!string.IsNullOrEmpty(_tools.NullToString(cellKodePanggil)) &&
                     (!string.IsNullOrEmpty(_tools.NullToString(cellDigitTerakhir)))))
                {
                    MessageBox.Show(@"PPTK tidak dilengkapi pada baris ke - " + actualRow);
                    _connection.Close();
                    e.Cancel = true;
                    Bantai();
                    return;
                }

                if (CekIfKeyNotFound(_connection, transaction, _tools.NullToString(cellKodePanggil)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellPPTK)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellKodePanggil)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellDigitTerakhir)))
                {
                    if (_tools.NullToString(cellUraian).Contains("*"))
                        _queryAngkas = "update kasda..angkas_dtl set tsubsi = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) +
                            ", tsub_pak = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", tsub_sblm_pak = " + _tools.NullToNumber(cellSebelum) + ", " +
                            "tot_angkas = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", tot_sblm_pak = " + _tools.NullToNumber(cellSebelum) +
                            " where id_rinci_rs = '" + _tools.NullToString(cellKodePanggil) + "'";
                    else
                        _queryAngkas = "update kasda..angkas_dtl set tfungsi = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) +
                            ", tfung_pak = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", tfung_sblm_pak = " + _tools.NullToNumber(cellSebelum) + ", " +
                            "tot_angkas = " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", tot_sblm_pak = " + _tools.NullToNumber(cellSebelum) +
                            " where id_rinci_rs = '" + _tools.NullToString(cellKodePanggil) + "'";
                    try
                    {
                        _connect.MasukkanData(_queryAngkas, _connection, transaction);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(@"Terjadi Kesalahan SQL, kode : " + ex.Message);
                        transaction.Rollback();
                        _connection.Close();
                        return;
                    }
                }
                else if (!CekIfKeyNotFound(_connection, transaction, _tools.NullToString(cellKodePanggil)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellPPTK)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellKodePanggil)) &&
                    !string.IsNullOrEmpty(_tools.NullToString(cellDigitTerakhir)))
                {
                    _queryRekening = "insert into realanggar..a_rekening values ('" +
                                     _tools.NullToString(cellKodePanggil) + "', '" +
                                     Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "', '" +
                                     Convert.ToInt16(_tools.NullToNumber(cellDigit2)) +
                                     "', '" + _tools.NullToString(cellUraian) + "', '" +
                                     Convert.ToInt16(_tools.NullToNumber(cellDigit3)) + "', '" +
                                     Convert.ToInt16(_tools.NullToNumber(cellDigit4)) +
                                     "', '" + Convert.ToInt16(_tools.NullToNumber(cellDigit5)) + "', '" +
                                     Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "', '" +
                                     Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) +
                                     "', '" + _tools.NullToString(cellDigit1) + "." + _tools.NullToString(cellDigit2) +
                                     "." + _tools.NullToString(cellDigit3) + _tools.NullToString(cellDigit4) +
                                     _tools.NullToString(cellDigit5) + "." + _tools.NullToString(cellDigit6) + "." +
                                     _tools.NullToString(cellDigitTerakhir) + "')";

                    _queryKasda = "insert into kasda..akd_rincian values ('" +
                                  _tools.NullToString(cellKodePanggil) + "', '" +
                                  Convert.ToInt16(_tools.NullToNumber(cellDigit3)) + "', '" +
                                  Convert.ToInt16(_tools.NullToNumber(cellDigit4)) + "', '" +
                                  Convert.ToInt16(_tools.NullToNumber(cellDigit5)) +
                                  "', '" + Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "', '" +
                                  Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) + "', '" +
                                  _tools.NullToString(cellUraian) +
                                  "', '" + Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "', '" +
                                  Convert.ToInt16(_tools.NullToNumber(cellDigit2)) + "', '" +
                                  _tools.NullToString(cellDigit1) + "." + _tools.NullToString(cellDigit2) + "." +
                                  _tools.NullToString(cellDigit3) + _tools.NullToString(cellDigit4) +
                                  _tools.NullToString(cellDigit5) + "." + _tools.NullToString(cellDigit6) + "." +
                                  _tools.NullToString(cellDigitTerakhir) + "')";

                    if (_tools.NullToString(cellUraian).Contains("*"))
                        _queryAngkas =
                            "insert into kasda..angkas_dtl (IdAngkas_Dtl, IdAngkas, Id_Rinci_Rs, P, K, TSubsi, Tot_Angkas, BANTU, kode_Kelompok, kode_Jenis, " +
                            "kode_Obyek, kode_Rincian, IdKtg_Blj, Thn_Ang) values (" + incrementAngkas + ", " + idAngkas +
                            ", '" + Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "', '" +
                            Convert.ToInt16(_tools.NullToNumber(cellDigit2)) + "', '" +
                            _tools.NullToString(cellKodePanggil) + "'" +
                            "', " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", " +
                            _tools.NullToNumber(cellSesudah) + ", " +
                            "'" + _tools.NullToString(cellUraian) + "', '" +
                            Convert.ToInt16(_tools.NullToNumber(cellDigit3)) +
                            "', '" + Convert.ToInt16(_tools.NullToNumber(cellDigit4)) + "', '" +
                            Convert.ToInt16(_tools.NullToNumber(cellDigit5)) +
                            "', '" + Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "', '" +
                            Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) +
                            "', '" + DateTime.Now.Year + "')";
                    else
                    _queryAngkas =
                        "insert into kasda..angkas_dtl (IdAngkas_Dtl, IdAngkas, Id_Rinci_Rs, P, K, TFungsi, Tot_Angkas, BANTU, kode_Kelompok, kode_Jenis, " +
                        "kode_Obyek, kode_Rincian, IdKtg_Blj, Thn_Ang) values ('" + incrementAngkas + "', '" + idAngkas +
                        "', '" + _tools.NullToString(cellKodePanggil) +
                        "', '" + Convert.ToInt16(_tools.NullToNumber(cellDigit1)) + "', '" +
                        Convert.ToInt16(_tools.NullToNumber(cellDigit2)) + "'" +
                        ", " + Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) + ", " +
                        Convert.ToDecimal(_tools.NullToNumber(cellSesudah)) +
                        ", '" + _tools.NullToString(cellUraian) + "', '" +
                        Convert.ToInt16(_tools.NullToNumber(cellDigit3)) +
                        "', '" + Convert.ToInt16(_tools.NullToNumber(cellDigit4)) + "', '" +
                        Convert.ToInt16(_tools.NullToNumber(cellDigit5)) +
                        "', '" + Convert.ToInt16(_tools.NullToNumber(cellDigit6)) + "', '" +
                        Convert.ToInt16(_tools.NullToNumber(cellDigitTerakhir)) +
                        "', '" + DateTime.Now.Year + "')";
                    try
                    {
                        _connect.MasukkanData(_queryRekening, _connection, transaction);
                        _connect.MasukkanData(_queryKasda, _connection, transaction);
                        _connect.MasukkanData(_queryAngkas, _connection, transaction);
                        _connect.MasukkanData("insert into realanggar..a_det_pptk values ('" + _tools.NullToString(cellPPTK) +
                            "', '" + _tools.NullToString(cellKodePanggil) + "')", _connection, transaction);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(@"Terjadi Kesalahan SQL, kode : " + ex.Message);
                        transaction.Rollback();
                        _connection.Close();
                        return;
                    }
                }
                incrementAngkas++;
            }
            transaction.Commit();
            MessageBox.Show(@"transaksi sukses");
            _connection.Close();
        }

        private decimal HitungAngkas()
        {
            decimal jumlah = 0;
            _connection.Open();
            SqlDataReader reader = _connect.MembacaData("SELECT COUNT (*) FROM KASDA..ANGKAS_DTL", _connection);
            if (reader.HasRows)
            {
                reader.Read();
                jumlah = Convert.ToDecimal(_tools.PengecekField(reader, 0));
                reader.Close();
            }
            _connection.Close();
            return jumlah + 1;
        }

        private string GetidAngkas()
        {
            string jumlah = "";
            _connection.Open();
            SqlDataReader reader = _connect.MembacaData("SELECT IdAngkas FROM KASDA..ANGKAS_MST WHERE Thn_Ang = '" + DateTime.Now.Year + "'", _connection);
            if (reader.HasRows)
            {
                reader.Read();
                jumlah = _tools.PengecekField(reader, 0);
                reader.Close();
            }
            else
            {
                MessageBox.Show(@"Admin belum menambah Master Anggaran.\nSilahkan hubungi 1062", @"PERHATIAN");
                Close();
            }
            _connection.Close();
            return jumlah;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button2.Enabled = true;
        }

/*
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
        /// <param name="refNilai"></param>
        /// <returns>jika nilai bool true maka update</returns>
        private bool CekIfKeyNotFound(SqlConnection sqlConnection, SqlTransaction transaction, string keystring, ref decimal refNilai)
        {
            SqlDataReader reader =
                _connect.MembacaData(
                    "SELECT     c.TFungsi, c.TSubsi " +
                    "FROM         kasda..AKD_RINCIAN AS a INNER JOIN " +
                    "realanggar.dbo.A_REKENING AS b ON a.Id_Rinci_RS = b.Kd_Reken INNER JOIN " +
                    "kasda..ANGKAS_DTL AS c ON b.Kd_Reken = c.Id_Rinci_Rs " +
                    "WHERE     (REPLACE(REPLACE(REPLACE(a.Id_Rinci_RS, ' ', ''), CHAR(10), ''), CHAR(13), '') = '"+keystring+"')", sqlConnection, transaction);
            if (reader.HasRows)
            {
                reader.Read();
                refNilai = (decimal) reader[0] + (decimal) reader[1];
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }
*/
        /// <summary>
        /// same as above but this one doesnt include the ref parameter
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="keystring"></param>
        /// <returns></returns>
        private bool CekIfKeyNotFound(SqlConnection sqlConnection, SqlTransaction transaction, string keystring)
        {
            SqlDataReader reader =
                _connect.MembacaData(
                    "SELECT     c.TFungsi " +
                    "FROM         kasda..AKD_RINCIAN AS a INNER JOIN " +
                    "realanggar.dbo.A_REKENING AS b ON a.Id_Rinci_RS = b.Kd_Reken INNER JOIN " +
                    "kasda..ANGKAS_DTL AS c ON b.Kd_Reken = c.Id_Rinci_Rs " +
                    "WHERE     (REPLACE(REPLACE(REPLACE(a.Id_Rinci_RS, ' ', ''), CHAR(10), ''), CHAR(13), '') = '" + keystring + "')", sqlConnection, transaction);
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
