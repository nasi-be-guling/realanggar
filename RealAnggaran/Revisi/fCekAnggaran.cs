using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;

namespace RealAnggaran.Revisi
{
    public partial class fCekAnggaran : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader reader = null;
        private List<tampungAnggaran> tabelHasilAnggaran;
        List<ListViewItem> lvItemGroup = new List<ListViewItem>();
        string query = null;
        int status = 0;
        int statusSort = 0;

        public fCekAnggaran()
        {
            koneksi = konek.KonekDb();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        private class tampungAnggaran
        {
            //Alternatif buat class dengan atau tanpa constructor
            public tampungAnggaran(string idRek, decimal totalAnggaran, decimal totalBelanja,
                decimal anggaranMinusBelanja)
            {
                this.idRek = idRek;                                    // Jika tidak menggunakan constructor
                this.totalAnggaran = totalAnggaran;                                     // hapus pada baris yg di comment
                this.totalBelanja = totalBelanja;
                this.anggaranMinusBelanja = anggaranMinusBelanja;
            }
            public string idRek { set; get; }
            public decimal totalAnggaran { set; get; }
            public decimal totalBelanja { set; get; }
            public decimal anggaranMinusBelanja { set; get; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (timer1.Enabled == false)
            //    timer1.Enabled = true;
            //else if (timer1.Enabled == true)
            //    timer1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            query = "SELECT B.Id_Rinci_Rs, B.Tot_Angkas, COALESCE(SUM(A.TFungsi) + SUM(A.TSubsi),0) AS 'Total Belanja', " +
		        "COALESCE(B.Tot_Angkas - (SUM(A.TFungsi) + SUM(A.TSubsi)),0) 'Total Anggaran - Total Belanja' " +
                "FROM KASDA..BLJ_DETAIL A RIGHT OUTER JOIN " +
                "KASDA..ANGKAS_DTL B ON A.Id_Rinci_Rs = B.Id_Rinci_Rs " +
                "GROUP BY B.Id_Rinci_Rs, B.Tot_Angkas";
            string queryBuatCount = "SELECT     COUNT(*) " +
                "FROM         KASDA..AKD_RINCIAN";
            progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Visible = true);
            progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Minimum = 0);
            List<tampungAnggaran> grupAnggaran = new List<tampungAnggaran>();
            koneksi.Open();
            reader = konek.MembacaData(queryBuatCount, koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                int jumlahRecord = Convert.ToInt16(alat.PengecekField(reader, 0));
                reader.Close();
                progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Maximum = jumlahRecord + 1);
                //MessageBox.Show((jumlahRecord).ToString());
                reader = konek.MembacaData(query, koneksi);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        grupAnggaran.Add(new tampungAnggaran(alat.PengecekField(reader, 0), Convert.ToDecimal(alat.PengecekField(reader, 1)), 
                            Convert.ToDecimal(alat.PengecekField(reader, 2)), Convert.ToDecimal(alat.PengecekField(reader, 3))));
                        //MessageBox.Show(Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", reader[2])).ToString());
                        progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Value++);
                        //MessageBox.Show(String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(alat.pengecekField(reader, 2))));
                    }
                }
            }
            koneksi.Close();
            e.Result = grupAnggaran;
        }
        
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tabelHasilAnggaran = (List<tampungAnggaran>)e.Result;
            showList();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].SubItems[3].Text.Contains("-"))
                {
                    listView1.Items[i].BackColor = Color.Red;
                }
            }
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 0;
            progressBar1.Visible = false;
        }

        private void showList()
        {
            foreach (var i in tabelHasilAnggaran)
            {
                ListViewItem item = new ListViewItem(i.idRek);
                item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}" ,i.totalAnggaran));
                item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.totalBelanja));
                item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.anggaranMinusBelanja));
                lvItemGroup.Add(item);
            }
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Items.AddRange(lvItemGroup.ToArray());
            //for (int i = 0; i <= listView1.Items.Count; i++)
            //{
            //    if (listView1.Items[i].Text == "Y")
            //    {
            //        listView1.Items[1].Text = "asd";
            //    )
            //}
            listView1.EndUpdate();
            lvItemGroup.Clear();
        }

        private void fCekAnggaran_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private string filterRupiah(string rupiah)
        {
            rupiah = rupiah.Replace("Rp. ", "");
            rupiah = rupiah.Replace(".", "");
            rupiah = rupiah.Replace(",", ".");
            return rupiah;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            contextMenuStrip1.SafeControlInvoke(conteks => contextMenuStrip1.Enabled = false);
            progressBar1.SafeControlInvoke(ProgressBar =>
            {
                progressBar1.Visible = true;
                progressBar1.Maximum = listView1.SafeControlInvoke(listView => listView1.Items.Count + 1);
            }
            );
            IEnumerable<tampungAnggaran> namaKpa = null;
            List<tampungAnggaran> grupAnggaran = new List<tampungAnggaran>();
            listView1.SafeControlInvoke(
                listView =>
                {
                    foreach (ListViewItem item in listView1.Items)
                    {
                        grupAnggaran.Add(new tampungAnggaran(item.SubItems[0].Text, Convert.ToDecimal(filterRupiah(item.SubItems[1].Text)),
                                        Convert.ToDecimal(filterRupiah(item.SubItems[2].Text)), Convert.ToDecimal(filterRupiah(item.SubItems[3].Text))));
                        //MessageBox.Show(item.SubItems[0].Text + " " + filterRupiah(item.SubItems[1].Text) + " " +
                        //                filterRupiah(item.SubItems[2].Text) + " " + filterRupiah(item.SubItems[3].Text));
                    }
                }
            );
            if (statusSort == 1)
            {
                namaKpa = (from s in grupAnggaran
                           orderby s.idRek //descending
                           select s);
            }
            else if (statusSort == 2)
            {
                namaKpa = (from s in grupAnggaran
                           orderby s.totalAnggaran //descending
                           select s);
            }
            else if (statusSort == 3)
            {
                namaKpa = (from s in grupAnggaran
                           where s.idRek == textBox1.Text //condition
                           select s);
            }
            foreach (var i in namaKpa)
            {
                if (string.IsNullOrEmpty(i.idRek))
                {
                    MessageBox.Show("Test");
                }
                else
                {
                    ListViewItem item = new ListViewItem(i.idRek);
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.totalAnggaran));
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.totalBelanja));
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.anggaranMinusBelanja));
                    lvItemGroup.Add(item);
                    progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Value++);
                }
            }
            listView1.SafeControlInvoke(
                listView =>
                {
                    listView1.BeginUpdate();
                    listView1.Items.Clear();
                    listView1.Items.AddRange(lvItemGroup.ToArray());
                    listView1.EndUpdate();
                    lvItemGroup.Clear();
                }
            );
            //e.Result = grupAnggaran;
        }

        private void sortUrutanEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusSort = 1;
            if (!backgroundWorker2.IsBusy)
                backgroundWorker2.RunWorkerAsync();
        }

        private void sortNoSPKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusSort = 2;
            if (!backgroundWorker2.IsBusy)
                backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //tabelKasdaFilter = (List<tampungKasda>)e.Result;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 0;
            progressBar1.Visible = false;
            statusSort = 0;
            contextMenuStrip1.Enabled = true;
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (listView1.Items.Count > 0)
            {
                //itemSelected = 1;
                contextMenuStrip1.Enabled = true;
            }
            else
            {
                //itemSelected = 0;
                contextMenuStrip1.Enabled = false;
            }
            if (!backgroundWorker2.IsBusy)
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(this.listView1, e.Location);
                }
            }
        }

        private void fCekAnggaran_KeyDown(object sender, KeyEventArgs e)
        {
            // =====================================================================================================================
            // =================== EXAMPLE: mengambil data langsung dari listview dgn menggunakan LINQ =============================
            // =====================================================================================================================

            if (e.KeyData == Keys.Enter)
            {
                //IEnumerable<ListViewItem> lv = listView1.Items.Cast<ListViewItem>();
                //var items = (from s in lv
                //             where s.SubItems[0].Text == textBox1.Text
                //             select s);
                //foreach (var i in items)
                //{
                //    MessageBox.Show(i.SubItems[0].Text + " " + i.SubItems[1].Text + " " + i.SubItems[2].Text + " " + i.SubItems[3].Text);
                //}
                statusSort = 3;
                showData();
            }
        }

        private void showData()
        {
            //tabelKasda = refinery();
            //IEnumerable<tampungKasda> namaKpa = (from s in tabelKasda where s.subsi == 0 select s);
            try
            {
                IEnumerable<tampungAnggaran> namaKpa = null;
                if (statusSort == 3)
                {
                    namaKpa = (from s in tabelHasilAnggaran
                               where s.idRek.Contains(textBox1.Text) //condition
                               select s);
                }

                foreach (var i in namaKpa)
                {
                    ListViewItem item = new ListViewItem(i.idRek);
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.totalAnggaran));
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.totalBelanja));
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.anggaranMinusBelanja));
                    lvItemGroup.Add(item);
                }
                listView1.BeginUpdate();
                listView1.Items.Clear();
                listView1.Items.AddRange(lvItemGroup.ToArray());
                listView1.EndUpdate();
                lvItemGroup.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan : " + ex.Message + "\nSilahkan hubungi 1062");
            }

        }
    }
}
