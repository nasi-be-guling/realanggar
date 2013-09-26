using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RealAnggaran
{
    public partial class fSetConn : Form
    {
        CAlat alat = new CAlat();

        private class settingList
        {
            public string iniFile { set; get; }
        }

        private void fillTxt()
        {
            TextReader tR = new StreamReader("settings.ini");
            string reader = null;

            List<settingList> grupSetting = new List<settingList>();
            while ((reader = tR.ReadLine()) != null)
            {
                settingList itemSettingList = new settingList();
                itemSettingList.iniFile = reader.ToString();
                grupSetting.Add(itemSettingList);
            }
            txtNama.Text = grupSetting[0].iniFile.ToString();
            txtPort.Text = grupSetting[1].iniFile.ToString();
            txtUser.Text = grupSetting[2].iniFile.ToString();
            txtPass.Text = grupSetting[3].iniFile.ToString();
            tR.Close();
        }

        public fSetConn()
        {
            InitializeComponent();
        }

        private void bLogin_Click(object sender, EventArgs e)
        {
            TextWriter tW = new StreamWriter("settings.ini");
            tW.WriteLine(txtNama.Text);
            tW.WriteLine(txtPort.Text);
            tW.WriteLine(txtUser.Text);
            tW.WriteLine(txtPass.Text);
            tW.Close();

            fLogin login = new fLogin();
            login.StartPosition = FormStartPosition.CenterScreen;
            login.ShowDialog();
            login.Show();

            this.Close();
        }

        private void fSetConn_Load(object sender, EventArgs e)
        {
            alat.DeSerial(this);
            fillTxt();
        }

        private void bBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
