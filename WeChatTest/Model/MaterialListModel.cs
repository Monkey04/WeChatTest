using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class MaterialListModel : ErrorBaseModel
    {
        /// <summary>
        /// 该类型的素材的总数
        /// </summary>
        public int Total_Count { get; set; }

        /// <summary>
        /// 本次调用获取的素材的数量
        /// </summary>
        public int Item_Count { get; set; }

        /// <summary>
        /// 素材列表
        /// </summary>
        public List<ItemList> Item { get; set; }

        #region 内层类

        public class ItemList
        {
            /// <summary>
            /// 素材微信唯一Id
            /// </summary>
            public string Media_Id { get; set; }

            /// <summary>
            /// 素材名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 素材最后更新时间
            /// </summary>
            public long Update_Time { get; set; }

            /// <summary>
            /// 图文页的URL，或者，当获取的列表是图片素材列表时，该字段是图片的URL
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 图文消息
            /// </summary>
            public NewsItem Content { get; set; }
        }

        public class NewsItem
        {
            public List<GraphicsMaterialModel> News_Item { get; set; }

            public long Create_Time { get; set; }

            public long Update_Time { get; set; }
        }

        #endregion
    }
}