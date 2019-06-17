
using XNY.DataAccess.Utils;
using System;
using System.Data;

namespace XNY.DataAccess
{
    /// <summary>
    /// 数据库接口
    /// </summary>
    public interface IDatabase
    {
        IDbConnection Connection { get; }

        DataBaseType dbType { get; }

        string ConnKey { get; }
    }

    /// <summary>
    /// 数据库类对象
    /// </summary>
    public class Database : IDatabase
    {
        public IDbConnection Connection { get; private set; }

        public DataBaseType dbType { get; private set; }

        public string ConnKey { get; set; }


        public Database(string connKey)
        {
            this.ConnKey = connKey;
            DataBaseType dbType;
            Connection = DBUtils.CreateDBConnection(connKey, out dbType);
            this.dbType = dbType;
        }

        public Database(DataBaseType dbType, string connKey)
        {
            this.ConnKey = connKey;
            this.dbType = dbType;
            this.Connection = DBUtils.CreateDBConnection(dbType, connKey);
        }

    }


    /// <summary>
    /// 数据连接事务的Session接口
    /// </summary>
    public interface IDBSession : IDisposable
    {
        string ConnKey { get; }
        DataBaseType dbType { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        IDbTransaction Begin(IsolationLevel isolation = IsolationLevel.ReadCommitted);
        void Commit();
        void Rollback();
    }
}
