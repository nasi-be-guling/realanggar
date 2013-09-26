using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RealAnggaran
{
    public partial class fSetFont : Form
    {
        CAlat alat = new CAlat();
        public fSetFont()
        {
            InitializeComponent();
        }

        private void fSetFont_Load(object sender, EventArgs e)
        {
            Font hasilDeserial = deSerialkanFont("fontSettings.dat", label1.Font);
            label1.Font = hasilDeserial;
            cmbFont.SelectedItem = hasilDeserial.FontFamily;
            cmbUkur.Text = hasilDeserial.Size.ToString();
            UpdateFontStyles();
            //selectAllControl();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            serialkanFont("fontSettings.dat", label1.Font);
            this.Close();
        }

        #region updateIsiKombo -- mengupdate isi dari kombobox jenis font
        private void UpdateFontStyles()
        {
            //Update the styles combo box
            FontFamily ff = (FontFamily)cmbFont.SelectedItem;
            cmbJenis.Items.Clear();
            if (ff.IsStyleAvailable(FontStyle.Regular))
            {
                cmbJenis.Items.Add(FontStyle.Regular);
            }
            if (ff.IsStyleAvailable(FontStyle.Bold))
            {
                cmbJenis.Items.Add(FontStyle.Bold);
            }
            if (ff.IsStyleAvailable(FontStyle.Italic))
            {
                cmbJenis.Items.Add(FontStyle.Italic);
            }
            if (ff.IsStyleAvailable(FontStyle.Bold | FontStyle.Italic))
            {
                cmbJenis.Items.Add((FontStyle.Bold | FontStyle.Italic));
            }
            if (cmbJenis.Items.Contains(label1.Font))
            {
                cmbJenis.Text = label1.Font.ToString();
            }
            else
            {
                //Use first available
                cmbJenis.Text = cmbJenis.Items[0].ToString();
            }
        }
        #endregion

        private void cmbJenis_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cmbFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFontStyles();
        }

        #region Mencoba teknik SERIALISASI & DESERIALISASI guna menyimpan format font
        // http://www.switchonthecode.com/tutorials/csharp-tutorial-serialize-objects-to-a-file
        // http://www.codeproject.com/KB/cs/serializedeserialize.aspx
        // http://www.java2s.com/Code/CSharp/File-Stream/Deserialize.htm

        private void serialkanFont(string namaFile, Font fontnya)
        {
            Stream fileTampung = File.Open(namaFile, FileMode.Create);
            BinaryFormatter formaterFile = new BinaryFormatter();
            formaterFile.Serialize(fileTampung, fontnya);
            fileTampung.Close();
        }

        private Font deSerialkanFont (string namaFile, Font fontSekarang)
        {
            Font settingFont = fontSekarang;
            Stream fileTampung = File.Open(namaFile, FileMode.Open);
            BinaryFormatter formaterFile = new BinaryFormatter();
            settingFont = (Font)formaterFile.Deserialize(fileTampung);
            fileTampung.Close();
            return settingFont;
        }
        #endregion

        private void bPreview_Click(object sender, EventArgs e)
        {
            float ukuranFont = (float)Convert.ToInt16(cmbUkur.SelectedItem.ToString());
            label1.Font = new Font((FontFamily)cmbFont.SelectedItem, ukuranFont, (FontStyle)cmbJenis.SelectedItem);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
