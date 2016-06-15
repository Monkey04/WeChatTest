using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WeChatTest.Model;
using WeChatTest.MsgPush;
using WeChatTest.Service;

namespace WeChatTest
{
    public partial class _interface : System.Web.UI.Page
    {
        public string Token = ConfigurationManager.AppSettings["Token"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
            {
                string postData = Tools.GetPostData();
                Tools.LogMsg("postData：" + postData);
                XmlDocument xmlDoc = Tools.ParseXml(postData);
                string _msgType = xmlDoc.SelectSingleNode("xml/MsgType").InnerText;
                MsgType msgType = Tools.ConvertToEnumType<MsgType>(_msgType);

                Response.Clear();
                switch (msgType)
                {
                    case MsgType.EVENT:
                        string _event = xmlDoc.SelectSingleNode("xml/Event").InnerText;
                        Tools.LogMsg(_event);
                        if (_event.ToLower() == "subscribe")
                        {
                            string toUserName = xmlDoc.SelectSingleNode("xml/ToUserName").InnerText;
                            string fromUserName = xmlDoc.SelectSingleNode("xml/FromUserName").InnerText;
                            WeChatConfig config = WeChatConfig.GetInstance();
                            MessageService.SendText(config.AccessToken, fromUserName, "这是Monkey的个人测试号，非常感谢您的关注！");
                        }
                        Response.Write("");
                        break;
                    case MsgType.TEXT:
                        MsgText msgText = new MsgText(xmlDoc);
                        Response.Write(msgText.BuildResponseXml("你刚才发送的是：" + msgText.Content));                     
                        break;
                    case MsgType.IMAGE:
                        MsgImage msgImage = new MsgImage(xmlDoc);
                        Response.Write(msgImage.BuildResponseXml(msgImage.MediaId));
                        break;
                    case MsgType.VOICE:
                        MsgVoice msgVoice = new MsgVoice(xmlDoc);
                        Response.Write(msgVoice.BuilResponseXml(msgVoice.MediaId));
                        break;
                    case MsgType.VIDEO:
                        MsgVideo msgVideo = new MsgVideo(xmlDoc);
                        Response.Write(msgVideo.BuildResponseXml(msgVideo.MediaId, "测试", "描述测试"));
                        break;
                    case MsgType.SHORTVIDEO:
                        MsgShortVideo msgShortVideo = new MsgShortVideo(xmlDoc);
                        Response.Write(MsgText.BuildResponseXml(msgShortVideo.FromUserName, msgShortVideo.ToUserName, "您刚才发送了一个小视频"));
                        break;
                    case MsgType.LOCATION:
                        MsgLocation msgLocation = new MsgLocation(xmlDoc);
                        Response.Write(MsgText.BuildResponseXml(msgLocation.FromUserName, msgLocation.ToUserName, "您刚才发送的地址为：" + msgLocation.Label));
                        break;
                    case MsgType.LINK:
                        MsgLink msgLink = new MsgLink(xmlDoc);
                        Response.Write(MsgText.BuildResponseXml(msgLink.FromUserName, msgLink.ToUserName, "您刚才发送的链接地址为：" + msgLink.Url));
                        break;
                    default:
                        Response.Write("");
                        break;
                }
                Response.End();
            }
            else
            { 
                CheckWeChat();
            }         
        }     

        /// <summary>
        /// 验证微信API接口
        /// </summary>
        private void CheckWeChat()
        {
            string echoStr = Request.QueryString["echoStr"];
            if (string.IsNullOrEmpty(echoStr)) {
                Response.Write("参数有误");
                Response.End();
                return;
            }

            Tools.LogMsg("echoStr：" + echoStr);
            if (CheckSignature())
            {
                if (!string.IsNullOrEmpty(echoStr))
                {
                    Response.Write(echoStr);
                    Response.End();
                }
            }
        }

        /// <summary>
        /// 验证配置接口的签名
        /// </summary>
        /// <returns></returns>
        private bool CheckSignature()
        {
            string signature = Request.QueryString["signature"].ToString();
            string timestamp = Request.QueryString["timestamp"].ToString();
            string nonce = Request.QueryString["nonce"].ToString();
            string[] ArrTmp = { Token, timestamp, nonce };
            Array.Sort(ArrTmp);　　 //字典排序  
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();
           

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("signature：{0}", signature);
            sb.AppendFormat("timestamp：{0}", timestamp);
            sb.AppendFormat("nonce：{0}", nonce);
            sb.AppendFormat("tmpStr：{0}", tmpStr);
            Tools.LogMsg(sb.ToString());

            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}