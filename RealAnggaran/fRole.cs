using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fRole : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        string query = null;
        string kode_role = null;
        int selectedItems = 1000;

        public fRole()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void inisiasiListView()
        {
            // Set the view to show details
            lvTampil2.View = View.Details;
            lvTampil1.View = View.Details;
            // Disallow the user from edit the item
            //lvTampil.LabelEdit = false;

            // Allow the user to rearrange columns
            lvTampil2.AllowColumnReorder = true;
            lvTampil1.AllowColumnReorder = true;
            // Select the item and subitems when selection is made
            //lvTampil.FullRowSelect = true;

            // Display the gridline
            lvTampil2.GridLines = true;
            lvTampil1.GridLines = true;
            // Sort the items in the list in accending order
            lvTampil2.Sorting = System.Windows.Forms.SortOrder.Ascending;
            lvTampil1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            // Restrict the multiselect
            lvTampil2.MultiSelect = false;
            lvTampil1.MultiSelect = false;
            // Full Row Selection
            lvTampil2.FullRowSelect = true;
            lvTampil1.FullRowSelect = true;

            ColumnHeader namaMenu2 = this.lvTampil2.Columns.Add("Nama Menu", 20 * Convert.ToInt16(lvTampil2.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader keterangan2 = this.lvTampil2.Columns.Add("Keterangan", 40 * Convert.ToInt16(lvTampil2.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader namaMenu1 = this.lvTampil1.Columns.Add("Nama Menu", 20 * Convert.ToInt16(lvTampil1.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader keterangan1 = this.lvTampil1.Columns.Add("Keterangan", 40 * Convert.ToInt16(lvTampil1.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader pk1 = this.lvTampil1.Columns.Add("kodePrimer", 0 * Convert.ToInt16(lvTampil1.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader pk2 = this.lvTampil2.Columns.Add("kodePrimer", 0 * Convert.ToInt16(lvTampil1.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil2.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);
            lvTampil1.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);   
        }

        private void tampilTabel()
        {
            lvTampil2.Items.Clear();
            query = "SELECT * FROM A_MENU";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 0));

                    lvTampil2.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void ambilSemuaForm()
        {
            //Assembly[] array = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (Assembly a in array)
            //{
            //    Type[] types = a.GetTypes();
            //    foreach (Type t in types)
            //    {
            //        if (t.IsPublic && t.BaseType == typeof(Form))
            //        {
                        
            //        }
            //    }
            //}
            //foreach (ToolStripMenuItem cobek in menuStrip.Items)
            //{
            //    //if
            //    //alat.writeToFile(@"C:\Tampung.txt", cobek.Name);
            //}
        }

        private void fRole_Load(object sender, EventArgs e)
        {
            //alat.deSerial(this);
            inisiasiListView();
            tampilTabel();
            bersih2();
        }

        private void bersih2()
        {
            alat.CleaningService(groupBox1);
            lvTampil1.Items.Clear();
            tampilTabel();
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            if (selectedItems < 999)
            {
                ListViewItem lvItem = new ListViewItem(lvTampil2.Items[selectedItems].Text);
                lvItem.SubItems.Add(lvTampil2.Items[selectedItems].SubItems[1].Text);
                lvItem.SubItems.Add(lvTampil2.Items[selectedItems].SubItems[2].Text);

                lvTampil1.Items.Add(lvItem);
                lvTampil2.Items[selectedItems].Remove();
                selectedItems = 1000;
            }
            else
            {
                MessageBox.Show("THERE IS NOTHIN' TO SELECT SIRE!");
            }
        }

        private void lvTampil2_MouseClick(object sender, MouseEventArgs e)
        {
            selectedItems = lvTampil2.SelectedItems[0].Index;
        }

        private void bRem_Click(object sender, EventArgs e)
        {
            if (selectedItems < 999)
            {
                ListViewItem lvItem = new ListViewItem(lvTampil1.Items[selectedItems].Text);
                lvItem.SubItems.Add(lvTampil1.Items[selectedItems].SubItems[1].Text);
                lvItem.SubItems.Add(lvTampil1.Items[selectedItems].SubItems[2].Text);

                lvTampil2.Items.Add(lvItem);
                lvTampil1.Items[selectedItems].Remove();
                selectedItems = 1000;
            }
            else
            {
                MessageBox.Show("THERE IS NOTHIN' TO SELECT SIRE!");
            }
        }

        private void lvTampil1_MouseClick(object sender, MouseEventArgs e)
        {
            selectedItems = lvTampil1.SelectedItems[0].Index;
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
        }

        private void bSimpan_MouseDown(object sender, MouseEventArgs e)
        {
            //query = "INSERT INTO A_MENU (nama_menu, keterangan) VALUES ('" + lvTampil1.Items[i].Text
            //    + "','" + lvTampil1.Items[i].SubItems[1].Text + "')";
            query = "EXECUTE sp_role '" + txtkd.Text + "', '" + txtNama.Text + "'";
            if (txtkd.Text == "")
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN KODE POSISI, SILAHKAN MASUKAN!", "PERHATIAN");
                txtkd.Focus();
            }
            else if (txtNama.Text == "")
            {
                MessageBox.Show("ANDA BELUM MEMASUKAN NAMA POSISI, SILAHKAN MASUKAN!", "PERHATIAN");
                txtNama.Focus();
            }
            else
            {
                if (konek.CekFieldUnik("A_ROLE", "kd_Role", txtkd.Text) == false)
                {
                    koneksi.Open();
                    pembaca = konek.MembacaData(query, koneksi);
                    if (pembaca.HasRows)
                    {
                        pembaca.Read();
                        kode_role = alat.PengecekField(pembaca, 0);
                        pembaca.Close();
                    }
                    koneksi.Close();
                }
                else
                {
                    MessageBox.Show("KODE YANG SAMA DITEMUKAN, SILAHKAN MASUKAN KODE LAIN!", "PERHATIAN");
                    txtkd.Text = "";
                    txtkd.Focus();
                }
            }

        }

        private void bSimpan_MouseUp(object sender, MouseEventArgs e)
        {
            koneksi.Open();
            for (int i = 0; i <= lvTampil1.Items.Count - 1; i++)
            {
                konek.MasukkanData("INSERT INTO A_D_ROLE (id_Role, id_Menu) VALUES ('" + kode_role + "','" + 
                    lvTampil1.Items[i].SubItems[2].Text + "')", koneksi);
            }
            koneksi.Close();
            MessageBox.Show("DATA POSISI TELAH TERSIMPAN");
            bersih2();
        }

        private void bKeluar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
