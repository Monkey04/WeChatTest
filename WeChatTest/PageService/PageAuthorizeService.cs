using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WeChatTest.Model;

namespace WeChatTest.PageService
{
    public class PageAuthorizeService
    {
        #region 私有变量

        private static WeChatConfig weChatConfig = WeChatConfig.GetInstance();

        #endregion

        /// <summary>
        /// 生成授权链接
        /// </summary>
        /// <param name="redirectUri">授权后重定向的回调链接地址，线上完整地址</param>
        /// <param name="state">重定向后会带上state参数，开发者可以填写a-zA-Z0-9的参数值，最多128字节</param>
        /// <returns></returns>
        public static string GennerateAuthorizeLink(string redirectUri, string state)
        {
            string linkUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_userinfo&state={2}#wechat_redirect";
            linkUrl = string.Format(weChatConfig.AppId, System.Web.HttpUtility.UrlEncode(redirectUri), state.IsNotEmpty() ? state : "state");
            return linkUrl;
        }

        /// <summary>
        /// 根据用户授权后，回调地址中带的code参数获取网页授权信息
        /// </summary>
        /// <param name="code">code作为换取access_token的票据，每次用户授权带上的code将不一样，code只能使用一次，5分钟未被使用自动过期。</param>
        /// <returns></returns>
        public static PageAuthorizationModel GetPageAuthorizationInfo(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, weChatConfig.AppId, weChatConfig.AppSecret, code);
            string returnInfo = Tools.HttpGet(url);
            PageAuthorizationModel pageAuthorization = Tools.ConvertToModel<PageAuthorizationModel>(returnInfo);
            return pageAuthorization;
        }

        /// <summary>
        /// 根据用户授权后，回调地址中带的code参数获取网页授权信息
        /// </summary>
        /// <param name="code">code作为换取access_token的票据，每次用户授权带上的code将不一样，code只能使用一次，5分钟未被使用自动过期。</param>
        /// <param name="pageAuthorizationModel">获取成功后，返回网页授权信息实体</param>
        /// <returns></returns>
        public static bool GetPageAuthorizationInfo(string code, ref PageAuthorizationModel pageAuthorizationModel)
        {
            PageAuthorizationModel PageAuthorization = GetPageAuthorizationInfo(code);
            if (PageAuthorization.ErrCode.IsEmpty())
            {
                pageAuthorizationModel = PageAuthorization;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 拉取用户信息(需scope为 snsapi_userinfo)
        /// </summary>
        /// <param name="accessToken">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同</param>
        /// <param name="openId">用户的唯一标识</param>
        /// <returns></returns>
        public static CustomerModel GetUserInfo(string accessToken, string openId)
        {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN";
            url = string.Format(url, accessToken, openId);
            string returnInfo = Tools.HttpGet(url);
            CustomerModel customer = Tools.ConvertToModel<CustomerModel>(returnInfo);
            return customer;
        }

        /// <summary>
        /// 拉取用户信息(需scope为 snsapi_userinfo)
        /// </summary>
        /// <param name="accessToken">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同</param>
        /// <param name="openId">用户的唯一标识</param>
        /// <param name="customerModel">获取用户信息成功后，返回对应的用户信息实体</param>
        /// <returns></returns>
        public static bool GetUserInfo(string accessToken, string openId, ref CustomerModel customerModel)
        {
            CustomerModel customer = GetUserInfo(accessToken, openId);
            if (customer.ErrCode.IsEmpty())
            {
                customerModel = customer;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检验授权凭证（access_token）是否有效
        /// </summary>
        /// <param name="accessToken">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同</param>
        /// <param name="openId">用户的唯一标识</param>
        /// <returns></returns>
        public static bool CheckAccessToken(string accessToken, string openId)
        {
            string url = "https://api.weixin.qq.com/sns/auth?access_token=" + accessToken + "&openid=" + openId;
            return Tools.CheckIsSuccessAfterHttpGet(url);
        }

        /// <summary>
        /// 由于access_token拥有较短的有效期，当access_token超时后，可以使用refresh_token进行刷新，
        /// refresh_token拥有较长的有效期（7天、30天、60天、90天），当refresh_token失效的后，需要用户重新授权。
        /// </summary>
        /// <param name="refreshToken">填写通过access_token获取到的refresh_token参数</param>
        /// <returns></returns>
        public static PageAuthorizationModel RefreshAccessToken(string refreshToken)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}";
            url = string.Format(url, weChatConfig.AppId, refreshToken);
            string returnInfo = Tools.HttpGet(url);
            PageAuthorizationModel pageAuthorization = Tools.ConvertToModel<PageAuthorizationModel>(returnInfo);
            return pageAuthorization;
        }

        /// <summary>
        /// 由于access_token拥有较短的有效期，当access_token超时后，可以使用refresh_token进行刷新，
        /// refresh_token拥有较长的有效期（7天、30天、60天、90天），当refresh_token失效的后，需要用户重新授权。
        /// </summary>
        /// <param name="refreshToken">填写通过access_token获取到的refresh_token参数</param>
        /// <param name="pageAuthorizationModel">刷新成功后返回网页授权信息实体</param>
        /// <returns></returns>
        public static bool RefreshAccessToken(string refreshToken, ref PageAuthorizationModel pageAuthorizationModel)
        {
            PageAuthorizationModel pageAuthorization = RefreshAccessToken(refreshToken);
            if (pageAuthorization.ErrCode.IsEmpty())
            {
                pageAuthorizationModel = pageAuthorization;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}