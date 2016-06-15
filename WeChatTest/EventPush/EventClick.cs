using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.EventPush
{
    public class EventClick : EventBase
    {
        public EventClick(XmlDocument xmlDoc)
            : base(xmlDoc)
        {

        }
    }
}