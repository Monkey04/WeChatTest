using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using WeChatTest.Model;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WeChatTest
{
    public class Tools
    {
        #region 参数
        private static CookieContainer cookie = new CookieContainer();
        #endregion

        /// <summary>
        /// 以Post方式访问网站资源
        /// </summary>
        /// <param name="url">要访问的目标网站资源</param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postData);
            request.CookieContainer = cookie;

            Stream stream = request.GetRequestStream();
            stream.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));
            //StreamWriter streamWriter = new StreamWriter(stream, Encoding.GetEncoding("gb2312"));
            //streamWriter.Write(postData);
            //streamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Cookies = cookie.GetCookies(response.ResponseUri);
            Stream responseStream = response.GetResponseStream();
            StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);
            String responseStr = responseStreamReader.ReadToEnd();
            responseStreamReader.Close();
            responseStream.Close();


            return responseStr;
        }

        /// <summary>
        /// 以Get方式访问网站资源
        /// </summary>
        /// <param name="url">要访问的网站资源</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string responseString = streamReader.ReadToEnd();
            streamReader.Close();
            stream.Close();

            return responseString;
        }

        /// <summary>
        /// 写日志
        /// 每天自动生成新的文件
        /// </summary>
        /// <param name="msg"></param>
        public static void LogMsg(string msg)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            string path = HttpContext.Current.Server.MapPath("~/log");
            //判断Log目录是否存在，不存在则创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path +"\\Log"+ date + ".log";
            //使用StreamWriter写日志，包含时间，错误路径，错误信息
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("-----------------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-----------------");
                sw.WriteLine(HttpContext.Current.Request.Url.ToString());
                sw.WriteLine(msg);
                sw.WriteLine("\r\n");
            }
        }

        /// <summary>
        /// 解析xml字符串
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static XmlDocument ParseXml(string xmlStr)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStr);
            return xmlDoc;
        }

        /// <summary>
        /// 获取post过来的数据
        /// </summary>
        /// <returns></returns>
        public static string GetPostData()
        {
            string postData = string.Empty;
            Stream s = HttpContext.Current.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            postData = Encoding.UTF8.GetString(b);
            return postData;
        }

        /// <summary>
        /// 把json字符串转为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ConvertToModel<T>(string json)
        {
            JavaScriptSerializer jser = new JavaScriptSerializer();
            return jser.Deserialize<T>(json);
        }

        /// <summary>
        /// 把对象转为字符串
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ConvertToJson(object t, bool isIgnoreNull)
        {
            return JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = isIgnoreNull ? NullValueHandling.Ignore : NullValueHandling.Include });
        }

        /// <summary>
        /// 把字符串转为对应的枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="typeValue"></param>
        /// <returns></returns>
        public static T ConvertToEnumType<T>(string typeValue)
        {
            return (T)System.Enum.Parse(typeof(T), typeValue.ToUpper(), false);
        }

        /// <summary>
        /// 统一处理基于ErrorBaseModel的Post返回值
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static bool CheckIsSuccessAfterHttpPost(string url, string postData)
        {
            string returnInfo = Tools.HttpPost(url, postData);
            ErrorBaseModel errorModel = Tools.ConvertToModel<ErrorBaseModel>(returnInfo);
            return errorModel.ErrMsg.ToLower() == "ok";
        }

        /// <summary>
        /// 统一处理基于ErrorBaseModel的Get返回值
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool CheckIsSuccessAfterHttpGet(string url)
        {
            string returnInfo = Tools.HttpGet(url);
            ErrorBaseModel errorModel = Tools.ConvertToModel<ErrorBaseModel>(returnInfo);
            return errorModel.ErrMsg.ToLower() == "ok";
        }

        /// <summary>
        /// 预处理post给微信的json数据，所有键都必须为小写
        /// </summary>
        /// <param name="postJson"></param>
        /// <returns></returns>
        public static string ProcessPostJson(string postJson)
        {
            return Regex.Replace(postJson, @"""(\w+?)"":", delegate(Match match)
            {
                return match.ToString().ToLower();
            });
        }

        /// <summary>
        /// post提交表单
        /// </summary>
        /// <param name="url"></param>
        /// <param name="textParams">字段表单</param>
        /// <param name="fileParams">文件表单</param>
        /// <returns></returns>
        public static string HttpPostForm(string url, IDictionary<string, string> textParams, IDictionary<string, string> fileParams)
        {
            using (WebClient webClient = new WebClient())
            {
                List<byte[]> bytesList = new List<byte[]>();
                string boundary = DateTime.Now.Ticks.ToString("X");
                webClient.Headers.Add("Content-Type", "multipart/form-data;charset=utf-8;boundary=" + boundary);

                byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                // 组装文本请求参数
                if (textParams != null)
                {
                    string textTemplate = "Content-Disposition:form-data;name=\"{0}\"\r\nContent-Type:text/plain\r\n\r\n{1}";
                    IEnumerator<KeyValuePair<string, string>> textEnum = textParams.GetEnumerator();
                    while (textEnum.MoveNext())
                    {
                        string textEntry = string.Format(textTemplate, textEnum.Current.Key, textEnum.Current.Value);
                        byte[] itemBytes = Encoding.UTF8.GetBytes(textEntry);
                        bytesList.Add(itemBoundaryBytes.Concat(itemBytes).ToArray());
                    }
                }

                // 组装文件请求参数
                if (fileParams != null)
                {
                    string fileTemplate = "Content-Disposition:form-data;name=\"{0}\";filename=\"{1}\"\r\nContent-Type:application/octet-stream\r\n\r\n";
                    IEnumerator<KeyValuePair<string, string>> fileEnum = fileParams.GetEnumerator();
                    while (fileEnum.MoveNext())
                    {
                        string fileFullName = fileEnum.Current.Value;
                        string fileEntry = string.Format(fileTemplate, fileEnum.Current.Key, Path.GetFileName(fileFullName));
                        byte[] itemBytes = Encoding.UTF8.GetBytes(fileEntry);

                        byte[] fileBytes = File.ReadAllBytes(fileFullName);
                        bytesList.Add(itemBoundaryBytes.Concat(itemBytes).Concat(fileBytes).ToArray());
                    }
                }

                //组装上传要用的数据字节
                byte[] dataBytes = bytesList.Count > 0 ? bytesList[0] : null;
                for (int i = 1; i < bytesList.Count(); i++)
                {
                    dataBytes.Concat(bytesList[i]);
                }

                dataBytes = dataBytes == null ? endBoundaryBytes : dataBytes.Concat(endBoundaryBytes).ToArray();

                byte[] returnByte = webClient.UploadData(url, dataBytes);
                return Encoding.Default.GetString(returnByte);
            }
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string StringToMD5(string inputValue)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(inputValue, "MD5");
        }

        /// <summary>
        /// 生成32位随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomStr()
        {
            Random random = new Random();
            return StringToMD5(random.Next(100000).ToString());
        }

        /// <summary>
        /// 获取当前系统时间的时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            return DateTime.Now.DateTimeToTimeStamp();
        }
    }
}