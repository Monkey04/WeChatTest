using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class CustomerGroupModel : ErrorBaseModel
    {
        /// <summary>
        /// 分组id，由微信分配
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 分组名字，UTF8编码
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分组内用户数量
        /// </summary>
        public int Count { get; set; }
    }
}