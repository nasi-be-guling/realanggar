using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fMRek : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        string query = null;
        string kodeRek = null;
        int kd_Kpa = 0;
        public fMRek()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
            query = "BEGIN TRAN INSERT INTO A_REKENING (Kd_Reken, Rekening, id_Kpa) VALUES ('" + kodeRek.Trim() + 
                "','" + txtUraian.Text + "','" + kd_Kpa + "') COMMIT TRAN";
            if (kodeRek.Trim() == "")
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN KODE REKENING, SILAHKAN MASUKAN!", "PERHATIAN");
                txtKd.Focus();
            }
            if (kd_Kpa == 0)
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN KODE KPA, SILAHKAN MASUKAN!", "PERHATIAN");
                ckdKPA.Focus();
            }
            else
            {
                if (konek.CekFieldUnik("A_REKENING", "kd_Reken", kodeRek) == false)
                {
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
            //        MessageBox.Show(kodeRek);
                    koneksi.Close();
                    bersih2();
                }
                else
                {
                    if (MessageBox.Show("KODE YANG SAMA DITEMUKAN, LAKUKAN UPDATE?", "KONFIRMASI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        koneksi.Open();
                        konek.MasukkanData("UPDATE A_REKENING SET Rekening = '" + txtUraian.Text + "' WHERE Kd_Reken = '" + txtKd.Text.Trim() +  "'", koneksi);
                        koneksi.Close();
                    }
                    bersih2();
                }
            }
            tampilTabel();
        }

        private void inisiasiListView()
        {
            // Set the view to show details
            lvTampil.View = View.Details;

            // Disallow the user from edit the item
            //lvTampil.LabelEdit = false;

            // Allow the user to rearrange columns
            lvTampil.AllowColumnReorder = true;

            // Select the item and subitems when selection is made
            //lvTampil.FullRowSelect = true;

            // Display the gridline
            lvTampil.GridLines = true;

            // Sort the items in the list in accending order
            lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;

            // Restrict the multiselect
            lvTampil.MultiSelect = false;

            ColumnHeader headerkodeRek = this.lvTampil.Columns.Add("Kode Rekening", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerUraian = this.lvTampil.Columns.Add("Uraian", 45 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKodeKPA = this.lvTampil.Columns.Add("Kode KPA", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNamaKPA = this.lvTampil.Columns.Add("Nama KPA", 30 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);
        }

        private void tampilTabel()
        {
            lvTampil.Items.Clear();
            query = "SELECT kd_Reken, Rekening, KdKpa, NamaKpa FROM A_REKENING a, A_KPA b WHERE b.Id_Kpa = a.Id_Kpa";
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

                    lvTampil.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void txtKd_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void txtKd_TextChanged(object sender, EventArgs e)
        {
            kodeRek = alat.FilterMBox(txtKd.Text);
            //txtUraian.Text = alat.filterMBox(txtKd.Text);
        }

        private void txtKd_Leave(object sender, EventArgs e)
        {
            //txt
        }

        private void ckdKPA_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ckdKPA_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void ckdKPA_TextChanged(object sender, EventArgs e)
        {
            query = "SELECT id_kpa, namaKpa FROM A_KPA WHERE kdKpa = '" + ckdKPA.Text.Trim() + "'";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                kd_Kpa = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                lNama.Text = alat.PengecekField(pembaca, 1);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void bersih2()
        {
            alat.CleaningService(groupBox2);
            kodeRek = "";
            kd_Kpa = 0;
            lNama.Text = "<nama_posisi>";
            txtKd.Focus();
        }

        private void ambilKdKpa()
        {
            query = "SELECT kdKpa FROM A_KPA";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ckdKPA.Items.Add(alat.PengecekField(pembaca, 0));
                }
                pembaca.Close();
            }
            koneksi.Close();                
        }

        private void fMRek_Load(object sender, EventArgs e)
        {
            alat.DeSerial(this);
            bersih2();
            ambilKdKpa();
            inisiasiListView();
            tampilTabel();
        }

        private void bKeluar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
