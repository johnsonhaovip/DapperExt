using DapperExtensions.Lambda;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using XNY.DataAccess;
using XNY.DataAccess.Utils;

namespace DapperExtensions {
    public static class DapperExtension {
        [ThreadStatic]
        private static IDapperImplementor _DapperImplementor_MultipleDBType;//应对多类型数据库  需要配置 EnabledMultipleDB =true
        private static IDapperImplementor _DapperImplementor;//单一类型数据库
        private static readonly bool IsEnabledMultipleDBType = Convert.ToBoolean((ConfigurationManager.AppSettings["IsEnabledMultipleDBType"] ?? "false"));//是否启用多类型数据库
        private const DataBaseType DefaultDBType = DataBaseType.SqlServer;
        private static Func<IDapperExtensionConfiguration, DataBaseType, IDapperImplementor> _instanceFactory;
        private static ConcurrentDictionary<DataBaseType, IDapperImplementor> _instanceList = new ConcurrentDictionary<DataBaseType, IDapperImplementor>();
        private static readonly string ormMapperAssemblyStr = (ConfigurationManager.AppSettings["OrmMapperAssemblyStr"] ?? "");//ORM映射的程序集

        public static IDapperImplementor DapperImplementor {
            get {
                if (!IsEnabledMultipleDBType) {
                    if (null == _DapperImplementor) {
                        Instance();
                    }
                    return DapperExtension._DapperImplementor;
                } else {
                    if (null == _DapperImplementor_MultipleDBType) {
                        Instance();
                    }
                    return DapperExtension._DapperImplementor_MultipleDBType;
                }
            }
        }

        /// <summary>
        /// Get or sets the Dapper Extensions Implementation Factory.
        /// </summary>
        public static Func<IDapperExtensionConfiguration, DataBaseType, IDapperImplementor> InstanceFactory {
            get {
                if (_instanceFactory == null) {
                    _instanceFactory = (config, dbType) => new DapperImplementor(new SqlGeneratorImpl(config, dbType), dbType);
                }
                return _instanceFactory;
            }
        }

        /// <summary>
        /// 活动Dapper扩展对象的实例
        /// </summary>
        /// <param name="dbType">DB类型，默认SqlServer</param>
        /// <param name="ormMapperAssemblyList">ORM映射的程序集</param>
        /// <returns></returns>
        public static IDapperImplementor Instance(DataBaseType dbType = DefaultDBType, List<string> ormMapperAssemblyList = null) {
            IDapperImplementor instance;
            if (!_instanceList.TryGetValue(dbType, out instance)) {
                ISqlDialect sqlDialect;
                switch (dbType) {
                    case DataBaseType.SqlServer: sqlDialect = new SqlServerDialect(); break;
                    case DataBaseType.Oracle: sqlDialect = new OracleDialect(); break;
                    case DataBaseType.MySql: sqlDialect = new MySqlDialect(); break;
                    default: sqlDialect = new SqlServerDialect(); break;
                }
                List<Assembly> assemblyList = new List<Assembly>();
                if (ormMapperAssemblyList != null) {
                    foreach (var item in ormMapperAssemblyList) {
                        assemblyList.Add(Assembly.Load(item));
                    }
                } else {
                    if (!string.IsNullOrEmpty(ormMapperAssemblyStr)) {
                        foreach (var item in ormMapperAssemblyStr.Split(';')) {
                            if (!string.IsNullOrEmpty(item)) {
                                assemblyList.Add(Assembly.Load(item));
                            }
                        }
                    }
                }
                IDapperExtensionConfiguration iDapperExtensionsConfiguration = new DapperExtensionConfiguration(
                    typeof(AutoClassMapper<>),
                   assemblyList,
                    sqlDialect
                    );
                instance = InstanceFactory(iDapperExtensionsConfiguration, dbType);
                _instanceList[dbType] = instance;
            }
            if (!IsEnabledMultipleDBType) {
                if (null == _DapperImplementor) {
                    _DapperImplementor = instance;
                }
            } else {
                if (null == _DapperImplementor_MultipleDBType || _DapperImplementor_MultipleDBType.DbType != dbType) {
                    _DapperImplementor_MultipleDBType = instance;
                }
            }
            return instance;
        }



        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static T Get<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            var result = Instance(dbType).Get<T, T>(connection, id, transaction, commandTimeout);
            return (T)result;
        }


        public static T Get<T>(string connKey, dynamic id, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return Get<T>(conn, id, null, commandTimeout, dbType);
            }
        }



        /// <summary>
        /// Executes a query for the specified id, returning the data typed as per T
        /// </summary>
        public static TReturn Get<T, TReturn>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType)
            where T : class
            where TReturn : class {
            var result = Instance(dbType).Get<T, TReturn>(connection, id, transaction, commandTimeout);
            return (TReturn)result;
        }


        /// <summary>
        /// Executes a query for the specified id, returning the data typed as per T
        /// </summary>
        public static TReturn Get<T, TReturn>(string connKey, dynamic id, int? commandTimeout = null, DataBaseType dbType = DefaultDBType)
            where T : class
            where TReturn : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return Get<T, TReturn>(conn, id, null, commandTimeout, dbType);
            }
        }




        /// <summary>
        /// Executes an insert query for the specified entity.
        /// </summary>
        public static void Insert<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            Instance(dbType).Insert<T>(connection, entities, transaction, commandTimeout);
        }


        public static void Insert<T>(string connKey, IEnumerable<T> entities, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                Insert<T>(conn, entities, null, commandTimeout, dbType);
            }
        }



        /// <summary>
        /// Executes an insert query for the specified entity, returning the primary key.  
        /// If the entity has a single key, just the value is returned.  
        /// If the entity has a composite key, an IDictionary&lt;string, object&gt; is returned with the key values.
        /// The key value for the entity will also be updated if the KeyType is a Guid or Identity.
        /// </summary>
        public static dynamic Insert<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).Insert<T>(connection, entity, transaction, commandTimeout);
        }

        public static dynamic Insert<T>(string connKey, T entity, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return Insert<T>(conn, entity, null, commandTimeout, dbType);
            }
        }




        /// <summary>
        /// Executes an update query for the specified entity.
        /// </summary>
        public static bool Update<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).Update<T>(connection, entity, transaction, commandTimeout);
        }

        public static bool Update<T>(string connKey, T entity, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return Update<T>(conn, entity, null, commandTimeout, dbType);
            }
        }





        /// <summary>
        /// Executes a delete query for the specified entity.
        /// </summary>
        public static int Delete<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).Delete<T>(connection, entity, transaction, commandTimeout);
        }


        public static int Delete<T>(string connKey, T entity, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return Delete<T>(conn, entity, null, commandTimeout, dbType);
            }
        }




        /// <summary>
        /// Executes a delete query using the specified predicate.
        /// </summary>
        public static int Delete<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).Delete<T>(connection, predicate, transaction, commandTimeout);
        }

        public static int Delete<T>(string connKey, object predicate, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return Delete<T>(conn, predicate, null, commandTimeout, dbType);
            }
        }


        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// </summary>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null, string tableName = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).GetList<T, T>(connection, predicate, sort, transaction, commandTimeout, tableName);
        }
        public static IEnumerable<T> GetList<T>(string connKey, object predicate = null, IList<ISort> sort = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return GetList<T>(conn, predicate, sort, null, commandTimeout, null, dbType);
            }
        }



        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// </summary>
        public static IEnumerable<TReturn> GetList<T, TReturn>(this IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType)
            where T : class
            where TReturn : class {
            return Instance(dbType).GetList<T, TReturn>(connection, predicate, sort, transaction, commandTimeout);
        }

        public static IEnumerable<TReturn> GetList<T, TReturn>(string connKey, object predicate = null, IList<ISort> sort = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType)
            where T : class
            where TReturn : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return GetList<T, TReturn>(conn, predicate, sort, null, commandTimeout, dbType);
            }
        }

        public static IEnumerable<T> GetPage<T>(this IDbConnection connection, int page, int resultsPerPage, out long allRowsCount, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).GetPage<T>(connection, page, resultsPerPage, out allRowsCount, sql, param, allRowsCountSql, transaction, commandTimeout);
        }

        public static IEnumerable<T> GetPage<T>(string connKey, int page, int resultsPerPage, out long allRowsCount, string sql, dynamic param = null, string allRowsCountSql = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return GetPage<T>(conn, page, resultsPerPage, out allRowsCount, sql, param, allRowsCountSql, null, commandTimeout, dbType);
            }
        }


        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified page and resultsPerPage.
        /// </summary>
        public static IEnumerable<T> GetPage<T>(this IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).GetPage<T, T>(connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout);
        }

        public static IEnumerable<T> GetPage<T>(string connKey, object predicate, IList<ISort> sort, int page, int resultsPerPage, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return GetPage<T>(conn, predicate, sort, page, resultsPerPage, null, commandTimeout, dbType);
            }
        }


        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified page and resultsPerPage.
        /// </summary>
        public static IEnumerable<TReturn> GetPage<T, TReturn>(this IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType)
            where T : class
            where TReturn : class {
            return Instance(dbType).GetPage<T, TReturn>(connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout);
        }

        public static IEnumerable<TReturn> GetPage<T, TReturn>(string connKey, object predicate, IList<ISort> sort, int page, int resultsPerPage, int? commandTimeout = null, DataBaseType dbType = DefaultDBType)
            where T : class
            where TReturn : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return GetPage<TReturn>(conn, predicate, sort, page, resultsPerPage, null, commandTimeout, dbType);
            }
        }



        /// <summary>
        /// Executes a query using the specified predicate, returning an integer that represents the number of rows that match the query.
        /// </summary>
        public static int Count<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).Count<T>(connection, predicate, transaction, commandTimeout);
        }

        public static int Count<T>(string connKey, object predicate, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return Count<T>(conn, predicate, null, commandTimeout, dbType);
            }
        }


        /// <summary>
        /// Executes a select query for multiple objects, returning IMultipleResultReader for each predicate.
        /// </summary>
        public static IMultipleResultReader GetMultiple(this IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) {
            return Instance(dbType).GetMultiple(connection, predicate, transaction, commandTimeout);
        }


        public static IMultipleResultReader GetMultiple(string connKey, GetMultiplePredicate predicate, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) {
            using (IDbConnection conn = DBUtils.CreateDBConnection(dbType, connKey)) {
                return GetMultiple(conn, predicate, null, commandTimeout, dbType);
            }
        }


        /// <summary>
        /// Gets the appropriate mapper for the specified type T. 
        /// If the mapper for the type is not yet created, a new mapper is generated from the mapper type specifed by DefaultMapper.
        /// </summary>
        public static IClassMapper GetMap<T>(DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).SqlGenerator.Configuration.GetMap<T>();
        }

        /// <summary>
        /// Clears the ClassMappers for each type.
        /// </summary>
        public static void ClearCache(DataBaseType dbType = DefaultDBType) {
            Instance(dbType).SqlGenerator.Configuration.ClearCache();
        }

        /// <summary>
        /// Generates a COMB Guid which solves the fragmented index issue.
        /// See: http://davybrion.com/blog/2009/05/using-the-guidcomb-identifier-strategy
        /// </summary>
        public static Guid GetNextGuid(DataBaseType dbType = DefaultDBType) {
            return Instance(dbType).SqlGenerator.Configuration.GetNextGuid();
        }


        //public static LambdaInsertHelper<T> LambdaInsert<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class
        //{
        //    return Instance(dbType).LambdaInsert<T>(connection, transaction, commandTimeout);
        //}


        public static LambdaDeleteHelper<T> LambdaDelete<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).LambdaDelete<T>(connection, transaction, commandTimeout);
        }

        public static LambdaUpdateHelper<T> LambdaUpdate<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).LambdaUpdate<T>(connection, transaction, commandTimeout);
        }

        public static LambdaQueryHelper<T> LambdaQuery<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).LambdaQuery<T>(connection, transaction, commandTimeout);
        }


        public static LambdaDeleteHelper<T> LambdaDelete<T>(string connKey, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).LambdaDelete<T>(connKey, commandTimeout);
        }

        public static LambdaUpdateHelper<T> LambdaUpdate<T>(string connKey, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).LambdaUpdate<T>(connKey, commandTimeout);
        }

        public static LambdaQueryHelper<T> LambdaQuery<T>(string connKey, int? commandTimeout = null, DataBaseType dbType = DefaultDBType) where T : class {
            return Instance(dbType).LambdaQuery<T>(connKey, commandTimeout);
        }

    }
}
