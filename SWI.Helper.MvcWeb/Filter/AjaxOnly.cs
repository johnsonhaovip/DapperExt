using System;
using System.Web.Mvc;

namespace SWI.Helper.MvcWeb.Filter
{
    /// <summary>
    /// 只允许ajax请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class AjaxOnlyAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext != null && !filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new HttpNotFoundResult();
            }
        }
    }
    /// <summary>
    /// 只允许ajax请求与ChildAction
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AjaxOrChildActionOnlyAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext != null && !filterContext.HttpContext.Request.IsAjaxRequest() && !filterContext.IsChildAction)
            {
                filterContext.Result = new HttpNotFoundResult();
            }
        }
    }
}
