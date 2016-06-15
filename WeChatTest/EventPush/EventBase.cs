using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using WeChatTest.Push;

namespace WeChatTest.EventPush
{
    public class EventBase : PushBase
    {
        #region 属性

        /// <summary>
        /// 事件KEY值
        /// 除了跳转链接时表示跳转的url外，都是自定义的菜单值
        /// </summary>
        public string EventKey { get; set; }

        #endregion

        public EventBase(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            EventKey = xmlDoc.SelectSingleNode("xml/EventKey").InnerText;
        }
    }
}