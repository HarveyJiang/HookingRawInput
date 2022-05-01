using System.Configuration;

namespace Huitai.Cssd.Common.Util.Barcode
{
    /// <summary>
    /// 扫码配置element对象
    /// </summary>
    public class BarcodeScannerListenerConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// ID属性
        /// </summary>
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        public string Id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }
    }
}
