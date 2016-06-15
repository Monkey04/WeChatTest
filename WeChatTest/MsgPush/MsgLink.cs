using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.MsgPush
{
    public class MsgLink : MsgBase
    {
        #region 属性

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 消息链接
        /// </summary>
        public string Url { get; set; }

        #endregion

        public MsgLink(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            Title = xmlDoc.SelectSingleNode("xml/Title").InnerText;
            Description = xmlDoc.SelectSingleNode("xml/Description").InnerText;
            Url = xmlDoc.SelectSingleNode("xml/Url").InnerText;
        }
    }
}