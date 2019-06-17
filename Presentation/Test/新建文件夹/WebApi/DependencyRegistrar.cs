/*----------------------------------------------------------------
* 项目名称 ：WebApi
* 类 名 称 ：DependencyRegistrar
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 15:13:16
* 更新时间 ：2018/10/15 15:13:16
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XNY.Helper.Infrastructure;

namespace WebApi
{
    /// <summary>
    /// WebApi依赖注入
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="typeFinder"></param>
        /// 创建者：蒋浩
        /// 创建时间：2018-6-21
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context
            if (HttpContext.Current != null)
            {
                builder.Register(c =>
                    (new HttpContextWrapper(HttpContext.Current) as HttpContextBase))
                    .As<HttpContextBase>()
                    .InstancePerLifetimeScope();
            }
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            //controllers
            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());            

        }

        /// <summary>
        /// 注入序列
        /// </summary>
        public int Order {
            get { return 0; }
        }

    }
}