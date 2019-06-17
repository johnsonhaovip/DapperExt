/****************************************************************
 * 项目名称：XNY.Helper.Caching
 * 类 名 称：MongodbCacheManager
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.Caching
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/8/2 9:53:23
 * 更新时间：2018/8/2 9:53:23
 * 
 * 描述说明：
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Johnson 2018 All rights reserved
*****************************************************************/

using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using XNY.Helper.Configuration;

namespace XNY.Helper.Caching {

    #region 缓存对象实体模型

    public class CacheRecords {
        public CacheRecords() { }
        public CacheRecords(string _cacheKey, object _cacheValue) {
            this.Key = _cacheKey;
            this.Value = _cacheValue;
        }
        public CacheRecords(string _cacheKey, object _cacheValue, int _expiration, Guid _userGuid) {
            this.Key = _cacheKey;
            this.Value = _cacheValue;
            this.Expiration = DateTime.Now.AddMinutes(_expiration);
            this.UserGuid = _userGuid;
        }

        public ObjectId _id { get; set; }
        public Guid UserGuid { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public DateTime Expiration { get; set; }
    }

    #endregion

    public class MongodbCacheManager : ICacheManager {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDb;
        private readonly MongoDbConfig _mongoDbCfg;
        private readonly XNYConfig _xnyCfg;

        public MongodbCacheManager() {
            this._xnyCfg = XNYConfig.Instance;
            this._mongoDbCfg = MongoDbConfig.Instance;
            this._mongoClient = new MongoClient(this._mongoDbCfg.Connection.Value);

            #region 根据环境判断获取链接数据库

            string _cacheDb = string.Concat(this._xnyCfg.Debug ? "test_" : "", this._mongoDbCfg.MongoCache.Name);
            this._mongoDb = _mongoClient.GetDatabase(_cacheDb);

            #endregion
        }

        private IFindFluent<CacheRecords, CacheRecords> GetRecords(string key) {
            var _collection = _mongoDb.GetCollection<CacheRecords>(this._mongoDbCfg.MongoCache.CollectionName);
            var filter = Builders<CacheRecords>.Filter.Eq("Key", key);
            return _collection.Find(filter);
        }

        public T Get<T>(string key) {
            try {
                CacheRecords model = this.GetRecords(key).FirstOrDefault();
                if (model != null && model.Value != null) {
                    return JsonConvert.DeserializeObject<T>(model.Value.ToString());
                }
            } catch (Exception) {

            }
            return default(T);
        }

        public void Set(string key, object data, int cacheTime) {
            if (data == null)
                return;

            string serializerData = JsonConvert.SerializeObject(data);

            var _collection = _mongoDb.GetCollection<CacheRecords>(this._mongoDbCfg.MongoCache.CollectionName);
            _collection.InsertOne(new CacheRecords(key, serializerData));
        }

        public bool IsSet(string key) {
            try {
                var _records = this.GetRecords(key);
                return _records != null && _records.Count() > 0;
            } catch (Exception) {
                return false;
            }
        }

        public void Remove(string key) {
            var _collection = _mongoDb.GetCollection<CacheRecords>(this._mongoDbCfg.MongoCache.CollectionName);
            var filter = Builders<CacheRecords>.Filter.Eq("Key", key);
            _collection.FindOneAndDelete(filter);
        }

        public void Update(string key, object data, int cacheTime) {
            if (IsSet(key))
                Remove(key);
            Set(key, data, cacheTime);
        }

        public void RemoveByPattern(string pattern) {
            var _collection = _mongoDb.GetCollection<CacheRecords>(this._mongoDbCfg.MongoCache.CollectionName);
            var filter = Builders<CacheRecords>.Filter.Regex("Key", new BsonRegularExpression(pattern));
            _collection.DeleteMany(filter);
        }

        /// <summary>
        /// 移除所有缓存数据
        /// </summary>
        public void Clear() {
            var _collection = _mongoDb.GetCollection<CacheRecords>(this._mongoDbCfg.MongoCache.CollectionName);
            _collection.DeleteMany(new BsonDocument());
        }

        #region db

        public void Set(string key, object data, int cacheTime, Guid userGuid) {
            if (data == null)
                return;

            string serializerData = JsonConvert.SerializeObject(data);
            var _collection = _mongoDb.GetCollection<CacheRecords>(this._mongoDbCfg.MongoCache.CollectionName);

            CacheRecords _records = new CacheRecords(key, serializerData, cacheTime, userGuid);
            _collection.InsertOne(_records);
        }
        public void Remove(Guid userGuid) {
            try {
                var _collection = _mongoDb.GetCollection<CacheRecords>(this._mongoDbCfg.MongoCache.CollectionName);
                var filter = Builders<CacheRecords>.Filter.Eq("UserGuid", userGuid);
                Task.Run(() => {
                    _collection.DeleteMany(filter);
                });
            } catch {
            }
        }
        public void Update(string key, object data, int cacheTime, Guid userGuid) {
            if (IsSet(key))
                Remove(key);
            Set(key, data, cacheTime, userGuid);
        }

        #endregion

    }
}
