using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XNY.Helper.Web
{
    /// <summary>
    /// Cookie操作的帮助类 
    /// </summary>
    /// 创建者：蒋浩
    /// 创建时间：218-3-25
    public static class CookieHelper
    {

        /// <summary>
        /// 清除指定的Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        public static void ClearCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 根据cookieName获取对应Cookie的值
        /// </summary>
        /// <param name="cookieName">CookieName</param>
        /// <returns>返回cookie的值</returns>
        public static string GetCookieValue(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            string result;
            if (cookie != null)
                result = cookie.Value;
            else
                result = null;
            return result;
        }

        /// <summary>
        /// 添加一个Cookie
        /// </summary>
        /// <param name="cookieName">cookieName</param>
        /// <param name="cookieValue">cookie值</param>
        public static void SetCookie(string cookieName, string cookieValue)
        {
            HttpCookie cookie = new HttpCookie(cookieName)
            {
                Value = cookieValue
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 添加一个cookie
        /// </summary>
        /// <param name="cookieName">检索Cookie的索引</param>
        /// <param name="cookieValue">Cookie的值</param>
        /// <param name="expires">过期时间(指定的分钟数加到DateTime.Now的值上)</param>
        public static void SetCookie(string cookieName, string cookieValue, int expires)
        {
            SetCookie(cookieName, cookieValue, DateTime.Now.AddMinutes(expires));
        }

        /// <summary>
        /// 添加一个cookie
        /// </summary>
        /// <param name="cookieName">cookieName</param>
        /// <param name="cookieValue">cookieValue</param>
        /// <param name="expires">过期时间</param>
        public static void SetCookie(string cookieName, string cookieValue, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(cookieName)
            {
                Value = cookieValue,
                Expires = expires
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


    }
}
