using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
using Dapper;
using System.Data.SqlClient;
using XNY.DataAccess;
using Model.Entities;
using System.Configuration;
using System.Data;
using XNY.Helper.Extensions;

namespace XNYFrame_Test.PerformanceTest
{
    public class DapperExtensionsTest : IPerformanceTest
    {
        public DapperExtensionsTest(bool isClearData = false)
        {
            if (isClearData)
            {
                Common.TruncateData();
            }
            Common.ClearDBCache();
            DapperExtension.ClearCache();
        }


        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
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
            return num;
        }
        public bool BulkCopy()
        {
            string connKey = "DefaultConnection";
            string connstr = ConfigurationManager.ConnectionStrings[connKey].ConnectionString;

            //构造一个Datatable存储将要批量导入的数据  
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("UserId", typeof(int));
            dt.Columns.Add("Sex", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("CardId", typeof(string));
            dt.Columns.Add("Age", typeof(int));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Mobile", typeof(string));
            dt.Columns.Add("Remark", typeof(string));

            // 见识下SqlBulkCopy强悍之处，来个一百万条数数据试验
            //测试结果一百万条数据的插入:最长耗时一分钟8秒，最短耗时4秒08   
            int i;
            for (i = 0; i < 1000000; i++)
            {
                DataRow dr = dt.NewRow();
                dr["id"] = i.ToString();
                dr["UserId"] = i.ToString();
                dr["Sex"] = 2;
                dr["Name"] = "测试更新" + i.ToString();
                dr["CardId"] = "测试更新CardId" + i.ToString();
                dr["Age"] = 2;
                dr["Email"] = i.ToString() + "@qq.com";
                dr["Mobile"] = "152" + i.ToString();
                dr["Remark"] = "测试更新";
                dt.Rows.Add(dr);
            }

            //return XNY.DataAccess.SqlDBHelper.SQLHelper.SqlBulkCopyByDataTable(connstr, "[Test].[dbo].[User]", dt);
            return XNY.DataAccess.SqlDBHelper.SQLHelper.SqlBulkUpdate(connstr, "[Test].[dbo].[User]", dt);

        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public int GetData(int num = 0)
        {
            IEnumerable<UsersEntity> list = new List<UsersEntity>();
            using (SqlConnection sqlconn = Common.GetConnByKey())
            {
                list = sqlconn.GetList<UsersEntity>(new { Status = num % 2 });
            }
            return list.Count();
        }

        /// <summary>
        /// 测试名称
        /// </summary>
        public string TestName
        {
            get { return "DapperExt"; }
        }
    }
}
