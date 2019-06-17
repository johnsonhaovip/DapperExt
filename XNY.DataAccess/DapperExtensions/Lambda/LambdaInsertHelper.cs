//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Data.Common;
//using System.Linq;
//using System.Linq.Expressions;

//using Dapper;
//using DapperExtensions;
//using DapperExtensions.Mapper;
//using DapperExtensions.Sql;
//using DapperExtensions.ValueObject;
//using HY.DataAccess;

//namespace DapperExtensions.Lambda
//{


//    public class LambdaInsertHelper<T> : LambdaInsertHelper where T : class
//    {

//        public LambdaInsertHelper(IDbConnection connection, IDbTransaction transaction, IClassMapper classMap, int? commandTimeout = null)
//            : base(connection, transaction, classMap, commandTimeout)
//        {
//        }

//        #region Sql组装  Insert





//        public LambdaInsertHelper<T> Insert(IWhere where)
//        {
//            return (LambdaInsertHelper<T>)base.Where(where.ToWhereClip());
//        }

//        public LambdaInsertHelper<T> Insert(IWhere<T> where)
//        {
//            return (LambdaInsertHelper<T>)base.Where(where.ToWhereClip());
//        }

//        /// <summary>
//        ///  
//        /// </summary>
//        public LambdaInsertHelper<T> Insert(Expression<Func<T, bool>> lambdaWhere)
//        {
//            return (LambdaInsertHelper<T>)Where(ExpressionToClip<T>.ToWhereClip(lambdaWhere));
//        }


//        #endregion




//    }


//    public class LambdaInsertHelper
//    {
//        #region 属性 变量
//        public IClassMapper ClassMap { get; private set; }
//        public IDbConnection Connection { get; private set; }
//        public IDbTransaction Transaction { get; private set; }
//        public DataBaseType DbType { get; private set; }

//        [DefaultValue(30000)]
//        public int CommandTimeout { get; private set; }


//        private string _SqlString = string.Empty;
//        private WhereClip _InsertClip = WhereClip.All;

//        private Dictionary<string, KeyValuePair<string, WhereClip>> _Joins = new Dictionary<string, KeyValuePair<string, WhereClip>>();
//        private List<Field> _Fields = new List<Field>();

//        public List<Field> Fields
//        {
//            get { return _Fields; }
//            set { _Fields = value; }
//        }
//        private Dictionary<string, Parameter> _Parameters = new Dictionary<string, Parameter>();

//        public WhereClip InsertClip
//        {
//            get { return _InsertClip; }
//            set { _InsertClip = value; }
//        }

//        public Dictionary<string, Parameter> Parameters
//        {
//            get
//            {
//                if (null == _Parameters || _Parameters.Count == 0)
//                {
//                    if (!WhereClip.IsNullOrEmpty(_InsertClip))
//                    {
//                        foreach (var item in _InsertClip.Parameters)
//                        {
//                            _Parameters.Add(item.ParameterName, item);
//                        }
//                    }
//                }
//                return _Parameters;
//            }
//            internal set
//            {
//                this._Parameters = value;
//            }


//        }


//        #endregion

//        #region 构造函数
//        public LambdaInsertHelper(IDbConnection connection, IDbTransaction transaction, IClassMapper classMap, int? commandTimeout = null)
//        {
//            this.Connection = connection;
//            this.DbType = DapperExtension.DapperImplementor.DbType;
//            this.Transaction = transaction;
//            this.ClassMap = classMap;
//            if (null != commandTimeout)
//            {
//                this.CommandTimeout = CommandTimeout;
//            }
//        }
//        #endregion

//        #region Sql组装  Select  OrderBy  Group ……




//        protected LambdaInsertHelper Where(WhereClip where)
//        {
//            IsChangeSql();
//            this._InsertClip = where;
//            return this;
//        }

//        #endregion


//        /// <summary>
//        /// 执行的sql语句
//        /// </summary>
//        public string SqlString
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_SqlString))
//                {

//                    _SqlString = DapperExtension.DapperImplementor.SqlGenerator.LambdaInsert(this, ref _Parameters);
//                }
//                return _SqlString;
//            }
//        }

//        #region Execute

//        public int Execute()
//        {
//            if (Transaction != null)
//            {
//                return DBUtils.GetDBHelper(DbType).ExecuteNonQuery(Transaction, this.SqlString, CommandType.Text, DBUtils.ConvertToDbParameter(Parameters, DbType).ToArray());
//            }
//            else
//            {
//                return DBUtils.GetDBHelper(DbType).ExecuteNonQuery(Connection, this.SqlString, CommandType.Text, DBUtils.ConvertToDbParameter(Parameters, DbType).ToArray());
//            }
//        }
//        #endregion


//        #region Private方法

//        private void IsChangeSql()
//        {
//            _Parameters.Clear();
//            _SqlString = string.Empty;
//        }

//        #endregion
//    }
//}
