using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace RealAnggaran.misc_tool
{
    public partial class fCekSaldoPPTK : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader reader = null;
        List<ListViewItem> lvItemGroup = new List<ListViewItem>();
        string query = null;
        int statusPencarian = 1;
        int statusClose = 0;

        public fCekSaldoPPTK()
        {
            koneksi = konek.KonekDb();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }
        private class tampungSaldo
        {
            //Alternatif buat class dengan atau tanpa constructor
            public tampungSaldo(string kdPPTK, string kdRekening, decimal totalAnggaran, decimal totalBelanja, decimal sisa) //
            {                                                             // Jika tidak menggunakan constructor      
                this.kdPPTK = kdPPTK;                                       // hapus pada baris yg di comment 
                this.kdRekening = kdRekening;                                       //  
                this.totalAnggaran = totalAnggaran;                                   // 
                this.totalBelanja = totalBelanja;
                this.sisa = sisa;
            }
            public string kdPPTK { set; get; }
            public string kdRekening { set; get; }
            public decimal totalAnggaran { set; get; }
            public decimal totalBelanja { set; get; }
            public decimal sisa { set; get; }
        }
        private void bTampil_Click(object sender, EventArgs e)
        {
            if (statusClose == 0)
            {
                if (!backgroundWorker1.IsBusy)
                {
                    disableButton(0);
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (statusPencarian == 1) //PPTK based
                query = "SELECT     B.kd_PPTK, B.Kd_Reken, C.Tot_Angkas, COALESCE (SUM(D.TSubsi) + SUM(D.TFungsi), 0) AS Total_Belanja, C.Tot_Angkas - COALESCE (SUM(D.TSubsi) " +
                      "+ SUM(D.TFungsi), 0) AS Sisa, A.nama_PPTK, C.BANTU " +
                      "FROM         KASDA.dbo.BLJ_MASTER AS E INNER JOIN " +
                      "KASDA.dbo.BLJ_DETAIL AS D ON E.IdBlj_Master = D.IdBlj_Master AND E.Lunas = 'y' RIGHT OUTER JOIN " +
                      "A_PPTK AS A INNER JOIN " +
                      "A_DET_PPTK AS B ON A.kd_PPTK = B.kd_PPTK INNER JOIN " +
                      "KASDA.dbo.ANGKAS_DTL AS C ON B.Kd_Reken = C.Id_Rinci_Rs ON D.Id_Rinci_Rs = C.Id_Rinci_Rs " +
                      "WHERE     (A.kd_PPTK LIKE '" + textBox1.Text.Trim() + "%') AND (C.Thn_Ang = " + textBox2.Text.Trim() + ") " +
                      "GROUP BY B.Kd_Reken, B.kd_PPTK, C.Tot_Angkas, A.nama_PPTK, C.BANTU, C.Id_Rinci_Rs";
            else if (statusPencarian == 2) //Kd Panggil based
                query = "SELECT     B.kd_PPTK, B.Kd_Reken, C.Tot_Angkas, COALESCE (SUM(D.TSubsi) + SUM(D.TFungsi), 0) AS Total_Belanja, C.Tot_Angkas - COALESCE (SUM(D.TSubsi) " +
                      "+ SUM(D.TFungsi), 0) AS Sisa, A.nama_PPTK, C.BANTU " +
                      "FROM         KASDA.dbo.BLJ_MASTER AS E INNER JOIN " +
                      "KASDA.dbo.BLJ_DETAIL AS D ON E.IdBlj_Master = D.IdBlj_Master AND E.Lunas = 'y' RIGHT OUTER JOIN " +
                      "A_PPTK AS A INNER JOIN " +
                      "A_DET_PPTK AS B ON A.kd_PPTK = B.kd_PPTK INNER JOIN " +
                      "KASDA.dbo.ANGKAS_DTL AS C ON B.Kd_Reken = C.Id_Rinci_Rs ON D.Id_Rinci_Rs = C.Id_Rinci_Rs " +
                      "WHERE     (C.Thn_Ang = " + textBox2.Text + ") AND (C.Id_Rinci_Rs = '" + textBox3.Text + "') " +
                      "GROUP BY B.Kd_Reken, B.kd_PPTK, C.Tot_Angkas, A.nama_PPTK, C.BANTU, C.Id_Rinci_Rs";
            List<tampungSaldo> grupSaldo = new List<tampungSaldo>();
            koneksi.Open();
            reader = konek.MembacaData(query, koneksi);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    grupSaldo.Add(new tampungSaldo(alat.PengecekField(reader, 0), alat.PengecekField(reader, 1),
                        Convert.ToDecimal(alat.PengecekField(reader, 2)), Convert.ToDecimal(alat.PengecekField(reader, 3)),
                        Convert.ToDecimal(alat.PengecekField(reader, 4))));
                }
                reader.Close();
                progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Maximum = grupSaldo.Count + 1);
                foreach (var i in grupSaldo)
                {
                    ListViewItem item = new ListViewItem(i.kdPPTK);
                    item.SubItems.Add(i.kdRekening);
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.totalAnggaran));
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.totalBelanja));
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.sisa));
                    lvItemGroup.Add(item);
                    progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Value++);
                }
                listView1.SafeControlInvoke(listView =>
                {
                    listView1.BeginUpdate();
                    listView1.Items.Clear();
                    listView1.Items.AddRange(lvItemGroup.ToArray());
                    listView1.EndUpdate();
                }
                );
                lvItemGroup.Clear();
            }
            else
            {
                if (statusPencarian == 1)
                {
                    MessageBox.Show("PPTK dengan kode tersebut tidak ditemukan!", "PERHATIAN");
                    textBox1.SafeControlInvoke(teksBoks => textBox1.Focus());
                }
                else if (statusPencarian == 2)
                {
                    MessageBox.Show("Kode Rekening/Panggil tidak ditemukan!", "PERHATIAN");
                    textBox3.SafeControlInvoke(teksBoks => textBox3.Focus());
                }
            }
            koneksi.Close();
            e.Result = grupSaldo;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Maximum = 0;
            progressBar1.Maximum = 0;
            progressBar1.Value = 0;
            List<tampungSaldo> tabelSaldo = (List<tampungSaldo>)e.Result;
            if (tabelSaldo.Count > 0)
            {
                var namaKpa = (from s in tabelSaldo
                               group s by new
                                   {
                                       s.kdPPTK
                                   }
                                   into ss
                                   select new
                                       {
                                           //ss.Key.kdPPTK,
                                           jumlah = ss.Sum(s => s.sisa)
                                       }).First();
                label3.Text = string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", namaKpa.jumlah);
            }
            disableButton(1);
        }

        private void fCekSaldoPPTK_Load(object sender, EventArgs e)
        {
            foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
            {
                //if (proc.MainWindowTitle == "")
                proc.Kill();
            }
            textBox2.Text = DateTime.Now.Year.ToString();
            label3.Text = "";
        }

        #region Release Object
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        #endregion

        private void fCekSaldoPPTK_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (statusClose == 0)
                {
                    if (!backgroundWorker1.IsBusy)
                    {
                        disableButton(0);
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            statusPencarian = 1;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            statusPencarian = 2;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            disableButton(0);
            statusClose = 1;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Sheets xlSheets = null;
            object misValue = System.Reflection.Missing.Value;
            try
            {
                xlApp = new Excel.ApplicationClass();
                //xlWorkBook = xlApp.Workbooks.Add(misValue); if there is no existing excel file
                xlWorkBook = xlApp.Workbooks.Open(textBox4.Text, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1); if there is no existing excel file
                xlSheets = xlWorkBook.Worksheets;
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.get_Item("Sheet1");
                Microsoft.Office.Interop.Excel.Range range = xlWorkSheet.UsedRange;

                xlWorkSheet.Cells[1, 1] = "PPTK";
                xlWorkSheet.Cells[1, 2] = "Rekening";
                xlWorkSheet.Cells[1, 3] = "Total Anggaran";
                xlWorkSheet.Cells[1, 4] = "Total Belanja";
                xlWorkSheet.Cells[1, 5] = "Sisa";

                int baris = 1; //cari cara menghitung jumlah baris yg telah ditulis di excel
                progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Maximum = listView1.Items.Count + 1);
                progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Minimum = 0);

                listView1.SafeControlInvoke(listView =>
                {
                    foreach (ListViewItem items in listView1.Items)
                    {
                        baris++;
                        progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Value++);
                        xlWorkSheet.Cells[baris, 1] = items.SubItems[0].Text;
                        xlWorkSheet.Cells[baris, 2] = items.SubItems[1].Text;
                        xlWorkSheet.Cells[baris, 3] = items.SubItems[2].Text;
                        xlWorkSheet.Cells[baris, 4] = items.SubItems[3].Text;
                        xlWorkSheet.Cells[baris, 5] = items.SubItems[4].Text;
                    }
                });
                xlApp.AlertBeforeOverwriting = false;
                xlApp.DisplayAlerts = false;
                xlWorkBook.SaveAs(textBox4.Text, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan penulisan excel\nPesan kesalahan : " + ex.Message, "PERHATIAN");
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
            statusClose = 0;
            disableButton(1);
            MessageBox.Show("File Tercetak", "KONFIRMASI");
        }

        private void disableButton(int statusButton)
        {
            if (statusButton == 1)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                bTampil.Enabled = true;
                listView1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                button2.Enabled = false;
                bTampil.Enabled = false;
                listView1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox4.Text.Trim()))
            {
                var fi = new FileInfo(textBox4.Text);
                if (fi.Exists)
                {
                    if (listView1.Items.Count > 0)
                    {
                        if (!backgroundWorker2.IsBusy)
                        {
                            disableButton(0);
                            backgroundWorker2.RunWorkerAsync();
                        }
                    }
                    else
                        MessageBox.Show("Tidak ada data yg dicetak\nHarap tampilkan data", "PERHATIAN");
                }
                else
                    MessageBox.Show("Pilih file excel yg Benar\nFile Excel yg didukung adalah excel 2003", "PERHATIAN");
            }
            else
                MessageBox.Show("Pilih file excel yg Benar\nFile Excel yg didukung adalah excel 2003", "PERHATIAN");
        }

        private void fCekSaldoPPTK_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (statusClose == 1)
            {
                e.Cancel = true;
                MessageBox.Show("Masih Ada Proses berjalan harap tunggu", "PERHATIAN");
            }
            else
            {
                foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                {
                    if (proc.MainWindowTitle == "")
                        proc.Kill();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox4.Text = openFileDialog1.FileName;
            }
        }
    }
}
