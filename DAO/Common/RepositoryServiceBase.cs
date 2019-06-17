using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using DapperExtensions;
using XNY.DataAccess;
using DapperExtensions.Lambda;
using XNY.DataAccess.Utils;
using XNY.Freamework.Demo;

namespace DAO.Common {
    public class RepositoryServiceBase<T> : IDataRepository<T> where T : class {

        /// <summary>
        /// 链接字符串
        /// </summary>
        public string ConnKey { get; private set; }

        public IDBSession DBSession { get; private set; }

        public RepositoryServiceBase(string connKey = "DefaultConnection") {
            ConnKey = connKey;
            if (Helper.EnabledDbSession) {
                DBSession = Helper.GetPerHttpRequestDBSession(connKey);
            }
        }

        #region 非传入Sql 方法


        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        public T GetById(dynamic primaryId) {
            DbConnObj ConnObj = GetConnObj();
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.Get<T>(primaryId as object, ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    return conn.Get<T>(primaryId as object, ConnObj.DbTransaction, dbType: dbType);
                }
            }
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <typeparam name="TReturn">返回的类型</typeparam>
        /// <param name="primaryId">主键</param>
        /// <returns></returns>
        public TReturn GetById<TReturn>(dynamic primaryId) where TReturn : class {
            DbConnObj ConnObj = GetConnObj();
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.Get<T, TReturn>(primaryId as object, ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    return conn.Get<T, TReturn>(primaryId as object, ConnObj.DbTransaction, dbType: dbType);
                }
            }
        }


        /// <summary>
        /// 根据多个Id获取多个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<T> GetByIds(IList<dynamic> ids) {
            var tblName = string.Format("dbo.{0}", typeof(T).Name);
            var idsin = string.Join(",", ids.ToArray<dynamic>());
            var sql = "SELECT * FROM @table WHERE Id in (@ids)";
            DbConnObj ConnObj = GetConnObj();
            IEnumerable<T> dataList = new List<T>();
            if (ConnObj.DbConnection != null) {
                dataList = ConnObj.DbConnection.Query<T>(sql, new { table = tblName, ids = idsin }, ConnObj.DbTransaction);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    dataList = conn.Query<T>(sql, new { table = tblName, ids = idsin });
                }
            }
            return dataList;
        }

        /// <summary>
        /// 根据多个Id获取多个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<TReturn> GetByIds<TReturn>(IList<dynamic> ids) where TReturn : class {
            var tblName = string.Format("dbo.{0}", typeof(T).Name);
            var idsin = string.Join(",", ids.ToArray<dynamic>());
            var sql = "SELECT * FROM @table WHERE Id in (@ids)";
            DbConnObj ConnObj = GetConnObj();
            IEnumerable<TReturn> dataList = new List<TReturn>();
            if (ConnObj.DbConnection != null) {
                dataList = ConnObj.DbConnection.Query<TReturn>(sql, new { table = tblName, ids = idsin }, ConnObj.DbTransaction);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    dataList = conn.Query<TReturn>(sql, new { table = tblName, ids = idsin }, ConnObj.DbTransaction);
                }
            }
            return dataList;
        }



        /// <summary>
        /// 获取全部数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAll(string tableName = null) {
            DbConnObj ConnObj = GetConnObj();
            if (ConnObj.DbConnection != null) {
                if (string.IsNullOrWhiteSpace(tableName)) {
                    return ConnObj.DbConnection.GetList<T>(transaction: ConnObj.DbTransaction, dbType: ConnObj.dbType);
                } else {
                    return ConnObj.DbConnection.GetList<T>(transaction: ConnObj.DbTransaction, dbType: ConnObj.dbType, tableName: tableName);
                }
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    if (string.IsNullOrWhiteSpace(tableName)) {
                        return conn.GetList<T>(transaction: ConnObj.DbTransaction, dbType: dbType);
                    } else {
                        return conn.GetList<T>(transaction: ConnObj.DbTransaction, dbType: dbType, tableName: tableName);
                    }
                }
            }
        }


        /// <summary>
        /// 获取全部数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<TReturn> GetAll<TReturn>() where TReturn : class {
            DbConnObj ConnObj = GetConnObj();
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.GetList<T, TReturn>(transaction: ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    return conn.GetList<T, TReturn>(transaction: ConnObj.DbTransaction, dbType: dbType);
                }
            }
        }


        /// <summary>
        /// 统计记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int Count(object predicate) {
            DbConnObj ConnObj = GetConnObj();
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.Count<T>(predicate, transaction: ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    return conn.Count<T>(predicate, transaction: ConnObj.DbTransaction, dbType: dbType);
                }
            }
        }

        /// <summary>
        /// 查询列表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList(object predicate = null, IList<ISort> sort = null) {
            DbConnObj ConnObj = GetConnObj();
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.GetList<T>(predicate, sort, ConnObj.DbTransaction, null, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    return conn.GetList<T>(predicate, sort, ConnObj.DbTransaction, null, dbType: dbType);
                }
            }
        }


        /// <summary>
        /// 查询列表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IEnumerable<TReturn> GetList<TReturn>(object predicate = null, IList<ISort> sort = null) where TReturn : class {
            DbConnObj ConnObj = GetConnObj();
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.GetList<T, TReturn>(predicate, sort, ConnObj.DbTransaction, null, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    return conn.GetList<T, TReturn>(predicate, sort, ConnObj.DbTransaction, null, dbType: dbType);
                }
            }
        }



        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allRowsCount"></param>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IEnumerable<T> GetPageList(int pageIndex, int pageSize, out long allRowsCount,
            object predicate = null, IList<ISort> sort = null) {
            DbConnObj ConnObj = GetConnObj();
            allRowsCount = 0;
            IEnumerable<T> entityList = new List<T>();
            if (ConnObj.DbConnection != null) {
                entityList = ConnObj.DbConnection.GetPage<T>(predicate, sort, pageIndex, pageSize, ConnObj.DbTransaction, null, dbType: ConnObj.dbType);
                allRowsCount = ConnObj.DbConnection.Count<T>(predicate, transaction: ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    entityList = conn.GetPage<T>(predicate, sort, pageIndex, pageSize, ConnObj.DbTransaction, null, dbType: dbType);
                    allRowsCount = conn.Count<T>(predicate, transaction: ConnObj.DbTransaction, dbType: dbType);
                }
            }
            return entityList;
        }


        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allRowsCount"></param>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IEnumerable<TReturn> GetPageList<TReturn>(int pageIndex, int pageSize, out long allRowsCount,
            object predicate = null, IList<ISort> sort = null) where TReturn : class {
            DbConnObj ConnObj = GetConnObj();
            allRowsCount = 0;
            IEnumerable<TReturn> entityList = new List<TReturn>();
            if (ConnObj.DbConnection != null) {
                entityList = ConnObj.DbConnection.GetPage<T, TReturn>(predicate, sort, pageIndex, pageSize, ConnObj.DbTransaction, null, dbType: ConnObj.dbType);
                allRowsCount = ConnObj.DbConnection.Count<T>(predicate, transaction: ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    entityList = conn.GetPage<T, TReturn>(predicate, sort, pageIndex, pageSize, ConnObj.DbTransaction, null, dbType: dbType);
                    allRowsCount = conn.Count<T>(predicate, transaction: ConnObj.DbTransaction, dbType: dbType);
                }
            }
            return entityList;
        }





        /// <summary>
        /// 插入单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public dynamic Insert(T entity, IDbTransaction transaction = null) {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.Insert<T>(entity, ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    return conn.Insert<T>(entity, ConnObj.DbTransaction, dbType: dbType);
                }
            }
        }

        /// <summary>
        /// 更新单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool Update(T entity, IDbTransaction transaction = null) {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.Update<T>(entity, ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    return conn.Update<T>(entity, ConnObj.DbTransaction, dbType: dbType);
                }
            }
        }

        /// <summary>
        /// 删除单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public int Delete(dynamic primaryId, IDbTransaction transaction = null) {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                object entity = this.GetById(primaryId);
                var obj = entity as T;
                return ConnObj.DbConnection.Delete<T>(obj, ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    object entity = conn.Get<T>(primaryId as object, ConnObj.DbTransaction, dbType: dbType);
                    var obj = entity as T;
                    return conn.Delete<T>(obj, ConnObj.DbTransaction, dbType: dbType);
                }
            }
        }

        /// <summary>
        /// 删除单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns> 
        public int DeleteList(object predicate = null, IDbTransaction transaction = null) {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.Delete<T>(predicate, ConnObj.DbTransaction, dbType: ConnObj.dbType);
            } else {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType)) {
                    return conn.Delete<T>(predicate, ConnObj.DbTransaction, dbType: dbType);
                }
            }
        }

        /// <summary>
        /// 批量插入功能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        public bool InsertBatch(IEnumerable<T> entityList, IDbTransaction transaction = null) {
            bool isOk = false;
            foreach (var item in entityList) {
                Insert(item, transaction);
            }
            isOk = true;
            return isOk;
        }

        /// <summary>
        /// 批量更新（）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool UpdateBatch(IEnumerable<T> entityList, IDbTransaction transaction = null) {
            bool isOk = false;
            foreach (var item in entityList) {
                Update(item, transaction);
            }
            isOk = true;
            return isOk;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool DeleteBatch(IEnumerable<dynamic> ids, IDbTransaction transaction = null) {
            bool isOk = false;
            foreach (var id in ids) {
                this.Delete(id, transaction);
            }
            isOk = true;
            return isOk;
        }
        public bool DeleteBatch(IEnumerable<long> ids, IDbTransaction transaction = null) {
            bool isOk = false;
            foreach (var id in ids) {
                this.Delete(id, transaction);
            }
            isOk = true;
            return isOk;
        }
        public bool DeleteBatch(IEnumerable<int> ids, IDbTransaction transaction = null) {
            bool isOk = false;
            foreach (var id in ids) {
                this.Delete(id, transaction);
            }
            isOk = true;
            return isOk;
        }
        public bool DeleteBatch(IEnumerable<Guid> ids, IDbTransaction transaction = null) {
            bool isOk = false;
            foreach (var id in ids) {
                this.Delete(id, transaction);
            }
            isOk = true;
            return isOk;
        }
        #endregion

        #region Lambda

        //public LambdaInsertHelper<T> LambdaInsert(IDbTransaction transaction = null, int? commandTimeout = null)
        //{
        //    DbConnObj ConnObj = GetConnObj(transaction);
        //    return ConnObj.DbConnection.LambdaInsert<T>(transaction, commandTimeout);
        //}

        //public LambdaInsertHelper<TEntity> LambdaInsert<TEntity>(IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class
        //{
        //    DbConnObj ConnObj = GetConnObj(transaction);
        //    return ConnObj.DbConnection.LambdaInsert<TEntity>(transaction, commandTimeout);
        //}


        public LambdaDeleteHelper<T> LambdaDelete(IDbTransaction transaction = null, int? commandTimeout = null) {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.LambdaDelete<T>(transaction, commandTimeout);
            } else {
                return DapperExtension.LambdaDelete<T>(ConnKey, commandTimeout);
            }
        }
        public LambdaDeleteHelper<TEntity> LambdaDelete<TEntity>(IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.LambdaDelete<TEntity>(transaction, commandTimeout);
            } else {
                return DapperExtension.LambdaDelete<TEntity>(ConnKey, commandTimeout);
            }
        }

        public LambdaUpdateHelper<T> LambdaUpdate(IDbTransaction transaction = null, int? commandTimeout = null) {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.LambdaUpdate<T>(transaction, commandTimeout);
            } else {
                return DapperExtension.LambdaUpdate<T>(ConnKey, commandTimeout);
            }
        }
        public LambdaUpdateHelper<TEntity> LambdaUpdate<TEntity>(IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.LambdaUpdate<TEntity>(transaction, commandTimeout);
            } else {
                return DapperExtension.LambdaUpdate<TEntity>(ConnKey, commandTimeout);
            }
        }

        public LambdaQueryHelper<T> LambdaQuery(IDbTransaction transaction = null, int? commandTimeout = null) {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.LambdaQuery<T>(transaction, commandTimeout);
            } else {
                return DapperExtension.LambdaQuery<T>(ConnKey, commandTimeout);
            }
        }

        public LambdaQueryHelper<TEntity> LambdaQuery<TEntity>(IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class {
            DbConnObj ConnObj = GetConnObj(transaction);
            if (ConnObj.DbConnection != null) {
                return ConnObj.DbConnection.LambdaQuery<TEntity>(transaction, commandTimeout);
            } else {
                return DapperExtension.LambdaQuery<TEntity>(ConnKey, commandTimeout);
            }
        }
        #endregion



        protected DbConnObj GetConnObj(IDbTransaction transaction = null) {
            DbConnObj dbConnObj = new DbConnObj();
            if (null != transaction) {
                dbConnObj.DbConnection = transaction.Connection;
                dbConnObj.DbTransaction = transaction;
            } else if (null != this.DBSession) {
                if (DBSession.Connection.ConnectionString == "") {
                    DBSession = Helper.GetPerHttpRequestDBSession(ConnKey);
                }
                dbConnObj.DbConnection = this.DBSession.Connection;
                dbConnObj.dbType = this.DBSession.dbType;
            }
            return dbConnObj;
        }
    }
}