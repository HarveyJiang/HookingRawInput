using BarcodeScanner;

using Linearstar.Windows.RawInput;

using Newtonsoft.Json;

using RawInput.Sharp.SimpleExample;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppHooks
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWINPUTDEVICE
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort usUsagePage;
        [MarshalAs(UnmanagedType.U2)]
        public ushort usUsage;
        [MarshalAs(UnmanagedType.U4)]
        public int dwFlags;
        public IntPtr hwndTarget;
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
      

        //定义成静态，这样不会抛出回收异常
        private static HookProc hookproc;

        delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);


        [DllImport("USER32", SetLastError = true)]
        static extern IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //设置钩子
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //卸载钩子
        private static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //继续下个钩子
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

        string hid = @"\\?\HID#VID_28E9&PID_0283#6&2c606dc8&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";

        [DllImport("kernel32.dll")]
        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        public static extern IntPtr GetModuleHandle(string name);


        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string fileName);

        ScanerHook scanerHook = new ScanerHook();
        public MainWindow()
        {
            InitializeComponent();

            // var window = new RawInputReceiverWindow();
            SourceInitialized += MainWindow_SourceInitialized;

            //if (PresentationSource.FromVisual(this) is HwndSource source)
            //{
            //    source.AddHook(WinProc);
            //}
            // window.Input += Window_Input;
            // var devices = RawInputDevice.GetDevices();

            // Keyboards will be returned as a RawInputKeyboard.
            // var keyboards = devices.OfType<RawInputKeyboard>();

            // RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard, RawInputDeviceFlags.ExInputSink | RawInputDeviceFlags.NoLegacy, window.Handle);
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {

            //var windowInteropHelper = new WindowInteropHelper(this);
            //var hwnd = windowInteropHelper.Handle;

            //var mar = LoadLibrary("user32.dll");

            //RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];

            //rid[0].usUsagePage = 0x01;
            //rid[0].usUsage = 0x06;
            //rid[0].dwFlags = 0x00000100;
            //rid[0].hwndTarget = hwnd;// GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
            //                         //RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0]));
            //if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
            //{
            //    throw new ApplicationException("Failed to register raw input device(s).");
            //}


            //HwndSource source = HwndSource.FromHwnd(hwnd);
            //source.AddHook(new HwndSourceHook(Hook));
            //// Get the devices that can be handled with Raw Input.
            //// device = RawInputDevice.GetDevices().FirstOrDefault(d => d.DevicePath == hid);

            //// register the keyboard device and you can register device which you need like mouse
            //// RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard, RawInputDeviceFlags.ExInputSink | RawInputDeviceFlags.NoLegacy, hwnd);
            scanerHook.Start(IntPtr.Zero);


           // HwndSource source = HwndSource.FromHwnd(hwnd);
            //source.AddHook(new HwndSourceHook(Hook));

            
        }

        private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            const int WM_INPUT = 0x00FF;
           

            // You can read inputs by processing the WM_INPUT message.
            if (msg == WM_INPUT)
            {
                Log(new { order="=======", Hooklparam = lparam, wparam = wparam });
                // Create an RawInputData from the handle stored in lParam.
                var data = RawInputData.FromHandle(lparam);
                // You can identify the source device using Header.DeviceHandle or just Device.
                var sourceDeviceHandle = data.Header.DeviceHandle;
                var sourceDevice = data.Device;
                //if (sourceDevice.DevicePath == hid)
                //{
                // The data will be an instance of either RawInputMouseData, RawInputKeyboardData, or RawInputHidData.
                // They contain the raw input data in their properties.
                //Log(data);
                switch (data)
                {
                    case RawInputMouseData mouse:
                        Debug.WriteLine(mouse.Mouse);
                        break;
                    case RawInputKeyboardData keyboard:
                        Debug.WriteLine(keyboard.Keyboard);
                        break;
                    case RawInputHidData hid:
                        Debug.WriteLine(hid.Hid);
                        break;
                }
                //}
                //else
                //{
                //    // return CallNextHookEx(IntPtr.Zero, 0, wparam, lparam); ;
                //}
            }

            return IntPtr.Zero;
        }

        public void Log(object obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            using (StreamWriter fs = new StreamWriter("log.txt", true))
            {
                fs.WriteLineAsync(content);
            }

            lblMsg.Content = content+"\r\n";
        }

       

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // RawInputDevice.UnregisterDevice(HidUsageAndPage.Keyboard);
            scanerHook.Stop();
        }
    }
}
