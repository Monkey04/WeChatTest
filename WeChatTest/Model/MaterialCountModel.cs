using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class MaterialCountModel : ErrorBaseModel
    {
        /// <summary>
        /// 语音总数量
        /// </summary>
        public int Voice_Count { get; set; }

        /// <summary>
        /// 视频总数量
        /// </summary>
        public int Video_Count { get; set; }

        /// <summary>
        /// 图片总数量
        /// </summary>
        public int Image_Count { get; set; }

        /// <summary>
        /// 图文总数量
        /// </summary>
        public int News_Count { get; set; }
    }
}