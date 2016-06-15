using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class MenuButtonModel : ButtonModel
    {
        /// <summary>
        /// 子菜单
        /// </summary>
        public SubButtonList Sub_Button { get; set; }

        #region 内层类

        public class SubButtonList
        {
            public List<MenuButtonModel> List { get; set; }
        }

        #endregion
    } 
}