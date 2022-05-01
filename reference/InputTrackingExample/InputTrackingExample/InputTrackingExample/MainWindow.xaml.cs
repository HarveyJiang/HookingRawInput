using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace InputTrackingExample
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private AllInputSources lastInput;
        private KeyboardInput keyboard;
        private MouseInput mouse;

        public MainWindow()
        {
            InitializeComponent();

            keyboard = new KeyboardInput();
            keyboard.KeyBoardKeyPressed += keyboard_KeyBoardKeyPressed;

            mouse = new MouseInput();
            mouse.MouseMoved += mouse_MouseMoved;

            lastInput = new AllInputSources();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void keyboard_KeyBoardKeyPressed(object sender, EventArgs e)
        {
            keyboardTime.Content = FormatDateTime(DateTime.Now);
        }

        private string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss fff", CultureInfo.CurrentUICulture);
        }
        void mouse_MouseMoved(object sender, EventArgs e)
        {
            mouseTime.Content = FormatDateTime(DateTime.Now);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lastInputTime.Content = FormatDateTime(lastInput.GetLastInputTime());
        }
    }
}
