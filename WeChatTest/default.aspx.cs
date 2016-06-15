using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WeChatTest.Enum;
using WeChatTest.Helper;
using WeChatTest.Model;
using WeChatTest.Service;

namespace WeChatTest
{
    public partial class _default : System.Web.UI.Page
    {
        public string html = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            //WeChatConfig config = WeChatConfig.GetInstance();
            //string myOpenId = ConfigurationManager.AppSettings["MyOpenId"];
        }

    }
}