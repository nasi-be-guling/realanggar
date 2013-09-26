using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fMJenis : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        string query = null;
        int sumberDana = 0;

        public fMJenis()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void bersih2()
        {
            alat.CleaningService(groupBox2);
            alat.CleaningService(groupBox3);
            sumberDana = 0;
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

            ColumnHeader headerkodeJen = this.lvTampil.Columns.Add("Kode Jenis", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNamaJen = this.lvTampil.Columns.Add("Nama Jenis", 50 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);

            // Set the view to show details
            listView1.View = View.Details;

            // Disallow the user from edit the item
            //lvTampil.LabelEdit = false;

            // Allow the user to rearrange columns
            listView1.AllowColumnReorder = true;

            // Select the item and subitems when selection is made
            //lvTampil.FullRowSelect = true;

            // Display the gridline
            listView1.GridLines = true;

            // Sort the items in the list in accending order
            listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;

            // Restrict the multiselect
            listView1.MultiSelect = false;

            ColumnHeader headerkodeJen1 = this.listView1.Columns.Add("Kode", 15 * Convert.ToInt16(listView1.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNamaJen1 = this.listView1.Columns.Add("Keterangan", 50 * Convert.ToInt16(listView1.Font.SizeInPoints), HorizontalAlignment.Center);

            listView1.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);
        }

        private void tampilTabel()
        {
            lvTampil.Items.Clear();
            query = "SELECT * FROM A_JENIS";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    lvTampil.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void tampilTabel1()
        {
            listView1.Items.Clear();
            query = "SELECT * FROM A_SUMBER_DANA";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    listView1.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void fMJenis_Load(object sender, EventArgs e)
        {
            alat.DeSerial(this);
            if (konek.CekKoneksi(this) != null)
            {
                inisiasiListView();
                tampilTabel();
                tampilTabel1();
            }
            bersih2();
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
            query = "INSERT INTO A_JENIS (kdJenis, Jenis) VALUES ('" + txtkd.Text
                + "','" + txtNama.Text + "')";
            if (sumberDana == 0)
            {
                if (txtkd.Text == "")
                {
                    MessageBox.Show("ANDA BELUM MEMASUKAN KODE JENIS, SILAHKAN MASUKAN!", "PERHATIAN");
                    txtkd.Focus();
                }
                else if (txtNama.Text == "")
                {
                    MessageBox.Show("ANDA BELUM MEMASUKAN URAIAN JENIS, SILAHKAN MASUKAN!", "PERHATIAN");
                    txtNama.Focus();
                }
                else
                {
                    if (konek.CekFieldUnik("A_JENIS", "kdJenis", txtkd.Text) == false)
                    {
                        koneksi.Open();
                        konek.MasukkanData(query, koneksi);
                        koneksi.Close();
                        bersih2();
                    }
                    else
                    {
                        if (MessageBox.Show("KODE YANG SAMA DITEMUKAN, LAKUKAN UPDATE?", "KONFIRMASI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            koneksi.Open();
                            konek.MasukkanData("UPDATE A_JENIS SET Jenis = '" + txtNama.Text + "' WHERE KdJenis = '" + txtkd.Text.Trim() + "'", koneksi);
                            koneksi.Close();
                        }
                        bersih2();
                    }
                }
            }
            else if (sumberDana == 1)
            {
                if (textBox2.Text == "")
                {
                    MessageBox.Show("ANDA BELUM MEMASUKAN KODE, SILAHKAN MASUKAN!", "PERHATIAN");
                    textBox2.Focus();
                }
                else if (textBox1.Text == "")
                {
                    MessageBox.Show("ANDA BELUM MEMASUKAN URAIAN, SILAHKAN MASUKAN!", "PERHATIAN");
                    textBox1.Focus();
                }
                else
                {
                    if (konek.CekFieldUnik("A_SUMBER_DANA", "kdSumber", textBox2.Text) == false)
                    {
                        koneksi.Open();
                        konek.MasukkanData("INSERT INTO A_SUMBER_DANA VALUES ('" + textBox2.Text + "','" + textBox1.Text + "')", koneksi);
                        koneksi.Close();
                        bersih2();
                    }
                    else
                    {
                        if (MessageBox.Show("KODE YANG SAMA DITEMUKAN, LAKUKAN UPDATE?", "KONFIRMASI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            koneksi.Open();
                            konek.MasukkanData("UPDATE A_SUMBER_DANA SET namaSumber = '" + textBox1.Text + "' WHERE KdSumber = '" + textBox2.Text.Trim() + "'", koneksi);
                            koneksi.Close();
                        }
                        bersih2();
                    }
                }
            }
            tampilTabel();
            tampilTabel1();
        }

        private void bKeluar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtkd_Validating(object sender, CancelEventArgs e)
        {
            sumberDana = 0;
        }

        private void txtNama_Validating(object sender, CancelEventArgs e)
        {
            sumberDana = 0;
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            sumberDana = 1;
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            sumberDana = 1;
        }
    }
}
