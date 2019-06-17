/*----------------------------------------------------------------
* 项目名称 ：WebApi.Common.Attibues
* 类 名 称 ：AccessTokenValidator
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi.Common.Attibues
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:28:57
* 更新时间 ：2018/10/15 14:28:57
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using Service.OAuths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Common.Attibues;
using XNY.Helper;
using XNY.Helper.Constants;
using XNY.Helper.Extensions;

namespace WebApi
{
    /// <summary>
    /// 验证token和ip
    /// </summary>
    public class AccessTokenValidator : IAccessTokenValidator
    {

        private OAuthClientAuthService clientAuthService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleName"></param>
        public AccessTokenValidator(string moduleName)
        {
            clientAuthService = new OAuthClientAuthService(moduleName);
        }
        /// <summary>
        /// 验证用户并更新Token
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public ResultJson<string> ValidateAccount(string accountId, string passWord)
        {
            var result = new ResultJson<string>();
            if (string.IsNullOrWhiteSpace(accountId) || string.IsNullOrWhiteSpace(passWord))
            {
                result.code = (int)EnumErrors.OAuth_NotValidAccount;
                result.message = EnumErrors.OAuth_NotValidAccount.ToDescription();
                return result;
            }
            var account = clientAuthService.GetByAccountId(accountId);
            if (account == null)
            {
                result.code = (int)EnumErrors.OAuth_NotValidAccount;
                result.message = EnumErrors.OAuth_NotValidAccount.ToDescription();
                return result;
            }
            else
            {
                if (account.PassWord.ToLower() == passWord.ToLower())
                {
                    account.Token = Guid.NewGuid().ToString("N");
                    account.UpdateDateUtc = DateTime.UtcNow;
                    bool isSuccess = clientAuthService.UpDataToken(account);
                    if (isSuccess)
                    {
                        result.code = 0;
                        result.message = "获取Token成功";
                        result.data = account.Token;
                    }
                    else
                    {
                        result.code = 1;
                    }
                    return result;
                }
                else
                {
                    result.code = (int)EnumErrors.OAuth_NotValidAccount;
                    result.message = EnumErrors.OAuth_NotValidAccount.ToDescription();
                    return result;
                }
            }

        }
        /// <summary>
        /// 验证ip地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public ResultJson<string> ValidateIp(string ip)
        {
            var result = new ResultJson<string>();
            if (string.IsNullOrWhiteSpace(ip))
            {
                result.code = (int)EnumErrors.OAuth_NotValidIp;
                result.message = EnumErrors.OAuth_NotValidIp.ToDescription();
                return result;
            }
            bool isAccessIp = clientAuthService.GetByIp(ip);
            if (!isAccessIp)
            {
                result.code = (int)EnumErrors.OAuth_NotValidIp;
                result.message = EnumErrors.OAuth_NotValidIp.ToDescription();
                return result;
            }
            return result;
        }
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="scope"></param>
        /// <param name="exHour">过期时间（几小时）</param>
        /// <returns></returns>
        public ResultJson<string> ValidateToken(string token, string[] scope, int exHour)
        {
            var result = new ResultJson<string>();
            if (string.IsNullOrEmpty(token))
            {
                result.code = (int)EnumErrors.OAuth_NotValidToken;
                result.message = EnumErrors.OAuth_NotValidToken.ToDescription();
                return result;
            }

            var auth = clientAuthService.GetByToken(token);
            if (auth == null)
            {
                result.code = (int)EnumErrors.OAuth_NotValidToken;
                result.message = EnumErrors.OAuth_NotValidToken.ToDescription();
                return result;
            }

            if (auth.UpdateDateUtc.Value < DateTime.UtcNow.AddHours(-(exHour)))
            {
                result.code = (int)EnumErrors.OAuth_AccessTokenExpired;
                result.message = EnumErrors.OAuth_AccessTokenExpired.ToDescription();
                return result;
            }
            return result;
        }
    }
}