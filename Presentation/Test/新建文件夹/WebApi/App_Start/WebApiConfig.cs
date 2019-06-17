using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi.Common.Files;

namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //跨域请求配置
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));  //origins：限定的访问顶级域名，英文逗号分割
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //统计Api调用次数数据
            config.Filters.Add(new ApiUsageMonitorFilter());
        }
    }
}
