/****************************************************************
 * 项目名称：XNY.Helper.Configuration
 * 类 名 称：RedisDbConfig
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.Configuration
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/8/2 10:04:29
 * 更新时间：2018/8/2 10:04:29
 * 
 * 描述说明：
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Johnson 2018 All rights reserved
*****************************************************************/

using System;
using System.Xml;
using System.Xml.Serialization;

namespace XNY.Helper.Configuration {

    /// <summary>
    /// Redis 配置
    /// </summary>
    [Serializable, XmlRoot("redis")]
    public class RedisDbConfig : ConfigLoader<RedisDbConfig> {
        #region Ctor
        public RedisDbConfig()
            : base(PubConstant.REDIS_CONFIG_PATH) { }
        #endregion

        #region Properties

        /// <summary>
        /// db host
        /// </summary>
        private string host = "127.0.0.1";
        [XmlElement("host")]
        public string Host {
            get { return host; }
            set { host = value; }
        }

        /// <summary>
        /// 端口(默认6379)
        /// </summary>
        private int port = 6379;
        [XmlElement("port")]
        public int Port {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("password")]
        public string Password { get; set; }

        /// <summary>
        /// db编号(默认0, max:16384)
        /// </summary>
        private int db = 0;
        [XmlElement("db")]
        public int Db {
            get { return db; }
            set { db = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        /// <returns></returns>
        protected override RedisDbConfig initConfig() {
            var config = new RedisDbConfig() {
                Host = "127.0.0.1",
                Port = 6379,
                Password = "swi2018",
                Db = 0
            };
            return this.saveConifg(config);
        }

        #endregion
    }
}
