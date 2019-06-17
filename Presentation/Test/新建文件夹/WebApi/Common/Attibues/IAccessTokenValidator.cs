/*----------------------------------------------------------------
* 项目名称 ：WebApi.Common.Attibues
* 类 名 称 ：IAccessTokenValidator
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi.Common.Attibues
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:33:08
* 更新时间 ：2018/10/15 14:33:08
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using XNY.Helper;

namespace WebApi
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface IAccessTokenValidator
    {

        /// <summary>
        /// 验证用户并更新Token
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        ResultJson<string> ValidateAccount(string accountId, string passWord);

        /// <summary>
        /// 验证token接口
        /// </summary>
        /// <param name="token"></param>
        /// <param name="scope"></param>
        /// <param name="exHour">过期几小时</param>
        /// <returns></returns>
        ResultJson<string> ValidateToken(string token, string[] scope, int exHour = 12);

        /// <summary>
        /// 验证Ip地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        ResultJson<string> ValidateIp(string ip);
    }
}