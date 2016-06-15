using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.EventPush
{
    public class EventView : EventBase
    {
        #region 属性

        /// <summary>
        /// 指菜单ID，如果是个性化菜单，则可以通过这个字段，知道是哪个规则的菜单被点击了。
        /// </summary>
        public string MenuId { get; set; }

        #endregion

        public EventView(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            MenuId = xmlDoc.SelectSingleNode("xml/MenuId").InnerText;
        }
    }
}