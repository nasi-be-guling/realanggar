using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using RealAnggaran.misc_tool;

namespace RealAnggaran
{
    public partial class fUtama : Form
    {
        //private int childFormNumber = 0;
        readonly CKonek _konek = new CKonek();
        readonly CAlat _alat = new CAlat();
        readonly SqlConnection _koneksi;
        SqlDataReader _pembaca = null;
        string _query = null;
        int _status = 1;
        string _eksepsi = null;
        readonly int _lebarLayar = Screen.PrimaryScreen.WorkingArea.Size.Width - 10;
        readonly int _tinggiLayar = Screen.PrimaryScreen.WorkingArea.Size.Height - 100;
        
        // LIST OF FORM
        fEnFront _bayar;
        fMAnggaran _anggar;
        fMJenis _jenis;
        fMKpa _kpa;
        fMOpp _opp;
        Revisi.masterRekening _rekening;
        fMSupp _supplier;
        fRole _rolePlaying;
        fLunas _kasir;
        fSetFont _settingFont;
        //fKasdanya kasdanya;
        private Revisi.fKasir _kasdanya;
        private Revisi.fSubsidi _subsidiNyaCok;
        private Revisi.fCekAnggaran _cekAnggaran;
        private cetak.FCetakTransaksi _exportExcel;
        private misc_tool.fCekSaldoPPTK _cekSaldoPptk;
        private cetak.fCetakLapEvi _cekLapEvi;
        private decrypt _dekripKoneksi;
        private FPak _importFPak;
        private Revisi.FMundurTanggal _mundurTanggal;

        public fUtama()
        {
            InitializeComponent();
            _koneksi = _konek.KonekDb();
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
            _alat.WriteFileVersion();
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
            _query = "SELECT d.nama_menu, a.Nama_Opp, b.Nama_Role FROM A_OPP a, A_ROLE b, A_D_ROLE c, A_MENU" +
                " d WHERE ((a.id_opp = '" + id + "' AND b.id_Role = a.id_Role) AND (c.id_role = b.id_role" +
                " AND d.id_menu = c.id_menu))";
            _koneksi.Open();
            //DataTable dTabel = konek.dTabel(query, koneksi);
            _pembaca = _konek.MembacaData(_query, _koneksi);
            //DataRow dRow = null;
            //if (dTabel.Rows.Count > 0)
            //{
            //    DataTableReader dPembaca = dTabel.CreateDataReader();
            //    dPembaca.Read();
            //    for (int i = 0; i <= dTabel.Rows.Count; i++)
            //    {
            if (_pembaca.HasRows)
            {
                while (_pembaca.Read())
                {
                    foreach (ToolStripItem item in fileMenu.DropDownItems)
                    {
                        if (item.Name == _alat.PengecekField(_pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in transToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == _alat.PengecekField(_pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in masterToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == _alat.PengecekField(_pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in transToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == _alat.PengecekField(_pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in optionsToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == _alat.PengecekField(_pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in mLaporan.DropDownItems)
                    {
                        if (item.Name == _alat.PengecekField(_pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    foreach (ToolStripItem item in keyGenToolStripMenuItem.DropDownItems)
                    {
                        if (item.Name == _alat.PengecekField(_pembaca, 0))
                        {
                            item.Enabled = true;
                        }
                    }
                    statusUser.Text = "User Aktip : " + _alat.PengecekField(_pembaca, 1) + " .:|:. Posisi : " +
                        _alat.PengecekField(_pembaca, 2);
                }
                _pembaca.Close();
            }
                    //dRow = dTabel.Rows.;
            //    }
            //    MessageBox.Show(dPembaca[0].ToString());
            //}
            _koneksi.Close();
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
                _koneksi.Open();
            }
            catch (Exception ex)
            {
                _eksepsi = ex.Message;
                _status = 0;
            }
            finally
            {
                disabler();
                _koneksi.Close();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_status == 0)
            {
                if (MessageBox.Show("TELAH TERJADI KESALAHAN DENGAN PESAN:\n\n" + _eksepsi + "\n\nSILAHKAN HUBUNGI" +
                    " TEKNISI ANDA", "PERINGATAN", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    statusUser.Text = " : CLIENT GAGAL MELAKUKAN KONEKSI DENGAN SERVER!!!!!!!!!!!!!!!!!!!";
                    //fSetConn setConn = new fSetConn();
                    //setConn.StartPosition = FormStartPosition.CenterScreen;
                    //setConn.ShowDialog();
                    //setConn.Show();
                    if ((_dekripKoneksi = (decrypt)_alat.FormSudahDibuat(typeof(decrypt))) == null)
                    {
                        _dekripKoneksi = new decrypt();
                        //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                        //fillTheSearchTable(lvTampil);
                        //setConn.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                        _dekripKoneksi.StartPosition = FormStartPosition.CenterScreen;
                        _dekripKoneksi.MdiParent = this;
                        _dekripKoneksi.Show();
                    }
                    else
                    {
                        //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                        //fillTheSearchTable(lvTampil);
                        _dekripKoneksi.Select();
                    }
                }
                else
                    this.Close();

            }
            else if (_status == 1)
            {
                statusUser.Text = "User Aktip : .:|:. Posisi : ";
                toolStripStatusServer.Text = " Server : " + _konek.GetServer();
                showLogin();
            }
        }

        private void mBayar_Click(object sender, EventArgs e)
        {
            if ((_bayar = (fEnFront)_alat.FormSudahDibuat(typeof(fEnFront))) == null)
            {
                _bayar = new fEnFront();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _bayar.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                _bayar.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _bayar.StartPosition = FormStartPosition.CenterScreen;
                _bayar.MdiParent = this;
                _bayar.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _bayar.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                _bayar.Select();
            }
        }

        private void mSetAkses_Click(object sender, EventArgs e)
        {
            if ((_rolePlaying = (fRole)_alat.FormSudahDibuat(typeof(fRole))) == null)
            {
                _rolePlaying = new fRole();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _rolePlaying.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _rolePlaying.StartPosition = FormStartPosition.CenterScreen;
                _rolePlaying.MdiParent = this;
                _rolePlaying.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _rolePlaying.Select();
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
            if ((_dekripKoneksi = (decrypt)_alat.FormSudahDibuat(typeof(decrypt))) == null)
            {
                _dekripKoneksi = new decrypt();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //setConn.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                _dekripKoneksi.StartPosition = FormStartPosition.CenterScreen;
                _dekripKoneksi.MdiParent = this;
                _dekripKoneksi.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _dekripKoneksi.Select();
            }
        }

        private void mMAnggar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("MENU INI MASIH DIPERDEBATKAN, LANJUTKAN???", "KONFIRMASI", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                if ((_anggar = (fMAnggaran)_alat.FormSudahDibuat(typeof(fMAnggaran))) == null)
                {
                    _anggar = new fMAnggaran();
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    _anggar.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                    _anggar.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                    _anggar.StartPosition = FormStartPosition.CenterScreen;
                    _anggar.MdiParent = this;
                    _anggar.Show();
                }
                else
                {
                    //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                    //fillTheSearchTable(lvTampil);
                    _anggar.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                    _anggar.Select();
                }
            }
        }

        private void mMOpp_Click(object sender, EventArgs e)
        {
            if ((_opp = (fMOpp)_alat.FormSudahDibuat(typeof(fMOpp))) == null)
            {
                _opp = new fMOpp();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _opp.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _opp.StartPosition = FormStartPosition.CenterScreen;
                _opp.MdiParent = this;
                _opp.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _opp.Select();
            }
        }

        private void mMKpa_Click(object sender, EventArgs e)
        {
            if ((_kpa = (fMKpa)_alat.FormSudahDibuat(typeof(fMKpa))) == null)
            {
                _kpa = new fMKpa();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _kpa.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _kpa.StartPosition = FormStartPosition.CenterScreen;
                _kpa.MdiParent = this;
                _kpa.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _kpa.Select();
            }

        }

        private void mMJenis_Click(object sender, EventArgs e)
        {
            if ((_jenis = (fMJenis)_alat.FormSudahDibuat(typeof(fMJenis))) == null)
            {
                _jenis = new fMJenis();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _jenis.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _jenis.StartPosition = FormStartPosition.CenterScreen;
                _jenis.MdiParent = this;
                _jenis.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _jenis.Select();
            }
        }

        private void mMSupp_Click(object sender, EventArgs e)
        {
            if ((_supplier = (fMSupp)_alat.FormSudahDibuat(typeof(fMSupp))) == null)
            {
                _supplier = new fMSupp();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _supplier.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _supplier.StartPosition = FormStartPosition.CenterScreen;
                _supplier.MdiParent = this;
                _supplier.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _supplier.Select();
            }
        }

        private void mMRek_Click(object sender, EventArgs e)
        {
            if ((_rekening = (Revisi.masterRekening)_alat.FormSudahDibuat(typeof(Revisi.masterRekening))) == null)
            {
                _rekening = new Revisi.masterRekening();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //rekening.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                _rekening.StartPosition = FormStartPosition.CenterScreen;
                _rekening.MdiParent = this;
                _rekening.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _rekening.Select();
            }
        }

        private void mKasir_Click(object sender, EventArgs e)
        {
            if ((_kasir = (fLunas)_alat.FormSudahDibuat(typeof(fLunas))) == null)
            {
                _kasir = new fLunas();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _kasir.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                _kasir.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _kasir.StartPosition = FormStartPosition.CenterScreen;
                _kasir.MdiParent = this;
                _kasir.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _kasir.Select();
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
            if ((_cekLapEvi = (cetak.fCetakLapEvi)_alat.FormSudahDibuat(typeof(cetak.fCetakLapEvi))) == null)
            {
                _cekLapEvi = new cetak.fCetakLapEvi();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //cekLapEvi.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                _cekLapEvi.StartPosition = FormStartPosition.CenterScreen;
                _cekLapEvi.MdiParent = this;
                _cekLapEvi.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _cekLapEvi.Select();
            }
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((_settingFont = (fSetFont)_alat.FormSudahDibuat(typeof(fSetFont))) == null)
            {
                _settingFont = new fSetFont();
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                //settingFont.Size = new System.Drawing.Size(lebarLayar, tinggiLayar);
                _settingFont.StartPosition = FormStartPosition.CenterScreen;
                _settingFont.MdiParent = this;
                _settingFont.Show();
            }
            else
            {
                //System.Windows.Forms.ListView lvTampil = (System.Windows.Forms.ListView)cari.Controls["groupBox1"].Controls["lvTampil"];
                //fillTheSearchTable(lvTampil);
                _settingFont.Select();
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
            if ((_kasdanya = (Revisi.fKasir)_alat.FormSudahDibuat(typeof(Revisi.fKasir))) == null)
            {
                _kasdanya = new Revisi.fKasir();
                _kasdanya.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                _kasdanya.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _kasdanya.StartPosition = FormStartPosition.CenterScreen;
                _kasdanya.MdiParent = this;
                _kasdanya.Show();
            }
            else
            {
                _kasdanya.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                _kasdanya.Select();
            }
        }

        private void mPembayaranKhusus_Click(object sender, EventArgs e)
        {
            if ((_subsidiNyaCok = (Revisi.fSubsidi)_alat.FormSudahDibuat(typeof(Revisi.fSubsidi))) == null)
            {
                _subsidiNyaCok = new Revisi.fSubsidi();
                _subsidiNyaCok.idOpp = Convert.ToInt16(txtkd_Pos.Text);
                _subsidiNyaCok.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _subsidiNyaCok.StartPosition = FormStartPosition.CenterScreen;
                _subsidiNyaCok.MdiParent = this;
                _subsidiNyaCok.Show();
            }
            else
            {
                _subsidiNyaCok.Select();
            }
        }

        private void cekOverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((_cekAnggaran = (Revisi.fCekAnggaran)_alat.FormSudahDibuat(typeof(Revisi.fCekAnggaran))) == null)
            {
                _cekAnggaran = new Revisi.fCekAnggaran();
                _cekAnggaran.Size = new System.Drawing.Size(_lebarLayar, _tinggiLayar);
                _cekAnggaran.StartPosition = FormStartPosition.CenterScreen;
                _cekAnggaran.MdiParent = this;
                _cekAnggaran.Show();
            }
            else
            {
                _cekAnggaran.Select();
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
            if ((_exportExcel = (cetak.FCetakTransaksi)_alat.FormSudahDibuat(typeof(cetak.FCetakTransaksi))) == null)
            {
                _exportExcel = new cetak.FCetakTransaksi();
                _exportExcel.StartPosition = FormStartPosition.CenterScreen;
                _exportExcel.MdiParent = this;
                _exportExcel.Show();
            }
            else
            {
                _exportExcel.Select();
            }
        }

        private void mcekSisaPPTK_Click(object sender, EventArgs e)
        {
            if ((_cekSaldoPptk = (misc_tool.fCekSaldoPPTK)_alat.FormSudahDibuat(typeof(misc_tool.fCekSaldoPPTK))) == null)
            {
                _cekSaldoPptk = new misc_tool.fCekSaldoPPTK();
                _cekSaldoPptk.StartPosition = FormStartPosition.CenterScreen;
                _cekSaldoPptk.MdiParent = this;
                _cekSaldoPptk.Show();
            }
            else
            {
                _cekSaldoPptk.Select();
            }
        }

        private void penyesuaianSPKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((_mundurTanggal = (Revisi.FMundurTanggal)_alat.FormSudahDibuat(typeof(Revisi.FMundurTanggal))) == null)
            {
                _mundurTanggal = new Revisi.FMundurTanggal();
                _mundurTanggal.StartPosition = FormStartPosition.CenterScreen;
                _mundurTanggal.MdiParent = this;
                _mundurTanggal.Show();
            }
            else
            {
                _mundurTanggal.Select();
            }
        }

        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void importPAKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((_importFPak = (FPak)_alat.FormSudahDibuat(typeof(FPak))) == null)
            {
                _importFPak = new FPak {StartPosition = FormStartPosition.CenterScreen, MdiParent = this};
                _importFPak.Show();
            }
            else
            {
                _mundurTanggal.Select();
            }
        }

    }
}
