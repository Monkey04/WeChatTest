using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using WeChatTest.Model;

namespace WeChatTest.Service
{
    public class AccountService
    {
        #region 私有变量

        private static string _CreateQrCodeUrlTemplate = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";

        private static Random random = new Random();

        #endregion

        #region 私有方法

        /// <summary>
        /// 生成随机的二维码场景Id
        /// </summary>
        /// <returns></returns>
        private static long GenerateSceneId()
        {
            return random.Next(100, 100000);
        }

        #endregion

        /// <summary>
        /// 生成临时二维码
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="expireSeconds">临时二维码有效时间，最少30秒，最多2592000秒（30天）</param>
        /// <returns></returns>
        public static QrCodeModel CreateTempQrCode(string accessToken, long expireSeconds)
        {
            string url = string.Format(_CreateQrCodeUrlTemplate, accessToken);
            //临时二维码有效时间最少30秒，最多2592000秒（30天）
            if (expireSeconds < 30)
            {
                expireSeconds = 30;
            }
            else if (expireSeconds >= 2592000)
            {
                expireSeconds = 2592000;
            }
            string postData = "{\"expire_seconds\":" + expireSeconds + ", \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\":" + GenerateSceneId() + "}}}";
            string returnInfo = Tools.HttpPost(url, postData);
            return Tools.ConvertToModel<QrCodeModel>(returnInfo);
        }

        /// <summary>
        /// 生成永久二维码（使用scene_id）
        /// 永久二维码数量只有10万条
        /// </summary>
        /// <param name="accessToken"></param>
        public static QrCodeModel CreateQrCode(string accessToken)
        {
            string url = string.Format(_CreateQrCodeUrlTemplate, accessToken);
            string postData = "{\"action_name\": \"QR_LIMIT_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": " + GenerateSceneId() + "}}}";
            string returnInfo = Tools.HttpPost(url, postData);
            return Tools.ConvertToModel<QrCodeModel>(returnInfo);
        }

        /// <summary>
        /// 生成永久二维码（使用scene_str）
        /// 永久二维码数量只有10万条
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="sceneStr">自定义字符串类型场景值，长度限制为1到64，仅永久二维码支持此字段</param>
        public static QrCodeModel CreateQrCode(string accessToken, string sceneStr)
        {
            string url = string.Format(_CreateQrCodeUrlTemplate, accessToken);
            string postData = "{\"action_name\": \"QR_LIMIT_STR_SCENE\", \"action_info\": {\"scene\": {\"scene_str\": \"" + sceneStr + "\"}}}";
            string returnInfo = Tools.HttpPost(url, postData);
            return Tools.ConvertToModel<QrCodeModel>(returnInfo);
        }

        /// <summary>
        /// 获取二维码get链接，可直接用于img标签展示
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static string GetQrCodePictureLink(string ticket)
        {
            return "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + HttpUtility.UrlEncode(ticket);
        }

        /// <summary>
        /// 下载二维码图片
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="saveFolderFullName">要保存二维码图片的文件夹</param>
        /// <param name="pictureName">保存的二维码的全名，包括后缀，保存成功后返回全路径</param>
        public static bool DownloadQrCodePicture(string ticket, string saveFolderFullName, ref string pictureName)
        {
            string url = GetQrCodePictureLink(ticket);
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    byte[] returnData = webClient.DownloadData(url);
                    string fileFullName = saveFolderFullName + ((saveFolderFullName.LastIndexOf(@"\") == (saveFolderFullName.Length - 1)) ? "" : "\\") + pictureName;
                    File.WriteAllBytes(fileFullName, returnData);
                    pictureName = fileFullName;
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 将一条长链接转成短链接。
        /// 主要使用场景： 开发者用于生成二维码的原链接（商品、支付二维码等）太长导致扫码速度和成功率下降，
        /// 将原长链接通过此接口转成短链接再生成二维码将大大提升扫码速度和成功率。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="longLink"></param>
        /// <param name="shortLink"></param>
        public static bool LongLinkToShort(string accessToken, string longLink, ref string shortLink)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/shorturl?access_token=" + accessToken;
            string postData = "{\"action\":\"long2short\",\"long_url\":\"" + longLink + "\"}";
            string returnInfo = Tools.HttpPost(url, postData);
            JsonDeserializeHelper jsonHelper = new JsonDeserializeHelper(returnInfo);
            if (jsonHelper.GetValue("errcode").ToString() == "0")
            {
                shortLink = jsonHelper.GetValue("short_url").ToString();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}