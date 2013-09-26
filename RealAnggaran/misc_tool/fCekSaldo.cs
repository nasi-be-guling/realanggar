using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Data.SqlClient;

namespace RealAnggaran.misc_tool
{
    public partial class fCekSaldo : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader reader = null;
        string noSPK = null;
        string kdOpp = null;

        public fCekSaldo()
        {
            koneksi = konek.KonekDb();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        private void fCekSaldo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(textBox1.Text.Trim()) || !string.IsNullOrEmpty(textBox2.Text.Trim()))
                    {
                        koneksi.Open();
                        reader = konek.MembacaData("SElECT (B.Tot_Angkas - (SUM(A.TFungsi) + SUM(A.TSubsi))) " +
                            "FROM KASDA..BLJ_DETAIL A INNER JOIN KASDA..ANGKAS_DTL B ON B.Id_Rinci_Rs = A.Id_Rinci_Rs AND B.Id_Rinci_Rs = '" + textBox1.Text + "' GROUP BY B.Tot_Angkas", koneksi);
                        if (reader.HasRows)
                        {
                            reader.Read();
                            if ((Convert.ToDecimal(alat.PengecekField(reader, 0)) - Convert.ToDecimal(textBox2.Text)) < 0) // saldo tidak mencukupi
                            {
                                label3.Text = "OVER!!!"; // maka akan keluar blink2
                                timer1.Enabled = true;
                            }
                            else
                            {
                                label3.Text = "CUKUP";
                                timer1.Enabled = false;
                                label3.ForeColor = Color.Black;
                            }
                            toolSatusSisa.Text = string.Format(new System.Globalization.CultureInfo("id-ID"), "Rp. {0:n}", (Convert.ToDecimal(alat.PengecekField(reader, 0)) - Convert.ToDecimal(textBox2.Text)));
                            reader.Close();
                        }
                        else
                        {
                            MessageBox.Show("Kode Panggil Tidak Ditemukan", "PERHATIAN");
                            textBox1.Focus();
                        }
                    }
                    else
                        MessageBox.Show("SILAHKAN MASUKKAN NILAI YANG BENAR PADA KOTAK YG DISEDIAKAN!!!", "PERHATIAN");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SILAHKAN MASUKKAN NILAI YANG BENAR PADA KOTAK YG DISEDIAKAN!!!\nPesan Teknis: " + ex.Message, "PERHATIAN");
            }
            finally
            {
                koneksi.Close();
            }
        }

        //private void blink2(int status)
        //{
        //    if (status == 0)
        //    {
        //        if (timer1.Enabled == false)
        //            timer1.Enabled = true;
        //        else
        //            timer1.Enabled = false;
        //    }
        //    else if (status == 1)
        //    {

        //    }
        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (label3.ForeColor == Color.Black)
                label3.ForeColor = Color.Red;
            else if (label3.ForeColor == Color.Red)
                label3.ForeColor = Color.Black;
        }
    }
}
