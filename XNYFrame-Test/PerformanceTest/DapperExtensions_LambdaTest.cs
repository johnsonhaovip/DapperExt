using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
using Dapper;
using XNY.DataAccess;
using Model.Entities;

namespace XNYFrame_Test.PerformanceTest
{
    class DapperExtensions_LambdaTest : IPerformanceTest
    {
        public DapperExtensions_LambdaTest(bool isClearData = false)
        {
            if (isClearData)
            {
                Common.TruncateData();
            }
            Common.ClearDBCache();
            DapperExtension.ClearCache();
        }
        public bool BulkCopy()
        {
            return false;
        }
        public int InsertData(int num)
        {
            using (SqlConnection sqlconn = Common.GetConnByKey())
            {
                UsersEntity entity = new UsersEntity();
                entity.LoginName = Guid.NewGuid().ToString("N");
                entity.Password = "";
                entity.CreateTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;
                entity.Remark = num.ToString();
                for (int i = 0; i < num; i++)
                {
                    entity.Status = i % 2;
                    sqlconn.Insert<UsersEntity>(entity, null, dbType: DataBaseType.SqlServer);
                }
            }
            return 0;
        }

        public int GetData(int num = 0)
        {
            IEnumerable<UsersEntity> list = new List<UsersEntity>();
            using (SqlConnection sqlconn = Common.GetConnByKey())
            {
                var lambdaHelper = sqlconn.LambdaQuery<UsersEntity>()
                                          .Select((p) => new { p.UserId, p.LoginName, p.Password, p.Status, p.CreateTime, p.UpdateTime, p.Remark })
                                          .Where(p => p.Status == num % 2);
                string sql = "";//lambdaHelper.SqlString;
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var item in lambdaHelper.Parameters)
                {
                    dynamicParameters.Add(item.Value.ParameterName, item.Value.ParameterValue);
                }
                list = sqlconn.Query<UsersEntity>(sql, dynamicParameters);

            }
            return list.Count();
        }

        public string TestName
        {
            get { return "DapperExt_Lambda"; }
        }
    }
}
