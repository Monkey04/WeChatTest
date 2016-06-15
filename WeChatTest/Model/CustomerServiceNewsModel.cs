using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    /// <summary>
    /// 客服消息实体
    /// </summary>
    public class CustomerServiceNewsModel
    {
        public class Base
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 描述
            /// </summary>
            public string Description { get; set; }
        }

        #region 图文消息（点击跳转到外链）

        /// <summary>
        /// 图文消息（点击跳转到外链）
        /// </summary>
        public class News : Base
        {
            /// <summary>
            /// 图文消息被点击后跳转的链接
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 图文消息的图片链接，支持JPG、PNG格式，较好的效果为大图640*320，小图80*80
            /// </summary>
            public string PicUrl { get; set; }
        }

        #endregion

        #region 视频消息

        public class Video : Base
        {
            /// <summary>
            /// 媒体ID
            /// </summary>
            public string Media_Id { get; set; }

            /// <summary>
            /// 缩略图的媒体ID
            /// </summary>
            public string Thumb_Media_Id { get; set; }
        }

        #endregion

        #region 音乐消息

        public class Music : Base
        {
            /// <summary>
            /// 音乐链接
            /// </summary>
            public string MusicUrl { get; set; }

            /// <summary>
            /// 高品质音乐链接，wifi环境优先使用该链接播放音乐
            /// </summary>
            public string HQMusicUrl { get; set; }

            /// <summary>
            /// 缩略图的媒体ID
            /// </summary>
            public string Thumb_Media_Id { get; set; }
        }

        #endregion
    }
}