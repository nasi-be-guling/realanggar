using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;
using System.Collections;

namespace RealAnggaran.Revisi
{
    public partial class fSubsidi : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader reader = null;
        string query = null;
        List<ListViewItem> lvItemGroup = new List<ListViewItem>();
        private List<tampungKasda> tabelKasda;
        private List<tampungKasda> tabelKasdaFilter;
        int statusShowData = 0;
        private List<tampungKpa> tabelKpa;
        int idKPA = 0;
        int statusSimpan = 0;
        int rowCount = 0;
        int itemSelected = 0; // ini buat ngecek apa listViewItem udah dipilih | belum
        int statusSort = 0;
        public int idOpp = 0;
        //khusus buat koreksiSubsidi
        koreksiSubsidi formKoreksiSubsidi;
        public fSubsidi()
        {
            koneksi = konek.KonekDb();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "dd/MM/yyyy";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker3.CustomFormat = "dd/MM/yyyy";
            dateTimePicker3.Format = DateTimePickerFormat.Custom;
            tabelKpa = fillComboKPA();
        }
        private class tampungKpa
        {
            //Alternatif buat class dengan atau tanpa constructor
            public tampungKpa(string idKpa, string kdKpa, string namaKpa) //
            {                                                             // Jika tidak menggunakan constructor      
                this.idKpa = idKpa;                                       // hapus pada baris yg di comment 
                this.kdKpa = kdKpa;                                       //  
                this.namaKpa = namaKpa;                                   //  
            }
            public string idKpa { set; get; }
            public string kdKpa { set; get; }
            public string namaKpa { set; get; }
        }
        private List<tampungKpa> fillComboKPA()
        {
            comboBox1.Items.Clear();
            List<tampungKpa> grupSetting = new List<tampungKpa>();

            koneksi.Open();
            reader = konek.MembacaData("SELECT * FROM A_KPA", koneksi);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    comboBox1.Items.Add(alat.PengecekField(reader, 1));
                    //tampungKpa itemKpa = new tampungKpa();
                    //itemKpa.idKpa = alat.pengecekField(reader, 0);
                    //itemKpa.kdKpa = alat.pengecekField(reader, 1);
                    //itemKpa.namaKpa = alat.pengecekField(reader, 2);
                    //grupSetting.Add(itemKpa);
                    // Alternatif add item ke lis, jika menggunakan deklarasi class langsung, 
                    // constructor class yg digunakan harus menggandung
                    grupSetting.Add(new tampungKpa(alat.PengecekField(reader, 0), alat.PengecekField(reader, 1), alat.PengecekField(reader, 2)));
                }
                reader.Close();
            }
            koneksi.Close();
            comboBox1.Items.Add("99.NN");
            return grupSetting;
        }
        private void disabler(int statusDisabler)
        {
            if (statusDisabler == 0)
            {
                button1.SafeControlInvoke(
                    Button =>
                    {
                        button1.Enabled = false;
                    }
                    );
                button2.SafeControlInvoke(
                    Button =>
                    {
                        button2.Enabled = false;
                    }
                    );
                button2.SafeControlInvoke(
                    Button =>
                    {
                        button3.Enabled = false;
                    }
                    );
            }
            else if (statusDisabler == 1)
            {
                button1.SafeControlInvoke(
                    Button =>
                    {
                        button1.Enabled = true;
                    }
                    );
                button2.SafeControlInvoke(
                    Button =>
                    {
                        button2.Enabled = true;
                    }
                    );
                button2.SafeControlInvoke(
                    Button =>
                    {
                        button3.Enabled = true;
                    }
                    );
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //MessageBox.Show(label12.Text.Length.ToString());
            //MessageBox.Show(label12.Text.Substring(20, (label12.Text.Length - 22)));
            int counter = 6;
            for (int i = 23; i > label12.Text.Length; i++)
            {
                counter++;
            }
            int subStringLength = label12.Text.Length - 22;
            int currentNumber = Convert.ToInt16(label12.Text.Substring(20, subStringLength));
            string currentCode = label12.Text.Substring(14, counter);
            //MessageBox.Show(label12.Text.Substring(14, (label12.Text.Length - 16)));
            //MessageBox.Show(label12.Text.Substring(25, subStringLength));
            disabler(0);
            koneksi.Open();
            SqlTransaction transaksi = koneksi.BeginTransaction();
            listView1.SafeControlInvoke(
                listView =>
                {
                    if (listView1.SelectedItems.Count > 0)
                    {
                        try
                        {
                            MessageBox.Show("Transaksi dimulai");
                            // yg ini kye cocok buat implement export to excel
                            //var qry = from i in listView1.Items.Cast<ListViewItem>()
                            //          from si in i.SubItems.Cast<System.Windows.Forms.ListViewItem.ListViewSubItem>()
                            //          select si.Text;
                            //foreach (string a in qry)
                            //{
                            //    MessageBox.Show(a.ToString());
                            //}
                            foreach (ListViewItem i in listView1.SelectedItems)
                            {
                                //konek.masukkanData("insert into dumies..dummy_1 values ('" + i.Text + "','" + 
                                //    i.SubItems[1].Text + "','" + i.SubItems[2].Text + "')", koneksi, transaksi);
                                //if (konek.cekFieldUnikNoKoneksi("KASDA.dbo.BLJ_MASTER", "No_Bukti", currentCode + (currentNumber++).ToString() + "/S", koneksi) == false)
                                //{
                                konek.MasukkanData(@"UPDATE KASDA.dbo.BLJ_MASTER " +
                                    "SET No_Bukti = '" + currentCode + (currentNumber++).ToString() + "/S" + "', Lunas = 'Y', TGL_SPJ = CONVERT(SMALLDATETIME, '" + dateTimePicker3.Text.Trim() +
                                    "', 103) WHERE idBlj_master = '" + i.Text.Trim() + "'", koneksi, transaksi);
                                //MessageBox.Show((currentCode) + (currentNumber++).ToString() + "/S");
                                //MessageBox.Show(label12.Text.Substring(14, 11) + (currentNumber++).ToString());
                                //}
                                //else
                                //    transaksi.Rollback();
                            }
                            transaksi.Commit();

                            MessageBox.Show("Data tersimpan");
                        }
                        catch (SqlException sqlEx)
                        {
                            transaksi.Rollback();
                            MessageBox.Show("Terjadi kesalahan : " + sqlEx.Message + " \nSilahkan hubungi 1062");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Silahkan Pilih Transaksi yang akan dilunasi", "PERHATIAN");
                    }
                }
                );
            koneksi.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            statusShowData = 0;
            backgroundWorker2.RunWorkerAsync();
            fillComboKPA();
        }
        private class tampungKasda
        {
            //Alternatif buat class dengan atau tanpa constructor
            public tampungKasda(string idSPK, string NoSPK, DateTime tglSP, 
                string kodeRek, string uraian, string ket, decimal subsi, 
                decimal fungsi, string lunas, string noBukti, string noRek,
                string noRekPanjang, string noRekanan, DateTime tglSPJ)
            {                                                                    
                this.idSPK = idSPK;                                    // Jika tidak menggunakan constructor
                this.NoSPK = NoSPK;                                     // hapus pada baris yg di comment
                this.tglSP = tglSP;                         
                this.kodeRek = kodeRek;
                this.uraian = uraian;
                this.ket = ket;
                this.subsi = subsi; 
                this.fungsi = fungsi;
                this.lunas = lunas;
                this.noBukti = noBukti;
                this.noRek = noRek;
                this.noRekPanjang = noRekPanjang;
                this.noRekanan = noRekanan;
                this.tglSPJ = tglSPJ;
            }
            public string idSPK {set; get;}
            public string NoSPK {set; get;}
            public DateTime tglSP {set; get;}
            public string kodeRek {set; get;}
            public string uraian {set; get;}
            public string ket {set; get;}
            public decimal subsi {set; get;}
            public decimal fungsi {set; get;}
            public string lunas {set; get;}
            public string noBukti {set; get;}
            public string noRek {set; get;}
            public string noRekPanjang {set; get;}
            public string noRekanan { set; get; }
            public DateTime tglSPJ { set; get; }
        }
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Visible = true);
            progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Minimum = 0);
            progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Maximum = 0);
            disabler(0);
            List<tampungKasda> grupKasda = new List<tampungKasda>();
            koneksi.Open();
            reader = konek.MembacaData("SELECT COUNT(A.IdBlj_Master) AS Expr1 " +
                "FROM KASDA..AKD_RINCIAN AS C INNER JOIN " +
                "KASDA..BLJ_DETAIL AS B ON C.Id_Rinci_RS = B.Id_Rinci_Rs INNER JOIN " +
                "KASDA..BLJ_MASTER AS A ON B.IdBlj_Master = A.IdBlj_Master", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                int jumlahRecord = Convert.ToInt16(alat.PengecekField(reader, 0));
                reader.Close();
                progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Maximum = jumlahRecord + 2);
                reader = konek.MembacaData("SELECT A.IdBlj_Master, A.No_SPK, A.Tgl_SP, C.Id_Rinci_Rs, C.Uraian, A.Keter, B.TSubsi, B.TFungsi, " + // US REGION
                    "A.Lunas, A.No_bukti, D.IdAngkas, C.Kode_jenis, C.Kode_Obyek, C.Kode_Kelompok, C.idKtg_blj, C.formatPanjang, A.IdSupplier, A.tgl_spj " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, KASDA..AKD_RINCIAN C, KASDA..ANGKAS_DTL D " +
                    "WHERE D.Id_Rinci_Rs = B.Id_Rinci_RS AND C.Id_Rinci_RS = B.Id_Rinci_RS AND B.IdBlj_Master = A.IdBlj_Master", koneksi);
                //reader = konek.membacaData("SELECT A.IdBlj_Master, A.No_SPK, CONVERT(VARCHAR(20), A.Tgl_SP, 103), C.Id_Rinci_Rs, C.Uraian, A.Keter, B.TSubsi, B.TFungsi, " + // ID REGION
                //    "A.Lunas, A.No_bukti, D.IdAngkas, C.Kode_jenis, C.Kode_Obyek, C.Kode_Kelompok, C.idKtg_blj, C.formatPanjang, A.IdSupplier " +
                //    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, KASDA..AKD_RINCIAN C, KASDA..ANGKAS_DTL D " +
                //    "WHERE D.Id_Rinci_Rs = B.Id_Rinci_RS AND C.Id_Rinci_RS = B.Id_Rinci_RS AND B.IdBlj_Master = A.IdBlj_Master", koneksi);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        grupKasda.Add(new tampungKasda(alat.PengecekField(reader, 0), alat.PengecekField(reader, 1), Convert.ToDateTime(reader[2]), //US REGION
                            alat.PengecekField(reader, 3), alat.PengecekField(reader, 4), alat.PengecekField(reader, 5), Convert.ToDecimal(alat.PengecekField(reader, 6)),
                            Convert.ToDecimal(alat.PengecekField(reader, 7)), alat.PengecekField(reader, 8), alat.PengecekField(reader, 9), alat.PengecekField(reader, 10) + "." +
                            alat.PengecekField(reader, 11) + "." + alat.PengecekField(reader, 12) + "." + alat.PengecekField(reader, 13) + "." +
                            alat.PengecekField(reader, 14), alat.PengecekField(reader, 15), alat.PengecekField(reader, 16), Convert.ToDateTime(reader[17])));
                        //MessageBox.Show(Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", reader[2])).ToString());
                        progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Value++);
                        //MessageBox.Show(String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(alat.pengecekField(reader, 2))));
                    }
                    //listView1.SafeControlInvoke(
                    //    listView =>
                    //    {
                    //        listView1.BeginUpdate();
                    //        listView1.Items.Clear();
                    //        listView1.Items.AddRange(lvItemGroup.ToArray());
                    //        listView1.EndUpdate();
                    //    }
                    //);
                }
            }
            koneksi.Close();
            e.Result = grupKasda;
        }
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 0;
            progressBar1.Visible = false;
            tabelKasda = (List<tampungKasda>)e.Result;
            disabler(1);
            statusSimpan = 0;
            label1.Text = "Jumlah Transaksi : ";
            label12.Text = "Nomor Bukti : ";
        }
        private void fSubsidi_Load(object sender, EventArgs e)
        {
            //cordCounter();
            backgroundWorker2.RunWorkerAsync();
            if (!backgroundWorker2.IsBusy)
                ShowData();
        }
        private void fSubsidi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                label1.Text = "Jumlah Transaksi : " + Convert.ToString(listView1.Items.Count);
            }
            //if (e.KeyData == Keys.Enter)
            //{
            //    showData();
            //}
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //int counter = 1;
            //label1.Text = "progess " + Convert.ToString(counter++);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            disabler(1);
            comboBox1.Text = "";
            label12.Text = "Nomor Bukti :";
            itemSelected = 0;
        }

        private bool cekFungORSubs()
        {
            bool status = true;
            foreach (ListViewItem i in listView1.SelectedItems)
            {
                if (int.Parse(i.SubItems[7].Text.Substring(4, 1)) > 0)
                    status = false;
            }
            return status;
        }

        private bool cekLunas()
        {
            bool status = true;
            foreach (ListViewItem i in listView1.SelectedItems)
            {
                if (i.SubItems[8].Text == "Y")
                    status = false;
            }
            return status;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (statusSimpan == 0)
            {
                MessageBox.Show(@"Tidak dapat menciptakan No Bukti \nSilahkan pilih KPA", @"PERHATIAN");
                comboBox1.Focus();
            }
            else if (cekFungORSubs() == false)
            {
                MessageBox.Show(@"Pelunasan Fungsional Dilakukan oleh Bagian Perbendaharaan", @"PERHATIAN");
            }
            else if (cekLunas() == false)
            {
                MessageBox.Show(@"Data Telah Dilunasi\n Jika ingin melakukan koreksi, Silahkan klik ganda data", @"PERHATIAN");
            }
            else if (itemSelected == 0 | itemSelected < 1)
            {
                MessageBox.Show(@"Silahkan Pilih Data yang akan dilunasi!", @"PERHATIAN");
            }
            else
                //MessageBox.Show("boleh");
                backgroundWorker1.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            //tabelKasda = refinery();
            //IEnumerable<tampungKasda> namaKpa = (from s in tabelKasda where s.subsi == 0 select s);
            try
            {
                IEnumerable<tampungKasda> namaKpa = null;
                if (statusShowData == 0) // tampilkan semua
                {
                    namaKpa = (from s in tabelKasda select s);
                }
                else if (statusShowData == 1) // tampilkan berdasarkan tanggal
                {
                    namaKpa = (from s in tabelKasda
                               where s.tglSP >= Convert.ToDateTime(dateTimePicker1.Value)
                                   && s.tglSP <= Convert.ToDateTime(dateTimePicker2.Value)
                               select s);
                }
                else if (statusShowData == 2) // tampilkan berdasarkan nominal
                {
                    if (checkBox1.Checked && string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
                    {
                        namaKpa = (from s in tabelKasda
                                   where s.fungsi == 0
                                       && s.tglSP >= Convert.ToDateTime(dateTimePicker1.Value)
                                       && s.tglSP <= Convert.ToDateTime(dateTimePicker2.Value)
                                   select s);
                    }
                    else
                    {
                        namaKpa = (from s in tabelKasda
                                   where s.subsi > Convert.ToDecimal(textBox1.Text) - 1
                                       && s.subsi < Convert.ToDecimal(textBox2.Text) + 1
                                       && s.tglSP >= Convert.ToDateTime(dateTimePicker1.Value)
                                       && s.tglSP <= Convert.ToDateTime(dateTimePicker2.Value)
                                   select s);
                    }
                }
                else if (statusShowData == 3) // tampilkan berdasarkan nominal
                {
                    if (checkBox2.Checked && string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
                    {
                        namaKpa = (from s in tabelKasda
                                   where s.subsi == 0
                                       && s.tglSP >= Convert.ToDateTime(dateTimePicker1.Value)
                                       && s.tglSP <= Convert.ToDateTime(dateTimePicker2.Value)
                                   select s);
                    }
                    else
                    {
                        namaKpa = (from s in tabelKasda
                                   where s.fungsi > Convert.ToDecimal(textBox1.Text) - 1
                                       && s.fungsi < Convert.ToDecimal(textBox2.Text) + 1
                                       && s.tglSP >= Convert.ToDateTime(dateTimePicker1.Value)
                                       && s.tglSP <= Convert.ToDateTime(dateTimePicker2.Value)
                                   select s);
                    }
                }
                else if (statusShowData == 4) // tampilkan berdasarkan pelunasan
                {
                    if (radioButton2.Checked)
                    {
                        namaKpa = (from s in tabelKasda
                                   where s.lunas.Contains("Y") 
                                   && s.tglSP >= Convert.ToDateTime(dateTimePicker1.Value)
                                   && s.tglSP <= Convert.ToDateTime(dateTimePicker2.Value)
                                   select s);
                    }
                    else
                    {
                        namaKpa = (from s in tabelKasda
                                   where !s.lunas.Contains("Y")
                                   && s.tglSP >= Convert.ToDateTime(dateTimePicker1.Value)
                                   && s.tglSP <= Convert.ToDateTime(dateTimePicker2.Value)
                                   select s);
                    }
                }
                else if (statusShowData == 5) // tampilkan berdasarkan no spk
                {
                    //namaKpa = (from s in tabelKasda
                    //            where s.NoSPK == textBox3.Text || s.noBukti == textBox3.Text
                    //            select s);
                    namaKpa = (from s in tabelKasda
                               where s.NoSPK.ToUpper().Contains(textBox3.Text.ToUpper())
                               select s);
                }
                foreach (var i in namaKpa)
                {
                    ListViewItem item = new ListViewItem(i.idSPK);
                    item.SubItems.Add(i.NoSPK);
                    item.SubItems.Add(string.Format("{0:MM/dd/yyyy}", i.tglSP));
                    item.SubItems.Add(i.kodeRek);
                    item.SubItems.Add(i.uraian);
                    item.SubItems.Add(i.ket);
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.subsi));
                    item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.fungsi));
                    item.SubItems.Add(i.lunas);
                    item.SubItems.Add(i.noBukti);
                    item.SubItems.Add(i.noRek);
                    item.SubItems.Add(i.noRekPanjang);
                    item.SubItems.Add(i.noRekanan);
                    item.SubItems.Add(string.Format("{0:MM/dd/yyyy}", i.tglSPJ));
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

        #region status
        private void dateTimePicker1_Enter(object sender, EventArgs e)
        {
            statusShowData = 1;
        }

        private void dateTimePicker2_Enter(object sender, EventArgs e)
        {
            statusShowData = 1;
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            statusShowData = 2;
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            statusShowData = 3;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            statusShowData = 4;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            statusShowData = 4;
        }
        #endregion

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.Trim() != "")
            {
                ArrayList listku = searchinListUsingLinq(comboBox1.Text, 0);
                //idKPA = Convert.ToInt16(listku[0].ToString());
                label12.Text = "Nomor Bukti : " + generateNoKwitansi();
                statusSimpan = 1;
            }
        }

        //private List<tampungKasda> refinery()
        //{
        //    disabler(0);
        //    List<tampungKasda> grupKasda = new List<tampungKasda>();
        //    koneksi.Open();
        //    reader = konek.membacaData("SELECT A.IdBlj_Master, A.No_SPK, A.Tgl_SP, C.Id_Rinci_Rs, C.Uraian, A.Keter, B.TSubsi, B.TFungsi, " +
        //        "A.Lunas, A.No_bukti " +
        //        "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, KASDA..AKD_RINCIAN C " +
        //        "WHERE C.Id_Rinci_RS = B.Id_Rinci_RS AND B.IdBlj_Master = A.IdBlj_Master", koneksi);
        //    if (reader.HasRows)
        //    {
        //        while (reader.Read())
        //        {
        //            grupKasda.Add(new tampungKasda(alat.pengecekField(reader, 0), alat.pengecekField(reader, 1), alat.pengecekField(reader, 2),
        //                alat.pengecekField(reader, 3), alat.pengecekField(reader, 4), alat.pengecekField(reader, 5), Convert.ToDecimal(alat.pengecekField(reader, 6)),
        //                Convert.ToDecimal(alat.pengecekField(reader, 7)), alat.pengecekField(reader, 8), alat.pengecekField(reader, 9)));
        //        }
        //        //listView1.SafeControlInvoke(
        //        //    listView =>
        //        //    {
        //        //        listView1.BeginUpdate();
        //        //        listView1.Items.Clear();
        //        //        listView1.Items.AddRange(lvItemGroup.ToArray());
        //        //        listView1.EndUpdate();
        //        //    }
        //        //);
        //    }
        //    koneksi.Close();
        //    return grupKasda;
        //}
        private ArrayList searchinListUsingLinq(string kd, int status)  // Status query 0 =  select KPA | 1 = select DF
        {
            /*  CONTOH LINQ PADA LIST<>
             * 
             *  (FROM inv IN invoicelist
             *  WHERE inv.customerid = 100 
             *  FROM od IN inv.OrderDetail
             *  FROM p in productList
             *  WHERE p.id = od.productid
             *  SELECT p).Distinct().ToList()
             * 
             * */
            // LINQ EXPRESSION
            // Buat select 1 item/cell/field:
            //string namaKpa = (from s in tabelKpa where s.kdKpa == kdKpa select s.namaKpa).Single();
            // Buat select 1 row:
            // Alternatif #1 linq dgn menggunakan var atau IEnumerable
            //var namaKpa = (from s in tabelKpa where s.kdKpa == kdKpa select s);
            ArrayList listSayanya = new ArrayList();
            if (status == 0)
            {
                IEnumerable<tampungKpa> namaKpa = (from s in tabelKpa where s.kdKpa == kd select s);
                // LAMDA EXPRESSION
                //#1.string namaKpa = tabelKpa.Single(p => p.idKpa == "1");
                //#2.Product product = db.Products.Single(p => p.ProductID == productID);
                foreach (var i in namaKpa)
                {
                    listSayanya.Add(i.idKpa);
                    listSayanya.Add(i.kdKpa);
                    listSayanya.Add(i.namaKpa);
                }
            }
            return listSayanya;
        }
        private string generateNoKwitansi()
        {
            string noBukti = null;
            //query = @"SELECT TOP 1 CONVERT(INT, SUBSTRING(REPLACE(REPLACE(REPLACE(no_bukti, '/', ''), CHAR(10), ''), CHAR(13), ''), 9, 10)) AS NOMER FROM " +
            //    "KASDA..blj_master A WHERE YEAR(A.Tgl_SP) = " + dateTimePicker3.Text.Substring(6, 4) + " AND SUBSTRING(no_bukti, 1, 2) = '" + comboBox1.Text.Right1(2) + "' ORDER BY NOMER DESC";
            query = @"SELECT TOP 1 CONVERT(INT, SUBSTRING(REPLACE(REPLACE(REPLACE(REPLACE(no_bukti, '/S', ''), CHAR(10), ''), CHAR(13), ''), '/', ''), 5, 2)) AS NOMER FROM " +
                "KASDA..blj_master A WHERE YEAR(A.Tgl_SP) = " + dateTimePicker3.Value.Year + " AND SUBSTRING(no_bukti, 1, 2) = '" + comboBox1.Text.Right1(2) + "' AND no_bukti like '%/s' " +
                "AND SUBSTRING(no_bukti, 4, 2) = '" + String.Format("{0:MM}", dateTimePicker3.Value) + "' ORDER BY NOMER DESC";
            //MessageBox.Show(query);
            koneksi.Open();
            reader = konek.MembacaData(query, koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                try
                {
                    noBukti = comboBox1.Text.Right1(2) + "/" + String.Format("{0:MM}", dateTimePicker3.Value) + "/" +
                            (Convert.ToInt16(alat.PengecekField(reader, 0)) + 1).ToString() + "/S";                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal Membuat nomor otomatis!\nFormat sebelumnya" +
                        " tidak sesuai kesepakatan\nLanjutkan dengan penomoran manual.", ex.Message);
                    koneksi.Close();
                }
                reader.Close();
            }
            else
            {
                noBukti = comboBox1.Text.Right1(2) + "/" + String.Format("{0:MM}", dateTimePicker3.Value) + "/1/S";
            }
            koneksi.Close();
            //MessageBox.Show(dateTimePicker1.Text.Substring(6, 4));
            //MessageBox.Show(comboBox1.Text + "/" + dateTimePicker1.Text.Substring(6, 4) + "/" + dateTimePicker1.Text.Substring(3, 2) + "/" + "1");
            return noBukti;
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listView1.SelectedItems[0].SubItems[1].Text);
        }
        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (listView1.Items.Count > 0)
            {
                itemSelected = 1;
                contextMenuStrip1.Enabled = true;
            }
            else
            {
                itemSelected = 0;
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
        private void textBox3_Enter(object sender, EventArgs e)
        {
            statusShowData = 5;
        }
        //private void hitungRecord()
        //{
        //    koneksi.Open();
        //    reader = konek.membacaData("SELECT A.IdBlj_Master, A.No_SPK, A.Tgl_SP, C.Id_Rinci_Rs, C.Uraian, A.Keter, B.TSubsi, B.TFungsi, " +
        //        "A.Lunas, A.No_bukti " +
        //        "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, KASDA..AKD_RINCIAN C " +
        //        "WHERE C.Id_Rinci_RS = B.Id_Rinci_RS AND B.IdBlj_Master = A.IdBlj_Master", koneksi);
        //    if (reader.HasRows)
        //    {
        //        while (reader.Read())
        //        {

        //        }
        //    }
        //    koneksi.Close();
        //}
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //koreksiSubsidi koreksiSubsidi;
            if ((formKoreksiSubsidi = (koreksiSubsidi)alat.FormSudahDibuat(typeof(koreksiSubsidi))) == null)
            {
                formKoreksiSubsidi = new koreksiSubsidi();
                formKoreksiSubsidi.idKasda = Convert.ToInt32(listView1.SelectedItems[0].Text);
                formKoreksiSubsidi.txtNoSPK.Text = listView1.SelectedItems[0].SubItems[1].Text;
                formKoreksiSubsidi.txtKeterangan.Text = listView1.SelectedItems[0].SubItems[5].Text;
                formKoreksiSubsidi.txtSubs.Text = listView1.SelectedItems[0].SubItems[6].Text;
                formKoreksiSubsidi.txtFung.Text = listView1.SelectedItems[0].SubItems[7].Text;
                formKoreksiSubsidi.dateTimePicker1.Value = Convert.ToDateTime( listView1.SelectedItems[0].SubItems[2].Text);
                formKoreksiSubsidi.txtNoRek.Text = listView1.SelectedItems[0].SubItems[3].Text;
                formKoreksiSubsidi.txtNoBukti.Text = listView1.SelectedItems[0].SubItems[9].Text;
                formKoreksiSubsidi.txtDetailRek.Text = listView1.SelectedItems[0].SubItems[10].Text;
                formKoreksiSubsidi.txtRekDowo.Text = listView1.SelectedItems[0].SubItems[11].Text;
                formKoreksiSubsidi.txtSupplier.Text = listView1.SelectedItems[0].SubItems[12].Text;
                formKoreksiSubsidi.dateTimePicker2.Value = Convert.ToDateTime(listView1.SelectedItems[0].SubItems[13].Text);
                formKoreksiSubsidi.StartPosition = FormStartPosition.CenterScreen;
                formKoreksiSubsidi.idOpp = idOpp;
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //koreksiSubsidi.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                //MessageBox.Show(listView1.SelectedItems[0].Text);
                formKoreksiSubsidi.StartPosition = FormStartPosition.CenterScreen;
                //koreksiSubsidi.MdiParent = this;
                //formKoreksiSubsidi.Parent = this;
                formKoreksiSubsidi.ShowDialog();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                formKoreksiSubsidi.Select();
            }
        }
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                ShowData();
            }
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.Trim() != "")
            {
                ArrayList listku = searchinListUsingLinq(comboBox1.Text, 0);
                //idKPA = Convert.ToInt16(listku[0].ToString());
                label12.Text = "Nomor Bukti : " + generateNoKwitansi();
                statusSimpan = 1;
            }
        }

        private string filterRupiah(string rupiah)
        {
            rupiah = rupiah.Replace("Rp. ", "");
            rupiah = rupiah.Replace(".", "");
            rupiah = rupiah.Replace(",",".");
            return rupiah;
        }

        private void sortNoSPKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusSort = 1;
            if (!backgroundWorker3.IsBusy)
                backgroundWorker3.RunWorkerAsync();
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            contextMenuStrip1.SafeControlInvoke(conteks => contextMenuStrip1.Enabled = false);
            progressBar1.SafeControlInvoke(ProgressBar =>
                {
                    progressBar1.Visible = true;
                    progressBar1.Maximum = listView1.SafeControlInvoke(listView => listView1.Items.Count + 1);
                }
            );
            IEnumerable<tampungKasda> namaKpa = null;
            List<tampungKasda> grupKasda = new List<tampungKasda>();
            listView1.SafeControlInvoke(
                listView =>
                {
                    foreach (ListViewItem item in listView1.Items)
                    {
                        //MessageBox.Show(item.SubItems[1].Text);
                        grupKasda.Add(new tampungKasda(item.SubItems[0].Text, item.SubItems[1].Text, Convert.ToDateTime(item.SubItems[2].Text), //US REGION
                            item.SubItems[3].Text, item.SubItems[4].Text, item.SubItems[5].Text, Convert.ToDecimal(filterRupiah(item.SubItems[6].Text)),
                            Convert.ToDecimal(filterRupiah(item.SubItems[7].Text)), item.SubItems[8].Text, item.SubItems[9].Text, item.SubItems[10].Text,
                            item.SubItems[11].Text, item.SubItems[12].Text, Convert.ToDateTime(item.SubItems[13].Text)));
                    }
                }
            );
            if (statusSort == 1)
            {
                namaKpa = (from s in grupKasda
                           orderby s.NoSPK //descending
                           select s);
            }
            else if (statusSort == 2)
            {
                namaKpa = (from s in grupKasda
                           orderby s.tglSP //descending
                           select s);
            }
            else if (statusSort == 3)
            {
                namaKpa = (from s in grupKasda
                           orderby s.idSPK //descending
                           select s);
            }
            foreach (var i in namaKpa)
            {
                ListViewItem item = new ListViewItem(i.idSPK);
                item.SubItems.Add(i.NoSPK);
                item.SubItems.Add(string.Format("{0:MM/dd/yyyy}", i.tglSP));
                item.SubItems.Add(i.kodeRek);
                item.SubItems.Add(i.uraian);
                item.SubItems.Add(i.ket);
                item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.subsi));
                item.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", i.fungsi));
                item.SubItems.Add(i.lunas);
                item.SubItems.Add(i.noBukti);
                item.SubItems.Add(i.noRek);
                item.SubItems.Add(i.noRekPanjang);
                item.SubItems.Add(i.noRekanan);
                item.SubItems.Add(string.Format("{0:MM/dd/yyyy}", i.tglSPJ));
                lvItemGroup.Add(item);
                progressBar1.SafeControlInvoke(ProgressBar => progressBar1.Value++);
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
            e.Result = grupKasda;
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tabelKasdaFilter = (List<tampungKasda>)e.Result;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 0;
            progressBar1.Visible = false;
            statusSort = 0;
            contextMenuStrip1.Enabled = true;
        }

        private void sortTglSPKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusSort = 2;
            if (!backgroundWorker3.IsBusy)
                backgroundWorker3.RunWorkerAsync();
        }

        private void sortUrutanEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusSort = 3;
            if (!backgroundWorker3.IsBusy)
                backgroundWorker3.RunWorkerAsync();
        } 
    }
}
