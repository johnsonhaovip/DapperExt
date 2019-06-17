using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Dapper;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using DapperExtensions.Lambda;
using XNY.DataAccess;

namespace DapperExtensions {
    public interface IDapperImplementor {
        DataBaseType DbType { get; }
        ISqlGenerator SqlGenerator { get; }
        TReturn Get<T, TReturn>(IDbConnection connection, dynamic id, IDbTransaction transaction, int? commandTimeout)
            where T : class
            where TReturn : class;
        void Insert<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class;
        dynamic Insert<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        bool Update<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        int Delete<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        int Delete<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;
        IEnumerable<TReturn> GetList<T, TReturn>(IDbConnection connection, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, string tableName = null)
            where T : class
            where TReturn : class;
        IEnumerable<T> GetPage<T>(IDbConnection connection, int page, int resultsPerPage, out long allRowsCount, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        IEnumerable<TReturn> GetPage<T, TReturn>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout)
            where T : class
            where TReturn : class;
        int Count<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;
        IMultipleResultReader GetMultiple(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout);


        //LambdaInsertHelper<T> LambdaInsert<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class;
        LambdaDeleteHelper<T> LambdaDelete<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class;
        LambdaUpdateHelper<T> LambdaUpdate<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class;
        LambdaQueryHelper<T> LambdaQuery<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class;

        LambdaDeleteHelper<T> LambdaDelete<T>(string connKey, int? commandTimeout) where T : class;
        LambdaUpdateHelper<T> LambdaUpdate<T>(string connKey, int? commandTimeout) where T : class;
        LambdaQueryHelper<T> LambdaQuery<T>(string connKey, int? commandTimeout) where T : class;

    }

    internal class DapperImplementor : IDapperImplementor {
        public DapperImplementor(ISqlGenerator sqlGenerator, DataBaseType dbType) {
            SqlGenerator = sqlGenerator;
            DbType = dbType;
        }

        public DataBaseType DbType { get; private set; }


        public ISqlGenerator SqlGenerator { get; private set; }

        public TReturn Get<T, TReturn>(IDbConnection connection, dynamic id, IDbTransaction transaction, int? commandTimeout)
            where T : class
            where TReturn : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetIdPredicate(classMap, id);
            TReturn result = GetList<TReturn>(connection, classMap, predicate, null, transaction, commandTimeout).SingleOrDefault();
            return result;
        }

        public void Insert<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            var properties = classMap.Properties.Where(p => p.Value.KeyType != KeyType.NotAKey);

            foreach (var e in entities) {
                foreach (var column in properties) {
                    if (column.Value.KeyType == KeyType.Guid && (Guid)column.Value.PropertyInfo.GetValue(e, null) == Guid.Empty) {
                        Guid comb = SqlGenerator.Configuration.GetNextGuid();
                        column.Value.PropertyInfo.SetValue(e, comb, null);
                    }
                }
            }

            string sql = SqlGenerator.Insert(classMap);

            connection.Execute(sql, entities, transaction, commandTimeout, CommandType.Text);
        }

        public dynamic Insert<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IDictionary<string, IPropertyMap> nonIdentityKeyProperties = classMap.Properties.Where(p => p.Value.KeyType == KeyType.Guid || p.Value.KeyType == KeyType.Assigned).ToDictionary(p => p.Key, p => p.Value);
            KeyValuePair<string, IPropertyMap>? identityColumn = classMap.Properties.SingleOrDefault(p => p.Value.KeyType == KeyType.Identity);
            foreach (var column in nonIdentityKeyProperties) {
                if (column.Value.KeyType == KeyType.Guid && (Guid)column.Value.PropertyInfo.GetValue(entity, null) == Guid.Empty) {
                    Guid comb = SqlGenerator.Configuration.GetNextGuid();
                    column.Value.PropertyInfo.SetValue(entity, comb, null);
                }
            }

            IDictionary<string, object> keyValues = new ExpandoObject();
            string sql = SqlGenerator.Insert(classMap);
            if (identityColumn != null) {
                IEnumerable<long> result = null;
                if (SqlGenerator.SupportsMultipleStatements()) {
                    if (nonIdentityKeyProperties != null && nonIdentityKeyProperties.Count > 0 && nonIdentityKeyProperties.FirstOrDefault().Value.KeyType == KeyType.Guid) {
                        sql += SqlGenerator.Configuration.Dialect.BatchSeperator;
                        var line = connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text);
                        return line;
                    } else {
                        sql += SqlGenerator.Configuration.Dialect.BatchSeperator + SqlGenerator.IdentitySql(classMap);
                        result = connection.Query<long>(sql, entity, transaction, false, commandTimeout, CommandType.Text);
                    }
                } else {
                    connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text);
                    sql = SqlGenerator.IdentitySql(classMap);
                    result = connection.Query<long>(sql, entity, transaction, false, commandTimeout, CommandType.Text);
                }

                bool hasResult = false;
                int identityInt = 0;
                foreach (var identityValue in result) {
                    if (hasResult) {
                        continue;
                    }
                    identityInt = Convert.ToInt32(identityValue);
                    hasResult = true;
                }
                if (!hasResult) {
                    throw new InvalidOperationException("The source sequence is empty.");
                }
                keyValues.Add(identityColumn.Value.Value.Name, identityInt);
                identityColumn.Value.Value.PropertyInfo.SetValue(entity, identityInt, null);
            } else {
                connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text);
            }

            foreach (var column in nonIdentityKeyProperties) {
                keyValues.Add(column.Value.Name, column.Value.PropertyInfo.GetValue(entity, null));
            }

            if (keyValues.Count == 1) {
                return keyValues.First().Value;
            }

            return keyValues;
        }

        public bool Update<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate<T>(classMap, entity);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Update(classMap, predicate, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();

            var columns = classMap.Properties.Where(p => !(p.Value.Ignored || p.Value.IsReadOnly || p.Value.KeyType == KeyType.Identity));
            foreach (var property in ReflectionHelper.GetObjectValues(entity).Where(property => columns.Any(c => c.Value.Name == property.Key))) {
                dynamicParameters.Add(property.Key, property.Value);
            }

            foreach (var parameter in parameters) {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        public int Delete<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate<T>(classMap, entity);
            return Delete<T>(connection, classMap, predicate, transaction, commandTimeout);
        }

        public int Delete<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return Delete<T>(connection, classMap, wherePredicate, transaction, commandTimeout);
        }

        public IEnumerable<TReturn> GetList<T, TReturn>(IDbConnection connection, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, string tableName = null)
            where T : class
            where TReturn : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return GetList<TReturn>(connection, classMap, wherePredicate, sort, transaction, commandTimeout, tableName);
        }

        public IEnumerable<TReturn> GetPage<T, TReturn>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout)
            where T : class
            where TReturn : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return GetPage<TReturn>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout);
        }

        public int Count<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Count(classMap, wherePredicate, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters) {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return (int)connection.Query(sql, dynamicParameters, transaction, false, commandTimeout, CommandType.Text).Single().Total;
        }

        public IMultipleResultReader GetMultiple(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout) {
            if (SqlGenerator.SupportsMultipleStatements()) {
                return GetMultipleByBatch(connection, predicate, transaction, commandTimeout);
            }

            return GetMultipleBySequence(connection, predicate, transaction, commandTimeout);
        }

        protected IEnumerable<T> GetList<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, string tableName = null) where T : class {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = "";
            if (string.IsNullOrWhiteSpace(tableName)) {
                sql = SqlGenerator.Select(classMap, predicate, sort, parameters);
            } else {
                sql = SqlGenerator.Select(classMap, tableName, predicate, sort, parameters);
            }
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters) {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }
            return connection.Query<T>(sql, dynamicParameters, transaction, true, commandTimeout, CommandType.Text);
        }


        public IEnumerable<T> GetPage<T>(IDbConnection connection, int page, int resultsPerPage, out long allRowsCount, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class {
            while (sql.Contains("\r\n")) {
                sql = sql.Replace("\r\n", " ");
            }
            while (sql.Contains("  ")) {
                sql = sql.Replace("  ", " ");
            }
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            StringBuilder pageSql = new StringBuilder(SqlGenerator.SelectPaged(sql, page, resultsPerPage, parameters));
            DynamicParameters dynamicParameters = new DynamicParameters();
            if (param != null) {
                dynamicParameters = param as DynamicParameters;
            }
            foreach (var parameter in parameters) {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }
            if (allRowsCountSql != null) {
                while (allRowsCountSql.Contains("\r\n")) {
                    allRowsCountSql = allRowsCountSql.Replace("\r\n", " ");
                }
                while (allRowsCountSql.Contains("  ")) {
                    allRowsCountSql = allRowsCountSql.Replace("  ", " ");
                }
            } else {
                allRowsCountSql = SqlGenerator.PageCount(sql);
            }
            IEnumerable<T> list = connection.Query<T>(pageSql.ToString(), dynamicParameters, transaction, true, commandTimeout, CommandType.Text);

            allRowsCount = (long)connection.Query(allRowsCountSql, dynamicParameters, transaction, false, commandTimeout, CommandType.Text).Single().Total;
            return list;
        }


        protected IEnumerable<T> GetPage<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout) where T : class {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SelectPaged(classMap, predicate, sort, page, resultsPerPage, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters) {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return connection.Query<T>(sql, dynamicParameters, transaction, true, commandTimeout, CommandType.Text);
        }

        protected int Delete<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IDbTransaction transaction, int? commandTimeout) where T : class {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (null == predicate) {
                return 0;
            }
            string sql = SqlGenerator.Delete(classMap, predicate, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters) {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
        }

        protected IPredicate GetPredicate(IClassMapper classMap, object predicate) {
            IPredicate wherePredicate = predicate as IPredicate;
            if (wherePredicate == null && predicate != null) {
                wherePredicate = GetEntityPredicate(classMap, predicate);
            }

            return wherePredicate;
        }

        protected IPredicate GetIdPredicate(IClassMapper classMap, object id) {
            bool isSimpleType = ReflectionHelper.IsSimpleType(id.GetType());
            var keys = classMap.Properties.Where(p => p.Value.KeyType != KeyType.NotAKey);
            IDictionary<string, object> paramValues = null;
            IList<IPredicate> predicates = new List<IPredicate>();
            if (!isSimpleType) {
                paramValues = ReflectionHelper.GetObjectValues(id);
            }

            foreach (var key in keys) {
                object value = id;
                if (!isSimpleType) {
                    value = paramValues[key.Value.Name];
                }

                Type predicateType = typeof(FieldPredicate<>).MakeGenericType(classMap.EntityType);

                IFieldPredicate fieldPredicate = Activator.CreateInstance(predicateType) as IFieldPredicate;
                fieldPredicate.Not = false;
                fieldPredicate.Operator = Operator.Eq;
                fieldPredicate.PropertyName = key.Value.Name;
                fieldPredicate.Value = value;
                predicates.Add(fieldPredicate);
            }

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

        protected IPredicate GetKeyPredicate<T>(IClassMapper classMap, T entity) where T : class {
            if (null == entity) {
                return null;
            }

            var whereFields = classMap.Properties.Where(p => p.Value.KeyType != KeyType.NotAKey);
            if (!whereFields.Any()) {
                throw new ArgumentException("At least one Key column must be defined.");
            }

            IList<IPredicate> predicates = (from field in whereFields
                                            select new FieldPredicate<T> {
                                                Not = false,
                                                Operator = Operator.Eq,
                                                PropertyName = field.Value.Name,
                                                Value = field.Value.PropertyInfo.GetValue(entity, null)
                                            }).Cast<IPredicate>().ToList();

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

        protected IPredicate GetEntityPredicate(IClassMapper classMap, object entity) {
            Type predicateType = typeof(FieldPredicate<>).MakeGenericType(classMap.EntityType);
            IList<IPredicate> predicates = new List<IPredicate>();
            foreach (var kvp in ReflectionHelper.GetObjectValues(entity)) {
                IFieldPredicate fieldPredicate = Activator.CreateInstance(predicateType) as IFieldPredicate;
                fieldPredicate.Not = false;
                fieldPredicate.Operator = Operator.Eq;
                fieldPredicate.PropertyName = kvp.Key;
                fieldPredicate.Value = kvp.Value;
                predicates.Add(fieldPredicate);
            }

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

        protected GridReaderResultReader GetMultipleByBatch(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout) {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();
            foreach (var item in predicate.Items) {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap(item.Type);
                IPredicate itemPredicate = item.Value as IPredicate;
                if (itemPredicate == null && item.Value != null) {
                    itemPredicate = GetPredicate(classMap, item.Value);
                }

                sql.AppendLine(SqlGenerator.Select(classMap, itemPredicate, item.Sort, parameters) + SqlGenerator.Configuration.Dialect.BatchSeperator);
            }

            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters) {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            SqlMapper.GridReader grid = connection.QueryMultiple(sql.ToString(), dynamicParameters, transaction, commandTimeout, CommandType.Text);
            return new GridReaderResultReader(grid);
        }

        protected SequenceReaderResultReader GetMultipleBySequence(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout) {
            IList<SqlMapper.GridReader> items = new List<SqlMapper.GridReader>();
            foreach (var item in predicate.Items) {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                IClassMapper classMap = SqlGenerator.Configuration.GetMap(item.Type);
                IPredicate itemPredicate = item.Value as IPredicate;
                if (itemPredicate == null && item.Value != null) {
                    itemPredicate = GetPredicate(classMap, item.Value);
                }

                string sql = SqlGenerator.Select(classMap, itemPredicate, item.Sort, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters) {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                SqlMapper.GridReader queryResult = connection.QueryMultiple(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
                items.Add(queryResult);
            }

            return new SequenceReaderResultReader(items);
        }


        //public LambdaInsertHelper<T> LambdaInsert<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class
        //{
        //    IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
        //    return new LambdaInsertHelper<T>(connection, transaction, classMap, commandTimeout);
        //}

        public LambdaDeleteHelper<T> LambdaDelete<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            return new LambdaDeleteHelper<T>(connection, transaction, classMap, commandTimeout);
        }
        public LambdaDeleteHelper<T> LambdaDelete<T>(string connKey, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            return new LambdaDeleteHelper<T>(connKey, classMap, commandTimeout);
        }


        public LambdaUpdateHelper<T> LambdaUpdate<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            return new LambdaUpdateHelper<T>(connection, transaction, classMap, commandTimeout);
        }

        public LambdaUpdateHelper<T> LambdaUpdate<T>(string connKey, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            return new LambdaUpdateHelper<T>(connKey, classMap, commandTimeout);
        }

        public LambdaQueryHelper<T> LambdaQuery<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            return new LambdaQueryHelper<T>(connection, transaction, classMap, commandTimeout);
        }

        public LambdaQueryHelper<T> LambdaQuery<T>(string connKey, int? commandTimeout) where T : class {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            return new LambdaQueryHelper<T>(connKey, classMap, commandTimeout);
        }
    }
}
