using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeChatTest.Model;
using WeChatTest.PageService;

namespace WeChatTest
{
    public partial class WebChatPage : System.Web.UI.Page
    {
        public string AuthHtml = "您没有授权或信息查询有误";
        public WeChatConfig config = WeChatConfig.GetInstance();
        protected void Page_Load(object sender, EventArgs e)
        {
            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];
            if (code.IsNotEmpty())
            {
                PageAuthorizationModel pageAuthorization = PageAuthorizeService.GetPageAuthorizationInfo(code);
                //Tools.LogMsg(Tools.ConvertToJson(pageAuthorization, true));
                if (pageAuthorization.ErrCode.IsEmpty())
                {
                    CustomerModel customer = PageAuthorizeService.GetUserInfo(pageAuthorization.Access_Token, pageAuthorization.OpenId);
                    //Tools.LogMsg(Tools.ConvertToJson(customer, true));
                    if (customer.ErrCode.IsEmpty())
                    {
                        AuthHtml = "您已授权，您的基本信息如下：";
                        AuthHtml += "<br/>";
                        AuthHtml += customer.NickName;
                        AuthHtml += "&nbsp;&nbsp; <img src='" + customer.HeadImgUrl + "' style='width:40px;height:40px'>";
                        AuthHtml += "<br/>";
                        AuthHtml += customer.Sex == 1 ? "男" : "女";
                        AuthHtml += "<br/>";
                        AuthHtml += customer.Province + " - " + customer.City;
                        AuthHtml += "<br/>";
                    }
                }
            }
        }
    }
}