using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.MsgPush
{
    public class MsgText : MsgBase
    {
        #region 属性

        /// <summary>
        /// 文本消息
        /// </summary>
        public string Content { get; set; }

        #endregion

        public MsgText(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            Content = xmlDoc.SelectSingleNode("xml/Content").InnerText;
        }

        /// <summary>
        /// 被动回复文本消息
        /// </summary>
        /// <param name="content">文本消息</param>
        /// <returns></returns>
        public string BuildResponseXml(string content)
        {
            return BuildResponseXml(FromUserName, ToUserName, content);
        }

        /// <summary>
        /// 被动回复文本消息
        /// </summary>
        /// <param name="toUserName">接收方帐号（收到的OpenID）</param>
        /// <param name="fromUserName">开发者微信号</param>
        /// <param name="content">文本消息</param>
        /// <returns></returns>
        public static string BuildResponseXml(string toUserName, string fromUserName, string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            string xmlTemplate = @"<xml>
                                     <ToUserName><![CDATA[{0}]]></ToUserName>
                                     <FromUserName><![CDATA[{1}]]></FromUserName>
                                     <CreateTime>{2}</CreateTime>
                                     <MsgType><![CDATA[text]]></MsgType>
                                     <Content><![CDATA[{3}]]></Content>
                                   </xml>";
            string xml = String.Format(xmlTemplate, toUserName, fromUserName, DateTime.Now.DateTimeToTimeStamp(), content);
            Tools.LogMsg(xml);
            return xml;
        }
    }
}