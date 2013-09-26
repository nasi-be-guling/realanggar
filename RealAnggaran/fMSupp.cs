using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fMSupp : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        string query = null;

        public fMSupp()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
            query = "INSERT INTO A_SUPPLIER (kdSupp, NamaSupp, alamat, No_Telp) VALUES ('" + txtkd.Text
                + "','" + txtNama.Text + "', '" + txtAlamat.Text + "','" + txtHP.Text + "')";
            if (txtkd.Text == "")
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN KODE SUPPLIER, SILAHKAN MASUKAN!", "PERHATIAN");
                txtkd.Focus();
            }
            else if (txtNama.Text == "")
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN NAMA SUPPLIER, SILAHKAN MASUKAN!", "PERHATIAN");
                txtNama.Focus();
            }
            else
            {
                if (konek.CekFieldUnik("A_SUPPLIER", "kdSupp", txtkd.Text) == false)
                {
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
                    koneksi.Close();
                    bersih2();
                }
                else
                {
                    if (MessageBox.Show("KODE YANG SAMA DITEMUKAN, LANJUTKAN DENGAN UPDATE?", "KONFIRMASI", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        koneksi.Open();
                        konek.MasukkanData("UPDATE A_SUPPLIER SET NamaSupp = '" + txtNama.Text + 
                            "', alamat = '" + txtAlamat.Text + "', No_Telp = '" + txtHP.Text + 
                            "' WHERE KdSupp = '" + txtkd.Text.Trim() + "'", koneksi);
                        koneksi.Close();
                    }
                    bersih2();
                }
            }
            tampilTabel();
        }

        private void tampilTabel()
        {
            lvTampil.Items.Clear();
            query = "SELECT * FROM A_SUPPLIER";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 3));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 4));

                    lvTampil.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
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

            ColumnHeader headerkodeSupp = this.lvTampil.Columns.Add("Kode Supplier", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNamaSupp = this.lvTampil.Columns.Add("Nama Supplier", 50 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerAlamatSupp = this.lvTampil.Columns.Add("Alamat", 30 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNIPSupp = this.lvTampil.Columns.Add("Contact Person", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);
        }

        private void bersih2()
        {
            alat.CleaningService(groupBox2);
        }

        private void fMSupp_Load(object sender, EventArgs e)
        {
            alat.DeSerial(this);
            if (konek.CekKoneksi(this) != null)
            {
                inisiasiListView();
                tampilTabel();
            }
            bersih2();
        }

        private void bKeluar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
