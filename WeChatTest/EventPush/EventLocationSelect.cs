using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.EventPush
{
    public class EventLocationSelect : EventBase
    {
        #region 属性

        /// <summary>
        /// X坐标信息
        /// </summary>
        public double Location_X { get; set; }

        /// <summary>
        /// Y坐标信息
        /// </summary>
        public double Location_Y { get; set; }

        /// <summary>
        /// 精度，可理解为精度或者比例尺、越精细的话 scale越高
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 地理位置的字符串信息
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 朋友圈POI的名字，可能为空
        /// </summary>
        public string Poiname { get; set; }

        #endregion

        public EventLocationSelect(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            Location_X = Convert.ToDouble(xmlDoc.SelectSingleNode("xml/SendLocationInfo/Location_X").InnerText);
            Location_Y = Convert.ToDouble(xmlDoc.SelectSingleNode("xml/SendLocationInfo/Location_Y").InnerText);
            Scale = Convert.ToInt32(xmlDoc.SelectSingleNode("xml/SendLocationInfo/Scale").InnerText);
            Label = xmlDoc.SelectSingleNode("xml/SendLocationInfo/Label").InnerText;
            Poiname = xmlDoc.SelectSingleNode("xml/SendLocationInfo/Poiname").InnerText;
        }
    }
}