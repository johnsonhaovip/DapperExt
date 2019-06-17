/****************************************************************
 * 项目名称：XNY.Helper.Caching
 * 类 名 称：CacheExtensions
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.Caching
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/8/2 9:00:37
 * 更新时间：2018/8/2 9:47:37
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Caching {
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CacheExtensions {
        /// <summary>
        /// 获取缓存，如果缓存不存在，则通过指定方法获取并设置缓存60分钟。
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">缓存容器</param>
        /// <param name="key">关键字</param>
        /// <param name="acquire">获取缓存数据的方法</param>
        /// <returns></returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire) {
            return Get(cacheManager, key, 60, acquire);
        }

        /// <summary>
        /// 获取缓存，如果缓存不存在，则通过指定方法获取并设置缓存。
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">缓存容器</param>
        /// <param name="key">关键字</param>
        /// <param name="cacheTime">缓存时间（单位分钟），0为不缓存。</param>
        /// <param name="acquire">获取缓存数据的方法</param>
        /// <returns></returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire) {
            if (cacheManager.IsSet(key) && cacheManager.Get<T>(key) != null) {
                return cacheManager.Get<T>(key);
            } else {
                var result = acquire();
                if (cacheTime > 0)
                    cacheManager.Set(key, result, cacheTime);
                return result;
            }
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="cacheTime"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        public static T Update<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire) {
            if (cacheManager.IsSet(key)) {
                cacheManager.Remove(key);
            }

            var result = acquire();
            if (cacheTime > 0)
                cacheManager.Set(key, result, cacheTime);
            return result;
        }


        #region

        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Guid userGuid, Func<T> acquire) {
            if (cacheManager.IsSet(key) && cacheManager.Get<T>(key) != null) {
                return cacheManager.Get<T>(key);
            } else {
                var result = acquire();
                if (cacheTime > 0)
                    cacheManager.Set(key, result, cacheTime, userGuid);
                return result;
            }
        }

        public static T Update<T>(this ICacheManager cacheManager, string key, int cacheTime, Guid userGuid, Func<T> acquire) {
            if (cacheManager.IsSet(key)) {
                cacheManager.Remove(key);
            }

            var result = acquire();
            if (cacheTime > 0)
                cacheManager.Update(key, result, cacheTime, userGuid);
            return result;
        }

        #endregion

    }
}
