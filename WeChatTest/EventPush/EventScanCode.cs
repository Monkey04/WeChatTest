using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.EventPush
{
    public class EventScanCode : EventBase
    {
        #region 属性

        /// <summary>
        /// 扫描类型，一般是qrcode
        /// </summary>
        public string ScanType { get; set; }

        /// <summary>
        /// 扫描结果，即二维码对应的字符串信息
        /// </summary>
        public string ScanResult { get; set; }

        #endregion

        public EventScanCode(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            ScanType = xmlDoc.SelectSingleNode("xml/ScanCodeInfo/ScanType").InnerText;
            ScanResult = xmlDoc.SelectSingleNode("xml/ScanCodeInfo/ScanResult").InnerText;
        }
    }
}