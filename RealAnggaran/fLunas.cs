using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fLunas : Form
    {
        readonly SqlConnection koneksi;
        SqlDataReader _pembaca = null;
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        string query = null;
        public int idOpp = 0;
        int idBayar = 0;
        string sf = null;
        int idRek = 0;
        //int statusCari = 0;
        string idCek = "";
        int idKpa = 0;
        int idSumber = 0;

        public fLunas()
        {
            InitializeComponent();
            dtpEnBayar.CustomFormat = "dd/MM/yyyy";
            dtpEnBayar.Format = DateTimePickerFormat.Custom;
            dateTimePicker6.CustomFormat = "dd/MM/yyyy";
            dateTimePicker6.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker3.CustomFormat = "dd/MM/yyyy";
            dateTimePicker3.Format = DateTimePickerFormat.Custom;
            koneksi = konek.KonekDb();
        }

        private void bersih2()
        {
            txtEnNoBukti.Focus();
            alat.CleaningService(panel1);
            alat.CleaningService(panel2);
            alat.CleaningService(panel3);
            alat.CleaningService(panel4);
            lUraian.Text = "[uraian]";
            lSupp.Text = "[nama_suplier]";
            lKet.Text = "[ket_jenis]";
            //lUraianV.Text = "[uraian]";
            //lSuppEn.Text = "[nama_suplier]";
            //lKetV.Text = "[ket_jenis]";
            idBayar = 0;
            idRek = 0;
            //idSupp = 0;
            //idOpp = 0;
            //idJen = 0;
            sf = "";
            label7.Text = "[nama_suplier]";
            //statusCari = 0;
            lvTampil.Items.Clear();
            lvTampil2.Items.Clear();
            lvTampil3.Items.Clear();
            idCek = "";
            idKpa = 0;
            idSumber = 0;
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

            ColumnHeader headeridBayarMst = this.lvTampil2.Columns.Add("idBayarMst", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerTgl2 = this.lvTampil2.Columns.Add("Tanggal", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoKwi2 = this.lvTampil2.Columns.Add("Nomor Kwitansi", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoRek2 = this.lvTampil2.Columns.Add("No Rekening", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKetRek2 = this.lvTampil2.Columns.Add("Uraian", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSupp2 = this.lvTampil2.Columns.Add("Supplier", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerPPn2 = this.lvTampil2.Columns.Add("PPn (Rp)", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerPPh2 = this.lvTampil2.Columns.Add("PPh (Rp)", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJum2 = this.lvTampil2.Columns.Add("Jumlah", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerStatus2 = this.lvTampil2.Columns.Add("Status", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSubFung2 = this.lvTampil2.Columns.Add("Sumber", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJenis2 = this.lvTampil2.Columns.Add("Jenis", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

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
            ColumnHeader headerTgl = this.lvTampil.Columns.Add("Tanggal", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoKwi = this.lvTampil.Columns.Add("No Kwitansi", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoRek = this.lvTampil.Columns.Add("No Rekening", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);            
            ColumnHeader headerSupp = this.lvTampil.Columns.Add("Supplier", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJum = this.lvTampil.Columns.Add("Jumlah (Rp)", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerIdRek = this.lvTampil.Columns.Add("idRek", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headersf = this.lvTampil.Columns.Add("sf", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

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
        }

        private void tampilTabelEn()
        {
            lvTampil2.Items.Clear();
            query = "SELECT id_Bayar_Mst, CONVERT(CHAR(10), TglBayar, 103), NoBayar, kd_Reken, ketBayar, NamaSupp, PPnRp, PPhRp, JmlRp, lunas, kdSumber, Jenis " +
                "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE " +
                "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND (lunas = 0 AND batal = 0 AND kwi = 0) AND e.idSumber = a.idSumber) ORDER BY TglBayar DESC";
            koneksi.Open();
            _pembaca = konek.MembacaData(query, koneksi);
            if (_pembaca.HasRows)
            {
                while (_pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(_pembaca, 0));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 3));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 4));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 5));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 6));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 7));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 8));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 9));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 10));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 11));

                    lvTampil2.Items.Add(lvItem);
                }
                _pembaca.Close();
            }
            koneksi.Close();
        }

        private bool ambilDataBayar(string noBayar, int idBayarMst)
        {
            bool dataAda = false;
            if (idBayarMst < 1)
                query = "SELECT CONVERT(CHAR(10), TglBayar, 103), PPnRp, PPhRp, JmlRp, kd_Reken, ketBayar, KdSupp, NamaSupp, kdSumber, KdJenis, Jenis, " +
                    "id_Bayar_Mst, a.id_Reken, noBayar, namaSumber, id_Kpa, a.idSumber FROM A_PENGELUARAN a, " +
                    "A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE ((b.id_reken = a.id_reken AND c.id_supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND e.idSumber = a.idSumber" +
                    " AND NoBayar = '" + noBayar + "')";
            else
                query = "SELECT CONVERT(CHAR(10), TglBayar, 103), PPnRp, PPhRp, JmlRp, kd_Reken, ketBayar, KdSupp, NamaSupp, kdSumber, KdJenis, Jenis, " +
                    "id_Bayar_Mst, a.id_Reken, noBayar, namaSumber, id_Kpa, a.idSumber FROM A_PENGELUARAN a, " +
                    "A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE ((b.id_reken = a.id_reken AND c.id_supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND e.idSumber = a.idSumber" +
                    " AND id_Bayar_Mst = " + idBayarMst + ")";
            koneksi.Open();
            _pembaca = konek.MembacaData(query, koneksi);
            if (_pembaca.HasRows)
            {
                _pembaca.Read();
                dtpEnBayar.Text = alat.PengecekField(_pembaca, 0);
                dateTimePicker6.Text = alat.PengecekField(_pembaca, 0);
                txtEnBayar.Text = Convert.ToString(Convert.ToDecimal(alat.PengecekField(_pembaca, 1)) + Convert.ToDecimal(alat.PengecekField(_pembaca, 2)) +
                    Convert.ToDecimal(alat.PengecekField(_pembaca, 3)));
                txtENPPN.Text = alat.PengecekField(_pembaca, 1);
                txtEnPPh.Text = alat.PengecekField(_pembaca, 2);
                enJmlh.Text = alat.PengecekField(_pembaca, 3);
                txtEnKdRek.Text = alat.PengecekField(_pembaca, 4);
                txtKetBayar.Text = alat.PengecekField(_pembaca, 5);
                txtEnKodeSup.Text = alat.PengecekField(_pembaca, 6);
                lSupp.Text = alat.PengecekField(_pembaca, 7);
                cbEnSF.Text = alat.PengecekField(_pembaca, 8);
//                if (alat.pengecekField(pembaca, 8).Trim() == "S")
//                {
//                    cbEnSF.Text = "SUBSIDI";
////                    sf = "s";
//                }
//                else
//                {
//                    cbEnSF.Text = "FUNGSIONAL";
////                    sf = "f";
//                }
                //comboBox4.Text = alat.pengecekField(pembaca, 8);
                cbEnJenis.Text = alat.PengecekField(_pembaca, 9);
                lKet.Text = alat.PengecekField(_pembaca, 10);
                idBayar = Convert.ToInt16(alat.PengecekField(_pembaca, 11));
                idRek = Convert.ToInt16(alat.PengecekField(_pembaca, 12));
                txtEnNoBukti.Text = alat.PengecekField(_pembaca, 13);
                lKet.Text = alat.PengecekField(_pembaca, 14);
                //MessageBox.Show("SF : " + sf + " idbayar : " + alat.pengecekField(pembaca, 11) + " idrek : " + alat.pengecekField(pembaca, 12) + " no bukti: " + alat.pengecekField(pembaca, 13));
                idKpa = Convert.ToInt16(alat.PengecekField(_pembaca, 15));
                idSumber = Convert.ToInt16(alat.PengecekField(_pembaca, 16));
                _pembaca.Close();
                dataAda = true;
                //statusCari = 1;
            }
            else
            {
                //MessageBox.Show("Data dengan Nomor Pembayaran tersebut, TIDAK DITEMUKAN!", "PERHATIAN");
                //textBox10.Focus();
                //bersih2();
                dataAda = false;
            }
            koneksi.Close();
            return dataAda;
        }

        private void filterLVKwitasi(int statusPencarian)
        {
            lvTampil2.Items.Clear();
            if (statusPencarian == 1)
                query = "SELECT id_Bayar_Mst, CONVERT(CHAR(10), TglBayar, 103), NoBayar, kd_Reken, ketBayar, NamaSupp, PPnRp, PPhRp, JmlRp, lunas, kdSumber, Jenis " +
                    "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE " +
                    "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND (lunas = 0 AND batal = 0 AND kwi = 0 AND e.idSumber = a.idSumber) AND NoBayar LIKE '" + textBox10.Text + "%')";
            else if (statusPencarian == 2)
            {
                query = "SELECT id_Bayar_Mst, CONVERT(CHAR(10), TglBayar, 103), NoBayar, kd_Reken, ketBayar, NamaSupp, PPnRp, PPhRp, JmlRp, lunas, kdSumber, Jenis " +
                    "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_SUMBER_DANA e WHERE " +
                    "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis) AND (lunas = 0 AND batal = 0 AND kwi = 0 AND e.idSumber = a.idSumber) AND (TglBayar BETWEEN '" +
                    dtpEnBayar.Text + "' AND '" + dateTimePicker6.Text + "'))";
            }
            koneksi.Open();
            _pembaca = konek.MembacaData(query, koneksi);
            if (_pembaca.HasRows)
            {
                while (_pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(_pembaca, 0));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 3));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 4));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 5));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 6));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 7));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 8));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 9));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 10));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 11));

                    lvTampil2.Items.Add(lvItem);
                }
                _pembaca.Close();
            }

            koneksi.Close();
        }

        private void filterLVCek(int statusPencarian)
        {
            lvTampil3.Items.Clear();
            lvTampil.Items.Clear();
            if (statusPencarian == 3)
                query = "SELECT DISTINCT kd_Cek, ket, periodeA, periodeB " +
                    "FROM A_CEK WHERE " +
                    "status = 0 AND kd_Cek LIKE '" + textBox1.Text + "%'";
            else if (statusPencarian == 4)
            {
                query = "SELECT DISTINCT kd_Cek, ket, periodeA, periodeB " +
                    "FROM A_CEK WHERE " +
                    "status = 0 AND periodeA BETWEEN '" +
                    dateTimePicker1.Text + "' AND '" + dateTimePicker3.Text + "'";
            }
            else if (statusPencarian == 5)
            {
                query = "SELECT DISTINCT kd_Cek, ket, periodeA, periodeB " +
                    "FROM A_CEK WHERE " +
                    "status = 0";
            }

            koneksi.Open();
            _pembaca = konek.MembacaData(query, koneksi);
            if (_pembaca.HasRows)
            {
                while (_pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(_pembaca, 0));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 3));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 1));
                    //lvItem.SubItems.Add(alat.pengecekField(pembaca, 2));

                    lvTampil3.Items.Add(lvItem);
                }
                _pembaca.Close();
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
        private bool cekSaldo()
        {
            query = "SELECT sisa FROM A_PENERIMAAN a, A_REKENING b WHERE a.id_Kpa = b.id_Kpa AND b.id_reken = " + idRek + " AND idSumber = " + idSumber + " AND Dipakai = '-'";
            bool yaTidak = true;
            koneksi.Open();
            _pembaca = konek.MembacaData(query, koneksi);
            if (_pembaca.HasRows)
            {
                _pembaca.Read();
                if (Convert.ToDecimal(alat.PengecekField(_pembaca, 0)) < Convert.ToDecimal(enJmlh.Text))
                {
                    yaTidak = false;
                }
                _pembaca.Close();
            }
            else
            {
                yaTidak = false;
            }
            koneksi.Close();
            return yaTidak;
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
            if (idBayar == 0)
            {
                MessageBox.Show("SILAHKAN MASUKAN DATA YANG INGIN DIBAYAR!", "PERINGATAN");
                txtEnNoBukti.Focus();
            }
            else
            {
                if (cekSaldo() == false)
                {
                    MessageBox.Show("SALDONYA KPA TERSEBUT KURANG!", "PERHATIAN");
                    bersih2();
                    txtEnNoBukti.Focus();
                }
                else
                {
                    query = "UPDATE A_PENGELUARAN SET TglBayar = GETDATE(), Lunas = 1, Id_Opp_Ver = " + idOpp + " WHERE id_Bayar_mst = " + idBayar + " ";
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
                    koneksi.Close();
                    //updateAnggar(Convert.ToDecimal(enJmlh.Text), Convert.ToDecimal(dtpEnBayar.Text.Substring(6, 4)));
                    updateTerima();
                    MessageBox.Show("DATA TELAH DISIMPAN, TERIMAKASIH", "PERHATIAN");
                    bersih2();
                    txtEnNoBukti.Focus();
                }
            }
            tampilTabelEn(); 
            //MessageBox.Show(alat.dateFormater(dtpEnBayar.Text));
        }

        private int cekRekAnggar(int Id_Rekening)
        {
            int id_Rek = 0;
            query = "SELECT Id_Reken FROM A_ANGGAR WHERE status = 1 AND Id_Reken = " + Id_Rekening + "";
            koneksi.Open();
            _pembaca = konek.MembacaData(query, koneksi);
            if (_pembaca.HasRows)
            {
                _pembaca.Read();
                id_Rek = Convert.ToInt16(alat.PengecekField(_pembaca,0));
                _pembaca.Close();
            }
            koneksi.Close();
            return id_Rek;
        }

        private void updateAnggar(decimal jumlah, decimal tahun)
        {
            int id_rek = cekRekAnggar(idRek);
            if (id_rek > 0)
            {
                if (sf == "s")
                    query = "UPDATE A_ANGGAR SET RealSubsi = RealSubsi + " + jumlah +
                        ", SisaSubsi = SisaSubsi - " + jumlah + ", TotAnggar = TotAnggar - " +
                        jumlah + " WHERE id_Reken = " + id_rek + "";
                else if (sf == "f")
                    query = "UPDATE A_ANGGAR SET RealFungsi = RealFungsi + " + jumlah +
                        ", SisaFungsi = SisaFungsi - " + jumlah + ", TotAnggar = TotAnggar - " +
                        jumlah + " WHERE id_Reken = " + id_rek + "";
            }
            else if (id_rek <= 0)
            {
                if (sf == "s")
                    query = "EXECUTE sp_sub_anggar " + idRek + ", " + tahun +
                        ", " + jumlah + "," + idOpp + "";
                else if (sf == "f")
                    query = "EXECUTE sp_fung_anggar " + idRek + ", " + tahun +
                        ", " + jumlah + "," + idOpp + "";
            }
            koneksi.Open();
            konek.MasukkanData(query, koneksi);
            koneksi.Close();
        }

        private void updateTerima()
        {
            koneksi.Open();
            konek.MasukkanData("EXECUTE sp_keluar " + Convert.ToDecimal(enJmlh.Text) + ", '" + 
                txtEnNoBukti.Text + "', " + idOpp + ", " + idKpa + ", " + idSumber + "", koneksi);
            koneksi.Close();
        }

        private void bKeluar2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bKeluar1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fLunas_Load(object sender, EventArgs e)
        {
            alat.DeSerial(this);
            bersih2();
            inisiasiListView();
            tampilTabelEn();
            filterLVCek(5);
        }

        private void dtpEnBayar_Validating(object sender, CancelEventArgs e)
        {
            if (dateTimePicker6.Value < dtpEnBayar.Value)
                dateTimePicker6.Value = dtpEnBayar.Value;
            filterLVKwitasi(2);
        }

        private void dateTimePicker6_Validating(object sender, CancelEventArgs e)
        {
            if (dateTimePicker6.Value < dtpEnBayar.Value)
                dateTimePicker6.Value = dtpEnBayar.Value;
            filterLVKwitasi(2);
        }

        private void dateTimePicker1_Validating(object sender, CancelEventArgs e)
        {
            if (dateTimePicker3.Value < dateTimePicker1.Value)
                dateTimePicker3.Value = dateTimePicker1.Value;
            filterLVKwitasi(4);
        }

        private void dateTimePicker3_Validating(object sender, CancelEventArgs e)
        {
            if (dateTimePicker3.Value < dateTimePicker1.Value)
                dateTimePicker3.Value = dateTimePicker1.Value;
            filterLVKwitasi(4);
        }

        private void txtEnNoBukti_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool status;
            if (e.KeyChar == (char)13)
            {
                status = ambilDataBayar(txtEnNoBukti.Text, 0);
                if (status == false)
                {
                    filterLVKwitasi(1);
                }
            }
        }

        private void txtEnNoBukti_Validating(object sender, CancelEventArgs e)
        {

        }

        private void lvTampil2_DoubleClick(object sender, EventArgs e)
        {
            ambilDataBayar("", Convert.ToInt16(lvTampil2.SelectedItems[0].Text));
        }

        private void dateTimePicker6_ValueChanged(object sender, EventArgs e)
        {
            //filterLVKwitasi(2);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                filterLVCek(3);
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == "Page1")
            {
                tampilTabelEn();
            }
            else
            {
                filterLVCek(5);
            }
        }

        private void isiDataCek(string kdCek)
        {
            lvTampil.Items.Clear();
            query = "SELECT d.id_Bayar_Mst, NoBayar, CONVERT(CHAR(10), TglBayar, 103), kd_Reken, NamaSupp, JmlRp, kd_cek, periodeA, periodeB, ket, b.Id_Reken, SubFung " +
                "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_CEK d WHERE " +
                "((b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier) AND (lunas = 0 AND d.batal = 0 AND kwi = 1 AND d.status = 0) AND" +
                " (a.Id_Bayar_Mst = d.Id_Bayar_Mst AND kd_Cek = '" + kdCek.Trim() + "')) ORDER BY TglBayar DESC";


            koneksi.Open();
            _pembaca = konek.MembacaData(query, koneksi);
            if (_pembaca.HasRows)
            {
                while (_pembaca.Read())
                {
                    dateTimePicker1.Text = alat.PengecekField(_pembaca, 7);
                    dateTimePicker3.Text = alat.PengecekField(_pembaca, 8);
                    textBox1.Text = alat.PengecekField(_pembaca, 6);
                    textBox2.Text = alat.PengecekField(_pembaca, 9);
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(_pembaca, 0));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 3));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 4));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 5));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 10));
                    lvItem.SubItems.Add(alat.PengecekField(_pembaca, 11).Trim());

                    lvTampil.Items.Add(lvItem);
                }

                _pembaca.Close();
            }

            koneksi.Close();
        }

        private void lvTampil3_DoubleClick(object sender, EventArgs e)
        {
            isiDataCek(lvTampil3.SelectedItems[0].Text.Trim());
            idCek = lvTampil3.SelectedItems[0].Text.Trim();
        }

        private void bLunas_Click(object sender, EventArgs e)
        {
            query = "UPDATE A_CEK SET Status = 1, id_Opp_Ver = " + idOpp + " WHERE kd_Cek = '" + idCek + "' ";
            if (idCek == "")
            {
                MessageBox.Show("Silahkan Masukkan data Cek!", "PERHATIAN");
                textBox1.Focus();
            }
            else
            {
                koneksi.Open();
                konek.MasukkanData(query, koneksi);
                koneksi.Close();
                RapidUpdate();
                MessageBox.Show("DATA TELAH DISIMPAN, TERIMAKASIH", "PERHATIAN");
                bersih2();
                textBox1.Focus();
            }
            filterLVCek(5);
            //for (int i = 0; i < lvTampil.Items.Count; i++)
            //{
            //    idRek = Convert.ToInt16(lvTampil.Items[i].SubItems[6].Text);
            //    MessageBox.Show(lvTampil.Items[i].SubItems[7].Text);
            //}
        }

        private void RapidUpdate()
        {
            //koneksi.Open();
            for (int i = 0; i < lvTampil.Items.Count; i++)
            {
                idRek = Convert.ToInt16(lvTampil.Items[i].SubItems[6].Text);
                sf = lvTampil.Items[i].SubItems[7].Text.ToLower();
                idRek = Convert.ToInt16((lvTampil.Items[i].SubItems[6].Text.Trim()));
                updateLunasKwitansi("UPDATE A_PENGELUARAN SET TglBayar = GETDATE(), Lunas = 1, Id_Opp_Ver = " + idOpp + " WHERE id_Bayar_mst = " +
                        lvTampil.Items[i].Text + "");
                koneksi.Open();
                konek.MembacaData(query, koneksi).Close();
                konek.MasukkanData("EXECUTE sp_keluar '" + dtpEnBayar.Text + " " + DateTime.Now.ToShortTimeString() + "', " +
                    Convert.ToDecimal(lvTampil.Items[i].SubItems[5].Text) + ", '" + lvTampil.Items[i].SubItems[2].Text + 
                    "', " + idOpp + "", koneksi);
                koneksi.Close();
                //updateAnggar(Convert.ToDecimal(lvTampil.Items[i].SubItems[5].Text), 
                //    Convert.ToDecimal(DateTime.Now.ToShortDateString().Substring(6, 4)));
            }
            //koneksi.Close();
        }

        private void updateLunasKwitansi(string QUERY)
        {
            koneksi.Open();
            konek.MembacaData(QUERY, koneksi);
            koneksi.Close();
        }

        private void lvTampil_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(lvTampil.SelectedItems[0].Text);
        }
    }
}
