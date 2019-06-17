using System.Configuration;

namespace XNY.Helper.Configuration {

    /// <summary>
    /// 节点配置
    /// </summary>
    /// 创建者：蒋浩
    /// 创建时间：2018-5-21
    public class PubConstant {
        #region Config(节点名称,通过节点名称获取节点值)

        /// <summary>
        /// 日志配置文件节点名
        /// </summary>
        public const string LOG_CONFIG_PATH = "logConfigPath";      

        /// <summary>
        /// 网站配置文件节点名
        /// </summary>
        public const string SITE_CONFIG_PATH = "siteConfigPath";

        /// <summary>
        /// 数据库链接字符串名称
        /// </summary>
        public const string CONNECTION_SECTION_NAME = "DataContext";


        /// <summary>
        /// mongodb配置节点名
        /// </summary>
        public const string MONGO_CONFIG_PATH = "mongoDbConfigPath";

        /// <summary>
        /// Redis配置节点名
        /// </summary>
        public const string REDIS_CONFIG_PATH = "redisConfigPath";
        #endregion

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString {
            get {
                return GetConnectionString(CONNECTION_SECTION_NAME);
            }
        }

        /// <summary>
        /// 得到web.config里配置项的数据库连接字符串。
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string sectionName) {
            return ConfigurationManager.ConnectionStrings[sectionName].ConnectionString;
        }

        /// <summary>
        /// 获取设置的信息
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static string GetAppSetting(string sectionName) {
            return ConfigurationManager.AppSettings[sectionName];
        }

    }
}
