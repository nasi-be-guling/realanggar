using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class fUtama : Form
    {
        //private int childFormNumber = 0;
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        string query = null;
        int status = 1;
        string eksepsi = null;
        int lebarLayar = Screen.PrimaryScreen.WorkingArea.Size.Width - 10;
        int tinggiLayar = Screen.PrimaryScreen.WorkingArea.Size.Height - 100;
        
        // LIST OF FORM
        fEnTerima terima;
        fEnFront bayar;
        fLogin login;
        fMAnggaran anggar;
        fMJenis jenis;
        fMKpa kpa;
        fMOpp Opp;
        Revisi.masterRekening rekening;
        fMSupp supplier;
        fRole rolePlaying;
        fSetConn setConn;
        fLunas kasir;
        fLAP_KELUAR lapKeluar;
        fSetFont settingFont;
        //fKasdanya kasdanya;
        Revisi.fKasir kasdanya;
        Revisi.fSubsidi subsidiNYA_COK;
        Revisi.fCekAnggaran cekAnggaran;
        cetak.FCetakTransaksi exportExcel;
        misc_tool.fCekSaldoPPTK cekSaldoPptk;
        cetak.fCetakLapEvi cekLapEvi;
        decrypt dekripKoneksi;
        private Revisi.FMundurTanggal mundurTanggal;

        public fUtama()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            //Form childForm = new Form();
            //childForm.MdiParent = this;
            //childForm.Text = "Window " + childFormNumber++;
            //childForm.Show();
            showLogin();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            //if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            //{
            //    string FileName = openFileDialog.FileName;
            //}
            disabler();
            foreach (Form child in this.MdiChildren)
                child.Close();
            mLogin.Enabled = true;
            txtkd_Pos.Text = "";
            statusUser.Text = "User Aktip : .:|:. Posisi :";
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fEnFront mOpp = new fEnFront();
            mOpp.MdiParent = this;
            mOpp.Size = new Size(this.Width - 15, this.Height - 110);
            mOpp.StartPosition = FormStartPosition.CenterScreen;
            mOpp.Show();
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        
        }

        private void daftarMenu()
        {
            //TextWriter tW = new StreamWriter("listOfMenu.txt");
            //foreach (ToolStripMenuItem item in menuStrip.Items)
            //{
            //    //if (item.DropDownItems.GetType().ToString() == "System.Windows.Forms.ToolStripSeparator")
            //    //{

            //    //}
            //    //else
            //    //{
            //    //    foreach (Control subItem in item.DropDownItems)
            //    //    {
            //    //        alat.writeToFile(@"C:\Tampung.txt", subItem.Text);
            //    //    }
            //    //}
            //    for (int i = 0; i < item.DropDownItems.Count; i++)
            //    {
            //        //alat.writeToFile(@"C:\Tampung.txt", item.DropDownItems[0].Text);
            //        //    alat.writeToFile(@"C:\Tampung.txt", item.DropDownItems.Count.ToString());
            //        //alat.writeToFile(@"listOfMenu.txt", item.DropDownItems[i].Name);
            //        tW.WriteLine(item.DropDownItems[i].Name);
                    
            //    }
            //}
            //tW.Close();
        }

        private void showLogin()
        {
            fLogin login = new fLogin();
            login.StartPosition = FormStartPosition.CenterScreen;
            login.TopMost = true;
            login.ShowDialog();
        }

        private void fUtama_Load(object sender, EventArgs e)
        {
            //daftarMenu();
            this.Text = "RealAnggaran ver." + Application.ProductVersion.ToString();
            statusUser.Text = " : HARAP TUNGGU, SEDANG MELAKUKAN KONEKSI DENGAN SERVER !!!";
            timer1.Enabled = true;
            alat.WriteFileVersion();
            disabler();
            //fillTxt();
            backgroundWorker1.RunWorkerAsync();
            //showLogin();
            //txtkd_Pos.Text = "6";
        }

        private void fUtama_Activated(object sender, EventArgs e)
        {
        }

        //private ToolStripMenuItem set(ToolStripMenuItem benar, to

        private void txtkd_Pos_TextChanged(object sender, EventArgs e)
        {
            //char item = Convert.ToChar("mTerima");
            //menuItem = ;
            //menuItem.Enabled = false;
            /*
             * 
             * NOW IM STUCK...
             * I SHOULD BE SOLVING THIS MATTER  !!!!!!!!!!!
             * MEANWHILE I'LL BE DOING ANOTHER FORM
             * 
             */
            //foreach (ToolStripMenuItem item in menuStrip.Items)
            //{
            //    //if (item.Tag.ToString() == "1")
            //    //    item.Enabled = false;
            //}
            /*
             * finnally solve the matter, not soo nice but it work, should be anyone jerking this
             * function they better bring solution along with 'em
             */
            //disabler();
            if (txtkd_Pos.Text != "")
                check_id(txtkd_Pos.Text);
        }

        private void disabler()
        {
            foreach (ToolStripItem item in fileMenu.DropDownItems)
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in transToolStripMenuItem.DropDownItems)
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in masterToolStripMenuItem.DropDownItems)
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in transToolStripMenuItem.DropDownItems)
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in keyGenToolStripMenuItem.DropDownItems)
            {
                item.Enabled = false;
            } 
            foreach (ToolStripItem item in optionsToolStripMenuItem.DropDownItems)
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in mLaporan.DropDownItems)
            {
                item.Enabled = false;
            }

            mExit.Enabled = true;
        }

        private void check_id(string id)
        {
            query = "SELECT d.nama_menu, a.Nama_Opp, b.Nama_Role FROM A_OPP a, A_ROLE b, A_D_ROLE c, A_MENU" +
                " d WHERE ((a.id_opp = '" + id + "' AND b.id_Role = a.id_Role) AND (c.id_role = b.id_role" +
                " AND d.id_menu = c.id_menu))";
            koneksi.Open();
            //DataTable dTabel = konek.dTabel(query, koneksi);
            pembaca = konek.MembacaData(query, koneksi);
            //DataRow dRow = null;
            //if (dTabel.Rows.Count > 0)
            //{
            //    DataTableReader dPembaca = dTabel.CreateDataReader();
            //    dPembaca.Read();
            //    for (int i = 0; i <= dTabel.Rows.Count; i++)
            //    {
            if (pembaca.HasRows)
            {
                while (pembaca.Read())
                {
                    foreach (ToolStripItem item in fileMenu.DropDownItems)
                    {
                        if (item.Name == alat.PengecekField(pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in transToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == alat.PengecekField(pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in masterToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == alat.PengecekField(pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in transToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == alat.PengecekField(pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in optionsToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == alat.PengecekField(pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in mLaporan.DropDownItems)
                    {
                        if (item.Name == alat.PengecekField(pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in keyGenToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == alat.PengecekField(pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    statusUser.Text = "User Aktip : " + alat.PengecekField(pembaca, 1) + " .:|:. Posisi : " +
                        alat.PengecekField(pembaca, 2);
                }
                pembaca.Close();
            }
                    //dRow = dTabel.Rows.;
            //    }
            //    MessageBox.Show(dPembaca[0].ToString());
            //}
            koneksi.Close();
            mLogin.Enabled = false;
            mLogout.Enabled = true;
        }

        private void mTerima_Click(object sender, EventArgs e)
        {
            //if ((terima = (fEnTerima)alat.formSudahDibuat(typeof(fEnTerima))) == null)
            //{
            //    terima = new fEnTerima();
            //    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
            //    //fillTheSearchTable(lvTampil);
            //    terima.idOpp = Convert.ToInt16(txtkd_Pos.Text);
            //    terima.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
            //    terima.StartPosition = FormStartPosition.CenterScreen;
            //    terima.MdiParent = this;
            //    terima.Show();
            //}
            //else
            //{
            //    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
            //    //fillTheSearchTable(lvTampil);
            //    terima.idOpp = Convert.ToInt16(txtkd_Pos.Text);
            //    terima.Select();
            //}
            MessageBox.Show("MENU INI TIDAK DIGUNAKAN LAGI\nTERIMA KASIH", "DEPRECATED CODE");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            statusDateTime.Text = DateTime.Now.ToLocalTime().ToShortDateString()+ 
                " .:|:. " + DateTime.Now.ToLocalTime().ToLongTimeString();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                koneksi.Open();
            }
            catch (Exception ex)
            {
                eksepsi = ex.Message;
                status = 0;
            }
            finally
            {
                disabler();
                koneksi.Close();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (status == 0)
            {
                if (MessageBox.Show("TELAH TERJADI KESALAHAN DENGAN PESAN:\n\n" + eksepsi + "\n\nSILAHKAN HUBUNGI" +
                    " TEKNISI ANDA", "PERINGATAN", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    statusUser.Text = " : CLIENT GAGAL MELAKUKAN KONEKSI DENGAN SERVER!!!!!!!!!!!!!!!!!!!";
                    //fSetConn setConn = new fSetConn();
                    //setConn.StartPosition = FormStartPosition.CenterScreen;
                    //setConn.ShowDialog();
                    //setConn.Show();
                    if ((dekripKoneksi = (decrypt)alat.FormSudahDibuat(typeof(decrypt))) == null)
                    {
                        dekripKoneksi = new decrypt();
                        //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                        //fillTheSearchTable(lvTampil);
                        //setConn.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                        dekripKoneksi.StartPosition = FormStartPosition.CenterScreen;
                        dekripKoneksi.MdiParent = this;
                        dekripKoneksi.Show();
                    }
                    else
                    {
                        //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                        //fillTheSearchTable(lvTampil);
                        dekripKoneksi.Select();
                    }
                }
                else
                    this.Close();

            }
            else if (status == 1)
            {
                statusUser.Text = "User Aktip : .:|:. Posisi : ";
                toolStripStatusServer.Text = " Server : " + konek.GetServer();
                showLogin();
            }
        }

        private void mBayar_Click(object sender, EventArgs e)
        {
            if ((bayar = (fEnFront)alat.FormSudahDibuat(typeof(fEnFront))) == null)
            {
                bayar = new fEnFront();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                bayar.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                bayar.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                bayar.StartPosition = FormStartPosition.CenterScreen;
                bayar.MdiParent = this;
                bayar.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                bayar.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                bayar.Select();
            }
        }

        private void mSetAkses_Click(object sender, EventArgs e)
        {
            if ((rolePlaying = (fRole)alat.FormSudahDibuat(typeof(fRole))) == null)
            {
                rolePlaying = new fRole();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                rolePlaying.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                rolePlaying.StartPosition = FormStartPosition.CenterScreen;
                rolePlaying.MdiParent = this;
                rolePlaying.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                rolePlaying.Select();
            }
        }

        private void mSetKoneksi_Click(object sender, EventArgs e)
        {
            //if ((setConn = (fSetConn)alat.formSudahDibuat(typeof(fSetConn))) == null)
            //{
            //    setConn = new fSetConn();
            //    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
            //    //fillTheSearchTable(lvTampil);
            //    //setConn.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
            //    setConn.StartPosition = FormStartPosition.CenterScreen;
            //    setConn.MdiParent = this;
            //    setConn.Show();
            //}
            //else
            //{
            //    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
            //    //fillTheSearchTable(lvTampil);
            //    setConn.Select();
            //}
            if ((dekripKoneksi = (decrypt)alat.FormSudahDibuat(typeof(decrypt))) == null)
            {
                dekripKoneksi = new decrypt();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //setConn.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                dekripKoneksi.StartPosition = FormStartPosition.CenterScreen;
                dekripKoneksi.MdiParent = this;
                dekripKoneksi.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                dekripKoneksi.Select();
            }
        }

        private void mMAnggar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("MENU INI MASIH DIPERDEBATKAN, LANJUTKAN???", "KONFIRMASI", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                if ((anggar = (fMAnggaran)alat.FormSudahDibuat(typeof(fMAnggaran))) == null)
                {
                    anggar = new fMAnggaran();
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    anggar.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                    anggar.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                    anggar.StartPosition = FormStartPosition.CenterScreen;
                    anggar.MdiParent = this;
                    anggar.Show();
                }
                else
                {
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    anggar.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                    anggar.Select();
                }
            }
        }

        private void mMOpp_Click(object sender, EventArgs e)
        {
            if ((Opp = (fMOpp)alat.FormSudahDibuat(typeof(fMOpp))) == null)
            {
                Opp = new fMOpp();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                Opp.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                Opp.StartPosition = FormStartPosition.CenterScreen;
                Opp.MdiParent = this;
                Opp.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                Opp.Select();
            }
        }

        private void mMKpa_Click(object sender, EventArgs e)
        {
            if ((kpa = (fMKpa)alat.FormSudahDibuat(typeof(fMKpa))) == null)
            {
                kpa = new fMKpa();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                kpa.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                kpa.StartPosition = FormStartPosition.CenterScreen;
                kpa.MdiParent = this;
                kpa.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                kpa.Select();
            }

        }

        private void mMJenis_Click(object sender, EventArgs e)
        {
            if ((jenis = (fMJenis)alat.FormSudahDibuat(typeof(fMJenis))) == null)
            {
                jenis = new fMJenis();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                jenis.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                jenis.StartPosition = FormStartPosition.CenterScreen;
                jenis.MdiParent = this;
                jenis.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                jenis.Select();
            }
        }

        private void mMSupp_Click(object sender, EventArgs e)
        {
            if ((supplier = (fMSupp)alat.FormSudahDibuat(typeof(fMSupp))) == null)
            {
                supplier = new fMSupp();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                supplier.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                supplier.StartPosition = FormStartPosition.CenterScreen;
                supplier.MdiParent = this;
                supplier.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                supplier.Select();
            }
        }

        private void mMRek_Click(object sender, EventArgs e)
        {
            if ((rekening = (Revisi.masterRekening)alat.FormSudahDibuat(typeof(Revisi.masterRekening))) == null)
            {
                rekening = new Revisi.masterRekening();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //rekening.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                rekening.StartPosition = FormStartPosition.CenterScreen;
                rekening.MdiParent = this;
                rekening.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                rekening.Select();
            }
        }

        private void mKasir_Click(object sender, EventArgs e)
        {
            if ((kasir = (fLunas)alat.FormSudahDibuat(typeof(fLunas))) == null)
            {
                kasir = new fLunas();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                kasir.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                kasir.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                kasir.StartPosition = FormStartPosition.CenterScreen;
                kasir.MdiParent = this;
                kasir.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                kasir.Select();
            }
        }

        private void mLapBayar_Click(object sender, EventArgs e)
        {
            //if ((lapKeluar = (fLAP_KELUAR)alat.formSudahDibuat(typeof(fLAP_KELUAR))) == null)
            //{
            //    lapKeluar = new fLAP_KELUAR();
            //    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
            //    //fillTheSearchTable(lvTampil);
            //    lapKeluar.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
            //    lapKeluar.StartPosition = FormStartPosition.CenterScreen;
            //    lapKeluar.MdiParent = this;
            //    lapKeluar.Show();
            //}
            //else
            //{
            //    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
            //    //fillTheSearchTable(lvTampil);
            //    lapKeluar.Select();
            //}
            if ((cekLapEvi = (cetak.fCetakLapEvi)alat.FormSudahDibuat(typeof(cetak.fCetakLapEvi))) == null)
            {
                cekLapEvi = new cetak.fCetakLapEvi();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //cekLapEvi.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                cekLapEvi.StartPosition = FormStartPosition.CenterScreen;
                cekLapEvi.MdiParent = this;
                cekLapEvi.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                cekLapEvi.Select();
            }
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((settingFont = (fSetFont)alat.FormSudahDibuat(typeof(fSetFont))) == null)
            {
                settingFont = new fSetFont();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //settingFont.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                settingFont.StartPosition = FormStartPosition.CenterScreen;
                settingFont.MdiParent = this;
                settingFont.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                settingFont.Select();
            }
        }

        private void fUtama_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("APAKAH AKAN KELUAR DARI APLIKASI REALANGGARAN ver." +
                Application.ProductVersion.ToString(), "KONFIRMASI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
            else
                e.Cancel = false;                
        }

        private void mLinkKasdanya_Click(object sender, EventArgs e)
        {
            if ((kasdanya = (Revisi.fKasir)alat.FormSudahDibuat(typeof(Revisi.fKasir))) == null)
            {
                kasdanya = new Revisi.fKasir();
                kasdanya.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                kasdanya.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                kasdanya.StartPosition = FormStartPosition.CenterScreen;
                kasdanya.MdiParent = this;
                kasdanya.Show();
            }
            else
            {
                kasdanya.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                kasdanya.Select();
            }
        }

        private void mPembayaranKhusus_Click(object sender, EventArgs e)
        {
            if ((subsidiNYA_COK = (Revisi.fSubsidi)alat.FormSudahDibuat(typeof(Revisi.fSubsidi))) == null)
            {
                subsidiNYA_COK = new Revisi.fSubsidi();
                subsidiNYA_COK.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                subsidiNYA_COK.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                subsidiNYA_COK.StartPosition = FormStartPosition.CenterScreen;
                subsidiNYA_COK.MdiParent = this;
                subsidiNYA_COK.Show();
            }
            else
            {
                subsidiNYA_COK.Select();
            }
        }

        private void cekOverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((cekAnggaran = (Revisi.fCekAnggaran)alat.FormSudahDibuat(typeof(Revisi.fCekAnggaran))) == null)
            {
                cekAnggaran = new Revisi.fCekAnggaran();
                cekAnggaran.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                cekAnggaran.StartPosition = FormStartPosition.CenterScreen;
                cekAnggaran.MdiParent = this;
                cekAnggaran.Show();
            }
            else
            {
                cekAnggaran.Select();
            }
        }

        private void noSPKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            misc_tool.fGenerator keygen = new misc_tool.fGenerator();
            keygen.StartPosition = FormStartPosition.CenterScreen;
            keygen.idOpp = Convert.ToInt16(txtkd_Pos.Text);
            keygen.TopMost = true;
            keygen.Show();
            misc_tool.fCekSaldo CekSaldo = new misc_tool.fCekSaldo();
            CekSaldo.Location = new Point(keygen.Location.X, keygen.Location.Y - 300);
            CekSaldo.StartPosition = FormStartPosition.Manual;
            CekSaldo.TopMost = true;
            CekSaldo.Show();
            this.Hide();
        }

        private void tsmiExportExcel_Click(object sender, EventArgs e)
        {
            if ((exportExcel = (cetak.FCetakTransaksi)alat.FormSudahDibuat(typeof(cetak.FCetakTransaksi))) == null)
            {
                exportExcel = new cetak.FCetakTransaksi();
                exportExcel.StartPosition = FormStartPosition.CenterScreen;
                exportExcel.MdiParent = this;
                exportExcel.Show();
            }
            else
            {
                exportExcel.Select();
            }
        }

        private void mcekSisaPPTK_Click(object sender, EventArgs e)
        {
            if ((cekSaldoPptk = (misc_tool.fCekSaldoPPTK)alat.FormSudahDibuat(typeof(misc_tool.fCekSaldoPPTK))) == null)
            {
                cekSaldoPptk = new misc_tool.fCekSaldoPPTK();
                cekSaldoPptk.StartPosition = FormStartPosition.CenterScreen;
                cekSaldoPptk.MdiParent = this;
                cekSaldoPptk.Show();
            }
            else
            {
                cekSaldoPptk.Select();
            }
        }

        private void penyesuaianSPKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((mundurTanggal = (Revisi.FMundurTanggal)alat.FormSudahDibuat(typeof(Revisi.FMundurTanggal))) == null)
            {
                mundurTanggal = new Revisi.FMundurTanggal();
                mundurTanggal.StartPosition = FormStartPosition.CenterScreen;
                mundurTanggal.MdiParent = this;
                mundurTanggal.Show();
            }
            else
            {
                mundurTanggal.Select();
            }
        }

    }
}
