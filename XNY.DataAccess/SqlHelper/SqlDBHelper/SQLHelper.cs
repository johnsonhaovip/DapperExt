//////////////////////////////////////////////////////////////////////////////////////////////////
//目的：封装数据库的基本操作
//方法： static SqlConnection GetDataCon　根据数据库名称返回连接　　
//　　　 static SqlTransaction BeginTransaction　开始对应数据库的事务，返回事务实例
//　　　 static int ExecuteNonQuery　执行ＳＱＬ语句或者存储过程 ,不返回参数
//       static DataSet ExecuteDataset 执行ＳＱＬ语句或者存储过程，返回dataset
//       static SqlDataReader ExecuteReader 执行ＳＱＬ语句或者存储过程，返回SqlDataReader
//       static object ExecuteScalar 执行ＳＱＬ语句或者存储过程，返回object
//  
/////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Transactions;
using XNY.Helper.Extensions;

namespace XNY.DataAccess.SqlDBHelper {
    /// <summary>
    /// 封装数据库的基本操作
    /// </summary>
    /// <remarks>    
    public class SQLHelper {
        #region 私有方法和工具

        //sql
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, out bool mustCloseConnection, List<IDataParameter> commandParameters, int? commandTimeout = null) {
            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open) {
                mustCloseConnection = true;
                connection.Open();
            } else {
                mustCloseConnection = false;
            }
            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null) {
                command.Transaction = transaction;
            }
            if (commandTimeout != null) {
                command.CommandTimeout = Convert.ToInt32(commandTimeout);
            }
            // Set the command type
            command.CommandType = commandType;
            // Attach the command parameters if they are provided
            if (commandParameters != null) {
                AttachParameters(command, commandParameters);
            }
            return;
        }



        //通用
        private static void AttachParameters(SqlCommand command, List<IDataParameter> commandParameters) {

            if (commandParameters != null && commandParameters.Count > 0) {
                foreach (SqlParameter p in commandParameters) {
                    if (p != null) {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null)) {
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
        public static IDbTransaction BeginTransaction(SqlConnection conn, System.Data.IsolationLevel Iso) {
            conn.Open();
            return conn.BeginTransaction(Iso);
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <returns>当前事务</returns>
        public static IDbTransaction BeginTransaction(SqlConnection conn) {
            conn.Open();
            return conn.BeginTransaction();
        }

        /// <summary>
        /// 结束事务，确认操作
        /// </summary>
        /// <param name="Transaction">要结束的事务</param>
        public static void endTransactionCommit(IDbTransaction Transaction) {
            using (DbConnection con = (DbConnection)Transaction.Connection) {
                Transaction.Commit();
            }
        }

        /// <summary>
        /// 结束事务，回滚操作
        /// </summary>
        /// <param name="Transaction">要结束的事务</param>
        public static void endTransactionRollback(IDbTransaction Transaction) {
            using (DbConnection con = (DbConnection)Transaction.Connection) {
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
        public static int ExecuteNonQuery(SqlConnection connection, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) {
            int retval = 0;
            //要检查参数
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
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
        public static int ExecuteNonQuery(IDbTransaction transaction, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) {
            //要检查参数  
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, ((SqlTransaction)transaction).Connection, (SqlTransaction)transaction, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
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
        public static DataSet ExecuteDataset(SqlConnection connection, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) {
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            using (SqlDataAdapter da = new SqlDataAdapter(cmd)) {
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
        public static DataSet ExecuteDataset(IDbTransaction transaction, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) {
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, (SqlConnection)transaction.Connection, (SqlTransaction)transaction, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            using (SqlDataAdapter da = new SqlDataAdapter(cmd)) {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }
        #endregion

        #region ExecuteReader

        //通用
        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, bool isClose, List<IDataParameter> commandParameters = null, int? commandTimeout = null) {
            bool mustCloseConnection = false;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            SqlDataReader dataReader = null;
            if (isClose) {
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            } else {
                dataReader = cmd.ExecuteReader();
            }
            bool canClear = true;
            foreach (IDataParameter commandParameter in cmd.Parameters) {
                if (commandParameter.Direction != ParameterDirection.Input)
                    canClear = false;
            }
            if (canClear) {
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
        public static SqlDataReader ExecuteReader(SqlConnection connection, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) {
            bool mustCloseConnection = true;
            return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, mustCloseConnection, commandParameters, commandTimeout);
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
        public static SqlDataReader ExecuteReader(IDbTransaction transaction, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) {
            bool mustCloseConnection = false;
            return ExecuteReader((SqlConnection)transaction.Connection, (SqlTransaction)transaction, commandType, commandText, mustCloseConnection, commandParameters, commandTimeout);
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
        public static object ExecuteScalar(SqlConnection connection, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) {
            object retval = null;
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
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
        public static object ExecuteScalar(IDbTransaction transaction, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) {
            object retval = null;
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, ((SqlTransaction)transaction).Connection, (SqlTransaction)transaction, commandType, commandText, out mustCloseConnection, commandParameters, commandTimeout);
            retval = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return retval;
        }

        #endregion

        #region 批量数据处理：SqlBulkCopy

        /// <summary>
        /// SqlBulkCopy批量插入数据 
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
        public static bool SqlBulkCopyByDataTable(string connectionString, string dataTableName, DataTable sourceDataTable, int batchSize = 100000, int copyTimeout = 600) {
            bool flag = false;
            try {
                using (var conn = new SqlConnection(connectionString)) {
                    using (TransactionScope scope = new TransactionScope()) {
                        conn.Open();
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction)) {
                            //服务器上目标表的名称     
                            sqlBulkCopy.DestinationTableName = dataTableName;
                            sqlBulkCopy.BatchSize = batchSize;
                            sqlBulkCopy.BulkCopyTimeout = copyTimeout;
                            for (int i = 0; i < sourceDataTable.Columns.Count; i++) {
                                //列映射定义数据源中的列和目标表中的列之间的关系     
                                sqlBulkCopy.ColumnMappings.Add(sourceDataTable.Columns[i].ColumnName, sourceDataTable.Columns[i].ColumnName);
                            }
                            try {
                                sqlBulkCopy.WriteToServer(sourceDataTable);
                                scope.Complete();//有效的事务 
                                flag = true;
                            } catch (Exception ex) {
                                LogHelper.Error(ex);
                                return false;
                            } finally {
                                conn.Close();
                                conn.Dispose();
                                if (sqlBulkCopy != null)
                                    sqlBulkCopy.Close();
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                LogHelper.Error(ex);
                return false;
            }
            return flag;
        }

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dataTableName">表名</param>
        /// <param name="sourceDataTable">数据源</param>
        /// <param name="condition">条件</param>
        /// <param name="batchSize">一次事务插入的行数</param>
        /// <param name="commTimeout">最长执行时间(默认定义为10分钟)</param>
        /// <returns>返回是否成功</returns>
        /// 创建者：蒋浩
        /// 创建时间：2018-5-23
        public static bool SqlBulkUpdate(string connectionString, string dataTableName, DataTable sourceDataTable, string condition = "", int batchSize = 100000, int commTimeout = 600) {
            DataSet ds = new DataSet();
            bool flag = false;
            sourceDataTable.TableName = dataTableName;//表名
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandTimeout = commTimeout;
            comm.CommandType = CommandType.Text;
            StringBuilder conmmandText = new StringBuilder();
            conmmandText.AppendFormat("SELECT * FROM {0} WHERE 1=1 ", dataTableName);//获取数据库现有数据
            if (!string.IsNullOrWhiteSpace(condition)) {
                conmmandText.AppendFormat(" AND {0}",condition);
            }
            comm.CommandText = conmmandText.ToString();
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            SqlCommandBuilder commandBulider = new SqlCommandBuilder(adapter);
            commandBulider.ConflictOption = ConflictOption.OverwriteChanges;
            try {
                conn.Open();
                //设置批量更新的每次处理条数 这就是批量更新
                adapter.UpdateBatchSize = batchSize;
                adapter.SelectCommand.Transaction = conn.BeginTransaction();//开始事务
                adapter.Fill(ds);
                foreach (DataRow trow in ds.Tables[0].Rows) {
                    trow.BeginEdit();
                    DataRow[] myrow = sourceDataTable.Select("UserId='" + trow["UserId"] + "'");
                    if (myrow.Length > 0) {
                        trow["UserId"] = myrow[0]["UserId"].ToString();
                        trow["Sex"] = Convert.ToBoolean(myrow[0]["Sex"]);
                        trow["Name"] = myrow[0]["Name"].ToString();
                        trow["CardId"] = myrow[0]["CardId"].ToString();
                        trow["Age"] = myrow[0]["Age"];
                        trow["Email"] = myrow[0]["Email"].ToString();
                        trow["Mobile"] = myrow[0]["Mobile"].ToString();
                        trow["Remark"] = myrow[0]["Remark"].ToString();
                    }
                    trow.EndEdit();
                }
                adapter.Update(ds);
                adapter.SelectCommand.Transaction.Commit();//提交事务 
                flag = true;
            } catch (Exception ex) {
                LogHelper.Error(ex);
                flag = false;
                if (adapter.SelectCommand != null && adapter.SelectCommand.Transaction != null) {
                    adapter.SelectCommand.Transaction.Rollback();//回滚
                }
            } finally {
                conn.Close();
                conn.Dispose();
            }
            return flag;
        }
        #endregion
    }
}
