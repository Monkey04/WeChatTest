using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace WeChatTest.Helper
{
    public class CacheHelper
    {
        private static Cache cache = HttpRuntime.Cache;

        /// <summary>
        /// 获取单个缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetCache(string key)
        {
            return cache[key];
        }

        /// <summary>
        /// 判断缓存是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsCacheExist(string key)
        {
            return GetCache(key) != null;
        }

        /// <summary>
        /// 移除单个缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveCache(string key)
        {
            cache.Remove(key);
        }

        /// <summary>
        /// 新增/覆盖单个缓存
        /// </summary>
        /// <param name="key">唯一的缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="second">缓存有效时间（秒），若为0，则有效时间为永久</param>
        public static void AddCache(string key, object value, long second)
        {
            cache.Insert(key, value, null, second != 0 ? DateTime.Now.AddSeconds(second) : DateTime.MaxValue, TimeSpan.Zero);
        }

        /// <summary>
        /// 新增/覆盖单个缓存
        /// </summary>
        /// <param name="key">唯一的缓存键</param>
        /// <param name="value">缓存值</param>
        public static void AddCache(string key, object value)
        {
            AddCache(key, value, 0);
        }
    }
}