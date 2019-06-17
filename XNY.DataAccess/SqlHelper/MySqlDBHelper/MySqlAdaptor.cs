using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using XNY.DataAccess.Tool;
using XNY.DataAccess.MySqlDBHelper;
using MySql.Data.MySqlClient;
using XNY.DataAccess.Utils;

namespace XNY.DataAccess.MySqlDBHelper
{
    public class MySqlAdaptor : IDBHelper
    {
        private readonly string _ConnectionStringKey = "DefaultConnection";

        public MySqlAdaptor(string connectionStringKey = "")
        {
            if (!string.IsNullOrEmpty(connectionStringKey))
            {
                _ConnectionStringKey = connectionStringKey;
            }
        }

        /// <summary>
        /// 取得数据库连接
        /// </summary>
        /// <param name="DBKey">数据库连接主键</param>
        /// <returns></returns>
        public static MySqlConnection GetConnByKey(string connectionStringKey)
        {
            ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[connectionStringKey];
            string constr = css.ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);
            return con;
        }

        /// <summary>
        /// 生成分页SQL语句
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectSql"></param>
        /// <param name="sqlCount"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public string GetPagingSql(int pageIndex, int pageSize, string selectSql, string sqlCount, string orderBy)
        {
            return PageHelper.GetPagingSql(pageIndex, pageSize, selectSql, sqlCount, orderBy);
        }

        #region 事务
        /// <summary>
        /// 开始一个事务
        /// </summary>
        public IDbTransaction BeginTractionand(IsolationLevel Iso = IsolationLevel.Unspecified)
        {
            MySqlConnection con = GetConnByKey(_ConnectionStringKey);
            IDbTransaction transaction = SQLHelper.BeginTransaction(con, Iso);
            return transaction;
        }

        /// <summary>
        /// 开始一个事务
        /// </summary>
        public IDbTransaction BeginTractionand(string connKey, IsolationLevel Iso = IsolationLevel.Unspecified)
        {
            MySqlConnection con = GetConnByKey(connKey);
            IDbTransaction transaction = SQLHelper.BeginTransaction(con, Iso);
            return transaction;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTractionand(IDbTransaction dbTransaction)
        {
            SQLHelper.endTransactionRollback(dbTransaction);
        }

        /// <summary>
        /// 结束并确认事务
        /// </summary>
        public void CommitTractionand(IDbTransaction dbTransaction)
        {
            SQLHelper.endTransactionCommit(dbTransaction);
        }
        #endregion

        #region ExecuteDataSet
        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        public DataSet ExecuteDataSet(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return ExecuteDataSet(_ConnectionStringKey, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        ///  执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        public DataSet ExecuteDataSet(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            using (MySqlConnection conn = GetConnByKey(connKey))
            {
                return SQLHelper.ExecuteDataset(conn, commandText, commandParameters, commandType, commandTimeout);
            }
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        public DataSet ExecuteDataSet(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return SQLHelper.ExecuteDataset((MySqlConnection)conn, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        public DataSet ExecuteDataSet(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return SQLHelper.ExecuteDataset(trans, commandText, commandParameters, commandType, commandTimeout);
        }

        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,只返回影响行数
        /// </summary>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>影响的行数</returns>
        public int ExecuteNonQuery(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return ExecuteNonQuery(_ConnectionStringKey, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,只返回影响行数
        /// </summary>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>影响的行数</returns>
        public int ExecuteNonQuery(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            using (MySqlConnection conn = GetConnByKey(connKey))
            {
                return SQLHelper.ExecuteNonQuery(conn, commandText, commandParameters, commandType, commandTimeout);
            }
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,只返回影响行数
        /// </summary>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>影响的行数</returns>
        public int ExecuteNonQuery(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return SQLHelper.ExecuteNonQuery((MySqlConnection)conn, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,只返回影响行数
        /// </summary>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>影响的行数</returns>
        public int ExecuteNonQuery(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return SQLHelper.ExecuteNonQuery(trans, commandText, commandParameters, commandType, commandTimeout);
        }
        #endregion

        #region ExecuteReader

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回DataReader
        /// </summary>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteReader(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return ExecuteReader(_ConnectionStringKey, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回DataReader
        /// </summary>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteReader(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            MySqlConnection conn = GetConnByKey(connKey);
            return SQLHelper.ExecuteReader(conn, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回DataReader
        /// </summary>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteReader(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return SQLHelper.ExecuteReader((MySqlConnection)conn, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回DataReader
        /// </summary>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteReader(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return SQLHelper.ExecuteReader(trans, commandText, commandParameters, commandType, commandTimeout);
        }
        #endregion

        #region ExecuteIEnumerable

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">返回类似</typeparam>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<T> ExecuteIEnumerable<T>(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) where T : class, new()
        {
            return ExecuteIEnumerable<T>(_ConnectionStringKey, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">返回类似</typeparam>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<T> ExecuteIEnumerable<T>(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) where T : class, new()
        {
            using (MySqlConnection conn = GetConnByKey(connKey))
            {
                using (IDataReader dr = SQLHelper.ExecuteReader(conn, commandText, commandParameters, commandType, commandTimeout))
                {
                    return DataReaderExtensions.DataReaderToList<T>(dr);
                }
            }
        }


        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">返回类似</typeparam>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<T> ExecuteIEnumerable<T>(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) where T : class, new()
        {
            IDataReader dr = SQLHelper.ExecuteReader((MySqlConnection)conn, commandText, commandParameters, commandType, commandTimeout);
            return DataReaderExtensions.DataReaderToList<T>(dr);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">返回类似</typeparam>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<T> ExecuteIEnumerable<T>(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) where T : class, new()
        {
            IDataReader dr = SQLHelper.ExecuteReader(trans, commandText, commandParameters, commandType, commandTimeout);
            return DataReaderExtensions.DataReaderToList<T>(dr);
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值
        /// </summary>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        public object ExecuteScalar(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return ExecuteScalar(_ConnectionStringKey, commandText, commandParameters, commandType, commandTimeout);
        }


        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数类似T．第一行，第一列的值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        public T ExecuteScalar<T>(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return (T)ExecuteScalar(_ConnectionStringKey, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值
        /// </summary>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        public object ExecuteScalar(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            using (MySqlConnection conn = GetConnByKey(connKey))
            {
                return SQLHelper.ExecuteScalar(conn, commandText, commandParameters, commandType, commandTimeout);
            }
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数类似T．第一行，第一列的值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        public T ExecuteScalar<T>(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return (T)ExecuteScalar(connKey, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值
        /// </summary>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        public object ExecuteScalar(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return SQLHelper.ExecuteScalar((MySqlConnection)conn, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数类似T．第一行，第一列的值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        public T ExecuteScalar<T>(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return (T)ExecuteScalar(conn, commandText, commandParameters, commandType, commandTimeout);
        }


        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值
        /// </summary>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        public object ExecuteScalar(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return SQLHelper.ExecuteScalar(trans, commandText, commandParameters, commandType, commandTimeout);
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数类似T．第一行，第一列的值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        public T ExecuteScalar<T>(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            return (T)ExecuteScalar(trans, commandText, commandParameters, commandType, commandTimeout);
        }
        #endregion
    }
}
