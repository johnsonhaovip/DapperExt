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


    public class LambdaUpdateHelper<T> : LambdaUpdateHelper where T : class
    {

        public LambdaUpdateHelper(IDbConnection connection, IDbTransaction transaction, IClassMapper classMap, int? commandTimeout = null)
            : base(connection, transaction, classMap, commandTimeout)
        {
        }

        public LambdaUpdateHelper(string connKey, IClassMapper classMap, int? commandTimeout = null)
            : base(connKey, classMap, commandTimeout)
        {
        }

        #region Sql组装  Set   Where ……

        public LambdaUpdateHelper<T> Set(IWhere set)
        {
            return (LambdaUpdateHelper<T>)base.Set(set.ToWhereClip());
        }

        public LambdaUpdateHelper<T> Set(IWhere<T> set)
        {
            return (LambdaUpdateHelper<T>)base.Set(set.ToWhereClip());
        }

        /// <summary>
        ///  
        /// </summary>
        public LambdaUpdateHelper<T> Set(Expression<Func<T, bool>> lambdaSet)
        {
            return (LambdaUpdateHelper<T>)Set(ExpressionToClip<T>.ToWhereClip(lambdaSet));
        }



        public LambdaUpdateHelper<T> Where(IWhere where)
        {
            return (LambdaUpdateHelper<T>)base.Where(where.ToWhereClip());
        }

        public LambdaUpdateHelper<T> Where(IWhere<T> where)
        {
            return (LambdaUpdateHelper<T>)base.Where(where.ToWhereClip());
        }

        /// <summary>
        /// 
        /// </summary>
        public LambdaUpdateHelper<T> Where(Expression<Func<T, bool>> lambdaWhere)
        {
            return (LambdaUpdateHelper<T>)Where(ExpressionToClip<T>.ToWhereClip(lambdaWhere));
        }




        #endregion


        #region Execute

        #endregion
    }


    public class LambdaUpdateHelper
    {
        #region 属性 变量
        public IClassMapper ClassMap { get; private set; }
        public IDbConnection Connection { get; private set; }
        public string ConnKey { get; private set; }
        public IDbTransaction Transaction { get; private set; }
        public DataBaseType DbType { get; private set; }

        [DefaultValue(30000)]
        public int CommandTimeout { get; private set; }


        [DefaultValue(false)]
        public bool EnabledNoLock { get; private set; }

        private string _SqlString = string.Empty;
        private WhereClip _WhereClip = WhereClip.All;
        private WhereClip _SetClip = WhereClip.All;



        private Dictionary<string, KeyValuePair<string, WhereClip>> _Joins = new Dictionary<string, KeyValuePair<string, WhereClip>>();
        private Dictionary<string, Parameter> _Parameters = new Dictionary<string, Parameter>();

        public WhereClip SetClip
        {
            get { return _SetClip; }
            private set { _SetClip = value; }
        }

        public WhereClip WhereClip
        {
            get { return _WhereClip; }
            private set { _WhereClip = value; }
        }



        public Dictionary<string, KeyValuePair<string, WhereClip>> Joins
        {
            get { return _Joins; }
            private set { _Joins = value; }
        }




        public Dictionary<string, Parameter> Parameters
        {
            get
            {
                if (null == _Parameters || _Parameters.Count == 0)
                {
                    if (!WhereClip.IsNullOrEmpty(_SetClip))
                    {
                        foreach (var item in _SetClip.Parameters)
                        {
                            _Parameters.Add(item.ParameterName, item);
                        }
                    }
                    if (!WhereClip.IsNullOrEmpty(_WhereClip))
                    {
                        foreach (var item in _WhereClip.Parameters)
                        {
                            _Parameters.Add(item.ParameterName, item);
                        }
                    }
                }
                return _Parameters;
            }
            internal set
            {
                this._Parameters = value;
            }
        }


        #endregion

        #region 构造函数
        public LambdaUpdateHelper(IDbConnection connection, IDbTransaction transaction, IClassMapper classMap, int? commandTimeout = null)
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

        public LambdaUpdateHelper(string connKey, IClassMapper classMap, int? commandTimeout = null)
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


        protected LambdaUpdateHelper Where(WhereClip where)
        {
            IsChangeSql();
            this._WhereClip = where;
            return this;
        }


        protected LambdaUpdateHelper Set(WhereClip set)
        {
            IsChangeSql();
            this._SetClip = set;
            return this;
        }


        #endregion


        /// <summary>
        /// 执行的sql语句
        /// </summary>
        public string GetSqlString()
        {
            if (string.IsNullOrEmpty(_SqlString))
            {
                ReLoadParameters();
                _SqlString = DapperExtension.DapperImplementor.SqlGenerator.LambdaUpdate(this, ref _Parameters);
            }
            return _SqlString;

        }

        #region Execute
        public int Execute()
        {
            if (Transaction != null)
            {
                return DBUtils.GetDBHelper(DbType).ExecuteNonQuery(Transaction, this.GetSqlString(), DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
            else
            {
                return DBUtils.GetDBHelper(DbType).ExecuteNonQuery(Connection, this.GetSqlString(), DBUtils.ConvertToDbParameter(Parameters, DbType));
            }
        }
        #endregion


        #region Private方法

        /// <summary>
        /// 是否修改sql
        /// </summary>
        private void IsChangeSql()
        {
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
