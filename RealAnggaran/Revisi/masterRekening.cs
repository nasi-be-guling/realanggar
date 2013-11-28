using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;

namespace RealAnggaran.Revisi
{
    public partial class masterRekening : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader reader = null;


        public masterRekening()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private string getidANGKAS()
        {
            string jumlah = "";
            koneksi.Open();
            reader = konek.MembacaData("SELECT IdAngkas FROM KASDA..ANGKAS_MST WHERE Thn_Ang = '" + DateTime.Now.Year + "'", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                jumlah = alat.PengecekField(reader, 0);
                reader.Close();
            }
            else
            {
                MessageBox.Show("Admin belum menambah Master Anggaran.\nSilahkan hubungi 1062", "PERHATIAN");
                this.Close();
            }
            koneksi.Close();
            return jumlah;
        }

        private decimal hitungidRek()
        {
            decimal jumlah = 0;
            koneksi.Open();
            reader = konek.MembacaData("SELECT COUNT (*) FROM KASDA..AKD_RINCIAN", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                jumlah = Convert.ToDecimal(alat.PengecekField(reader, 0));
                reader.Close();
            }
            koneksi.Close();
            return jumlah + 1;
        }

        private void clearance()
        {
            txtIdRinciRS.Text = ""; txtkdJenis.Text = ""; txtKdKelompok.Text = ""; txtKdObjek.Text = "";
            txtkdRincian.Text = ""; txtKegiatan.Text = ""; txtKtgBlj.Text = ""; txtNoRek.Text = "";
            txtProgram.Text = ""; txtUraian.Text = ""; txtFungsi.Text = ""; txtSubsidi.Text = "";
        }

        private decimal hitungAngkas()
        {
            decimal jumlah = 0;
            koneksi.Open();
            reader = konek.MembacaData("SELECT COUNT (*) FROM KASDA..ANGKAS_DTL", koneksi);
            if (reader.HasRows)
            {
                reader.Read();
                jumlah = Convert.ToDecimal(alat.PengecekField(reader, 0));
                reader.Close();
            }
            koneksi.Close();
            return jumlah + 1;
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(getidANGKAS());
            decimal IDRek = hitungidRek();
            decimal IdAngkas = hitungAngkas();
            decimal jumlahFungSub = Convert.ToDecimal(txtFungsi.Text.Trim()) + Convert.ToDecimal(txtSubsidi.Text.Trim());
            string getIdAngkasVar = getidANGKAS();

            if (String.IsNullOrEmpty(txtIdRinciRS.Text) | String.IsNullOrEmpty(txtkdJenis.Text) |
                String.IsNullOrEmpty(txtKdKelompok.Text) | String.IsNullOrEmpty(txtKdObjek.Text) |
                String.IsNullOrEmpty(txtkdRincian.Text) | String.IsNullOrEmpty(txtKegiatan.Text) |
                String.IsNullOrEmpty(txtKtgBlj.Text) | String.IsNullOrEmpty(txtNoRek.Text) |
                String.IsNullOrEmpty(txtProgram.Text) | String.IsNullOrEmpty(txtUraian.Text.Trim()) |
                String.IsNullOrEmpty(txtFungsi.Text) | String.IsNullOrEmpty(txtSubsidi.Text) | (Convert.ToDecimal(txtSubsidi.Text) + Convert.ToDecimal(txtFungsi.Text)) == 0)
            {
                //MessageBox.Show((Convert.ToDecimal(txtSubsidi.Text) + Convert.ToDecimal(txtFungsi.Text)).ToString());
                MessageBox.Show("Data yang anda masukan tidak lengkap", "PERHATIAN");
                txtIdRinciRS.Focus();
            }
            else
            {
                koneksi.Open();
                SqlTransaction tran = koneksi.BeginTransaction();
                if (statusUpdate == 0)
                {
                    try
                    {
                        konek.MasukkanData("INSERT INTO REALANGGAR..A_REKENING VALUES " +
                            "('" + txtIdRinciRS.Text + "', '" + txtProgram.Text + "', '" + txtKegiatan.Text + "', '" + txtUraian.Text + "', '" + txtKdKelompok.Text + "', " +
                            "'" + txtkdJenis.Text + "', '" + txtKdObjek.Text + "', '" + txtkdRincian.Text + "', '" + txtKtgBlj.Text + "', '" + txtNoRek.Text + "')", koneksi, tran);
                        konek.MasukkanData("INSERT INTO KASDA..ANGKAS_DTL (IdAngkas_Dtl, IdAngkas, Id_Rinci_Rs, P, K, TSubsi, TFungsi, " +
                            "Tot_Angkas, Tot_Sblm_Pak, BANTU, kode_Kelompok, kode_Jenis, kode_Obyek, kode_Rincian, IdKtg_Blj, Thn_Ang)" +
                            "VALUES ('" + IdAngkas + "', '" + getIdAngkasVar + "', '" + txtIdRinciRS.Text + "', '" + txtProgram.Text + "', '" + txtKegiatan.Text + "', " + txtSubsidi.Text + ", " + txtFungsi.Text + ", " +
                            "" + jumlahFungSub + ", '" + jumlahFungSub + "', '" + txtUraian.Text + "', '" + txtKdKelompok.Text + "', '" + txtkdJenis.Text + "', '" + txtKdObjek.Text + "', '" + txtkdRincian.Text + "', '" + txtKtgBlj.Text + "', '" + DateTime.Now.Year + "')", koneksi, tran);
                        konek.MasukkanData("INSERT INTO KASDA..AKD_RINCIAN VALUES ('" + txtIdRinciRS.Text + "', '" + txtKdKelompok.Text + "', '" + txtkdJenis.Text + "', '" + txtKdObjek.Text + "', " +
                            "'" + txtkdRincian.Text + "', '" + txtKtgBlj.Text + "', '" + txtUraian.Text + "', '" + txtProgram.Text + "', '" + txtKegiatan.Text + "', '" + txtNoRek.Text + "')", koneksi, tran);
                        MessageBox.Show("TERSIMPAN", "PERHATIAN");
                        txtUraian.Text = "";
                        txtSubsidi.Text = "0";
                        txtFungsi.Text = "0";
                        //txtIdRinciRS.Text = (Convert.ToDecimal(txtIdRinciRS.Text) + 1).ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Terjadi kesalahan : \n" + ex.Message + "\nHubungi teknisi anda.", "PERHATIAN");
                        tran.Rollback();
                    }
                    tran.Commit();
                }
                else if (statusUpdate == 1)
                {
                    try
                    {
                        string query1 = "UPDATE REALANGGAR..A_REKENING SET " +
                            "program = '" + txtProgram.Text + "', kegiatan = '" + txtKegiatan.Text + "', uraian = '" + txtUraian.Text + "', kode_kelompok = '" + txtKdKelompok.Text + "', kode_jenis = " +
                            "'" + txtkdJenis.Text + "', kode_objek = '" + txtKdObjek.Text + "', kode_rincian = '" + txtkdRincian.Text + "', idKtg_blj = '" + txtKtgBlj.Text + "', formatPanjang = '" + txtNoRek.Text + "' WHERE kd_reken = '" + txtIdRinciRS.Text + "'";
                        string query2 = "UPDATE KASDA..ANGKAS_DTL SET IdAngkas_Dtl = '" + IdAngkas + "', IdAngkas = '" + getIdAngkasVar + "', P = '" + txtProgram.Text + "', K = '" + txtKegiatan.Text + "', TSubsi = " + txtSubsidi.Text + ", TFungsi = " + txtFungsi.Text + ", " +
                            "Tot_Angkas = " + jumlahFungSub + ", Tot_Sblm_Pak = " + jumlahFungSub + ", BANTU = '" + txtUraian.Text + "', kode_Kelompok = '" + txtKdKelompok.Text + "', kode_Jenis = '" + txtkdJenis.Text + "', kode_Obyek = '" + txtKdObjek.Text + "', kode_Rincian = '" + txtkdRincian.Text + "', IdKtg_Blj = '" + txtKtgBlj.Text + "', Thn_Ang = '" + DateTime.Now.Year + "'" +
                            "WHERE Id_Rinci_Rs = '" + txtIdRinciRS.Text + "'";
                        string query3 = "UPDATE KASDA..AKD_RINCIAN SET kode_kelompok = '" + txtKdKelompok.Text + "', kode_jenis = '" + txtkdJenis.Text + "', kode_obyek = '" + txtKdObjek.Text + "', kode_rincian = " +
                            "'" + txtkdRincian.Text + "', idktg_blj = '" + txtKtgBlj.Text + "', uraian = '" + txtUraian.Text + "', p = '" + txtProgram.Text + "', k = '" + txtKegiatan.Text + "', formatPanjang = '" + txtNoRek.Text + "' WHERE Id_rinci_rs = '" + txtIdRinciRS.Text + "'";

                        konek.MasukkanData(query1, koneksi, tran);
                        konek.MasukkanData(query2, koneksi, tran);
                        konek.MasukkanData(query3, koneksi, tran);
                        MessageBox.Show("TERSIMPAN", "PERHATIAN");
                        txtUraian.Text = "";
                        txtSubsidi.Text = "0";
                        txtFungsi.Text = "0";
                        txtIdRinciRS.Text = "";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Terjadi kesalahan : \n" + ex.Message + "\nHubungi teknisi anda.", "PERHATIAN");
                        tran.Rollback();
                    }
                    tran.Commit();
                }
                koneksi.Close();
                statusUpdate = 0;
            }
        }

        private void masterRekening_Load(object sender, EventArgs e)
        {

        }

        int statusUpdate = 0;

        private void txtIdRinciRS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (string.IsNullOrEmpty(txtIdRinciRS.Text.Trim()))
                {
                    MessageBox.Show("Silahkan Masukkan Kode Panggil yg akan dirubah", "PERHATIAN");
                    txtIdRinciRS.Focus();
                }
                else
                {
                    statusUpdate = 1;
                    koneksi.Open();
                    reader = konek.MembacaData("SELECT     ANGKAS_DTL.Id_Rinci_Rs, ANGKAS_DTL.Kode_Kelompok, ANGKAS_DTL.Kode_Jenis, ANGKAS_DTL.Kode_Obyek, ANGKAS_DTL.Kode_Rincian, " +
                      "ANGKAS_DTL.IdKtg_Blj, ANGKAS_DTL.P, ANGKAS_DTL.K, AKD_RINCIAN.formatPanjang, ANGKAS_DTL.BANTU, ANGKAS_DTL.TSubsi, ANGKAS_DTL.TFungsi " +
                      "FROM KASDA..ANGKAS_DTL ANGKAS_DTL INNER JOIN " +
                      "KASDA..AKD_RINCIAN AKD_RINCIAN ON ANGKAS_DTL.Id_Rinci_Rs = AKD_RINCIAN.Id_Rinci_RS " +
                      "WHERE ANGKAS_DTL.Id_Rinci_Rs = '" + txtIdRinciRS.Text.Trim() + "'", koneksi);
                    if (reader.HasRows)
                    {
                        reader.Read();
                        //txtIdRinciRS.Text = alat.pengecekField(reader, 0);
                        txtKdKelompok.Text = alat.PengecekField(reader, 1);
                        txtkdJenis.Text = alat.PengecekField(reader, 2);
                        txtKdObjek.Text = alat.PengecekField(reader, 3);
                        txtkdRincian.Text = alat.PengecekField(reader, 4);
                        txtKtgBlj.Text = alat.PengecekField(reader, 5);
                        txtProgram.Text = alat.PengecekField(reader, 6);
                        txtKegiatan.Text = alat.PengecekField(reader, 7);
                        if (string.IsNullOrEmpty(alat.PengecekField(reader, 8)))
                            txtNoRek.Text = "*No Rekening format anggaran Belum Diisi*";
                        else
                            txtNoRek.Text = alat.PengecekField(reader, 8);
                        txtUraian.Text = alat.PengecekField(reader, 9);
                        txtSubsidi.Text = alat.PengecekField(reader, 10);
                        txtFungsi.Text = alat.PengecekField(reader, 11);
                        reader.Close();
                    }
                    else
                    {
                        MessageBox.Show("Kode Panggil yang dimasukkan tidak ditemukan", "PERHATIAN");
                        txtIdRinciRS.Focus();
                    }
                    koneksi.Close();
                }
            }
        }

        private void txtSubsidi_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubsidi.Text.Trim()))
                txtSubsidi.Text = "0";
        }

        private void txtFungsi_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFungsi.Text.Trim()))
                txtFungsi.Text = "0";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clearance();
        }

        private void txtNoRek_Validated(object sender, EventArgs e)
        {
            try
            {
                txtProgram.Text = Convert.ToInt16(txtNoRek.Text.Trim().Substring(0, 2)).ToString();
                txtKegiatan.Text = Convert.ToInt16(txtNoRek.Text.Trim().Substring(3, 3)).ToString();
                txtKdKelompok.Text = Convert.ToInt16(txtNoRek.Text.Trim().Substring(7, 1)).ToString();
                txtkdJenis.Text = Convert.ToInt16(txtNoRek.Text.Trim().Substring(8, 1)).ToString();
                txtKdObjek.Text = Convert.ToInt16(txtNoRek.Text.Trim().Substring(9, 1)).ToString();
                txtkdRincian.Text = Convert.ToInt16(txtNoRek.Text.Trim().Substring(11, 2)).ToString();
                txtKtgBlj.Text = Convert.ToInt16(txtNoRek.Text.Trim().Substring(14, 3)).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan : \n================\n" + ex.Message +
                    "\n================\nHarap Cek Kembali Format Penulisan");
            }
        }

        private void masterRekening_KeyDown(object sender, KeyEventArgs e)
        {
            //if (keyData == (Keys.Control | Keys.S))
            //{
            //    MessageBox.Show("Test");
            //    foreach (Control kontrol in groupBox1.Controls)
            //    {
            //        //if (kontrol.Focused == true)
            //        //{
            //        MessageBox.Show(kontrol.Name);
            //        //}
            //    }
            //}
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                foreach (Control kontrol in groupBox1.Controls)
                {
                    if (kontrol.Focused == true)
                    {
                        kontrol.Text = Clipboard.GetText();
                    }
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtSubsidi_MouseClick(object sender, MouseEventArgs e)
        {
            txtFungsi.BackColor = Color.Gray;
            txtSubsidi.BackColor = Color.White;
            txtSubsidi.SelectAll();
        }

        private void txtFungsi_MouseClick(object sender, MouseEventArgs e)
        {
            txtSubsidi.BackColor = Color.Gray;
            txtFungsi.BackColor = Color.White;
            txtFungsi.SelectAll();
        }

        private void txtNoRek_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtNoRek_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Back)
            {
                if (txtNoRek.TextLength == 2)
                {
                    txtNoRek.Text = txtNoRek.Text + ".";
                    txtNoRek.SelectionStart = txtNoRek.TextLength;
                }
                if (txtNoRek.TextLength == 6)
                {
                    txtNoRek.Text = txtNoRek.Text + ".";
                    txtNoRek.SelectionStart = txtNoRek.TextLength;
                }
                if (txtNoRek.TextLength == 10)
                {
                    txtNoRek.Text = txtNoRek.Text + ".";
                    txtNoRek.SelectionStart = txtNoRek.TextLength;
                }
                if (txtNoRek.TextLength == 13)
                {
                    txtNoRek.Text = txtNoRek.Text + ".";
                    txtNoRek.SelectionStart = txtNoRek.TextLength;
                }
            }
        }
    }
}
