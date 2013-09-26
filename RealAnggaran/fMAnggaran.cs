using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fMAnggaran : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        string query = null;
        decimal total = 0;
        int idReken = 0;
        public int idOpp = 0;

        public fMAnggaran()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void bersih2()
        {
            alat.CleaningService(panel1);
            alat.CleaningService(groupBox2);
            txtTAng.Text = "2011";
            lUraian.Text = "[uraian]";
            txtTotal.Text = "";
            total = 0;
            idReken = 0;
        }

        private void hitungTotal()
        {
            total = Convert.ToDecimal(txtEnASub.Text) + Convert.ToDecimal(txtEnAFung.Text);
            //txtTotal.Text = total.ToString();
        }

        private void hitungSisaSub()
        {
            query = "SELECT ";
            //int total = Convert.ToInt32(txtTotal.Text);
            //int 
            koneksi.Open();
            
            koneksi.Close();
        }

        private int cekStatusAnggar()
        {
            int id_Reken = 0;
            query = "SELECT id_Reken FROM A_ANGGAR WHERE id_Reken = " + idReken + " AND status = 0";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                id_Reken = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                pembaca.Close();
            }
            koneksi.Close();
            return id_Reken;
        }

        //private void textBox4_TextChanged(object sender, EventArgs e)
        //{

        //}

        public void showRekening(string id_Rek, string noRek, string uraian)
        {
            idReken = Convert.ToInt16(id_Rek);
            txtRek.Text = noRek;
            lUraian.Text = uraian;
        }

        private void bSimpan_Click(object sender, EventArgs e)
        {
            int id_rek = cekStatusAnggar();
            decimal jSub = 0;
            decimal jFung = 0;
            int tahun = 2011;
            if (txtEnASub.Text.Trim() == "")
                jSub = 0;
            else
                jSub = Convert.ToDecimal(txtEnASub.Text);
            if (txtEnAFung.Text.Trim() == "")
                jFung = 0;
            else
                jFung = Convert.ToDecimal(txtEnAFung.Text);
            if (txtTAng.Text.Trim() == "")
                tahun = 2011;
            else
                tahun = Convert.ToInt16(txtTAng.Text);
            if (konek.CekFieldUnik("A_ANGGAR", "Id_Reken", idReken.ToString()) == false)
            {
                query = "INSERT INTO A_ANGGAR (ThnAnggar, Id_Reken, Subsidi, Sisasubsi, Fungsional, SisaFungsi, TotAnggar, id_Opp)" +
                    " VALUES ('" + txtTAng.Text + "'," + idReken + "," + jSub + "," + jSub + "," + jFung +
                    "," + jFung + "," + total + "," + idOpp + ")";
            }
            else
            {
                if (id_rek > 0)
                {
                    query = "UPDATE A_ANGGAR SET Subsidi = Subsidi + " + jSub + ", sisaSubsi = sisaSubsi + " + jSub + ", Fungsional = Fungsional + " + jFung +
                        ", SisaFungsi = SisaFungsi + " + jFung + ", TotAnggar = TotAnggar + " + total + ", id_Opp = " + idOpp + ", status = 1 WHERE Id_Reken = " + id_rek + "";
                }
                else
                {
                    koneksi.Open();
                    pembaca = konek.MembacaData("SELECT id_MstAnggar FROM A_ANGGAR WHERE id_Reken = " + idReken + "", koneksi);
                    if (pembaca.HasRows)
                    {
                        pembaca.Read();
                        query = "UPDATE A_ANGGAR SET Subsidi = Subsidi + " + jSub + ", sisaSubsi = sisaSubsi + " + jSub + ", Fungsional = Fungsional + " + jFung +
                            ", SisaFungsi = SisaFungsi + " + jFung + ", TotAnggar = TotAnggar + " + total + ", id_Opp = " + idOpp + " WHERE id_MstAnggar = " + Convert.ToInt16(alat.PengecekField(pembaca, 0)) + "";
                        pembaca.Close();
                    }
                    koneksi.Close();
                }
            }
            if (idReken == 0)
            {
                MessageBox.Show("Silahkan Masukan Kode Rekening!");
                txtRek.Focus();
            }
            else
            {

                koneksi.Open();
                konek.MasukkanData(query, koneksi);
                koneksi.Close();
                MessageBox.Show("Penyimpanan berhasil, Terimakasih");
                bersih2();
            }
            tampilTabel();
            totalAnggar();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void fMAnggaran_Load(object sender, EventArgs e)
        {
            alat.DeSerial(this);
            bersih2();
            inisiasiListView();
            tampilTabel();
            totalAnggar();
        }

        private void txtEnAFung_Leave(object sender, EventArgs e)
        {
            hitungTotal();
        }

        private void txtEnASub_Leave(object sender, EventArgs e)
        {
            hitungTotal();
        }

        private void txtRek_KeyPress(object sender, KeyPressEventArgs e)
        {
            string kd_rek = alat.FilterMBox(txtRek.Text);
            query = "SELECT id_reken, Rekening FROM A_REKENING WHERE kd_Reken = '" + kd_rek.Trim() + "'";
            if (e.KeyChar == (char)13)
            {
                koneksi.Open();
                pembaca = konek.MembacaData(query, koneksi);
                if (pembaca.HasRows)
                {
                    pembaca.Read();
                    idReken = Convert.ToInt16(alat.PengecekField(pembaca, 0));
                    lUraian.Text = alat.PengecekField(pembaca, 1);
                    pembaca.Close();
                }
                else
                {
                    MessageBox.Show("Rekening Dengan Kode Tersebut Tidak Ditemukan!!!");
                    txtRek.Focus();
                }
                koneksi.Close();
                //MessageBox.Show(query);
            }
        }

        private void bKeluar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtEnASub_TextChanged(object sender, EventArgs e)
        {
            if (txtEnASub.Text.Trim() != "")
                hitungTotal();
            else
                txtEnASub.Text = "0";
        }

        private void txtEnAFung_TextChanged(object sender, EventArgs e)
        {
            if (txtEnAFung.Text.Trim() != "")
                hitungTotal();
            else
                txtEnAFung.Text = "0";
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

            ColumnHeader headerTahun = this.lvTampil.Columns.Add("Tahun", 10 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerKode = this.lvTampil.Columns.Add("Kode Rekening", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerUraian = this.lvTampil.Columns.Add("Uraian", 40 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerjSub = this.lvTampil.Columns.Add("Jumlah Subsidi", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerrSub = this.lvTampil.Columns.Add("Realisasi", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headersSub = this.lvTampil.Columns.Add("Sisa", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerjFung = this.lvTampil.Columns.Add("Jumlah Fungsional", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerrFung = this.lvTampil.Columns.Add("Realisasi", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headersFung = this.lvTampil.Columns.Add("Sisa", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
            ColumnHeader headerTotal = this.lvTampil.Columns.Add("Total Anggaran", 20 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

            lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);
        }

        private void tampilTabel()
        {
            lvTampil.Items.Clear();
            query = "SELECT ThnAnggar, kd_Reken, Rekening, subsidi, RealSubsi, SisaSubsi, fungsional, RealFungsi, SisaFungsi," +
                " totAnggar FROM A_ANGGAR, A_REKENING WHERE A_REKENING.id_reken = A_ANGGAR.Id_reken";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    ListViewItem lvItem = new ListViewItem(alat.PengecekField(pembaca, 0));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 1));
                    lvItem.SubItems.Add(alat.PengecekField(pembaca, 2));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 3))));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 4))));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 5))));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 6))));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 7))));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 8))));
                    lvItem.SubItems.Add(string.Format("Rp. {0:n}", Convert.ToDecimal(alat.PengecekField(pembaca, 9))));

                    lvTampil.Items.Add(lvItem);
                }
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void totalAnggar()
        {
            query = "SELECT SUM(TotAnggar) AS 'Total' FROM A_ANGGAR";
            koneksi.Open();
            pembaca = konek.MembacaData(query, koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                txtTotal.Text = alat.PengecekField(pembaca, 0).ToString();
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void txtEnASub_Click(object sender, EventArgs e)
        {
            txtEnASub.SelectionStart = 0;
        }

        private void txtEnAFung_Click(object sender, EventArgs e)
        {
            txtEnAFung.SelectionStart = 0;
        }

        private void txtTAng_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
        }

        private void txtEnASub_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
        }

        private void txtEnAFung_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') e.KeyChar = (char)0;
        }

        private void txtRek_KeyUp(object sender, KeyEventArgs e)
        {
            fCari cari;
            if (e.KeyData == Keys.F8)
            {
                if ((cari = (fCari)alat.FormSudahDibuat(typeof(fCari))) == null)
                {
                    cari = new fCari();
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    //cari.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                    cari.groupBox1.Text = "REKENING";
                    cari.status = 2;
                    cari.query = "SELECT id_Reken, Kd_Reken, Rekening, NamaKpa FROM A_REKENING a, A_KPA b WHERE" +
                        " b.Id_Kpa = a.Id_Kpa AND Kd_Reken LIKE '%" + alat.FilterMBox(txtRek.Text).Trim() + "%'";
                    cari.StartPosition = FormStartPosition.CenterScreen;
                    cari.lvTampil.View = View.Details;
                    cari.lvTampil.AllowColumnReorder = true;
                    cari.lvTampil.FullRowSelect = true;
                    cari.lvTampil.GridLines = true;
                    cari.lvTampil.Sorting = System.Windows.Forms.SortOrder.Ascending;
                    cari.lvTampil.MultiSelect = false;

                    ColumnHeader headeridKet = cari.lvTampil.Columns.Add("id_ket", 0 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerkdRek = cari.lvTampil.Columns.Add("No Rekening", 30 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerUraian = cari.lvTampil.Columns.Add("Uraian", 30 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);
                    ColumnHeader headerKpa = cari.lvTampil.Columns.Add("KPA", 25 * Convert.ToInt16(lvTampil.Font.SizeInPoints), HorizontalAlignment.Center);

                    cari.lvTampil.Font = new Font("VERDANA", 11, FontStyle.Bold | FontStyle.Italic);

                    cari.TopMost = true;
                    cari.ShowDialog();
                }
                else
                {
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    cari.TopMost = true;
                    cari.Select();
                }
            }
        }
    }
}
