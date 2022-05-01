using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using RawInput_dll;


namespace Keyboard
{
    public partial class Keyboard : Form
    {
        private readonly RawInput _rawinput;

        public Keyboard()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _rawinput = new RawInput(Handle);
            _rawinput.CaptureOnlyIfTopMostWindow = true;    // Otherwise default behavior is to capture always
            _rawinput.AddMessageFilter();                   // Adding a message filter will cause keypresses to be handled
            _rawinput.KeyPressed += OnKeyPressed;           

            Win32.DeviceAudit();                            // Writes a file DeviceAudit.txt to the current directory
        }

        private void OnKeyPressed(object sender, InputEventArg e)
        {
            lbHandle.Text = e.KeyPressEvent.DeviceHandle.ToString();
            lbType.Text = e.KeyPressEvent.DeviceType;
            lbName.Text = e.KeyPressEvent.DeviceName;
            lbDescription.Text = e.KeyPressEvent.Name;
            lbKey.Text = e.KeyPressEvent.VKey.ToString(CultureInfo.InvariantCulture);
            lbNumKeyboards.Text = _rawinput.NumberOfKeyboards.ToString(CultureInfo.InvariantCulture);
            lbVKey.Text = e.KeyPressEvent.VKeyName;
            lbSource.Text = e.KeyPressEvent.Source;
            lbKeyPressState.Text = e.KeyPressEvent.KeyPressState;
            lbMessage.Text = string.Format("0x{0:X4} ({0})", e.KeyPressEvent.Message);
           
            switch (e.KeyPressEvent.Message)
            {
                case Win32.WM_KEYDOWN:
                    Debug.WriteLine(e.KeyPressEvent.KeyPressState);
                    break;
                 case Win32.WM_KEYUP:
                    Debug.WriteLine(e.KeyPressEvent.KeyPressState);
                    break;
            }
        }

        private void Keyboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            _rawinput.KeyPressed -= OnKeyPressed;
        }

        private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (null == ex) return;

            // Log this error. Logging the exception doesn't correct the problem but at least now
            // you may have more insight as to why the exception is being thrown.
            Debug.WriteLine("Unhandled Exception: " + ex.Message);
            Debug.WriteLine("Unhandled Exception: " + ex);
            MessageBox.Show(ex.Message);
        }

    }
}
