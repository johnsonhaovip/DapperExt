using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace XNY.DataAccess.Dapper {
    public class DapperSqlConn {
        public static readonly string connectionString = "DefaultConnection";

        /// <summary>
        /// SqlService连接方式
        /// </summary>
        /// <param name="ModuleName"></param>
        /// <param name="mars"></param>
        /// <returns></returns>
        public static SqlConnection GetOpenConnection(string ModuleName = null, bool mars = false) {
            string cs = "";
            if (string.IsNullOrWhiteSpace(ModuleName))
                cs = ConfigurationManager.ConnectionStrings[connectionString].ToString();
            else
                cs = ConfigurationManager.ConnectionStrings[ModuleName].ToString();
            if (mars) {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(cs);
                scsb.MultipleActiveResultSets = true;
                cs = scsb.ConnectionString;
            }
            var connection = new SqlConnection(cs);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// MySql连接方式
        /// </summary>
        /// <param name="ModuleName"></param>
        /// <param name="mars"></param>
        /// <returns></returns>
        public static MySqlConnection GetOpenMySqlConnection(string ModuleName = null, bool mars = false) {
            string cs = "";
            if (string.IsNullOrWhiteSpace(ModuleName))
                cs = ConfigurationManager.ConnectionStrings[connectionString].ToString();
            else
                cs = ConfigurationManager.ConnectionStrings[ModuleName].ToString();
            if (mars) {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(cs);
                scsb.MultipleActiveResultSets = true;
                cs = scsb.ConnectionString;
            }
            var connection = new MySqlConnection(cs);
            connection.Open();
            return connection;
        }

        public static SqlConnection GetClosedConnection() {
            return new SqlConnection(connectionString);
        }

        public static MySqlConnection GetClosedMySqlConnection() {
            return new MySqlConnection(connectionString);
        }
    }
}
