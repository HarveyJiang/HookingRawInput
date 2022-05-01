using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppQR
{
    class BarCodeHook
    {
        public delegate void BarCodeDelegate(BarCodes barCode);
        public event BarCodeDelegate BarCodeEvent;
        public struct BarCodes
        {
            public int VirtKey;  //虚拟码 
            public int ScanCode;  //扫描码 
            public string KeyName; //键名 
            public uint AscII;  //AscII 
            public char Chr;   //字符
            public string BarCode; //条码信息 
            public bool IsValid;  //条码是否有效 
            public DateTime Time; //扫描时间 
        }
        private struct EventMsg
        {
            public int message;
            public int paramL;
            public int paramH;
            public int Time;
            public int hwnd;
        }
        // 安装钩子 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        // 卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);
        // 继续下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        //获取键名的字符串
        [DllImport("user32", EntryPoint = "GetKeyNameText")]
        private static extern int GetKeyNameText(int lParam, StringBuilder lpBuffer, int nSize);
        //将256个虚拟键复制到指定的缓冲区中
        [DllImport("user32", EntryPoint = "GetKeyboardState")]
        private static extern int GetKeyboardState(byte[] pbKeyState);
        //将指定的虚拟键码和键盘状态为相应的字符串
        [DllImport("user32", EntryPoint = "ToAscii")]
        private static extern bool ToAscii(int VirtualKey, int ScanCode, byte[] lpKeyState, ref uint lpChar, int uFlags);
        //声明定义回调函数
        delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        BarCodes barCode = new BarCodes();
        int hKeyboardHook = 0;
        string strBarCode = "";
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
{
           

            if (nCode == 0)
            {
                EventMsg msg = (EventMsg)Marshal.PtrToStructure(lParam, typeof(EventMsg));

                if (wParam == 0x100) //WM_KEYDOWN = 0x100
                {
                    var s = JsonConvert.SerializeObject(msg);
                    Log(msg);
                    barCode.VirtKey = msg.message & 0xff; //虚拟码 
                    barCode.ScanCode = msg.paramL & 0xff; //扫描码
                    StringBuilder strKeyName = new StringBuilder(255);
                    if (GetKeyNameText(barCode.ScanCode * 65536, strKeyName, 255) > 0)
                    {
                        barCode.KeyName = strKeyName.ToString().Trim(new char[] { ' ', '\0' });
                    }
                    else
                    {
                        barCode.KeyName = "";
                    }
                    byte[] kbArray = new byte[256];
                    uint uKey = 0;
                    GetKeyboardState(kbArray);
                    if (ToAscii(barCode.VirtKey, barCode.ScanCode, kbArray, ref uKey, 0))
                    {
                        barCode.AscII = uKey;
                        barCode.Chr = Convert.ToChar(uKey);
                    }
                    if (DateTime.Now.Subtract(barCode.Time).TotalMilliseconds > 50)
                    {
                        strBarCode = barCode.Chr.ToString();
                    }
                    else
                    {
                        if ((msg.message & 0xff) == 13 && strBarCode.Length > 3)
                        //回车
                        {
                            barCode.BarCode = strBarCode;
                            barCode.IsValid = true;

                            Log(barCode);
                        }
                        strBarCode += barCode.Chr.ToString();
                        
                    }
                    barCode.Time = DateTime.Now;
                    if (BarCodeEvent != null) BarCodeEvent(barCode);
                    //触发事件 
                    barCode.IsValid = false;
                }
            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
        // 安装钩子 
        public bool Start()
        {
            if (hKeyboardHook == 0)
            {
                //WH_KEYBOARD_LL = 13 
                hKeyboardHook = SetWindowsHookEx(13, new HookProc(KeyboardHookProc), Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            }
            return (hKeyboardHook != 0);
        }
        // 卸载钩子 
        public bool Stop()
        {
            if (hKeyboardHook != 0)
            {
                return UnhookWindowsHookEx(hKeyboardHook);
            }
            return true;
        }

        public void Log(object obj)
        {
            using (StreamWriter fs = new StreamWriter("log.txt", true))
            {
                fs.WriteLineAsync(JsonConvert.SerializeObject(obj));
            }
        }
    }


    public struct MSG
    {
        public Point p;
        public IntPtr HWnd;
        public uint wHitTestCode;
        public int dwExtraInfo;
    }
    public struct KeyMSG
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }
}
