using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fLogin : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlDataReader pembaca = null;
        SqlConnection koneksi;
        string query = null;
        fUtama FormUtama;
        string kd_Opp = null;

        /* Changes on this form:
         * 
         * 1. This form is unaffected by the alat.deFormater a.k.a font setting 
         * 
         * */
        public fLogin()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void bBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fLogin_Load(object sender, EventArgs e)
        {
            //alat.deSerial(this);
            bersih2();
        }

        private void bLogin_Click(object sender, EventArgs e)
        {
            verifUser();
        }

        private void verifUser()
        {
            query = "SELECT passwd FROM A_OPP WHERE Kd_Opp = '" + txtKode.Text.Trim() + "' AND passwd = '" +
                txtPass.Text.Trim() + "'";
            try
            {
                koneksi.Open();
                pembaca = konek.MembacaData(query, koneksi);
                if (pembaca.HasRows)
                {
                    if ((FormUtama = (fUtama)alat.FormSudahDibuat(typeof(fUtama))) == null)
                    {
                        MessageBox.Show("BUT M'LORD,...IT CANNOT BE END LIKE THIS :)", "THIS IS BULLSHIT");
                    }
                    else
                    {
                        FormUtama.txtkd_Pos.Text = kd_Opp;
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("PASSWORD YANG DIMASUKAN SALAH, SILAHKAN MASUKAN YANG BENAR!", "PERHATIAN");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                koneksi.Close();
            }
        }

        private void lKeluar_Click(object sender, EventArgs e)
        {
            bBatal_Click(sender, e);
        }

        private void lKeluar_MouseMove(object sender, MouseEventArgs e)
        {
            lKeluar.ForeColor = Color.Red;
        }

        private void lKeluar_MouseLeave(object sender, EventArgs e)
        {
            lKeluar.ForeColor = Color.Black;
        }

        private void txtKode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                tampilUser();
            }
        }

        private void tampilUser()
        {
            query = "SELECT Kd_Opp, Nama_Opp, Nip_Opp, A.id_Opp, Nama_Role FROM A_OPP A, A_ROLE B WHERE" +
                " B.id_Role = A.id_Role AND Kd_Opp = '" + txtKode.Text.Trim() + "'";
            if (konek.CekFieldUnik("A_OPP", "Kd_Opp", txtKode.Text.Trim()) == false)
            {
                MessageBox.Show("OPERATOR DENGAN KODE TERSEBUT TIDAK DITEMUKAN", "PERHATIAN");
                txtKode.Text = "";
                txtKode.Focus();
            }
            else
            {
                koneksi.Open();
                pembaca = konek.MembacaData(query, koneksi);
                if (pembaca.HasRows)
                {
                    pembaca.Read();
                    txtKode.Text = alat.PengecekField(pembaca, 0);
                    lNama.Text = alat.PengecekField(pembaca, 1);
                    lNIP.Text = alat.PengecekField(pembaca, 2);
                    kd_Opp = alat.PengecekField(pembaca, 3);
                    lPosisi.Text = alat.PengecekField(pembaca, 4);
                    pembaca.Close();
                }
                koneksi.Close();
                txtPass.Focus();
            }
        }

        private void bersih2()
        {
            alat.CleaningService(groupBox1);
            lNama.Text = "<nama_operator>";
            lNIP.Text = "<no_induk_pegawai>";
            lPosisi.Text = "<posisi_operator>";
        }

        private void setPriv()
        {
        }

        private void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                verifUser();
            }
        }

        private void txtPass_Enter(object sender, EventArgs e)
        {
            tampilUser();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
