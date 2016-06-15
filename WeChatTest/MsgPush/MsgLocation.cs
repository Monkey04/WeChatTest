using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.MsgPush
{
    public class MsgLocation : MsgBase
    {
        #region 属性

        /// <summary>
        /// 地理位置维度
        /// </summary>
        public double Location_X { get; set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Location_Y { get; set; }

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; set; }

        #endregion

        public MsgLocation(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            Location_X = Convert.ToDouble(xmlDoc.SelectSingleNode("xml/Location_X").InnerText);
            Location_Y = Convert.ToDouble(xmlDoc.SelectSingleNode("xml/Location_Y").InnerText);
            Scale = Convert.ToInt32(xmlDoc.SelectSingleNode("xml/Scale").InnerText);
            Label = xmlDoc.SelectSingleNode("xml/Label").InnerText;
        }
    }
}