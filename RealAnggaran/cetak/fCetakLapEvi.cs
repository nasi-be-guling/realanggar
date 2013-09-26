using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Globalization;
using System.Data.SqlClient;
using System.IO;

namespace RealAnggaran.cetak
{
    public partial class fCetakLapEvi : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader reader = null;
        List<ListViewItem> lvItemGroup = new List<ListViewItem>();
        string query = null;
        int statusClose = 0;

        public fCetakLapEvi()
        {
            koneksi = konek.KonekDb();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            listView1.SafeControlInvoke(listView => listView1.Items.Clear());
            query = "SELECT a.TglBayar, a.NoBayar, d.KdKpa, a.ketBayar,  c.Kd_Reken + '.' + c.formatPanjang, ' ' AS Expr1, " +
                      "a.JmlRp, ' ' AS Expr2, a.PPnRp, a.PPhRp, b.kdSumber, a.PPhNo, " + 
                      "a.NTPNPPn, a.NTPNPPh " +
                      "FROM REALANGGAR..A_PENGELUARAN a INNER JOIN " +
                      "REALANGGAR..A_SUMBER_DANA b ON b.idSumber = a.idSumber INNER JOIN " +
                      "REALANGGAR..A_REKENING c ON c.Id_Reken = a.Id_Reken INNER JOIN " +
                      "REALANGGAR..A_KPA d ON d.Id_Kpa = a.Id_Kpa " +
                      "WHERE a.TglBayar BETWEEN CONVERT(DATETIME, '" + string.Format("{0:yyyy-MM-dd}", dateTimePicker1.Value) +
                      " 00:00:00', 121) AND CONVERT(DATETIME, '" + string.Format("{0:yyyy-MM-dd}", dateTimePicker2.Value) + " 23:59:59', 121) " +
                      "ORDER BY a.TglBayar ASC";
            koneksi.Open();
            reader = konek.MembacaData(query, koneksi);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    listView1.SafeControlInvoke(listView =>
                        {
                            //listView1.Items.Add("");
                            listView1.Items.Add(alat.PengecekField(reader, 0));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 1));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 2));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 3));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 4));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 5));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 6));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 7));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 8));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 9));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 10));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 11));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 12));
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(alat.PengecekField(reader, 13));
                        });
                }
                reader.Close();
            }
            koneksi.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            disableButton(0);
            string filePath = "";
            if (this.folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = folderBrowserDialog1.SelectedPath + "\\Laporan_" + string.Format("{0:dd_MM_yyyy}", DateTime.Now) + ".xls";
                filePath = filePath.Replace(@"\\", @"\");

                textBox1.Text = filePath;

                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                Excel.Range chartRange;

                xlApp = new Excel.ApplicationClass();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                //add data 
                xlWorkSheet.Cells[1, 1] = "No Urut";
                xlWorkSheet.Cells[1, 2] = "Tgl Bayar";
                xlWorkSheet.Cells[1, 3] = "No Bukti";
                xlWorkSheet.Cells[1, 4] = "KPA";
                xlWorkSheet.Cells[1, 5] = "Ket bayar";
                xlWorkSheet.Cells[1, 6] = "No rekening";
                xlWorkSheet.Cells[1, 7] = "penerimaan";
                xlWorkSheet.Cells[1, 8] = "pengeluaran";
                xlWorkSheet.Cells[1, 9] = "sisa";
                xlWorkSheet.Cells[1, 10] = "ppn";
                xlWorkSheet.Cells[1, 11] = "pph";
                xlWorkSheet.Cells[1, 12] = "f_df";
                xlWorkSheet.Cells[1, 13] = "no ppn";
                xlWorkSheet.Cells[1, 14] = "NTPNPPn";
                xlWorkSheet.Cells[1, 15] = "NTPNPPh";

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
                chartRange = xlWorkSheet.get_Range("a1", "o1");
                chartRange.Font.Bold = true;
                chartRange = xlWorkSheet.get_Range("a1", "o1");
                chartRange.Font.Size = 12;
                xlApp.AlertBeforeOverwriting = false;
                xlApp.DisplayAlerts = false;
                xlWorkBook.SaveAs(filePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlApp);
                releaseObject(xlWorkBook);
                releaseObject(xlWorkSheet);

                MessageBox.Show("File Tercetak", "KONFIRMASI");
            }
            disableButton(1);
        }

        private void disableButton(int statusButton)
        {

            if (statusButton == 1)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                listView1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                listView1.Enabled = false;
            }
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (!string.IsNullOrEmpty(textBox1.Text.Trim()))
                {
                    var fi = new FileInfo(textBox1.Text);
                    if (fi.Exists)
                    {
                        if (!backgroundWorker2.IsBusy)
                        {
                            disableButton(0);
                            backgroundWorker2.RunWorkerAsync();
                        }
                    }
                    else
                        MessageBox.Show("File Excel tidak ditemukan!\nSilahkan klik template dan arahkan " +
                    "ke file template tersebut\nLebih lanjut hubungi 1062", "PERHATIAN");
                }
                else
                    MessageBox.Show("Silahkan pilih file yg akan dieksekusi!", "PERHATIAN");
            }
            else
                MessageBox.Show("Silahkan Pilih Baris yg akan dicetak", "PERHATIAN");
        }

        private int cekBaris(Excel.Worksheet xlWorkSheet)
        {
            //Excel.Application xlApp;
            //Excel.Workbook xlWorkBook;
            //Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            int rCnt = 0;
            int baris = 0;

            //object misValue = System.Reflection.Missing.Value;

            //xlApp = new Excel.ApplicationClass();
            //xlWorkBook = xlApp.Workbooks.Open(textBox1.Text, 0,
            //    true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,
            //    "\t", false, false, 0, true, 1, 0);
            //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            for (rCnt = 1; rCnt <= range.Rows.Count; rCnt++) //Baris
            {
                baris++;
            }
            return baris;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                disableButton(0);
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            disableButton(1);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
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
                xlWorkBook = xlApp.Workbooks.Open(textBox1.Text, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1); if there is no existing excel file
                xlSheets = xlWorkBook.Worksheets;
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.get_Item("Sheet1");
                Microsoft.Office.Interop.Excel.Range range = xlWorkSheet.UsedRange;
                //xlWorkSheet.Cells[2, 2] = "http://csharp.net-informations.com"; if there is no existing excel file

                //int colCount = range.Columns.Count;
                //int rowCount = range.Rows.Count;
                //for (int index = 1; index < 20; index++)
                //{
                //    xlWorkSheet.Cells[rowCount + index, 1] = rowCount + index;
                //    xlWorkSheet.Cells[rowCount + index, 2] = "New Item" + index;
                //}
                int baris = 1; //cari cara menghitung jumlah baris yg telah ditulis di excel
                if (cekBaris(xlWorkSheet) > 1)
                    baris = cekBaris(xlWorkSheet);
                progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Maximum = listView1.SelectedItems.Count + 1);
                progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Minimum = 0);

                listView1.SafeControlInvoke(listView =>
                    {
                        foreach (ListViewItem items in listView1.SelectedItems)
                        {
                            baris++;
                            progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Value++);
                            xlWorkSheet.Cells[baris, 1] = " ";
                            xlWorkSheet.Cells[baris, 2] = items.SubItems[0].Text;
                            xlWorkSheet.Cells[baris, 3] = items.SubItems[1].Text;
                            xlWorkSheet.Cells[baris, 4] = items.SubItems[2].Text;
                            xlWorkSheet.Cells[baris, 5] = items.SubItems[3].Text;
                            xlWorkSheet.Cells[baris, 6] = items.SubItems[4].Text;
                            xlWorkSheet.Cells[baris, 7] = items.SubItems[5].Text;
                            xlWorkSheet.Cells[baris, 8] = items.SubItems[6].Text;
                            xlWorkSheet.Cells[baris, 9] = items.SubItems[7].Text;
                            xlWorkSheet.Cells[baris, 10] = items.SubItems[8].Text;
                            xlWorkSheet.Cells[baris, 11] = items.SubItems[9].Text;
                            xlWorkSheet.Cells[baris, 12] = items.SubItems[10].Text;
                            xlWorkSheet.Cells[baris, 13] = items.SubItems[11].Text;
                            xlWorkSheet.Cells[baris, 14] = items.SubItems[12].Text;
                            xlWorkSheet.Cells[baris, 15] = items.SubItems[13].Text;
                        }
                    });
                xlApp.AlertBeforeOverwriting = false;
                xlApp.DisplayAlerts = false;
                xlWorkBook.SaveAs(textBox1.Text, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
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

        private void fCetakLapEvi_FormClosing(object sender, FormClosingEventArgs e)
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


        private void fCetakLapEvi_Load(object sender, EventArgs e)
        {
            foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
            {
                if (proc.MainWindowTitle == "")
                proc.Kill();
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(this.listView1, e.Location);
            }
        }

        private void pilihSemuaDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count > 0)
            {
                foreach (ListViewItem i in listView1.Items)
                {
                    i.Selected = true;
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
