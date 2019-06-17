/*----------------------------------------------------------------
* 项目名称 ：WebApi.Models
* 类 名 称 ：Param
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：WebApi.Models
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 14:44:43
* 更新时间 ：2018/10/15 14:44:43
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 用户登录实体
    /// </summary>
    [Serializable]
    public class ParamStudent : AccessTokenBaseModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [DisplayName("id")]
        [Required(ErrorMessage = "缺少{0}")]
        public int id { get; set; }
        
    }
}