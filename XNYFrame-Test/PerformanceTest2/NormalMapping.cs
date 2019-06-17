using Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using XNYFrame_Test.PerformanceTest;

namespace XNYFrame_Test
{
    public class NormalMapping : IPerformanceTest
    {
        List<PropertyInfo> ModelPropertyInfo = new List<PropertyInfo>();
        public NormalMapping(bool isClearData = false)
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
                //DataSet ds = SQLHelper.ExecuteDataset(sqlconn, CommandType.Text, sql, parList.ToArray());
                //if (ds.Tables[0] != null)
                //{
                //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //    {
                //        list.Add(Populate_Model_FromDr<UsersEntity>(ds.Tables[0].Rows[i]));
                //    }
                //}



                using (SqlDataReader reader = SQLHelper.ExecuteReader(sqlconn, CommandType.Text, sql, parList.ToArray()))
                {
                    while (reader.Read())
                    {
                        UsersEntity entity = new UsersEntity();
                        //entity.UserId = Convert.ToInt32(reader["UserId"]);
                        //entity.LoginName = reader["LoginName"].ToString();
                        //entity.Password = reader["Password"].ToString();
                        //entity.Status = Convert.ToInt32(reader["Status"]);
                        //entity.CreateTime = Convert.ToDateTime(reader["CreateTime"]);
                        //entity.UpdateTime = Convert.ToDateTime(reader["UpdateTime"]);
                        //entity.Remark = reader["Remark"].ToString();
                        entity = Populate_Model_FromDr<UsersEntity>(reader);
                        list.Add(entity);
                    }
                }
            }

            return list.Count();
        }



        /// <summary>
        /// 从IDataReader转换到实体
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public TModel Populate_Model_FromDr<TModel>(System.Data.IDataReader dr) where TModel : new()
        {
            TModel Obj = new TModel();
            if (ModelPropertyInfo.Count == 0)
            {
                Type t = typeof(UsersEntity);
                foreach (PropertyInfo p in t.GetProperties())
                {
                    ModelPropertyInfo.Add(p);
                }
            }
            foreach (PropertyInfo p in ModelPropertyInfo)
            {
                p.SetValue(Obj,Get(dr[p.Name], p.PropertyType),null);
            }

            return Obj;
        }



        public TModel Populate_Model_FromDr<TModel>(System.Data.DataRow row) where TModel : new()
        {
            TModel Obj = new TModel();
            foreach (PropertyInfo p in ModelPropertyInfo)
            {
                if (row.Table.Columns[p.Name] != null)
                {
                    p.SetValue(Obj,Get(row[p.Name], p.PropertyType),null);
                }

            }

            return Obj;
        }



        static public object Get(object t, Type p)
        {
            if (t is System.DBNull)
            {
                if (p.Name.ToLower() == "string")
                {
                    return "";
                }
                return System.Reflection.Assembly.GetAssembly(p).CreateInstance(p.ToString());
            }
            return t;
        }

        public string TestName
        {
            get { return "NormalMapping"; }
        }
    }
}
