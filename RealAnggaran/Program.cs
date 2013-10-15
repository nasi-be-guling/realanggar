using System;
using System.Windows.Forms;
using RealAnggaran.cetak;
using RealAnggaran.Revisi;

namespace RealAnggaran
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new dari_pakUZ.Form1());
            //Application.Run(new fUtama());
            //Application.Run(new Revisi.fSubsidi());
            //Application.Run(new Revisi.fCekAnggaran());
            //Application.Run(new Revisi.koreksiSubsidi());
            //Application.Run(new fEnTerima());
            //Application.Run(new Revisi.masterRekening());
            //Application.Run(new misc_tool.fCekSaldoPPTK());
            //Application.Run(new misc_tool.fGenerator());
            Application.Run(new fSubsidi());
            //Application.Run(new FCetakTransaksi());
            //Application.Run(new decrypt());//
            //Application.Run(new Revisi.FMundurTanggal());
            //Application.Run(new fCetakLapEvi());
        }
    }
}
