using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class MenuModel : ErrorBaseModel
    {
        /// <summary>
        /// 菜单是否开启，0代表未开启，1代表开启
        /// </summary>
        public int Is_Menu_Open { get; set; }

        /// <summary>
        /// 菜单信息
        /// </summary>
        public SelfMenuInfo SelfMenu_Info { get; set; }

        #region 内层类

        public class SelfMenuInfo
        {
            public List<MenuButtonModel> Button { get; set; }
        }

        #endregion
    }
}