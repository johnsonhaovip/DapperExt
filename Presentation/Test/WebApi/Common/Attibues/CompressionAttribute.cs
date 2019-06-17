/*----------------------------------------------------------------
* 项目名称 ：WebApi.Common.Attibues
* 类 名 称 ：CompressionAttribute
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi.Common.Attibues
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:28:09
* 更新时间 ：2018/10/15 14:28:09
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebApi.Common.Attibues
{
    /// <summary>
    /// 压缩
    /// </summary>
    public class CompressionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.AppendHeader("Content-encoding", "gzip");
            response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            base.OnActionExecuted(filterContext);
        }
    }
}