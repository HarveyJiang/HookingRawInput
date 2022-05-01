using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsAppQR
{
    public partial class Form1 : Form
    {
        BarCodeHook BarCode = new BarCodeHook();
        public Form1()
        {
            InitializeComponent();
            BarCode.BarCodeEvent += new BarCodeHook.BarCodeDelegate(BarCode_BarCodeEvent);

            // textBox1.ImeMode = ImeMode.Disable;
        }


        private delegate void ShowInfoDelegate(BarCodeHook.BarCodes barCode);

        private void ShowInfo(BarCodeHook.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowInfoDelegate(ShowInfo), new object[] { barCode });
            }
            else
            {
                textBox1.Text = barCode.KeyName;//键名
                textBox2.Text = barCode.VirtKey.ToString();//虚拟码
                textBox3.Text = barCode.ScanCode.ToString();//扫描码
                textBox4.Text = barCode.AscII.ToString();//AscII
                textBox5.Text += barCode.Chr.ToString();//字符
                textBox6.Text = barCode.IsValid ? barCode.BarCode : "";
                //在这里进行键入值
            }
        }
        void BarCode_BarCodeEvent(BarCodeHook.BarCodes barCode)
        {
            ShowInfo(barCode);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           // var res =  BarCode.Start();

        }
        private void Form1_StyleChanged(object sender, EventArgs e)
        {
            BarCode.Stop();
        }

        DateTime _lastKeystroke = new DateTime(0);
        List<char> _barcode = new List<char>(10);
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // check timing (keystrokes within 100 ms)
            TimeSpan elapsed = (DateTime.Now - _lastKeystroke);
            if (elapsed.TotalMilliseconds > 100)
                _barcode.Clear();

            // record keystroke & timestamp
            _barcode.Add(e.KeyChar);
            _lastKeystroke = DateTime.Now;

            // process barcode
            if (e.KeyChar == 13 && _barcode.Count > 0)
            {
                string msg = new String(_barcode.ToArray());
                MessageBox.Show(msg);
                _barcode.Clear();
            }
        }
    }
}
