using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeChatTest.Model;

namespace WeChatTest
{
    /* 自定义菜单最多包括3个一级菜单，每个一级菜单最多包含5个二级菜单。
     * 一级菜单最多4个汉字，二级菜单最多7个汉字，多出来的部分将会以“...”代替。
     * 创建自定义菜单后，菜单的刷新策略是，在用户进入公众号会话页或公众号profile页时，如果发现上一次拉取菜单的请求在5分钟以前，就会拉取一下菜单，如果菜单有更新，就会刷新客户端的菜单。
     * 测试时可以尝试取消关注公众账号后再次关注，则可以看到创建后的效果。
     */
    public class MenuService
    {
        #region 获取公众号菜单
        /// <summary>
        /// 获取自定义菜单的配置json
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string QueryCustomMenuConfigInfo(string accessToken)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/menu/get";
            return Tools.HttpPost(url, "access_token=" + accessToken);
        }

        /// <summary>
        /// 获取自定义菜单的配置model
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static CustomMenuModel QueryCustomMenuConfigModel(string accessToken)
        {
            string returnInfo = QueryCustomMenuConfigInfo(accessToken);
            return Tools.ConvertToModel<CustomMenuModel>(returnInfo);
        }

        /// <summary>
        /// 获取公众号菜单的配置
        /// 如果公众号是通过API调用设置的菜单，则返回菜单的开发配置
        /// 而如果公众号是在公众平台官网通过网站功能发布菜单，则本接口返回运营者设置的菜单配置
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string QueryMenuConfigInfo(string accessToken)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/get_current_selfmenu_info";
            return Tools.HttpPost(url, "access_token=" + accessToken);
        }

        /// <summary>
        /// 获取公众号菜单的配置model
        /// 如果公众号是通过API调用设置的菜单，则返回菜单的开发配置
        /// 而如果公众号是在公众平台官网通过网站功能发布菜单，则本接口返回运营者设置的菜单配置
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static MenuModel QueryMenuConfigModel(string accessToken)
        {
            string returnInfo = QueryMenuConfigInfo(accessToken);
            return Tools.ConvertToModel<MenuModel>(returnInfo);
        }

        /// <summary>
        /// 个性化菜单匹配结果
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="userId">user_id可以是粉丝的OpenID，也可以是粉丝的微信号。</param>
        public static CustomMenuModel MatchMenu(string accessToken, string userId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/menu/trymatch?access_token=" + accessToken;
            string postData = "{\"user_id\":\"" + userId + "\"}";
            string returnInfo = Tools.HttpPost(url, postData);
            return Tools.ConvertToModel<CustomMenuModel>(returnInfo);
        }
        #endregion

        #region 公众号自定义菜单的创建和删除

        #region 自定义菜单格式
        /*click和view的请求示例
             * {
     "button":[
     {	
          "type":"click",
          "name":"今日歌曲",
          "key":"V1001_TODAY_MUSIC"
      },
      {
           "name":"菜单",
           "sub_button":[
           {	
               "type":"view",
               "name":"搜索",
               "url":"http://www.soso.com/"
            },
            {
               "type":"view",
               "name":"视频",
               "url":"http://v.qq.com/"
            },
            {
               "type":"click",
               "name":"赞一下我们",
               "key":"V1001_GOOD"
            }]
       }]
 }
             */

        /*其他新增按钮类型的请求示例
         * {
"button": [
    {
        "name": "扫码", 
        "sub_button": [
            {
                "type": "scancode_waitmsg", 
                "name": "扫码带提示", 
                "key": "rselfmenu_0_0", 
                "sub_button": [ ]
            }, 
            {
                "type": "scancode_push", 
                "name": "扫码推事件", 
                "key": "rselfmenu_0_1", 
                "sub_button": [ ]
            }
        ]
    }, 
    {
        "name": "发图", 
        "sub_button": [
            {
                "type": "pic_sysphoto", 
                "name": "系统拍照发图", 
                "key": "rselfmenu_1_0", 
               "sub_button": [ ]
             }, 
            {
                "type": "pic_photo_or_album", 
                "name": "拍照或者相册发图", 
                "key": "rselfmenu_1_1", 
                "sub_button": [ ]
            }, 
            {
                "type": "pic_weixin", 
                "name": "微信相册发图", 
                "key": "rselfmenu_1_2", 
                "sub_button": [ ]
            }
        ]
    }, 
    {
        "name": "发送位置", 
        "type": "location_select", 
        "key": "rselfmenu_2_0"
    },
    {
       "type": "media_id", 
       "name": "图片", 
       "media_id": "MEDIA_ID1"
    }, 
    {
       "type": "view_limited", 
       "name": "图文消息", 
       "media_id": "MEDIA_ID2"
    }
]
}
         */
        #endregion

        /// <summary>
        /// 创建自定义菜单
        /// 一级菜单数组，个数应为1~3个
        /// 二级菜单数组，个数应为1~5个
        /// 一级菜单最多4个汉字，二级菜单最多7个汉字，多出来的部分将会以“...”代替
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="menuJson">json格式的菜单信息</param>
        /// <returns></returns>
        public static bool CreateCustomMenu(string accessToken, string menuJson)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + accessToken;
            return Tools.CheckIsSuccessAfterHttpPost(url, menuJson);
        }

        /// <summary>
        /// 创建自定义菜单
        /// 一级菜单数组，个数应为1~3个
        /// 二级菜单数组，个数应为1~5个
        /// 一级菜单最多4个汉字，二级菜单最多7个汉字，多出来的部分将会以“...”代替
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="menuButton"></param>
        /// <returns></returns>
        public static bool CreateCustomMenu(string accessToken, CustomMenuModel.MenuButton menuButton)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + accessToken;
            return Tools.CheckIsSuccessAfterHttpPost(url, Tools.ProcessPostJson(Tools.ConvertToJson(menuButton, true)));
        }

        /// <summary>
        /// 删除公众号菜单
        /// 在个性化菜单时，调用此接口会删除默认菜单及全部个性化菜单。
        /// </summary>
        /// <param name="accessToken"></param>
        public static bool DeleteMenu(string accessToken)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/menu/delete?access_token=" + accessToken;
            string returnInfo = Tools.HttpGet(url);
            return Tools.CheckIsSuccessAfterHttpGet(url);
        }

        #endregion

        #region 公众号个性化菜单

        #region 个性化菜单格式
        /*
         * {
 	"button":[
 	{	
    	"type":"click",
    	"name":"今日歌曲",
     	"key":"V1001_TODAY_MUSIC" 
	},
	{ 
		"name":"菜单",
		"sub_button":[
		{	
			"type":"view",
			"name":"搜索",
			"url":"http://www.soso.com/"
		},
		{
			"type":"view",
			"name":"视频",
			"url":"http://v.qq.com/"
		},
		{
			"type":"click",
			"name":"赞一下我们",
			"key":"V1001_GOOD"
		}]
 }],
"matchrule":{
  "group_id":"2",
  "sex":"1",
  "country":"中国",
  "province":"广东",
  "city":"广州",
  "client_platform_type":"2"
  "language":"zh_CN"
  }
}
         */
        #endregion

        /// <summary>
        /// 创建个性化菜单
        /// 个性化菜单要求用户的微信客户端版本在iPhone6.2.2，Android 6.2.4以上
        /// 普通公众号的个性化菜单的新增接口每日限制次数为2000次
        /// 出于安全考虑，一个公众号的所有个性化菜单，最多只能设置为跳转到3个域名下的链接
        /// 创建个性化菜单之前必须先创建默认菜单
        /// 当公众号创建多个个性化菜单时，将按照发布顺序，由新到旧逐一匹配，直到用户信息与matchrule相符合。如果全部个性化菜单都没有匹配成功，则返回默认菜单。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="menuJson">
        /// matchrule共六个字段，均可为空，但不能全部为空，至少要有一个匹配信息是不为空的。
        /// country、province、city组成地区信息，将按照country、province、city的顺序进行验证，要符合地区信息表的内容。地区信息从大到小验证，小的可以不填，即若填写了省份信息，则国家信息也必填并且匹配，城市信息可以不填。
        /// </param>
        public static bool CreateConditionalMenu(string accessToken, string menuJson, out string menuId)
        {
            menuId = string.Empty;
            string url = "https://api.weixin.qq.com/cgi-bin/menu/addconditional?access_token=" + accessToken;
            string returnInfo = Tools.HttpPost(url, menuJson);
            JsonDeserializeHelper result = new JsonDeserializeHelper(returnInfo);
            string returnMenuId = result.GetValue("menuid") == null ? string.Empty : result.GetValue("menuid").ToString();
            if(returnMenuId.IsEmpty())
            {
                return false;
            }
            else
            {
                menuId = returnMenuId;
                return true;
            }
        }

        /// <summary>
        /// 创建个性化菜单
        /// 个性化菜单要求用户的微信客户端版本在iPhone6.2.2，Android 6.2.4以上
        /// 普通公众号的个性化菜单的新增接口每日限制次数为2000次
        /// 出于安全考虑，一个公众号的所有个性化菜单，最多只能设置为跳转到3个域名下的链接
        /// 创建个性化菜单之前必须先创建默认菜单
        /// 当公众号创建多个个性化菜单时，将按照发布顺序，由新到旧逐一匹配，直到用户信息与matchrule相符合。如果全部个性化菜单都没有匹配成功，则返回默认菜单。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="menuButton"></param>
        /// <param name="matchRule">
        /// matchrule共六个字段，均可为空，但不能全部为空，至少要有一个匹配信息是不为空的。
        /// country、province、city组成地区信息，将按照country、province、city的顺序进行验证，要符合地区信息表的内容。地区信息从大到小验证，小的可以不填，即若填写了省份信息，则国家信息也必填并且匹配，城市信息可以不填。
        /// </param>
        /// <param name="menuId"></param>
        public static bool CreateConditionalMenu(string accessToken, CustomMenuModel.MenuButton menuButton, MatchRuleModel matchRule, out string menuId)
        {
            menuId = string.Empty;
            string menuButtonJson = Tools.ConvertToJson(menuButton,true);
            string matchRuleJson = Tools.ConvertToJson(matchRule, false);
            string menuJson = Tools.ProcessPostJson(menuButtonJson.Insert(menuButtonJson.Length - 1, ",\"matchrule\":" + matchRuleJson));
            return CreateConditionalMenu(accessToken, menuJson, out menuId);
        }

        /// <summary>
        /// 删除个性化菜单
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="menuId">menuid为菜单id，可以通过自定义菜单查询接口获取。</param>
        /// <returns></returns>
        public static bool DeleteConditionalMenu(string accessToken, string menuId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/menu/delconditional?access_token=" + accessToken;
            string postData = "{\"menuid\":\"" + menuId + "\"}";
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }   

        #endregion
    }
}