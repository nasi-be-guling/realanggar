using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fMOpp : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlDataReader pembaca = null;
        SqlConnection koneksi;
        int kd_role = 0;
        string query = null;

        public fMOpp()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void fMOpp_Load(object sender, EventArgs e)
        {
            alat.DeSerial(this);
            if (konek.CekKoneksi(this) != null)
            {
                inisiasiListView();
                tampilTabel();
                ambilPosisi();
            }
            alat.CleaningService(groupBox2);
            lNama.Text = "<nama_posisi>";
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

            ColumnHeader headerkodeOpp = this.lvTampil.Columns.Add("Kode Operator", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNamaOpp = this.lvTampil.Columns.Add("Nama Operator", 50 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNIPOpp = this.lvTampil.Columns.Add("NIP/NBI Operator", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerkdPos = this.lvTampil.Columns.Add("Kode Posisi", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerNamaPos = this.lvTampil.Columns.Add("Nama Posisi Operator", 35 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);   
        }

        private void tampilTabel()
        {
            lvTampil.Items.Clear();
            query = "SELECT * FROM A_OPP A, A_ROLE B WHERE B.id_Role = A.id_Role";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 3));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 7));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 8));

                    lvTampil.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void ambilPosisi()
        {
            query = "SELECT Kd_Role FROM A_ROLE";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                //pembaca.Read();
                while (pembaca.Read())
                {
                    cKdPos.Items.Add(alat.PengecekField(pembaca, 0));
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
            query = "INSERT INTO A_OPP (kd_Opp, Nama_Opp, Nip_Opp, passwd, id_Role) VALUES ('" + txtkdOp.Text
                + "','" + txtNama.Text + "','" + txtNIP.Text + "','" + txtPass2.Text + "'," + kd_role + ")";
            if (txtkdOp.Text == "")
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN KODE OPERATOR, SILAHKAN MASUKAN!", "PERHATIAN");
                txtkdOp.Focus();
            }
            else if (txtPass2.Text == "" | txtPass2.Text != txtPass1.Text)
            {
                MessageBox.Show("PASSWORD YANG ANDA MAKSUDKAN TIDAK SESUAI, SILAHKAN MASUKAN LAGI!", "PERHATIAN");
                txtPass1.Focus();
                txtPass1.Text = "";
                txtPass2.Text = "";
            }
            else if (cKdPos.Text == "")
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN KODE POSISI, SILAHKAN MASUKAN!", "PERHATIAN");
                cKdPos.Focus();
            }
            else
            {
                if (konek.CekFieldUnik("A_OPP", "kd_Opp", txtkdOp.Text) == false)
                {
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
                    koneksi.Close();
                    alat.CleaningService(groupBox2);
                    lNama.Text = "<nama_posisi>";
                }
                else
                {
                    MessageBox.Show("KODE YANG SAMA DITEMUKAN, SILAHKAN MASUKAN KODE LAIN!", "PERHATIAN");
                    txtkdOp.Text = "";
                    txtkdOp.Focus();
                }
            }
            tampilTabel();
        }

        private void cKdPos_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cKdPos_Validated(object sender, EventArgs e)
        {

        }

        private void cKdPos_TextUpdate(object sender, EventArgs e)
        {

        }

        private void cKdPos_TextChanged(object sender, EventArgs e)
        {
            query = "SELECT id_Role, Nama_Role FROM A_ROLE WHERE Kd_Role = '" + cKdPos.Text.Trim() + "'";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                kd_role = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                lNama.Text = alat.PengecekField(pembaca, 1);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void bKeluar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNIP_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
