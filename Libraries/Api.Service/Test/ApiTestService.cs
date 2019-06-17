/*----------------------------------------------------------------
* 项目名称 ：Api.Service.Test
* 类 名 称 ：ApiTestService
* 所在的域 ：DESKTOP-4903FQH
* 命名空间 ：Api.Service.Test
* 机器名称 ：DESKTOP-4903FQH 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Johnson
* 创建时间 ：2018/10/15 13:37:10
* 更新时间 ：2018/10/15 13:37:10
* 版 本 号 ：v1.0.0.0
* 项目描述 ：
* 类 描 述 ：
*******************************************************************
* Copyright @ Johnson 2018. All rights reserved.
*******************************************************************
----------------------------------------------------------------*/

using Core.Test;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNY.DataAccess.Dapper;

namespace Api.Service.Test
{
    /// <summary>
    /// 这里是简单实例
    /// 比较复杂的数据操作逻辑可以直接写Sql语句：目的为了提升效率
    /// SQL语句都放在ApiService中；
    /// 常规简单、数据比较少的情况下，可以直接采用Service【类似EF】那种方式操作；
    /// </summary>
    public class ApiTestService
    {
        public string ModuleName { get; set; }
        public ApiTestService(string moduleName)
        {
            this.ModuleName = moduleName;
        }

        /// <summary>
        /// 比较复杂的数据操作逻辑可以直接写Sql语句：目的为了提升效率
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Student> GetAllData(int id) {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append("SELECT * FROM [Student] WHERE ID=@Id");
            var conn = DapperSqlConn.GetOpenConnection(ModuleName);
            var data = conn.Query<Student>(sqlStr.ToString(), new
            {
                Id = id
            }).ToList();
            return data;
        }
    }
}
