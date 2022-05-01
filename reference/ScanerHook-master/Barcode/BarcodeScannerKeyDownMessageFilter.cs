using System.Security.Permissions;
using System.Windows.Forms;

namespace Huitai.Cssd.Common.Util.Barcode
{
    /// <summary>
    /// WM_KEYDOWN消息过滤器
    /// </summary>
    public class BarcodeScannerKeyDownMessageFilter : IMessageFilter
    {
        /// <summary>

        /// 表示下一个按键信息是否需要被过滤
        /// </summary>
        public bool FilterNext
        {
            get;
            set;
        }

        /// <summary>
        /// 在按键消息分发之前，进行过滤
        /// </summary>
        /// <param name="m">被过滤的消息，此参数不能被修改</param>
        /// <returns>
        /// 如果返回true就阻止此消息被分发；如果是false，允许消息被继续分发
        /// </returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool PreFilterMessage(ref Message m)
        {
            bool filter = false;

            if (this.FilterNext && m.Msg == NativeMethods.WM_KEYDOWN)
            {
                filter = true;
                this.FilterNext = false;
            }

            return filter;
        }
    }
}
