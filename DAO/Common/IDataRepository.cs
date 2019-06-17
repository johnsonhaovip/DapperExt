using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using XNY.DataAccess;
using DapperExtensions;
using DapperExtensions.Lambda;

namespace DAO.Common {
    /// <summary>
    /// Repository接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataRepository<T>
        where T : class {
        string ConnKey { get; }

        IDBSession DBSession { get; }

        /// <summary>
        /// 根据主键获得实体对象
        /// </summary>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        T GetById(dynamic primaryId);

        /// <summary>
        /// 根据主键获得实体对象
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        TReturn GetById<TReturn>(dynamic primaryId) where TReturn : class;

        /// <summary>
        /// 根据主键列表 获得实体对象集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<T> GetByIds(IList<dynamic> ids);

        /// <summary>
        /// 根据主键列表 获得实体对象集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<TReturn> GetByIds<TReturn>(IList<dynamic> ids) where TReturn : class;

        /// <summary>
        /// 活动所有数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll(string tableName = null);

        /// <summary>
        /// 活动所有数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<TReturn> GetAll<TReturn>() where TReturn : class;

        /// <summary>
        /// 返回数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(object predicate);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        IEnumerable<T> GetList(object predicate = null, IList<ISort> sort = null);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        IEnumerable<TReturn> GetList<TReturn>(object predicate = null, IList<ISort> sort = null) where TReturn : class;


        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allRowsCount"></param>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        IEnumerable<T> GetPageList(int pageIndex, int pageSize, out long allRowsCount, object predicate = null, IList<ISort> sort = null);


        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allRowsCount"></param>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        IEnumerable<TReturn> GetPageList<TReturn>(int pageIndex, int pageSize, out long allRowsCount, object predicate = null, IList<ISort> sort = null) where TReturn : class;



        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        dynamic Insert(T entity, IDbTransaction transaction = null);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool InsertBatch(IEnumerable<T> entityList, IDbTransaction transaction = null);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Update(T entity, IDbTransaction transaction = null);

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool UpdateBatch(IEnumerable<T> entityList, IDbTransaction transaction = null);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="primaryId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int Delete(dynamic primaryId, IDbTransaction transaction = null);

        /// <summary>
        ///根据条件 批量删除数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int DeleteList(object predicate, IDbTransaction transaction = null);

        /// <summary>
        /// 根据主键列表 批量删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool DeleteBatch(IEnumerable<dynamic> ids, IDbTransaction transaction = null);


        /// <summary>
        /// 支持lambda的Insert帮助类
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        //LambdaInsertHelper<T> LambdaInsert(IDbTransaction transaction = null, int? commandTimeout = null);


        /// <summary>
        /// 支持lambda的Insert帮助类
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        //LambdaInsertHelper<TEntity> LambdaInsert<TEntity>(IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class;


        /// <summary>
        /// 支持lambda的Delete帮助类
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        LambdaDeleteHelper<T> LambdaDelete(IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// 支持lambda的Delete帮助类
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        LambdaDeleteHelper<TEntity> LambdaDelete<TEntity>(IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class;

        /// <summary>
        /// 支持lambda的Update帮助类
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        LambdaUpdateHelper<T> LambdaUpdate(IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// 支持lambda的Update帮助类
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        LambdaUpdateHelper<TEntity> LambdaUpdate<TEntity>(IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class;

        /// <summary>
        /// 支持lambda的查询帮助类
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        LambdaQueryHelper<T> LambdaQuery(IDbTransaction transaction = null, int? commandTimeout = null);


        /// <summary>
        /// 支持lambda的查询帮助类
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        LambdaQueryHelper<TEntity> LambdaQuery<TEntity>(IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class;

        //-------------------------------------

        //IEnumerable<T> Get(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        //IEnumerable<T2> Get<T2>(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) where T2 : class;


        //IEnumerable<T> Get<TFirst, TSecond>(string sql, Func<TFirst, TSecond, T> map,
        //        dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id",
        //        int? commandTimeout = null, CommandType? commandType = null);

        //IEnumerable<T2> Get<TFirst, TSecond, T2>(string sql, Func<TFirst, TSecond, T2> map,
        //        dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id",
        //        int? commandTimeout = null, CommandType? commandType = null) where T2 : class;

        //IEnumerable<T> Get<TFirst, TSecond, TThird>(string sql, Func<TFirst, TSecond, TThird, T> map,
        //        dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id",
        //        int? commandTimeout = null, CommandType? commandType = null);

        //IEnumerable<T2> Get<TFirst, TSecond, TThird, T2>(string sql, Func<TFirst, TSecond, TThird, T2> map,
        //        dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id",
        //        int? commandTimeout = null, CommandType? commandType = null) where T2 : class;


        //SqlMapper.GridReader GetMultiple(string sql, dynamic param = null, IDbTransaction transaction = null,
        //        int? commandTimeout = null, CommandType? commandType = null);


        //IEnumerable<T> GetPage(int pageIndex, int pageSize, out long allRowsCount, string sql, dynamic param = null, string allRowsCountSql = null, dynamic allRowsCountParam = null, int? commandTimeout = null, CommandType? commandType = null);
        //IEnumerable<T2> GetPage<T2>(int pageIndex, int pageSize, out long allRowsCount, string sql, dynamic param = null, string allRowsCountSql = null, dynamic allRowsCountParam = null, int? commandTimeout = null, CommandType? commandType = null) where T2 : class;

        //Int32 Execute(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);




    }
}
