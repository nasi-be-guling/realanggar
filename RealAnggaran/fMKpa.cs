using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fMKpa : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        string query = null;

        public fMKpa()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void bersih2()
        {
            alat.CleaningService(groupBox2);
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

            ColumnHeader headerkodeKPA = this.lvTampil.Columns.Add("Kode KPA", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNamaKPA = this.lvTampil.Columns.Add("Nama KPA", 50 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNIPKPA = this.lvTampil.Columns.Add("NIP/NBI KPA", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic); 
        }

        private void tampilTabel()
        {
            lvTampil.Items.Clear();
            query = "SELECT * FROM A_KPA";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 3));
                    lvTampil.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
            query = "INSERT INTO A_KPA (kdKpa, NamaKpa, Nip) VALUES ('" + txtkd.Text
                + "','" + txtNama.Text + "','" + txtNIP.Text + "')";
            if (txtkd.Text == "")
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN KODE KPA, SILAHKAN MASUKAN!", "PERHATIAN");
                txtkd.Focus();
            }
            else if (txtNama.Text == "")
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN NAMA KPA, SILAHKAN MASUKAN!", "PERHATIAN");
                txtNama.Focus();
            }
            else
            {
                if (konek.CekFieldUnik("A_KPA", "kdKpa", txtkd.Text) == false)
                {
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
                    koneksi.Close();
                    bersih2();
                }
                else
                {
                    MessageBox.Show("KODE YANG SAMA DITEMUKAN, SILAHKAN MASUKAN KODE LAIN!", "PERHATIAN");
                    txtkd.Text = "";
                    txtkd.Focus();
                }
            }
            tampilTabel();
        }

        private void fMKpa_Load(object sender, EventArgs e)
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
