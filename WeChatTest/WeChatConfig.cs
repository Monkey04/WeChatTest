using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WeChatTest.Helper;

namespace WeChatTest
{
    public class WeChatConfig
    {
        #region 参数
        
        private string _AppId { get; set; }
        private string _AppSecret { get; set; }
        private DateTime _LastGetAccessTokenTime { get; set; }

        private int EffectiveSecond = 7200;

        private string _AccessTokenCacheName = "WeChatAccessToken";

        private string _JsTicketCacheName = "WeChatJsTicket";

        private string _JsNonceStr = Tools.GenerateRandomStr();

        private long _JsTimeStamp = 0;

        /// <summary>
        /// 公众号AppId
        /// </summary>
        public string AppId { get { return _AppId; } }

        /// <summary>
        /// 公众号AppSecrect
        /// </summary>
        public string AppSecret { get { return _AppSecret; } }

        /// <summary>
        /// 公众号AccessToken，先从缓存中取，若缓存中不存在则重新获取，有效时间7200秒，获取失败返回空值
        /// </summary>
        public string AccessToken { get { return GetAccessToken(); } }

        /// <summary>
        /// 上次成功获取AccessToken的时间
        /// </summary>
        public DateTime LastGetAccessTokenTime { get { return _LastGetAccessTokenTime; } }

        /// <summary>
        /// 用于微信jssdk配置的时间戳，必须先获取JsSignature再调用，该时间戳才能和签名中的时间戳一致
        /// </summary>
        public string JsTimeStamp { get { return _JsTimeStamp.ToString(); } }

        //生成微信jssdk用的随机串
        public string JsNonceStr { get { return _JsNonceStr; } }

        /// <summary>
        /// 生成微信jssdk用的签名，先从缓存中取，若缓存中不存在则重新获取，有效时间7200秒，获取失败返回空值
        /// </summary>
        public string JsSignature { get { return GenerateJsSignature(); } }
        #endregion

        #region 单例

        private static WeChatConfig instance;
        private static object locker = new object();
        private WeChatConfig()
        {
            _AppId = ConfigurationManager.AppSettings["AppId"];
            _AppSecret = ConfigurationManager.AppSettings["AppSecret"];
        }

        public static WeChatConfig GetInstance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new WeChatConfig();
                    }
                }
            }

            return instance;
        }

        #endregion

        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        private string HttpGetAccessToken(string appId, string appSecret)
        {
            //缓存中没有找到AccessToken，则重新获取
            if (!CacheHelper.IsCacheExist(_AccessTokenCacheName))
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
                string response = Tools.HttpGet(String.Format(url, appId, appSecret));
                JsonDeserializeHelper jsHelper = new JsonDeserializeHelper(response);
                object accessToken = jsHelper.GetValue("access_token");
                if (accessToken == null)
                {
                    //获取失败默认返回空值
                    return string.Empty;
                }
                else
                {
                    _LastGetAccessTokenTime = DateTime.Now;
                    CacheHelper.AddCache(_AccessTokenCacheName, accessToken.ToString(), EffectiveSecond);
                    return accessToken.ToString();
                }
            }
            else
            {
                return CacheHelper.GetCache(_AccessTokenCacheName) as string;
            }

        }

        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        private string GetAccessToken()
        {
            return HttpGetAccessToken(_AppId, _AppSecret);
        }

        /// <summary>
        /// 获取微信jssdk的ticket
        /// </summary>
        /// <returns></returns>
        private string GetJsApiTicket()
        {
            //缓存中没有找到JsTicket，则重新获取
            if (!CacheHelper.IsCacheExist(_JsTicketCacheName))
            {
                string accessToken = this.AccessToken;
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + accessToken + "&type=jsapi";
                string returnInfo = Tools.HttpGet(url);
                JsonDeserializeHelper jsHelper = new JsonDeserializeHelper(returnInfo);
                object jsTicket = jsHelper.GetValue("ticket");
                if (jsTicket == null)
                {
                    //获取失败默认返回空值
                    return string.Empty;
                }
                else
                {
                    CacheHelper.AddCache(_JsTicketCacheName, jsTicket.ToString(), 7200);
                    return jsTicket.ToString();
                }
            }
            else
            {
                return CacheHelper.GetCache(_JsTicketCacheName) as string;
            }
        }

        /// <summary>
        /// 生成jssdk的signature
        /// </summary>
        /// <returns></returns>
        private string GenerateJsSignature()
        {
            string ticket = GetJsApiTicket();
            string nonceStr = _JsNonceStr;
            long timeStamp = Tools.GetTimeStamp();
            _JsTimeStamp = timeStamp;
            string url = System.Web.HttpContext.Current.Request.Url.ToString().Split('#')[0];

            string rawStr = "jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}";
            //if (url.IndexOf("?") > -1)
            //{
            //    string urlLeftPart = url.Substring(0, url.IndexOf("?"));
            //    List<string> urlRightPart = new List<string>();
            //    foreach (string key in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
            //    {
            //        string keyValue = System.Web.HttpContext.Current.Request.QueryString[key].ToString();
            //        urlRightPart.Add(key + "=" + System.Web.HttpUtility.UrlEncode(keyValue));
            //    }

            //    url = urlLeftPart + "?" + string.Join("&", urlRightPart.ToArray());
            //}
            
            //在浏览器中打开的页面地址和生成签名的地址必须一致
            //如果使用了内网映射工具，打开的页面和生成签名的地址必须同时存在或不存在端口号
            rawStr = string.Format(rawStr, ticket, nonceStr, timeStamp, url.Replace(":8011",""));

            string signature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(rawStr, "SHA1").ToLower();
            return signature;
        }
    }
}