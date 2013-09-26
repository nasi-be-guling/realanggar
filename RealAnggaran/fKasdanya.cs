using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace RealAnggaran
{
    public partial class fKasdanya : Form
    {
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        SqlConnection koneksi = null;
        SqlConnection koneksinya = null;
        string query = null;
        SqlDataReader pembaca = null;
        public int idOpp = 0;

        public fKasdanya()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
            koneksinya = this.konekDBnya();
            dtPick1.CustomFormat = "dd/MM/yyyy";
            dtPick1.Format = DateTimePickerFormat.Custom;
        }

        private class settingList
        {
            public string iniFile { set; get; }
        }

        private SqlConnection konekDBnya()
        {
            SqlConnection koneksiAktip;
            string dbName;
            string instance;
            string port;
            string username;
            string pass;

            TextReader tR = new StreamReader("kasdanya.ini");
            string reader = null;

            List<settingList> grupSetting = new List<settingList>();
            while ((reader = tR.ReadLine()) != null)
            {
                settingList itemSettingList = new settingList();
                itemSettingList.iniFile = reader.ToString();
                grupSetting.Add(itemSettingList);
            }
            dbName = grupSetting[0].iniFile.ToString();
            instance = grupSetting[1].iniFile.ToString();
            port = grupSetting[2].iniFile.ToString();
            username = grupSetting[3].iniFile.ToString();
            pass = grupSetting[4].iniFile.ToString();
            tR.Close();

            string koneksi = @"Initial Catalog=REALANGGAR;" +
                @"Data Source=" + instance + "," + port + ";" +
                @"User ID='" + username + "';" +
                @"Password='" + pass + "'";

            koneksiAktip = new SqlConnection(koneksi);

            return koneksiAktip;
        }



    }
}
