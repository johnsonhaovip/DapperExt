/*----------------------------------------------------------------
* 项目名称 ：Core.Test
* 类 名 称 ：Student
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：Core.Test
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 13:29:57
* 更新时间 ：2018/10/15 13:29:57
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

namespace Core.Test
{
    /// <summary>
    /// Student实体
    /// 注：Student名称对应为数据库中的Student表；
    /// 下面的字段也需要和数据库中的Student表中的字段一一对应
    /// </summary>
    public class Student
    {
        public Guid Id { get; set; }

        public Guid Number { get; set; }

        public string Sex { get; set; }

        public string Phone { get; set; }
    }
}
