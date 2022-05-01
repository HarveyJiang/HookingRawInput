using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Huitai.Cssd.Common.Util.Barcode
{
    /// <summary>
    /// 扫码枪配置集合
    /// </summary>
    [SuppressMessage(
        "Microsoft.Design",
        "CA1010",
        Justification = "This is just part of the design of the configuration classes.")]
    public class BarcodeScannerListenerConfigurationElementCollection : 
        ConfigurationElementCollection
    {
        /// <summary>
        /// 创建新的BarcodeScannerListenerConfigurationElement
        /// </summary>
        /// <returns>ConfigurationElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new BarcodeScannerListenerConfigurationElement();
        }

        /// <summary>
        /// 获取BarcodeScannerListenerConfigurationElement的ID
        /// </summary>
        /// <param name="element">ConfigurationElement</param>
        /// <returns>BarcodeScannerListenerConfigurationElement的ID</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            BarcodeScannerListenerConfigurationElement myElement =
                (BarcodeScannerListenerConfigurationElement)element;

            return myElement.Id;
        }
    }
}
