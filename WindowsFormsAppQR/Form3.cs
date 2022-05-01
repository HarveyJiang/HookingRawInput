using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppQR
{


    public partial class Form3 : Form
    {
        private ScanerHook listener = new ScanerHook();

        public Form3()
        {
            InitializeComponent();
            listener.ScanerEvent += Listener_ScanerEvent;
        }
        public static string RealUrl;


        private void Listener_ScanerEvent(ScanerHook.ScanerCodes codes)
        {
            RealUrl = codes.Result;
            label1.Text = $"{DateTime.Now}:{codes.Result}";
            //listener.Stop();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            listener.Start();
        }

        public string b64()
        {
            var AreaOfCarNumberList = new ObservableCollection<string>
            {
                "川",
                "鄂",
                "甘",
                "赣",
                "贵",
                "桂",
                "黑",
                "沪",
                "京",
                "津",
                "吉",
                "冀",
                "晋",
                "辽",
                "鲁",
                "蒙",
                "闽",
                "宁",
                "青",
                "琼",
                "陕",
                "苏",
                "皖",
                "湘",
                "新",
                "渝",
                "豫",
                "粤",
                "云",
                "浙",
                "藏"
            };

            byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(string.Join("", AreaOfCarNumberList));

            var encode = Convert.ToBase64String(bytes);
            return encode;
        }

    }
}
