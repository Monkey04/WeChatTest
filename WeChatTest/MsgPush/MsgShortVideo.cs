using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.MsgPush
{
    public class MsgShortVideo : MsgBase
    {
        #region 属性

        /// <summary>
        /// 小视频媒体Id
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 小视频消息缩略图的媒体id
        /// </summary>
        public string ThumbMediaId { get; set; }

        #endregion

        public MsgShortVideo(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            MediaId = xmlDoc.SelectSingleNode("xml/MediaId").InnerText;
            ThumbMediaId = xmlDoc.SelectSingleNode("xml/ThumbMediaId").InnerText;
        }

    }
}