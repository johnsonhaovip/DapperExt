/*----------------------------------------------------------------
* 项目名称 ：WebApi.Common.Files
* 类 名 称 ：ApiUsageMonitorFilter
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi.Common.Files
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 13:53:19
* 更新时间 ：2018/10/15 13:53:19
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using XNY.Helper.Extensions;

namespace WebApi.Common.Files
{
    /// <summary>
    /// api调用统计
    /// 注意：此处需要在WebApiConfig.cs中注册，因为FilterConfig.cs只能注册Mvc命名空间下的IActionFilter
    /// </summary>
    public class ApiUsageMonitorFilter : ActionFilterAttribute, IActionFilter
    {
        private static DateTime _reqBeginDt = DateTime.Now;
        private static readonly ConcurrentDictionary<string, int> _reqCount = new ConcurrentDictionary<string, int>(StringComparer.Ordinal);

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var reqPath = actionContext.Request.RequestUri.LocalPath;
            var reqMethod = actionContext.Request.Method;

            _reqCount.AddOrUpdate(reqPath, 1, (a, b) =>
            {
                return b + 1;
            });

            //访问200次
            if (_reqCount.Count % 200 == 0)
            {
                var builder = new StringBuilder();
                builder.AppendLine(String.Format("**************** 分段记录开始时间 {0:yyyy/MM/dd HH:mm:ss}****************", _reqBeginDt));
                foreach (var _req in _reqCount)
                {
                    builder.AppendLine(String.Format("{0}：{1}", _req.Key, _req.Value));
                }

                builder.AppendLine(String.Format("**************** 分段记录结束时间 {0:yyyy/MM/dd HH:mm:ss}****************", DateTime.Now));

                LogHelper.Info(builder.ToString());

                _reqCount.Clear();
                _reqBeginDt = DateTime.Now;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}