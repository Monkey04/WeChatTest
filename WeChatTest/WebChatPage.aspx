<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebChatPage.aspx.cs" Inherits="WeChatTest.WebChatPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript" src="http://res.wx.qq.com/open/js/jweixin-1.0.0.js"></script>
</head>
<body style="font-size:30px">
    <form id="form1" runat="server">
    <div>
       <h2 style="text-align:center">微信网页开发</h2>
       <div style="line-height:50px;">
            <%=AuthHtml %>
       </div>
    </div>
    </form> 
</body>
    <script>
        wx.config({
            debug: true,
            appId: "<%=config.AppId%>",
            signature: "<%=config.JsSignature%>",
            timestamp: "<%=config.JsTimeStamp%>",
            nonceStr: "<%=config.JsNonceStr%>",
            jsApiList: ["onMenuShareQQ"]
        })

        wx.ready(function () {
            /*
             * 所有分享都不能引导用户点击，需要用户主动点击微信右上角的分享
             */
            wx.onMenuShareQQ({
                title: '微信QQ分享测试', // 分享标题
                desc: '微信QQ分享测试', // 分享描述
                link: 'http://houzhihao.51mypc.cn', // 分享链接
                imgUrl: 'http://houzhihao.51mypc.cn/image/chenqing.jpg', // 分享图标
                success: function () {
                    // 用户确认分享后执行的回调函数
                    alert("分享成功");
                },
                cancel: function () {
                    // 用户取消分享后执行的回调函数
                    alert("取消分享");
                }
            });
        })
    </script>
</html>
