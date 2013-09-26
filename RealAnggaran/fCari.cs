using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fCari : Form
    {
        SqlDataReader pembaca = null;
        SqlConnection koneksi;
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        public int status = 0;
        public string query = null;

        // THIS SERVE UNDER THESE FORM
        fEnFront frontOffice;
        fMAnggaran anggar;


        public fCari()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void tampilTabel_kwitansi()
        {
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
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 5));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 6));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 7));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 8));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 9));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 10));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 11));

                    lvTampil.Items.Add(lvItem);       
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void tampilTabel_Rekening()
        {
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
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 5));
        
                    lvTampil.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void fCari_Load(object sender, EventArgs e)
        {
            if (status == 0)
                tampilTabel_kwitansi();
            else if (status == 1)
                tampilTabel_Rekening();
            else if (status == 2)
                tampilTabel_Rekening();
        }

        private void label37_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lvTampil_DoubleClick(object sender, EventArgs e)
        {
            if (status == 0)
                showTableKwitansi();
            else if (status == 1)
                showTableRek();
            else if (status == 2)
                showTableAnggar();
        }

        private void showTableRek()
        {
            if ((frontOffice = (fEnFront)alat.FormSudahDibuat(typeof(fEnFront))) == null)
            {
                frontOffice = new fEnFront();
                frontOffice.TopMost = true;
                frontOffice.ShowDialog();
            }
            else
            {
                frontOffice.Select();
                frontOffice.showRekening(lvTampil.SelectedItems[0].Text, 
                    lvTampil.SelectedItems[0].SubItems[1].Text, 
                    lvTampil.SelectedItems[0].SubItems[3].Text,
                    lvTampil.SelectedItems[0].SubItems[5].Text);
            }
            this.Close();
        }

        private void showTableKwitansi()
        {
            if ((frontOffice = (fEnFront)alat.FormSudahDibuat(typeof(fEnFront))) == null)
            {
                frontOffice = new fEnFront();
                frontOffice.TopMost = true;
                frontOffice.ShowDialog();
            }
            else
            {
                frontOffice.Select();
                //frontOffice.textBox10.Text = "dasd";
                frontOffice.tambahDaftar(lvTampil.SelectedItems[0].Text, lvTampil.SelectedItems[0].SubItems[1].Text,
                    lvTampil.SelectedItems[0].SubItems[2].Text, lvTampil.SelectedItems[0].SubItems[3].Text, lvTampil.SelectedItems[0].SubItems[5].Text, 
                    lvTampil.SelectedItems[0].SubItems[8].Text);
            }
            this.Close();
        }

        private void showTableAnggar()
        {
            if ((anggar = (fMAnggaran)alat.FormSudahDibuat(typeof(fMAnggaran))) == null)
            {
                anggar = new fMAnggaran();
                anggar.TopMost = true;
                anggar.ShowDialog();
            }
            else
            {
                anggar.Select();
                anggar.showRekening(lvTampil.SelectedItems[0].Text, 
                    lvTampil.SelectedItems[0].SubItems[1].Text,
                    lvTampil.SelectedItems[0].SubItems[2].Text);
            }
            this.Close();
        }

        private void lvTampil_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (status == 0)
                    showTableKwitansi();
                else if (status == 1)
                    showTableRek();
                else if (status == 2)
                    showTableAnggar();
            }
        }
    }
}





