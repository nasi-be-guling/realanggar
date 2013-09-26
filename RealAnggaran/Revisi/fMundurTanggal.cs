using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;


namespace RealAnggaran.Revisi
{
    public partial class FMundurTanggal : Form
    {   
        private readonly CKonek _connect = new CKonek();
        private readonly CAlat _tools = new CAlat();
        private readonly SqlConnection _connection;
        private SqlDataReader _reader;
        private List<TampungLaporanTran> _tabelKasda;
/*
        List<Bicycle> _testBicycles;
*/

        public FMundurTanggal()
        {
            _connection = _connect.KonekDb();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        //private class TampungLaporanTran
        //{
        //    //Alternatif buat class dengan atau tanpa constructor
        //    public TampungLaporanTran(decimal tahun, DateTime tglSp, string noSpk, string idSupplier, string keter, string noBukti, 
        //        string kdPanggil, string kdRekening, decimal total, string lunas, DateTime tglSpj, string idBljMaster)
        //    {
        //        this.IdBljMaster = idBljMaster;                 // Jika tidak menggunakan constructor
        //        this.IdSupplier = idSupplier;                   // hapus pada baris yg di comment
        //        this.KdPanggil = kdPanggil;                     // ====================================
        //        this.KdRekening = kdRekening;                   // | Alt + Insert untuk meng-generate |
        //        this.Keter = keter;                             // | construktor dari class yg dibuat |
        //        this.NoBukti = noBukti;                         // ====================================
        //        this.Lunas = lunas;
        //        this.NoSPK = noSpk;                                       DEPRECATED !!!
        //        this.Tahun = tahun;
        //        this.TglSP = tglSp;
        //        this.TglSpj = tglSpj;
        //        this.Total = total;
        //    }

        //    private decimal Tahun { set; get; }
        //    private DateTime TglSP { set; get; }
        //    private string NoSPK { set; get; }
        //    private string IdSupplier { set; get; }
        //    private string Keter { set; get; }
        //    private string NoBukti { set; get; }
        //    private string KdPanggil { set; get; }
        //    private string KdRekening { set; get; }
        //    private decimal Total { set; get; }
        //    private string Lunas { set; get; }
        //    private DateTime TglSpj { set; get; }
        //    private string IdBljMaster { set; get; }
        //} 

        private class TampungLaporanTran
        {
            public TampungLaporanTran(decimal tahun, DateTime tglSp, string noSpk, string idSupplier, string keter, string noBukti, string kdPanggil,
                string kdRekening, decimal total, string lunas, DateTime tglSpj, string idBljMaster)
            {
                IdBljMaster = idBljMaster;
                IdSupplier = idSupplier;
                KdPanggil = kdPanggil;
                KdRekening = kdRekening;
                Keter = keter;
                Lunas = lunas;
                NoBukti = noBukti;
                NoSpk = noSpk;
                Tahun = tahun;
                TglSp = tglSp;
                TglSpj = tglSpj;
                Total = total;
            }

            public string IdBljMaster { get; private set; }

            public DateTime TglSpj { get; private set; }

            public string Lunas { get; private set; }

            public decimal Total { get; private set; }

            public string KdRekening { get; private set; }

            public string KdPanggil { get; private set; }

            public string NoBukti { get; private set; }

            public string Keter { get; private set; }

            public string IdSupplier { get; private set; }

            public string NoSpk { get; private set; }

            public DateTime TglSp { get; private set; }

            public decimal Tahun { get; private set; }
        }

        public class Bicycle
        {

            private readonly string _cadence;
            private readonly int _gear;
            private readonly int _speed;

            public Bicycle(string cadence, int gear, int speed)
            {
                _cadence = cadence;
                _gear = gear;
                _speed = speed;
            }

            public int Speed
            {
                get { return _speed; }
            }

            public int Gear
            {
                get { return _gear; }
            }

            public string Cadence
            {
                get { return _cadence; }
            }
        }

/*
        private void FillBicycle()
        {
            List<Bicycle> testBicycles1 = new List<Bicycle> {new Bicycle("BISA", 1, 1), new Bicycle("HORE", 10, 10)};
            _testBicycles = testBicycles1;
        }
*/

/*
        private void GetBicycle()
        {
            var nama = (from s in _testBicycles select s);
            foreach (var i in nama)
            {
                MessageBox.Show(i.Speed + i.Gear + i.Cadence);
            }
        }
*/

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.BringToFront();
            progressBar1.Visible = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int progressIncrement = 1;
            string query = "SELECT A.Tahun, A.Tgl_SP, A.No_SPK, A.IdSupplier, A.Keter, A.No_Bukti, B.Id_Rinci_Rs, C.formatPanjang, B.TSubsi + B.TFungsi AS total, A.Lunas, A.TGL_SPJ, A.IdBlj_Master " +
                "FROM         KASDA..BLJ_MASTER AS A INNER JOIN " +
                "KASDA..BLJ_DETAIL AS B ON A.IdBlj_Master = B.IdBlj_Master INNER JOIN " +
                "KASDA..AKD_RINCIAN AS C ON B.Id_Rinci_Rs = C.Id_Rinci_RS " +
                "WHERE     (A.Lunas = ' ') AND (A.No_Bukti = ' ') AND (A.Tgl_SP BETWEEN CONVERT(DATETIME, '" + string.Format("{0:yyyy-MM-dd}", dateTimePicker1.Value) + " 00:00:00', 121) " +
                "AND CONVERT(DATETIME, '" + string.Format("{0:yyyy- MM-dd}", dateTimePicker2.Value) + " 23:59:59', 121))";
            List<TampungLaporanTran> grupLaporanTran = new List<TampungLaporanTran>();
            _connection.Open();
            // ===============  HITUNG ROW COUNT ========================
            SqlCommand objCommand = new SqlCommand(query, _connection);
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            adapter.SelectCommand = objCommand;
            adapter.Fill(ds);
            progressBar1.SafeControlInvoke(progressBar => progressBar1.Maximum = ds.Tables[0].Rows.Count + 1);
            // ==========================================================
            _reader = _connect.MembacaData(query, _connection);
            if (_reader.HasRows)
            {
                while (_reader.Read())
                {
                    progressIncrement++;
                    int increment = progressIncrement;
                    progressBar1.SafeControlInvoke(progressBar => progressBar1.Value = increment);
                    grupLaporanTran.Add(new TampungLaporanTran(Convert.ToDecimal(_tools.PengecekField(_reader, 0)), Convert.ToDateTime(_reader[1]), _tools.PengecekField(_reader, 2), _tools.PengecekField(_reader, 3), 
                        _tools.PengecekField(_reader, 4), _tools.PengecekField(_reader, 5), _tools.PengecekField(_reader, 6), _tools.PengecekField(_reader, 7), Convert.ToDecimal(_tools.PengecekField(_reader, 8)), _tools.PengecekField(_reader, 9),
                        Convert.ToDateTime(_reader[10]), _tools.PengecekField(_reader, 11)));
                }
                _reader.Close();
            }
            _connection.Close();
            e.Result = grupLaporanTran;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<ListViewItem> lvItemGroup = new List<ListViewItem>();
            _tabelKasda = (List<TampungLaporanTran>)e.Result;
            IEnumerable<TampungLaporanTran> namaKpa = (from s in _tabelKasda select s);
            //Bicycle tesBicycle = new Bicycle();
            //var nam1 = (from s in testBicycles select s);
            foreach (var i in namaKpa)
            {
                ListViewItem item = new ListViewItem(i.Tahun.ToString(CultureInfo.InvariantCulture));
                item.SubItems.Add(String.Format("{0:dd/MM/yyyy}", i.TglSp));
                item.SubItems.Add(i.NoSpk);
                item.SubItems.Add(i.IdSupplier);
                item.SubItems.Add(i.Keter);
                item.SubItems.Add(i.NoBukti);
                item.SubItems.Add(i.KdPanggil);
                item.SubItems.Add(i.KdRekening);
                item.SubItems.Add(i.Total.ToString("C0", CultureInfo.CreateSpecificCulture("id-ID")));
                item.SubItems.Add(i.Lunas);
                item.SubItems.Add(String.Format("{0:dd/MM/yyyy}", i.TglSpj));
                item.SubItems.Add(i.IdBljMaster);
                item.SubItems.Add(i.NoBukti);
                lvItemGroup.Add(item);
            }
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Items.AddRange(lvItemGroup.ToArray());
            listView1.EndUpdate();
            lvItemGroup.Clear();

            progressBar1.Value = 0;
            progressBar1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_connection != null)
            {
                _connection.Open();
                SqlTransaction trans = _connection.BeginTransaction();
                //SqlParameter p = new SqlParameter();
                //foreach (ListViewItem item in listView1.SelectedItems)
                //{
                //    MessageBox.Show(item.SubItems[11].Text);
                //    p.ParameterName = "@ID";
                //    p.Value = item.SubItems[11].Text;
                //    using (SqlConnection connect = new SqlConnection())
                //    {
                //        //connect. = "DELETE FROM tbl_Users WHERE userID = @id";   
                //    }
                //}
                try
                {
                    for (int i = 938; i < 950; i++)
                    {
                        _connect.BulkCommand(_connection, trans, "DELETE FROM cobanya..a_rekening WHERE id_reken = @id", "@id",
                            i.ToString(CultureInfo.InvariantCulture));
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Terjadi Kesalahan dengan tipe : {0}, Pesan : {1}", ex.GetType(),
                        ex.Message));
                    try
                    {
                        trans.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(string.Format("Terjadi Kesalahan dengan tipe : {0}, Pesan : {1}", ex2.GetType(),
                            ex2.Message));
                        throw;
                    }
                    throw;
                }

                _connection.Close();
            }
        }
    }
}
