using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class UploadResultModel : ErrorBaseModel
    {
        /// <summary>
        /// 媒体文件类型，分别有图片（image）、语音（voice）、视频（video）和缩略图（thumb，主要用于视频与音乐格式的缩略图）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 媒体文件上传后，获取时的唯一标识
        /// </summary>
        public string Media_Id { get; set; }

        /// <summary>
        /// 媒体文件上传时间戳
        /// </summary>
        public long Created_At { get; set; }

        /// <summary>
        /// 上传文件是图片类型才会返回url
        /// </summary>
        public string Url { get; set; }
    }
}