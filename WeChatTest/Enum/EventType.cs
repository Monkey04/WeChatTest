using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Enum
{
    /// <summary>
    /// 事件推送类型
    /// </summary>
    public enum EventType
    {
        ClICK,
        LOCATION,
        SUBSCRIBE,
        UNSUBSCRIBE,
        SCANCODE_PUSH,
        SCANCODE_WAITMSG,
        PIC_SYSPHOTO,
        PIC_PHOTO_OR_ALBUM,
        PIC_WEIXIN,
        LOCATION_SELECT
    }
}