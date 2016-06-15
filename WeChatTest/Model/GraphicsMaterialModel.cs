using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class GraphicsMaterialModel : ErrorBaseModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图文消息的封面图片素材id（必须是永久mediaID）
        /// </summary>
        public string Thumb_Media_Id { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 图文消息的摘要，仅有单图文消息才有摘要，多图文此处为空
        /// </summary>
        public string Digest { get; set; }

        /// <summary>
        /// 是否显示封面，0为false，即不显示，1为true，即显示
        /// </summary>
        public int Show_Cover_Pic { get; set; }

        /// <summary>
        /// 图文消息的具体内容，支持HTML标签，必须少于2万字符，小于1M，且此处会去除JS
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 图文消息的原文地址，即点击“阅读原文”后的URL
        /// </summary>
        public string Content_Source_Url { get; set; }
    }
}