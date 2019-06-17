using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using XNY.Helper.Configuration;

namespace XNY.Helper.Infrastructure
{
    /// <summary>
    /// 提供访问的西南院单例模式
    /// </summary>
    public class EngineContext
    {
        #region Utilities

        /// <summary>
        /// 创建工厂实例并添加HTTP应用程序注入设施
        /// </summary>
        /// <param name="config">Config</param>
        /// <returns>返回新的引擎实例</returns>
        protected static IEngine CreateEngineInstance(XNYConfig config)
        {
            if (config != null && !string.IsNullOrEmpty(config.EngineType))
            {
                var engineType = Type.GetType(config.EngineType);
                if (engineType == null)
                    throw new ConfigurationErrorsException("The type '" + config.EngineType + "' could not be found. Please check the configuration at /configuration/nop/engine[@engineType] or check for missing assemblies.");
                if (!typeof(IEngine).IsAssignableFrom(engineType))
                    throw new ConfigurationErrorsException("The type '" + engineType + "' doesn't implement 'IEngine' and cannot be configured in /configuration/nop/engine[@engineType] for that purpose.");
                return Activator.CreateInstance(engineType) as IEngine;
            }

            return new XNYEngine();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化西南院静态实例
        /// </summary>
        /// <param name="forceRecreate">创建新的工厂实例</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                var config = XNYConfig.Instance;
                Singleton<IEngine>.Instance = CreateEngineInstance(config);
                Singleton<IEngine>.Instance.Initialize(config);
            }
            return Singleton<IEngine>.Instance;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取用于访问西南院的单立引擎。
        /// </summary>
        public static IEngine Current {
            get {
                if (Singleton<IEngine>.Instance == null) {
                    Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
        }

        #endregion

    }
}
