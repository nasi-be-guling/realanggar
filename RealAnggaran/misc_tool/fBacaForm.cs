using System;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace RealAnggaran.misc_tool
{
    public partial class fBacaForm : Form
    {
        public fBacaForm()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            InitializeComponent();
        }

        public void Read(string filename)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Open(filename);

            // Get worksheet names
            foreach (Microsoft.Office.Interop.Excel.Worksheet sh in wb.Worksheets)
                Debug.WriteLine(sh.Name);

            // Get values from sheets SH1 and SH3 (in my file)
            //object val1 = wb.Sheets["SH1"].Cells[1, "A"].Value2;
            //object val3 = wb.Sheets["SH3"].Cells[1, "A"].Value2;
            //Debug.WriteLine("{0} / {1}", val1, val3);

            

            wb.Close();
            excel.Quit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            string str;
            int rCnt = 0;
            int cCnt = 0;

            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.ApplicationClass();
            xlWorkBook = xlApp.Workbooks.Open(textBox1.Text, 0, 
                true, 5, "", "", true, Excel.XlPlatform.xlWindows, 
                "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;

            // Menampilkan nilai pada cell A10
            //MessageBox.Show(xlWorkSheet.get_Range("A10", "A10").Value2.ToString());
            //Menampilkan Range
            string[,] myArray;
            myArray = new string[range.Rows.Count, range.Columns.Count];
            for (rCnt = 1; rCnt <= 49; rCnt++) //Baris
            {
                //for (cCnt = 1; cCnt <= 1; cCnt++) //Kolom
                //{
                    //str = (string)(range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                    //MessageBox.Show(str);
                    ListViewItem item = new ListViewItem((string)(range.Cells[rCnt, 1] as Excel.Range).Value2);
                    item.SubItems.Add((string)(range.Cells[rCnt, 2] as Excel.Range).Value2);
                    listView1.Items.Add(item);
                //}
            }

            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }

        private void showOnListView()
        {

        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        } 

        private void button3_Click(object sender, EventArgs e)
        {
            Read(textBox1.Text);
        }
    }
}
