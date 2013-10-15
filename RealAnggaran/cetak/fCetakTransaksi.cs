using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RealAnggaran.Properties;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Globalization;
using System.Data.SqlClient;
using System.IO;

namespace RealAnggaran.cetak
{
    public partial class FCetakTransaksi : Form
    {
        readonly CKonek _konek = new CKonek();
        readonly CAlat _alat = new CAlat();
        readonly SqlConnection _koneksi;
        SqlDataReader _reader = null;
        string _query = null;
        private List<TampungLaporanTran> _tabelKasda;
        List<ListViewItem> _lvItemGroup = new List<ListViewItem>();
        int _statusClose = 0;

        public FCetakTransaksi()
        {
            _koneksi = _konek.KonekDb();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        private void DisableButton(int statusButton)
        {

            if (statusButton == 1)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
            }
        }

        private class TampungLaporanTran
        {
            //Alternatif buat class dengan atau tanpa constructor
            public TampungLaporanTran(string idLaporan, DateTime tglSP, string noSPK, string idSupplier, string keter, 
                string noBukti, DateTime tglSPJ, decimal total, string idRInciRs, string p_, string k_, string kdKelompok,
                string kdJenis, string kdObjek, string kdRincian, string idKtgBlj, string uraian, string formatPanjang) //
            {                                                             // Jika tidak menggunakan constructor      
                this.idLaporan = idLaporan;                                       // hapus pada baris yg di comment 
                this.tglSP = tglSP;                                       //  
                this.noSPK = noSPK;                                   //
                this.idSupplier = idSupplier;
                this.keter = keter;
                this.noBukti = noBukti;
                this.tglSPJ = tglSPJ;
                this.total = total;
                this.idRInciRs = idRInciRs;
                this.p_ = p_;
                this.k_ = k_;
                this.kdKelompok = kdKelompok;  
                this.kdJenis = kdJenis;  
                this.kdObjek = kdObjek;  
                this.kdRincian = kdRincian;  
                this.idKtgBlj = idKtgBlj;  
                this.uraian = uraian;  
                this.formatPanjang = formatPanjang;  
            }
            public string idLaporan { set; get; }
            public DateTime tglSP { set; get; }
            public string noSPK { set; get; }
            public string idSupplier { set; get; }
            public string keter { set; get; }
            public string noBukti { set; get; }
            public DateTime tglSPJ { set; get; }
            public decimal total { set; get; }
            public string idRInciRs { set; get; }
            public string p_ { set; get; }
            public string k_ { set; get; }
            public string kdKelompok { set; get; }
            public string kdJenis { set; get; }
            public string kdObjek { set; get; }
            public string kdRincian { set; get; }
            public string idKtgBlj { set; get; }
            public string uraian { set; get; }
            public string formatPanjang { set; get; }
        }

        private void fCetakTransaksi_Load(object sender, EventArgs e)
        {
            foreach (Process proc in Process.GetProcessesByName("EXCEL"))
            {
                if (proc.MainWindowTitle == "")
                    proc.Kill();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(string.Format("{0: dd/MM/yyyy}", dateTimePicker1.Value));
            if (comboBox1.SelectedIndex >= 0)
            {
                if (!string.IsNullOrEmpty(textBox1.Text.Trim()))
                {
                    var fi = new FileInfo(textBox1.Text);
                    if (fi.Exists)
                    {
                        if (!backgroundWorker1.IsBusy)
                        {
                            DisableButton(0);
                            backgroundWorker1.RunWorkerAsync();
                        }
                    }
                    else
                        MessageBox.Show(Resources.FCetakTransaksi_button1_Click_,
                            Resources.FCetakTransaksi_button1_Click_PERHATIAN);
                }
                else
                    MessageBox.Show(Resources.FCetakTransaksi_button1_Click_Silahkan_pilih_file_yg_akan_dieksekusi_,
                        Resources.FCetakTransaksi_button1_Click_PERHATIAN);
            }
            else
            {
                MessageBox.Show(Resources.FCetakTransaksi_button1_Click_pilih, 
                    Resources.FCetakTransaksi_button1_Click_PERHATIAN);
                comboBox1.Focus();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string strTanggal = comboBox1.SafeControlInvoke(comboBox => comboBox1.SelectedIndex == 0 ? "A.Tgl_SP" : "A.Tgl_SPJ");
            if (rbSemua.Checked)
            {
                _query = "SELECT A.IdBlj_Master, A.Tgl_SP, A.No_SPK, A.IdSupplier, A.Keter, A.No_Bukti, " +
                     "A.TGL_SPJ, B.TSubsi + B.TFungsi AS Total, B.Id_Rinci_Rs, C.P, C.K, " +
                     "C.Kode_Kelompok, C.Kode_Jenis, C.Kode_Obyek, C.Kode_Rincian, C.idKtg_blj, C.Uraian, " +
                     "C.formatPanjang " +
                     "FROM KASDA..BLJ_MASTER A INNER JOIN " +
                     "KASDA..BLJ_DETAIL B ON B.IdBlj_Master = A.IdBlj_Master INNER JOIN " +
                     "KASDA..AKD_RINCIAN C ON C.Id_Rinci_Rs = B.Id_Rinci_RS " +
                     "WHERE " + strTanggal + " BETWEEN CONVERT(DATETIME, '" + string.Format("{0:yyyy-MM-dd}", dateTimePicker1.Value) +
                     " 00:00:00', 121) AND CONVERT(DATETIME, '" + string.Format("{0:yyyy-MM-dd}", dateTimePicker2.Value) + " 23:59:59', 121)";
                //CONVERT(SMALLDATETIME, '" + string.Format("{0:dd/MM/yyyy}", dateTimePicker1.Value) +
                //"', 103) AND CONVERT(SMALLDATETIME, '" + string.Format("{0:dd/MM/yyyy}", dateTimePicker2.Value) + "', 103)";
            }
            else if (rbLunas.Checked)
            {
                _query = "SELECT A.IdBlj_Master, A.Tgl_SP, A.No_SPK, A.IdSupplier, A.Keter, A.No_Bukti, " +
                     "A.TGL_SPJ, B.TSubsi + B.TFungsi AS Total, B.Id_Rinci_Rs, C.P, C.K, " +
                     "C.Kode_Kelompok, C.Kode_Jenis, C.Kode_Obyek, C.Kode_Rincian, C.idKtg_blj, C.Uraian, " +
                     "C.formatPanjang " +
                     "FROM KASDA..BLJ_MASTER A INNER JOIN " +
                     "KASDA..BLJ_DETAIL B ON B.IdBlj_Master = A.IdBlj_Master INNER JOIN " +
                     "KASDA..AKD_RINCIAN C ON C.Id_Rinci_Rs = B.Id_Rinci_RS " +
                     "WHERE " + strTanggal + " BETWEEN CONVERT(DATETIME, '" + string.Format("{0:yyyy-MM-dd}", dateTimePicker1.Value) +
                     " 00:00:00', 121) AND CONVERT(DATETIME, '" + string.Format("{0:yyyy-MM-dd}", dateTimePicker2.Value) + " 23:59:59', 121) AND A.Lunas = 'Y'";
            }
            else if (rbBelum.Checked)
            {
                _query = "SELECT A.IdBlj_Master, A.Tgl_SP, A.No_SPK, A.IdSupplier, A.Keter, A.No_Bukti, " +
                     "A.TGL_SPJ, B.TSubsi + B.TFungsi AS Total, B.Id_Rinci_Rs, C.P, C.K, " +
                     "C.Kode_Kelompok, C.Kode_Jenis, C.Kode_Obyek, C.Kode_Rincian, C.idKtg_blj, C.Uraian, " +
                     "C.formatPanjang " +
                     "FROM KASDA..BLJ_MASTER A INNER JOIN " +
                     "KASDA..BLJ_DETAIL B ON B.IdBlj_Master = A.IdBlj_Master INNER JOIN " +
                     "KASDA..AKD_RINCIAN C ON C.Id_Rinci_Rs = B.Id_Rinci_RS " +
                     "WHERE " + strTanggal + " BETWEEN CONVERT(DATETIME, '" + string.Format("{0:yyyy-MM-dd}", dateTimePicker1.Value) +
                     " 00:00:00', 121) AND CONVERT(DATETIME, '" + string.Format("{0:yyyy-MM-dd}", dateTimePicker2.Value) + " 23:59:59', 121) AND A.Lunas = ' '";
            }
            List<TampungLaporanTran> grupLaporanTran = new List<TampungLaporanTran>();
            _koneksi.Open();
            _reader = _konek.MembacaData(_query, _koneksi);
            if (_reader.HasRows)
            {
                while (_reader.Read())
                    grupLaporanTran.Add(new TampungLaporanTran(_alat.PengecekField(_reader, 0), Convert.ToDateTime(_reader[1]), _alat.PengecekField(_reader, 2),
                        _alat.PengecekField(_reader, 3), _alat.PengecekField(_reader, 4), _alat.PengecekField(_reader, 5), Convert.ToDateTime(_reader[6]),
                        Convert.ToDecimal(_alat.PengecekField(_reader, 7)), _alat.PengecekField(_reader, 8), _alat.PengecekField(_reader, 9), _alat.PengecekField(_reader, 10), _alat.PengecekField(_reader, 11),
                        _alat.PengecekField(_reader, 12), _alat.PengecekField(_reader, 13), _alat.PengecekField(_reader, 14), _alat.PengecekField(_reader, 15), _alat.PengecekField(_reader, 16),
                        _alat.PengecekField(_reader, 17)));
                _reader.Close();
            }
            _koneksi.Close();
           e.Result = grupLaporanTran;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _tabelKasda = (List<TampungLaporanTran>)e.Result;
            backgroundWorker2.RunWorkerAsync();
        }

/*
        private void showListView()
        {
            foreach (var i in _tabelKasda)
            {
                ListViewItem item = new ListViewItem(i.idLaporan);
                item.SubItems.Add(i.tglSP.ToString());
                item.SubItems.Add(i.noSPK);
                item.SubItems.Add(i.idSupplier);
                item.SubItems.Add(i.keter);
                item.SubItems.Add(i.noBukti);
                item.SubItems.Add(i.tglSPJ.ToString());
                item.SubItems.Add(i.idRInciRs);
                item.SubItems.Add(i.p_);
                item.SubItems.Add(i.k_);
                item.SubItems.Add(i.kdKelompok);
                item.SubItems.Add(i.kdJenis);
                item.SubItems.Add(i.kdObjek);
                item.SubItems.Add(i.kdRincian);
                item.SubItems.Add(i.idKtgBlj);
                item.SubItems.Add(i.uraian);
                item.SubItems.Add(i.formatPanjang);
                lvItemGroup.Add(item);
            }
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Items.AddRange(lvItemGroup.ToArray());
            listView1.EndUpdate();
            lvItemGroup.Clear();
        }
*/
        #region Release Object
        private void releaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show(Resources.FCetakTransaksi_releaseObject_Exception_Occured_while_releasing_object_ + 
                    ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        #endregion

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            _statusClose = 1;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Sheets xlSheets = null;
            object misValue = Missing.Value;
            try
            {
                xlApp = new Excel.ApplicationClass();
                //xlWorkBook = xlApp.Workbooks.Add(misValue); if there is no existing excel file
                xlWorkBook = xlApp.Workbooks.Open(textBox1.Text, 0, false, 5, "", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1); if there is no existing excel file
                xlSheets = xlWorkBook.Worksheets;
                xlWorkSheet = (Excel.Worksheet)xlSheets.get_Item("Sheet1");
                Excel.Range range = xlWorkSheet.UsedRange;
                //xlWorkSheet.Cells[2, 2] = "http://csharp.net-informations.com"; if there is no existing excel file

                //int colCount = range.Columns.Count;
                //int rowCount = range.Rows.Count;
                //for (int index = 1; index < 20; index++)
                //{
                //    xlWorkSheet.Cells[rowCount + index, 1] = rowCount + index;
                //    xlWorkSheet.Cells[rowCount + index, 2] = "New Item" + index;
                //}
                int baris = 1;
                progressBar1.SafeControlInvoke(progressBar => progressBar1.Maximum = _tabelKasda.Count + 1);
                progressBar1.SafeControlInvoke(progressBar => progressBar1.Minimum = 0);


                foreach (var i in _tabelKasda)
                {
                    baris++;
                    progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Value = baris);
                    xlWorkSheet.Cells[baris, 1] = i.idLaporan;
                    xlWorkSheet.Cells[baris, 2] = i.tglSP.ToString();
                    xlWorkSheet.Cells[baris, 3] = i.noSPK;
                    xlWorkSheet.Cells[baris, 4] = i.idSupplier;
                    xlWorkSheet.Cells[baris, 5] = i.keter;
                    xlWorkSheet.Cells[baris, 6] = i.noBukti;
                    xlWorkSheet.Cells[baris, 7] = i.tglSPJ.ToString();
                    xlWorkSheet.Cells[baris, 8] = i.total.ToString();
                    xlWorkSheet.Cells[baris, 9] = i.idRInciRs;
                    xlWorkSheet.Cells[baris, 10] = i.p_;
                    xlWorkSheet.Cells[baris, 11] = i.k_;
                    xlWorkSheet.Cells[baris, 12] = i.kdKelompok;
                    xlWorkSheet.Cells[baris, 13] = i.kdJenis;
                    xlWorkSheet.Cells[baris, 14] = i.kdObjek;
                    xlWorkSheet.Cells[baris, 15] = i.kdRincian;
                    xlWorkSheet.Cells[baris, 16] = i.idKtgBlj;
                    xlWorkSheet.Cells[baris, 17] = i.uraian;
                    xlWorkSheet.Cells[baris, 18] = i.formatPanjang;
                }
                xlApp.AlertBeforeOverwriting = false;
                xlApp.DisplayAlerts = false;
                xlWorkBook.SaveAs(textBox1.Text, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, 
                    misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                MessageBox.Show(Resources.FCetakTransaksi_backgroundWorker2_DoWork_File_Tercetak, Resources.FCetakTransaksi_backgroundWorker2_DoWork_KONFIRMASI);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.FCetakTransaksi_backgroundWorker2_DoWork_ + ex.Message, Resources.FCetakTransaksi_button1_Click_PERHATIAN);
                MessageBox.Show(Resources.FCetakTransaksi_backgroundWorker2_DoWork_File_GAGAL_Tercetak, Resources.FCetakTransaksi_backgroundWorker2_DoWork_KONFIRMASI);
            }
            finally
            {
                releaseObject(xlSheets);
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Maximum = 0;
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            _statusClose = 0;
            DisableButton(1);
        }

        private void fCetakTransaksi_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_statusClose == 1)
            {
                e.Cancel = true;
                MessageBox.Show(Resources.
                    FCetakTransaksi_fCetakTransaksi_FormClosing_Masih_Ada_Proses_berjalan_harap_tunggu, 
                    Resources.FCetakTransaksi_button1_Click_PERHATIAN);
            }
            else
            {
                foreach (Process proc in Process.GetProcessesByName("EXCEL"))
                {
                    if (proc.MainWindowTitle == "")
                        proc.Kill();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DisableButton(0);
            string filePath = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = folderBrowserDialog1.SelectedPath + "\\Laporan_" + string.Format("{0:dd_MM_yyyy}", 
                    DateTime.Now) + ".xls";
                filePath = filePath.Replace(@"\\", @"\");

                textBox1.Text = filePath;

                object misValue = Missing.Value;

                Excel.Application xlApp = new Excel.ApplicationClass();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Add(misValue);
                Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                //add data 
                xlWorkSheet.Cells[1, 1] = "No Urut";
                xlWorkSheet.Cells[1, 2] = "Tgl SP";
                xlWorkSheet.Cells[1, 3] = "No SPK";
                xlWorkSheet.Cells[1, 4] = "Rekanan";
                xlWorkSheet.Cells[1, 5] = "Keterangan";
                xlWorkSheet.Cells[1, 6] = "No Bukti";
                xlWorkSheet.Cells[1, 7] = "Tgl SPJ";
                xlWorkSheet.Cells[1, 8] = "Total";
                xlWorkSheet.Cells[1, 9] = "Kd Panggil";
                xlWorkSheet.Cells[1, 10] = "Kd Program";
                xlWorkSheet.Cells[1, 11] = "Kd Kegiatan";
                xlWorkSheet.Cells[1, 12] = "Kd Kelompok";
                xlWorkSheet.Cells[1, 13] = "Kd Jenis";
                xlWorkSheet.Cells[1, 14] = "Kd Objek";
                xlWorkSheet.Cells[1, 15] = "Kd Rincian";
                xlWorkSheet.Cells[1, 16] = "idKtgBlj";
                xlWorkSheet.Cells[1, 17] = "Uraian";
                xlWorkSheet.Cells[1, 18] = "No Rekening";

                #region Beberapa contoh manipulasi excel
                //xlWorkSheet.Cells[4, 2] = "";
                //xlWorkSheet.Cells[4, 3] = "Student1";
                //xlWorkSheet.Cells[4, 4] = "Student2";
                //xlWorkSheet.Cells[4, 5] = "Student3";

                //xlWorkSheet.Cells[5, 2] = "Term1";
                //xlWorkSheet.Cells[5, 3] = "80";
                //xlWorkSheet.Cells[5, 4] = "65";
                //xlWorkSheet.Cells[5, 5] = "45";

                //xlWorkSheet.Cells[6, 2] = "Term2";
                //xlWorkSheet.Cells[6, 3] = "78";
                //xlWorkSheet.Cells[6, 4] = "72";
                //xlWorkSheet.Cells[6, 5] = "60";

                //xlWorkSheet.Cells[7, 2] = "Term3";
                //xlWorkSheet.Cells[7, 3] = "82";
                //xlWorkSheet.Cells[7, 4] = "80";
                //xlWorkSheet.Cells[7, 5] = "65";

                //xlWorkSheet.Cells[8, 2] = "Term4";
                //xlWorkSheet.Cells[8, 3] = "75";
                //xlWorkSheet.Cells[8, 4] = "82";
                //xlWorkSheet.Cells[8, 5] = "68";

                //xlWorkSheet.Cells[9, 2] = "Total";
                //xlWorkSheet.Cells[9, 3] = "315";
                //xlWorkSheet.Cells[9, 4] = "299";
                //xlWorkSheet.Cells[9, 5] = "238";

                //xlWorkSheet.get_Range("b2", "e3").Merge(false);

                //chartRange = xlWorkSheet.get_Range("b2", "e3");
                //chartRange.FormulaR1C1 = "MARK LIST";
                //chartRange.HorizontalAlignment = 3;
                //chartRange.VerticalAlignment = 3;

                //chartRange = xlWorkSheet.get_Range("a1", "r1");
                //chartRange.Font.Bold = true;
                //chartRange = xlWorkSheet.get_Range("b9", "e9");
                //chartRange.Font.Bold = true;

                //chartRange = xlWorkSheet.get_Range("b2", "e9");
                //chartRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, Excel.XlColorIndex.xlColorIndexAutomatic);
                #endregion
                Excel.Range chartRange = xlWorkSheet.get_Range("a1", "r1");
                chartRange.Font.Bold = true;
                chartRange = xlWorkSheet.get_Range("a1", "r1");
                chartRange.Font.Size = 12;
                xlApp.AlertBeforeOverwriting = false;
                xlApp.DisplayAlerts = false;
                xlWorkBook.SaveAs(filePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, 
                    misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, 
                    misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlApp);
                releaseObject(xlWorkBook);
                releaseObject(xlWorkSheet);

                MessageBox.Show(Resources.FCetakTransaksi_backgroundWorker2_DoWork_File_Tercetak, 
                    Resources.FCetakTransaksi_backgroundWorker2_DoWork_KONFIRMASI);
            }
            DisableButton(1);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
