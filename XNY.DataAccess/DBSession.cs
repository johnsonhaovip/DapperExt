using XNY.DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace XNY.DataAccess
{
    /// <summary>
    /// 数据库连接事务的Session对象
    /// </summary>
    public class DBSessionBase : IDBSession
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private readonly DataBaseType _dbType;
        private readonly string _connKey;
        private bool _isDisposed = false;

        public DataBaseType dbType
        {
            get { return _dbType; }
        }

        public string ConnKey
        {
            get { return _connKey; }
        }

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public IDbConnection Connection
        {
            get { return _connection; }
        }


        /// <summary>
        /// 数据库事务对象
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return _transaction; }
        }

        public DBSessionBase(IDatabase Database)
        {
            _connection = Database.Connection;
            _dbType = Database.dbType;
            _connKey = Database.ConnKey;
        }

        public DBSessionBase(string connKey)
        {
            _dbType = DBUtils.GetDBTypeByConnKey(connKey);
            _connKey = connKey;
        }

        public DBSessionBase(string connKey, IDbConnection Connection, IDbTransaction Transaction)
        {
            _dbType = DBUtils.GetDBTypeByConnKey(connKey);
            _connKey = connKey;
            this._connection = Connection;
            this._transaction = Transaction;
        }


        /// <summary>
        /// 开启会话
        /// </summary>
        /// <param name="isolation"></param>
        /// <returns></returns>
        public IDbTransaction Begin(IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            _connection.Open();
            _transaction = _connection.BeginTransaction(isolation);
            return _transaction;
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        public void Commit()
        {
            _transaction.Commit();
            _transaction = null;
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }



        /// <summary>
        /// 析构函数
        /// </summary>
        ~DBSessionBase()
        {
            Dispose(false);//释放非托管资源
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (_connection != null)
                {
                    using (_connection)
                    {
                        if (_connection.State != ConnectionState.Closed)
                        {
                            if (_transaction != null)
                            {
                                _transaction.Rollback();
                                _transaction.Dispose();
                            }
                        }
                        _connection.Close();
                        _connection.Dispose();
                    }
                }
                this._isDisposed = true;
            }
        }




    }
}
