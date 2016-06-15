using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChatTest.Push
{
    public class PushBase
    {
        #region 私有属性

        private XmlDocument _RootXmlDocument = null;

        #endregion

        #region 属性

        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间戳
        /// </summary>
        public long CreateTimeStamp { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MsgType MsgType { get; set; }

        /// <summary>
        /// 创建时间，已转为本地时区（UTC+8），即添加8小时
        /// </summary>
        public DateTime LocalCreatTime { get; set; }

        /// <summary>
        /// 初始化时使用的xml文件（只读）
        /// </summary>
        public XmlDocument RootXmlDocument { get { return _RootXmlDocument; } }

        #endregion

        public PushBase(XmlDocument xmlDoc)
        {
            _RootXmlDocument = xmlDoc;

            ToUserName = xmlDoc.SelectSingleNode("xml/ToUserName").InnerText;
            FromUserName = xmlDoc.SelectSingleNode("xml/FromUserName").InnerText;
            CreateTimeStamp = Convert.ToInt64(xmlDoc.SelectSingleNode("xml/CreateTime").InnerText);
            MsgType = Tools.ConvertToEnumType<MsgType>(xmlDoc.SelectSingleNode("xml/MsgType").InnerText);
            //转为本地时间，添加8小时
            LocalCreatTime = CreateTimeStamp.TimeStampToDateTime().AddHours(8);
        }
    }
}