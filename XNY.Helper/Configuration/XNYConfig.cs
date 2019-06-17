using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml;

namespace XNY.Helper.Configuration
{
    public partial class XNYConfig : IConfigurationSectionHandler
    {
        private static readonly object _locker = new object();
        
        /// <summary>
        /// 配置文件节点名称
        /// </summary>
        public const string CONFIG_SECTION_NAME = "XNYConfig";

        /// <summary>
        /// 创建配置文件
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new XNYConfig();
            var dynamicDiscoveryNode = section.SelectSingleNode("DynamicDiscovery");
            if (dynamicDiscoveryNode != null && dynamicDiscoveryNode.Attributes != null)
            {
                var attribute = dynamicDiscoveryNode.Attributes["Enabled"];
                if (attribute != null)
                    config.DynamicDiscovery = Convert.ToBoolean(attribute.Value);
            }

            var engineNode = section.SelectSingleNode("Engine");
            if (engineNode != null && engineNode.Attributes != null)
            {
                var attribute = engineNode.Attributes["Type"];
                if (attribute != null)
                    config.EngineType = attribute.Value;
            }

            var startupNode = section.SelectSingleNode("Startup");
            if (startupNode != null && startupNode.Attributes != null)
            {
                var attribute = startupNode.Attributes["IgnoreStartupTasks"];
                if (attribute != null)
                    config.IgnoreStartupTasks = Convert.ToBoolean(attribute.Value);
            }

            var themeNode = section.SelectSingleNode("Themes");
            if (themeNode != null && themeNode.Attributes != null)
            {
                var attribute = themeNode.Attributes["basePath"];
                if (attribute != null)
                    config.ThemeBasePath = attribute.Value;
            }

            var userAgentStringsNode = section.SelectSingleNode("UserAgentStrings");
            if (userAgentStringsNode != null && userAgentStringsNode.Attributes != null)
            {
                var attribute = userAgentStringsNode.Attributes["databasePath"];
                if (attribute != null)
                    config.UserAgentStringsPath = attribute.Value;
            }

            var cs = ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
            if (cs != null)
            {
                config.Debug = cs.Debug;
            }

            return config;
        }

        /// <summary>
        /// 获取配置的文件信息
        /// </summary>
        /// <returns></returns>
        private static XNYConfig _instance = null;
        public static XNYConfig Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = ConfigurationManager.GetSection(CONFIG_SECTION_NAME) as XNYConfig;
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// 动态读取
        /// </summary>
        public bool DynamicDiscovery { get; private set; }

        /// <summary>
        /// 引擎类型
        /// </summary>
        public string EngineType { get; private set; }

        /// <summary>
        /// 皮肤路径
        /// </summary>
        public string ThemeBasePath { get; private set; }

        /// <summary>
        /// 忽略其实任务
        /// </summary>
        public bool IgnoreStartupTasks { get; private set; }

        /// <summary>
        /// User-Agent
        /// </summary>
        public string UserAgentStringsPath { get; private set; }
        /// <summary>
        /// 运行模式
        /// </summary>
        public bool Debug { get; private set; }

        /// <summary>
        /// 是否执行依赖注入
        /// </summary>
        public bool IsResolve { get; private set; }
    }
}
