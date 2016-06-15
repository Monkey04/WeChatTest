using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest
{
    /// <summary>
    /// 消息推送类型
    /// </summary>
    public enum MsgType
    {
        TEXT,
        IMAGE,
        VOICE,
        VIDEO,
        SHORTVIDEO,
        LOCATION,
        LINK,
        EVENT
    }
}