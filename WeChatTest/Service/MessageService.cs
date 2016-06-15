using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WeChatTest.Model;

namespace WeChatTest.Service
{
    public class MessageService
    {
        #region 私有参数

        private const string _SendUrlTemplate = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";

        private const string _SendUrlTemplateByGroupId = "https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token={0}";

        private const string _SendUrlTemplateByOpenId = "https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token={0}";

        #endregion

        #region 私有类

        private class KF_List : ErrorBaseModel
        {
            public List<CustomerServiceStaff> kf_list { get; set; }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 如果需要以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像），则需在JSON数据包的后半部分加入customservice参数
        /// </summary>
        /// <param name="sourceJson"></param>
        /// <param name="kf_account">客服账号</param>
        private static void InsertCustomServiceJson(ref string sourceJson, string kf_account)
        {
            if (kf_account.IsNotEmpty() && sourceJson.IsNotEmpty())
            {
                string customServiceJson = "\"customservice\":{\"kf_account\":\"" + kf_account + "\"}";
                sourceJson.Insert(sourceJson.Length - 1, "," + customServiceJson);
            }
        }

        /// <summary>
        /// 获取群发结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static bool GetCommonResultByMultipleSend(string url, string postData, ref string msgId)
        {
            string returnInfo = Tools.HttpPost(url, postData);
            JsonDeserializeHelper jsonHelper = new JsonDeserializeHelper(returnInfo);
            object _msgId = jsonHelper.GetValue("msg_id");
            if (_msgId != null)
            {
                msgId = _msgId.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 客服账号管理静态类（测试号无法使用）

        /// <summary>
        /// 必须先在公众平台官网为公众号设置微信号后才能使用
        /// 开发者在根据开发文档的要求完成开发后，使用6.0.2版及以上版本的微信用户在与公众号进行客服沟通，公众号使用不同的客服账号进行回复后，用户可以看到对应的客服头像和昵称。
        /// </summary>
        public static class Staff
        {
            /// <summary>
            /// 添加客服账号，每个公众号最多添加10个客服账号
            /// </summary>
            /// <param name="accessToken"></param>
            /// <param name="kf_account">完整客服账号，格式为：账号前缀@公众号微信号</param>
            /// <param name="nickname">客服昵称，最长6个汉字或12个英文字符</param>
            /// <param name="password">客服账号登录密码，格式为密码明文的32位加密MD5值。该密码仅用于在公众平台官网的多客服功能中使用，若不使用多客服功能，则不必设置密码</param>
            public static bool AddAccount(string accessToken, string kf_account, string nickName, string password)
            {
                string url = "https://api.weixin.qq.com/customservice/kfaccount/add?access_token=" + accessToken;
                string postData = string.Format("{{\"kf_account\":\"{0}\",\"nickname\":\"{1}\",\"password\":\"{2}\"}}", kf_account, nickName, password);
                return Tools.CheckIsSuccessAfterHttpPost(url, postData);
            }

            /// <summary>
            /// 修改客服账号
            /// </summary>
            /// <param name="accessToken"></param>
            /// <param name="kf_account">完整客服账号，格式为：账号前缀@公众号微信号</param>
            /// <param name="nickName">客服昵称，最长6个汉字或12个英文字符</param>
            /// <param name="password">客服账号登录密码，格式为密码明文的32位加密MD5值。该密码仅用于在公众平台官网的多客服功能中使用，若不使用多客服功能，则不必设置密码</param>
            /// <returns></returns>
            public static bool ModifyAccount(string accessToken, string kf_account, string nickName, string password)
            {
                string url = "https://api.weixin.qq.com/customservice/kfaccount/update?access_token=" + accessToken;
                string postData = string.Format("{{\"kf_account\":\"{0}\",\"nickname\":\"{1}\",\"password\":\"{2}\"}}", kf_account, nickName, password);
                return Tools.CheckIsSuccessAfterHttpPost(url, postData);
            }

            /// <summary>
            /// 删除客服账号
            /// </summary>
            /// <param name="accessToken"></param>
            /// <param name="kf_account">完整客服账号，格式为：账号前缀@公众号微信号</param>
            /// <param name="nickName">客服昵称，最长6个汉字或12个英文字符</param>
            /// <param name="password">客服账号登录密码，格式为密码明文的32位加密MD5值。</param>
            public static bool DeleteAccount(string accessToken, string kf_account, string nickName, string password)
            {
                string url = "https://api.weixin.qq.com/customservice/kfaccount/del?access_token=" + accessToken;
                string postData = string.Format("{{\"kf_account\":\"{0}\",\"nickname\":\"{1}\",\"password\":\"{2}\"}}", kf_account, nickName, password);
                return Tools.CheckIsSuccessAfterHttpPost(url, postData);
            }

            /// <summary>
            /// 设置客服帐号的头像
            /// </summary>
            /// <param name="accessToken"></param>
            /// <param name="kf_account">完整客服账号，格式为：账号前缀@公众号微信号</param>
            /// <param name="headImageFileFullPath">要上传作为头像的本地图片路径</param>
            /// <returns></returns>
            public static bool SetAccountHeadImage(string accessToken, string kf_account, string headImageFileFullPath)
            {
                string url = "http://api.weixin.qq.com/customservice/kfaccount/uploadheadimg?access_token=" + accessToken + "&kf_account=" + kf_account;
                using (WebClient webClient = new WebClient())
                {
                    byte[] returnByte = webClient.UploadFile(url, headImageFileFullPath);
                    string returnInfo = Encoding.UTF8.GetString(returnByte);
                    ErrorBaseModel errorBase = Tools.ConvertToModel<ErrorBaseModel>(returnInfo);
                    return errorBase.ErrMsg.IsNotEmpty() && errorBase.ErrMsg.ToLower() == "ok";
                }
            }

            /// <summary>
            /// 查询所有客服账号
            /// </summary>
            /// <param name="accessToken"></param>
            /// <param name="customerServiceStaffList">查询成功后返回客服列表</param>
            public static bool QueryAccountList(string accessToken, out List<CustomerServiceStaff> customerServiceStaffList)
            {
                customerServiceStaffList = new List<CustomerServiceStaff>();
                string url = "https://api.weixin.qq.com/cgi-bin/customservice/getkflist?access_token=" + accessToken;
                string returnInfo = Tools.HttpGet(url);
                KF_List kfList = Tools.ConvertToModel<KF_List>(returnInfo);
                if (kfList.ErrCode.IsEmpty())
                {
                    customerServiceStaffList = kfList.kf_list;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region 客服接口发信息（用户与公众号在48小时内有交互才能发送）

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客服ID</param>
        /// <param name="content">文本内容</param>
        /// <param name="kf_account">以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像）</param>
        /// <returns></returns>
        public static bool SendText(string accessToken, string openId, string content, string kf_account)
        {
            string url = String.Format(_SendUrlTemplate, accessToken);
            string postData = "{\"touser\":\"" + openId + "\",\"msgtype\":\"text\",\"text\":{\"content\":\"" + content + "\"}}";
            InsertCustomServiceJson(ref postData, kf_account);
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客服ID</param>
        /// <param name="content">文本内容</param>
        /// <returns></returns>
        public static bool SendText(string accessToken, string openId, string content)
        {
            return SendText(accessToken, openId, content, null);
        }

        /// <summary>
        /// 发送图文消息（点击跳转到外链）
        ///  图文消息条数限制在10条以内，注意，如果图文数超过10，则将会无响应。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="newsList">图文消息列表</param>
        /// <param name="kf_account">以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像）</param>
        /// <returns></returns>
        public static bool SendGraphicsMessage(string accessToken, string openId, List<CustomerServiceNewsModel.News> newsList, string kf_account)
        {
            string url = String.Format(_SendUrlTemplate, accessToken);
            string postData = "{\"touser\":\"" + openId + "\",\"msgtype\":\"news\",\"news\":{\"articles\":{0}}}";

            postData = postData.Replace("{0}", Tools.ConvertToJson(newsList, false));

            InsertCustomServiceJson(ref postData, kf_account);

            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 发送图文消息（点击跳转到外链）
        /// 图文消息条数限制在10条以内，注意，如果图文数超过10，则将会无响应。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="newsList">图文消息列表</param>
        /// <returns></returns>
        public static bool SendGraphicsMessage(string accessToken, string openId, List<CustomerServiceNewsModel.News> newsList)
        {
            return SendGraphicsMessage(accessToken, openId, newsList, null);
        }

        /// <summary>
        /// 发送图片消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="mediaId">图片素材ID</param>
        /// <param name="kf_account">以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像）</param>
        /// <returns></returns>
        public static bool SendImage(string accessToken, string openId, string mediaId, string kf_account)
        {
            string url = String.Format(_SendUrlTemplate, accessToken);
            string postData = "{\"touser\":\"" + openId + "\",\"msgtype\":\"image\",\"image\":{\"media_id\":\"" + mediaId + "\"}}";
            InsertCustomServiceJson(ref postData, kf_account);
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 发送图片消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="mediaId">图片素材ID</param>
        /// <returns></returns>
        public static bool SendImage(string accessToken, string openId, string mediaId)
        {
            return SendImage(accessToken, openId, mediaId, null);
        }

        /// <summary>
        /// 发送语音消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="mediaId">语音素材ID</param>
        /// <param name="kf_account">以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像）</param>
        public static bool SendVoice(string accessToken, string openId, string mediaId, string kf_account)
        {
            string url = String.Format(_SendUrlTemplate, accessToken);
            string postData = "{\"touser\":\"" + openId + "\",\"msgtype\":\"voice\",\"voice\":{\"media_id\":\"" + mediaId + "\"}}";
            InsertCustomServiceJson(ref postData, kf_account);
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 发送语音消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="mediaId">语音素材ID</param>
        /// <returns></returns>
        public static bool SendVoice(string accessToken, string openId, string mediaId)
        {
            return SendVoice(accessToken, openId, mediaId, null);
        }

        /// <summary>
        /// 发送视频消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="video">视频实体</param>
        /// <param name="kf_account">以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像）</param>
        /// <returns></returns>
        public static bool SendVideo(string accessToken, string openId, CustomerServiceNewsModel.Video video, string kf_account)
        {
            string url = String.Format(_SendUrlTemplate, accessToken);
            string postData = "{\"touser\":\"" + openId + "\",\"msgtype\":\"video\",\"video\":{0}}";

            postData = postData.Replace("{0}", Tools.ConvertToJson(video, false));

            InsertCustomServiceJson(ref postData, kf_account);

            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 发送视频消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="video">视频实体</param>
        public static bool SendVideo(string accessToken, string openId, CustomerServiceNewsModel.Video video)
        {
            return SendVideo(accessToken, openId, video, null);
        }

        /// <summary>
        /// 发送音频消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="video">音频实体</param>
        /// <param name="kf_account">以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像）</param>
        /// <returns></returns>
        public static bool SendMusic(string accessToken, string openId, CustomerServiceNewsModel.Music music, string kf_account)
        {
            string url = String.Format(_SendUrlTemplate, accessToken);
            string postData = "{\"touser\":\"" + openId + "\",\"msgtype\":\"music\",\"music\":{0}}";

            postData = postData.Replace("{0}", Tools.ConvertToJson(music, false));

            InsertCustomServiceJson(ref postData, kf_account);

            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 发送音频消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="video">音频实体</param>
        public static bool SendMusic(string accessToken, string openId, CustomerServiceNewsModel.Music music)
        {
            return SendMusic(accessToken, openId, music, null);
        }

        /// <summary>
        /// 发送图文消息（点击跳转到图文消息页面）
        /// 图文消息条数限制在8条以内，注意，如果图文数超过8，则将会无响应。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="mediaId">图文素材ID</param>
        /// <param name="kf_account">以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像）</param>
        /// <returns></returns>
        public static bool SendGraphicsMessage(string accessToken, string openId, string mediaId, string kf_account)
        {
            string url = String.Format(_SendUrlTemplate, accessToken);
            string postData = "{\"touser\":\"" + openId + "\",\"msgtype\":\"mpnews\",\"mpnews\":{\"media_id\":\"" + mediaId + "\"}}";
            InsertCustomServiceJson(ref postData, kf_account);
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 发送图文消息（点击跳转到图文消息页面）
        /// 图文消息条数限制在8条以内，注意，如果图文数超过8，则将会无响应。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="mediaId">图文素材ID</param>
        public static bool SendGraphicsMessage(string accessToken, string openId, string mediaId)
        {
            return SendGraphicsMessage(accessToken, openId, mediaId, null);
        }

        /// <summary>
        /// 发送卡券
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="cardId">卡券ID</param>
        /// <param name="cardExt">卡券信息，为json格式
        /// 如：{\"code\":\"\",\"openid\":\"\",\"timestamp\":\"1402057159\",\"signature\":\"017bb17407c8e0058a66d72dcc61632b70f511ad\"}</param>
        /// <param name="kf_account">以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像）</param>
        /// <returns></returns>
        public static bool SendWxCard(string accessToken, string openId, string cardId, string cardExt, string kf_account)
        {
            string url = String.Format(_SendUrlTemplate, accessToken);
            string postData = "{\"touser\":\"" + openId + "\",\"msgtype\":\"wxcard\",\"wxcard\":{\"card_id\":\"" + cardId + "\",\"card_ext\":\"" + cardExt + "\"}}";
            InsertCustomServiceJson(ref postData, kf_account);
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 发送卡券
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId">客户ID</param>
        /// <param name="cardId">卡券ID</param>
        /// <param name="cardExt">卡券信息，为json格式
        /// 如：{\"code\":\"\",\"openid\":\"\",\"timestamp\":\"1402057159\",\"signature\":\"017bb17407c8e0058a66d72dcc61632b70f511ad\"}</param>
        public static bool SendWxCard(string accessToken, string openId, string cardId, string cardExt)
        {
            return SendWxCard(accessToken, openId, cardId, cardExt, null);
        }

        #endregion

        /*
         * 在返回成功时，意味着群发任务提交成功，并不意味着此时群发已经结束，
         * 所以，仍有可能在后续的发送过程中出现异常情况导致用户未收到消息，如消息有时会进行审核、服务器不稳定等。
         * 此外，群发任务一般需要较长的时间才能全部发送完毕，请耐心等待。
         */
        #region 高级群发（测试号只能群发文本消息）

        /// <summary>
        /// 根据分组群发图文消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="isToAll">
        /// 使用is_to_all为true且成功群发，会使得此次群发进入历史消息列表。
        /// 为防止异常，认证订阅号在一天内，只能使用is_to_all为true进行群发一次，或者在公众平台官网群发（不管本次群发是对全体还是对某个分组）一次。以避免一天内有2条群发进入历史消息列表
        /// 类似地，服务号在一个月内，使用is_to_all为true群发的次数，加上公众平台官网群发（不管本次群发是对全体还是对某个分组）的次数，最多只能是4次。
        /// 设置is_to_all为false时是可以多次群发的，但每个用户只会收到最多4条，且这些群发不会进入历史消息列表。</param>
        /// <param name="groupId">用户分组Id</param>
        /// <param name="graphicsMediaId">图文素材Id</param>
        /// <param name="msgDataId">
        /// 消息的数据ID，该字段只有在群发图文消息时，才会出现。
        /// 可以用于在图文分析数据接口中，获取到对应的图文消息的数据，是图文分析数据接口中的msgid字段中的前半部分，详见图文分析数据接口中的msgid字段的介绍。</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        public static bool MultipleSendGraphicsMessage(string accessToken, bool isToAll, int groupId, string graphicsMediaId, out string msgDataId, ref string msgId)
        {
            msgDataId = string.Empty;
            string url = string.Format(_SendUrlTemplateByGroupId, accessToken);
            string postData = "{\"filter\":{\"is_to_all\":" + (isToAll ? "true" : "false") + ",\"group_id\":\"" + groupId + "\"},\"mpnews\":{\"media_id\":\"" + graphicsMediaId + "\"},\"msgtype\":\"mpnews\"}";
            string returnInfo = Tools.HttpPost(url, postData);
            JsonDeserializeHelper jsonHepler = new JsonDeserializeHelper(returnInfo);
            object msg_data_id = jsonHepler.GetValue("msg_data_id");
            if (msg_data_id != null)
            {
                msgDataId = msg_data_id.ToString();
                msgId = jsonHepler.GetValue("msg_id").ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据用户OpenId列表群发图文信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIdArray">用户OpenId列表</param>
        /// <param name="mediaId">图文素材Id</param>
        /// <param name="msgDataId">
        /// 消息的数据ID，该字段只有在群发图文消息时，才会出现。
        /// 可以用于在图文分析数据接口中，获取到对应的图文消息的数据，是图文分析数据接口中的msgid字段中的前半部分，详见图文分析数据接口中的msgid字段的介绍。</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        public static bool MultipleSendGraphicsMessage(string accessToken, string[] openIdArray, string mediaId, out string msgDataId, ref string msgId)
        {
            msgDataId = string.Empty;
            string url = string.Format(_SendUrlTemplateByOpenId, accessToken);
            string postData = "{\"touser\":" + Tools.ConvertToJson(openIdArray, true) + ",\"mpnews\":{\"media_id\":\"" + mediaId + "\"},\"msgtype\":\"mpnews\"}";
            string returnInfo = Tools.HttpPost(url, postData);
            JsonDeserializeHelper jsonHepler = new JsonDeserializeHelper(returnInfo);
            object msg_data_id = jsonHepler.GetValue("msg_data_id");
            if (msg_data_id != null)
            {
                msgDataId = msg_data_id.ToString();
                msgId = jsonHepler.GetValue("msg_id").ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据分组群发文本消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="isToAll">
        /// 使用is_to_all为true且成功群发，会使得此次群发进入历史消息列表。
        /// 为防止异常，认证订阅号在一天内，只能使用is_to_all为true进行群发一次，或者在公众平台官网群发（不管本次群发是对全体还是对某个分组）一次。以避免一天内有2条群发进入历史消息列表
        /// 类似地，服务号在一个月内，使用is_to_all为true群发的次数，加上公众平台官网群发（不管本次群发是对全体还是对某个分组）的次数，最多只能是4次。
        /// 设置is_to_all为false时是可以多次群发的，但每个用户只会收到最多4条，且这些群发不会进入历史消息列表。</param>
        /// <param name="groupId">用户分组Id</param>
        /// <param name="content">要群发的文本内容</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendText(string accessToken, bool isToAll, int groupId, string content, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByGroupId, accessToken);
            string postData = "{\"filter\":{\"is_to_all\":" + (isToAll ? "true" : "false") + ",\"group_id\":\"" + groupId + "\"},\"text\":{\"content\":\"" + content + "\"},\"msgtype\":\"text\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 根据用户OpenId列表群发文本消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIdList">用户OpenId列表</param>
        /// <param name="content">要群发的文本内容</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendText(string accessToken, string[] openIdArray, string content, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByOpenId, accessToken);
            string postData = "{\"touser\":" + Tools.ConvertToJson(openIdArray, true) + ",\"text\":{\"content\":\"" + content + "\"},\"msgtype\":\"text\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 根据分组群发图片消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="isToAll">
        /// 使用is_to_all为true且成功群发，会使得此次群发进入历史消息列表。
        /// 为防止异常，认证订阅号在一天内，只能使用is_to_all为true进行群发一次，或者在公众平台官网群发（不管本次群发是对全体还是对某个分组）一次。以避免一天内有2条群发进入历史消息列表
        /// 类似地，服务号在一个月内，使用is_to_all为true群发的次数，加上公众平台官网群发（不管本次群发是对全体还是对某个分组）的次数，最多只能是4次。
        /// 设置is_to_all为false时是可以多次群发的，但每个用户只会收到最多4条，且这些群发不会进入历史消息列表。</param>
        /// <param name="groupId">用户分组Id</param>
        /// <param name="mediaId">图片素材Id</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendImage(string accessToken, bool isToAll, int groupId, string mediaId, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByGroupId, accessToken);
            string postData = "{\"filter\":{\"is_to_all\":" + (isToAll ? "true" : "false") + ",\"group_id\":\"" + groupId + "\"},\"image\":{\"media_id\":\"" + mediaId + "\"},\"msgtype\":\"image\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 根据用户OpenId列表群发图片消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIdArray">用户OpenId列表</param>
        /// <param name="mediaId">图片素材Id</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendImage(string accessToken, string[] openIdArray, string mediaId, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByOpenId, accessToken);
            string postData = "{\"touser\":" + Tools.ConvertToJson(openIdArray, true) + ",\"image\":{\"media_id\":\"" + mediaId + "\"},\"msgtype\":\"image\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 根据分组群发语音消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="isToAll">
        /// 使用is_to_all为true且成功群发，会使得此次群发进入历史消息列表。
        /// 为防止异常，认证订阅号在一天内，只能使用is_to_all为true进行群发一次，或者在公众平台官网群发（不管本次群发是对全体还是对某个分组）一次。以避免一天内有2条群发进入历史消息列表
        /// 类似地，服务号在一个月内，使用is_to_all为true群发的次数，加上公众平台官网群发（不管本次群发是对全体还是对某个分组）的次数，最多只能是4次。
        /// 设置is_to_all为false时是可以多次群发的，但每个用户只会收到最多4条，且这些群发不会进入历史消息列表。</param>
        /// <param name="groupId">用户分组Id</param>
        /// <param name="mediaId">语音素材Id</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendVoice(string accessToken, bool isToAll, int groupId, string mediaId, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByGroupId, accessToken);
            string postData = "{\"filter\":{\"is_to_all\":" + (isToAll ? "true" : "false") + ",\"group_id\":\"" + groupId + "\"},\"voice\":{\"media_id\":\"" + mediaId + "\"},\"msgtype\":\"voice\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 根据用户OpenId列表群发音频消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIdArray">用户OpenId列表</param>
        /// <param name="mediaId">音频素材Id</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendVoice(string accessToken, string[] openIdArray, string mediaId, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByOpenId, accessToken);
            string postData = "{\"touser\":" + Tools.ConvertToJson(openIdArray, true) + ",\"voice\":{\"media_id\":\"" + mediaId + "\"},\"msgtype\":\"voice\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 根据分组群发视频消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="isToAll">
        /// 使用is_to_all为true且成功群发，会使得此次群发进入历史消息列表。
        /// 为防止异常，认证订阅号在一天内，只能使用is_to_all为true进行群发一次，或者在公众平台官网群发（不管本次群发是对全体还是对某个分组）一次。以避免一天内有2条群发进入历史消息列表
        /// 类似地，服务号在一个月内，使用is_to_all为true群发的次数，加上公众平台官网群发（不管本次群发是对全体还是对某个分组）的次数，最多只能是4次。
        /// 设置is_to_all为false时是可以多次群发的，但每个用户只会收到最多4条，且这些群发不会进入历史消息列表。</param>
        /// <param name="groupId">用户分组Id</param>
        /// <param name="mediaId">视频素材Id</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendVideo(string accessToken, bool isToAll, int groupId, string mediaId, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByGroupId, accessToken);
            string postData = "{\"filter\":{\"is_to_all\":" + (isToAll ? "true" : "false") + ",\"group_id\":\"" + groupId + "\"},\"mpvideo\":{\"media_id\":\"" + mediaId + "\"},\"msgtype\":\"mpvideo\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 根据用户OpenId列表群发音频消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIdArray">用户OpenId列表</param>
        /// <param name="mediaId">视频素材Id</param>
        /// <param name="title">视频标题</param>
        /// <param name="description">视频描述</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendVideo(string accessToken, string[] openIdArray, string mediaId, string title, string description, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByOpenId, accessToken);
            string postData = "{\"touser\":" + Tools.ConvertToJson(openIdArray, true) + ",\"video\":{\"media_id\":\"" + mediaId + "\",\"title\":\"" + title + "\",\"description\":\"" + description + "\"},\"msgtype\":\"video\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 根据分组群发卡券消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="isToAll">
        /// 使用is_to_all为true且成功群发，会使得此次群发进入历史消息列表。
        /// 为防止异常，认证订阅号在一天内，只能使用is_to_all为true进行群发一次，或者在公众平台官网群发（不管本次群发是对全体还是对某个分组）一次。以避免一天内有2条群发进入历史消息列表
        /// 类似地，服务号在一个月内，使用is_to_all为true群发的次数，加上公众平台官网群发（不管本次群发是对全体还是对某个分组）的次数，最多只能是4次。
        /// 设置is_to_all为false时是可以多次群发的，但每个用户只会收到最多4条，且这些群发不会进入历史消息列表。</param>
        /// <param name="groupId">用户分组Id</param>
        /// <param name="cardId">卡券素材Id</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendWxCard(string accessToken, bool isToAll, int groupId, string cardId, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByGroupId, accessToken);
            string postData = "{\"filter\":{\"is_to_all\":" + (isToAll ? "true" : "false") + ",\"group_id\":\"" + groupId + "\"},\"wxcard\":{\"card_id\":\"" + cardId + "\"},\"msgtype\":\"wxcard\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 根据用户OpenId列表群发卡券消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIdArray">用户OpenId列表</param>
        /// <param name="cardId">卡券素材Id</param>
        /// <param name="msgId">发送成功后返回的msgId</param>
        /// <returns></returns>
        public static bool MultipleSendWxCard(string accessToken, string openIdArray, string cardId, ref string msgId)
        {
            string url = string.Format(_SendUrlTemplateByOpenId, accessToken);
            string postData = "{\"touser\":" + Tools.ConvertToJson(openIdArray, true) + ",\"wxcard\":{\"card_id\":\"" + cardId + "\"},\"msgtype\":\"wxcard\"}";
            return GetCommonResultByMultipleSend(url, postData, ref msgId);
        }

        /// <summary>
        /// 删除群发【订阅号与服务号认证后均可用】
        /// 由于技术限制，群发只有在刚发出的半小时内可以删除，发出半小时之后将无法被删除。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="msgId">发送成功的消息Id</param>
        public static bool DeleteMultipleSendMessage(string accessToken, string msgId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/message/mass/delete?access_token=" + accessToken;
            string postData = "{\"msg_id\":\"" + msgId + "\"}";
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        #endregion

    }
}