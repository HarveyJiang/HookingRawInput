

using System;
using BarcodeSample.Model;

namespace Huitai.Cssd.Common.Util.Barcode
{
    /// <summary>
    /// 扫码事件类，包含了扫码必要的信息
    /// </summary>
    public class BarcodeScannedEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化BarcodeScannedEventArgs实例
        /// Initializes a new instance of the BarcodeScannedEventArgs class.
        /// </summary>
        /// <param name="barcode">扫码内容</param>
        /// <param name="deviceInfo">扫码设备信息</param>
        /// <exception cref="ArgumentNullException">barcode或deviceInfo为null抛出异常</exception>
        public BarcodeScannedEventArgs(string barcode, BarcodeScannerDeviceInfo deviceInfo)
        {
            if (barcode == null)
            {
                throw new ArgumentNullException("barcode");
            }

            if (deviceInfo == null)
            {
                throw new ArgumentNullException("deviceInfo");
            }

            this.Barcode = barcode;
            this.DeviceInfo = deviceInfo;
        }

        /// <summary>
        /// 获得扫码内容
        /// </summary>
        public string Barcode { get; private set; }

        /// <summary>
        /// 获得扫码设备信息
        /// </summary>
        public BarcodeScannerDeviceInfo DeviceInfo { get; private set; }
    }
}