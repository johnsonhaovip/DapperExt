/*----------------------------------------------------------------
* 项目名称 ：Core.OAuths
* 类 名 称 ：OAuthClientAuthorizations
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：Core.OAuths
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:30:49
* 更新时间 ：2018/10/15 14:30:49
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.OAuths
{
    /// <summary>
    /// oauthmodel
    /// </summary>
    public partial class OAuthClientAuthorizations : BaseEntity
    {
        public string Token { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public int ClientId { get; set; }
        /// <summary>
        /// 当前授权用户的UID(Person.Id)
        /// </summary>
        public string AccountId { get; set; }
        public string Scope { get; set; }

        /// <summary>
        /// 允许的ip地址
        /// </summary>
        public string AccessIp { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public Nullable<DateTime> UpdateDateUtc { get; set; }
    }
}
