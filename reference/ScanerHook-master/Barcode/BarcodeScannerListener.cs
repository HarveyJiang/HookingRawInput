using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Xml;
using BarcodeSample.Model;
using BarcodeSample.UI.WinForms;
using BarcodeSample.UI.WinForms.Interop;
using Application = System.Windows.Application;

namespace Huitai.Cssd.Common.Util.Barcode
{
    /// <summary>
    /// 扫码监听器，使用windows原生的Input API，对指定的设备的输入信息进行监听。
    /// 使用这种方法，就可以实现无需将输入焦点点击到文本框上而取得输入的内容
    /// </summary>
    public class BarcodeScannerListener
    {
        /// <summary>
        /// 设备信息字典
        /// </summary>
        private Dictionary<IntPtr, BarcodeScannerDeviceInfo> devices;

        /// <summary>
        /// The WM_KEYDOWN filter.
        /// WM_KEYDOWN过滤器
        /// </summary>
        private BarcodeScannerKeyDownMessageFilter filter;

        /// <summary>
        /// The barcode currently being read.
        /// 当前被读取的条码字符串
        /// </summary>
        private StringBuilder keystrokeBuffer;

        /// <summary>
        /// interop helper.
        /// </summary>
        private BarcodeScannerListenerInteropHelper interopHelper =
            new BarcodeScannerListenerInteropHelper();

        /// <summary>
        /// Event fired when a barcode is scanned.
        /// 条码扫描所触发的事件
        /// </summary>
        public event EventHandler BarcodeScanned;

        

        private bool _ControlHandled;


        /// <summary>
        /// 将监听器附着到窗体上
        /// </summary>
        /// <param name="form">需要附着的窗体（WPF）</param>
        public void Attach(Window form)
        {
            var helper = new WindowInteropHelper(form);
            IntPtr hwnd = helper.Handle;
            form.KeyDown += (sender, args) =>
            {
                if (_ControlHandled)
                {
                    args.Handled = true;
                    _ControlHandled = false;
                }
            };
            DoAttach(hwnd);
        }

        /// <summary>
        /// 将监听器附着到窗体上
        /// </summary>
        /// <param name="form">需要附着的窗体（WinForm）</param>
        public void Attach(System.Windows.Controls.Control control)
        {

            //var helper = new WindowInteropHelper(Window.GetWindow(control));
            var fromVisual = (HwndSource)PresentationSource.FromVisual(control);
            if (fromVisual != null)
            {
                IntPtr handle = fromVisual.Handle;
                DoAttach(handle);
            }

        }


        /// <summary>
        /// 监听绑定
        /// </summary>
        /// <param name="hwnd">设备指针</param>
        private void DoAttach(IntPtr hwnd)
        {

            this.keystrokeBuffer = new StringBuilder();

            this.InitializeBarcodeScannerDeviceHandles();
            this.interopHelper.HookRawInput(hwnd);
            //this.HookHandleEvents(form);

            //this.AssignHandle(ptr);

            this.filter = new BarcodeScannerKeyDownMessageFilter();
            ComponentDispatcher.ThreadFilterMessage -= ComponentDispatcher_ThreadFilterMessage;
            ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
            //Application.AddMessageFilter(this.filter);
        }



        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            switch (msg.message)
            {
                case NativeMethods.WM_INPUT:
                    if (ProcessRawInputMessage(msg.lParam))
                    {
                        filter.FilterNext = true;
                        handled = true;
                        _ControlHandled = true;
                    }

                    break;
            }
        }

       

        /// <summary>
        /// 触发扫码事件
        /// </summary>
        /// <param name="deviceInfo">扫码设备信息</param>
        private void FireBarcodeScanned(BarcodeScannerDeviceInfo deviceInfo)
        {
            string barcode;
            EventHandler handler;

            barcode = this.keystrokeBuffer.ToString();

            if (barcode.Length > 0)
            {
                handler = this.BarcodeScanned;

                this.keystrokeBuffer = new StringBuilder();

                if (handler != null)
                {
                    handler(this, new BarcodeScannedEventArgs(barcode, deviceInfo));
                }
            }
        }


        /// <summary>
        /// 初始化扫码枪设备Initializes the barcode scanner device handles.
        /// </summary>
        private void InitializeBarcodeScannerDeviceHandles()
        {
            BarcodeScannerListenerConfigurationSection config;
            BarcodeScannerListenerConfigurationElementCollection hardwareIdsConfig;
            IEnumerable<string> hardwareIds = null;

            //从Ap.config配置中取
            config = BarcodeScannerListenerConfigurationSection.GetConfiguration();
            hardwareIdsConfig = config.HardwareIds;
            hardwareIds = from hardwareIdConfig in hardwareIdsConfig.Cast<BarcodeScannerListenerConfigurationElement>()
                select hardwareIdConfig.Id;
            
            this.devices = this.interopHelper.InitializeBarcodeScannerDeviceHandles(hardwareIds);
        }



        /// <summary>
        /// 处理WM_INPUT消息
        /// </summary>
        /// <param name="rawInputHeader">rawInputHeader的指针</param>
        /// <returns>按键是否被处理</returns>
        private bool ProcessRawInputMessage(IntPtr rawInputHeader)
        {
            BarcodeScannerDeviceInfo deviceInfo;
            bool handled;
            bool keystroke;
            string localBuffer;
            IntPtr rawInputDeviceHandle;

            handled = false;
            keystroke = false;
            localBuffer = string.Empty;
            rawInputDeviceHandle = IntPtr.Zero;

            this.interopHelper.GetRawInputInfo(
                rawInputHeader,
                ref rawInputDeviceHandle,
                ref keystroke,
                ref localBuffer);

            if (this.devices.TryGetValue(rawInputDeviceHandle, out deviceInfo) && keystroke)
            {
                handled = true;
                // 这里判断的是Tab按键，可以更换为其他按键
                if (localBuffer.Length == 1 && (localBuffer[0] == 0x09 || localBuffer[0] == '\t'))
                {
                    this.FireBarcodeScanned(deviceInfo);
                }
                else
                {
                    this.keystrokeBuffer.Append(localBuffer);
                }
            }

            return handled;
        }
    }
}