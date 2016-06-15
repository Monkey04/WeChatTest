using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WeChatTest.Enum;
using WeChatTest.Model;

namespace WeChatTest.Service
{
    public class MaterialService
    {
        #region 私有类

        private class GraphicsMaterialList
        {
           public List<GraphicsMaterialModel> Articles { get; set; }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 通用上传素材方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileFullPath"></param>
        /// <returns></returns>
        private static UploadResultModel UploadMaterial(string url, string fileFullPath)
        {
            using (WebClient client = new WebClient())
            {
                
                byte[] returnByte = client.UploadFile(url, fileFullPath);
                string returnInfo = Encoding.Default.GetString(returnByte);
                UploadResultModel uploadResult = Tools.ConvertToModel<UploadResultModel>(returnInfo);
                return uploadResult;
            }
        }

        #endregion

        #region 临时素材上传下载

        /// <summary>
        /// 新增临时素材
        /// 媒体文件在后台保存时间为3天，即3天后media_id失效。
        /// 上传的临时多媒体文件有格式和大小限制，如下：
        /// 图片（image）: 1M，支持JPG格式
        /// 语音（voice）：2M，播放长度不超过60s，支持AMR\MP3格式
        /// 视频（video）：10MB，支持MP4格式
        /// 缩略图（thumb）：64KB，支持JPG格式
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="fileFullPath">媒体文件全路径</param>
        /// <param name="mediaType">媒体文件类型</param>
        public static UploadResultModel UpLoadTempMaterial(string accessToken, string fileFullPath, MediaType mediaType)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}";
            return UploadMaterial(string.Format(url, accessToken, mediaType.ToString().ToLower()), fileFullPath);
        }

        /// <summary>
        /// 新增临时素材
        /// 媒体文件在后台保存时间为3天，即3天后media_id失效。
        /// 上传的临时多媒体文件有格式和大小限制，如下：
        /// 图片（image）: 1M，支持JPG格式
        /// 语音（voice）：2M，播放长度不超过60s，支持AMR\MP3格式
        /// 视频（video）：10MB，支持MP4格式
        /// 缩略图（thumb）：64KB，支持JPG格式
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="fileFullPath">媒体文件全路径</param>
        /// <param name="mediaType">媒体文件类型</param>
        /// <param name="mediaId">成功上传媒体文件后返回的mediaId</param>
        /// <returns></returns>
        public static bool UpLoadTempMaterial(string accessToken, string fileFullPath, MediaType mediaType, out string mediaId)
        {
            mediaId = string.Empty;
            UploadResultModel uploadResult = UpLoadTempMaterial(accessToken, fileFullPath, mediaType);
            if (uploadResult.ErrCode.IsEmpty())
            {
                mediaId = uploadResult.Media_Id;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取临时素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId"></param>
        /// <param name="fileFullName">本地保存地址</param>
        /// <returns></returns>
        public static bool DownLoadTempMaterial(string accessToken, string mediaId, string fileFullName)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/media/get?access_token=" + accessToken + "&media_id=" + mediaId;
            using (WebClient client = new WebClient())
            {
                byte[] returnByte = client.DownloadData(url);
                string returnInfo = Encoding.Default.GetString(returnByte);
                if (returnInfo.Contains("errcode"))
                {
                    return false;
                }
                else
                {
                    try
                    {
                        File.WriteAllBytes(fileFullName, returnByte);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
            }
        }

        #endregion

        #region 永久素材的新增、获取和删除

        /// <summary>
        /// 新增图文永久素材
        /// 新增的永久素材也可以在公众平台官网素材管理模块中看到
        /// 永久素材的数量是有上限的，请谨慎新增。图文消息素材和图片素材的上限为5000，其他类型为1000
        /// 素材的格式大小等要求与公众平台官网一致。具体是，图片大小不超过2M，支持bmp/png/jpeg/jpg/gif格式，语音大小不超过5M，长度不超过60秒，支持mp3/wma/wav/amr格式
        /// 永久图片素材新增后，将带有URL返回给开发者，开发者可以在腾讯系域名内使用（腾讯系域名外使用，图片将被屏蔽）
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="jsonData"></param>
        /// <param name="mediaId">新增成功返回对应的媒体Id</param>
        /// <returns></returns>
        public static bool UploadRegularGraphicsMaterial(string accessToken, string jsonData, out string mediaId)
        {
            mediaId = string.Empty;
            string url = "https://api.weixin.qq.com/cgi-bin/material/add_news?access_token=" + accessToken;
            string returnInfo = Tools.HttpPost(url, jsonData);
            JsonDeserializeHelper jsonHelper = new JsonDeserializeHelper(returnInfo);
            object returnMediaId = jsonHelper.GetValue("media_id");
            if (returnMediaId != null)
            {
                mediaId = returnMediaId.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 新增图文永久素材
        /// 新增的永久素材也可以在公众平台官网素材管理模块中看到
        /// 永久素材的数量是有上限的，请谨慎新增。图文消息素材和图片素材的上限为5000，其他类型为1000
        /// 素材的格式大小等要求与公众平台官网一致。具体是，图片大小不超过2M，支持bmp/png/jpeg/jpg/gif格式，语音大小不超过5M，长度不超过60秒，支持mp3/wma/wav/amr格式
        /// 永久图片素材新增后，将带有URL返回给开发者，开发者可以在腾讯系域名内使用（腾讯系域名外使用，图片将被屏蔽）
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="graphicsMaterials"></param>
        /// <param name="mediaId">新增成功返回对应的媒体Id</param>
        /// <returns></returns>
        public static bool UploadRegularGraphicsMaterial(string accessToken, List<GraphicsMaterialModel> graphicsMaterials, out string mediaId)
        {
            mediaId = string.Empty;
            GraphicsMaterialList graphicsMaterialList = new GraphicsMaterialList();
            graphicsMaterialList.Articles = graphicsMaterials;
            string jsonData = Tools.ProcessPostJson(Tools.ConvertToJson(graphicsMaterialList, false));

            return UploadRegularGraphicsMaterial(accessToken, jsonData, out mediaId);
        }

        /// <summary>
        /// 在图文消息的具体内容中，将过滤外部的图片链接，开发者可以通过下述接口上传图片得到URL，放到图文内容中使用。
        /// 本接口所上传的图片不占用公众号的素材库中图片数量的5000个的限制。图片仅支持jpg/png格式，大小必须在1MB以下。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="imgFullPath">本地图片全路径</param>
        /// <param name="imgUrl">上传成功后微信返回的图片地址</param>
        public static bool UploadImageForGraphicMaterial(string accessToken, string imgFullPath, out string imgUrl)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token=" + accessToken;
            imgUrl = string.Empty;
            using (WebClient client = new WebClient())
            {
                byte[] returnByte = client.UploadFile(url, imgFullPath);
                string returnInfo = Encoding.Default.GetString(returnByte);
                if (returnInfo.Contains("errcode"))
                {
                    return false;
                }
                else
                {
                    imgUrl = new JsonDeserializeHelper(returnInfo).GetValue("url").ToString();
                    return true;
                }
            }
        }

        /// <summary>
        /// 新增其他类型永久素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="fileName"></param>
        public static UploadResultModel UploadRegularMaterial(string accessToken, string fileFullName, MediaType mediaType)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}&type={1}", accessToken, mediaType.ToString().ToLower());
            Dictionary<string, string> fileDic = new Dictionary<string, string>();
            fileDic.Add("media", fileFullName);
            string returnInfo = Tools.HttpPostForm(url, null, fileDic);
            UploadResultModel uploadResult = Tools.ConvertToModel<UploadResultModel>(returnInfo);
            return uploadResult;
        }

        /// <summary>
        /// 新增永久视频素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="videoFileFullPath">本地视频地址全路径</param>
        /// <param name="title">视频素材的标题</param>
        /// <param name="introduction">视频素材的描述</param>
        /// <returns></returns>
        public static UploadResultModel UploadRegularVideoMaterial(string accessToken, string videoFileFullPath, string title, string introduction)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}&type={1}", accessToken, "video");
            Dictionary<string, string> fileDic = new Dictionary<string, string>();
            fileDic.Add("media", videoFileFullPath);
            Dictionary<string, string> textDic = new Dictionary<string, string>();
            textDic.Add("description", "{\"title\":\"" + title + "\",\"introduction\":\"" + introduction + "\"}");

            string returnInfo = Tools.HttpPostForm(url, textDic, fileDic);
            UploadResultModel uploadResult = Tools.ConvertToModel<UploadResultModel>(returnInfo);
            return uploadResult;
        }

        /// <summary>
        /// 获取素材总数
        /// 永久素材的总数，也会计算公众平台官网素材管理中的素材
        /// 图片和图文消息素材（包括单图文和多图文）的总数上限为5000，其他素材的总数上限为1000
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static MaterialCountModel QueryRegularMaterialCount(string accessToken)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/material/get_materialcount?access_token=" + accessToken;
            string returnInfo = Tools.HttpGet(url);
            return Tools.ConvertToModel<MaterialCountModel>(returnInfo);
        }

        /// <summary>
        /// 获取永久素材列表
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaType">素材类型</param>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材返回</param>
        /// <param name="count">返回素材的数量，取值在1到20之间</param>
        public static MaterialListModel QueryRegularMaterialList(string accessToken, MediaType mediaType, int offset, int count)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token=" + accessToken;
            string type = mediaType.ToString().ToLower();
            if (mediaType == MediaType.THUMB)
            {
                type = "image";
            }

            string postData = string.Format("{{\"type\":\"{0}\",\"offset\":\"{1}\",\"count\":\"{2}\"}}", type, offset, count);
            string returnInfo = Tools.HttpPost(url, postData);
            return Tools.ConvertToModel<MaterialListModel>(returnInfo);
        }

        /// <summary>
        /// 获取单个永久素材
        /// 如果请求的素材是图文消息或视频，则返回json字符串
        /// 如果是其他类型素材，则响应的直接为素材的内容，可以自行保存为文件
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId"></param>
        public static bool GetSingleRegularMaterial(string accessToken, string mediaId, out byte[] dataByte)
        {
            dataByte = null;
            string url = "https://api.weixin.qq.com/cgi-bin/material/get_material?access_token=" + accessToken;
            string postData = "{\"media_id\":\"" + mediaId + "\"}";
            using (WebClient webClient = new WebClient())
            {
                byte[] returnByte = webClient.UploadData(url, Encoding.UTF8.GetBytes(postData));
                string returnInfo = Encoding.UTF8.GetString(returnByte);
                if (returnInfo.Contains("errcode"))
                {
                    return false;
                }
                else
                {
                    dataByte = returnByte;
                    return true;
                }
            }
        }

        /// <summary>
        /// 删除单个永久素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId"></param>
        public static bool DeleteSingleRegularMaterial(string accessToken, string mediaId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/material/del_material?access_token=" + accessToken;
            string postData = "{\"media_id\":\"" + mediaId + "\"}";
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        #endregion
    }
}