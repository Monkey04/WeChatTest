using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Enum
{
    /// <summary>
    /// 菜单按钮类型
    /// </summary>
    public enum ButtonType
    {
        CLICK,
        VIEW,
        SCANCODE_PUSH,
        SCANCODE_WAITMSG,
        PIC_SYSPHOTO,
        PIC_PHOTO_OR_ALBUM,
        PIC_WEIXIN,
        LOCATION_SELECT,
        MEDIA_ID,
        VIEW_LIMITED
    }
}