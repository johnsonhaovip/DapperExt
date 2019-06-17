using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNY.Helper.Configuration;

namespace XNY.Helper.Infrastructure {
    public interface IEngine {
        /// <summary>
        /// 容器管理
        /// </summary>
        ContainerManager ContainerManager { get; }

        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="config"></param>
        void Initialize(XNYConfig config);

        #region 依赖注入

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();

        #endregion
    }
}
