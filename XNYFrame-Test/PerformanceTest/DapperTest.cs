using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Model.Entities;

namespace XNYFrame_Test.PerformanceTest
{
    public class DapperTest : IPerformanceTest
    {
        public DapperTest(bool isClearData = false)
        {
            if (isClearData)
            {
                Common.TruncateData();
            }
            Common.ClearDBCache();
        }

        public bool BulkCopy()
        {
            return false;
        }
        public int InsertData(int num)
        {
            using (SqlConnection sqlconn = Common.GetConnByKey())
            {
                string sql = @"INSERT INTO [dbo].[Users]
           ([LoginName]
           ,[Password]
           ,[Status]
           ,[CreateTime]
           ,[UpdateTime]
           ,[Remark])
     VALUES
           ('" + Guid.NewGuid().ToString("N") + "','',0,GETDATE(),GETDATE()," + num + ")";

                string sql2 = @"INSERT INTO [dbo].[Users] 
           ([LoginName]
           ,[Password]
           ,[Status]
           ,[CreateTime]
           ,[UpdateTime]
           ,[Remark])
     VALUES
           ('" + Guid.NewGuid().ToString("N") + "','',1,GETDATE(),GETDATE()," + num + ")";

                for (int i = 0; i < num; i++)
                {
                    if (i % 2 == 0)
                    {
                        sqlconn.Execute(sql);
                    }
                    else
                    {
                        sqlconn.Execute(sql2);
                    }
                }
            }
            return num;
        }

        public int GetData(int num = 0)
        {
            IEnumerable<UsersEntity> list = new List<UsersEntity>();
            string sql = @"SELECT [UserId]
                                  ,[LoginName]
                                  ,[Password]
                                  ,[Status]
                                  ,[CreateTime]
                                  ,[UpdateTime]
                                  ,[Remark]
                              FROM [dbo].[Users] u
                            WHERE u.[Status]=@Status ";

            using (SqlConnection sqlconn = Common.GetConnByKey())
            {
                list = sqlconn.Query<UsersEntity>(sql, new { Status = num % 2 });
            }

            return list.Count();
        }

        public string TestName
        {
            get { return "Dapper"; }
        }
    }
}
