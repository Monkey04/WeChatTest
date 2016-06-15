using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.MsgPush
{
    public class MsgVideo : MsgBase
    {
        #region 属性

        /// <summary>
        /// 视频媒体Id
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 视频消息缩略图的媒体id
        /// </summary>
        public string ThumbMediaId { get; set; }

        #endregion

        public MsgVideo(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            MediaId = xmlDoc.SelectSingleNode("xml/MediaId").InnerText;
            ThumbMediaId = xmlDoc.SelectSingleNode ("xml/ThumbMediaId").InnerText;
        }

        /// <summary>
        /// 被动回复视频信息
        /// </summary>
        /// <param name="mediaId">视频素材Id</param>
        /// <param name="title">视频标题</param>
        /// <param name="description">视频描述</param>
        /// <returns></returns>
        public string BuildResponseXml(string mediaId, string title, string description)
        {
            return BuildResponseXml(FromUserName, ToUserName, mediaId, title, description);
        }

        /// <summary>
        /// 被动回复视频信息
        /// </summary>
        /// <param name="toUserName">接收方帐号（收到的OpenID）</param>
        /// <param name="fromUserName">开发者微信号</param>
        /// <param name="mediaId">视频素材Id</param>
        /// <param name="title">视频标题</param>
        /// <param name="description">视频描述</param>
        /// <returns></returns>
        public static string BuildResponseXml(string toUserName, string fromUserName, string mediaId, string title, string description)
        {
            string xmlTemplate = @"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[video]]></MsgType>
                                    <Video>
                                      <MediaId><![CDATA[{3}]]></MediaId>
                                      <Title><![CDATA[{4}]]></Title>
                                      <Description><![CDATA[{5}]]></Description>
                                    </Video> 
                                   </xml>";
            string xml = string.Format(xmlTemplate, toUserName, fromUserName, DateTime.Now.DateTimeToTimeStamp(), mediaId, title, description);
            Tools.LogMsg(xml);
            return xml;
        }
    }
}