using System;
using System.Web;
using System.Web.Mvc;

namespace SWI.Helper.MvcWeb.Filter
{
    /// <summary>
    /// 页面缓存设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CacheFilterAttribute : OutputCacheAttribute
    {
        bool IgnoreChildCache(ControllerContext filterContext)
        {
            return filterContext.IsChildAction
                && (Location == System.Web.UI.OutputCacheLocation.None || Duration == 0);
        }

        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext != null)
            {
                if (IgnoreChildCache(filterContext))
                {
                    return;
                }
                else
                {
                    filterContext.HttpContext.Response.Cache.SetOmitVaryStar(true);
                    base.OnActionExecuting(filterContext);
                }
            }
        }
        /// <summary>
        /// 此方法是 System.Web.Mvc.IActionFilter.OnActionExecuted(System.Web.Mvc.ActionExecutedContext)
        /// 的实现并支持 ASP.NET MVC 基础结构。它不应直接在您的代码中使用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (IgnoreChildCache(filterContext))
            {
                return;
            }
            else
            {
                base.OnResultExecuted(filterContext);
            }
        }
        /// <summary>
        /// 在操作结果执行之前调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文，它封装有关使用 System.Web.Mvc.AuthorizeAttribute 的信息。</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (IgnoreChildCache(filterContext))
            {
                return;
            }
            else
            {
                base.OnResultExecuting(filterContext);
            }
        }
    }
}
