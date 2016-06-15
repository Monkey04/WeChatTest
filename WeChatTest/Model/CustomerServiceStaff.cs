using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class CustomerServiceStaff
    {
        /// <summary>
        /// 完整客服账号，格式为：账号前缀@公众号微信号
        /// </summary>
        public string Kf_Account { get; set; }

        /// <summary>
        /// 客服昵称
        /// </summary>
        public string Kf_Nick { get; set; }

        /// <summary>
        /// 客服工号
        /// </summary>
        public string Kf_Id { get; set; }

        /// <summary>
        /// 客服头像
        /// </summary>
        public string Kf_HeadImgUrl { get; set; }
    }
}