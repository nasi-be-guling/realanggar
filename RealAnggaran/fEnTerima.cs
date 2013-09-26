using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;

namespace RealAnggaran
{
    public partial class fEnTerima : Form
    {
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        string query = null;
        public int idOpp = 0;
        int idKpa = 0;
        int idSumber = 0;
        bool statusDelete = false;
        bool statusUpdate = false;
        int opsiUpdate = 0;
        int id_Terima = 0;
        private ListViewColumnSorter lvwColumnSorter;

        public fEnTerima()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            InitializeComponent();
            koneksi = konek.KonekDb();
            dtTanggal.CustomFormat = "dd/MM/yyyy";
            dtTanggal.Format = DateTimePickerFormat.Custom;
            lvwColumnSorter = new ListViewColumnSorter();
            this.lvTampil.ListViewItemSorter = lvwColumnSorter;
        }

        private void bersih2()
        {
            alat.CleaningService(groupBox2);
            ambilSisa();
            txtJTerima.Focus();
            idKpa = 0;
            idSumber = 0;
            label12.Text = "";
            label13.Text = "";
            id_Terima = 0;
            statusDelete = false;
            statusUpdate = false;
            opsiUpdate = 0;
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
//            MessageBox.Show(idKpa.ToString() + "...." + idSumber.ToString());
            //string tglSkrg = alat.dateFormater(DateTime.Now.ToShortDateString().ToString());
            string tglSkrg = dtTanggal.Text + " " + string.Format("{0:HH:mm:ss}", DateTime.Now);
            query = "EXECUTE sp_terima '" + dtTanggal.Text + " " + string.Format("{0:HH:mm:ss}", DateTime.Now) + " ', '" + filtertxt(txtJTerima.Text.Trim()) + "', '" + txtKet.Text.Trim() + "', " + idOpp + ", " + idKpa + ", " + idSumber + "";
            //MessageBox.Show(query);
            if (txtJTerima.Text == "" | txtJTerima.Text == "0")
            {
                MessageBox.Show("Silahkan masukkan data Penerimaan!","PERHATIAN");
                txtJTerima.Focus();
            }
            else if (txtKet.Text.Trim() == "")
            {
                MessageBox.Show("Silahkan masukkan Keterangan!", "PERHATIAN");
                txtKet.Focus();
            }
            else if (idKpa == 0)
            {
                MessageBox.Show("Silahkan masukkan Kode KPA!", "PERHATIAN");
                comboBox1.Focus();
            }
            else if (idSumber == 0)
            {
                MessageBox.Show("Silahkan masukkan Kode SUMBER!", "PERHATIAN");
                comboBox2.Focus();
            }
            else
            {
                koneksi.Open();
                konek.MasukkanData(query, koneksi);
                koneksi.Close();
                MessageBox.Show("Data Telah Tersimpan, Terimakasih!", "PERHATIAN");
                bersih2();
                tampilTabel();
            }
        }

        private void isiKomboKPA()
        {
            comboBox1.Items.Clear();
            koneksi.Open();
            pembaca = konek.MembacaData("SELECT * FROM A_KPA", koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    comboBox1.Items.Add(alat.PengecekField(pembaca, 1));
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void isiKomboSumber()
        {
            comboBox2.Items.Clear();
            koneksi.Open();
            pembaca = konek.MembacaData("SELECT * FROM A_SUMBER_DANA", koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    comboBox2.Items.Add(alat.PengecekField(pembaca, 1));
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
            lvTampil.FullRowSelect = true;

            // Display the gridline
            lvTampil.GridLines = true;

            // Sort the items in the list in accending order
            lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;

            // Restrict the multiselect
            lvTampil.MultiSelect = true;

            //Descending
            //lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;

            ColumnHeader headerTglTerima = this.lvTampil.Columns.Add("Tanggal Terima", 15 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKPA = this.lvTampil.Columns.Add("KPA", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSumber = this.lvTampil.Columns.Add("SUMBER", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSaldoAwal = this.lvTampil.Columns.Add("Saldo Awal", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerJmlh = this.lvTampil.Columns.Add("Jumlah Terima", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            //ColumnHeader headerKeluar = this.lvTampil.Columns.Add("Pengeluaran", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerSisa = this.lvTampil.Columns.Add("Sisa", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKet = this.lvTampil.Columns.Add("Keterangan", 40 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headeridTerima = this.lvTampil.Columns.Add("Id Terima", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            
            lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);
        }

        private void tampilTabel()
        {
            lvTampil.Items.Clear();
            query = "SELECT Id_Terima, TglTerima, SaldoAwal, JmTerima, Keluar, Sisa, ket, Dipakai, id_Opp, a.idSumber, a.id_Kpa, b.Id_kpa, KdKpa, NamaKpa, NIP, c.idSumber, KdSumber, namaSumber " +
                "FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c WHERE b.Id_Kpa = a.Id_Kpa AND c.idSumber = a.idSumber AND (ket = 'saldo awal' or ket = 'penerimaan') ORDER BY TglTerima ASC";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 12));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 16));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 2))));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 3))));
                    //lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.pengecekField(pembaca, 4))));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 5))));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 6));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 0));
                    lvTampil.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void selectIsiListView()
        {
            switchingButton(true);
            //dtTanggal.Text = 
            //MessageBox.Show(String.Format("{0: MM/dd/yyyy}", Convert.ToDateTime(lvTampil.SelectedItems[0].Text.Trim())));
            //MessageBox.Show(lvTampil.SelectedItems[0].Text);
            
            txtJTerima.Text = lvTampil.SelectedItems[0].SubItems[4].Text.Replace("Rp. ", "");
            txtJTerima.Text = txtJTerima.Text.Replace(",", "");
            txtJTerima.Text = txtJTerima.Text.Replace(".", ",");
            txtKet.Text = lvTampil.SelectedItems[0].SubItems[6].Text;
            comboBox1.Text = lvTampil.SelectedItems[0].SubItems[1].Text;
            comboBox2.Text = lvTampil.SelectedItems[0].SubItems[2].Text;
            id_Terima = Convert.ToInt16(lvTampil.SelectedItems[0].SubItems[7].Text);
            //int jumlahList = lvTampil.Items.Count - 1;
            //int posisiIndeksSekarang = lvTampil.SelectedItems[0].Index;
            //if (posisiIndeksSekarang < jumlahList)
            //    statusDelete = false;
            //else if (posisiIndeksSekarang >= jumlahList)
            //{
                //statusDelete = true;
                koneksi.Open();
                pembaca = konek.MembacaData("SELECT id_kpa, idSumber, Dipakai FROM A_PENERIMAAN WHERE id_Terima = " +
                    id_Terima + "", koneksi);
                if (pembaca.HasRows)
                {
                    pembaca.Read();
                    //MessageBox.Show(id_Terima.ToString() + " " + alat.pengecekField(pembaca, 0) + " " + alat.pengecekField(pembaca, 1) + " " + alat.pengecekField(pembaca, 2));
                    idKpa = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                    idSumber = Convert.ToInt16(alat.PengecekField(pembaca, 1));
                    if (alat.PengecekField(pembaca, 2).ToString().Trim() == "Y")
                        statusDelete = false;
                    else if (alat.PengecekField(pembaca, 2).ToString().Trim() == "-")
                        statusDelete = true;
                    pembaca.Close();
                }
                koneksi.Close();
            //}
        }

        private void ambilSisa()
        {
            query = "SELECT SUM(sisa) FROM A_PENERIMAAN WHERE Dipakai = '-'";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                txtSaldo.Text = alat.PengecekField(pembaca, 0);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void fEnTerima_Load(object sender, EventArgs e)
        {
            alat.DeSerial(this);
            inisiasiListView();
            tampilTabel();
            bersih2();
            isiKomboSumber();
            isiKomboKPA();
        }

        private void txtJTerima_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 31 && (e.KeyChar < '0' || e.KeyChar > '9'))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
        }

        private void bKeluar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtJTerima_Click(object sender, EventArgs e)
        {
            txtJTerima1.SelectionStart = 0;
        }

        private void switchingButton(bool status)
        {
            if (status == true)
            {
                button1.Visible = true;
                bSimpan.Visible = false;
            }
            else
            {
                button1.Visible = false;
                bSimpan.Visible = true;
            }
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            koneksi.Open();
            pembaca = konek.MembacaData("SELECT * FROM A_KPA WHERE kdKpa = '" + comboBox1.Text.Trim() + "'", koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                idKpa = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                label12.Text = alat.PengecekField(pembaca, 2);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            koneksi.Open();
            pembaca = konek.MembacaData("SELECT * FROM A_SUMBER_DANA WHERE kdSumber = '" + comboBox2.Text.Trim() + "'", koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                idSumber = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                label13.Text = alat.PengecekField(pembaca, 2);
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void bHapus_Click(object sender, EventArgs e)
        {
            if (statusDelete == false)
            {
                MessageBox.Show("Tidak bisa mengapus data,\nsilahkan cek apa" +
                    " data sudah benar\natau hanya bisa menghapus transaksi trakhir.", "PERHATIAN");
                switchingButton(false);
                bersih2();
            }
            else if (statusDelete == true)
            {
                if (id_Terima != 0)
                {
                    koneksi.Open();
                    konek.MasukkanData("EXECUTE sp_hapus_terima " + id_Terima + ", " + idKpa + ", " + idSumber + ", " + idOpp + "", koneksi);
                    koneksi.Close();
                }
                MessageBox.Show("Hapus data sukses", "PERHATIAN");
                switchingButton(false);
                statusDelete = false;
                bersih2();
                tampilTabel();
            }
        }

        private void lvTampil_DoubleClick(object sender, EventArgs e)
        {
            selectIsiListView();
            statusUpdate = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switchingButton(false);
            statusUpdate = false;
            bersih2();
        }

        private void lvTampil_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == System.Windows.Forms.SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lvTampil.Sort();

        }

        private void txtJTerima_KeyDown(object sender, KeyEventArgs e)
        {
            alat.FilterTextBox(sender, e);
        }

        private void txtJTerima_Leave(object sender, EventArgs e)
        {
            //mytoolTip.Hide(txtPPn);
            string temp = txtJTerima.Text;
            if (txtJTerima.Text == "")
                txtJTerima.Text = "0";
            else
            {
                try
                {
                    txtJTerima.Text = string.Format(new System.Globalization.CultureInfo("id-ID"), "{0:n}", Convert.ToDecimal(filtertxt(temp)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            temp = "0";
        }

        private string filtertxt(string teks)
        {
            teks = teks.Replace(".", "");
            teks = teks.Replace(",", ".");
            return teks;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (opsiUpdate == 0)
                query = "UPDATE REALANGGAR.dbo.A_PENERIMAAN SET TglTerima = CONVERT(DATETIME, '" + dtTanggal.Text + " " + string.Format("{0:HH:mm:ss}", DateTime.Now) + "', 103)" +
                    " WHERE Id_Terima = " + id_Terima + "";
            else if (opsiUpdate == 1)
                query = "EXECUTE REALANGGAR..sp_koreksi_terima " + id_Terima + ", " + idKpa + ", " + idSumber + ", " + idOpp + ", " + filtertxt(txtJTerima.Text.Trim()) + "";
            if (statusUpdate == false)
            {
                MessageBox.Show("Silahkan pilih data yg akan di update","PERHATIAN");
            }
            else if (statusUpdate == true)
            {
                if (txtJTerima.Text == "" | txtJTerima.Text == "0")
                {
                    MessageBox.Show("Silahkan masukkan data Penerimaan!", "PERHATIAN");
                    txtJTerima.Focus();
                }
                else if (txtKet.Text.Trim() == "")
                {
                    MessageBox.Show("Silahkan masukkan Keterangan!", "PERHATIAN");
                    txtKet.Focus();
                }
                else if (idKpa == 0)
                {
                    MessageBox.Show("Silahkan masukkan Kode KPA!", "PERHATIAN");
                    comboBox1.Focus();
                }
                else if (idSumber == 0)
                {
                    MessageBox.Show("Silahkan masukkan Kode SUMBER!", "PERHATIAN");
                    comboBox2.Focus();
                }
                else
                {
                    koneksi.Open();
                    konek.MasukkanData(query, koneksi);
                    koneksi.Close();
                    MessageBox.Show("Data Telah Tersimpan, Terimakasih!", "PERHATIAN");
                    bersih2();
                    tampilTabel();
                    statusUpdate = false;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(lvTampil.SelectedItems[0].SubItems[7].Text);
            koneksi.Open();
            //MessageBox.Show("EXECUTE sp_update_saldo " + lvTampil.SelectedItems[0].SubItems[7].Text + ", '" + dtTanggal.Text + " " +
            //    string.Format("{0:HH:mm:ss}", DateTime.Now) + "', '" + filtertxt(txtJTerima.Text.Trim()) + "', '" +
            //    txtKet.Text.Trim() + "', " + idOpp + ", " + idKpa + ", " + idSumber + "");
            konek.MasukkanData("EXECUTE sp_update_saldo " + lvTampil.SelectedItems[0].SubItems[7].Text + ", '" + dtTanggal.Text + " " +
                string.Format("{0:HH:mm:ss}", DateTime.Now) + "', '" + filtertxt(txtJTerima.Text.Trim()) + "', '" +
                txtKet.Text.Trim() + "', " + idOpp + ", " + idKpa + ", " + idSumber + "", koneksi);
    //        sp_update_saldo
    //@id_terima int, 
    //@tglTerima varchar(20),
    //@jmlTerima numeric (13),
    //@ket text,
    //@opp int,
    //@idKpa int,
    //@idSumber int
            koneksi.Close();
        }

        private void comboBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                //Convert.ToDecimal(filtertxt(txtPPh.Text.Trim())) + Convert.ToDecimal(filtertxt(txtPPn.Text.Trim()));
                koneksi.Open();
                /* reader = konek.membacaData("SELECT sisa, namaSumber, NamaKpa " +
                    "FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c " +
                    "WHERE (b.Id_Kpa = a.Id_Kpa AND c.idSumber = a.idSumber) AND a.Id_Kpa = " + idKPA + " AND a.idSumber = " + idSumber + " " +
                    " ORDER BY TglTerima DESC", koneksi); */
                pembaca = konek.MembacaData("SELECT sisa, namaSumber, NamaKpa " +
                    "FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c " +
                    "WHERE (b.Id_Kpa = a.Id_Kpa AND c.idSumber = a.idSumber) AND a.Id_Kpa = " + idKpa + " AND a.idSumber = " + idSumber + " " +
                    " AND (a.Dipakai = '-')", koneksi);
                if (pembaca.HasRows)
                {
                    pembaca.Read();
                    txtSaldo.Text = alat.PengecekField(pembaca, 0);
                    pembaca.Close();
                }
                else
                {
                    txtSaldo.Text = "0";
                }
                koneksi.Close();
            }
        }

        private void txtJTerima_Enter(object sender, EventArgs e)
        {
            opsiUpdate = 1;
        }

    }
}
