using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Collections;
using System.Globalization;

namespace RealAnggaran.Revisi
{
    /* Revision april 26 2012
     * cuz we must read from external db, which is fetch from kasda.db, we should start from scrath, which are:
     * 1. Select record from BLJ_MASTER, BLJ_DETAIL, AKD_RINCIAN, AKD_SUPPLIER tables.
     * 2. Update field lunas in BLJ_MASTER table.
     * then we must merge the data from out .db into our, which are
     * 3. Insert into our table.
     * 4. Check if balance in our account is possible for the payment.
     * 5. Add the taxes.
     * 6. determine the KPA's.
     * 
     * */
    /* Revision 16/05/2012:
    codename: "CHANGES!!"
    Do:
    After some conversation with stakeholder, some change has been made:
    How:
    * Table pengeluaran, change pp1 into pphNo datatype varchar, change pp2 into pph
    * Var decimal payment = Sum the four type of taxation with ammount of payment
    * See if (deposit - payment > 0)
    * else messagebox.show('WARNING-NOT ENOUGH MONEY') 
    */
    /* Revision 23/05/2012:
    1. NTPN terlambat = user can later input the NTPN number, so we must program it to filter record based on nullnes
       of the ntpn field (or pph1 field)	
    2. PPh satu aja masukan pasal berapapun = leave the pph and pph2 field, we later use it to store pph and pph pasal
       data
    3. NoKwitansi format KPA\Tahun\Bulan\noUrut = generate noKwitansi based on those format mention
    4. NoRek ganti no panggil = no explanation needed
    */
    /* Revision 31/05/2012:
    1. Pergantian nama field di table pengeluaran, pp1 diganti NTPNPPn, pp2 diganti NTPNPPh, ditambah NoPPh	
    2. NTPN terlambat = user can later input the NTPN number, so we must program it to filter record based on nullnes
       of the ntpn field (or pph1 field)
   */
    public partial class fKasir : Form
    {
        #region Private Public variable
        cKonek konek = new cKonek();
        cAlat alat = new cAlat();
        SqlConnection koneksi;
        SqlDataReader reader = null;
        string query = null;
        //Buat sorting listView
        private ListViewColumnSorter lvwColumnSorter;
        private List<tampungKpa> tabelKpa;
        private List<tampungDF> tabelDF;
        int idSupplier = 0;
        int idRek = 0;
        public int idOpp = 0;
        int idSumber = 0;
        int idKPA = 0;
        ListViewItem selectedListView = null;
        int statusShowTable = 0;
        List<ListViewItem> lvItemGroup = new List<ListViewItem>();
        ToolTip mytoolTip = new ToolTip();
        // This for indentify if txtRekDowo is empy or not
        int txtRekDowoEmpyorNot = 0;

        #endregion
        public fKasir()
        {
            /* try to change current culture
             * id-ID       id-ID       Indonesian (Indonesia)
             * en-GB       en-GB       English (United Kingdom)
             * en-US       en-US       English (United States) 
             */
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            InitializeComponent();
            koneksi = konek.konekDB();

            //Component buat sorting listview
            lvwColumnSorter = new ListViewColumnSorter();
            this.lvTampil.ListViewItemSorter = lvwColumnSorter;
            //format datetime picker
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            tabelKpa = fillComboKPA();
            tabelDF = fillComboDF();
        }
        #region transact
        private void txtNoSPK_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                statusShowTable = 1;
                if (backgroundWorker1.IsBusy == false)
                    backgroundWorker1.RunWorkerAsync();
                if (txtNoSPK.Text.Trim() == "")
                {
                    clearance();
                }
                else
                {
                    //lvTampil.Items[0].SubItems[1].to
                    if (lvTampil.Items.Count > 0 && lvTampil.Items.Count < 2)
                    {
                        string String1 = filtertxt(lvTampil.Items[0].SubItems[7].Text);
                        string String2 = filtertxt(lvTampil.Items[0].SubItems[8].Text);
                        decimal a = decimal.Parse(String1.Remove(0, 3));
                        decimal b = decimal.Parse(String2.Remove(0, 3));

                        txtTglSPK.Text = lvTampil.Items[0].Text;
                        //txtNoSPK.Text = lvTampil.Items[0].SubItems[1].Text;
                        txtKetSPK.Text = lvTampil.Items[0].SubItems[2].Text;
                        txtJmlBayar.Text = string.Format(new System.Globalization.CultureInfo("id-ID"), "{0:n}", (a + b));
                        txtNoRek.Text = lvTampil.Items[0].SubItems[3].Text;
                        txtUraian.Text = lvTampil.Items[0].SubItems[4].Text;
                        txtSuplier.Text = lvTampil.Items[0].SubItems[5].Text;
                        txtSumberDana.Text = lvTampil.Items[0].SubItems[6].Text;
                        if (lvTampil.Items[0].SubItems[9].Text.Trim() == "Y")
                            lStatus.Text = "SUDAH BAYAR";
                        else
                            lStatus.Text = "BELUM BAYAR";
                        lNoBukti.Text = lvTampil.Items[0].SubItems[11].Text;
                        idSumber = selekIdSumber(txtSumberDana.Text.Trim());
                        idRek = Convert.ToInt16(lvTampil.Items[0].SubItems[12].Text.Trim());
                        idSupplier = Convert.ToInt16(lvTampil.Items[0].SubItems[13].Text.Trim());
                        if (lStatus.Text == "SUDAH BAYAR")
                            toggleDisable(false);
                        else
                            toggleDisable(true);
                    }
                    if (lvTampil.Items.Count <= 0)
                        MessageBox.Show("DATA TIDAK DITEMUKAN");
                }
            }
        }
        private void bSave_Click(object sender, EventArgs e)
        {
            //CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            //MessageBox.Show(filtertxt(currentCulture.));
            try
            {
                if (bSave.Text == "SIMPAN")
                {
                    if (comboBox1.Text.Trim() == "" | idKPA == 0 | idSumber == 0 | idSupplier == 0)
                    {
                        MessageBox.Show("Data Harap Dilengkapi", "PERHATIAN");
                        comboBox1.Focus();
                    }
                    else if (txtPPn.Text.Trim() == "")
                    {
                        MessageBox.Show("PPh Dilengkapi", "PERHATIAN");
                        txtPPn.Focus();
                    }
                    else if (txtPPn.Text.Trim() == "")
                    {
                        MessageBox.Show("PPn Harap Dilengkapi", "PERHATIAN");
                        txtPPn.Focus();
                    }
                    else if (txtNoKwitansi.Text.Trim() == "")
                    {
                        MessageBox.Show("No. Kwitansi harap diisi", "PERHATIAN");
                        txtNoKwitansi.Focus();
                    }
                    else if (txtRekDowo.Text.Trim() == "" ||
                        string.IsNullOrEmpty(txtRekDowo.Text.Trim()) ||
                        txtRekDowo.Text.Trim() == "-")
                    {
                        MessageBox.Show("No. Rekening Harap Diisi", "PERHATIAN");
                        txtRekDowoEmpyorNot = 1;
                        txtRekDowo.Focus();
                    }
                    //else if (txtNTPNPPh.Text == "") // Tidak udah dikontrol karena user boleh tidak memasukan dulu NTPN
                    //{
                    //    MessageBox.Show("PP1 Harap Dilengkapi", "PERHATIAN");
                    //    txtNTPNPPh.Focus();
                    //}
                    else if (txtNoPPh.Text.Trim() == "")
                    {
                        MessageBox.Show("No PPh Harap Dilengkapi", "PERHATIAN");
                        txtNoPPh.Focus();
                    }
                    else if (idOpp == 0)
                    {
                        MessageBox.Show("Operator id not supplied", "PERHATIAN");
                        comboBox1.Focus();
                    }
                    //else if (cekNoBayar() > 0) // Karena kode sasad cicing itu udah ga dipake, jadi kita ga perlu pake ini lagi
                    //{
                    //    MessageBox.Show("No Bayar melebihi quota yg telah ditetapkan, harap hubungi pak Aries", "PERHATIAN");
                    //    comboBox1.Focus();
                    //}
                    else if (cek_saldo() == false)
                    {
                        MessageBox.Show("Jumlah Saldo yang dianggarkan oleh KPA dan/atau pada SUMBER DANA ini\nTIDAK MENCUKUPI UNTUK MELAKUKAN PEMBAYARAN", "PERHATIAN");
                        comboBox1.Focus();
                    }

                    else
                    {
                        simpanData(0);
                        //MessageBox.Show(generateNoBukti() + " " + idOpp.ToString() + " " + idSupplier.ToString() + " " + idSumber.ToString());
                    }
                }
                else if (bSave.Text == "UPDATE")
                {
                    if (comboBox1.Text.Trim() == "" | idKPA == 0 | idSumber == 0 | idSupplier == 0)
                    {
                        MessageBox.Show("Data Harap Dilengkapi", "PERHATIAN");
                        comboBox1.Focus();
                    }
                    else if (txtPPn.Text.Trim() == "")
                    {
                        MessageBox.Show("PPh Dilengkapi", "PERHATIAN");
                        txtPPn.Focus();
                    }
                    else if (txtPPn.Text.Trim() == "")
                    {
                        MessageBox.Show("PPn Harap Dilengkapi", "PERHATIAN");
                        txtPPn.Focus();
                    }
                    else if (txtNoKwitansi.Text.Trim() == "")
                    {
                        MessageBox.Show("No. Kwitansi harap diisi", "PERHATIAN");
                        txtNoKwitansi.Focus();
                    }
                    else if (txtRekDowo.Text.Trim() == "" ||
                        string.IsNullOrEmpty(txtRekDowo.Text.Trim()) ||
                        txtRekDowo.Text.Trim() == "-")
                    {
                        MessageBox.Show("No. Rekening Harap Diisi", "PERHATIAN");
                        txtRekDowoEmpyorNot = 1;
                        txtRekDowo.Focus();
                    }
                    //else if (txtNTPNPPh.Text == "") // Tidak udah dikontrol karena user boleh tidak memasukan dulu NTPN
                    //{
                    //    MessageBox.Show("PP1 Harap Dilengkapi", "PERHATIAN");
                    //    txtNTPNPPh.Focus();
                    //}
                    else if (txtNoPPh.Text.Trim() == "")
                    {
                        MessageBox.Show("No PPh Harap Dilengkapi", "PERHATIAN");
                        txtNoPPh.Focus();
                    }
                    else if (idOpp == 0)
                    {
                        MessageBox.Show("Operator id not supplied", "PERHATIAN");
                        comboBox1.Focus();
                    }
                    //else if (cekNoBayar() > 0) // Karena kode sasad cicing itu udah ga dipake, jadi kita ga perlu pake ini lagi
                    //{
                    //    MessageBox.Show("No Bayar melebihi quota yg telah ditetapkan, harap hubungi pak Aries", "PERHATIAN");
                    //    comboBox1.Focus();
                    //}
                    else
                    {
                        simpanData(1);
                        //MessageBox.Show(generateNoBukti() + " " + idOpp.ToString() + " " + idSupplier.ToString() + " " + idSumber.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("asd" + ex.Message);
            }
        }
        private void simpanData(int statusTrans)
        {
            bSave.Enabled = false;
            //String noBukti = null;
            //noBukti = generateNoBukti();
            Thread.Sleep(500);
            koneksi.Open();
            SqlTransaction tran = koneksi.BeginTransaction();
            try
            {
                if (statusTrans == 0)
                {
                    insertTrans(tran);
                    Thread.Sleep(100);
                    updateKASDA(txtNoKwitansi.Text.Trim(), dateTimePicker1.Text.Trim() + " " +
                        string.Format("{0:HH:mm:ss}", DateTime.Now), txtNoSPK.Text.Trim(), tran);
                    Thread.Sleep(100);
                    updateBayar(tran);
                }
                else if (statusTrans == 1)
                    UpdateTrans(tran);
                tran.Commit();
                removeUsedList();
                MessageBox.Show("PENYIMPANAN BERHASIL, TERIMAKASIH", "PERHATIAN");
                clearance();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                tran.Rollback();
            }
            finally
            {
                koneksi.Close();
                bSave.Enabled = true;   
            }
        }
        private string filtertxt(string teks)
        {
            teks = teks.Replace(".", "");
            teks = teks.Replace(",", ".");
            return teks;
        }
        private void insertTrans(SqlTransaction trans)
        {
            query = @"INSERT INTO A_PENGELUARAN (TglBayar, NoBayar, Id_Reken, Id_Supplier, PPnRp, PPhRp, JmlRp, Lunas, " +
                "idSumber, Id_Opp_En, Batal, Id_Jenis, Id_Opp_Ver, kwi, ketBayar, NTPNPPn, NTPNPPh, PPhNo, NO_SPK) VALUES (CONVERT(DATETIME, '" + dateTimePicker1.Text +
                " " + string.Format("{0:HH:mm:ss}", DateTime.Now) + "', 103), '" + txtNoKwitansi.Text + "', " + idRek +
                ", " + idSupplier + ", " + filtertxt(txtPPn.Text.Trim()) + ", " + filtertxt(txtPPh.Text.Trim()) + ", " + filtertxt(txtJmlBayar.Text.Trim()) + ", 1, " + idSumber +
                ", " + idOpp + ", 0," + 1 + "," + idOpp + ", 0, '" + txtKetSPK.Text + "', '" + txtNTPNPPn.Text.Trim() + "', '" + txtNTPNPPh.Text.Trim() +
                "', '" + txtNoPPh.Text.Trim() + "', '" + txtNoSPK.Text + "')";
            //MessageBox.Show(query);
            //MessageBox.Show("ini query menyimpan ke REALANGGAR : " + query);
            konek.masukkanData(query, koneksi, trans);
            Thread.Sleep(100);
            if (txtRekDowoEmpyorNot > 0)
            {
                konek.masukkanData("UPDATE REALANGGAR..A_REKENING SET formatPanjang = '" + txtRekDowo.Text.Trim() + "' WHERE Kd_Reken = '" +
                    txtNoRek.Text.Trim() + "'", koneksi, trans);
            }
        }
        private void UpdateTrans(SqlTransaction trans)
        {
            query = @"UPDATE REALANGGAR.dbo.A_PENGELUARAN SET PPnRp = " + filtertxt(txtPPn.Text.Trim()) + ", PPhRp = " + filtertxt(txtPPh.Text.Trim()) +
                ", NTPNPPn = '" + txtNTPNPPn.Text.Trim() + "', NTPNPPh = '" + txtNTPNPPn.Text.Trim() + "', PPhNo = '" + txtNoPPh.Text.Trim() + "'" +
                ", TglBayar = CONVERT(DATETIME, '" + dateTimePicker1.Text + " " + string.Format("{0:HH:mm:ss}", DateTime.Now) + "', 103)" +
                ", NoBayar = '" + txtNoKwitansi.Text + "' WHERE No_SPK = '" + txtNoSPK.Text.Trim() + "'" +
                " UPDATE REALANGGAR.dbo.A_PENERIMAAN SET TglTerima = CONVERT(DATETIME, '" + dateTimePicker1.Text + " " + string.Format("{0:HH:mm:ss}", DateTime.Now) +
                "', 103), ket = '" + txtNoKwitansi.Text + "' WHERE ket = '" + lNoBukti.Text.Trim() + "'" +
                " UPDATE KASDA.dbo.Blj_Master Set no_bukti = '" + txtNoKwitansi.Text + "' WHERE No_SPK = '" + txtNoSPK.Text.Trim() + "'";
            MessageBox.Show(query);
            //MessageBox.Show("ini query menyimpan ke REALANGGAR : " + query);
            konek.masukkanData(query, koneksi, trans);
            Thread.Sleep(100);
            if (txtRekDowoEmpyorNot > 0)
            {
                konek.masukkanData("UPDATE REALANGGAR..A_REKENING SET formatPanjang = '" + txtRekDowo.Text.Trim() + "' WHERE Kd_Reken = '" +
                    txtNoRek.Text.Trim() + "'", koneksi, trans);
            }
        }
        private void updateKASDA(string noBukti, string datetime, string noSPK, SqlTransaction trans)
        {
            //koneksi.Open();
            query = @"UPDATE KASDA.dbo.BLJ_MASTER " +
                "SET No_Bukti = '" + txtNoKwitansi.Text.Trim() + "', Lunas = 'Y', TGL_SPJ = CONVERT(SMALLDATETIME, '" + datetime +
                "', 103) WHERE No_SPK = '" + noSPK + "'";
            //MessageBox.Show("ini query menyimpan ke KASDA : " + query);
            konek.masukkanData(query, koneksi, trans);
            Thread.Sleep(100);
            if (txtRekDowoEmpyorNot > 0)
            {
                konek.masukkanData("UPDATE KASDA..AKD_RINCIAN SET formatPanjang = '" + txtRekDowo.Text +
                    "' WHERE Id_Rinci_RS = '" + txtNoRek.Text + "'", koneksi, trans);
            }
            //koneksi.Close();
        }
        private void updateBayar(SqlTransaction trans)
        {
            /* pake sp_keluar yg lama, yg tgl entry nya dientrykan dari sistem (bukan dari database)
	            @jmlKeluar numeric (13),
	            @ket text, 
	            @id_Opp int,
	            @id_Kpa int,
	            @idSumber int, 
                @tglTerima <-- sp yg lama udah ketambahan ini*/

            //koneksi.Open();
            //konek.masukkanData("EXECUTE sp_keluar " + amountOF + ", '" +
            //    number + "', " + idOPP + ", " + idKPA + ", " + idSUMBER + "", koneksi);
            // PAKE SP_KELUAR YG LAMA;
            query = @"EXECUTE sp_keluar " + filtertxt(txtJmlBayar.Text.Trim()) + ", '" + txtNoKwitansi.Text.Trim() + "', " + idOpp + ", " +
                idKPA + ", " + idSumber + ", '" + dateTimePicker1.Text + " " + string.Format("{0:HH:mm:ss}", DateTime.Now) + "'";
            //MessageBox.Show("ini query menyimpan ke SP : " + query); // trouble here <--
            konek.masukkanData(query, koneksi, trans);
            //koneksi.Close();
            //MessageBox.Show(idOpp.ToString() + " " + idKpa + " " + idSumber.ToString());
        }
        private void lvTampil_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string String1 = lvTampil.SelectedItems[0].SubItems[7].Text;
            string String2 = lvTampil.SelectedItems[0].SubItems[8].Text;
            //MessageBox.Show(filtertxt(String1.Remove(0, 4)) + " " + String2);
            decimal a = decimal.Parse(filtertxt(String1.Remove(0, 4)));
            decimal b = decimal.Parse(filtertxt(String2.Remove(0, 4)));

            txtTglSPK.Text = lvTampil.SelectedItems[0].Text;
            txtNoSPK.Text = lvTampil.SelectedItems[0].SubItems[1].Text;
            txtKetSPK.Text = lvTampil.SelectedItems[0].SubItems[2].Text;
            txtJmlBayar.Text = string.Format(new System.Globalization.CultureInfo("id-ID"), "{0:n}", (a + b));
            txtNoRek.Text = lvTampil.SelectedItems[0].SubItems[3].Text;
            txtUraian.Text = lvTampil.SelectedItems[0].SubItems[4].Text;
            txtSuplier.Text = lvTampil.SelectedItems[0].SubItems[5].Text;
            txtSumberDana.Text = lvTampil.SelectedItems[0].SubItems[6].Text;
            if (lvTampil.SelectedItems[0].SubItems[9].Text.Trim() == "Y")
            {
                lStatus.Text = "SUDAH BAYAR";
                bSave.Text = "UPDATE";
            }
            else
            {
                lStatus.Text = "BELUM BAYAR";
                bSave.Text = "SIMPAN";
            }
            lNoBukti.Text = lvTampil.SelectedItems[0].SubItems[11].Text;
            idSumber = selekIdSumber(txtSumberDana.Text.Trim());
            idRek = Convert.ToInt16(lvTampil.SelectedItems[0].SubItems[12].Text.Trim());
            idSupplier = Convert.ToInt16(lvTampil.SelectedItems[0].SubItems[13].Text.Trim());
            //txtNTPNPPn.Text = lvTampil.SelectedItems[0].SubItems[14].Text.Trim();
            //txtNTPNPPh.Text = lvTampil.SelectedItems[0].SubItems[15].Text.Trim();
            //txtNoPPh.Text = lvTampil.SelectedItems[0].SubItems[16].Text.Trim();
            //idKPA = Convert.ToInt16(lvTampil.SelectedItems[0].SubItems[17].Text.Trim());
            //comboBox1.Text = lvTampil.SelectedItems[0].SubItems[18].Text.Trim();
            selectedListView = lvTampil.SelectedItems[0];
            //MessageBox.Show(lvTampil.SelectedItems[0].SubItems[12].Text +
            //    " " + lvTampil.SelectedItems[0].SubItems[13].Text + " " +
            //    lvTampil.SelectedItems[0].SubItems[13].Text);
        }
        private void fKasir_Load(object sender, EventArgs e)
        {
            clearance();
            inisiasiListView();
            statusShowTable = 0;
            toolStripProgressBar1.Visible = true;
            if (backgroundWorker1.IsBusy == false)
                backgroundWorker1.RunWorkerAsync();
            //foreach (var i in listku.i)
            //{
            //    MessageBox.Show(i.ToString());
            //}
        }
        private void bCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.Trim() != "")
            {
                ArrayList listku = searchinListUsingLinq(comboBox1.Text, 0);
                idKPA = Convert.ToInt16(listku[0].ToString());
                txtNoKwitansi.Text = generateNoKwitansi();
            }
            //MessageBox.Show(idKPA.ToString());
        }
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void txtPPn_KeyDown(object sender, KeyEventArgs e)
        {
            alat.filterTextBox(sender, e);
        }
        private void txtPPh_KeyDown(object sender, KeyEventArgs e)
        {
            alat.filterTextBox(sender, e);
        }
        private void txtPP1_KeyDown(object sender, KeyEventArgs e)
        {
            alat.filterTextBox(sender, e);
        }
        private void txtPP2_KeyDown(object sender, KeyEventArgs e)
        {
            alat.filterTextBox(sender, e);
        }
        private void lvTampil_MouseDown(object sender, MouseEventArgs e)
        {
            //if ((e.Button == MouseButtons.Right) && (lvTampil.GetItemAt(e.X, e.Y) != null)) //If it's the right button.
            if (!backgroundWorker1.IsBusy)
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(this.lvTampil, e.Location);
                }
            }
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvTampil.SelectedItems[0].SubItems[1].Text);
        }
        private void showToolTip(Control namaControl, string teks)
        {
            if (namaControl.Focused)
            {
                mytoolTip.ShowAlways = true; // to force it
                mytoolTip.Show(teks, namaControl);
            }
            //            Thread.Sleep(1500);
            //            mytoolTip.Active = false;
        }
        private void txtPPn_TextChanged(object sender, EventArgs e)
        {
            if (txtPPn.Text != "")
            //    showToolTip(this.txtPPn, alat.Terbilang(Convert.ToDouble(txtPPn.Text)));
            {
                //txtPPn.Text = string.Format("{0:n}", Convert.ToDecimal(txtPPn.Text));
            }
        }
        private void txtPPn_Leave(object sender, EventArgs e)
        {
            //mytoolTip.Hide(txtPPn);
            string temp = txtPPn.Text;
            if (txtPPn.Text == "")
                txtPPn.Text = "0";
            else
            {
                try
                {
                    txtPPn.Text = string.Format(new System.Globalization.CultureInfo("id-ID"), "{0:n}", Convert.ToDecimal(filtertxt(temp)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            temp = "0";
        }
        #endregion
        #region ======== TOOLS ==========
        private string generateNoKwitansi()
        {
            string noBukti = null; ;
            query = @"SELECT TOP 1 substring(NoBayar, 12, 7) AS 'A' FROM A_PENGELUARAN WHERE YEAR(TglBayar) = " + dateTimePicker1.Text.Substring(6, 4) +
                " ORDER BY A DESC";
            koneksi.Open();
            reader = konek.membacaData(query, koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                try
                {
                    noBukti = comboBox1.Text.Right1(2) + "/" + dateTimePicker1.Text.Substring(6, 4) + "/" + dateTimePicker1.Text.Substring(3, 2) + "/" +
                            (Convert.ToInt16(alat.pengecekField(reader, 0)) + 1).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal Membuat nomor otomatis!\nFormat sebelumnya" +
                        " tidak sesuai kesepakatan\nLanjutkan dengan penomoran manual.", ex.Message);
                    txtNoKwitansi.Focus();
                    koneksi.Close();
                }
                reader.Close();
            }
            else
            {
                noBukti = comboBox1.Text.Right1(2) + "/" + dateTimePicker1.Text.Substring(6, 4) + "/" + dateTimePicker1.Text.Substring(3, 2) + "/" + "1";
            }
            koneksi.Close();
            //MessageBox.Show(dateTimePicker1.Text.Substring(6, 4));
            //MessageBox.Show(comboBox1.Text + "/" + dateTimePicker1.Text.Substring(6, 4) + "/" + dateTimePicker1.Text.Substring(3, 2) + "/" + "1");
            return noBukti;
        }
        private void removeUsedList()
        {
            //if (selectedListView != null)
            //{
            foreach (ListViewItem item in lvTampil.Items)
            {
                if (item == selectedListView)
                    lvTampil.Items.Remove(item);
            }
            //}
            //selectedListView = null;
        }
        /*
        private string generateNoBukti()
        {
            /* Langkah2:
             * 1. Pilih record yg sesuai tahun sekarang
             * 2. Kalo ada lakukan increment pada top 1 no_bukti left(4) 
             * 3. Kalo tidak ada tambahi data tahun_sekarang + 0001
             * 4. Bullshit!! Ga dipakai
            string noBukti = null; ;
            query = @"select TOP 1 right(rtrim(no_bukti), 4) AS 'A', YEAR(Tgl_SP) AS 'B' , no_bukti from KASDA..BLJ_MASTER WHERE" +
                " YEAR(Tgl_SP) = YEAR(getdate()) ORDER BY A DESC";
            koneksi.Open();
            reader = konek.membacaData(query, koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                //noBukti = Convert.ToInt16(alat.pengecekField(reader, 1)).ToString() + 
                //    (Convert.ToInt16(alat.pengecekField(reader, 0)) + 1).ToString();
                //noBukti = (Convert.ToDecimal(alat.pengecekField(reader, 2)) + 1).ToString();
                noBukti = cekdigit(alat.pengecekField(reader, 1), Convert.ToInt16(alat.pengecekField(reader,0)));
                reader.Close();
            }
            else
            {
                noBukti = DateTime.Now.Year.ToString() + "0001";
            }
            koneksi.Close();
            return noBukti;
        }
        */
        private void clearance()
        {
            txtNoSPK.Text = ""; txtJmlBayar.Text = "0"; txtNoRek.Text = "";
            txtTglSPK.Text = ""; txtKetSPK.Text = ""; txtSuplier.Text = "";
            txtSumberDana.Text = ""; lNoBukti.Text = ""; lStatus.Text = "";
            txtPPn.Text = "0"; txtPPh.Text = "0"; txtNTPNPPh.Text = "-"; txtNoPPh.Text = "-";
            txtUraian.Text = ""; comboBox1.Text = ""; idSupplier = 0; idRek = 0;
            idSumber = 0; idKPA = 0; lStatus.Text = ""; lNoBukti.Text = ""; lTerbilang.Text = "(-)";
            txtNoKwitansi.Text = ""; txtNTPNPPn.Text = "-"; txtDetailRek.Text = "";
            txtRekDowoEmpyorNot = 0; comboDF.Text = "";
            // special disable for comboDF
            comboDF.Enabled = false;
            // special for switching saveButton
            bSave.Text = "SIMPAN";
        }
        private string cekdigit(string awal, int akhir)
        {
            string nomer = null; ;
            if (akhir < 9)
                nomer = awal.ToString() + "000" + (akhir + 1).ToString();
            else if (akhir < 99)
                nomer = awal.ToString() + "00" + (akhir + 1).ToString();
            else if (akhir < 999)
                nomer = awal.ToString() + "0" + (akhir + 1).ToString();
            else if (akhir < 9999)
                nomer = awal.ToString() + (akhir + 1).ToString();
            else if (akhir > 9999)
                nomer = "Gagal";
            return nomer;
        }
        private int selekIdSumber(string namaSumber)
        {
            if (koneksi.State == ConnectionState.Closed)
                koneksi.Open();
            reader = konek.membacaData("SELECT idSumber FROM A_SUMBER_DANA WHERE namaSumber = '" +
                namaSumber.Trim() + "'", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                idSumber = Convert.ToInt16(alat.pengecekField(reader, 0));
                reader.Close();
            }
            if (koneksi.State == ConnectionState.Open)
                koneksi.Close();
            return idSumber;
        }
        /*
        private int cekNoBayar()
        {
            int status = 0;
            koneksi.Open();
            reader = konek.membacaData("select TOP 1 " +
                "right(rtrim(no_bukti), 4) AS 'A' from KASDA..BLJ_MASTER WHERE Tahun = YEAR(getdate()) ORDER BY A DESC", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                if (Convert.ToInt16(alat.pengecekField(reader, 0)) > 9989)
                {
                    status = 1;
                }
                reader.Close();
            }
            koneksi.Close();
            return status;
        }
        */
        private void lvTampil_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == System.Windows.Forms.SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lvTampil.Sort();
            //Found a new way = WEIRD WAY
            // Determine whether the column is the same as the last column clicked.  
            //if (e.Column != sortColumn)
            //{
            //    // Set the sort column to the new column.  
            //    sortColumn = e.Column;
            //    // Set the sort order to ascending by default.  
            //    lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;
            //}
            //else
            //{
            //    // Determine what the last sort order was and change it.  
            //    if (lvTampil.Sorting == System.Windows.Forms.SortOrder.Ascending)
            //        lvTampil.Sorting = System.Windows.Forms.SortOrder.Descending;
            //    else
            //        lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;
            //}
            //// Call the sort method to manually sort.  
            //lvTampil.Sort();
            //// Set the ListViewItemSorter property to a new ListViewItemComparer  
            //// object.  
            //this.lvTampil.ListViewItemSorter = new cListViewComparer(e.Column,
            //                                                  lvTampil.Sorting);  
        }
        private void inisiasiListView()
        {
            // ============================= Front Kwitansi ===========================================
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

            ColumnHeader headerTglSPK = this.lvTampil.Columns.Add("Tgl SPK", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoSPK = this.lvTampil.Columns.Add("Nomor SPK", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKetSPK = this.lvTampil.Columns.Add("Keterangan", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoRek = this.lvTampil.Columns.Add("Kode Panggil Rek", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKetRek = this.lvTampil.Columns.Add("Uraian", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSupp = this.lvTampil.Columns.Add("Supplier", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSumberDana = this.lvTampil.Columns.Add("Sumber Dana", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJumF = this.lvTampil.Columns.Add("Jumlah FUNGSIONAL", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJumS = this.lvTampil.Columns.Add("Jumlah SUBSIDI", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerStatus = this.lvTampil.Columns.Add("Status", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerTglSPJ = this.lvTampil.Columns.Add("Tgl SPJ", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJenis = this.lvTampil.Columns.Add("No Bukti", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headeridRek = this.lvTampil.Columns.Add("idRek", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headeridSupplier = this.lvTampil.Columns.Add("idSupplier", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headeridKpa = this.lvTampil.Columns.Add("idKpa", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNoKpa = this.lvTampil.Columns.Add("noKpa", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNTPNn = this.lvTampil.Columns.Add("NTPNn", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNTPNh = this.lvTampil.Columns.Add("NTPNh", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerPPno = this.lvTampil.Columns.Add("PPno", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);


            lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);
        }
        public static void listViewAddItem(ListView varListView, ListViewItem item)
        {
            if (varListView.InvokeRequired)
            {
                varListView.BeginInvoke(new MethodInvoker(() => listViewAddItem(varListView, item)));
            }
            else
            {
                varListView.Items.Add(item);
            }
        }
        /*
         * showTable -- Ga dipakai
        private void showTable(string kriteria)
        {
            lvTampil.Items.Clear();
            if (kriteria.Trim() == "")
            query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D " +
                "WHERE B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier " +
                "ORDER BY A.Tgl_SP";
            else
            query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D " +
                "WHERE A.No_SPK = '" + kriteria + "' AND B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier " +
                "ORDER BY A.Tgl_SP";
            koneksi.Open();
            reader = konek.membacaData(query, koneksi);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.pengecekField(reader, 1));
                    lvItem.Font = new Font("Arial", 11, FontStyle.Bold | FontStyle.Italic);
                    lvItem.SubItems.Add(alat.pengecekField(reader, 0)); // tglSPK
                    lvItem.SubItems.Add(alat.pengecekField(reader, 2)); // noSPK
                    lvItem.SubItems.Add(alat.pengecekField(reader, 3)); // Ket Spk
                    lvItem.SubItems.Add(alat.pengecekField(reader, 4));
                    lvItem.SubItems.Add(alat.pengecekField(reader, 5));
                    // buat ngecek status:
                    if (Convert.ToDecimal(alat.pengecekField(reader, 6)) == 0 && Convert.ToDecimal(alat.pengecekField(reader, 7)) > 0)
                        lvItem.SubItems.Add("SUBSIDI"); // STATUS
                    else
                        lvItem.SubItems.Add("FUNGSIONAL"); // STATUS
                    lvItem.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), 
                        "Rp. {0:n}", Convert.ToDecimal(alat.pengecekField(reader, 6)))); //jumlah FUNG
                    lvItem.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), 
                        "Rp. {0:n}", Convert.ToDecimal(alat.pengecekField(reader, 7)))); //jumlah SUBS
                    lvItem.SubItems.Add(alat.pengecekField(reader, 8));
                    lvItem.SubItems.Add(alat.pengecekField(reader, 9)); // tgl_SPJ
                    lvItem.SubItems.Add(alat.pengecekField(reader, 10));
                    lvItem.SubItems.Add(alat.pengecekField(reader, 11));
                    lvItem.SubItems.Add(alat.pengecekField(reader, 12));
                    //lvTampil.Items.Add(lvItem);
                    listViewAddItem(lvTampil, lvItem);
                }
                reader.Close();
            }
            koneksi.Close();
        }
        */
        # region Berkutat dengan LIST<> dan LINQ
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
        private class tampungDF
        {
            //Alternatif buat class dengan atau tanpa constructor
            public tampungDF(string idDF, string kdDF, string namaDF) //
            {                                                             // Jika tidak menggunakan constructor      
                this.idDF = idDF;                                       // hapus pada baris yg di comment 
                this.kdDF = kdDF;                                       //  
                this.namaDF = namaDF;                                   //  
            }
            public string idDF { set; get; }
            public string kdDF { set; get; }
            public string namaDF { set; get; }
        }           
        private List<tampungKpa> fillComboKPA()
        {
            List<tampungKpa> grupSetting = new List<tampungKpa>();

            koneksi.Open();
            reader = konek.membacaData("SELECT * FROM A_KPA", koneksi);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    comboBox1.Items.Add(alat.pengecekField(reader, 1));
                    //tampungKpa itemKpa = new tampungKpa();
                    //itemKpa.idKpa = alat.pengecekField(reader, 0);
                    //itemKpa.kdKpa = alat.pengecekField(reader, 1);
                    //itemKpa.namaKpa = alat.pengecekField(reader, 2);
                    //grupSetting.Add(itemKpa);
                    // Alternatif add item ke lis, jika menggunakan deklarasi class langsung, 
                    // constructor class yg digunakan harus menggandung
                    grupSetting.Add(new tampungKpa(alat.pengecekField(reader, 0), alat.pengecekField(reader, 1), alat.pengecekField(reader, 2)));
                }
                reader.Close();
            }
            koneksi.Close();
            return grupSetting;
        }
        private List<tampungDF> fillComboDF()
        {
            List<tampungDF> grupSetting = new List<tampungDF>();

            koneksi.Open();
            reader = konek.membacaData("SELECT * FROM A_SUMBER_DANA", koneksi);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    comboDF.Items.Add(alat.pengecekField(reader, 1));
                    //tampungKpa itemKpa = new tampungKpa();
                    //itemKpa.idKpa = alat.pengecekField(reader, 0);
                    //itemKpa.kdKpa = alat.pengecekField(reader, 1);
                    //itemKpa.namaKpa = alat.pengecekField(reader, 2);
                    //grupSetting.Add(itemKpa);
                    // Alternatif add item ke lis, jika menggunakan deklarasi class langsung, 
                    // constructor class yg digunakan harus menggandung
                    grupSetting.Add(new tampungDF(alat.pengecekField(reader, 0), alat.pengecekField(reader, 1), alat.pengecekField(reader, 2)));
                }
                reader.Close();
            }
            koneksi.Close();
            return grupSetting;
        }
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
            else if (status == 1)
            {
                IEnumerable<tampungDF> namaDF = (from s in tabelDF where s.kdDF == kd select s);
                // LAMDA EXPRESSION
                //#1.string namaKpa = tabelKpa.Single(p => p.idKpa == "1");
                //#2.Product product = db.Products.Single(p => p.ProductID == productID);
                foreach (var i in namaDF)
                {
                    listSayanya.Add(i.idDF);
                    listSayanya.Add(i.kdDF);
                    listSayanya.Add(i.namaDF);
                }
            }
            return listSayanya;
        }

        # endregion
        private void toggleDisable(bool status)
        {
            if (status == true)
            {
                //txtNTPNPPh.Enabled = true;
                //txtNTPNPPn.Enabled = true;
                //txtNoPPh.Enabled = true;
                //txtPPh.Enabled = true;
                //txtPPn.Enabled = true;
                //dateTimePicker1.Enabled = true;
                //comboBox1.Enabled = true;
            }
            else if (status == false)
            {
                //txtNTPNPPn.Enabled = false;
                //txtNTPNPPh.Enabled = false;
                //txtNoPPh.Enabled = false;
                //txtPPh.Enabled = false;
                //txtPPn.Enabled = false;
                //dateTimePicker1.Enabled = false;
                //comboBox1.Enabled = false;
                comboBox1.Text = "";
                txtNoKwitansi.Text = "";
                txtPPn.Text = "0";
                txtPPh.Text = "0";
                txtNTPNPPh.Text = "-";
                txtNTPNPPn.Text = "-";
                txtNoPPh.Text = "-";
            }
        }
        private bool cek_saldo()
        {
            bool kecukupan = true;
            decimal totalBayar = Convert.ToDecimal(filtertxt(txtJmlBayar.Text.Trim()));// + 
                //Convert.ToDecimal(filtertxt(txtPPh.Text.Trim())) + Convert.ToDecimal(filtertxt(txtPPn.Text.Trim()));
            decimal sisaAnggaran = 0;
            koneksi.Open();
            reader = konek.membacaData("SELECT sisa, namaSumber, NamaKpa " +
                "FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c " +
                "WHERE (b.Id_Kpa = a.Id_Kpa AND c.idSumber = a.idSumber) AND a.Id_Kpa = " + idKPA + " AND a.idSumber = " + idSumber + " " +
                " ORDER BY TglTerima DESC", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                sisaAnggaran = Convert.ToDecimal(alat.pengecekField(reader, 0));
                reader.Close();
            }
            else
            {
                kecukupan = false;
            }
            koneksi.Close();
            if ((sisaAnggaran - totalBayar) < 1)
                kecukupan = false;
            else if ((sisaAnggaran - totalBayar) > 0)
                kecukupan = true;
            return kecukupan;
        }
        #endregion
        #region backGroundWorker
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //if (statusShowTable == 0)
            //    showTable("");
            //else
            //    showTable(txtNoSPK.Text.Trim());
            #region bullshit1
            backgroundWorker1.ReportProgress(10);
            Thread.Sleep(100);
            backgroundWorker1.ReportProgress(20);
            Thread.Sleep(100);
            backgroundWorker1.ReportProgress(30);
            Thread.Sleep(100);
            backgroundWorker1.ReportProgress(40);
            Thread.Sleep(100);
            #endregion
            //lvTampil.Items.Clear();
            if (statusShowTable == 0) // Tanpa filter
            {
                query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                    "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                    " " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D " +
                    " " +
                    "WHERE B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier " +
                    " " +
                    "ORDER BY A.Tgl_SP";
                //MessageBox.Show("Masih loading data besar, harap sabar....", "KONFIRMASI");
            }
            else if (statusShowTable == 1) // event txtNoSPK keyPressed
                query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                    "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                    " " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D " +
                    " " +
                    "WHERE A.No_SPK LIKE '%" + txtNoSPK.Text.Trim() + "%' AND B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier " +
                    " " +
                    "ORDER BY A.Tgl_SP";
            else if (statusShowTable == 2) // Filter NTPN
                query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                    "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                    " " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D, REALANGGAR..A_PENGELUARAN E " +
                    " " +
                    "WHERE ((E.NTPNPPh is null or E.NTPNPPn is null) or (E.NTPNPPh = '' or E.NTPNPPn = '') or (E.NTPNPPh = '-' or E.NTPNPPn = '-')) " +
                    "AND B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier AND A.No_SPK = E.NO_SPK " +
                    " " +
                    "ORDER BY A.Tgl_SP";
            else if (statusShowTable == 3) // Filter Lunas
                query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                    "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                    " " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D " +
                    " " +
                    "WHERE A.Lunas = 'Y' AND B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier " +
                    " " +
                    "ORDER BY A.Tgl_SP";
            else if (statusShowTable == 4) // Filter Belum Lunas
                query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                    "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                    " " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D " +
                    " " +
                    "WHERE A.Lunas <> 'Y' AND B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier " +
                    " " +
                    "ORDER BY A.Tgl_SP";
            else if (statusShowTable == 5) // Filter PPn/h
                query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                    "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                    " " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D, REALANGGAR..A_PENGELUARAN E " +
                    " " +
                    "WHERE ((E.PPhRp is null or E.PPnRp is null) or (E.PPhNo = 0 or E.PPnRp = 0) or (E.PPhNo = '0' or E.PPnRp = '0')) " +
                    "AND B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier " +
                    " " +
                    "ORDER BY A.Tgl_SP";
            else if (statusShowTable == 6) // Filter no PP
                query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                    "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                    ", E.NTPNPPn, E.NTPNPPh, E.PPhNo, G.Id_Kpa, G.KdKpa " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D " +
                    " " +
                    "WHERE (E.PPhNo is null or E.PPhNo = '' or E.PPhNo = '-') " +
                    "AND B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier " +
                    " " +
                    "ORDER BY A.Tgl_SP";
            else if (statusShowTable == 7) // Filter ammount of PPn
                query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                    "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                    ", E.NTPNPPn, E.NTPNPPh, E.PPhNo, G.Id_Kpa, G.KdKpa " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D, REALANGGAR..A_PENGELUARAN E " +
                    ", REALANGGAR..A_PENERIMAAN F, REALANGGAR..A_KPA G " +
                    "WHERE (PPnRp = " + filtertxt(txtPPn.Text.Trim()) + ") " +
                    "AND B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier AND A.No_SPK = E.NO_SPK " +
                    "AND (G.id_Kpa = F.id_Kpa AND F.ket = E.NoBayar) " +
                    "ORDER BY A.Tgl_SP";
            else if (statusShowTable == 8) // Filter ammount of PPh
                query = "SELECT DISTINCT A.No_SPK, A.Tgl_SP, A.Keter, B.Id_Rinci_Rs, CONVERT(varchar(1000),C.Uraian) AS 'uraian', D.NamaSupp, B.TFungsi, " +
                    "B.TSubsi AS 'ANEH', A.Lunas, A.TGL_SPJ, A.No_Bukti, C.Id_Reken, D.id_Supplier " +
                    ", E.NTPNPPn, E.NTPNPPh, E.PPhNo, G.Id_Kpa, G.KdKpa " +
                    "FROM KASDA..BLJ_MASTER A, KASDA..BLJ_DETAIL B, REALANGGAR..A_REKENING C, REALANGGAR..A_SUPPLIER D, REALANGGAR..A_PENGELUARAN E " +
                    ", REALANGGAR..A_PENERIMAAN F, REALANGGAR..A_KPA G " +
                    "WHERE (PPhRp = " + filtertxt(txtPPh.Text.Trim()) + ") " +
                    "AND B.IdBlj_Master = A.IdBlj_Master AND C.Kd_Reken = B.Id_Rinci_Rs AND D.KdSupp = A.IdSupplier AND A.No_SPK = E.NO_SPK " +
                    "AND (G.id_Kpa = F.id_Kpa AND F.ket = E.NoBayar) " +
                    "ORDER BY A.Tgl_SP";


            koneksi.Open();
            reader = konek.membacaData(query, koneksi);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.pengecekField(reader, 1));
                    lvItem.Font = new Font("Arial", 11, FontStyle.Bold | FontStyle.Italic);
                    lvItem.SubItems.Add(alat.pengecekField(reader, 0)); // tglSPK
                    lvItem.SubItems.Add(alat.pengecekField(reader, 2)); // noSPK
                    lvItem.SubItems.Add(alat.pengecekField(reader, 3)); // Ket Spk
                    lvItem.SubItems.Add(alat.pengecekField(reader, 4));
                    lvItem.SubItems.Add(alat.pengecekField(reader, 5));
                    // buat ngecek status:
                    if (Convert.ToDecimal(alat.pengecekField(reader, 6)) == 0 && Convert.ToDecimal(alat.pengecekField(reader, 7)) > 0)
                        lvItem.SubItems.Add("SUBSIDI"); // STATUS
                    else
                        lvItem.SubItems.Add("FUNGSIONAL"); // STATUS
                    lvItem.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", Convert.ToDecimal(alat.pengecekField(reader, 6)))); //jumlah FUNG
                    lvItem.SubItems.Add(string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", Convert.ToDecimal(alat.pengecekField(reader, 7)))); //jumlah SUBS
                    lvItem.SubItems.Add(alat.pengecekField(reader, 8));
                    lvItem.SubItems.Add(alat.pengecekField(reader, 9)); // tgl_SPJ
                    lvItem.SubItems.Add(alat.pengecekField(reader, 10));
                    lvItem.SubItems.Add(alat.pengecekField(reader, 11));
                    lvItem.SubItems.Add(alat.pengecekField(reader, 12));
                    //lvItem.SubItems.Add(alat.pengecekField(reader, 13));
                    //lvItem.SubItems.Add(alat.pengecekField(reader, 14));
                    //lvItem.SubItems.Add(alat.pengecekField(reader, 15));
                    //lvItem.SubItems.Add(alat.pengecekField(reader, 16));
                    //lvItem.SubItems.Add(alat.pengecekField(reader, 17));

                    //lvTampil.Items.Add(lvItem);
                    //listViewAddItem(lvTampil, lvItem);
                    //lvTampil.SafeControlInvoke(m => m.Items.Add(lvItem));
                    //backgroundWorker1.ReportProgress(i++/lvTampil.Items.Count);
                    lvItemGroup.Add(lvItem);
                    backgroundWorker1.ReportProgress(50);
                }
                reader.Close();
            }
            koneksi.Close();
            //for (int i = 0; i < lvItemGroup.Count; i++)
            //{
            //    backgroundWorker1.ReportProgress(i);
            //}
            #region bullshit2
            backgroundWorker1.ReportProgress(60);
            Thread.Sleep(100);
            backgroundWorker1.ReportProgress(70);
            Thread.Sleep(100);
            backgroundWorker1.ReportProgress(80);
            Thread.Sleep(100);
            #endregion
            lvTampil.SafeControlInvoke(
                listView =>
                {
                    lvTampil.BeginUpdate();
                    lvTampil.Items.Clear();
                    lvTampil.Items.AddRange(lvItemGroup.ToArray());
                    lvTampil.EndUpdate();
                }
                );
            backgroundWorker1.ReportProgress(100);
            lvItemGroup.Clear();
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Increment(+1);
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel1.Text = "MASIH LOADING.......................";
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            toolStripProgressBar1.Visible = false;
        }
        #endregion
        private void txtNoRek_TextChanged(object sender, EventArgs e)
        {
            if (koneksi.State == ConnectionState.Closed)
                koneksi.Open();
            reader = konek.membacaData("SELECT kode_kelompok, kode_jenis, kode_objek, " +
                "kode_rincian, idKtg_blj, formatPanjang FROM A_REKENING WHERE Kd_Reken = '" + txtNoRek.Text.Trim() + "'", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                txtDetailRek.Text = alat.pengecekField(reader, 0) + "." + alat.pengecekField(reader, 1) + "." + alat.pengecekField(reader, 2) +
                    "." + alat.pengecekField(reader, 3) + "." + alat.pengecekField(reader, 4);
                if (string.IsNullOrEmpty(alat.pengecekField(reader, 5)))
                {
                    txtRekDowo.Enabled = true;
                    txtRekDowo.Text = "";
                    txtRekDowoEmpyorNot = 1;
                }
                else
                {
                    txtRekDowo.Text = alat.pengecekField(reader, 5);
                    txtRekDowo.Enabled = false;
                }
                reader.Close();
            }
            if (koneksi.State == ConnectionState.Open)
                koneksi.Close();
        }
        private void comboBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtNTPNPPn_Leave(object sender, EventArgs e)
        {
            if (txtNTPNPPn.Text == "")
                txtNTPNPPn.Text = "-";
        }

        private void txtNTPNPPh_Leave(object sender, EventArgs e)
        {
            if (txtNTPNPPh.Text == "")
                txtNTPNPPh.Text = "-";
        }

        private void txtNoPPh_Leave(object sender, EventArgs e)
        {
            if (txtNoPPh.Text == "")
                txtNoPPh.Text = "-";
        }

        private void txtPPh_Leave(object sender, EventArgs e)
        {
            if (txtPPh.Text == "")
                txtPPh.Text = "0";
            else
            {
                try
                {
                    txtPPh.Text = string.Format(new System.Globalization.CultureInfo("id-ID"), "{0:n}", Convert.ToDecimal(filtertxt(txtPPh.Text)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void comboDF_TextChanged(object sender, EventArgs e)
        {
            if (comboDF.Text.Trim() != "")
            {
                ArrayList listku = searchinListUsingLinq(comboDF.Text, 1);
                idSumber = Convert.ToInt16(listku[0].ToString());
                txtSumberDana.Text = listku[2].ToString();
            }
        }

        private void comboDF_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtSumberDana_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F8)
            {
                comboDF.Enabled = true;
                comboDF.Focus();
            }
        }

        #region this for filtering listview based on user choice
        private void lunasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusShowTable = 3;
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        private void belumLunasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusShowTable = 4;
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        private void nTPNBelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusShowTable = 2;
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        private void pPnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusShowTable = 5;
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        private void pPhToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void noPPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusShowTable = 6;
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }
        #endregion

        private void txtJmlBayar_TextChanged(object sender, EventArgs e)
        {
            //string _1 = 
        }

        private void txtNoKwitansi_TextChanged(object sender, EventArgs e)
        {
            //string temp = txtNoKwitansi.Text;

            //MessageBox.Show(alat.Terbilang(Convert.ToDouble(txtNoKwitansi.Text)));
        }

        private void lStatus_TextChanged(object sender, EventArgs e)
        {
            if (lStatus.Text == "SUDAH BAYAR")
                toggleDisable(true);
            else
                toggleDisable(false);
        }

        private void txtPPn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                statusShowTable = 7;
                if (backgroundWorker1.IsBusy == false)
                    backgroundWorker1.RunWorkerAsync();
            }
        }

        private void txtPPh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                statusShowTable = 8;
                if (backgroundWorker1.IsBusy == false)
                    backgroundWorker1.RunWorkerAsync();
            }
        }
    }
    static class Extensions
    {
        /// <summary>
        /// Get substring of specified number of characters on the right.
        /// </summary>
        public static string Right1(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }
    }
}
