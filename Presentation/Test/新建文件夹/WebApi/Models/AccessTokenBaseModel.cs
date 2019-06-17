/*----------------------------------------------------------------
* 项目名称 ：WebApi.Models
* 类 名 称 ：AccessTokenBaseModel
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi.Models
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:48:18
* 更新时间 ：2018/10/15 14:48:18
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebApi.Models
{
    /// <summary>
    /// access token 基类
    /// </summary>
    [Serializable, DataContract]
    public class AccessTokenBaseModel
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        [Required(ErrorMessage = "缺少参数{0}")]
        [DisplayName("访问令牌")]
        [IgnoreDataMember]
        public string access_token { get; set; }
    }
}