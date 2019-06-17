/****************************************************************
 * 项目名称：XNY.Helper.Configuration
 * 类 名 称：MongoDbConfig
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.Configuration
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/8/2 9:59:26
 * 更新时间：2018/8/2 9:59:26
 * 
 * 描述说明：
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Johnson 2018 All rights reserved
*****************************************************************/

using System;
using System.Xml.Serialization;


namespace XNY.Helper.Configuration {

    /// <summary>
    /// MongoDb配置
    /// </summary>
    [Serializable, XmlRoot("mongo")]
    public sealed class MongoDbConfig : ConfigLoader<MongoDbConfig> {
        #region Ctor
        public MongoDbConfig()
            : base(PubConstant.MONGO_CONFIG_PATH) { }
        #endregion

        #region Properties

        /// <summary>
        /// 数据库连接
        /// </summary>
        [XmlElement("connection")]
        public MongoConnection Connection { get; set; }

        /// <summary>
        /// 默认数据库
        /// </summary>
        [XmlElement("defaultDb")]
        public MongoDb DefaultDb { get; set; }

        /// <summary>
        /// 统计数据 库名
        /// </summary>
        [XmlElement("statisticsDb")]
        public MongoDb StatisticsDb { get; set; }

        /// <summary>
        /// 缓存数据库
        /// </summary>
        [XmlElement("cacheDb")]
        public MongoCacheDb MongoCache { get; set; }

        #endregion

        #region Methods
        protected override MongoDbConfig initConfig() {
            var config = new MongoDbConfig() {
                Connection = new MongoConnection() { Value = "mongodb://root:swi2018@10.172.178.218:27017" },
                StatisticsDb = new MongoDb() {
                    Name = "StatisticDb"
                },
                MongoCache = new MongoCacheDb() {
                    Name = "swiCaching",
                    CollectionName = "CacheRecords"
                }
            };

            return this.saveConifg(config);
        }
        #endregion
    }

    /// <summary>
    /// db连接配置
    /// </summary>
    [Serializable]
    public sealed class MongoConnection {
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// db通用配置
    /// </summary>
    [Serializable]
    public class MongoDb {
        /// <summary>
        /// 数据库名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 缓存数据库配置
    /// </summary>
    [Serializable]
    public sealed class MongoCacheDb : MongoDb {
        /// <summary>
        /// 缓存数据集合(表)名称
        /// </summary>
        [XmlElement("collection")]
        public string CollectionName { get; set; }
    }
}
