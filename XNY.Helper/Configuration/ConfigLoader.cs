using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace XNY.Helper.Configuration {
    /// <summary>
    /// 基础配置结构
    /// </summary>
    /// 创建者：蒋浩
    /// 创建时间：2018-5-21
    [Serializable]
    public abstract class ConfigLoader<T> where T : ConfigLoader<T>, new() {
        #region Fields

        /// <summary>
        /// 文件锁
        /// </summary>
        private static readonly object _locker = new object();

        /// <summary>
        /// 实例锁
        /// </summary>
        private static readonly object _instance_locker = new object();

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private readonly string _configFilePath;

        /// <summary>
        /// 缓存
        /// </summary>
        private static ConcurrentDictionary<int, T> _cache;

        /// <summary>
        /// 缓存键
        /// </summary>
        private int _cacheHashCode { get { return _configFilePath.GetHashCode(); } }

        #endregion

        public ConfigLoader(string settingKey) {
            _cache = new ConcurrentDictionary<int, T>();

            var _setting_key = Utils.GetAppSetting(settingKey);

            if (XNYConfig.Instance.Debug) {
                var _separator = _setting_key.Contains("/") ? "/" : "\\";
                var _path = _setting_key.Split(_separator[0]).ToList();
                _path.Insert(_path.Count - 1, "debug");

                _configFilePath = Utils.GetXMLMapPath(string.Join(_separator, _path));
            }

            if (!File.Exists(_configFilePath)) {
                _configFilePath = Utils.GetXMLMapPath(_setting_key);
            }
        }

        #region Methods
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        protected virtual T loadConfig() {
            if (_cache.ContainsKey(this._cacheHashCode)) {
                return _cache[this._cacheHashCode];
            }

            T model = default(T);

            if (!existsConfigFile) model = this.initConfig();
            else model = SerializationHelper.Load(typeof(T), this._configFilePath) as T;

            _cache[this._cacheHashCode] = model;

            return model;
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual T saveConifg(T model) {
            SerializationHelper.Save(model, this._configFilePath);
            _cache[this._cacheHashCode] = model;
            return model;
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <returns></returns>
        protected virtual bool existsConfigFile {
            get {
                if (!_cache.ContainsKey(this._cacheHashCode)) return File.Exists(this._configFilePath);
                return true;
            }
        }

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        /// <returns></returns>
        protected abstract T initConfig();

        /// <summary>
        /// 获取实例对象
        /// </summary>
        private static T _instance = null;
        public static T Instance {
            get {
                lock (_locker) {
                    if (_instance == null) _instance = new T().loadConfig();
                }
                return _instance;
            }
        }

        #endregion
    }

    #region include class


    [Serializable]
    [XmlRoot("app")]
    public class AppElement {
        /// <summary>
        /// 应用标识
        /// </summary>
        [XmlElement("id")]
        public virtual string Id { get; set; }

        /// <summary>
        /// 应用密码
        /// </summary>
        [XmlElement("secret")]
        public virtual string Secret { get; set; }
    }

    /// <summary>
    /// api详情
    /// </summary>
    [Serializable]
    public class ApiElement {
        /// <summary>
        /// api调用标识名称
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// api地址
        /// </summary>
        [XmlAttribute("url")]
        public string Url { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlAttribute("description")]
        public string Description;
    }

    #endregion
}
