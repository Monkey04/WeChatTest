using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.MsgPush
{
    public class MsgVoice : MsgBase
    {
        #region 属性

        /// <summary>
        /// 音频媒体Id
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public string Format { get; set; }

        #endregion

        public MsgVoice(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            MediaId = xmlDoc.SelectSingleNode("xml/MediaId").InnerText;
            Format = xmlDoc.SelectSingleNode("xml/Format").InnerText;
        }

        /// <summary>
        /// 被动回复音频信息
        /// </summary>
        /// <param name="mediaId">音频素材Id</param>
        /// <returns></returns>
        public string BuilResponseXml(string mediaId)
        {
            return BuilResponseXml(FromUserName, ToUserName, mediaId);
        }

        /// <summary>
        /// 被动回复音频信息
        /// </summary>
        /// <param name="toUserName">接收方帐号（收到的OpenID）</param>
        /// <param name="fromUserName">开发者微信号</param>
        /// <param name="mediaId">音频素材Id</param>
        /// <returns></returns>
        public static string BuilResponseXml(string toUserName, string fromUserName, string mediaId)
        {
            string xmlTemplate = @"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[voice]]></MsgType>
                                    <Voice>
                                       <MediaId><![CDATA[{3}]]></MediaId>
                                    </Voice>
                                  </xml>";
            string xml = string.Format(xmlTemplate, toUserName, fromUserName, DateTime.Now.DateTimeToTimeStamp(), mediaId);
            Tools.LogMsg(xml);
            return xml;
        }
    }
}