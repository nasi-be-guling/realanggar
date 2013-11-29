using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Data;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using RealAnggaran.Properties;

namespace RealAnggaran
{
    class CAlat
    {
        // Jika tidak menggunakan constructor
        // hapus pada baris yg di comment
        // ====================================
        // | Alt + Insert untuk meng-generate |
        // | construktor dari class yg dibuat |
        // ====================================
        public string PengecekField(SqlDataReader hasilPembaca, byte kolom)
        { 
            string hasil = "";
            if (hasilPembaca.IsDBNull(kolom))
            {
                if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Byte")
                    hasil = "0";
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Boolean")
                    hasil = "false";
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Int32")
                    hasil = "0";
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Int16")
                    hasil = "0";
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.String")
                    hasil = "";
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Decimal")
                    hasil = "0";
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Int64")
                    hasil = "0";
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.DateTime")
                    hasil = "0";
                else
                {
                    MessageBox.Show(Resources.CAlat_PengecekField_Tipe_data_tidak_di_kenali, Resources.CAlat_PengecekField_Infomasi, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }    
            }
            else
            {
                if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Byte")
                    hasil = hasilPembaca.GetByte(kolom).ToString().Trim();
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Boolean")
                    hasil = hasilPembaca.GetBoolean(kolom).ToString().Trim();
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Int32")
                    hasil = hasilPembaca.GetInt32(kolom).ToString().Trim();
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Int16")
                    hasil = hasilPembaca.GetInt16(kolom).ToString().Trim();
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.String")
                    hasil = hasilPembaca.GetString(kolom).ToString().Trim();
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Decimal")
                    hasil = hasilPembaca.GetDecimal(kolom).ToString().Trim();
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.Int64")
                    hasil = hasilPembaca.GetInt64(kolom).ToString().Trim();
                else if (hasilPembaca.GetFieldType(kolom).ToString().Trim() == "System.DateTime")
                    hasil = string.Format("{0:MM/dd/yyyy}", hasilPembaca.GetDateTime(kolom));
                else
                {
                    MessageBox.Show(Resources.CAlat_PengecekField_Tipe_data_tidak_di_kenali, Resources.CAlat_PengecekField_Infomasi, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }

            return hasil;
        }

        public string FilterMBox(string txtbox)
        {
            string test2 = txtbox;
            test2 = test2.Replace(",", ".");
            test2 = test2.Replace(" .", "");
            test2 = test2.Replace(". ", "");
            return test2;
        }

        public Image OlahPoto(SqlConnection koneksiAktip, string query, string kode)
        {

            SqlDataAdapter sAdap = new SqlDataAdapter(query, koneksiAktip);

            DataTable dTabel = new DataTable();
            sAdap.Fill(dTabel);

            FileStream fStream = new FileStream("poto.jpg", FileMode.Create);

            foreach (DataRow dRow in dTabel.Rows)
            {
                if (dRow[0].ToString() == kode)
                {
                    byte[] blob = (byte[])dRow[3];
                    if (fStream != null)
                    {
                        fStream.Write(blob, 0, blob.Length);
                        fStream.Close();
                    }
                    fStream = null;
                }

            }
            Image poto = Image.FromFile("poto.jpg");

            return poto;
        }

        public void IsiCombo(ComboBox namaCombo, string query, SqlConnection koneksiAktip, int field)
        {
            try
            {

                SqlDataAdapter sAdap = new SqlDataAdapter {SelectCommand = new SqlCommand(query, koneksiAktip)};

                DataTable dTabel = new DataTable();

                sAdap.Fill(dTabel);

                namaCombo.Items.Clear();

                foreach (DataRow dRow in dTabel.Rows)
                {
                    namaCombo.Items.Add(dRow[field].ToString().Trim());
                    //namaCombo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                //if (ex ==
                MessageBox.Show(ex.Message);
            }
            //return null;
        }

        public void TampilFormCari(TextBox namatxt, string namaTabel, string pKey1, string pkey2, Form namaForm)
        {
            fCari dfCARI = new fCari();
            TextBox txtCari = (TextBox)dfCARI.Controls["groupBox1"].Controls["txtCari"];
            TextBox txtNamaTabel = (TextBox)dfCARI.Controls["txtNamaTabel"];
            TextBox txtPKey1 = (TextBox)dfCARI.Controls["txtPKey1"];
            TextBox txtPKey2 = (TextBox)dfCARI.Controls["txtPKey2"];
            TextBox txtNamaForm = (TextBox)dfCARI.Controls["txtNamaForm"];
            txtCari.Text = namatxt.Text;
            txtNamaTabel.Text = namaTabel;
            txtPKey1.Text = pKey1;
            txtPKey2.Text = pkey2;
            dfCARI.MdiParent = namaForm.MdiParent;
            txtNamaForm.Text = namaForm.Name;
            if (namaForm.Name != "fDetKorek")
            {
                dfCARI.WindowState = FormWindowState.Maximized;
                dfCARI.TopLevel = false;
                dfCARI.Dock = DockStyle.Fill;
                dfCARI.Parent = namaForm.Parent;
                dfCARI.BringToFront();
                dfCARI.TopMost = true;
            }
            dfCARI.Show();
            txtCari.Focus();

            // GET ON TOP PROPERTY !!!!
        }

        public Form FeedBack(string namaForm)
        {
            Form umpan = new Form();
            return umpan;
        }

        public Form FormSudahDibuat(Type tipeForm)
        {
            return Application.OpenForms.Cast<Form>().FirstOrDefault(formTerbuat => formTerbuat.GetType() == tipeForm);
        }

        public string DateFormater(string originalDate)
        {
            string formatedDate = originalDate.Substring(3, 2) + "/" + originalDate.Substring(0, 2) + "/" +
                originalDate.Substring(6, 4) + " " + DateTime.Now.ToLongTimeString();
            return formatedDate;
        }

        public void WriteToFile(string lokasiPenulisan, string teksYangDitulis)
        {
            using (System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(lokasiPenulisan, true))
            {
                fileWriter.WriteLine(teksYangDitulis);
                fileWriter.Close();
            }
        }

        public void WriteFileVersion()
        {
            WriteToFile(@"RealAnggaranVer.txt", "========================================");
            WriteToFile(@"RealAnggaranVer.txt", Application.ProductVersion);
            WriteToFile(@"RealAnggaranVer.txt", DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        public void CleaningService(Control namaKontrol)
        {
            foreach (Control txt in namaKontrol.Controls)
            {
                if ((txt.GetType().ToString() == "System.Windows.Forms.TextBox") |
                    (txt.GetType().ToString() == "System.Windows.Forms.ComboBox"))
                {
                    txt.Text = "";
                    if (txt.TabIndex == 0)
                        txt.Focus();
                }
                else if (txt.GetType().ToString() == "System.Windows.Forms.MaskedTextBox")
                {
                    txt.Text = @"0";
                }
            }
        }

        public void KontrolDesimal(Control namaKontrol)
        {
            foreach (Control txt in namaKontrol.Controls)
            {
                if ((txt.GetType().ToString() == "System.Windows.Forms.TextBox") |
                    (txt.GetType().ToString() == "System.Windows.Forms.ComboBox"))
                {
                    if (txt.Text == "")
                        txt.Text = @"0";
                }
                else if (txt.GetType().ToString() == "System.Windows.Forms.MaskedTextBox")
                {
                    if (txt.Text == "")
                        txt.Text = @"0";
                }

            }
        }

        #region THIS INCLUDE IMPLEMENTATION OF SERIALIZATION/DESERIALIZATION AND LIST<>/DATA COLLECTION OF CONTROL/SUB CONTROL 
        public void DeSerial(Form namaForm)
        {
            Font settingFont = new Font("Arial", 10, FontStyle.Bold);
            Stream fileTampung = File.Open("fontSettings.dat", FileMode.Open);
            BinaryFormatter formaterFile = new BinaryFormatter();
            settingFont = (Font)formaterFile.Deserialize(fileTampung);
            fileTampung.Close();
            //foreach (Control panel in namaForm.Controls)
            //{
            //    if (panel.GetType().ToString() == "System.Windows.Forms.TextBox")
            //    {
            //        MessageBox.Show(panel.Name.ToString());
            //        //foreach (Control textBox in panel.Controls)
            //        //{

            //        //    if (textBox.GetType().ToString() == "System.Windows.Forms.TextBox" |
            //        //        textBox.GetType().ToString() == "System.Windows.Forms.MaskedTextBox")
            //        //        textBox.Font = settingFont;
            //        //}
            //    }
            //}

            List<Control> lstcontrol = GetAllControls(namaForm.Controls);

            foreach (Control t in lstcontrol)
            {
//MessageBox.Show(lstcontrol[i].Text.ToString(), "");
                if (t.GetType().ToString() == "System.Windows.Forms.TextBox" |
                    t.GetType().ToString() == "System.Windows.Forms.DateTimePicker" |
                    t.GetType().ToString() == "System.Windows.Forms.MaskedTextBox")
                {
                    t.Font = settingFont;
                }
            }
        }

        private static List<Control> GetAllControls(IList ctrl)
        {
            List<Control> retCtrl = new List<Control>();
            foreach (Control ctl in ctrl)
            {
                retCtrl.Add(ctl);
                List<Control> subCtrl = GetAllControls(ctl.Controls);
                retCtrl.AddRange(subCtrl);
            }
            return retCtrl;
        }
        #endregion
        #region THIS IS FOR FILTERING INPUT INTO THE TEXTBOX -- the first way was sucks, the second was too great

        /* =============== THIS IS USED FOR LIMITING USER FROM COPY PASTE NON NUMERIC CHARACTER ================ *
             * 
             * protected void OnTextChanged(object sender, TextChangedEventArgs e)
                {
                    base.Text = LeaveOnlyNumbers(Text);
                }                
                private string LeaveOnlyNumbers(String inString)
                {
                    String tmp = inString;
                    foreach (char c in inString.ToCharArray())
                    {
                        if (!IsDigit(c))
                        {
                            tmp = tmp.Replace(c.ToString(), "");
                        }
                    }
                    return tmp;
                }                 
                public bool IsDigit(char c)
                {
                    return (c >= '0' || c <= '9');
                }
             * 
             *  ======================================= ANOTHER VERSION OF ABOVE ======================================== *
             * 
             *  private void txtType3_KeyDown(object sender, KeyEventArgs e)
                {
                    //Allow navigation keyboard arrows
                    switch (e.KeyCode)
                    {
                        case Keys.Up:
                        case Keys.Down:
                        case Keys.Left:
                        case Keys.Right:
                        case Keys.PageUp:
                        case Keys.PageDown:
                            e.SuppressKeyPress = false;
                            return;
                        default:
                            break;
                    }

                    //Block non-number characters
                    char currentKey = (char)e.KeyCode;
                    bool modifier = e.Control || e.Alt || e.Shift;
                    bool nonNumber = char.IsLetter(currentKey) || 
                                     char.IsSymbol(currentKey) || 
                                     char.IsWhiteSpace(currentKey) || 
                                     char.IsPunctuation(currentKey);

                    if (!modifier && nonNumber)
                        e.SuppressKeyPress = true;

                    //Handle pasted Text
                    if (e.Control && e.KeyCode == Keys.V)
                    {
                        //Preview paste data (removing non-number characters)
                        string pasteText = Clipboard.GetText();
                        string strippedText = "";
                        for (int i = 0; i < pasteText.Length; i++)
                        {
                            if (char.IsDigit(pasteText[i]))
                                strippedText += pasteText[i].ToString();
                        }

                        if (strippedText != pasteText)
                        {
                            //There were non-numbers in the pasted text
                            e.SuppressKeyPress = true;

                            //OPTIONAL: Manually insert text stripped of non-numbers
                            TextBox me = (TextBox)sender;
                            int start = me.SelectionStart;
                            string newTxt = me.Text;
                            newTxt = newTxt.Remove(me.SelectionStart, me.SelectionLength); //remove highlighted text
                            newTxt = newTxt.Insert(me.SelectionStart, strippedText); //paste
                            me.Text = newTxt;
                            me.SelectionStart = start + strippedText.Length;
                        }
                        else
                            e.SuppressKeyPress = false;
                    }
                }
             * 
             *  =================================== THIS IS USING REGEX ============================================ *
             *  
             private void txtType2_KeyPress(object sender, KeyPressEventArgs e)
             {
                 if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
                      e.Handled = true;
             }
             * 
             * =================================== THIS IS SIMPLEST WAY ============================================ *
             * 
             private void txtType1_KeyPress(object sender, KeyPressEventArgs e)
             {
                 int isNumber = 0;
                 e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
             }
             */
        // Im trying to use the first way
        //Allow navigation keyboard arrows
         
        public bool Validate(char key)
        {
            // Try converting the introduced char value to a byte value
            byte i;
            bool parsed = byte.TryParse(key.ToString(CultureInfo.InvariantCulture), out i);
            bool val = true;

            //If convresion succeeded, show allow character.
            if (parsed)
            {
                val = false;
            }
            // If the conversion did not succeed, check if the pressed
            // button was Backspace (char code is 8) or Delete (char code is 46).
            else
            {
                switch (Convert.ToInt32(key))
                {
                    case 8:
                        val = false;  // If Backspace, allow char.
                        break;
                    case 46:
                        val = false;  // If Delete, allow char.
                        break;
                }
            }
            return val;
        }
        public void FilterTextBox(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                case Keys.Enter:
                    e.SuppressKeyPress = false;
                    return;
            }

            //Block non-number characters
            char currentKey = (char)e.KeyCode;
            bool modifier = e.Control || e.Alt || e.Shift;
            bool nonNumber = char.IsLetter(currentKey) ||
                             char.IsSymbol(currentKey) ||
                             char.IsWhiteSpace(currentKey) ||
                             char.IsPunctuation(currentKey);

            if (!modifier && nonNumber)
                e.SuppressKeyPress = true;

            //Handle pasted Text
            if (e.Control && e.KeyCode == Keys.V)
            {
                //Preview paste data (removing non-number characters)
                string pasteText = Clipboard.GetText();
                string strippedText = pasteText.Where(char.IsDigit).Aggregate("", (current, t) => current + t.ToString(CultureInfo.InvariantCulture));

                if (strippedText != pasteText)
                {
                    //There were non-numbers in the pasted text
                    e.SuppressKeyPress = true;

                    //OPTIONAL: Manually insert text stripped of non-numbers
                    TextBox me = (TextBox)sender;
                    int start = me.SelectionStart;
                    string newTxt = me.Text;
                    newTxt = newTxt.Remove(me.SelectionStart, me.SelectionLength); //remove highlighted text
                    newTxt = newTxt.Insert(me.SelectionStart, strippedText); //paste
                    me.Text = string.Format(new CultureInfo("id-ID"), "{0:n}", newTxt);
                    me.SelectionStart = start + strippedText.Length;
                }
                else
                    e.SuppressKeyPress = false;
            }
        }
        #endregion
        public string Terbilang(double x)
        {
            string[] bilangan = {" ", "satu", "dua", "tiga", "empat", "lima",
                "enam", "tujuh", "delapan", "sembilan", "sepuluh",
                "sebelas"};
            string temp = "";

            if (x < 12)
            {
                temp = " " + bilangan[(int)x];
            }
            else if (x < 20)
            {
                temp = Terbilang(x - 10).ToString(CultureInfo.InvariantCulture) + " belas";
            }
            else if (x < 100)
            {
                temp = Terbilang(x / 10) + " puluh" + Terbilang(x % 10);
            }
            else if (x < 200)
            {
                temp = " seratus" + Terbilang(x - 100);
            }
            else if (x < 1000)
            {
                temp = Terbilang(x / 100) + " ratus" + Terbilang(x % 100);
            }
            else if (x < 2000)
            {
                temp = " seribu" + Terbilang(x - 1000);
            }
            else if (x < 1000000)
            {
                temp = Terbilang(x / 1000) + " ribu" + Terbilang(x % 1000);
            }
            else if (x < 1000000000)
            {
                temp = Terbilang(x / 1000000) + " juta" + Terbilang(x % 1000000);
            }
            else if (x < 1000000000000)
            {
                temp = Terbilang(x / 1000000000) + " milyar" + Terbilang(x % 1000000000);
            }
            else if (x < 1000000000000000)
            {
                temp = Terbilang(x / 1000000000000) + " trilyun" + Terbilang(x % 1000000000000);
            }

            return temp;
        }
        public string TerbilangKoma(string x)
        {
            string[] bilangan = {"nol", "satu", "dua", "tiga", "empat", "lima",
                "enam", "tujuh", "delapan", "sembilan", "sepuluh",
                "sebelas"};
            return x.Aggregate("", (current, t) => current + " " + bilangan[Convert.ToInt16(t.ToString(CultureInfo.InvariantCulture))]);
        }
        public Form PengecekForm(Type tipeForm)
        {
            return Application.OpenForms.Cast<Form>().FirstOrDefault(openForm => openForm.GetType() == tipeForm);
        }
        /// <summary>
        /// DESC :  this function is a copy-paste code from class c4module created by Eka Rudito
        /// FUNC :  to automatically resize column of an ListView            
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="intColsCount"></param>
        public void AutoresizeLv(ListView lv, int intColsCount)
        {
            //int ii = 0;
            //for (ii = 0; ii <= intColsCount - 1; ++ii)
            //    cSaveInvoke.SafeControlInvoke<ListView>(lv, (cSaveInvoke.SafeControlInvokeHandler<ListView>)(ListView => lv.AutoResizeColumn(ii, ColumnHeaderAutoResizeStyle.ColumnContent)));
            //for (ii = 0; ii <= intColsCount - 1; ++ii)
            //    cSaveInvoke.SafeControlInvoke<ListView>(lv, (cSaveInvoke.SafeControlInvokeHandler<ListView>)(ListView => lv.AutoResizeColumn(ii, ColumnHeaderAutoResizeStyle.HeaderSize)));
            int ii = 0;
            for (ii = 0; ii <= intColsCount - 1; ++ii)
                lv.AutoResizeColumn(ii, ColumnHeaderAutoResizeStyle.ColumnContent);
            for (ii = 0; ii <= intColsCount - 1; ++ii)
                lv.AutoResizeColumn(ii, ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        public string NullToString(object value)
        {
            // Value.ToString() allows for Value being DBNull, but will also convert int, double, etc.
            return value == null ? "" : value.ToString();

            // If this is not what you want then this form may suit you better, handles 'Null' and DBNull otherwise tries a straight cast
            // which will throw if Value isn't actually a string object.
            //return Value == null || Value == DBNull.Value ? "" : (string)Value;
        }
        public string NullToNumber(object value)
        {
            return value == null ? "0" : (string.IsNullOrEmpty(value.ToString().Trim()) ? "0" : value.ToString());
        }
    }
}
