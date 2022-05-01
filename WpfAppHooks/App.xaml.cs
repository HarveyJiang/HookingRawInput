using Linearstar.Windows.RawInput;

using RawInput.Sharp.SimpleExample;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppHooks
{


    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        [DllImport("user32", EntryPoint = "ToAscii")]
        private static extern bool ToAscii(int VirtualKey, int ScanCode, byte[] lpKeyState, ref uint lpChar, int uFlags);


        [DllImport("user32.dll")]
        public static extern bool ToUnicode(int virtualKeyCode, int scanCode, byte[] keyboardState, [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)] StringBuilder receivingBuffer, int bufferSize, uint flags);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(uint nVirtKey);

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32", EntryPoint = "GetKeyNameText")]
        private static extern int GetKeyNameText(int IParam, StringBuilder lpBuffer, int nSize);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        private const uint MAPVK_VK_TO_CHAR = 0x02;

        private const uint VK_SHIFT = 0x10;

        private const uint MAPVK_VK_TO_VSC = 0x00;

        // KeyboardListener KListener = new KeyboardListener();
        private string result = "";
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // KeyboardListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
            //var window = new RawInputReceiverWindow();
            //window.Input += Window_Input;
            //RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard, RawInputDeviceFlags.ExInputSink | RawInputDeviceFlags.NoLegacy, window.Handle);
            //System.Windows.Forms.Application.Run();
            
        }

        private void Window_Input(object sender, RawInputEventArgs e)
        {
            var data = e.Data as RawInputKeyboardData;
            var kb = data.Keyboard;
            var d = e.Data.Device;
            if (kb.Flags == Linearstar.Windows.RawInput.Native.RawKeyboardFlags.None)
            {
                // Log(new { Keyboard = kb });

                char c = (char)MapVirtualKey((uint)kb.ScanCode, (uint)2);

                // Log(new { MapVirtualKey = c });

                // long keyCode = key & 0xffff;

                var keyCode = kb.VirutalKey;


                long scanCode = kb.ScanCode; // MapVirtualKey((uint)keyCode, MAPVK_VK_TO_VSC);

                // shift the scancode to the high word
                //scanCode = (scanCode << 16); // | (1 << 24);
                //if (keyCode == 45 ||
                //    keyCode == 46 ||
                //    keyCode == 144 ||
                //    (33 <= keyCode && keyCode <= 40))
                //{
                //    // add the extended key flag
                //    scanCode |= 0x1000000;
                //}

                //var sb = new StringBuilder(256);

                //GetKeyNameText((int)kb.ScanCode, sb, 256);

                //Log(new { GetKeyNameText = sb.ToString() });

                result += KeyCodeToUnicode(kb.VirutalKey, kb.ScanCode);
                if (kb.VirutalKey == 13)
                {

                    Console.WriteLine("end");
                    result = "";
                }
            }

            // 20 capslook enter  a:65
            // 16 shift 
            // 13 enter

            uint uKey = 0;
            if (ToAscii(kb.VirutalKey, kb.ScanCode, data.ToStructure(), ref uKey, (int)kb.Flags))
            {
                var AscII = uKey;
                var Chr = Convert.ToChar(uKey);
                Console.WriteLine($"ascii:{AscII},char:{Chr}");
                // Log(new { ascii = uKey, cahr = Chr });
            }
            Console.WriteLine(data);

        }

        public static string KeyCodeToUnicode(int virtualKeyCode, int scanCode)
        {
            byte[] keyboardState = new byte[255];
            bool keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return "";
            }

            // uint virtualKeyCode = (uint)key;
            //uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            //IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            ToUnicode(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0);

            return result.ToString();
        }


        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //KListener.Dispose();
            //RawInputDevice.UnregisterDevice(HidUsageAndPage.Keyboard);
        }
    }
}
