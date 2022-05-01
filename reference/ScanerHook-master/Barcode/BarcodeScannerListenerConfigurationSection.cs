
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Huitai.Cssd.Common.Util.Barcode
{
    /// <summary>
    /// 扫码枪监听器的configuration section配置，需要配置到App.config
    /// </summary>
    public class BarcodeScannerListenerConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// 设置扫码枪的硬件ID
        /// </summary>
        [ConfigurationProperty("hardwareIds", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(BarcodeScannerListenerConfigurationElementCollection))]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2227",
            Justification = "The setter is required for the configuration classes to deserialize.")]
        public BarcodeScannerListenerConfigurationElementCollection HardwareIds
        {
            get { return this["hardwareIds"] as BarcodeScannerListenerConfigurationElementCollection; }
            set { this["hardwareIds"] = value; }
        }

        /// <summary>
        /// 获取configuration section
        /// </summary>
        /// <returns>配置信息</returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1024",
            Justification = "The method call conveys that this is an expensive operation that may fail.")]
        public static BarcodeScannerListenerConfigurationSection GetConfiguration()
        {
            return ConfigurationManager.GetSection("barcodeScanner") as
                BarcodeScannerListenerConfigurationSection;
        }
    }
}
