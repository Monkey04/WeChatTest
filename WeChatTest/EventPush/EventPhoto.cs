using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.EventPush
{
    public class EventPhoto : EventBase
    {
        #region 属性

        /// <summary>
        /// 发送的图片数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 图片的MD5值，开发者若需要，可用于验证接收到图片
        /// </summary>
        public List<string> PicMd5SumList { get; set; }

        #endregion

        public EventPhoto(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            Count = Convert.ToInt32(xmlDoc.SelectSingleNode("xml/SendPicsInfo/Count").InnerText);
            PicMd5SumList = new List<string>();
            foreach (XmlNode node in xmlDoc.SelectSingleNode("xml/SendPicsInfo/PicList/item"))
            {
                PicMd5SumList.Add(node.SelectSingleNode("PicMd5Sum").InnerText);
            }
        }
    }
}