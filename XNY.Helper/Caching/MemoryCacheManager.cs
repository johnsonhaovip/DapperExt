/****************************************************************
 * 项目名称：XNY.Helper.Caching
 * 类 名 称：MemoryCacheManager
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.Caching
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/8/2 9:48:33
 * 更新时间：2018/8/2 9:48:33
 * 
 * 描述说明：
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Johnson 2018 All rights reserved
*****************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace XNY.Helper.Caching {
    /// <summary>
    /// 表示用于在HTTP请求（长期缓存）之间进行缓存的管理器。
    /// </summary>
    public partial class MemoryCacheManager : ICacheManager {
        protected ObjectCache Cache {
            get {
                return MemoryCache.Default;
            }
        }

        /// <summary>
        /// 获取或设置与指定的键相关联的值。
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T Get<T>(string key) {
            return (T)Cache[key];
        }

        /// <summary>
        /// 添加指定的键和对象的缓存，单单位分钟。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">缓存时间，单位分钟</param>
        public virtual void Set(string key, object data, int cacheTime) {
            if (data == null)
                return;

            var policy = new CacheItemPolicy();
            policy.SlidingExpiration = TimeSpan.FromMinutes(cacheTime); //设置为滑动过期
            Cache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        /// 判断是否已缓存关键字
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public virtual bool IsSet(string key) {
            return (Cache.Contains(key));
        }

        /// <summary>
        /// 移除指定缓存
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key) {
            Cache.Remove(key);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public virtual void Update(string key, object data, int cacheTime) {
            if (IsSet(key))
                Remove(key);
            Set(key, data, cacheTime);
        }

        /// <summary>
        /// 通过正则表达式删除缓存
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern) {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<string>();

            foreach (var item in Cache)
                if (regex.IsMatch(item.Key))
                    keysToRemove.Add(item.Key);

            foreach (string key in keysToRemove) {
                Remove(key);
            }
        }

        /// <summary>
        /// 清空缓存数据
        /// </summary>
        public virtual void Clear() {
            foreach (var item in Cache)
                Remove(item.Key);
        }


        public void Set(string key, object data, int cacheTime, Guid userGuid) {
            //throw new NotImplementedException();
            this.Set(key, data, cacheTime);
        }

        public void Remove(Guid userGuid) {
            throw new NotImplementedException();
        }

        public void Update(string key, object data, int cacheTime, Guid userGuid) {
            //throw new NotImplementedException();
            this.Update(key, data, cacheTime);
        }
    }
}
