using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

using WPFRawInput;

using Application = System.Windows.Application;

namespace WindowsApplication1
{
    // https://docs.microsoft.com/zh-cn/windows/win32/inputdev/using-raw-input?redirectedfrom=MSDN#buffered_read
    public partial class Window1 : Window
    {

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        RawStuff.InputDevice id;
        int NumberOfKeyboards;
        Message message = new Message();

        public Window1()
        {
            Activate();
        }

        private void _KeyPressed(object sender, RawStuff.InputDevice.KeyControlEventArgs e)
        {
            //string[] tokens = e.Keyboard.Name.Split(';');
            //string token = tokens[1];

            lbHandle.Content = e.Keyboard.deviceHandle.ToString();
            lbType.Content = e.Keyboard.deviceType;
            lbName.Content = e.Keyboard.deviceName;
            lbKey.Content = e.Keyboard.key.ToString();
            lbVKey.Content = e.Keyboard.vKey;
            lbDescription.Content = e.Keyboard.Name;
            lbNumKeyboards.Content = NumberOfKeyboards.ToString();
        }

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (id != null)
            {
                // I could have done one of two things here.
                // 1. Use a Message as it was used before.
                // 2. Changes the ProcessMessage method to handle all of these parameters(more work).
                //    I opted for the easy way.

                //Note: Depending on your application you may or may not want to set the handled param.

                message.HWnd = hwnd;
                message.Msg = msg;
                message.LParam = lParam;
                message.WParam = wParam;

                // handled = true;

                id.ProcessMessage(message, ref handled);

                id.ActiveCurrentWin += ActiveWin;
            }

            //IntPtr ptr1 = Marshal.AllocHGlobal(sizeof(int));
            //Marshal.WriteInt32(ptr1, 1);

            return IntPtr.Zero;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            // I am new to WPF and I don't know where else to call this function.
            // It has to be called after the window is created or the handle won't
            // exist yet and the function will throw an exception.
            StartWndProcHandler();

            base.OnSourceInitialized(e);
        }

        void StartWndProcHandler()
        {
            IntPtr hwnd = IntPtr.Zero;
            Window myWin = Application.Current.MainWindow;

            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            try
            {
                hwnd = new WindowInteropHelper(myWin).Handle;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //ScanerHook scanerHook = new ScanerHook();

            //scanerHook.Start();

            //Get the Hwnd source   
            HwndSource source = HwndSource.FromHwnd(hwnd);
            //Win32 queue sink
            source.AddHook(WndProc);
            id = new RawStuff.InputDevice(hwnd);
            NumberOfKeyboards = id.EnumerateDevices();
            // id.KeyPressed += new RawStuff.InputDevice.DeviceEventHandler(_KeyPressed);
        }

        void CloseMe(object sender, RoutedEventArgs e)
        {
            Close();
        }


        public void ActiveWin()
        {


            // Log($"{DateTime.Now} active win..");
             

            if (!this.IsVisible)
            {
                this.Show();
            }

            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }

            this.Activate();

            this.Topmost = true;  // important
            this.Topmost = false; // important

            //label3.Focus();
            //this.Focus();
            tbtest.Focus();
            //this.Show();
            //this.Activate();
            //SetForegroundWindow(new WindowInteropHelper(this).Handle);

        }
        public void Log(object obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            using (StreamWriter fs = new StreamWriter("log.txt", true))
            {
                fs.WriteLineAsync(content);
            }


        }
    }


}