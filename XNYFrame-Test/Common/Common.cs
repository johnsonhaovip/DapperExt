using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
using Dapper;
using System.Data.SqlClient;
using XNY.DataAccess;
using System.Configuration;
using System.Data;
using Model.Entities;

namespace XNYFrame_Test
{
    public class Common
    {

        /// <summary>
        /// 得到web.config里配置项的数据库连接字符串。
        /// </summary>
        /// <param name="connKey">连接数据库名,主要值由web.config配置</param>
        /// <returns>数据库连接字符串</returns>
        public static SqlConnection GetConnByKey(string connKey = "DefaultConnection")
        {
            string connstr = ConfigurationManager.ConnectionStrings[connKey].ConnectionString;
            SqlConnection con = new SqlConnection(connstr);
            return con;
        }


        public static void TruncateData()
        {
            string sql = @" TRUNCATE TABLE Users  
                            TRUNCATE TABLE UserInfo ";
            using (SqlConnection sqlconn = Common.GetConnByKey())
            {
                SQLHelper.ExecuteNonQuery(sqlconn, CommandType.Text, sql);
            }
        }

        public static void InsertTestData(int num)
        {

            using (SqlConnection sqlconn = Common.GetConnByKey())
            {
                for (int i = 0; i < num; i++)
                {
                    //Random r = new Random(i);
                    //UsersEntity entity = new UsersEntity();
                    //entity.LoginName = Guid.NewGuid().ToString("N");
                    //entity.Password = "";
                    //entity.Status = i ;
                    //entity.CreateTime = DateTime.Now;
                    //entity.UpdateTime = DateTime.Now;
                    //entity.Remark = i.ToString();
                    //int userId = sqlconn.Insert<UsersEntity>(entity, null, dbType: DataBaseType.SqlServer);


                    string sql = string.Format(@"INSERT INTO [dbo].[Users]
           ([LoginName]
           ,[Password]
           ,[Status]
           ,[CreateTime]
           ,[UpdateTime]
           ,[Remark])
     VALUES
           ('{0}','{1}',{2},GETDATE(),GETDATE(),{3})", Guid.NewGuid().ToString("N"), "", 1, i);
                    SQLHelper.ExecuteNonQuery(sqlconn, CommandType.Text, sql);


                    //UserInfoEntity userInfoEntity = new UserInfoEntity();
                    //userInfoEntity.UserId = userId;
                    //userInfoEntity.Sex = i % 2;
                    //userInfoEntity.Name = Guid.NewGuid().ToString("N");
                    //userInfoEntity.CardId = i.ToString(); 
                    //userInfoEntity.Age = i / 100;
                    //userInfoEntity.Email = "Test" + i + "@qq.com";
                    //userInfoEntity.Mobile = i.ToString();
                    //userInfoEntity.Remark = i.ToString();

                    //sqlconn.Insert<UserInfoEntity>(userInfoEntity, null, dbType: DataBaseType.SqlServer);
                }

            }

        }



        public static void ClearDBCache()
        {

            string sql = @"DBCC DROPCLEANBUFFERS   
                           DBCC FREEPROCCACHE

                             ";
            using (SqlConnection sqlconn = Common.GetConnByKey())
            {
                SQLHelper.ExecuteNonQuery(sqlconn, CommandType.Text, sql);
            }

        }
    }
}
