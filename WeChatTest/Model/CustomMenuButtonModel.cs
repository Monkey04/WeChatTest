using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class CustomMenuButtonModel : ButtonModel
    {
        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<CustomMenuButtonModel> Sub_Button { get; set; }
    }
}