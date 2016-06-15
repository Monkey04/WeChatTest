using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using WeChatTest.Model;
using WeChatTest.Push;

namespace WeChatTest.MsgPush
{
    public abstract class MsgBase:PushBase
    {
        #region 属性

        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public string MsgId { get; set; }

        #endregion

        public MsgBase(XmlDocument xmlDoc)
            : base(xmlDoc)
        {
            MsgId = xmlDoc.SelectSingleNode("xml/MsgId").InnerText;
        }

        /// <summary>
        /// 被动回复图文消息
        /// 如果图文数超过10，则将会无响应
        /// PicUrl：图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
        /// </summary>
        /// <param name="toUserName">接收方帐号（收到的OpenID）</param>
        /// <param name="fromUserName">开发者微信号</param>
        /// <param name="newsList">图文消息列表</param>
        /// <returns></returns>
        public static string BuildGraphicsMsgResponseXml(string toUserName, string fromUserName, List<CustomerServiceNewsModel.News> newsList)
        {
            if (newsList == null || newsList.Count == 0) return string.Empty;

            string xmlTemplate = @"<xml>
                                     <ToUserName><![CDATA[{0}]]></ToUserName>
                                     <FromUserName><![CDATA[{1}]]></FromUserName>
                                     <CreateTime>{2}</CreateTime>
                                     <MsgType><![CDATA[news]]></MsgType>
                                     <ArticleCount>{3}</ArticleCount>
                                     <Articles>
                                     {4}
                                     </Articles>
                                  </xml> ";

            string itemTemplate = @"<item>
                                        <Title><![CDATA[{0}]]></Title> 
                                        <Description><![CDATA[{1}]]></Description>
                                        <PicUrl><![CDATA[{2}]]></PicUrl>
                                        <Url><![CDATA[{3}]]></Url>
                                   </item>";

            //图文信息最多为10条
            int newsCount = newsList.Count;
            newsCount = newsCount >= 10 ? 10 : newsCount;

            StringBuilder itemBuilder = new StringBuilder();

            for (int i = 0; i < newsCount; i++)
            {
                CustomerServiceNewsModel.News newsItem = newsList[i];
                itemBuilder.AppendFormat(itemTemplate, newsItem.Title.IsEmpty() ? "" : newsItem.Title, newsItem.Description.IsEmpty() ? "" : newsItem.Description, newsItem.PicUrl.IsEmpty() ? "" : newsItem.PicUrl, newsItem.Url.IsEmpty() ? "" : newsItem.Url);
            }

            string xml = string.Format(xmlTemplate, toUserName, fromUserName, DateTime.Now.DateTimeToTimeStamp(), newsCount, itemBuilder.ToString());
            Tools.LogMsg(xml);
            return xml;
        }
    }
}