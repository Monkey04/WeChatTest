using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class VideoMaterialModel : ErrorBaseModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 视频描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 视频可下载地址
        /// </summary>
        public string Down_Url { get; set; }
    }
}