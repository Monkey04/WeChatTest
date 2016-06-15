using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WeChatTest.Model;

namespace WeChatTest.Service
{
    public class CustomerService
    {
        #region 私有类
        private class MultiCustomer : ErrorBaseModel
        {
            public List<CustomerModel> User_Info_List { get; set; } 
        }

        private class MultiCustomerGroup : ErrorBaseModel
        {
            public List<CustomerGroupModel> Groups { get; set; }
        }

        private class SingleCustomerGroupId : ErrorBaseModel
        {
            public int GroupId { get; set; }
        }

        private class SingleCustomerGroup : ErrorBaseModel
        {
            public CustomerGroupModel group { get; set; }
        }
        #endregion

        #region 获取用户信息
        /// <summary>
        /// 获取单个用户的Json信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static string QuerySingleCustomerInfo(string accessToken, string openId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";
            return Tools.HttpGet(String.Format(url, accessToken, openId));
        }

        /// <summary>
        /// 获取单个用户的Model
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static CustomerModel QuerySingleCustomerModel(string accessToken, string openId)
        {
            string customerInfo = QuerySingleCustomerInfo(accessToken, openId);
            return Tools.ConvertToModel<CustomerModel>(customerInfo);
        }

        /// <summary>
        /// 批量获取（单次最多100）个用户的json信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIdArray"></param>
        /// <returns></returns>
        public static string QueryMultiCustomersInfo(string accessToken, string[] openIdArray)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token=" + accessToken;
            List<string> builder = new List<string>();
            string postData = "{{\"user_list\":[{0}]}}";
            foreach (string openId in openIdArray)
            {
                builder.Add(String.Format("{{\"openid\":\"{0}\",\"lang\":\"zh-CN\"}}", openId));
            }

            return Tools.HttpPost(url, String.Format(postData, String.Join(",", builder.ToArray())));
        }

        /// <summary>
        /// 批量获取（单次最多100）个用户的Model
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIdArray"></param>
        /// <param name="customerModelList"></param>
        /// <returns></returns>
        public static bool QueryMultiCustomersModel(string accessToken, string[] openIdArray, out List<CustomerModel> customerModelList)
        {
            customerModelList = new List<CustomerModel>();
            string multiCustomerInfo = QueryMultiCustomersInfo(accessToken, openIdArray);
            MultiCustomer multiCustomer = Tools.ConvertToModel<MultiCustomer>(multiCustomerInfo);
            if (multiCustomer.ErrCode.IsEmpty())
            {
                customerModelList = multiCustomer.User_Info_List;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 用户分组管理

        /// <summary>
        /// 查询所有用户分组信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string QueryAllGroupsInfo(string accessToken)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/groups/get?access_token=" + accessToken;
            return Tools.HttpGet(url);
        }

        /// <summary>
        /// 查询所有用户分组model
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="customerGroupModelList"></param>
        /// <returns></returns>
        public static bool QueryAllGroupsModel(string accessToken, out List<CustomerGroupModel> customerGroupModelList)
        {
            customerGroupModelList = new List<CustomerGroupModel>();
            string allGroupInfo = QueryAllGroupsInfo(accessToken);
            MultiCustomerGroup multiCustomerGroup = Tools.ConvertToModel<MultiCustomerGroup>(allGroupInfo);
            if (multiCustomerGroup.ErrCode.IsEmpty())
            {
                customerGroupModelList = multiCustomerGroup.Groups;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 通过用户的OpenID查询其所在的GroupID
        /// 返回errcode时返回-1
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static int QuerySingleCustomerGroupId(string accessToken, string openId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/groups/getid?access_token=" + accessToken;
            string postData = "{\"openid\":\"" + openId + "\"}";
            string singleCustomerGroupIdInfo = Tools.HttpPost(url, postData);
            SingleCustomerGroupId singleCustomerGroupId = Tools.ConvertToModel<SingleCustomerGroupId>(singleCustomerGroupIdInfo);
            return singleCustomerGroupId.ErrCode.IsEmpty() ? singleCustomerGroupId.GroupId : -1;
        }

        /// <summary>
        /// 创建分组
        /// 一个公众账号，最多支持创建100个分组
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="groupName">分组名字（30个字符以内）</param>
        public static bool CreateSingleCustomerGroup(string accessToken, string groupName, out CustomerGroupModel customerGroup)
        {
            customerGroup = new CustomerGroupModel();
            string url = "https://api.weixin.qq.com/cgi-bin/groups/create?access_token=" + accessToken;
            string postData = "{\"group\":{\"name\":\"" + groupName + "\"}}";
            string singleCustomerGroupInfo = Tools.HttpPost(url, postData);
            SingleCustomerGroup singleCustomerGroup = Tools.ConvertToModel<SingleCustomerGroup>(singleCustomerGroupInfo);
            if (singleCustomerGroup.ErrCode.IsEmpty())
            {
                customerGroup = singleCustomerGroup.group;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 修改单个分组名
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="id">微信分配的分组Id</param>
        /// <param name="newName">新的分组名</param>
        public static bool ModifySingleCustomerGroupName(string accessToken, int id, string newName)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/groups/update?access_token=" + accessToken;
            string postData = "{\"group\":{\"id\":" + id + ",\"name\":\"" + newName + "\"}}";
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 修改单个分组名
        /// </summary>
        /// <param name="accessToken">微信分配的分组Id</param>
        /// <param name="newCustomerGroupModel">新的分组model</param>
        /// <returns></returns>
        public static bool ModifySingleCustomerGroupName(string accessToken, CustomerGroupModel newCustomerGroupModel)
        {
            if (newCustomerGroupModel.Id < 0 || newCustomerGroupModel.Name.IsEmpty()) return false;
            return ModifySingleCustomerGroupName(accessToken, newCustomerGroupModel.Id, newCustomerGroupModel.Name);
        }

        /// <summary>
        /// 移动用户到其他分组
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <param name="newGroupId"></param>
        public static bool MoveSingleCustomerToNewGroup(string accessToken, string openId, int newGroupId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/groups/members/update?access_token=" + accessToken;
            string postData = "{\"openid\":\"" + openId + "\",\"to_groupid\":" + newGroupId + "}";
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 批量移动用户到同一分组
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIdList"></param>
        /// <param name="newGroupId"></param>
        public static bool MoveMultiCustomersToNewGroup(string accessToken, string[] openIdList, int newGroupId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/groups/members/batchupdate?access_token=" + accessToken;
            List<string> builder = new List<string>();
            string postData = "{{\"openid_list\":[{0}],\"to_groupid\":{1}}}";
            foreach(string openId in openIdList)
            {
                builder.Add("\""+openId+"\"");
            }
            postData = String.Format(postData, String.Join(",", builder.ToArray()), newGroupId);

            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        /// <summary>
        /// 删除单个分组
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="groupId"></param>
        public static bool DeleteSingleCustomerGroup(string accessToken, int groupId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/groups/delete?access_token=" + accessToken;
            string postData = "{\"group\":{\"id\":" + groupId + "}}";
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        #endregion

        #region 设置用户备注名

        /// <summary>
        /// 设置单个用户的备注名
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <param name="newRemark">新备注名</param>
        public static bool SetSingleCustomerRemark(string accessToken, string openId, string newRemark)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/info/updateremark?access_token=" + accessToken;
            string postData = "{\"openid\":\"" + openId + "\",\"remark\":\"" + newRemark + "\"}";
            return Tools.CheckIsSuccessAfterHttpPost(url, postData);
        }

        #endregion

        #region 获取用户列表
        
        /// <summary>
        /// 获取用户列表的json信息
        /// 一次拉取调用最多拉取10000个关注者的OpenID，可以通过多次拉取的方式来满足需求
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="nextOpenId">第一个拉取的OPENID，不填默认从头开始拉取</param>
        /// <returns></returns>
        public static string QueryCustomersListInfo(string accessToken, string nextOpenId)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/get?access_token=" + accessToken + "&next_openid=" + nextOpenId;
            return Tools.HttpGet(url);
        }

        /// <summary>
        /// 获取用户列表model
        /// 一次拉取调用最多拉取10000个关注者的OpenID，可以通过多次拉取的方式来满足需求
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="nextOpenId">第一个拉取的OPENID，不填默认从头开始拉取</param>
        public static CustomerListModel QueryCustomersListModel(string accessToken, string nextOpenId)
        {
            string customersListInfo = QueryCustomersListInfo(accessToken, nextOpenId);
            return Tools.ConvertToModel<CustomerListModel>(customersListInfo);
        }

        /// <summary>
        /// 获取用户列表model
        /// 一次拉取调用最多拉取10000个关注者的OpenID，可以通过多次拉取的方式来满足需求
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="nextOpenId">第一个拉取的OPENID，不填默认从头开始拉取</param>
        public static CustomerListModel QueryCustomersListModel(string accessToken)
        {
            return QueryCustomersListModel(accessToken, "");
        }

        #endregion

    }
}