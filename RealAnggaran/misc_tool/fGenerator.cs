using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;

namespace RealAnggaran.misc_tool
{
    public partial class fGenerator : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader reader = null;
        string noSPK = null;
        public int idOpp = 0;
        
        public fGenerator()
        {
            koneksi = konek.KonekDb();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        private string getOppAlias()
        {
            string kode = null;
            try
            {
                koneksi.Open();
                reader = konek.MembacaData("SELECT Kd_Opp FROM REALANGGAR..A_OPP WHERE id_Opp = " + idOpp + "", koneksi);
                if (reader.HasRows)
                {
                    reader.Read();
                    kode = alat.PengecekField(reader, 0);
                    reader.Close();
                }
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return kode;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string kode = getOppAlias();
            // berikut ini adalah contoh penggunaan string.format. dimana {0} untuk parameter 1st dan {1} parameter 2nd
            //MessageBox.Show(string.Format("Menit: {0:MM} \nJam: {1}", DateTime.Now, DateTime.Now.ToShortTimeString()));
            try
            {
                koneksi.Open();
                reader = konek.MembacaData("SELECT COUNT(*) FROM KASDA..BLJ_MASTER WHERE YEAR(Tgl_SP) = YEAR(GETDATE())", koneksi);
                if (reader.HasRows)
                {
                    reader.Read();
                    if (alat.PengecekField(reader, 0) == "0")
                    {
                        Clipboard.SetText(string.Format("1/{0:MM}/{1:yy}/{2}", DateTime.Now, DateTime.Now, kode));
                        //MessageBox.Show("Test");
                    }
                    else
                        //MessageBox.Show(string.Format("{0}/{1:MM}/{2:yy}/{3}", alat.pengecekField(reader, 0),
                        //    DateTime.Now, DateTime.Now, kode));
                        Clipboard.SetText(string.Format("{0}/{1:MM}/{2:yy}/{3}", alat.PengecekField(reader, 0),
                            DateTime.Now, DateTime.Now, kode));
                    reader.Close();
                }
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(fUtama))
                    form.Show();
            }
        }
        
    }
}
