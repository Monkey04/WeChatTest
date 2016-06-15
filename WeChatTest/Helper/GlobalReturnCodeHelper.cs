using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WeChatTest.Helper
{
    public class GlobalReturnCodeHelper
    {
        /// <summary>
        /// 根据代码获取描述
        /// </summary>
        /// <param name="code"></param>
        public static string GetCodeDescription(int code)
        {
            Dictionary<int, string> returnCodeDictionary = GetCodeList();
            return returnCodeDictionary.Count > 0 ? returnCodeDictionary[code] : null;
        }

        public static Dictionary<int, string> GetCodeList()
        {
            Dictionary<int, string> returnCodeDictionary = new Dictionary<int, string>();
            using (System.IO.StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Document/GlobalReturnCode.txt")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    //把连续多个空格合并为一个
                    line = Regex.Replace(line, @"\s+", " ");
                    string[] codeGroup = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    int code;
                    if (codeGroup.Length != 2 || !int.TryParse(codeGroup[0], out code))
                    {
                        continue;
                    }
                    else
                    {
                        returnCodeDictionary.Add(code, codeGroup[1]);
                    }
                }
            }
            return returnCodeDictionary;
        }
    }
}