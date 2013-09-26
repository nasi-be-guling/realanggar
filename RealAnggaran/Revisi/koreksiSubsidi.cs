using System;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Data.SqlClient;


namespace RealAnggaran.Revisi
{
    public partial class koreksiSubsidi : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader reader = null;
        string query = null;
        public int idKasda = 0;
        string idRek = null;
        string idSupp = null;
        public int idOpp = 0;

        public koreksiSubsidi()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            koneksi = konek.KonekDb();
            InitializeComponent();
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
        }

        private void label9_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void koreksiSubsidi_Load(object sender, EventArgs e)
        {
            idRek = selectIdRek();
            idSupp = selectIdSupplier();
        }

        private void txtNoSPK_TextChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(idKasda.ToString());
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            string fConverted = txtFung.Text.Replace("Rp. ", "");
            string sConverted = txtSubs.Text.Replace("Rp. ", "");

            fConverted = fConverted.Replace(".", "");
            fConverted = fConverted.Replace(",", ".");

            sConverted = sConverted.Replace(".", "");
            sConverted = sConverted.Replace(",", ".");
 
            if (idKasda == 0)
            {
                MessageBox.Show("Id Kasda bernilai nol", "PERHATIAN");
                this.Close();
            }
            else if (string.IsNullOrEmpty(idRek))
            {
                MessageBox.Show("Id Rekening bernilai null", "PERHATIAN");
                txtNoRek.Focus();
            }
            else if (string.IsNullOrEmpty(idSupp))
            {
                MessageBox.Show("Id Supplier bernilai null", "PERHATIAN");
                txtSupplier.Focus();
            }
            else if (cekRekon() == true)
            {
                // kondisi jika data yg di koreksi berhubungan dengan perbendarahaan
                if (MessageBox.Show("Data berhubungan dengan pelunasan Penbendaharaan\nSilahkan Konfirmasi bag. Perbendaharaan.\nTetap Melanjutkan?",
                    "KONFIRMASI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (!string.IsNullOrEmpty(txtFung.Text) | txtFung.Text.Trim() != "0")
                    {
                        try
                        {
                            koneksi.Open();
                            SqlTransaction tran = koneksi.BeginTransaction();
                            try
                            {
                                //MessageBox.Show("UPDATE KASDA..BLJ_MASTER SET no_SPK = '" + txtNoSPK.Text + "', Tgl_SP = CONVERT(SMALLDATETIME, '" + dateTimePicker1.Text +
                                //    "', 103), No_Bukti = '" + txtNoBukti.Text + "' WHERE IdBlj_Master = " + idKasda + "");
                                konek.MasukkanData("UPDATE KASDA..BLJ_MASTER SET Tgl_SP = CONVERT(SMALLDATETIME, '" + dateTimePicker1.Text +
                                    "', 103), No_Bukti = '" + txtNoBukti.Text + "', Keter ='" + txtKeterangan.Text + "', IdSupplier = '" + txtSupplier.Text + "' WHERE IdBlj_Master = " + idKasda + "", koneksi, tran);
                                konek.MasukkanData("UPDATE KASDA..BLJ_DETAIL SET Id_Rinci_RS = '" + txtNoRek.Text + "', TSubsi = " + sConverted +
                                    ", TFungsi = " + fConverted + " WHERE IdBlj_Master = " + idKasda + "", koneksi, tran);
                                konek.MasukkanData("kasda.dbo.sp_update_saldo_dr_akuntansi" + " '" + txtNoBukti.Text + "', " + fConverted + ", " + idOpp + "", koneksi, tran);
                                konek.MasukkanData("UPDATE REALANGGAR..A_PENGELUARAN SET Id_Reken = " + idRek + ", Id_Supplier = " + idSupp + ", JmlRp = " + fConverted + ", Id_Opp_Ver = " + idOpp + ", Wkt_Update = GETDATE() " +
                                    "WHERE NoBayar = '" + txtNoBukti.Text + "'", koneksi, tran);
                                tran.Commit();
                                MessageBox.Show("Data Tersimpan.", "PERHATIAN");
                            }
                            catch (SqlException exsql)
                            {
                                MessageBox.Show("Terjadi Kesalahan: " + exsql.Message, "KESALAHAN PENYIMPANAN KOREKSI SUBSIDI");
                                tran.Rollback();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Terjadi Kesalahan: " + ex.Message, "KESALAHAN KONEKSI KOREKSI SUBSIDI");
                        }
                        finally
                        {
                            koneksi.Close();
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nilai Fungsional Tidak Boleh Nol atau Kosong", "PERHATIAN");
                        txtFung.Focus();
                    }
                }
                else
                    this.Close();
            }
            else
            {
                try
                {
                    koneksi.Open();
                    SqlTransaction tran = koneksi.BeginTransaction();
                    try
                    {
                        //MessageBox.Show("UPDATE KASDA..BLJ_MASTER SET no_SPK = '" + txtNoSPK.Text + "', Tgl_SP = CONVERT(SMALLDATETIME, '" + dateTimePicker1.Text +
                        //    "', 103), No_Bukti = '" + txtNoBukti.Text + "' WHERE IdBlj_Master = " + idKasda + "");
                        konek.MasukkanData("UPDATE KASDA..BLJ_MASTER SET no_SPK = '" + txtNoSPK.Text + "', Tgl_SP = CONVERT(SMALLDATETIME, '" + dateTimePicker1.Text +
                            "', 103), No_Bukti = '" + txtNoBukti.Text + "', Keter ='" + txtKeterangan.Text + "', IdSupplier = '" + txtSupplier.Text + "' WHERE IdBlj_Master = " + idKasda + "", koneksi, tran);
                        konek.MasukkanData("UPDATE KASDA..BLJ_DETAIL SET Id_Rinci_RS = '" + txtNoRek.Text + "', TSubsi = " + sConverted +
                            ", TFungsi = " + fConverted + " WHERE IdBlj_Master = " + idKasda + "", koneksi, tran);
                        tran.Commit();
                        MessageBox.Show("Data Tersimpan.", "PERHATIAN");
                    }
                    catch (SqlException exsql)
                    {
                        MessageBox.Show("Terjadi Kesalahan: " + exsql.Message, "KESALAHAN PENYIMPANAN KOREKSI SUBSIDI");
                        tran.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi Kesalahan: " + ex.Message, "KESALAHAN KONEKSI KOREKSI SUBSIDI");
                }
                finally
                {
                    koneksi.Close();
                    this.Close();
                }
            }
        }

        private string selectIdRek()
        {
            string nilai = null;
            koneksi.Open();
            reader = konek.MembacaData("SELECT id_Reken FROM A_REKENING WHERE Kd_Reken = '" + txtNoRek.Text.Trim() + "'", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                nilai = alat.PengecekField(reader, 0);
                reader.Close();
            }
            koneksi.Close();
            return nilai;
        }

        private string selectIdSupplier()
        {
            string nilai = null;
            koneksi.Open();
            reader = konek.MembacaData("SELECT id_Supplier FROM A_SUPPLIER WHERE kdSupp = '" + txtSupplier.Text + "'", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                nilai = alat.PengecekField(reader, 0);
                reader.Close();
            }
            koneksi.Close();
            return nilai;
        }

        private bool cekRekon()
        {
            bool status = false;
            query = "SELECT No_SPK FROM REALANGGAR..A_PENGELUARAN WHERE No_SPK = '" + txtNoSPK.Text + "'";
            koneksi.Open();
            reader = konek.MembacaData(query, koneksi);
            if (reader.HasRows)
            {
                status = true;
            }
            else
            {
                status = false;
            }
            koneksi.Close();
            return status;
        }

        private void txtNoRek_Leave(object sender, EventArgs e)
        {
            idRek = null;
            string nilai = selectIdRek();
            if (!string.IsNullOrEmpty(txtNoRek.Text))
            {
                if (string.IsNullOrEmpty(nilai))
                {
                    MessageBox.Show("DATA REKENING TIDAK DITEMUKAN.\nSILAHKAN BERHUBUNGAN DENGAN BAGIAN ITIKOM", "PERHATIAN");
                    txtNoRek.Focus();
                }
                else
                    idRek = nilai;
            }
        }

        private void txtSupplier_Leave(object sender, EventArgs e)
        {
            idSupp = null;
            string nilai = selectIdSupplier();
            if (!string.IsNullOrEmpty(txtSupplier.Text))
            {
                if (string.IsNullOrEmpty(nilai))
                {
                    MessageBox.Show("DATA SUPPLIER TIDAK DITEMUKAN.\nSILAHKAN BERHUBUNGAN DENGAN BAGIAN ITIKOM", "PERHATIAN");
                    txtSupplier.Focus();
                }
                else
                    idSupp = nilai;
            }
        }

        private void txtSubs_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubs.Text))
                txtSubs.Text = "0";
        }

        private void txtFung_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFung.Text))
                txtFung.Text = "0";
        }
    }
}
