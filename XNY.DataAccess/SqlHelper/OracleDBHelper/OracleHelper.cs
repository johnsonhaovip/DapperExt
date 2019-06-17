using System;
using System.Collections.Generic;
using System.Data;
//using Oracle.DataAccess.Client;
using System.Data.OracleClient;
using System.Data.Common;

namespace XNY.DataAccess.OracleDBHelper
{
    /// <summary>
    /// 封装数据库的基本操作
    /// </summary>
    /// <remarks>    
    public class SQLHelper
    {
        #region 私有方法和工具

        //sql
        private static void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, out bool mustCloseConnection, List<IDataParameter> commandParameters, int? commandTimeout = null)
        {
            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }
            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            if (commandTimeout != null)
            {
                command.CommandTimeout = Convert.ToInt32(commandTimeout);
            }
            // Set the command type
            command.CommandType = commandType;
            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }



        //通用
        private static void AttachParameters(OracleCommand command, List<IDataParameter> commandParameters)
        {

            if (commandParameters != null && commandParameters.Count > 0)
            {
                foreach (OracleParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        #endregion

        #region transaction 事务处理
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="Iso">指定连接的事务锁定行为</param>
        /// <returns>当前事务</returns>  
        public static IDbTransaction BeginTransaction(OracleConnection conn, IsolationLevel Iso)
        {
            conn.Open();
            return conn.BeginTransaction(Iso);
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <returns>当前事务</returns>
        public static IDbTransaction BeginTransaction(OracleConnection conn)
        {
            conn.Open();
            return conn.BeginTransaction();
        }

        /// <summary>
        /// 结束事务，确认操作
        /// </summary>
        /// <param name="Transaction">要结束的事务</param>
        public static void endTransactionCommit(IDbTransaction Transaction)
        {
            using (DbConnection con = (DbConnection)Transaction.Connection)
            {
                Transaction.Commit();
            }
        }

        /// <summary>
        /// 结束事务，回滚操作
        /// </summary>
        /// <param name="Transaction">要结束的事务</param>
        public static void endTransactionRollback(IDbTransaction Transaction)
        {
            using (DbConnection con = (DbConnection)Transaction.Connection)
            {
                Transaction.Rollback();
            }
        }

        #endregion

        #region ExecuteNonQuery


        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,不返回参数,只返回影响行数
        /// </summary>
        /// <param name="connection">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>影响的行数</returns>
        public static int ExecuteNonQuery(OracleConnection connection, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            int retval = 0;
            //要检查参数
            OracleCommand cmd = new OracleCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            retval = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }


        /// <summary>
        ///  执行ＳＱＬ语句或者存储过程 ,不返回参数,只返回影响行数(通用)
        /// </summary>
        /// <param name="transaction">语句所在的事务</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <returns>影响的行数</returns>
        public static int ExecuteNonQuery(IDbTransaction transaction, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            //要检查参数  
            OracleCommand cmd = new OracleCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, ((OracleTransaction)transaction).Connection, (OracleTransaction)transaction, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            int retval = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return retval;
        }
        #endregion

        #region ExecuteDataset

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="connection">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        public static DataSet ExecuteDataset(OracleConnection connection, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            OracleCommand cmd = new OracleCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                if (mustCloseConnection)
                    connection.Close();
                return ds;
            }
        }

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="transaction">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        public static DataSet ExecuteDataset(IDbTransaction transaction, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            OracleCommand cmd = new OracleCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, (OracleConnection)transaction.Connection, (OracleTransaction)transaction, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }
        #endregion

        #region ExecuteReader

        //通用
        private static OracleDataReader ExecuteReader(OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, bool isClose, List<IDataParameter> commandParameters = null, int? commandTimeout = null)
        {
            bool mustCloseConnection = false;
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            OracleDataReader dataReader = null;
            if (isClose)
            {
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            else
            {
                dataReader = cmd.ExecuteReader();
            }
            bool canClear = true;
            foreach (IDataParameter commandParameter in cmd.Parameters)
            {
                if (commandParameter.Direction != ParameterDirection.Input)
                    canClear = false;
            }
            if (canClear)
            {
                cmd.Parameters.Clear();
            }
            return dataReader;
        }


        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数datareader(通用)
        /// <remarks >
        /// 需要显示关闭连接
        /// </remarks>
        /// </summary>
        /// <param name="connection">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <returns>DataReader</returns>
        public static OracleDataReader ExecuteReader(OracleConnection connection, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            bool mustCloseConnection = true;
            return ExecuteReader(connection, (OracleTransaction)null, commandType, commandText, mustCloseConnection, commandParameters, commandTimeout);
        }


        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数datareader
        /// <remarks >
        /// 需要显示关闭连接
        /// </remarks>
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <returns>DataReader</returns>
        public static OracleDataReader ExecuteReader(IDbTransaction transaction, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            bool mustCloseConnection = false;
            return ExecuteReader((OracleConnection)transaction.Connection, (OracleTransaction)transaction, commandType, commandText, mustCloseConnection, commandParameters, commandTimeout);
        }

        #endregion

        #region ExecuteScalar


        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值(通用)
        /// </summary>
        /// <param name="connection">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        public static object ExecuteScalar(OracleConnection connection, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            object retval = null;
            OracleCommand cmd = new OracleCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            retval = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }



        /// <summary>
        ///  执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值
        /// </summary>
        /// <param name="transaction">语句所在的事务</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <returns>执行结果集第一行，第一列的值</returns>
        public static object ExecuteScalar(IDbTransaction transaction, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            object retval = null;
            OracleCommand cmd = new OracleCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, ((OracleTransaction)transaction).Connection, (OracleTransaction)transaction, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            retval = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return retval;
        }

        #endregion

        #region 批量数据处理：OracleBulkCopy 

        /// <summary>
        /// OracleBulkCopy批量插入数据 
        /// 【*注：调用此方法DataTable中的字段必须与对应插入表的字段保持一致】
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dataTableName">表名</param>
        /// <param name="sourceDataTable">数据源</param>
        /// <param name="batchSize">一次事务插入的行数</param>
        /// <param name="copyTimeout">最长执行时间(默认定义为10分钟)</param>
        /// <returns>返回是否成功</returns>
        /// 创建者：蒋浩
        /// 创建时间：2018-4-8
        public static bool OracleBulkCopyByDataTable(string connectionString, string dataTableName, DataTable sourceDataTable, int batchSize = 100000, int copyTimeout = 600)
        {
            bool flag = false;
            try
            {
                if (sourceDataTable == null || sourceDataTable.Rows.Count == 0)
                    return flag; 
                using (var conn = new System.Data.OleDb.OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (Oracle.DataAccess.Client.OracleBulkCopy oracleBulkCopy = new Oracle.DataAccess.Client.OracleBulkCopy(connectionString, Oracle.DataAccess.Client.OracleBulkCopyOptions.UseInternalTransaction))
                    {
                        //服务器上目标表的名称     
                        oracleBulkCopy.DestinationTableName = dataTableName;
                        oracleBulkCopy.BatchSize = batchSize;
                        oracleBulkCopy.BulkCopyTimeout = copyTimeout;
                        for (int i = 0; i < sourceDataTable.Columns.Count; i++)
                        {
                            //列映射定义数据源中的列和目标表中的列之间的关系     
                            oracleBulkCopy.ColumnMappings.Add(sourceDataTable.Columns[i].ColumnName, sourceDataTable.Columns[i].ColumnName);
                        }
                        try
                        {
                            oracleBulkCopy.WriteToServer(sourceDataTable);
                            flag = true;
                        }
                        catch
                        {
                            return false;
                        }
                        finally
                        {
                            conn.Close();
                            if (oracleBulkCopy != null)
                                oracleBulkCopy.Close();
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return flag;
        }

        #endregion

    }
}
