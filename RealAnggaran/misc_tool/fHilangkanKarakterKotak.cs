using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;

namespace RealAnggaran.misc_tool
{
    public partial class fHilangkanKarakterKotak : Form
    {
        CKonek konek = new CKonek();
        CKonek konek1 = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlConnection koneksi1;
        SqlDataReader reader = null;
        string query = null;

        public fHilangkanKarakterKotak()
        {
            koneksi = konek.KonekDb();
            koneksi1 = konek.KonekDb();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            koneksi.Open();
            reader = konek.MembacaData("SELECT " + textBox3.Text + " FROM " + textBox2.Text + "", koneksi);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    progressBar1.Maximum++;
                    updating(alat.PengecekField(reader, 0));
                    progressBar1.Value++; 
                    //MessageBox.Show(reader[0].ToString());
                }
                reader.Close();
            }            
            koneksi.Close();
            MessageBox.Show("SUKSES");
            progressBar1.Maximum = 0;
            progressBar1.Value = 0;
            button1.Enabled = true;
        }

        private void updating(string idField)
        {
            query = "UPDATE " + textBox2.Text + " SET " + textBox1.Text + " = (SELECT REPLACE(REPLACE(REPLACE(REPLACE(" + textBox1.Text + ", '/', ''), CHAR(10), ''), CHAR(13), ''), ' ', '') AS NOMER " +
                "FROM " + textBox2.Text + " WHERE " + textBox3.Text + " = '" + idField + "') WHERE " + textBox3.Text + " = '" + idField + "'";
            koneksi1.Open();
            konek1.MasukkanData(query, koneksi1);
            //MessageBox.Show(query);
            koneksi1.Close();
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.SelectAll();
        }

        private void textBox3_MouseClick(object sender, MouseEventArgs e)
        {
            textBox3.SelectAll();
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            textBox2.SelectAll();
        }
    }
}
