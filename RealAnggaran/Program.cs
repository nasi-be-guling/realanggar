using System;
using System.Windows.Forms;
using RealAnggaran.cetak;
using RealAnggaran.misc_tool;
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
            Application.Run(new FPak());
        }
    }
}
