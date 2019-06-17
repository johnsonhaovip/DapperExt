/****************************************************************
 * 项目名称：XNY.Helper.Caching
 * 类 名 称：RedisManager
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.Caching
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/8/2 10:01:31
 * 更新时间：2018/8/2 10:01:31
 * 
 * 描述说明：
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Johnson 2018 All rights reserved
*****************************************************************/

using ServiceStack.Redis;
using System;
using XNY.Helper.Configuration;

namespace XNY.Helper.Caching {
    /// <summary>
    /// Redies管理
    /// </summary>
    public class RedisManager : ICacheManager {
        /// <summary>
        /// Redis配置
        /// </summary>
        private readonly RedisDbConfig _rdsCfg;
        private readonly SiteConfig _siteCfg;

        /// <summary>
        /// 连接客户端
        /// </summary>
        protected static RedisClient _redisClient;

        public RedisManager() {
            this._rdsCfg = RedisDbConfig.Instance;
            this._siteCfg = SiteConfig.Instance;

            string redisHost = string.IsNullOrEmpty(this._siteCfg.IM.Host) ? this._rdsCfg.Host : this._siteCfg.IM.Host;
            _redisClient = new RedisClient(redisHost, this._rdsCfg.Port, this._rdsCfg.Password, this._rdsCfg.Db);

            _redisClient.ConnectTimeout = 1000;
        }

        /// <summary>
        /// 根据key获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key) {
            return _redisClient.Get<T>(key);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public void Set<T>(string key, T data, int cacheTime) {
            if (data == null)
                return;

            _redisClient.Set<T>(key, data);
        }

        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsSet(string key) {
            var hasKey = _redisClient.ContainsKey(key);
            return hasKey;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) {
            _redisClient.Remove(key);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public void Update(string key, object data, int cacheTime) {
            if (IsSet(key))
                Remove(key);
            Set(key, data, cacheTime);
        }
        /// <summary>
        /// 根据模式移除缓存
        /// </summary>
        /// <param name="pattern"></param>
        public void RemoveByPattern(string pattern) {
            _redisClient.RemoveByPattern(pattern);
        }
        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear() {
            _redisClient.RemoveAll(_redisClient.GetAllKeys());
        }

        public void Set(string key, object data, int cacheTime) {
            if (data == null) return;
            _redisClient.Set(key, data);
        }

        public void Set(string key, object data, int cacheTime, Guid userGuid) {
            if (data == null) return;
            _redisClient.Set(key, data, DateTime.Now.AddMinutes(cacheTime));
        }

        public void Remove(Guid userGuid) {
            throw new NotImplementedException();
        }

        public void Update(string key, object data, int cacheTime, Guid userGuid) {
            throw new NotImplementedException();
        }
    }
}
