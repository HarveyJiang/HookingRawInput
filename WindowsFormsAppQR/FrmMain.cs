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
    public partial class FrmMain : Form
    {
        BardCodeHooK BarCode = new BardCodeHooK();
        public FrmMain()
        {
            InitializeComponent();
            BarCode.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            //textBox1.ImeMode = ImeMode.Disable;
            
            //textBox1.KeyDown += TextBox1_TextChanged;
        }

        private void TextBox1_TextChanged(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {


                string decode = "";
                byte[] bytes = Convert.FromBase64String(textBox1.Text);
                try
                {
                    decode = Encoding.GetEncoding("utf-8").GetString(bytes);
                }
                catch
                {
                    decode = "ERROR";
                }


                // MessageBox.Show(decode);
                textBox2.Text = decode;
            }
        }

        private delegate void ShowInfoDelegate(BardCodeHooK.BarCodes barCode);
        private void ShowInfo(BardCodeHooK.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowInfoDelegate(ShowInfo), new object[] { barCode });
            }
            else
            {
                //textBox1.Text = barCode.KeyName;
                //textBox2.Text = barCode.VirtKey.ToString();
                //textBox3.Text = barCode.ScanCode.ToString();
                //textBox4.Text = barCode.Ascll.ToString();
                //textBox5.Text = barCode.Chr.ToString();
                //textBox6.Text = barCode.IsValid ? barCode.BarCode : "";//是否为扫描枪输入，如果为true则是 否则为键盘输入
                //textBox7.Text += barCode.KeyName;

                // Console.WriteLine(textBox6.Text);
                label1.Text = barCode.BarCode;
                //MessageBox.Show(barCode.IsValid.ToString());
            }
        }

        //C#中判断扫描枪输入与键盘输入

        //Private DateTime _dt = DateTime.Now;  //定义一个成员函数用于保存每次的时间点
        //private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    DateTime tempDt = DateTime.Now;          //保存按键按下时刻的时间点
        //    TimeSpan ts = tempDt .Subtract(_dt);     //获取时间间隔
        //    if (ts.Milliseconds > 50)                           //判断时间间隔，如果时间间隔大于50毫秒，则将TextBox清空
        //        textBox1.Text = "";
        //    dt = tempDt ;
        //}

        void BarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode)
        {
            ShowInfo(barCode);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            BarCode.Start();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            BarCode.Stop();
        }


        private void textBox6_TextChanged(object sender, EventArgs e)

        {
            if (textBox6.Text.Length > 0)
            {
                MessageBox.Show("条码长度：" + textBox6.Text.Length + "\n条码内容：" + textBox6.Text, "系统提示");
            }
        }
    }
}
