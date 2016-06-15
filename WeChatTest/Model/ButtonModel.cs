using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeChatTest.Enum;

namespace WeChatTest.Model
{
    public class ButtonModel
    {
        /// <summary>
        /// 按钮类型
        /// click：点击推事件
        /// view：跳转URL
        /// scancode_push：扫码推事件
        /// scancode_waitmsg：扫码推事件且弹出“消息接收中”提示框
        /// pic_sysphoto：弹出系统拍照发图
        /// pic_photo_or_album：弹出拍照或者相册发图
        /// pic_weixin：弹出微信相册发图器
        /// location_select：弹出地理位置选择器
        /// media_id：下发消息（除文本消息）
        /// view_limited：跳转图文消息URL
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 菜单KEY值，用于消息接口推送，不超过128字节
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 对于不同的菜单类型，value的值意义不同。
        /// Text:保存文字到value； 
        /// Img、voice：保存mediaID到value； 
        /// Video：保存视频下载链接到value； 
        /// News：保存图文消息到news_info，同时保存mediaID到value； 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 菜单标题，不超过16个字节，子菜单不超过40个字节
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 网页链接，用户点击菜单可打开链接，不超过1024字节
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 调用新增永久素材接口返回的合法media_id
        /// </summary>
        public string Media_Id { get; set; }
    }
}