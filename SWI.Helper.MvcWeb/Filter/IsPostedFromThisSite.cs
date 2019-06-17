using System;
using System.Web;
using System.Web.Mvc;

namespace SWI.Helper.MvcWeb.Filter {
    /// <summary>
    /// 跨站请求屏蔽,盗链屏蔽
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class IsPostedFromThisSiteAttribute : ActionFilterAttribute {
        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext != null && filterContext.HttpContext != null) {
                if (filterContext.HttpContext.Request.UrlReferrer == null)
                    throw new System.Web.HttpException("Invalid submission");
                if (!string.Equals(
                        GetRootDomain(filterContext.HttpContext.Request.UrlReferrer.Host),
                        GetRootDomain(filterContext.HttpContext.Request.Url.Host),
                        StringComparison.CurrentCultureIgnoreCase))
                    throw new System.Web.HttpException("This form wasn't submitted from this site!");
            }
        }

        private string GetRootDomain(string domain) {
            if (string.IsNullOrEmpty(domain)) {
                throw new ArgumentNullException("参数'domain'不能为空");
            }
            string[] arr = domain.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length <= 2) {
                return arr[0].ToString();
            } else {
                return arr[arr.Length - 2].ToString();
            }
        }
    }
}
