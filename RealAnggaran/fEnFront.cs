using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

namespace RealAnggaran
{
    public partial class fEnFront : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        string query = null;
        int idRek = 0;
        int idSupp = 0;
        public int idOpp = 0;
        int idJen = 0;
        int idBayar = 0;
        //string sf = "";
        int idSumber = 0;
        int idKpa = 0;
        fCari cari;
        int statusCari = 2;
        string noRek = null;

        public fEnFront()
        {
            InitializeComponent();
            dtpEnBayar.CustomFormat = "dd/MM/yyyy";
            dtpEnBayar.Format = DateTimePickerFormat.Custom;
            dateTimePicker5.CustomFormat = "dd/MM/yyyy";
            dateTimePicker5.Format = DateTimePickerFormat.Custom;
            koneksi = konek.KonekDb();
        }

        private void bersih2()
        {
            txtEnNoBukti.Focus();
            alat.CleaningService(panel1);
            alat.CleaningService(panel2);
            alat.CleaningService(panel3);
            alat.CleaningService(panel4);
            lUraian.Text = "[KPA]";
            lSupp.Text = "[nama_suplier]";
            lKet.Text = "[ket_jenis]";
            //lUraianV.Text = "[uraian]";
            //lSuppEn.Text = "[nama_suplier]";
            //lKetV.Text = "[ket_jenis]";
            idBayar = 0;
            idRek = 0;
            idSupp = 0;
            idKpa = 0;
            //idOpp = 0;
            idJen = 0;
            //sf = "";
            idSumber = 0;
            label7.Text = "[nama_suplier]";
            statusCari = 2;
            lvTampil.Items.Clear();
            noRek = "";

            // Addition
            alat.CleaningService(panel5);
            alat.CleaningService(panel6);
            label54.Text = "[KPA]";
            label51.Text = "[nama_suplier]";
            label48.Text = "[ket_jenis]";
            enJmlh.Text = "0";
            txtEnPPh.Text = "0";
            txtENPPN.Text = "0";
            maskedTextBox5.Text = "0";
            maskedTextBox4.Text = "0";
            maskedTextBox3.Text = "0";
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
            query = "INSERT INTO A_PENGELUARAN (TglBayar, NoBayar, Id_Reken, Id_Supplier, PPnRp, PPhRp, JmlRp, Lunas, " +
                "idSumber, Id_Opp_En, Batal, Id_Jenis, Id_Opp_Ver, kwi, ketBayar) VALUES (CONVERT(SMALLDATETIME, '" + dtpEnBayar.Text + " " + DateTime.Now.ToLongTimeString() + " ', 103), '" + txtEnNoBukti.Text.Trim() + "', " + idRek + 
                ", " + idSupp + ", " + txtENPPN.Text.Trim() + ", " + txtEnPPh.Text.Trim() + ", " + enJmlh.Text.Trim() + ", 0, '" + idSumber + 
                "', " + idOpp + ", 0," + idJen + "," + idOpp + ", 0, '" + txtKetBayar.Text + "')";

            if (txtEnNoBukti.Text.Trim() == "")
            {
                MessageBox.Show("No Bukti Harap Diisi", "PERHATIAN");
                txtEnNoBukti.Focus();
            }
            else if (idRek == 0)
            {
                MessageBox.Show("No Rekening Harap Diperhatikan", "PERHATIAN");
                txtEnKdRek.Focus();
            }
            else if (idSupp == 0)
            {
                MessageBox.Show("Data Suplier harap Diperhatikan", "PERHATIAN");
                txtEnKodeSup.Focus();
            }
            else if (idSumber == 0)
            {
                MessageBox.Show("Sumber Dana harap Diperhatikan", "PERHATIAN");
                cbEnSF.Focus();
            }
            else
            {
                if (konek.CekFieldUnik("A_PENGELUARAN", "NoBayar", txtEnNoBukti.Text.Trim() + " AND BATAL = 0") == true)
                {
                    MessageBox.Show("NO BAYAR TELAH DIGUNAKAN, SILAHKAN PILIH YANG LAIN!", "PERHATIAN");
                    txtEnNoBukti.Focus();
                }
            //    else if (cekSaldo() == false)
            //    {
            //        MessageBox.Show("SALDONYA KPA TERSEBUT KURANG!", "PERHATIAN");
            //        bersih2();
            //        txtEnNoBukti.Focus();
            //    }
                else
                {
                    if (idJen == 0)
                    {
        //                MessageBox.Show("Jenis harap Diperhatikan", "PERHATIAN");
        //                cbEnJenis.Focus();
                        idJen = 1;
                    }
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
                    koneksi.Close();
                    MessageBox.Show("PENYIMPANAN BERHASIL, TERIMAKASIH", "PERHATIAN");
                    bersih2();
                }
            }
            tampilTabelEn();
        }

        private void inisiasiListView()
        {
            // ============================= Front Kwitansi ===========================================
            // Set the view to show details
            lvTampil2.View = View.Details;

            // Disallow the user from edit the item
            //lvTampil.LabelEdit = false;

            // Allow the user to rearrange columns
            lvTampil2.AllowColumnReorder = true;

            // Select the item and subitems when selection is made
            lvTampil2.FullRowSelect = true;

            // Display the gridline
            lvTampil2.GridLines = true;

            // Sort the items in the list in accending order
            lvTampil2.Sorting = System.Windows.Forms.SortOrder.Ascending;

            // Restrict the multiselect
            lvTampil2.MultiSelect = false;

            ColumnHeader headerTgl2 = this.lvTampil2.Columns.Add("Tanggal", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoKwi2 = this.lvTampil2.Columns.Add("Nomor Kwitansi", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoRek2 = this.lvTampil2.Columns.Add("No Rekening", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKetRek2 = this.lvTampil2.Columns.Add("Uraian", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSupp2 = this.lvTampil2.Columns.Add("Supplier", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerPPn2 = this.lvTampil2.Columns.Add("PPn (Rp)", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerPPh2 = this.lvTampil2.Columns.Add("PPh (Rp)", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJum2 = this.lvTampil2.Columns.Add("Jumlah", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerStatus2 = this.lvTampil2.Columns.Add("Status", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSubFung2 = this.lvTampil2.Columns.Add("Sub/Fung", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJenis2 = this.lvTampil2.Columns.Add("Jenis", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headeridBayar2 = this.lvTampil2.Columns.Add("idBayar", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            
            lvTampil2.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);

            // ============================= Back Kwitasi ===========================================
            // Set the view to show details
            lvTampil.View = View.Details;

            // Disallow the user from edit the item
            //lvTampil.LabelEdit = false;

            // Allow the user to rearrange columns
            lvTampil.AllowColumnReorder = true;

            // Select the item and subitems when selection is made
            lvTampil.FullRowSelect = true;

            // Display the gridline
            lvTampil.GridLines = true;

            // Sort the items in the list in accending order
            lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;

            // Restrict the multiselect
            lvTampil.MultiSelect = false;

            ColumnHeader headeridBayar = this.lvTampil.Columns.Add("NoBayar", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerTgl = this.lvTampil.Columns.Add("Tanggal", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoKwi = this.lvTampil.Columns.Add("Nomor Kwitansi", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoRek = this.lvTampil.Columns.Add("No Rekening", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);            
            ColumnHeader headerSupp = this.lvTampil.Columns.Add("Supplier", 30 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJum = this.lvTampil.Columns.Add("Jumlah", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);

            // ============================= CEK ===========================================
            // Set the view to show details
            lvTampil3.View = View.Details;

            // Disallow the user from edit the item
            //lvTampil.LabelEdit = false;

            // Allow the user to rearrange columns
            lvTampil3.AllowColumnReorder = true;

            // Select the item and subitems when selection is made
            lvTampil3.FullRowSelect = true;

            // Display the gridline
            lvTampil3.GridLines = true;

            // Sort the items in the list in accending order
            lvTampil3.Sorting = System.Windows.Forms.SortOrder.Ascending;

            // Restrict the multiselect
            lvTampil3.MultiSelect = false;

            //ColumnHeader headerIdCek = this.lvTampil3.Columns.Add("Id Cek", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoCek = this.lvTampil3.Columns.Add("No Cek", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerTglP1 = this.lvTampil3.Columns.Add("Periode", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerTglP2 = this.lvTampil3.Columns.Add("Sampai Dengan", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKet3 = this.lvTampil3.Columns.Add("Keterangan", 40 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil3.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);

            // ============================= SEMENTARA ===========================================
            // Set the view to show details
            lvTampil4.View = View.Details;

            // Disallow the user from edit the item
            //lvTampil.LabelEdit = false;

            // Allow the user to rearrange columns
            lvTampil4.AllowColumnReorder = true;

            // Select the item and subitems when selection is made
            lvTampil4.FullRowSelect = true;

            // Display the gridline
            lvTampil4.GridLines = true;

            // Sort the items in the list in accending order
            lvTampil4.Sorting = System.Windows.Forms.SortOrder.Ascending;

            // Restrict the multiselect
            lvTampil4.MultiSelect = false;

            ColumnHeader headerTgl4 = this.lvTampil4.Columns.Add("Tanggal", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoKwi4 = this.lvTampil4.Columns.Add("Nomor Kwitansi", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoRek4 = this.lvTampil4.Columns.Add("No Rekening", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKetRek4 = this.lvTampil4.Columns.Add("Uraian", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSupp4 = this.lvTampil4.Columns.Add("Supplier", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerPPn4 = this.lvTampil4.Columns.Add("PPn (Rp)", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerPPh4 = this.lvTampil4.Columns.Add("PPh (Rp)", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJum4 = this.lvTampil4.Columns.Add("Jumlah", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerStatus4 = this.lvTampil4.Columns.Add("Status", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSubFung4 = this.lvTampil4.Columns.Add("Sub/Fung", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJenis4 = this.lvTampil4.Columns.Add("Jenis", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headeridBayar4 = this.lvTampil4.Columns.Add("idBayar", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil4.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);

        }

        private void tampilTabelEn()
        {
            lvTampil2.Items.Clear();
            query = "SELECT CONVERT(CHAR(10), TglBayar, 103), NoBayar, kd_Reken, ketBayar, NamaSupp, PPnRp, PPhRp, JmlRp, lunas, kdSumber, Jenis, Id_Bayar_Mst " +
                "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE " +
                "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND (lunas = 0 AND batal = 0 AND kwi = 0) AND e.idSumber = a.idSumber) ORDER BY TglBayar DESC";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 0));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 3));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 4)); 
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 5)))); //ppn
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 6)))); //pph
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 7)))); //jumlah
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 8));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 9));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 10));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 11));

                    lvTampil2.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        //private void tampilTabelVer()
        //{
        //    lvTampil.Items.Clear();
        //    query = "SELECT TglBayar, NoBayar, kd_Reken, Rekening, NamaSupp, PPnRp, PPhRp, JmlRp, lunas, SubFung, Jenis " +
        //        "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d WHERE " +
        //        "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND (lunas = 0)) ORDER BY TglBayar ASC";
        //    koneksi.Open();
        //    pembaca = konek.membacaData(query, koneksi);
        //    if (pembaca.HasRows)
        //    {
        //        while (pembaca.Read())
        //        {
        //            ListViewItem lvItem = new ListViewItem(alat.pengecekField(pembaca, 0));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 1));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 2));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 3));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 4));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 5));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 6));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 7));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 8));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 9));
        //            lvItem.SubItems.Add(alat.pengecekField(pembaca, 10));

        //            lvTampil.Items.Add(lvItem);
        //        }
        //        pembaca.Close();
        //    }
        //    koneksi.Close();
        //}

        private void fEnFront_Load(object sender, EventArgs e)
        {
            alat.DeSerial(this);
            inisiasiListView();
            tampilTabelEn();
            tampilListView4();
            bersih2();
            isiKombo("SELECT * FROM A_SUPPLIER", 1, txtEnKodeSup);
            isiKombo("SELECT * FROM A_SUPPLIER", 1, comboBox1);
            isiKombo("SELECT * FROM A_JENIS", 1, cbEnJenis);
            // Addition
            isiKombo("SELECT * FROM A_SUPPLIER", 1, comboBox2);
            isiKombo("SELECT * FROM A_SUMBER_DANA", 1, comboBox4);
            isiKombo("SELECT * FROM A_JENIS", 1, comboBox3);

            isiKomboSumber();
            alat.WriteFileVersion();
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == "Page1")
            {
                tampilTabelEn();
            }
            else
            {
                tampilLVKwi();
            }
        }

        /*  We're currently working on a framework that look like alien
         *  Unlike our other cousin, we're dont count on the power of server, but depent on client and the network fastness
         *  Well... here the plant
         *  built a mechanic that:
         *  - Fetch the Rek. stuff, Supplier stuff, the person whom Operate this (operators :), and jenis stuff
         *  - Calculate the pph&ppn Stuff
         *  - LATER: we'll translate the meaning of true&false meaning of the lunas field to the fools
         *  - Doing some Update at table A_ANGGARAN & A_PENERIMAAN according to the following condition:
         *      > The Lunas button pressed: it will subtract the ammount of
         *      > The Batal button pressed: it will add the ammount of
         *  - Special case of the id_Opp: use the MDI form to send the data need to be fill in, with all mean nessesary
         *  - Proceed at cautions, good luck
         */

        private void txtEnKdRek_KeyPress(object sender, KeyPressEventArgs e)
        {
            string kd_rek = alat.FilterMBox(txtEnKdRek.Text);
            query = "SELECT id_reken, Rekening, KdKpa FROM A_REKENING a, A_KPA b WHERE b.Id_Kpa = a.Id_Kpa AND kd_Reken = '" + kd_rek.Trim() + "'";
            if (e.KeyChar == (char)13)
            {
                koneksi.Open();
                pembaca = konek.MembacaData(query, koneksi);
                if (pembaca.HasRows)
                {
                    pembaca.Read();
                    idRek = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                    lUraian.Text = alat.PengecekField(pembaca, 2);
                    pembaca.Close();
                }
                else
                {
                    MessageBox.Show("Rekening Dengan Kode Tersebut Tidak Ditemukan!!!");
                    txtEnKdRek.Focus();
                }
                koneksi.Close();
                //MessageBox.Show(query);
            }
        }

        private void comboBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void isiKwitansi()
        {
            query = "SELECT id_Bayar_Mst, CONVERT(CHAR(10), TglBayar, 103), NoBayar, NamaSupp, JmlRp, KdSupp FROM A_PENGELUARAN a, A_SUPPLIER b WHERE" +
                " b.id_Supplier = a.id_Supplier AND NoBayar = '" + textBox10.Text.Trim() + "'";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                idBayar = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                dateTimePicker2.Text = alat.PengecekField(pembaca, 1);
                dateTimePicker4.Text = alat.PengecekField(pembaca, 1);
                textBox10.Text = alat.PengecekField(pembaca, 2);
                comboBox1.Text = alat.PengecekField(pembaca, 5);
                label7.Text = alat.PengecekField(pembaca, 3);
                maskedTextBox8.Text = alat.PengecekField(pembaca, 4);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                //        if (konek.cekFieldUnik("A_PENGELUARAN", "Lunas", "1") == true)
                //        {
                //            MessageBox.Show("Data dengan Nomor Pembayaran tersebut, SUDAH DIBAYAR!", "PERHATIAN");
                //            textBox10.Focus();
                //            bersih2();
                //        }
                //        else
                //        {
                //            ambilDataBayar(textBox10.Text.Trim());
                //        }
                //    }
                if (textBox10.Text == "")
                {
                    statusCari = 2;
                }
                else if (textBox10.Text != "" & konek.CekFieldUnik("A_PENGELUARAN", "NoBayar", textBox10.Text) == true)
                {
                    isiKwitansi();
                }
            }
        }

        //private void ambilDataBayar(string noBayar)
        //{
        //    query = "SELECT TglBayar, PPnRp, PPhRp, JmlRp, kd_Reken, Rekening, KdSupp, NamaSupp, SubFung, KdJenis, Jenis, " +
        //        "id_Bayar_Mst, a.id_Reken, noBayar FROM A_PENGELUARAN a, " +
        //        "A_REKENING b, A_SUPPLIER c, A_JENIS d WHERE ((b.id_reken = a.id_reken AND c.id_supplier = a.id_supplier AND d.id_jenis = a.id_jenis)" +
        //        " AND NoBayar = '" + noBayar + "')";
        //    koneksi.Open();
        //    pembaca = konek.membacaData(query, koneksi);
        //    if (pembaca.HasRows)
        //    {
        //        pembaca.Read();
        //        dateTimePicker2.Text = alat.pengecekField(pembaca, 0);
        //        maskedTextBox6.Text = Convert.ToString(Convert.ToInt32(alat.pengecekField(pembaca, 1)) + Convert.ToInt32(alat.pengecekField(pembaca, 2)) +
        //            Convert.ToInt32(alat.pengecekField(pembaca, 3)));
        //        maskedTextBox7.Text = alat.pengecekField(pembaca, 1);
        //        maskedTextBox5.Text = alat.pengecekField(pembaca, 2);
        //        maskedTextBox8.Text = alat.pengecekField(pembaca, 3);
        //        txtVerKdRek.Text = alat.pengecekField(pembaca, 4);
        //        lUraianV.Text = alat.pengecekField(pembaca, 5);
        //        txt.Text = alat.pengecekField(pembaca, 6);
        //        lSuppEn.Text = alat.pengecekField(pembaca, 7);
        //        if (alat.pengecekField(pembaca, 8).Trim() == "S")
        //        {
        //            comboBox4.Text = "SUBSIDI";
        //            sf = "s";
        //        }
        //        else
        //        {
        //            comboBox4.Text = "FUNGSIONAL";
        //            sf = "f";
        //        }
        //        //comboBox4.Text = alat.pengecekField(pembaca, 8);
        //        comboBox3.Text = alat.pengecekField(pembaca, 9);
        //        lKetV.Text = alat.pengecekField(pembaca, 10);
        //        idBayar = Convert.ToInt16(alat.pengecekField(pembaca, 11));
        //        idRek = Convert.ToInt16(alat.pengecekField(pembaca, 12));
        //        textBox10.Text = alat.pengecekField(pembaca, 13);
        //        pembaca.Close();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Data dengan Nomor Pembayaran tersebut, TIDAK DITEMUKAN!", "PERHATIAN");
        //        textBox10.Focus();
        //        bersih2();
        //    }
        //    koneksi.Close();
        //}

        private void txtEnNoBukti_TextChanged(object sender, EventArgs e)
        {
            //txtEnNoBukti.Text = txtEnNoBukti.Text.Replace(" ", "");

        }

        private void lvTampil_DoubleClick(object sender, EventArgs e)
        {
            //string kode = lvTampil.SelectedItems[0].SubItems[1].Text.Trim();
            //ambilDataBayar(kode);
        }

        private void cbEnSF_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbEnJenis_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void isiKombo(string query, byte nomerField, System.Windows.Forms.ComboBox namaKombo)
        {
            koneksi.Open();
            SqlDataReader pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    namaKombo.Items.Add(alat.PengecekField(pembaca, nomerField));
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void txtEnKodeSup_TextChanged(object sender, EventArgs e)
        {
        }

        private void ambilSupplier(System.Windows.Forms.ComboBox namaCombo)
        {
            query = "SELECT id_Supplier, NamaSupp FROM A_SUPPLIER WHERE kdSupp = '" + namaCombo.Text.Trim() + "'";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                idSupp = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                if (namaCombo.Name == "txtEnKodeSup")
                    lSupp.Text = alat.PengecekField(pembaca, 1);
                else
                {
                    label7.Text = alat.PengecekField(pembaca, 1);
                    idSupp = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                }
                pembaca.Close();
            }
            else
            {
                MessageBox.Show("Maaf Supplier / Rekanan dengan kode tersebut TIDAK DITEMUKAN!","PERHATIAN");
                //namaCombo.Focus();
            }
            koneksi.Close();
        }

        private void cbEnJenis_TextChanged(object sender, EventArgs e)
        {

        }

        private void hitungTotal()
        {
            try
            {
                decimal total = 0;
                total = Convert.ToDecimal(enJmlh.Text) + Convert.ToDecimal(txtEnPPh.Text) + Convert.ToDecimal(txtENPPN.Text);
                if (total < 0)
                    txtEnBayar.Text = "0";
                else
                    txtEnBayar.Text = Convert.ToString(total);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void txtEnBayar_TextChanged(object sender, EventArgs e)
        {
            if (txtEnBayar.Text.Trim() != "")
                hitungTotal();
            else
                txtEnBayar.Text = "0";
        }

        private void txtENPPN_TextChanged(object sender, EventArgs e)
        {
            if (txtENPPN.Text.Trim() != "")
                hitungTotal();
            else
                txtENPPN.Text = "0";
        }

        private void txtEnPPh_TextChanged(object sender, EventArgs e)
        {
            if (txtEnPPh.Text.Trim() != "")
                hitungTotal();
            else
                txtEnPPh.Text = "0";
        }

        private void enJmlh_Enter(object sender, EventArgs e)
        {
            enJmlh.SelectionStart = 0;
        }

        private void txtENPPN_Click(object sender, EventArgs e)
        {
            txtENPPN.SelectionStart = 0;
        }

        private void enJmlh_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void enJmlh_Click(object sender, EventArgs e)
        {
            enJmlh.SelectionStart = 0;
        }

        private void txtEnPPh_Click(object sender, EventArgs e)
        {
            txtEnPPh.SelectionStart = 0;
        }

        private void cbEnSF_TextChanged(object sender, EventArgs e)
        {
            //if (cbEnSF.Text.Trim() == "FUNGSIONAL")
            //    sf = "F";
            //else
            //    sf = "S";
        }

        private void enJmlh_TextChanged(object sender, EventArgs e)
        {
            if (enJmlh.Text.Trim() != "")
                hitungTotal();
            else
                enJmlh.Text = "0";
        }

        private void enJmlh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')e.KeyChar = (char)0;
            e.Handled = alat.Validate(e.KeyChar);
        }

        private void txtENPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
            e.Handled = alat.Validate(e.KeyChar);
        }

        private void txtEnPPh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
            e.Handled = alat.Validate(e.KeyChar);
        }

        private void isiKomboSumber()
        {
            cbEnSF.Items.Clear();
            koneksi.Open();
            pembaca = konek.MembacaData("SELECT * FROM A_SUMBER_DANA", koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    cbEnSF.Items.Add(alat.PengecekField(pembaca, 1));
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void bLunas_Click(object sender, EventArgs e)
        {
            //query = "UPDATE A_PENGELUARAN SET Lunas = 1 WHERE id_Bayar_mst = " + idBayar + " ";
            //koneksi.Open();
            //konek.masukkanData(query, koneksi);
            //koneksi.Close();
            //updateAnggar();
            //updateTerima();
            //MessageBox.Show("DATA TELAH DISIMPAN, TERIMAKASIH","PERHATIAN");
            //bersih2();
            //textBox10.Focus();
            //tampilTabelVer();
            if (textBox1.Text == "")
            {
                MessageBox.Show("SILAHKAN MASUKAN NOMOR CEK!","PERHATIAN");
                textBox1.Focus();
            }
            else
            {
                if (konek.CekFieldUnik("A_CEK", "kd_CEk", textBox1.Text) == true)
                {
                    MessageBox.Show("No Cek Ditemukan, Silahkan pilih yang lain!", "PERHATIAN");
                    textBox1.Focus();
                }
                else
                {
                    koneksi.Open();
                    for (int i = 0; i < lvTampil.Items.Count; i++)
                    {
                        konek.MasukkanData("INSERT INTO A_CEK (kd_Cek, ket, periodeA, periodeB, id_Bayar_Mst, id_Opp_En, id_Opp_Ver) VALUES ('" + textBox1.Text +
                            "','" + txt.Text + "','" + dateTimePicker1.Text + "','" + dateTimePicker3.Text +
                            "'," + lvTampil.Items[i].Text + ", " + idOpp + ", " + idOpp +")", koneksi);
                        konek.MasukkanData("UPDATE A_PENGELUARAN SET kwi = 1 WHERE id_Bayar_Mst  = " + lvTampil.Items[i].Text + "", koneksi);
                    }
                    koneksi.Close();
                    MessageBox.Show("DATA CEK TELAH TERSIMPAN, TERIMAKASIH!", "PERHATIAN");
                    bersih2();
                    textBox1.Focus();
                }
            }
            tampilLVKwi();
        }

        private void bKeluar2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bKeluar1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void updateAnggar()
        //{
        //    if (sf == "s")
        //        query = "UPDATE A_ANGGAR SET RealSubsi = RealSubsi + " + Convert.ToDecimal(maskedTextBox8.Text) + 
        //            ", SisaSubsi = SisaSubsi - " + Convert.ToDecimal(maskedTextBox8.Text) + ", TotAnggar = TotAnggar - " + 
        //            Convert.ToDecimal(maskedTextBox8.Text) + " WHERE id_Reken = " + idRek + "";
        //    else if (sf == "f")
        //        query = "UPDATE A_ANGGAR SET RealFungsi = RealFungsi + " + Convert.ToDecimal(maskedTextBox8.Text) +
        //            ", SisaFungsi = SisaFungsi - " + Convert.ToDecimal(maskedTextBox8.Text) + ", TotAnggar = TotAnggar - " + 
        //            Convert.ToDecimal(maskedTextBox8.Text) + " WHERE id_Reken = " + idRek + "";
        //    koneksi.Open();
        //    konek.masukkanData(query, koneksi);
        //    koneksi.Close();
        //}

        //private void updateTerima()
        //{
        //    koneksi.Open();
        //    konek.masukkanData("EXECUTE sp_keluar '" + alat.dateFormater(dateTimePicker2.Text) + "', " + 
        //        Convert.ToDecimal(maskedTextBox8.Text) + ", '" + textBox10.Text + "'", koneksi);
        //    koneksi.Close();
        //}

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label45_Click(object sender, EventArgs e)
        {

        }

        private void bCari_Click(object sender, EventArgs e)
        {
            if ((cari = (fCari)alat.FormSudahDibuat(typeof(fCari))) == null)
            {
                cari = new fCari();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //cari.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                cari.groupBox1.Text = "KWITANSI";
                if (statusCari == 2)
                    cari.query = "SELECT id_Bayar_Mst, CONVERT(CHAR(10), TglBayar, 103), NoBayar, kd_Reken, ketBayar, NamaSupp, PPnRp, PPhRp, JmlRp, lunas, kdSumber, Jenis " +
                        "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE " +
                        "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND (lunas = 0 AND batal = 0 AND kwi = 0 AND e.idSumber = a.idSumber) AND NoBayar LIKE '" + textBox10.Text + "%')";
                else if (statusCari == 1)
                    cari.query = "SELECT id_Bayar_Mst, CONVERT(CHAR(10), TglBayar, 103), NoBayar, kd_Reken, ketBayar, NamaSupp, PPnRp, PPhRp, JmlRp, lunas, kdSumber, Jenis " +
                        "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE " +
                        "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND (lunas = 0 AND batal = 0 AND kwi = 0 AND e.idSumber = a.idSumber) AND (TglBayar BETWEEN '" +
                        dtpEnBayar.Text + "' AND '" + dtpEnBayar.Text + "'))";
                else if (statusCari == 3)
                    cari.query = "SELECT id_Bayar_Mst, CONVERT(CHAR(10), TglBayar, 103), NoBayar, kd_Reken, ketBayar, NamaSupp, PPnRp, PPhRp, JmlRp, lunas, kdSumber, Jenis " +
                        "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE " +
                        "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND (lunas = 0 AND batal = 0 AND kwi = 0 AND e.idSumber = a.idSumber) AND (a.Id_Supplier = " + idSupp + "))";

                cari.StartPosition = FormStartPosition.CenterScreen;
                cari.lvTampil.View = View.Details;
                cari.lvTampil.AllowColumnReorder = true;
                cari.lvTampil.FullRowSelect = true;
                cari.lvTampil.GridLines = true;
                cari.lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;
                cari.lvTampil.MultiSelect = false;

                ColumnHeader headerIdBayar = cari.lvTampil.Columns.Add("id_bayar", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerTgl2 = cari.lvTampil.Columns.Add("Tanggal", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerNoKwi2 = cari.lvTampil.Columns.Add("No Bukti", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerNoRek2 = cari.lvTampil.Columns.Add("No Rekening", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerKetRek2 = cari.lvTampil.Columns.Add("Uraian", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerSupp2 = cari.lvTampil.Columns.Add("Supplier", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerPPn2 = cari.lvTampil.Columns.Add("PPn (Rp)", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerPPh2 = cari.lvTampil.Columns.Add("PPh (Rp)", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerJum2 = cari.lvTampil.Columns.Add("Jumlah", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerStatus2 = cari.lvTampil.Columns.Add("Status", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerSubFung2 = cari.lvTampil.Columns.Add("Sumber", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                ColumnHeader headerJenis2 = cari.lvTampil.Columns.Add("Jenis", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

                cari.lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);

                cari.TopMost = true;
                cari.ShowDialog();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                cari.TopMost = true;
                cari.Select();
            }

        }

        private void bTambah_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker4_Validating(object sender, CancelEventArgs e)
        {
            if (dateTimePicker4.Value < dateTimePicker2.Value)
                dateTimePicker4.Value = dateTimePicker2.Value;
            statusCari = 1;
        }

        private void dateTimePicker2_Validating(object sender, CancelEventArgs e)
        {
            if (dateTimePicker4.Value < dateTimePicker2.Value)
                dateTimePicker4.Value = dateTimePicker2.Value;
            statusCari = 1;
        }

        private void dateTimePicker1_Validating(object sender, CancelEventArgs e)
        {
            if (dateTimePicker3.Value < dateTimePicker1.Value)
                dateTimePicker3.Value = dateTimePicker1.Value;
        }

        private void dateTimePicker3_Validating(object sender, CancelEventArgs e)
        {
            if (dateTimePicker3.Value < dateTimePicker1.Value)
                dateTimePicker3.Value = dateTimePicker1.Value;
        }

        private void txtEnKodeSup_Validating(object sender, CancelEventArgs e)
        {
            ambilSupplier(txtEnKodeSup);
        }

        private void comboBox1_Validating(object sender, CancelEventArgs e)
        {
            ambilSupplier(comboBox1);
            statusCari = 3;
        }

        public void tambahDaftar(string idKwitansi, string date1, string txtNoBayar, string noReke, string namaSupp, string total)
        {
            //if (lvTampil.Items.Count > 0)
            //{
                ListViewItem item = lvTampil.FindItemWithText(idKwitansi);
                if (item == null)
                {
                    ListViewItem lvItem = new ListViewItem(idKwitansi);
                    lvItem.SubItems.Add(date1);
                    lvItem.SubItems.Add(txtNoBayar);
                    lvItem.SubItems.Add(noReke);
                    lvItem.SubItems.Add(namaSupp);
                    lvItem.SubItems.Add(total);

                    lvTampil.Items.Add(lvItem);
                }

                //ListViewItem item = ListView1.FindItemWithText("test");
                //if (!ListView1.Items.Contains(item))
                //{
                //    // doesn't exist, add it
                //}

            //}
            //else
            //{
            //    ListViewItem lvItem = new ListViewItem(idKwitansi);
            //    lvItem.SubItems.Add(date1);
            //    lvItem.SubItems.Add(txtNoBayar);
            //    lvItem.SubItems.Add(namaSupp);
            //    lvItem.SubItems.Add(total);

            //    lvTampil.Items.Add(lvItem);
            //}
            /*
             * How to: Add Search Capabilities to a ListView Control
            Oftentimes when working with a large list of items in a ListView control, you want to offer search capabilities to the user. The ListView control offers this capability in two different ways: text matching and location searching.

            The FindItemWithText method allows you to perform a text search on a ListView in list or details view, given a search string and an optional starting and ending index. In contrast, the FindNearestItem method allows you to find an item in a ListView in icon or tile view, given a set of x- and y-coordinates and a direction to search.

            To find an item using text

            Create a ListView with the View property set to Details or List, and then populate the ListView with items.

            Call the FindItemWithText method, passing the text of the item you would like to find.

            The following code example demonstrates how to create a basic ListView, populate it with items, and use text input from the user to find an item in the list.

            VBC#C++F#JScript

            private ListView textListView = new ListView();
            private TextBox searchBox = new TextBox();
            private void InitializeTextSearchListView()
            {
                searchBox.Location = new Point(10, 60);
                textListView.Scrollable = true;
                textListView.Width = 80;
                textListView.Height = 50;

                // Set the View to list to use the FindItemWithText method.
                textListView.View = View.List;

                // Populate the ListViewWithItems
                textListView.Items.AddRange(new ListViewItem[]{ 
                    new ListViewItem("Amy Alberts"), 
                    new ListViewItem("Amy Recker"), 
                    new ListViewItem("Erin Hagens"), 
                    new ListViewItem("Barry Johnson"), 
                    new ListViewItem("Jay Hamlin"), 
                    new ListViewItem("Brian Valentine"), 
                    new ListViewItem("Brian Welker"), 
                    new ListViewItem("Daniel Weisman") });

                // Handle the TextChanged to get the text for our search.
                searchBox.TextChanged += new EventHandler(searchBox_TextChanged);

                // Add the controls to the form.
                this.Controls.Add(textListView);
                this.Controls.Add(searchBox);

            }

            private void searchBox_TextChanged(object sender, EventArgs e)
            {
                // Call FindItemWithText with the contents of the textbox.
                ListViewItem foundItem =
                    textListView.FindItemWithText(searchBox.Text, false, 0, true);
                if (foundItem != null)
                {
                    textListView.TopItem = foundItem;

                }
            }


            To find an item using x- and y-coordinates

            Create a ListView with the View property set to SmallIcon or LargeIcon, and then populate the ListView with items.

            Call the FindNearestItem method, passing the desired x- and y-coordinates and the direction you would like to search.

            The following code example demonstrates how to create a basic icon ListView, populate it with items, and capture the MouseDown event to find the nearest item in the up direction.

            VBC#C++F#JScript

            ListView iconListView = new ListView();
            TextBox previousItemBox = new TextBox();

            private void InitializeLocationSearchListView()
            {
                previousItemBox.Location = new Point(150, 20);

                // Create an image list for the icon ListView.
                iconListView.LargeImageList = new ImageList();
                iconListView.Height = 400;

                // Add an image to the ListView large icon list.
                iconListView.LargeImageList.Images.Add(
                    new Bitmap(typeof(Control), "Edit.bmp"));

                // Set the view to large icon and add some items with the image
                // in the image list.
                iconListView.View = View.LargeIcon;
                iconListView.Items.AddRange(new ListViewItem[]{
                    new ListViewItem("Amy Alberts", 0), 
                    new ListViewItem("Amy Recker", 0), 
                    new ListViewItem("Erin Hagens", 0), 
                    new ListViewItem("Barry Johnson", 0), 
                    new ListViewItem("Jay Hamlin", 0), 
                    new ListViewItem("Brian Valentine", 0), 
                    new ListViewItem("Brian Welker", 0), 
                    new ListViewItem("Daniel Weisman", 0) });
                this.Controls.Add(iconListView);
                this.Controls.Add(previousItemBox);

                // Handle the MouseDown event to capture user input.
               iconListView.MouseDown +=
                   new MouseEventHandler(iconListView_MouseDown);
                //iconListView.MouseWheel += new MouseEventHandler(iconListView_MouseWheel);   
            }

            void iconListView_MouseDown(object sender, MouseEventArgs e)
            {

                // Find the an item above where the user clicked.
                ListViewItem foundItem =
                    iconListView.FindNearestItem(SearchDirectionHint.Up, e.X, e.Y);

                // Display the results in a textbox..
                if (foundItem != null)
                    previousItemBox.Text = foundItem.Text;
                else
                    previousItemBox.Text = "No item found";
            }
             * 
             * 
             * 
             * 
             * */
        }

        private void textBox10_Validating(object sender, CancelEventArgs e)
        {
            statusCari = 2;
        }

        private void tampilLVKwi()
        {
            lvTampil3.Items.Clear();
            query = "SELECT DISTINCT kd_Cek, ket, periodeA, periodeB FROM A_CEK WHERE status = 0";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 0));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 3));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 1));
                    lvTampil3.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        public void showRekening(string id_Rek ,string noRek, string uraian, string id_KPA)
        {
            idRek = Convert.ToInt16(id_Rek);
            txtEnKdRek.Text = noRek;
            lUraian.Text = uraian;
            // Addition
            maskedTextBox1.Text = noRek;
            label54.Text = uraian;
            idKpa = Convert.ToInt16(id_KPA);
        }

        private void txtEnNoBukti_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
        }

        private void txtEnKdRek_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F8)
            {
                if ((cari = (fCari)alat.FormSudahDibuat(typeof(fCari))) == null)
                {
                    cari = new fCari();
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    //cari.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                    cari.groupBox1.Text = "REKENING";
                    cari.status = 1;
                    cari.query = "SELECT id_Reken, Kd_Reken, Rekening, KdKpa, NamaKpa, a.id_Kpa  FROM A_REKENING a, A_KPA b WHERE b.Id_Kpa = a.Id_Kpa AND Kd_Reken LIKE '%" + alat.FilterMBox(txtEnKdRek.Text).Trim() + "%'";
                    cari.StartPosition = FormStartPosition.CenterScreen;
                    cari.lvTampil.View = View.Details;
                    cari.lvTampil.AllowColumnReorder = true;
                    cari.lvTampil.FullRowSelect = true;
                    cari.lvTampil.GridLines = true;
                    cari.lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;
                    cari.lvTampil.MultiSelect = false;

                    ColumnHeader headeridKet = cari.lvTampil.Columns.Add("id_ket", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerkdRek = cari.lvTampil.Columns.Add("No Rekening", 30 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerUraian = cari.lvTampil.Columns.Add("Uraian", 30 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerKdKpa = cari.lvTampil.Columns.Add("KODE KPA", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);                    
                    ColumnHeader headerKpa = cari.lvTampil.Columns.Add("KPA", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerIDKpa = cari.lvTampil.Columns.Add("IDKPA", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

                    cari.lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);

                    cari.TopMost = true;
                    cari.ShowDialog();
                }
                else
                {
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    cari.TopMost = true;
                    cari.Select();
                }
            }
        }

        private void txtEnKdRek_TextChanged(object sender, EventArgs e)
        {
        }

        private void Page1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void txtEnKdRek_Validating(object sender, CancelEventArgs e)
        {
        }

        private bool cekSaldo(TextBox textboks)
        {
            bool yaTidak = true;
            query = "SELECT sisa, kd_reken, b.id_Kpa FROM A_PENERIMAAN a, A_REKENING b WHERE a.id_Kpa = b.id_Kpa AND b.id_reken = " + idRek + " AND idSumber = " + idSumber + " AND Dipakai = '-'";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                //MessageBox.Show(alat.pengecekField(pembaca, 1).ToString());
                //MessageBox.Show(alat.pengecekField(pembaca, 2).ToString());
                if (Convert.ToDecimal(alat.PengecekField(pembaca, 0)) < Convert.ToDecimal(textboks.Text))
                {
                    yaTidak = false;
                }
                pembaca.Close();
            }
            else
            {
                yaTidak = false;
            }
            koneksi.Close();
            return yaTidak;
        }

        private void cbEnSF_Validated(object sender, EventArgs e)
        {
            //MessageBox.Show("KPA: " + idKpa.ToString() + " SUMBER: " + idSumber.ToString() + " REK: " + idRek.ToString());
            if (cekSaldo(enJmlh) == false)
            {
                MessageBox.Show("SALDONYA KPA TERSEBUT KURANG!", "PERHATIAN");
                bersih2();
                txtEnNoBukti.Focus();
            }
        }

        private void cbEnSF_Validating(object sender, CancelEventArgs e)
        {
            koneksi.Open();
            pembaca = konek.MembacaData("SELECT * FROM A_SUMBER_DANA WHERE kdSumber = '" + cbEnSF.Text.Trim() + "'", koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                idSumber = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                lKet.Text = alat.PengecekField(pembaca, 2);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void bBatal_Click(object sender, EventArgs e)
        {
            if (idBayar == 0)
            {
                MessageBox.Show("Tidak bisa menghapus data, silahkan periksa kembali", "PERHATIAN");
            }
            else
            {
                koneksi.Open();
                konek.MasukkanData("UPDATE A_PENGELUARAN SET Batal = 1 WHERE Id_Bayar_Mst = " + idBayar + "", koneksi);
                MessageBox.Show("Hapus data sukses", "KONFIRMASI");
                koneksi.Close();
            }
            bersih2();
            tampilTabelEn();
            switchingButton(true);
        }

        private void ambilDariLv2()
        {
            query = "SELECT 	CONVERT(CHAR(10), TglBayar, 103), NoBayar, ketBayar, JmlRp, PPnRp, PphRp, Kd_Reken, KdKpa, kdSupp, NamaSupp, kdSumber, namaSumber, kdJenis " +
                    "FROM 	A_PENGELUARAN kel, A_REKENING rek, A_KPA kpa, A_SUPPLIER supp, A_SUMBER_DANA sumber, A_JENIS jen " +
                    "WHERE 	(rek.id_reken = kel.id_reken AND kpa.id_kpa = rek.id_kpa) AND supp.id_supplier = kel.id_supplier AND sumber.idSumber = kel.idSumber " +
                    "AND jen.id_jenis = kel.id_jenis AND id_bayar_mst = " + idBayar + "";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                string aneh = String.Format("{0: dd/MM/yy}", alat.PengecekField(pembaca, 0).Trim());
                dtpEnBayar.Text = aneh;
                //MessageBox.Show(String.Format("{0: MM/dd/yyyy}", alat.pengecekField(pembaca, 0)).Trim());
                //dtpEnBayar.Text = "12/20/2013";//String.Format(alat.pengecekField(pembaca, 0)).Trim();
                txtEnNoBukti.Text = alat.PengecekField(pembaca, 1);
                txtKetBayar.Text = alat.PengecekField(pembaca, 2);
                enJmlh.Text = alat.PengecekField(pembaca, 3);
                txtENPPN.Text = alat.PengecekField(pembaca, 4);
                txtEnPPh.Text = alat.PengecekField(pembaca, 5);
                txtEnKdRek.Text = alat.PengecekField(pembaca, 6);
                lUraian.Text = alat.PengecekField(pembaca, 7);
                txtEnKodeSup.Text = alat.PengecekField(pembaca, 8);
                lSupp.Text = alat.PengecekField(pembaca, 9);
                cbEnSF.Text = alat.PengecekField(pembaca, 10);
                lKet.Text = alat.PengecekField(pembaca, 11);
                cbEnJenis.Text = alat.PengecekField(pembaca, 12);
                switchingButton(false);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void lvTampil2_DoubleClick(object sender, EventArgs e)
        {
            ambilDariLv2();
            idBayar = Convert.ToInt16(lvTampil2.SelectedItems[0].SubItems[11].Text);
        }

        private void switchingButton(bool status)
        {
            if (status == true)
            {
                button1.Visible = false;
                bSimpan.Visible = true;
                button3.Visible = false;
                button4.Visible = true;
                button5.Enabled = true;
                bHapus.Enabled = false;
                button2.Enabled = false;
            }
            else
            {
                button1.Visible = true;
                bSimpan.Visible = false;
                button3.Visible = true;
                button4.Visible = false;
                button5.Enabled = false;
                bHapus.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bersih2();
            switchingButton(true);
        }

        #region THIS IS TEMPORARY (HOPEFULY) CODE
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("No Bukti Harap Diisi", "PERHATIAN");
                txtEnNoBukti.Focus();
            }
            else if (idRek == 0)
            {
                MessageBox.Show("No Rekening Harap Diperhatikan", "PERHATIAN");
                txtEnKdRek.Focus();
            }
            else if (idSupp == 0)
            {
                MessageBox.Show("Data Suplier harap Diperhatikan", "PERHATIAN");
                txtEnKodeSup.Focus();
            }
            else if (idSumber == 0)
            {
                MessageBox.Show("Sumber Dana harap Diperhatikan", "PERHATIAN");
                cbEnSF.Focus();
            }
            else
            {
                if (konek.CekFieldUnik("A_PENGELUARAN", "NoBayar", textBox3.Text.Trim() + " AND BATAL = 0") == true)
                {
                    MessageBox.Show("NO BAYAR TELAH DIGUNAKAN, SILAHKAN PILIH YANG LAIN!", "PERHATIAN");
                    textBox3.Focus();
                }
                //    else if (cekSaldo() == false)
                //    {
                //        MessageBox.Show("SALDONYA KPA TERSEBUT KURANG!", "PERHATIAN");
                //        bersih2();
                //        txtEnNoBukti.Focus();
                //    }
                else
                {
                    if (idJen == 0)
                    {
                    //                MessageBox.Show("Jenis harap Diperhatikan", "PERHATIAN");
                    //                cbEnJenis.Focus();
                    idJen = 1;
                    }
                    query = "INSERT INTO A_PENGELUARAN (TglBayar, NoBayar, Id_Reken, Id_Supplier, PPnRp, PPhRp, JmlRp, Lunas, " +
                        "idSumber, Id_Opp_En, Batal, Id_Jenis, Id_Opp_Ver, kwi, ketBayar) VALUES (CONVERT(SMALLDATETIME, '" + dateTimePicker5.Text + " " + DateTime.Now.ToLongTimeString() + " ', 103), '" + textBox3.Text.Trim() + "', " + idRek +
                        ", " + idSupp + ", " + maskedTextBox3.Text.Trim() + ", " + maskedTextBox4.Text.Trim() + ", " + maskedTextBox5.Text.Trim() + ", 0, '" + idSumber +
                        "', " + idOpp + ", 0," + idJen + "," + idOpp + ", 0, '" + textBox2.Text + "')";
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
                    koneksi.Close();
                    MessageBox.Show("PENYIMPANAN BERHASIL, TERIMAKASIH", "PERHATIAN");
                    bersih2();
                }
                tampilListView4();
            }
        }

        private void maskedTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string kd_rek = alat.FilterMBox(maskedTextBox1.Text);
            query = "SELECT id_reken, Rekening, KdKpa FROM A_REKENING a, A_KPA b WHERE b.Id_Kpa = a.Id_Kpa AND kd_Reken = '" + kd_rek.Trim() + "'";
            if (e.KeyChar == (char)13)
            {
                koneksi.Open();
                pembaca = konek.MembacaData(query, koneksi);
                if (pembaca.HasRows)
                {
                    pembaca.Read();
                    idRek = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                    label54.Text = alat.PengecekField(pembaca, 2);
                    pembaca.Close();
                }
                else
                {
                    MessageBox.Show("Rekening Dengan Kode Tersebut Tidak Ditemukan!!!");
                    maskedTextBox1.Focus();
                }
                koneksi.Close();
                //MessageBox.Show(query);
            }

        }

        private void maskedTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void maskedTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F8)
            {
                if ((cari = (fCari)alat.FormSudahDibuat(typeof(fCari))) == null)
                {
                    cari = new fCari();
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    //cari.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                    cari.groupBox1.Text = "REKENING";
                    cari.status = 1;
                    cari.query = "SELECT id_Reken, Kd_Reken, Rekening, KdKpa, NamaKpa, a.id_Kpa FROM A_REKENING a, A_KPA b WHERE b.Id_Kpa = a.Id_Kpa AND Kd_Reken LIKE '%" + alat.FilterMBox(maskedTextBox1.Text).Trim() + "%'";
                    cari.StartPosition = FormStartPosition.CenterScreen;
                    cari.lvTampil.View = View.Details;
                    cari.lvTampil.AllowColumnReorder = true;
                    cari.lvTampil.FullRowSelect = true;
                    cari.lvTampil.GridLines = true;
                    cari.lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;
                    cari.lvTampil.MultiSelect = false;

                    ColumnHeader headeridKet = cari.lvTampil.Columns.Add("id_ket", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerkdRek = cari.lvTampil.Columns.Add("No Rekening", 30 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerUraian = cari.lvTampil.Columns.Add("Uraian", 30 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerKdKpa = cari.lvTampil.Columns.Add("KODE KPA", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerKpa = cari.lvTampil.Columns.Add("KPA", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerIDKpa = cari.lvTampil.Columns.Add("IDKPA", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

                    cari.lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);

                    cari.TopMost = true;
                    cari.ShowDialog();
                }
                else
                {
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    cari.TopMost = true;
                    cari.Select();
                }
            }

        }

        private void ambilSupp2(System.Windows.Forms.ComboBox namaCombo)
        {
            query = "SELECT id_Supplier, NamaSupp FROM A_SUPPLIER WHERE kdSupp = '" + namaCombo.Text.Trim() + "'";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                idSupp = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                label51.Text = alat.PengecekField(pembaca, 1);
                pembaca.Close();
            }
            else
            {
                MessageBox.Show("Maaf Supplier / Rekanan dengan kode tersebut TIDAK DITEMUKAN!", "PERHATIAN");
                //namaCombo.Focus();
            }
            koneksi.Close();

        }
        private void comboBox2_Validating(object sender, CancelEventArgs e)
        {
            ambilSupp2(comboBox2);
        }
        private void comboBox4_Validated(object sender, EventArgs e)
        {
            //MessageBox.Show(idRek.ToString() + " " + idSupp.ToString() + " " + idSumber.ToString());
            //MessageBox.Show("KPA: " + idKpa.ToString() + " SUMBER: " + idSumber.ToString() + " REK: " + idRek.ToString());
            if (cekSaldo(maskedTextBox5) == false)
            {
                MessageBox.Show("SALDONYA KPA TERSEBUT KURANG!", "PERHATIAN");
                bersih2();
                textBox3.Focus();
            }
        }
        private void tampilListView4()
        {
            lvTampil4.Items.Clear();
            query = "SELECT CONVERT(CHAR(10), TglBayar, 103), NoBayar, kd_Reken, ketBayar, NamaSupp, PPnRp, PPhRp, JmlRp, lunas, kdSumber, Jenis, Id_Bayar_Mst " +
                "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE " +
                "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND (lunas = 0 AND batal = 0 AND kwi = 0) AND e.idSumber = a.idSumber) ORDER BY TglBayar DESC";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 0));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 3));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 4));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 5)))); //ppn
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 6)))); //pph
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 7)))); //jumlah
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 8));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 9));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 10));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 11));

                    lvTampil4.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (idBayar == 0)
            {
                MessageBox.Show("SILAHKAN MASUKAN DATA YANG INGIN DIBAYAR!", "PERINGATAN");
                txtEnNoBukti.Focus();
            }
            else
            {
                if (cekSaldo(maskedTextBox5) == false)
                {
                    MessageBox.Show("SALDONYA KPA TERSEBUT KURANG!", "PERHATIAN");
                    bersih2();
                    textBox3.Focus();
                }
                else
                {
                    query = "UPDATE A_PENGELUARAN SET TglBayar = CONVERT(SMALLDATETIME, '" + dateTimePicker5.Text + "', 103), Lunas = 1, Id_Opp_Ver = " + idOpp + " WHERE id_Bayar_mst = " + idBayar + " ";
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
                    koneksi.Close();
                    //updateAnggar(Convert.ToDecimal(enJmlh.Text), Convert.ToDecimal(dtpEnBayar.Text.Substring(6, 4)));
                    updateTerima(maskedTextBox5.Text, textBox3.Text, idOpp, idKpa, idSumber, dateTimePicker5.Text);
                    //updateTerima(Convert.ToDecimal(maskedTextBox5.Text), "aaaa", 4, 0, 0);
                    //MessageBox.Show(Convert.ToDecimal(maskedTextBox5.Text).ToString());
                    MessageBox.Show("DATA TELAH DISIMPAN, TERIMAKASIH", "PERHATIAN");
                    bersih2();
                    textBox3.Focus();
                }
            }
            tampilListView4();
            switchingButton(true);
        }
        private void updateTerima(string amountOF, string number, int idOPP, int idKPA, int idSUMBER, string DATETimePicker)
        {
            /* pake sp_keluar yg lama, yg tgl entry nya dientrykan dari sistem (bukan dari database)
	            @jmlKeluar numeric (13),
	            @ket text, 
	            @id_Opp int,
	            @id_Kpa int,
	            @idSumber int, 
                @tglTerima <-- sp yg lama udah ketambahan ini*/
            koneksi.Open();
            //konek.masukkanData("EXECUTE sp_keluar " + amountOF + ", '" +
            //    number + "', " + idOPP + ", " + idKPA + ", " + idSUMBER + "", koneksi);
            konek.MasukkanData("EXECUTE sp_keluar " + alat.FilterMBox(amountOF) + ", '" + number + "', " + idOPP + ", " +
                idKPA + ", " + idSUMBER + ", '" + DATETimePicker + " " + DateTime.Now.ToLongTimeString() + "'", koneksi);
            koneksi.Close();
            //MessageBox.Show(idOpp.ToString() + " " + idKpa + " " + idSumber.ToString());
        }
        private void lvTampil4_DoubleClick(object sender, EventArgs e)
        {
            query = "SELECT CONVERT(CHAR(10), TglBayar, 103), PPnRp, PPhRp, JmlRp, kd_Reken, ketBayar, KdSupp, NamaSupp, kdSumber, KdJenis, Jenis, " +
                "id_Bayar_Mst, a.id_Reken, noBayar, namaSumber, id_Kpa, a.idSumber FROM A_PENGELUARAN a, " +
                "A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE ((b.id_reken = a.id_reken AND c.id_supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND e.idSumber = a.idSumber" +
                " AND id_Bayar_Mst = " + lvTampil4.SelectedItems[0].SubItems[11].Text.Trim() + ")";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                switchingButton(false);
                dateTimePicker5.Text = String.Format("{0: dd/MM/yyyy}", alat.PengecekField(pembaca, 0));
                //MessageBox.Show(alat.pengecekField(pembaca, 0));
                textBox3.Text = alat.PengecekField(pembaca, 13);
                maskedTextBox5.Text = alat.PengecekField(pembaca, 3);
                maskedTextBox4.Text = alat.PengecekField(pembaca, 2);
                maskedTextBox3.Text = alat.PengecekField(pembaca, 1);
                maskedTextBox1.Text = alat.PengecekField(pembaca, 4);
                textBox2.Text = alat.PengecekField(pembaca, 5);
                comboBox2.Text = alat.PengecekField(pembaca, 6);
                label51.Text = alat.PengecekField(pembaca, 7);
                comboBox4.Text = alat.PengecekField(pembaca, 8);
                comboBox3.Text = alat.PengecekField(pembaca, 9);
                label48.Text = alat.PengecekField(pembaca, 10);
                idBayar = Convert.ToInt16(alat.PengecekField(pembaca, 11));
                idRek = Convert.ToInt16(alat.PengecekField(pembaca, 12));
                //txtEnNoBukti.Text = alat.pengecekField(pembaca, 13);
                //lKet.Text = alat.pengecekField(pembaca, 14);
                //MessageBox.Show("SF : " + sf + " idbayar : " + alat.pengecekField(pembaca, 11) + " idrek : " + alat.pengecekField(pembaca, 12) + " no bukti: " + alat.pengecekField(pembaca, 13));
                idKpa = Convert.ToInt16(alat.PengecekField(pembaca, 15));
                idSumber = Convert.ToInt16(alat.PengecekField(pembaca, 16));
                pembaca.Close();
            }
            koneksi.Close();
            //MessageBox.Show(maskedTextBox5.Text + " " + textBox3.Text + " " + idOpp.ToString() + " " + 
            //    idKpa.ToString() + " " + idSumber.ToString());
        }
        private void comboBox4_Validating(object sender, CancelEventArgs e)
        {
            koneksi.Open();
            pembaca = konek.MembacaData("SELECT * FROM A_SUMBER_DANA WHERE kdSumber = '" + comboBox4.Text.Trim() + "'", koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                idSumber = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                label48.Text = alat.PengecekField(pembaca, 2);
                pembaca.Close();
            }
            koneksi.Close();
        }
        private void maskedTextBox5_Click(object sender, EventArgs e)
        {
            maskedTextBox5.SelectionStart = 0;
        }

        private void maskedTextBox3_Click(object sender, EventArgs e)
        {
            maskedTextBox3.SelectionStart = 0;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            bersih2();
            switchingButton(true);
        }
        private void comboBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void comboBox3_Validating(object sender, CancelEventArgs e)
        {
            query = "SELECT id_Jenis, Jenis FROM A_Jenis WHERE kdJenis = '" + comboBox3.Text.Trim() + "'";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                idJen = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                //lKet.Text = alat.pengecekField(pembaca, 1);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("No Bukti Harap Diisi", "PERHATIAN");
                txtEnNoBukti.Focus();
            }
            else if (idRek == 0)
            {
                MessageBox.Show("No Rekening Harap Diperhatikan", "PERHATIAN");
                txtEnKdRek.Focus();
            }
            else if (idSupp == 0)
            {
                MessageBox.Show("Data Suplier harap Diperhatikan", "PERHATIAN");
                txtEnKodeSup.Focus();
            }
            else if (idSumber == 0)
            {
                MessageBox.Show("Sumber Dana harap Diperhatikan", "PERHATIAN");
                cbEnSF.Focus();
            }
            else
            {
                if (konek.CekFieldUnik("A_PENGELUARAN", "NoBayar", textBox3.Text.Trim() + " AND BATAL = 0") == true)
                {
                    MessageBox.Show("NO BAYAR TELAH DIGUNAKAN, SILAHKAN PILIH YANG LAIN!", "PERHATIAN");
                    textBox3.Focus();
                }
                //    else if (cekSaldo() == false)
                //    {
                //        MessageBox.Show("SALDONYA KPA TERSEBUT KURANG!", "PERHATIAN");
                //        bersih2();
                //        txtEnNoBukti.Focus();
                //    }
                else
                {
                    if (idJen == 0)
                    {
                        //                MessageBox.Show("Jenis harap Diperhatikan", "PERHATIAN");
                        //                cbEnJenis.Focus();
                        idJen = 1;
                    }
                    query = "INSERT INTO A_PENGELUARAN (TglBayar, NoBayar, Id_Reken, Id_Supplier, PPnRp, PPhRp, JmlRp, Lunas, " +
                        "idSumber, Id_Opp_En, Batal, Id_Jenis, Id_Opp_Ver, kwi, ketBayar) VALUES (CONVERT(SMALLDATETIME, '" + dateTimePicker5.Text + " " + DateTime.Now.ToLongTimeString() + " ', 103), '" + textBox3.Text.Trim() + "', " + idRek +
                        ", " + idSupp + ", " + maskedTextBox3.Text.Trim() + ", " + maskedTextBox4.Text.Trim() + ", " + maskedTextBox5.Text.Trim() + ", 1, " + idSumber +
                        ", " + idOpp + ", 0," + idJen + "," + idOpp + ", 0, '" + textBox2.Text + "')";
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
                    //MessageBox.Show(query);
                    koneksi.Close();
                    button5.Enabled = false;
                    Thread.Sleep(1000);
                    button5.Enabled = true;
                    updateTerima(maskedTextBox5.Text.Trim(), textBox3.Text.Trim(), idOpp, idKpa, idSumber, dateTimePicker5.Text);
                    //MessageBox.Show("JUMLAH RP: " + maskedTextBox5.Text.Trim() + " NO BUKTI: " + textBox3.Text.Trim() + " " 
                    //    + " OPP: " + idOpp.ToString() + " KPA: " + idKpa.ToString() + " ID SUMBER: " + idSumber.ToString() + "\n" + query);
                }
                MessageBox.Show("PENYIMPANAN BERHASIL, TERIMAKASIH", "PERHATIAN");
                bersih2();
                tampilListView4();
            }
        }

        #endregion

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            //string[] partNumbers = { "1298-673-4192", "A08Z-931-468A", 
            //                  "_A90-123-129X", "12345-KKA-1230", 
            //                  "0919-2893-1256" };
            //string pattern = @"^[a-zA-Z0-9]\d{2}[a-zA-Z0-9](-\d{3}){2}[A-Za-z0-9]$";
            //foreach (string partNumber in partNumbers)
            //    //Console.WriteLine("{0} {1} a valid part number.",
            //    //                  partNumber,
            //    //                  System.Text.RegularExpressions.Regex.IsMatch(partNumber, pattern) ? "is" : "is not");
            //MessageBox.Show("{0} {1} a valid part number." +
            //                      partNumber +
            //                       System.Text.RegularExpressions.Regex.IsMatch(partNumber, pattern) ? + "is" + : + "is not");
            e.Handled = alat.Validate(e.KeyChar);
        }

        private void enJmlh_Validating(object sender, CancelEventArgs e)
        {
            if (enJmlh.Text == "")
                enJmlh.Text = "0";
        }

        private void txtENPPN_Validating(object sender, CancelEventArgs e)
        {
            if (txtENPPN.Text == "")
                txtENPPN.Text = "0";
        }

        private void txtEnPPh_Validating(object sender, CancelEventArgs e)
        {
            if (txtEnPPh.Text == "")
                txtEnPPh.Text = "0";
        }

        private void maskedTextBox5_Leave(object sender, EventArgs e)
        {
            if (maskedTextBox5.Text == "")
                maskedTextBox5.Text = "0";
        }

        private void maskedTextBox3_Leave(object sender, EventArgs e)
        {
            if (maskedTextBox3.Text == "")
                maskedTextBox3.Text = "0";
        }

        private void maskedTextBox4_Leave(object sender, EventArgs e)
        {
            if (maskedTextBox4.Text == "")
                maskedTextBox4.Text = "0";
        }

        private void maskedTextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
            e.Handled = alat.Validate(e.KeyChar);
        }

        private void maskedTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
            e.Handled = alat.Validate(e.KeyChar);
        }

        private void maskedTextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
            e.Handled = alat.Validate(e.KeyChar);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
        }

        private void cbEnJenis_Leave(object sender, EventArgs e)
        {
            query = "SELECT id_Jenis, Jenis FROM A_Jenis WHERE kdJenis = '" + cbEnJenis.Text.Trim() + "'";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                idJen = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                //lKet.Text = alat.pengecekField(pembaca, 1);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {

        }


    }
}
