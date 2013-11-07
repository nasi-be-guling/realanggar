using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Data.SqlClient;

namespace RealAnggaran
{
    public partial class decrypt : Form
    {
        CKonek _connect = new CKonek();

        private SqlConnection _connection;

        public decrypt()
        {
            InitializeComponent();
            _connection = _connect.KonekDb();
            _temp = Text;
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public static class StringCipher
        {
            // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
            // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
            // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.

            private const string InitVector = "tu89geji340t89u2";

            // This constant is used to determine the keysize of the encryption algorithm.
            private const int Keysize = 256;

            public static string Encrypt(string plainText, string passPhrase)
            {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
                byte[] keyBytes = password.GetBytes(Keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                return Convert.ToBase64String(cipherTextBytes);
            }

            public static string Decrypt(string cipherText, string passPhrase)
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
                byte[] keyBytes = password.GetBytes(Keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string koneksi = @"Initial Catalog=" + txtdbName.Text + ";" +
                @"Data Source=" + txtinstance.Text + "," + txtport.Text + ";" +
                @"User ID='" + txtusername.Text + "';" +
                @"Password='" + txtpass.Text + "'";
            string encrypht = StringCipher.Encrypt(koneksi, "123");

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["RealAnggaran.Properties.Settings.Setting"].ConnectionString = encrypht;
            config.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string conString = ConfigurationManager.ConnectionStrings["RealAnggaran.Properties.Settings.Setting"].ConnectionString;
            textBox1.Text = StringCipher.Decrypt(conString, "123");
        }

        private void decrypt_Load(object sender, EventArgs e)
        {
            //string conString = ConfigurationManager.ConnectionStrings["RealAnggaran.Properties.Settings.Setting"].ConnectionString;
            //textBox1.Text = StringCipher.Decrypt(conString, "123");
        }

        readonly string _temp;
        private void button3_Click(object sender, EventArgs e)
        {
            
            Text = @"Mengetes koneksi...........................";
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Terjadi kesalahan : " + ex.Message);
                return;
            }
            MessageBox.Show(@"KONEKSI SUKSES");
            _connection.Close();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Text = _temp;
        }
    }
}
