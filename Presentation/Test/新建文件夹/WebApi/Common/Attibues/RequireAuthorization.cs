/*----------------------------------------------------------------
* 项目名称 ：WebApi.Common.Attibues
* 类 名 称 ：RequireAuthorization
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi.Common.Attibues
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:39:09
* 更新时间 ：2018/10/15 14:39:09
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using XNY.Helper.Extensions;

namespace WebApi
{
    /// <summary>
    /// 权限验证器
    /// </summary>
    public class RequireAuthorization : ActionFilterAttribute
    {
        #region Fields
        private static string ModuleName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleName"></param>
        public RequireAuthorization(string moduleName)
        {
            ModuleName = moduleName;
        }
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public RequireAuthorization() { }
        #endregion

        /// <summary>
        /// 权限
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 可访问的Ip
        /// </summary>
        public string AccessIp { get; set; }

        /// <summary>
        /// 执行action时验证token
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                #region 请求记录

                var builder = new StringBuilder();
                builder.AppendLine(String.Format("ApiName：{0}", actionContext.ActionDescriptor.ActionName));
                builder.AppendLine(String.Format("IP：{0}", XNY.Helper.String.ValueHelper.GetClientIP()));

                #region 获取请求Ip
                this.AccessIp = XNY.Helper.String.ValueHelper.GetClientIP();
                #endregion

                #endregion


                #region 获取access_token

                if (actionContext.Request.Method == HttpMethod.Get)
                {
                    string query = actionContext.Request.RequestUri.Query;
                    this.AccessToken = HttpUtility.ParseQueryString(query).Get("access_token");

                }
                else
                {
                    this.AccessToken = HttpContext.Current.Request.Form["access_token"];
                    builder.AppendLine(String.Format("[POST]FormData：{0}", HttpContext.Current.Request.Form.ToString()));
                }

                #endregion
                LogHelper.Info(builder.ToString());
            }
            catch (Exception ex)
            {
                var error = String.Format("actionContext：{0} Access {1} ", actionContext.Request.Method, ex.Message);
            }
            if (!string.IsNullOrWhiteSpace(AccessIp))
            {
                IAccessTokenValidator accessTokenValidator = new AccessTokenValidator(ModuleName);
                var validIp = accessTokenValidator.ValidateIp(AccessIp);
                if (validIp.code > 0)
                {
                    var response = new HttpResponseMessage
                    {
                        Content =
                          new StringContent("This ip is not valid"),
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                    var exception = new HttpResponseException(response);
                    throw new HttpResponseException(response);
                }

            }
            // we first check for valid token
            if (!String.IsNullOrWhiteSpace(AccessToken))
            {
                IAccessTokenValidator accessTokenValidator = new AccessTokenValidator(ModuleName);
                var validToken = accessTokenValidator.ValidateToken(AccessToken, (this.Scope ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                if (validToken.code > 0)
                {
                    var response = new HttpResponseMessage
                    {
                        Content =
                            new StringContent("This token is not valid, please refresh token or obtain valid token!"),
                        StatusCode = HttpStatusCode.Unauthorized
                    };

                    var exception = new HttpResponseException(response);
                    throw new HttpResponseException(response);
                }
            }
            else
            {
                var response = new HttpResponseMessage
                {
                    Content =
                        new StringContent("You must supply valid token to access method!"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                var exception = new HttpResponseException(response);
                throw exception;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}