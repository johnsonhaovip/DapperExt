using Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNYFrame_Test.PerformanceTest
{
    public class AdoTeset : IPerformanceTest
    {
        public AdoTeset(bool isClearData = false)
        {
            if (isClearData)
            {
                Common.TruncateData();
            }
            Common.ClearDBCache();

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
                        SQLHelper.ExecuteNonQuery(sqlconn, CommandType.Text, sql);
                    }
                    else
                    {
                        SQLHelper.ExecuteNonQuery(sqlconn, CommandType.Text, sql2);
                    }
                }
            }
            return num;
        }
        public bool BulkCopy()
        {
            return false;
        }
        public int GetData(int num = 0)
        {
            List<UsersEntity> list = new List<UsersEntity>();
            string sql = @"SELECT [UserId]
                                  ,[LoginName]
                                  ,[Password]
                                  ,[Status]
                                  ,[CreateTime]
                                  ,[UpdateTime]
                                  ,[Remark]
                              FROM [dbo].[Users] u
                            WHERE u.[Status]=@Status ";

            List<SqlParameter> parList = new List<SqlParameter>();
            parList.Add(new SqlParameter("@Status", num % 2));
            using (SqlConnection sqlconn = Common.GetConnByKey())
            {
                using (SqlDataReader reader = SQLHelper.ExecuteReader(sqlconn, CommandType.Text, sql, parList.ToArray()))
                {
                    while (reader.Read())
                    {
                        UsersEntity entity = new UsersEntity();
                        entity.UserId = Convert.ToInt32(reader["UserId"]);
                        entity.LoginName = reader["LoginName"].ToString();
                        entity.Password = reader["Password"].ToString();
                        entity.Status = Convert.ToInt32(reader["Status"]);
                        entity.CreateTime = Convert.ToDateTime(reader["CreateTime"]);
                        entity.UpdateTime = Convert.ToDateTime(reader["UpdateTime"]);
                        entity.Remark = reader["Remark"].ToString();
                        list.Add(entity);
                    }
                }
            }
            return list.Count();
        }

        public string TestName
        {
            get { return "SqlHelper"; }
        }
    }
}
