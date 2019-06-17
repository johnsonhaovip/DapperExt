/*----------------------------------------------------------------
* 项目名称 ：WebApi.Common.Files
* 类 名 称 ：GlobalErrorFilter
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi.Common.Files
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:25:41
* 更新时间 ：2018/10/15 14:25:41
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using System;
using System.Web;
using System.Web.Mvc;
using XNY.Helper;
using XNY.Helper.Extensions;

namespace WebApi.Common.Files
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class GlobalErrorFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled == true)
            {
                return;
            }
            LogHelper.Error(filterContext.Exception);


            //设置异常已经处理,否则会被其他异常过滤器覆盖
            filterContext.ExceptionHandled = true;

            //在派生类中重写时，获取或设置一个值，该值指定是否禁用IIS自定义错误。
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            var exception = new HttpException(filterContext.Exception.Message, filterContext.Exception);
            filterContext.Result = new JsonResult()
            {
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new ResultJson()
                {
                    code = exception.GetHttpCode(),
                    message = exception.Message
                }
            };
        }
    }
}