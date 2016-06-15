using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    /// <summary>
    /// 图文消息的信息类
    /// </summary>
    public class NewsInformationModel
    {
        /// <summary>
        /// 图文消息的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Digest { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 是否显示封面，0为不显示，1为显示
        /// </summary>
        public int Show_Cover { get; set; }

        /// <summary>
        /// 封面图片的URL
        /// </summary>
        public string Cover_Url { get; set; }

        /// <summary>
        /// 正文的URL
        /// </summary>
        public string Content_Url { get; set; }

        /// <summary>
        /// 原文的URL，若置空则无查看原文入口
        /// </summary>
        public string Source_Url { get; set; }
    }
}