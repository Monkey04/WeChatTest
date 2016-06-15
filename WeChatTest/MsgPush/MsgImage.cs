using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.MsgPush
{
    public class MsgImage : MsgBase
    {
        #region 属性

        /// <summary>
        /// 图片链接地址
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        /// 图片消息媒体id
        /// </summary>
        public string MediaId { get; set; }

        #endregion

        public MsgImage(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            PicUrl = xmlDoc.SelectSingleNode("xml/PicUrl").InnerText;
            MediaId = xmlDoc.SelectSingleNode("xml/MediaId").InnerText;
        }

        /// <summary>
        /// 被动回复图片消息
        /// </summary>
        /// <param name="mediaId">图片素材Id</param>
        /// <returns></returns>
        public string BuildResponseXml(string mediaId)
        {
            return BuildResponseXml(FromUserName, ToUserName, mediaId);
        }

        /// <summary>
        /// 被动回复图片消息
        /// </summary>
        /// <param name="toUserName">接收方帐号（收到的OpenID）</param>
        /// <param name="fromUserName">开发者微信号</param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public static string BuildResponseXml(string toUserName, string fromUserName, string mediaId)
        {
            string xmlTemplate = @"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[image]]></MsgType>
                                    <Image>
                                      <MediaId><![CDATA[{3}]]></MediaId>
                                    </Image>
                                   </xml>";
            string xml = string.Format(xmlTemplate, toUserName, fromUserName, DateTime.Now.DateTimeToTimeStamp(), mediaId);
            Tools.LogMsg(xml);
            return xml;
        }
    }
}