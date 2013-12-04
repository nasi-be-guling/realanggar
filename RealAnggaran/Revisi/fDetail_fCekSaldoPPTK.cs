using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RealAnggaran.Revisi
{
    public partial class FDetailFCekSaldoPPTK : Form
    {
        readonly CKonek _connect = new CKonek();
        readonly CAlat _tools = new CAlat();
        private readonly SqlConnection _connection;
        private string _query;

        public FDetailFCekSaldoPPTK()
        {
            _connection = _connect.KonekDb();
            InitializeComponent();
        }

        public void ShowData(string kdPanggil)
        {
            decimal totalBelanjaSubsidi = 0;
            decimal totalBelanjaFungsional = 0;
            int totalTrans = 1;
            int totalTransBelumLunas = 1;

            _connection.Open();
            _query = "SELECT b.No_SPK, b.Tgl_SP, b.IdSupplier, b.Keter, b.No_Bukti, b.Lunas, b.TGL_SPJ, " +
                "a.Id_Rinci_Rs, a.TSubsi, a.TFungsi " +
                "FROM KASDA.dbo.BLJ_DETAIL a INNER JOIN KASDA.dbo.BLJ_MASTER b ON a.IdBlj_Master = b.IdBlj_Master " +
                "WHERE a.Id_Rinci_Rs = '"+kdPanggil+"'";
            SqlDataReader reader = _connect.MembacaData(_query, _connection);
                    if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ListViewItem item = new ListViewItem(_tools.PengecekField(reader, 0));
                    item.SubItems.Add(_tools.PengecekField(reader, 1));
                    item.SubItems.Add(_tools.PengecekField(reader, 2));
                    item.SubItems.Add(_tools.PengecekField(reader, 3));
                    item.SubItems.Add(_tools.PengecekField(reader, 4));
                    item.SubItems.Add(_tools.PengecekField(reader, 5));
                    item.SubItems.Add(_tools.PengecekField(reader, 6));
                    item.SubItems.Add(_tools.PengecekField(reader, 7));
                    item.SubItems.Add(string.Format(new CultureInfo("id-ID"), "Rp. {0:n}", reader[8]));
                    item.SubItems.Add(string.Format(new CultureInfo("id-ID"), "Rp. {0:n}", reader[9]));
                    totalBelanjaSubsidi = totalBelanjaSubsidi + Convert.ToDecimal(reader[8]);
                    totalBelanjaFungsional = totalBelanjaFungsional + Convert.ToDecimal(reader[9]);
                    listView1.Items.Add(item);
                    totalTrans++;
                    if (string.IsNullOrEmpty(_tools.PengecekField(reader, 5).Trim()))
                    {
                        totalTransBelumLunas++;
                    }

                }
                _tools.AutoresizeLv(listView1, 10);
                reader.Close();
            }
            _connection.Close();
            label7.Text = totalTrans.ToString();
            label9.Text = totalTransBelumLunas.ToString();
            label3.Text = string.Format(new CultureInfo("id-ID"), "Rp. {0:n}", totalBelanjaSubsidi);
            label5.Text = string.Format(new CultureInfo("id-ID"), "Rp. {0:n}",totalBelanjaFungsional);
        }
    }
}
