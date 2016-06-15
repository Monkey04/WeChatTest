using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class CustomerListModel : ErrorBaseModel
    {
        /// <summary>
        /// 关注该公众账号的总用户数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 拉取的OPENID个数，最大值为10000
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 列表数据，OPENID的列表
        /// </summary>
        public OpenIdList Data { get; set; }

        /// <summary>
        /// 拉取列表的最后一个用户的OPENID
        /// </summary>
        public string Next_Openid { get; set; }

        #region 内层类
        public class OpenIdList
        {
            public List<string> OpenId { get; set; } 
        }
        #endregion

    }
}