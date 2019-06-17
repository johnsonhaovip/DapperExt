/*----------------------------------------------------------------
* 项目名称 ：Service.OAuths
* 类 名 称 ：OAuthClientAuthService
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：Service.OAuths
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:30:00
* 更新时间 ：2018/10/15 14:30:00
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using Core.OAuths;
using DAO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.OAuths
{
    /// <summary>
    /// token验证服务
    /// </summary>
    public class OAuthClientAuthService : RepositoryBase<OAuthClientAuthorizations>
    {
        public OAuthClientAuthService() { }
        public OAuthClientAuthService(string moduleName) : base(moduleName) { }

        /// <summary>
        /// 根据用户id获取
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public OAuthClientAuthorizations GetByAccountId(string accountId)
        {
            return this.GetAll().FirstOrDefault(w => w.AccountId == accountId);
        }

        /// <summary>
        /// 更新Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpDataToken(OAuthClientAuthorizations model)
        {
            return this.Update(model);
        }
        /// <summary>
        /// 根据Token获取实体
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public OAuthClientAuthorizations GetByToken(string token)
        {
            return this.GetAll().FirstOrDefault(c => c.Token == token);
        }

        /// <summary>
        /// 判断是否为有效Ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool GetByIp(string ip)
        {
            bool flag = false;
            try
            {
                var data = this.GetAll();
                if (data == null)
                {
                    return true;
                }
                else
                {
                    foreach (var item in data)
                    {
                        if (item.AccessIp == null || string.IsNullOrWhiteSpace(item.AccessIp))
                        {
                            return true;
                        }
                        else
                        {
                            var ips = item.AccessIp.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var itemIp in ips)
                            {
                                if (itemIp.Contains(ip))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                    return flag;
                }
            }
            catch (Exception)
            {
                return flag;
            }
        }
    }
}
