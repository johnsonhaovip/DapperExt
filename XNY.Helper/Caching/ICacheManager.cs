/****************************************************************
 * 项目名称：XNY.Helper.Caching
 * 类 名 称：ICacheManager
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.Caching
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/8/2 9:00:57
 * 更新时间：2018/8/2 9:46:57
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
    /// Cache manager interface
    /// </summary>
    public interface ICacheManager {
        /// <summary>
        /// 获取或设置与指定的键相关联的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 添加指定的键和对象的缓存，单位分钟。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">缓存时间，单位分钟</param>
        //void Set(string key, object data, int cacheTime);
        void Set(string key, object data, int cacheTime);

        /// <summary>
        /// 判断是否已缓存关键字
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        bool IsSet(string key);

        /// <summary>
        /// 移除指定缓存
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        ///  更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        void Update(string key, object data, int cacheTime);

        /// <summary>
        /// 通过正则表达式删除缓存
        /// </summary>
        /// <param name="pattern">pattern</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// 清空缓存数据
        /// </summary>
        void Clear();


        #region for db

        void Set(string key, object data, int cacheTime, Guid userGuid);
        void Remove(Guid userGuid);
        void Update(string key, object data, int cacheTime, Guid userGuid);

        #endregion
    }
}
