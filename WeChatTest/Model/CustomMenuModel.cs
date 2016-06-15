using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Model
{
    public class CustomMenuModel : ErrorBaseModel
    {
        /// <summary>
        /// 自定义菜单
        /// </summary>
        public MenuButton Menu { get; set; }

        /// <summary>
        /// 个性化菜单
        /// </summary>
        public List<ConditionalMenuButton> ConditionalMenu { get; set; }

        #region 内层类

        public class MenuButton : ErrorBaseModel
        {
            /// <summary>
            /// 按钮列表
            /// </summary>
            public List<CustomMenuButtonModel> Button { get; set; }

            /// <summary>
            /// 菜单Id
            /// </summary>
            public string MenuId { get; set; }
        }

        public class ConditionalMenuButton : MenuButton
        {
            public MatchRuleModel MatchRule { get; set; }
        }

        #endregion
    }
}