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
using ExcelLibrary.SpreadSheet;

namespace RealAnggaran.Revisi
{
    public partial class FDetailFCekSaldoPPTK : Form
    {
        readonly CKonek _connect = new CKonek();
        readonly CAlat _tools = new CAlat();
        private readonly SqlConnection _connection;
        private string _query;
        private decimal _totalBelanjaSubsidi;
        private decimal _totalBelanjaFungsional;
        private int _totalTrans;
        private int _totalTransBelumLunas;

        public FDetailFCekSaldoPPTK()
        {
            _connection = _connect.KonekDb();
            InitializeComponent();
        }

        public void ShowData(string kdPanggil)
        {
            _totalTrans = 1;
            _totalTransBelumLunas = 1;
            _totalBelanjaSubsidi = 0;
            _totalBelanjaFungsional = 0;
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
                    _totalBelanjaSubsidi = _totalBelanjaSubsidi + Convert.ToDecimal(reader[8]);
                    _totalBelanjaFungsional = _totalBelanjaFungsional + Convert.ToDecimal(reader[9]);
                    listView1.Items.Add(item);
                    _totalTrans++;
                    if (string.IsNullOrEmpty(_tools.PengecekField(reader, 5).Trim()))
                    {
                        _totalTransBelumLunas++;
                    }

                }
                _tools.AutoresizeLv(listView1, 10);
                reader.Close();
            }
            _connection.Close();
            label7.Text = _totalTrans.ToString();
            label9.Text = _totalTransBelumLunas.ToString();
            label3.Text = string.Format(new CultureInfo("id-ID"), "Rp. {0:n}", _totalBelanjaSubsidi);
            label5.Text = string.Format(new CultureInfo("id-ID"), "Rp. {0:n}",_totalBelanjaFungsional);
        }

        private void FDetailFCekSaldoPPTK_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F8)
            {
                if (backgroundWorker1 != null && !backgroundWorker1.IsBusy)
                {
                    if (saveFileDialog1.ShowDialog()== DialogResult.OK)
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //create new xls file
            //string file = "C:\\newdoc.xls";
            int rows = 1;
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet("First Sheet");

            listView1.SafeControlInvoke(listview =>
            {
                #region Inisiasi Header Kolom
                worksheet.Cells[0, 0] = new Cell("NO SPK");
                worksheet.Cells[0, 1] = new Cell("TGL SPK");
                worksheet.Cells[0, 2] = new Cell("SUPPLIER");
                worksheet.Cells[0, 3] = new Cell("KETERANGAN");
                worksheet.Cells[0, 4] = new Cell("NO BUKTI");
                worksheet.Cells[0, 5] = new Cell("LUNAS");
                worksheet.Cells[0, 6] = new Cell("TGL SPJ");
                worksheet.Cells[0, 7] = new Cell("KODE PANGGILA");
                worksheet.Cells[0, 8] = new Cell("SUBSIDI");
                worksheet.Cells[0, 9] = new Cell("FUNGSIONAL"); 
                #endregion
                foreach (ListViewItem item in listView1.Items)
                {
                    worksheet.Cells[rows, 0] = new Cell(item.Text);
                    worksheet.Cells[rows, 1] = new Cell(item.SubItems[1].Text);
                    worksheet.Cells[rows, 2] = new Cell(item.SubItems[2].Text);
                    worksheet.Cells[rows, 3] = new Cell(item.SubItems[3].Text);
                    worksheet.Cells[rows, 4] = new Cell(item.SubItems[4].Text);
                    worksheet.Cells[rows, 5] = new Cell(item.SubItems[5].Text);
                    worksheet.Cells[rows, 6] = new Cell(item.SubItems[6].Text);
                    worksheet.Cells[rows, 7] = new Cell(item.SubItems[7].Text);
                    worksheet.Cells[rows, 8] = new Cell(item.SubItems[8].Text);
                    worksheet.Cells[rows, 9] = new Cell(item.SubItems[9].Text);
                    rows++;
                }
                #region Total dan summary Footer
                worksheet.Cells[rows + 2, 0] = new Cell("Total Transaksi : " + _totalTrans);
                worksheet.Cells[rows + 2, 1] = new Cell("Total Transaksi Belum Lunas : " + _totalTransBelumLunas);
                worksheet.Cells[rows + 2, 8] = new Cell(Convert.ToString(_totalBelanjaSubsidi));
                worksheet.Cells[rows + 2, 9] = new Cell(Convert.ToString(_totalBelanjaFungsional)); 
                #endregion
            });
            worksheet.Cells.ColumnWidth[0, 1] = 3000;
            workbook.Worksheets.Add(worksheet);
            workbook.Save(saveFileDialog1.FileName);
        }

        private void Bantai()
        {
            backgroundWorker1.CancelAsync();
            backgroundWorker1.Dispose();
            backgroundWorker1 = null;
            GC.Collect();
        }
    }
}
