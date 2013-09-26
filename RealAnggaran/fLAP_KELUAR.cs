using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

namespace RealAnggaran
{
    public partial class fLAP_KELUAR : Form
    {
        SqlConnection koneksi;
        SqlDataReader pembaca = null;
        CKonek konek = new CKonek();
        CAlat alat = new CAlat();
        string query = null;
        SqlDataAdapter adaptor;
        int statusPencarianKpa = 0;
        int statusPencarianDF = 0;
        //private CrystalReport1 rapor = new CrystalReport1();
        private CrystalReport2 rapor = new CrystalReport2();
        private int buttonStatus = 0;
        private string isiKomboBox1 = "";
        private string isiKomboBox2 = "";

        public fLAP_KELUAR()
        {
            InitializeComponent();
            koneksi = konek.KonekDb();
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "dd/MM/yyyy";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker3.CustomFormat = "dd/MM/yyyy";
            dateTimePicker3.Format = DateTimePickerFormat.Custom;
            dateTimePicker4.CustomFormat = "dd/MM/yyyy";
            dateTimePicker4.Format = DateTimePickerFormat.Custom;
            dateTimePicker5.CustomFormat = "dd/MM/yyyy";
            dateTimePicker5.Format = DateTimePickerFormat.Custom;
            dateTimePicker6.CustomFormat = "dd/MM/yyyy";
            dateTimePicker6.Format = DateTimePickerFormat.Custom;
        }

 /* THIS is function to get count of working day of past mount
        private int GetWorkingDays(DateTime dtmStart, DateTime dtmEnd)
        {
            // This function includes the start and end date if it falls on a weekday 
            int dowStart = ((int)dtmStart.DayOfWeek == 0 ? 7 : (int)dtmStart.DayOfWeek);
            int dowEnd = ((int)dtmEnd.DayOfWeek == 0 ? 7 : (int)dtmEnd.DayOfWeek);
            TimeSpan tSpan = dtmEnd - dtmStart;
            if (dowStart <= dowEnd)
            {
                return (((tSpan.Days / 7) * 5) + Math.Max((Math.Min((dowEnd + 1), 6) - dowStart), 0));
            }
            else
            {
                return (((tSpan.Days / 7) * 5) + Math.Min((dowEnd + 6) - Math.Min(dowStart, 6), 5));
            }
        }
  */
  /* THIS is function to get the last friday of past mount
        public static DateTime LastDayOfTheMonth(int Month, System.DayOfWeek DayOfTheWeek)
        {
            return LastDayOfTheMonth(Month, DateTime.Today.Year, DayOfTheWeek);
        }

        public static DateTime LastDayOfTheMonth(int Month, int Year, System.DayOfWeek DayOfTheWeek)
        {
            Calendar MyCalendar = CultureInfo.CurrentCulture.Calendar;
            int NoOfDays = MyCalendar.GetDaysInMonth(Year, Month);
            DateTime MyDate = new DateTime(Year, Month, NoOfDays);
            while (MyDate.DayOfWeek != DayOfTheWeek)
            {
                MyDate = MyDate.AddDays(-1);
            }
            return MyDate;
        }
   */

        private void bTampil_Click(object sender, EventArgs e)
        {
            /* THIS is the call for function to get the last friday of past mount
            for (int i = 1; i < 13; i++)
            {
	            MessageBox.Show(LastDayOfTheMonth(i, System.DayOfWeek.Friday).ToShortDateString());
            } */
            //MessageBox.Show(LastDayOfTheMonth(5, 2011, DayOfWeek);
            //crystalReportViewer1.ReportSource = showReport(1);
            //MessageBox.Show(alat.dateFormater(dateTimePicker1.Text));
            //MessageBox.Show(DateTime.DaysInMonth(2011, 05).ToString());
            //MessageBox.Show(ambilTanggalAkhir(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month - 1).ToString() + "/" + Convert.ToString(dateTimePicker1.Value.Month - 1) + "/" + Convert.ToString(dateTimePicker1.Value.Year));
            buttonStatus = 1;
            progressBar1.Visible = true;
            tampilkanReport();
        }

        private void tampilkanReport()
        {
            progressBar1.Value = progressBar1.Minimum;
            lStatus.Text = "Now loading.....................................................";
            backgroundWorker1.RunWorkerAsync();
        }

        private int ambilTanggalAkhir(int tahun, int bulan)
        {
            int i = DateTime.DaysInMonth(tahun, bulan);
            DateTime datevalue = new DateTime(tahun, bulan, i);
            if (datevalue.ToString("dddd") == "Sabtu")
                i = i - 1;
            else if (datevalue.ToString("dddd") == "Minggu")
                i = i - 2;
            return i;
        }

        //private CrystalReport1 showReport(int buttonStatus)
        //{
        //    CrystalReport1 rapor = new CrystalReport1();
        //    //string awalDate = dtpAwal.Text.Substring(3, 2) + "/" + dtpAwal.Text.Substring(0, 2) + "/" + dtpAwal.Text.Substring(6, 4);
        //    //string akhirDate = dtpAkhir.Text.Substring(3, 2) + "/" + dtpAkhir.Text.Substring(0, 2) + "/" + dtpAkhir.Text.Substring(6, 4); ;
        //    //if (buttonStatus == 1)
        //    //{
        //    //    query = "SELECT A.no_jurnal, A.tgl_entry, D.nama_jen_jurnal, A.ket, B.kd_cetak, B.uraian, C.no_bukti, C.ket AS 'KetBukti', C.debet AS 'debet', C.kredit" +
        //    //        " AS 'kredit' FROM A_JURNAL A, A_KODE_REK B, A_DET_JUR C, A_JEN_JURNAL D WHERE C.no_jurnal = A.no_jurnal AND D.id_jen_jurnal = A.id_jen_jurnal" +
        //    //        " AND B.id_kode_rek = C.id_kode_rek AND (tgl_entry between '" + awalDate + "' AND '" + akhirDate + "') ORDER BY A.no_jurnal";
        //    //}
        //    //else if (buttonStatus == 2)
        //    {
        //        //query = "SELECT " +
        //        //    "TglBayar, NoBayar, kdKpa, ketBayar AS 'Rekening', kd_Reken, JmlRp, PPnRp, PPhRp, kdSumber AS 'SubFung', Sisa, " +
        //        //    "terimaTotal = (SELECT sum(JmTerima) FROM A_PENERIMAAN WHERE TglTerima BETWEEN '" + alat.dateFormater(dateTimePicker1.Text) +
        //        //    "' AND '" + alat.dateFormater(dateTimePicker2.Text) + "') GR " +
        //        //    "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
        //        //    "((b.id_reken = a.id_reken AND e.Id_Kpa = b.Id_Kpa AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber)" +
        //        //    " AND lunas = 1 AND f.ket = a.NoBayar AND TglBayar BETWEEN '" + alat.dateFormater(dateTimePicker1.Text) + "' AND '" + alat.dateFormater(dateTimePicker2.Text) + "') ORDER BY TglBayar DESC";
        //        //query = "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', kdKpa, 'Penerimaan' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', sisa  FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c WHERE b.id_Kpa = a.id_Kpa AND c.idSumber" +
        //        //    " = a.idSumber AND JmTerima <> 0 AND tglTerima BETWEEN '" + alat.dateFormater(dateTimePicker1.Text) + "' AND '" + alat.dateFormater(dateTimePicker2.Text) + "' " +
        //        //    "UNION " +
        //        //    "SELECT TglBayar, NoBayar, kdKpa, CAST(ketBayar AS VARCHAR (50)) AS 'Rekening', kd_Reken, JmlRp, PPnRp, PPhRp, kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', sisa " +
        //        //    "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
        //        //    "((e.id_Kpa = b.id_kpa AND b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber) AND lunas = 1) AND f.ket = a.NoBayar AND tglBayar BETWEEN '" + alat.dateFormater(dateTimePicker1.Text) + "' AND '" + alat.dateFormater(dateTimePicker2.Text) + "' " +
        //        //    "order by kdKpa, kdSumber, tglBayar";
        //        if (buttonStatus == 1)
        //        {
        //            query = "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', z.kdKpa, 'SALDO AWAL' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', z.kdSumber AS 'SubFung', sisa AS 'TerimaTotal', '0' AS 'Sisa' FROM A_PENERIMAAN b " +
        //                        "INNER JOIN " +
        //                        "(SELECT MAX(id_Terima) AS id_terima, kdKpa, kdSumber FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c " +
        //                        "WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Month - 1) + " GROUP BY kdKpa, kdSumber) z " +
        //                        "ON b.id_Terima = z.id_terima " +
        //                        "UNION " +
        //                        "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', kdKpa, 'Penerimaan' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa'  FROM A_PENERIMAAN" +
        //                        " a, A_KPA b, A_SUMBER_DANA c WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND JmTerima <> 0 AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Month) +
        //                        " UNION " +
        //                        "SELECT TglBayar, NoBayar, kdKpa, CAST(ketBayar AS VARCHAR (50)) AS 'Rekening', kd_Reken, JmlRp, PPnRp, PPhRp, kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa' " +
        //                        "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
        //                        "((e.id_Kpa = b.id_kpa AND b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber) AND lunas = 1) AND f.ket = a.NoBayar AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Month) +
        //                        " order by TglBayar, kdKpa, Rekening";
        //        }
        //        else if (buttonStatus == 2)
        //        {
        //            query = "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', z.kdKpa, 'SALDO AWAL' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', z.kdSumber AS 'SubFung', sisa AS 'TerimaTotal', '0' AS 'Sisa' FROM A_PENERIMAAN b " +
        //                "INNER JOIN " +
        //                "(SELECT MAX(id_Terima) AS id_terima, kdKpa, kdSumber FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c " +
        //                "WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker4.Value.Month - 1) + " GROUP BY kdKpa, kdSumber) z " +
        //                "ON b.id_Terima = z.id_terima AND z.KdKpa = '" + comboBox1.Text + "'" +
        //                "UNION " +
        //                "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', kdKpa, 'Penerimaan' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa'  FROM A_PENERIMAAN" +
        //                " a, A_KPA b, A_SUMBER_DANA c WHERE b.id_Kpa = a.id_Kpa AND b.KdKpa = '" + comboBox1.Text + "' AND c.idSumber = a.idSumber AND JmTerima <> 0 AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker4.Value.Month) +
        //                " UNION " +
        //                "SELECT TglBayar, NoBayar, kdKpa, CAST(ketBayar AS VARCHAR (50)) AS 'Rekening', kd_Reken, JmlRp, PPnRp, PPhRp, kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa' " +
        //                "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
        //                "((e.id_Kpa = b.id_kpa AND e.KdKpa = '" + comboBox1.Text + "' AND b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber) AND lunas = 1) AND f.ket = a.NoBayar AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker4.Value.Month) +
        //                " order by TglBayar, kdKpa, Rekening";
        //        }
        //        else if (buttonStatus == 3)
        //        {
        //            query = "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', z.kdKpa, 'SALDO AWAL' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', z.kdSumber AS 'SubFung', sisa AS 'TerimaTotal', '0' AS 'Sisa' FROM A_PENERIMAAN b " +
        //                "INNER JOIN " +
        //                "(SELECT MAX(id_Terima) AS id_terima, kdKpa, kdSumber FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c " +
        //                "WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Month - 1) + " GROUP BY kdKpa, kdSumber) z " +
        //                "ON b.id_Terima = z.id_terima " +
        //                "UNION " +
        //                "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', kdKpa, 'Penerimaan' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa'  FROM A_PENERIMAAN" +
        //                " a, A_KPA b, A_SUMBER_DANA c WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND JmTerima <> 0 AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Month) +
        //                " UNION " +
        //                "SELECT TglBayar, NoBayar, kdKpa, CAST(ketBayar AS VARCHAR (50)) AS 'Rekening', kd_Reken, JmlRp, PPnRp, PPhRp, kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa' " +
        //                "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
        //                "((e.id_Kpa = b.id_kpa AND b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber) AND lunas = 1) AND f.ket = a.NoBayar AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Month) +
        //                " order by TglBayar, kdKpa, Rekening";
        //        }
        //    }
        //    koneksi.Open();
        //    adaptor = new SqlDataAdapter(query, koneksi);
        //    DataSet ds = new DataSet();
        //    adaptor.Fill(ds, "dTPengeluaran");
        //    rapor.SetDataSource(ds);
        //    koneksi.Close();
        //    return rapor;
        //}

        private void isiKomboKPA()
        {
            alat.IsiCombo(comboBox1, "SELECT kdKpa FROM A_KPA", koneksi, 0);
        }

        private void isiKomboDF()
        {
            alat.IsiCombo(comboBox2, "SELECT kdSumber FROM A_SUMBER_DANA", koneksi, 0);
        }

        private void bTampilKPA_Click(object sender, EventArgs e)
        {
            if (statusPencarianKpa == 0)
            {
                MessageBox.Show("SILAHKAN TENTUKAN KRITERIA PENCARIAN!!", "PERHATIAN");
                comboBox1.Focus();
            }
            else if (statusPencarianKpa == 1)
            {
                progressBar1.Visible = true;
                //crystalReportViewer3.ReportSource = showReport(2);
                buttonStatus = 2;
                tampilkanReport();
            }
        }

        private void fLAP_KELUAR_Load(object sender, EventArgs e)
        {
            isiKomboKPA();
            isiKomboDF();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            koneksi.Open();
            pembaca = konek.MembacaData("SELECT namaKPA FROM A_KPA WHERE kdKPA = '" + comboBox1.Text.Trim() + "'", koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                label4.Text = alat.PengecekField(pembaca, 0);
                isiKomboBox1 = comboBox1.Text.Trim();
                statusPencarianKpa = 1;
                pembaca.Close();
            }
            koneksi.Close();
        }

        private void clearStatusPencarian()
        {
            statusPencarianKpa = 0;
            comboBox1.Text = "";
            label4.Text = "<nama_kpa>";
            statusPencarianDF = 0;
            comboBox2.Text = "";
            label3.Text = "df/f";
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void bTampilDF_Click(object sender, EventArgs e)
        {
            if (statusPencarianDF == 0)
            {
                MessageBox.Show("SILAHKAN TENTUKAN KRITERIA PENCARIAN!!", "PERHATIAN");
                comboBox2.Focus();
            }
            else if (statusPencarianDF == 1)
            {
                progressBar1.Visible = true;
                buttonStatus = 3;
                tampilkanReport();
            }
        }
        #region THIS IS FUCKIN IMPLEMENTATION OF FUCKIN THREAD HANDLING
        private void dg1(int dog)
        {
            //crystalReportViewer1.ReportSource = showReport(dog);
        }

        private void dg2()
        {
            //crystalReportViewer3.ReportSource = showReport(2);
        }

        private void dg3()
        {
            //crystalReportViewer1.ReportSource = showReport(3);
        }

        delegate void MyDelegate(int dog);

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //MyDelegate bogbog = new MyDelegate(dg1);
            ////Invoke crystalReportViewer1.ReportSource = showReport(1);
            ////label4.Text = "harap";

            //crystalReportViewer1.Invoke(bogbog, new object[] { 1 });
            //CrystalReport1 rapor = new CrystalReport1();
            //string awalDate = dtpAwal.Text.Substring(3, 2) + "/" + dtpAwal.Text.Substring(0, 2) + "/" + dtpAwal.Text.Substring(6, 4);
            //string akhirDate = dtpAkhir.Text.Substring(3, 2) + "/" + dtpAkhir.Text.Substring(0, 2) + "/" + dtpAkhir.Text.Substring(6, 4); ;
            //if (buttonStatus == 1)
            //{
            //    query = "SELECT A.no_jurnal, A.tgl_entry, D.nama_jen_jurnal, A.ket, B.kd_cetak, B.uraian, C.no_bukti, C.ket AS 'KetBukti', C.debet AS 'debet', C.kredit" +
            //        " AS 'kredit' FROM A_JURNAL A, A_KODE_REK B, A_DET_JUR C, A_JEN_JURNAL D WHERE C.no_jurnal = A.no_jurnal AND D.id_jen_jurnal = A.id_jen_jurnal" +
            //        " AND B.id_kode_rek = C.id_kode_rek AND (tgl_entry between '" + awalDate + "' AND '" + akhirDate + "') ORDER BY A.no_jurnal";
            //}
            //else if (buttonStatus == 2)
            {
                //query = "SELECT " +
                //    "TglBayar, NoBayar, kdKpa, ketBayar AS 'Rekening', kd_Reken, JmlRp, PPnRp, PPhRp, kdSumber AS 'SubFung', Sisa, " +
                //    "terimaTotal = (SELECT sum(JmTerima) FROM A_PENERIMAAN WHERE TglTerima BETWEEN '" + alat.dateFormater(dateTimePicker1.Text) +
                //    "' AND '" + alat.dateFormater(dateTimePicker2.Text) + "') GR " +
                //    "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
                //    "((b.id_reken = a.id_reken AND e.Id_Kpa = b.Id_Kpa AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber)" +
                //    " AND lunas = 1 AND f.ket = a.NoBayar AND TglBayar BETWEEN '" + alat.dateFormater(dateTimePicker1.Text) + "' AND '" + alat.dateFormater(dateTimePicker2.Text) + "') ORDER BY TglBayar DESC";
                //query = "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', kdKpa, 'Penerimaan' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', sisa  FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c WHERE b.id_Kpa = a.id_Kpa AND c.idSumber" +
                //    " = a.idSumber AND JmTerima <> 0 AND tglTerima BETWEEN '" + alat.dateFormater(dateTimePicker1.Text) + "' AND '" + alat.dateFormater(dateTimePicker2.Text) + "' " +
                //    "UNION " +
                //    "SELECT TglBayar, NoBayar, kdKpa, CAST(ketBayar AS VARCHAR (50)) AS 'Rekening', kd_Reken, JmlRp, PPnRp, PPhRp, kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', sisa " +
                //    "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
                //    "((e.id_Kpa = b.id_kpa AND b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber) AND lunas = 1) AND f.ket = a.NoBayar AND tglBayar BETWEEN '" + alat.dateFormater(dateTimePicker1.Text) + "' AND '" + alat.dateFormater(dateTimePicker2.Text) + "' " +
                //    "order by kdKpa, kdSumber, tglBayar";
                if (buttonStatus == 0)
                {
                    MessageBox.Show("Masih terjadi kesalahan, harap ulangi operasi");
                }
                else if (buttonStatus == 1)
                {
                    query = "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', z.kdKpa, 'SALDO AWAL' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', '-' AS 'NTPNPPn', '-' AS 'NTPNPPh', '-' AS 'NoPP', z.kdSumber AS 'SubFung', sisa AS 'TerimaTotal', '0' AS 'Sisa' FROM A_PENERIMAAN b " +
                        "INNER JOIN " +
                        "(SELECT MAX(id_Terima) AS id_terima, kdKpa, kdSumber FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c " +
                        "WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND (DATEPART(month, TglTerima) = DATEPART(MONTH, DATEADD(MM, -1, CONVERT(SMALLDATETIME, '" + dateTimePicker1.Text + "', 103))) AND DATEPART(YEAR, TglTerima) = " +
                        Convert.ToString(dateTimePicker1.Value.Year) + ") GROUP BY kdKpa, kdSumber) z " +
                        "ON b.id_Terima = z.id_terima " +
                        "UNION " +
                        "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', kdKpa, a.ket AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', '-' AS 'NTPNPPn', '-' AS 'NTPNPPh', '-' AS 'NoPP', kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa'  FROM A_PENERIMAAN" +
                        " a, A_KPA b, A_SUMBER_DANA c WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND JmTerima <> 0 AND (DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Month) + " AND DATEPART(YEAR, TglTerima) = " +
                        Convert.ToString(dateTimePicker1.Value.Year) + ")" +
                        " UNION " +
                        "SELECT TglBayar, NoBayar, kdKpa, CAST(ketBayar AS VARCHAR (50)) AS 'Rekening', RTRIM(kd_Reken) + '.' + formatPanjang AS 'kd_Reken', JmlRp, PPnRp, PPhRp, NTPNPPn, NTPNPPh, PPhNo, kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa' " +
                        "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
                        "(a.Batal = 0 AND (e.id_Kpa = f.id_kpa AND b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber) AND lunas = 1) AND f.ket = a.NoBayar AND (DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Month) +
                        //" AND DATEPART(YEAR, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Year) + ") order by TglBayar, kdKpa, Rekening"; ==> ordernya dikurangi kdKPA dan Rekening
                        " AND DATEPART(YEAR, TglTerima) = " + Convert.ToString(dateTimePicker1.Value.Year) + ") order by TglBayar";
                }
                else if (buttonStatus == 2)
                {
                    query = "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', z.kdKpa, 'SALDO AWAL' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', '-' AS 'NTPNPPn', '-' AS 'NTPNPPh', '-' AS 'NoPP', z.kdSumber AS 'SubFung', sisa AS 'TerimaTotal', '0' AS 'Sisa' FROM A_PENERIMAAN b " +
                        "INNER JOIN " +
                        "(SELECT MAX(id_Terima) AS id_terima, kdKpa, kdSumber FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c " +
                        "WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND DATEPART(month, TglTerima) = DATEPART(MONTH, DATEADD(MM, -1, CONVERT(SMALLDATETIME, '" + dateTimePicker4.Text + "', 103))) GROUP BY kdKpa, kdSumber) z " +
                        "ON b.id_Terima = z.id_terima AND z.KdKpa = '" + isiKomboBox1 + "'" +
                        "UNION " +
                        "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', kdKpa, a.ket AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', '-' AS 'NTPNPPn', '-' AS 'NTPNPPh', '-' AS 'NoPP', kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa'  FROM A_PENERIMAAN" +
                        " a, A_KPA b, A_SUMBER_DANA c WHERE b.id_Kpa = a.id_Kpa AND b.KdKpa = '" + isiKomboBox1 + "' AND c.idSumber = a.idSumber AND JmTerima <> 0 AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker4.Value.Month) +
                        " UNION " +
                        "SELECT TglBayar, NoBayar, kdKpa, CAST(ketBayar AS VARCHAR (50)) AS 'Rekening', RTRIM(kd_Reken) + '.' + formatPanjang AS 'kd_Reken', JmlRp, PPnRp, PPhRp, NTPNPPn, NTPNPPh, PPhNo, kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa' " +
                        "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
                        "((e.id_Kpa = f.id_kpa AND e.KdKpa = '" + isiKomboBox1 + "' AND b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber) AND lunas = 1) AND f.ket = a.NoBayar AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker4.Value.Month) +
                        //" order by TglBayar, kdKpa, Rekening"; 
                        " order by TglBayar";

                }
                else if (buttonStatus == 3)
                {
                    query = "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', z.kdKpa, 'SALDO AWAL' AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', '-' AS 'NTPNPPn', '-' AS 'NTPNPPh', '-' AS 'NoPP', z.kdSumber AS 'SubFung', sisa AS 'TerimaTotal', '0' AS 'Sisa' FROM A_PENERIMAAN b " +
                        "INNER JOIN " +
                        "(SELECT MAX(id_Terima) AS id_terima, kdKpa, kdSumber FROM A_PENERIMAAN a, A_KPA b, A_SUMBER_DANA c " +
                        "WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND DATEPART(month, TglTerima) = DATEADD(MM, -1, CONVERT(SMALLDATETIME, '" + dateTimePicker6.Text + "', 103)) GROUP BY kdKpa, kdSumber) z " +
                        "ON b.id_Terima = z.id_terima AND z.kdSumber = '" + isiKomboBox2 + "'" + 
                        "UNION " +
                        "SELECT TglTerima AS 'TglBayar', ' ' AS 'NoBayar', kdKpa, a.ket AS 'Rekening', ' ' AS 'kd_Reken', '0' AS 'JmlRp', '0' AS 'PPnRp', '0' AS 'PPhRp', '-' AS 'NTPNPPn', '-' AS 'NTPNPPh', '-' AS 'NoPP', kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa'  FROM A_PENERIMAAN" +
                        " a, A_KPA b, A_SUMBER_DANA c WHERE b.id_Kpa = a.id_Kpa AND c.idSumber = a.idSumber AND c.kdSumber = '" + isiKomboBox2 + "' AND JmTerima <> 0 AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker6.Value.Month) +
                        " UNION " +
                        "SELECT TglBayar, NoBayar, kdKpa, CAST(ketBayar AS VARCHAR (50)) AS 'Rekening', RTRIM(kd_Reken) + '.' + formatPanjang AS 'kd_Reken', JmlRp, PPnRp, PPhRp, NTPNPPn, NTPNPPh, PPhNo, kdSumber AS 'SubFung', jmTerima AS 'TerimaTotal', '0' AS 'sisa' " +
                        "FROM A_PENGELUARAN a, A_REKENING b, A_SUPPLIER c, A_JENIS d, A_KPA e, A_PENERIMAAN f, A_SUMBER_DANA g WHERE " +
                        "((e.id_Kpa = f.id_kpa AND b.id_reken = a.id_reken AND c.id_Supplier = a.id_supplier AND d.id_jenis = a.id_jenis AND g.idSumber = a.idSumber AND g.kdSumber = '" + isiKomboBox2 + "') AND lunas = 1) AND f.ket = a.NoBayar AND DATEPART(month, TglTerima) = " + Convert.ToString(dateTimePicker6.Value.Month) +
                        //" order by TglBayar, kdKpa, Rekening";
                        " order by TglBayar";
                }
                else
                {
                    MessageBox.Show("");
                }
            }

            koneksi.Open();
            backgroundWorker1.ReportProgress(10);
            Thread.Sleep(164);
            backgroundWorker1.ReportProgress(20);
            Thread.Sleep(164);
            backgroundWorker1.ReportProgress(30);
            Thread.Sleep(164);
            adaptor = new SqlDataAdapter(query, koneksi);
            DataSet ds = new DataSet();
            try
            {
                //MessageBox.Show(query);
                adaptor.Fill(ds, "dTPengeluaran");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            rapor.SetDataSource(ds);
            backgroundWorker1.ReportProgress(40);
            Thread.Sleep(164);
            backgroundWorker1.ReportProgress(50);
            Thread.Sleep(164);
            backgroundWorker1.ReportProgress(70);
            Thread.Sleep(164);
            backgroundWorker1.ReportProgress(95);
            Thread.Sleep(164);
            backgroundWorker1.ReportProgress(100);
            koneksi.Close();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            //crystalReportViewer3.Invoke(new MyDelegate(dg2));
        }
        #endregion

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lStatus.Text = "";
            progressBar1.Value = progressBar1.Minimum;
            if (buttonStatus == 1)
            {
                crystalReportViewer1.ReportSource = rapor;
            }
            else if (buttonStatus == 2)
            {
                crystalReportViewer3.ReportSource = rapor;
            }
            else if (buttonStatus == 3)
            {
                crystalReportViewer5.ReportSource = rapor;
            }
            clearStatusPencarian();
            progressBar1.Visible = false;
        }

        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            koneksi.Open();
            pembaca = konek.MembacaData("SELECT namaSumber FROM A_SUMBER_DANA WHERE kdSumber = '" + comboBox2.Text.Trim() + "'", koneksi);
            if (pembaca.HasRows)
            {
                pembaca.Read();
                label3.Text = alat.PengecekField(pembaca, 0);
                isiKomboBox2 = comboBox2.Text.Trim();
                statusPencarianDF = 1;
                pembaca.Close();
            }
            koneksi.Close();
        }

        #region SHOULD TRY THIS FOR MULTITHREAD/BACKGROUNDWORKING STUFF
        /*
         *
         *  using System;
            using System.Collections.Generic;
            using System.ComponentModel;
            using System.Data;
            using System.Drawing;
            using System.Linq;
            using System.Text;
            using System.Windows.Forms;
            using System.Threading;
            namespace myCryst
            {
            public partial class Form1 : Form
            {
            DataSet ds;
            frmCryst f = new frmCryst();
            public Form1()
            {
            InitializeComponent();
            }
            private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
            {
            bgWorker.ReportProgress(10);
            Thread.Sleep(164);
            bgWorker.ReportProgress(20);
            Thread.Sleep(164);
            bgWorker.ReportProgress(30);
            Thread.Sleep(164);
            ds = DBClass.ExecQuery("select * from students");
            bgWorker.ReportProgress(40);
            Thread.Sleep(164);
            bgWorker.ReportProgress(50);
            Thread.Sleep(164);
            bgWorker.ReportProgress(70);
            Thread.Sleep(164);
            bgWorker.ReportProgress(95);
            Thread.Sleep(164);
            bgWorker.ReportProgress(100);
            }
            private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
            {
            this.progressBar1.Value = e.ProgressPercentage;
            }
            private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
            showCrystalReport();
            }
            private void button1_Click(object sender, EventArgs e)
            {
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            bgWorker.WorkerReportsProgress = true;
            bgWorker.RunWorkerAsync();
            }
            private void showCrystalReport()
            {
            CrystalReport1 c = new CrystalReport1();
            Reportds d = new Reportds();
            d.Tables[0].Merge(ds.Tables["tab"]);
            frmCryst f = new frmCryst();
            c.SetDataSource(d);
            f.crystRep.ReportSource = c;
            f.Show();
            }
            private void button2_Click(object sender, EventArgs e)
            {
            Application.Exit();
            }
            }
            }
         * 
         */
        #endregion
    }
}
