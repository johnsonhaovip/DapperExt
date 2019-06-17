/*----------------------------------------------------------------
* 项目名称 ：Service.Test
* 类 名 称 ：StudentService
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：Service.Test
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 13:32:06
* 更新时间 ：2018/10/15 13:32:06
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using Core.Test;
using DAO.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Test
{
    /// <summary>
    /// 这里是简单实例
    /// 常规简单、数据比较少的情况下，可以直接采用Service【类似EF】这种方式操作；
    /// 逻辑比较复杂又为了兼顾效率问题，也可以采用ApiService的那种方式；
    /// </summary>
    public class StudentService : RepositoryBase<Student>
    {
        public StudentService(string moduleName) : base(moduleName) { }
        public StudentService() { }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetAllData() {
            return this.GetAll();
        }
    }
}
