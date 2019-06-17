using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using DapperExtensions;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using DapperExtensions.ValueObject;
using XNY.DataAccess;
using System.Configuration;
using XNY.DataAccess.Utils;

namespace DapperExtensions.Lambda
{
    public class LambdaQueryHelper<T> : LambdaQueryHelper where T : class
    {

        public LambdaQueryHelper(IDbConnection connection, IDbTransaction transaction, IClassMapper classMap, int? commandTimeout = null)
            : base(connection, transaction, classMap, commandTimeout)
        {
        }

        public LambdaQueryHelper(string connKey, IClassMapper classMap, int? commandTimeout = null)
            : base(connKey, classMap, commandTimeout)
        {
        }

        #region Sql组装  Select  OrderBy  Group ……



        #region Select

        public LambdaQueryHelper<T> Select(ISelect select)
        {
            return (LambdaQueryHelper<T>)base.Select(select.Fields.ToArray());
        }

        public LambdaQueryHelper<T> Select(ISelect<T> select)
        {
            return (LambdaQueryHelper<T>)base.Select(select.Fields.ToArray());
        }


        public LambdaQueryHelper<T> Select(Expression<Func<T, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2>(Expression<Func<T, T2, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }


        public LambdaQueryHelper<T> Select(Expression<Func<T, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2>(Expression<Func<T, T2, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> Select<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.Select(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }




        public LambdaQueryHelper<T> AddSelect(ISelect select)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(select.Fields.ToArray());
        }

        public LambdaQueryHelper<T> AddSelect(ISelect<T> select)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(select.Fields.ToArray());
        }

        public LambdaQueryHelper<T> AddSelect(Expression<Func<T, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2>(Expression<Func<T, T2, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }


        public LambdaQueryHelper<T> AddSelect(Expression<Func<T, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2>(Expression<Func<T, T2, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public LambdaQueryHelper<T> AddSelect<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaSelect)
        {
            return (LambdaQueryHelper<T>)base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }



        #endregion

        public LambdaQueryHelper<T> Where(IWhere where)
        {
            return (LambdaQueryHelper<T>)base.Where(where.ToWhereClip());
        }

        public LambdaQueryHelper<T> Where(IWhere<T> where)
        {
            return (LambdaQueryHelper<T>)base.Where(where.ToWhereClip());
        }

        /// <summary>
        /// 
        /// </summary>
        public LambdaQueryHelper<T> Where(Expression<Func<T, bool>> lambdaWhere)
        {
            return (LambdaQueryHelper<T>)Where(ExpressionToClip<T>.ToWhereClip(lambdaWhere));
        }

        /// <summary>
        /// 
        /// </summary>
        public LambdaQueryHelper<T> Where<T2>(Expression<Func<T, T2, bool>> lambdaWhere)
        {
            return (LambdaQueryHelper<T>)Where(ExpressionToClip<T>.ToWhereClip(lambdaWhere));
        }

        /// <summary>
        /// 
        /// </summary>
        public LambdaQueryHelper<T> Where<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaWhere)
        {
            return (LambdaQueryHelper<T>)Where(ExpressionToClip<T>.ToWhereClip(lambdaWhere));
        }

        /// <summary>
        /// 
        /// </summary>
        public LambdaQueryHelper<T> Where<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaWhere)
        {
            return (LambdaQueryHelper<T>)Where(ExpressionToClip<T>.ToWhereClip(lambdaWhere));
        }
        /// <summary>
        /// 
        /// </summary>
        public LambdaQueryHelper<T> Where<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaWhere)
        {
            return (LambdaQueryHelper<T>)Where(ExpressionToClip<T>.ToWhereClip(lambdaWhere));
        }

        /// <summary>
        /// 
        /// </summary>
        public LambdaQueryHelper<T> Where<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaWhere)
        {
            return (LambdaQueryHelper<T>)Where(ExpressionToClip<T>.ToWhereClip(lambdaWhere));
        }

        public LambdaQueryHelper<T> Having(IWhere having)
        {
            return (LambdaQueryHelper<T>)base.Having(having.ToWhereClip());
        }

        public LambdaQueryHelper<T> Having(Expression<Func<T, bool>> lambdaHaving)
        {
            return (LambdaQueryHelper<T>)base.Having(ExpressionToClip<T>.ToWhereClip(lambdaHaving));
        }



        public new LambdaQueryHelper<T> Distinct()
        {
            return (LambdaQueryHelper<T>)base.Distinct();
        }

        public new LambdaQueryHelper<T> WithNoLock()
        {
            return (LambdaQueryHelper<T>)base.WithNoLock();
        }


        public new LambdaQueryHelper<T> Top(int topCount)
        {
            return (LambdaQueryHelper<T>)base.Top(topCount);
        }


        #region OrderBy


        public LambdaQueryHelper<T> OrderBy(IOrderBy orderby)
        {
            return (LambdaQueryHelper<T>)base.OrderBy(orderby.ToOrderByClip());
        }

        public LambdaQueryHelper<T> OrderBy(IOrderBy<T> orderby)
        {
            return (LambdaQueryHelper<T>)base.OrderBy(orderby.ToOrderByClip());
        }

        public LambdaQueryHelper<T> OrderBy(Expression<Func<T, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public LambdaQueryHelper<T> OrderBy<T2>(Expression<Func<T, T2, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }
        public LambdaQueryHelper<T> OrderBy<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }
        public LambdaQueryHelper<T> OrderBy<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public LambdaQueryHelper<T> OrderBy<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public LambdaQueryHelper<T> OrderBy<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }


        public LambdaQueryHelper<T> OrderByDescending(Expression<Func<T, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }
        public LambdaQueryHelper<T> OrderByDescending<T2>(Expression<Func<T, T2, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public LambdaQueryHelper<T> OrderByDescending<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public LambdaQueryHelper<T> OrderByDescending<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public LambdaQueryHelper<T> OrderByDescending<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public LambdaQueryHelper<T> OrderByDescending<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)OrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }




        public LambdaQueryHelper<T> AddOrderBy(Expression<Func<T, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public LambdaQueryHelper<T> AddOrderBy<T2>(Expression<Func<T, T2, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }
        public LambdaQueryHelper<T> AddOrderBy<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }
        public LambdaQueryHelper<T> AddOrderBy<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public LambdaQueryHelper<T> AddOrderBy<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public LambdaQueryHelper<T> AddOrderBy<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }


        public LambdaQueryHelper<T> AddOrderByDescending(Expression<Func<T, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }
        public LambdaQueryHelper<T> AddOrderByDescending<T2>(Expression<Func<T, T2, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public LambdaQueryHelper<T> AddOrderByDescending<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public LambdaQueryHelper<T> AddOrderByDescending<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public LambdaQueryHelper<T> AddOrderByDescending<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public LambdaQueryHelper<T> AddOrderByDescending<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaOrderBy)
        {
            return (LambdaQueryHelper<T>)base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }


        #endregion


        public LambdaQueryHelper<T> GroupBy(Expression<Func<T, object>> lambdaGroupBy)
        {
            return (LambdaQueryHelper<T>)base.GroupBy(ExpressionToClip<T>.ToGroupByClip(lambdaGroupBy));
        }

        #endregion

        #region Join

        private new LambdaQueryHelper<T> Join(string tableName, WhereClip where, JoinType joinType)
        {
            return (LambdaQueryHelper<T>)base.Join(tableName, where, joinType);
        }


        public LambdaQueryHelper<T> InnerJoin<TEntity>(WhereClip where) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, where, JoinType.InnerJoin);
        }


        public LambdaQueryHelper<T> InnerJoin<TEntity>(Expression<Func<T, TEntity, bool>> lambdaWhere) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, ExpressionToClip<T>.ToJoinWhere(lambdaWhere), JoinType.InnerJoin);
        }

        public LambdaQueryHelper<T> InnerJoin<T1, T2>(Expression<Func<T1, T2, bool>> lambdaWhere)
            where T1 : class
            where T2 : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<T2>().TableName, ExpressionToClip<T1>.ToJoinWhere(lambdaWhere), JoinType.InnerJoin);
        }



        public LambdaQueryHelper<T> LeftJoin<TEntity>(WhereClip where) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, where, JoinType.LeftJoin);
        }

        public LambdaQueryHelper<T> LeftJoin<TEntity>(Expression<Func<T, TEntity, bool>> lambdaWhere) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, ExpressionToClip<T>.ToJoinWhere(lambdaWhere), JoinType.LeftJoin);
        }

        public LambdaQueryHelper<T> LeftJoin<T1, T2>(Expression<Func<T1, T2, bool>> lambdaWhere)
            where T1 : class
            where T2 : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<T2>().TableName, ExpressionToClip<T1>.ToJoinWhere(lambdaWhere), JoinType.LeftJoin);
        }


        public LambdaQueryHelper<T> RightJoin<TEntity>(WhereClip where) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, where, JoinType.RightJoin);
        }

        public LambdaQueryHelper<T> RightJoin<TEntity>(Expression<Func<T, TEntity, bool>> lambdaWhere) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, ExpressionToClip<T>.ToJoinWhere(lambdaWhere), JoinType.RightJoin);
        }

        public LambdaQueryHelper<T> RightJoin<T1, T2>(Expression<Func<T1, T2, bool>> lambdaWhere)
            where T1 : class
            where T2 : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<T2>().TableName, ExpressionToClip<T1>.ToJoinWhere(lambdaWhere), JoinType.RightJoin);
        }

        public LambdaQueryHelper<T> CrossJoin<TEntity>(WhereClip where) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, where, JoinType.CrossJoin);
        }

        public LambdaQueryHelper<T> CrossJoin<TEntity>(Expression<Func<T, TEntity, bool>> lambdaWhere) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, ExpressionToClip<T>.ToJoinWhere(lambdaWhere), JoinType.CrossJoin);
        }

        public LambdaQueryHelper<T> CrossJoin<T1, T2>(Expression<Func<T1, T2, bool>> lambdaWhere)
            where T1 : class
            where T2 : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<T2>().TableName, ExpressionToClip<T1>.ToJoinWhere(lambdaWhere), JoinType.CrossJoin);
        }


        public LambdaQueryHelper<T> FullJoin<TEntity>(WhereClip where) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, where, JoinType.FullJoin);
        }

        public LambdaQueryHelper<T> FullJoin<TEntity>(Expression<Func<T, TEntity, bool>> lambdaWhere) where TEntity : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<TEntity>().TableName, ExpressionToClip<T>.ToJoinWhere(lambdaWhere), JoinType.FullJoin);
        }

        public LambdaQueryHelper<T> FullJoin<T1, T2>(Expression<Func<T1, T2, bool>> lambdaWhere)
            where T1 : class
            where T2 : class
        {
            return Join(DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<T2>().TableName, ExpressionToClip<T1>.ToJoinWhere(lambdaWhere), JoinType.FullJoin);
        }



        #endregion

        #region Execute
        public new IDataReader ToDataReader()
        {
            return base.ToDataReader();
        }

        public new DataSet ToDataSet()
        {
            return base.ToDataSet();
        }

        public new DataSet ToPageDataSet(int pageIndex, int pageSize)
        {
            return base.ToPageDataSet(pageIndex, pageSize);
        }


        public new DataTable ToDataTable()
        {
            return base.ToDataTable();
        }


        public IEnumerable<T> ToList()
        {
            return base.ToList<T>();
        }
        public IEnumerable<T> ToPageList(int pageIndex, int pageSize)
        {
            return base.ToPageList<T>(pageIndex, pageSize);
        }

        public IEnumerable<T> ToPageList(int pageIndex, int pageSize, out long allRowsCount)
        {
            return base.ToPageList<T>(pageIndex, pageSize, out allRowsCount);
        }
        #endregion



    }


    public class LambdaQueryHelper
    {
        #region 属性 变量
        public IClassMapper ClassMap { get; private set; }
        public IDbConnection Connection { get; private set; }

        public string ConnKey { get; private set; }

        public IDbTransaction Transaction { get; private set; }
        public DataBaseType DbType { get; private set; }

        [DefaultValue(30000)]
        public int CommandTimeout { get; private set; }
        public int? PageIndex { get; private set; }
        public int? PageSize { get; private set; }
        public string DistinctString { get; private set; }

        [DefaultValue(false)]
        public bool EnabledNoLock { get; private set; }

        private string _SqlString = string.Empty;
        private WhereClip _WhereClip = WhereClip.All;
        private WhereClip _HavingClip = WhereClip.All;
        private OrderByClip _OrderByClip = OrderByClip.None;
        private GroupByClip _GroupByClip = GroupByClip.None;
        private Dictionary<string, KeyValuePair<string, WhereClip>> _Joins = new Dictionary<string, KeyValuePair<string, WhereClip>>();
        private List<Field> _Fields = new List<Field>();
        private Dictionary<string, Parameter> _Parameters = new Dictionary<string, Parameter>();

        public WhereClip WhereClip
        {
            get { return _WhereClip; }
            private set { _WhereClip = value; }
        }

        public WhereClip HavingClip
        {
            get { return _HavingClip; }
            private set { _HavingClip = value; }
        }
        public OrderByClip OrderByClip
        {
            get { return _OrderByClip; }
            private set { _OrderByClip = value; }
        }
        public GroupByClip GroupByClip
        {
            get { return _GroupByClip; }
            private set { _GroupByClip = value; }
        }


        public Dictionary<string, KeyValuePair<string, WhereClip>> Joins
        {
            get { return _Joins; }
            private set { _Joins = value; }
        }


        public List<Field> Fields
        {
            get { return _Fields; }
            private set { _Fields = value; }
        }

        public Dictionary<string, Parameter> Parameters
        {
            get
            {
                LoadParameters();
                return _Parameters;
            }
            internal set
            {
                this._Parameters = value;
            }


        }


        #endregion

        #region 构造函数
        public LambdaQueryHelper(IDbConnection connection, IDbTransaction transaction, IClassMapper classMap, int? commandTimeout = null)
        {
            this.Connection = connection;
            this.DbType = DapperExtension.DapperImplementor.DbType;
            this.Transaction = transaction;
            this.ClassMap = classMap;
            if (null != commandTimeout)
            {
                this.CommandTimeout = CommandTimeout;
            }
        }

        public LambdaQueryHelper(string connKey, IClassMapper classMap, int? commandTimeout = null)
        {
            this.ConnKey = connKey;
            this.DbType = DBUtils.GetDBTypeByConnKey(connKey);
            this.ClassMap = classMap;
            if (null != commandTimeout)
            {
                this.CommandTimeout = CommandTimeout;
            }
        }



        #endregion

        #region Sql组装  Select  OrderBy  Group ……


        /// <summary>
        /// 查询条件赋值
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public LambdaQueryHelper Select(params Field[] fields)
        {
            IsChangeSql();
            this._Fields.Clear();
            return AddSelect(fields);
        }

        /// <summary>
        /// 增加查询条件
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public LambdaQueryHelper AddSelect(params Field[] fields)
        {
            if (null != fields && fields.Length > 0)
            {
                IsChangeSql();
                foreach (Field field in fields)
                {
                    Field f = this._Fields.Find(fi => fi.Name.Equals(field.Name) && fi.TableName.Equals(field.TableName));
                    if (Field.IsNullOrEmpty(f))
                        this._Fields.Add(field);
                }
            }
            return this;
        }


        protected LambdaQueryHelper Where(WhereClip where)
        {
            IsChangeSql();
            this._WhereClip = where;
            return this;
        }


        protected LambdaQueryHelper OrderBy(OrderByClip orderBy)
        {
            IsChangeSql();
            this._OrderByClip = orderBy;
            return this;
        }

        protected LambdaQueryHelper AddOrderBy(OrderByClip orderBy)
        {
            IsChangeSql();
            this._OrderByClip = this._OrderByClip && orderBy; ;
            return this;
        }




        public LambdaQueryHelper GroupBy(GroupByClip groupBy)
        {
            IsChangeSql();
            this._GroupByClip = groupBy;
            return this;
        }


        public LambdaQueryHelper Having(WhereClip havingWhere)
        {
            IsChangeSql();
            this._HavingClip = havingWhere;
            return this;
        }

        public LambdaQueryHelper Distinct()
        {
            IsChangeSql();
            this.DistinctString = " DISTINCT ";
            return this;
        }


        public LambdaQueryHelper WithNoLock()
        {
            IsChangeSql();
            this.EnabledNoLock = true;
            return this;
        }

        public LambdaQueryHelper Top(int topCount)
        {
            if (topCount > 0)
            {
                IsChangeSql();
                this.PageIndex = 1;
                this.PageSize = topCount;
            }
            return this;
        }

        #endregion

        #region  JOIN


        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        protected LambdaQueryHelper Join(string tableName, WhereClip where, JoinType joinType)
        {
            IsChangeSql();
            if (string.IsNullOrEmpty(tableName) || WhereClip.IsNullOrEmpty(where))
                return this;
            if (!_Joins.ContainsKey(tableName))
            {
                _Joins.Add(tableName, new KeyValuePair<string, WhereClip>(DapperExtension.DapperImplementor.SqlGenerator.Configuration.Dialect.GetJoinString(joinType), where));
                if (where.Parameters.Count > 0)
                {
                    foreach (var item in where.Parameters)
                    {
                        _Parameters.Add(item.ParameterName, item);
                    }
                }

            }

            return this;
        }
        #endregion



        /// <summary>
        /// 执行的sql语句
        /// </summary>
        /// <returns></returns>
        public string GetSqlString(int? pageIndex = null, int? pageSize = null, bool loadOrderby = true, bool reLoadSql = false)
        {
            if (pageIndex != null && pageSize != null)
            {
                ReLoadParameters();
                return DapperExtension.DapperImplementor.SqlGenerator.LambdaSelect(this, ref _Parameters, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize), loadOrderby);
            }
            else
            {
                if (string.IsNullOrEmpty(_SqlString))
                {
                    ReLoadParameters();
                    _SqlString = DapperExtension.DapperImplementor.SqlGenerator.LambdaSelect(this, ref _Parameters);
                }
                return _SqlString;
            }
        }


        public string GetCountSqlString()
        {
            ReLoadParameters();
            return
                DapperExtension.DapperImplementor.SqlGenerator.PageCount(
                DapperExtension.DapperImplementor.SqlGenerator.LambdaSelect(this, ref _Parameters, null, null, loadOrderby: false));
        }





        #region Execute
        #region ToDataReader
        public IDataReader ToDataReader()
        {
            DbConnObj ConnObj = DBUtils.GetConnObj(Connection, Transaction);
            if (ConnObj.DbTransaction != null)
            {
                return DBUtils.GetDBHelper(DbType).ExecuteReader(Transaction, GetSqlString(), DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
            else if (ConnObj.DbConnection != null)
            {
                return DBUtils.GetDBHelper(DbType).ExecuteReader(Connection, GetSqlString(), DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
            else
            {
                return DBUtils.GetDBHelper(DbType).ExecuteReader(ConnKey, GetSqlString(), DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
        }

        #endregion


        public DataSet ToDataSet()
        {
            DbConnObj ConnObj = DBUtils.GetConnObj(Connection, Transaction);
            if (ConnObj.DbTransaction != null)
            {
                return DBUtils.GetDBHelper(DbType).ExecuteDataSet(Transaction, GetSqlString(), DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
            else if (ConnObj.DbConnection != null)
            {
                return DBUtils.GetDBHelper(DbType).ExecuteDataSet(Connection, GetSqlString(), DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
            else
            {
                return DBUtils.GetDBHelper(DbType).ExecuteDataSet(ConnKey, GetSqlString(), DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
        }


        public DataSet ToPageDataSet(int pageIndex, int pageSize)
        {
            string sql = GetSqlString(pageIndex, pageSize);
            string sqlcount = GetCountSqlString();

            string execSql = sql + " ; " + sqlcount;
            DbConnObj ConnObj = DBUtils.GetConnObj(Connection, Transaction);
            if (ConnObj.DbTransaction != null)
            {
                return DBUtils.GetDBHelper(DbType).ExecuteDataSet(Transaction, execSql, DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
            else if (ConnObj.DbConnection != null)
            {
                return DBUtils.GetDBHelper(DbType).ExecuteDataSet(Connection, execSql, DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
            else
            {
                return DBUtils.GetDBHelper(DbType).ExecuteDataSet(ConnKey, execSql, DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
        }


        public DataTable ToDataTable()
        {
            return this.ToDataSet().Tables[0];
        }



        public IEnumerable<T> ToList<T>() where T : class
        {
            string sql = GetSqlString();
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var item in Parameters)
            {
                dynamicParameters.Add(item.Value.ParameterName, item.Value.ParameterValue);
            }
            DbConnObj ConnObj = DBUtils.GetConnObj(Connection, Transaction);
            if (ConnObj.DbConnection != null)
            {
                return Connection.Query<T>(sql, dynamicParameters, Transaction, true, CommandTimeout, CommandType.Text);
            }
            else
            {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType))
                {
                    return conn.Query<T>(sql, dynamicParameters, Transaction, true, CommandTimeout, CommandType.Text);
                }
            }
        }


        public IEnumerable<T> ToPageList<T>(int pageIndex, int pageSize) where T : class
        {
            string sql = GetSqlString(pageIndex, pageSize);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var item in Parameters)
            {
                dynamicParameters.Add(item.Value.ParameterName, item.Value.ParameterValue);
            }
            DbConnObj ConnObj = DBUtils.GetConnObj(Connection, Transaction);
            if (ConnObj.DbConnection != null)
            {
                return Connection.Query<T>(sql, dynamicParameters, Transaction, true, CommandTimeout, CommandType.Text);
            }
            else
            {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType))
                {
                    return conn.Query<T>(sql, dynamicParameters, Transaction, true, CommandTimeout, CommandType.Text);
                }
            }
        }

        public IEnumerable<T> ToPageList<T>(int pageIndex, int pageSize, out long allRowsCount) where T : class
        {
            allRowsCount = 0;
            string sql = GetSqlString(pageIndex, pageSize);
            string sqlcount = GetCountSqlString();

            string execSql = sql + " ; " + sqlcount;
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var item in Parameters)
            {
                dynamicParameters.Add(item.Value.ParameterName, item.Value.ParameterValue);
            }
            DbConnObj ConnObj = DBUtils.GetConnObj(Connection, Transaction);
            if (ConnObj.DbConnection != null)
            {
                using (var multi = Connection.QueryMultiple(execSql, dynamicParameters, Transaction, CommandTimeout, CommandType.Text))
                {
                    var list = multi.Read<T>();
                    allRowsCount = multi.Read().Single().Total;
                    return list;
                }
            }
            else
            {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType))
                {
                    using (var multi = conn.QueryMultiple(execSql, dynamicParameters, Transaction, CommandTimeout, CommandType.Text))
                    {
                        var list = multi.Read<T>();
                        allRowsCount = multi.Read<long>().Single();
                        return list;
                    }
                }
            }
        }



        public IEnumerable<TReturn> ToList<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        {
            string sql = GetSqlString();
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var item in Parameters)
            {
                dynamicParameters.Add(item.Value.ParameterName, item.Value.ParameterValue);
            }
            DbConnObj ConnObj = DBUtils.GetConnObj(Connection, Transaction);
            if (ConnObj.DbConnection != null)
            {
                return Connection.Query<TFirst, TSecond, TReturn>(sql, map, dynamicParameters, Transaction, true, splitOn, CommandTimeout, CommandType.Text);
            }
            else
            {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType))
                {
                    return conn.Query<TFirst, TSecond, TReturn>(sql, map, dynamicParameters, Transaction, true, splitOn, CommandTimeout, CommandType.Text);
                }
            }
        }

        public IEnumerable<TReturn> ToList<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id")
        {
            string sql = GetSqlString();
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var item in Parameters)
            {
                dynamicParameters.Add(item.Value.ParameterName, item.Value.ParameterValue);
            }
            DbConnObj ConnObj = DBUtils.GetConnObj(Connection, Transaction);
            if (ConnObj.DbConnection != null)
            {
                return Connection.Query<TFirst, TSecond, TThird, TReturn>(sql, map, dynamicParameters, Transaction, true, splitOn, CommandTimeout, CommandType.Text);
            }
            else
            {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType))
                {
                    return conn.Query<TFirst, TSecond, TThird, TReturn>(sql, map, dynamicParameters, Transaction, true, splitOn, CommandTimeout, CommandType.Text);
                }
            }
        }



        /// <summary>
        /// 执行sql 返回总行数    请在 Page方法之前执行
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            string sql = DapperExtension.DapperImplementor.SqlGenerator.PageCount(GetSqlString());
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var item in Parameters)
            {
                dynamicParameters.Add(item.Value.ParameterName, item.Value.ParameterValue);
            }
            DbConnObj ConnObj = DBUtils.GetConnObj(Connection, Transaction);
            if (ConnObj.DbConnection != null)
            {
                return (int)Connection.Query(sql, dynamicParameters, Transaction, false, CommandTimeout, CommandType.Text).Single().Total;
            }
            else
            {
                DataBaseType dbType;
                using (IDbConnection conn = DBUtils.CreateDBConnection(ConnKey, out dbType))
                {
                    return (int)conn.Query(sql, dynamicParameters, Transaction, false, CommandTimeout, CommandType.Text).Single().Total;
                }
            }
        }
        #endregion



        public void Reset()
        {
            PageIndex = null;
            PageSize = null;
            DistinctString = string.Empty;
            EnabledNoLock = false;
            _SqlString = string.Empty;
            _WhereClip = WhereClip.All;
            _HavingClip = WhereClip.All;
            _OrderByClip = OrderByClip.None;
            _GroupByClip = GroupByClip.None;
            _Joins = new Dictionary<string, KeyValuePair<string, WhereClip>>();
            _Fields = new List<Field>();
            _Parameters = new Dictionary<string, Parameter>();
        }


        #region Private方法

        /// <summary>
        /// 是否修改sql
        /// </summary>
        private void IsChangeSql()
        {
            //_Parameters.Clear();
            //LoadParameters();
            _SqlString = string.Empty;
        }


        /// <summary>
        /// 重新加载参数
        /// </summary>
        private void LoadParameters()
        {
            if (null == _Parameters || _Parameters.Count == 0)
            {
                if (!WhereClip.IsNullOrEmpty(_WhereClip))
                {
                    foreach (var item in _WhereClip.Parameters)
                    {
                        _Parameters.Add(item.ParameterName, item);
                    }
                }
                //  处理groupby的having
                if (!GroupByClip.IsNullOrEmpty(_GroupByClip) && !WhereClip.IsNullOrEmpty(_HavingClip))
                {
                    foreach (var item in _HavingClip.Parameters)
                    {
                        _Parameters.Add(item.ParameterName, item);
                    }
                }
                if (_Joins != null && _Joins.Count > 0)
                {
                    foreach (var item in _Joins)
                    {
                        foreach (var p in item.Value.Value.Parameters)
                        {
                            _Parameters.Add(p.ParameterName, p);
                        }
                    }
                }
            }
        }


        private void ReLoadParameters()
        {
            _Parameters.Clear();
            LoadParameters();
        }

        #endregion
    }
}
