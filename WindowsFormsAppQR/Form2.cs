using PdfiumPrinter;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppQR
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // var printerName = "35F-yumying-243 (HP LaserJet MFP M427dw)xx"; //You can use your own printer;
            var printerName = "35F-yumying-243 (HP LaserJet MFP M427dw)";
            var printer = new PdfPrinter(printerName);
            var printFile = "test2021072112.pdf"; //The path to the pdf which needs to be printed;
            printer.Print(printFile);
        }
    }
}
