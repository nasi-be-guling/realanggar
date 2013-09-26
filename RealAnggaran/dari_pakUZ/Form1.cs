using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;

namespace RealAnggaran.dari_pakUZ
{
    public partial class Form1 : Form
    {
        /*
         * MASALAH YG DITEMUKAN IALAH TIDAK BISA MEMAKAI NAMA FORM atau class form1.cs secara dinamis, karena
         * nama class form yg dipanggil harus sesuai dengan nama ToolstripMenuItem
         * 
         * Solusi: dengan memakai atribute name dari ToolstripMenuItem sebagai dasar untuk memanggil class form Form1.cs
         * 
         * */

        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi = new SqlConnection();
        SqlDataReader reader = null;
        List<menuDinamis> grupMenu = new List<menuDinamis>();
        MenuStrip mainMenu = new MenuStrip();
        Form formDinamik = null;

        public Form1()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
        }

        class menuDinamis
        {
            public string idMenu { get; set; }
            public string menu { get; set; }
            public string idParentMenu { get; set; }
            public string KelasMenu { get; set; }
        }

        private void createMenuItem(ToolStripItem _menu)
        {
            ToolStripMenuItem @menu = (ToolStripMenuItem)_menu;

            //get the id parrent
            string idMenu = (from menus in grupMenu 
                             where menus.menu == @menu.Text 
                             select menus.idMenu).First();
            //get list from the parrent children
            var linqQuerryResult = (from menus in grupMenu 
                                    where menus.idParentMenu == idMenu 
                                    select menus.menu);

            if (linqQuerryResult.Count() > 0)
            {
                foreach (var itemMenu in linqQuerryResult)
                {
                    if (itemMenu == "-")
                        @menu.DropDownItems.Add(itemMenu); // add a separator
                    else
                        createMenuItem(@menu.DropDownItems.Add(itemMenu, null, new EventHandler(menuEvent)));
                }
            }
        }

        private void masukkanKeList()
        {
            koneksi.Open();
            reader = konek.MembacaData("SELECT * FROM tMenuDinamik", koneksi);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    menuDinamis menu = new menuDinamis();
                    menu.idMenu = alat.PengecekField(reader, 0);
                    menu.menu = alat.PengecekField(reader, 1);
                    menu.idParentMenu = alat.PengecekField(reader, 2);
                    menu.KelasMenu = alat.PengecekField(reader, 3);
                    grupMenu.Add(menu);
                }
                reader.Close();
            }
            koneksi.Close();
        }

        private void createMenuParent()
        {
            this.Controls.Add(mainMenu);

            var koleksiMenu = (from menus in grupMenu 
                               where string.IsNullOrEmpty(menus.idParentMenu) 
                               select menus);

            foreach (var setiapMenu in koleksiMenu)
            {
                createMenuItem(mainMenu.Items.Add(setiapMenu.menu));
            }
        }

        private void menuEvent(object sender, EventArgs e)
        {
            ToolStripMenuItem @menu = (ToolStripMenuItem)sender;
            Type typeForm = Type.GetType("RealAnggaran.dari_pakUZ." + @menu.Text);
            Form fFormLoad = alat.PengecekForm(typeForm);

            try
            {
                if (fFormLoad == null)
                {
                    if (typeForm != null)
                    {
                        ConstructorInfo ciInit = typeForm.GetConstructor(System.Type.EmptyTypes);
                        formDinamik = (Form)ciInit.Invoke(null);
                        formDinamik.MdiParent = this;
                        formDinamik.Icon = this.Icon;
                        formDinamik.StartPosition = FormStartPosition.CenterScreen;
                        formDinamik.Show();
                    }
                }
                else
                {
                    fFormLoad.Select();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            masukkanKeList();
            createMenuParent();
        }
    }
}
