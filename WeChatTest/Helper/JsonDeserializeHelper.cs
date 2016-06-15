using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace WeChatTest
{
    public class JsonDeserializeHelper
    {
        private string _JsonString { get; set; }
        private Dictionary<string, object> _rootDictionary { get; set; }
        public string JsonString { get { return _JsonString; } }
        public Dictionary<string, object> rootDictionary { get { return _rootDictionary; } }

        public JsonDeserializeHelper(string jsonStr)
        {
            _JsonString = jsonStr;
            JavaScriptSerializer jser = new JavaScriptSerializer();
            _rootDictionary = jser.Deserialize<Dictionary<string, object>>(_JsonString);
        }

        public object GetValue(string patternStr)
        {
            if (patternStr.IsEmpty()) return null;
            object result = null;
            bool isValid = true;
            string[] patternArray = patternStr.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < patternArray.Length; i++)
            {
                string patternSingle = patternArray[i];
                if (isValid)
                {
                    isValid = GetValueByDictionary(patternSingle, ref result);
                }
            }

            return isValid ? result : null;
        }

        private bool GetValueByDictionary(string _patternSingle, ref object value)
        {
            try
            {
                //object value = null;
                Regex reg = new Regex(@"\[""(?<name>\w+?)""\]|\['(?<name>\w+?)'\]|\[(?<name>\d+?)\]");
                MatchCollection regResult = reg.Matches(_patternSingle);
                if (regResult.Count > 0)
                {
                    //根据索引或键取值，如a["a"][0]
                    string objKey = _patternSingle.Substring(0, _patternSingle.IndexOf("["));
                    value = value == null ? _rootDictionary[objKey] : value;
                    for (int i = 0; i < regResult.Count; i++)
                    {
                        string key = regResult[i].Groups["name"].Value;
                        int intKey;
                        if (int.TryParse(key, out intKey))
                        {
                            value = ((ArrayList)value)[intKey];
                        }
                        else
                        {
                            value = ((Dictionary<string, object>)value)[key];
                        }
                    }
                }
                else
                {
                    //根据键取值
                    value = value == null ? _rootDictionary[_patternSingle] : ((Dictionary<string, object>)value)[_patternSingle];
                }

                return value != null;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}