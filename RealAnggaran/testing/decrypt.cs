﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;

namespace akuntansi.testing
{
    public partial class decrypt : Form
    {
        public decrypt()
        {
            InitializeComponent();
        }

        public static class StringCipher
        {
            // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
            // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
            // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.

            private const string initVector = "tu89geji340t89u2";

            // This constant is used to determine the keysize of the encryption algorithm.
            private const int keysize = 256;

            public static string Encrypt(string plainText, string passPhrase)
            {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
                byte[] keyBytes = password.GetBytes(keysize / 8);
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
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
                byte[] keyBytes = password.GetBytes(keysize / 8);
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
            config.ConnectionStrings.ConnectionStrings["akuntansi.Properties.Settings.Setting"].ConnectionString = encrypht;
            config.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string conString = ConfigurationManager.ConnectionStrings["akuntansi.Properties.Settings.Setting"].ConnectionString;
            textBox1.Text = StringCipher.Decrypt(conString, "123");
        }

        private void decrypt_Load(object sender, EventArgs e)
        {
            string conString = ConfigurationManager.ConnectionStrings["akuntansi.Properties.Settings.Setting"].ConnectionString;
            textBox1.Text = StringCipher.Decrypt(conString, "123");
        }
    }
}
