using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensions.Mapper;
using XNY.DataAccess;
using DapperExtensions.Lambda;

namespace DapperExtensions.Sql {

    public interface ISqlGenerator {
        IDapperExtensionConfiguration Configuration { get; }

        string Select(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters);
        string Select(IClassMapper classMap, string tableName, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters);
        string SelectPaged(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters);
        string SelectPaged(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDictionary<string, object> parameters);
        string SelectSet(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDictionary<string, object> parameters);
        string Count(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);
        string PageCount(string sql);
        string Insert(IClassMapper classMap);
        string Update(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);
        string Delete(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);

        string IdentitySql(IClassMapper classMap);
        string GetTableName(IClassMapper map);
        string GetTableName(string tableName);
        string GetFromTableName(IClassMapper map);
        string GetFromTableName(string schemaName, string tableName, string alias, string dbName = "");

        string GetColumnName(IClassMapper map, IPropertyMap property, bool includeAlias);
        string GetColumnName(IClassMapper map, string propertyName, bool includeAlias);
        bool SupportsMultipleStatements();


        string BuildSelectColumns(IClassMapper classMap);
        string BuildSelectColumns(IClassMapper classMap, List<Field> fields, Dictionary<string, KeyValuePair<string, WhereClip>> joins);

        string BuildFrom(IClassMapper ClassMap, Dictionary<string, KeyValuePair<string, WhereClip>> joins, bool isEnabledNoLock = false);


        //string LambdaInsert(LambdaInsertHelper insertHelper, ref Dictionary<string, Parameter> parameters);

        string LambdaDelete(LambdaDeleteHelper deleteHelper, ref Dictionary<string, Parameter> parameters);

        string LambdaUpdate(LambdaUpdateHelper updateHelper, ref Dictionary<string, Parameter> parameters);

        string LambdaSelect(LambdaQueryHelper selectHelper, ref Dictionary<string, Parameter> parameters);

        string LambdaSelect(LambdaQueryHelper selectHelper, ref Dictionary<string, Parameter> parameters, int? pageIndex, int? pageSize, bool loadOrderby = true);

    }

    public class SqlGeneratorImpl : ISqlGenerator {
        public SqlGeneratorImpl(IDapperExtensionConfiguration configuration, DataBaseType dbType) {
            Configuration = configuration;
            DbType = dbType;
        }

        public DataBaseType DbType { get; private set; }



        public IDapperExtensionConfiguration Configuration { get; private set; }

        public virtual string Select(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters) {
            if (parameters == null) {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sql = new StringBuilder(string.Format("SELECT {0} FROM {1}",
                BuildSelectColumns(classMap),
                GetTableName(classMap)));
            if (DbType == DataBaseType.SqlServer) {
                sql.Append(" WITH (NOLOCK) ");
            }
            if (predicate != null) {
                sql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters, Configuration.Dialect));
            }

            if (sort != null && sort.Any()) {
                sql.Append(" ORDER BY ")
                    .Append(sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings());
            }

            return sql.ToString();
        }

        public virtual string Select(IClassMapper classMap, string tableName, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters) {
            if (parameters == null) {
                throw new ArgumentNullException("Parameters");
            }
            StringBuilder sql = new StringBuilder(string.Format("SELECT {0} FROM {1}",
                BuildSelectColumns(tableName),
                GetTableName(tableName)));
            if (DbType == DataBaseType.SqlServer) {
                sql.Append(" WITH (NOLOCK) ");
            }
            if (predicate != null) {
                sql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters, Configuration.Dialect));
            }
            if (sort != null && sort.Any()) {
                sql.Append(" ORDER BY ")
                    .Append(sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings());
            }
            return sql.ToString();
        }

        public string SelectPaged(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters) {
            string pageSql = Configuration.Dialect.GetPagingSql(sql, page, resultsPerPage, parameters);
            return pageSql;
        }

        public virtual string SelectPaged(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDictionary<string, object> parameters) {
            //if (sort == null || !sort.Any())
            //{
            //    throw new ArgumentNullException("Sort", "Sort cannot be null or empty.");
            //}

            if (parameters == null) {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder innerSql = new StringBuilder(string.Format("SELECT {0} FROM {1}",
                BuildSelectColumns(classMap),
                GetTableName(classMap)));
            if (predicate != null) {
                innerSql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters, Configuration.Dialect));
            }
            if (sort != null && sort.Any()) {
                string orderBy = sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings();
                innerSql.Append(" ORDER BY " + orderBy);
            }

            string sql = Configuration.Dialect.GetPagingSql(innerSql.ToString(), page, resultsPerPage, parameters);
            return sql;
        }

        public virtual string SelectSet(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDictionary<string, object> parameters) {
            if (sort == null || !sort.Any()) {
                throw new ArgumentNullException("Sort", "Sort cannot be null or empty.");
            }

            if (parameters == null) {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder innerSql = new StringBuilder(string.Format("SELECT {0} FROM {1}",
                BuildSelectColumns(classMap),
                GetTableName(classMap)));
            if (predicate != null) {
                innerSql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters, Configuration.Dialect));
            }

            string orderBy = sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings();
            innerSql.Append(" ORDER BY " + orderBy);

            string sql = Configuration.Dialect.GetSetSql(innerSql.ToString(), firstResult, maxResults, parameters);
            return sql;
        }


        public virtual string Count(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters) {
            if (parameters == null) {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sql = new StringBuilder(string.Format("SELECT COUNT(*) {0}Total{1} FROM {2}",
                                Configuration.Dialect.OpenQuote,
                                Configuration.Dialect.CloseQuote,
                                GetTableName(classMap)));
            if (predicate != null) {
                sql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters, Configuration.Dialect));
            }

            return sql.ToString();
        }

        public virtual string PageCount(string sql) {
            if (string.IsNullOrEmpty(sql)) {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sqlCount = new StringBuilder();
            //sqlCount.Append(Configuration.Dialect.BatchSeperator);
            sqlCount.Append(string.Format("SELECT COUNT(*) AS {0}Total{1} FROM ({2}) {0}TempCountData{1}",
                                     Configuration.Dialect.OpenQuote,
                                     Configuration.Dialect.CloseQuote,
                                     sql));


            return sqlCount.ToString();
        }



        public virtual string Insert(IClassMapper classMap) {
            var columns = classMap.Properties.Where(p => !(p.Value.Ignored || p.Value.IsReadOnly || p.Value.KeyType == KeyType.Identity));
            if (!columns.Any()) {
                throw new ArgumentException("No columns were mapped.");
            }

            var columnNames = columns.Select(p => GetColumnName(classMap, p.Value, false));
            var parameters = columns.Select(p => Configuration.Dialect.ParameterPrefix + p.Value.Name);

            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                       GetTableName(classMap),
                                       columnNames.AppendStrings(),
                                       parameters.AppendStrings());

            return sql;
        }

        public virtual string Update(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters) {
            if (predicate == null) {
                throw new ArgumentNullException("Predicate");
            }

            if (parameters == null) {
                throw new ArgumentNullException("Parameters");
            }

            var columns = classMap.Properties.Where(p => !(p.Value.Ignored || p.Value.IsReadOnly || p.Value.KeyType == KeyType.Identity));
            if (!columns.Any()) {
                throw new ArgumentException("No columns were mapped.");
            }

            var setSql =
                columns.Select(
                    p =>
                    string.Format(
                        "{0} = {1}{2}", GetColumnName(classMap, p.Value, false), Configuration.Dialect.ParameterPrefix, p.Value.Name));


            return string.Format("UPDATE {0} SET {1} WHERE {2}",
                GetTableName(classMap),
                setSql.AppendStrings(),
                predicate.GetSql(this, parameters, Configuration.Dialect));
        }

        public virtual string Delete(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters) {
            if (predicate == null) {
                throw new ArgumentNullException("Predicate");
            }

            if (parameters == null) {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sql = new StringBuilder(string.Format("DELETE FROM {0}", GetTableName(classMap)));
            sql.Append(" WHERE ").Append(predicate.GetSql(this, parameters, Configuration.Dialect));
            return sql.ToString();
        }

        public virtual string IdentitySql(IClassMapper classMap) {
            return Configuration.Dialect.GetIdentitySql(GetTableName(classMap));
        }

        public virtual string GetTableName(IClassMapper map) {
            return Configuration.Dialect.GetTableName(map.SchemaName, map.TableName, null);
        }

        public string GetFromTableName(IClassMapper map) {
            return Configuration.Dialect.GetTableName(map.SchemaName, map.TableName, null, map.DbName);
        }


        public virtual string GetTableName(string tableName) {
            return Configuration.Dialect.GetTableName("", tableName, null);
        }

        public virtual string GetFromTableName(string schemaName, string tableName, string alias, string dbName = "") {
            return Configuration.Dialect.GetTableName(schemaName, tableName, alias, dbName);
        }


        public virtual string GetColumnName(IClassMapper map, IPropertyMap property, bool includeAlias) {
            string alias = null;
            if (property.ColumnName != property.Name && includeAlias) {
                alias = property.Name;
            }

            return Configuration.Dialect.GetColumnName(GetTableName(map.TableName), property.ColumnName, alias);
        }

        public virtual string GetColumnName(IClassMapper map, string propertyName, bool includeAlias) {
            IPropertyMap propertyMap = map.Properties.SingleOrDefault(p => p.Value.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)).Value;
            if (propertyMap == null) {
                throw new ArgumentException(string.Format("Could not find '{0}' in Mapping.", propertyName));
            }

            return GetColumnName(map, propertyMap, includeAlias);
        }

        public virtual bool SupportsMultipleStatements() {
            return Configuration.Dialect.SupportsMultipleStatements;
        }

        public virtual string BuildSelectColumns(IClassMapper classMap) {
            var columns = classMap.Properties
                .Where(p => !p.Value.Ignored)
                .Select(p => GetColumnName(classMap, p.Value, true));
            return columns.AppendStrings();
        }
        public virtual string BuildSelectColumns(string tableName) {
            return tableName + ".*";
        }
        public virtual string BuildSelectColumns(IClassMapper classMap, List<Field> fields, Dictionary<string, KeyValuePair<string, WhereClip>> joins) {
            if (fields.Count == 0) {
                if (joins.Count > 0) {
                    return "*";
                } else {
                    return BuildSelectColumns(classMap);
                }
            }
            StringBuilder columns = new StringBuilder();
            foreach (Field filed in fields) {
                columns.Append(",");
                columns.Append(Configuration.Dialect.GetColumnName(GetTableName(filed.TableName), filed.ColumnName, filed.AliasName));
            }
            return columns.ToString().Substring(1);
        }


        public virtual string BuildFrom(IClassMapper ClassMap, Dictionary<string, KeyValuePair<string, WhereClip>> joins, bool isEnabledNoLock = false) {
            StringBuilder fromstring = new StringBuilder();
            fromstring.Append(GetFromTableName(ClassMap));
            if (isEnabledNoLock) {
                fromstring.Append(" WITH (NOLOCK) ");
            }
            foreach (KeyValuePair<string, KeyValuePair<string, WhereClip>> kv in joins) {
                fromstring.Append(" ");
                fromstring.Append(kv.Value.Key);
                fromstring.Append(" ");
                fromstring.Append(kv.Key);
                if (isEnabledNoLock) {
                    fromstring.Append(" WITH (NOLOCK)");
                }
                fromstring.Append(" ON ");
                fromstring.Append(kv.Value.Value.ToString());
            }


            return fromstring.ToString();
        }



        //public virtual string LambdaInsert(LambdaInsertHelper insertHelper, ref Dictionary<string, Parameter> parameters)
        //{
        //    if (insertHelper.Fields.Count == 0)
        //    {
        //        throw new
        //    }

        //    StringBuilder sql = new StringBuilder();
        //    sql.Append("INSERT INTO ");
        //    sql.Append(GetTableName(insertHelper.ClassMap.TableName));

        //    foreach (Field filed in insertHelper.Fields)
        //    {
        //        columns.Append(",");
        //        columns.Append(Configuration.Dialect.GetColumnName(GetTableName(filed.TableName), filed.ColumnName, filed.AliasName));
        //    }

        //    sql.Append(BuildSelectColumns(insertHelper.ClassMap, insertHelper.Fields, insertHelper.Joins));

        //    sql.Append(BuildFrom(insertHelper.ClassMap, insertHelper.Joins, insertHelper.EnabledNoLock));
        //    sql.Append(" ");

        //    if (!WhereClip.IsNullOrEmpty(insertHelper.WhereClip))
        //    {
        //        sql.Append(insertHelper.WhereClip.WhereString);
        //    }
        //    if (!GroupByClip.IsNullOrEmpty(insertHelper.GroupByClip))
        //    {
        //        sql.Append(insertHelper.GroupByClip.GroupByString);
        //        if (!WhereClip.IsNullOrEmpty(insertHelper.HavingClip))
        //        {
        //            sql.Append(" HAVING ");
        //            sql.Append(insertHelper.HavingClip.ToString());
        //        }
        //    }
        //    if (!OrderByClip.IsNullOrEmpty(insertHelper.OrderByClip))
        //    {
        //        sql.Append(insertHelper.OrderByClip.OrderByString);
        //        sql.Append(" ");
        //    }

        //    if (insertHelper.PageIndex > 0 && insertHelper.PageSize > 0 && string.IsNullOrEmpty(topSql))
        //    {
        //        Dictionary<string, object> pageParameters = new Dictionary<string, object>();
        //        string pageSql = this.Configuration.Dialect.GetPagingSql(sql.ToString(), Convert.ToInt32(insertHelper.PageIndex), Convert.ToInt32(insertHelper.PageSize), pageParameters);
        //        foreach (var item in pageParameters)
        //        {
        //            if (!parameters.ContainsKey(item.Key))
        //            {
        //                parameters.Add(item.Key, new Parameter(item.Key, item.Value));
        //            }
        //        }
        //        return pageSql;
        //    }
        //    return sql.ToString();

        //}

        public virtual string LambdaDelete(LambdaDeleteHelper deleteHelper, ref Dictionary<string, Parameter> parameters) {
            StringBuilder sql = new StringBuilder();
            sql.Append("DELETE FROM  ");
            sql.Append(GetTableName(deleteHelper.ClassMap.TableName));
            if (!WhereClip.IsNullOrEmpty(deleteHelper.WhereClip)) {
                sql.Append(deleteHelper.WhereClip.WhereString);
            }
            return sql.ToString();
        }

        public virtual string LambdaUpdate(LambdaUpdateHelper updateHelper, ref Dictionary<string, Parameter> parameters) {
            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE ");
            sql.Append(GetTableName(updateHelper.ClassMap.TableName));

            if (!WhereClip.IsNullOrEmpty(updateHelper.SetClip)) {
                sql.Append(updateHelper.SetClip.SetString);
            } else {
                throw new Exception("Update 语句中未找到 Set 的字段信息");
            }

            if (!WhereClip.IsNullOrEmpty(updateHelper.WhereClip)) {
                sql.Append(updateHelper.WhereClip.WhereString);
            }
            return sql.ToString();
        }


        public virtual string LambdaSelect(LambdaQueryHelper selectHelper, ref Dictionary<string, Parameter> parameters) {
            string topSql = this.Configuration.Dialect.GetTopString(Convert.ToInt32(selectHelper.PageIndex), Convert.ToInt32(selectHelper.PageSize));
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            if (!string.IsNullOrEmpty(selectHelper.DistinctString)) {
                sql.Append(selectHelper.DistinctString);
                sql.Append(" ");
            }
            sql.Append(topSql);
            sql.Append(BuildSelectColumns(selectHelper.ClassMap, selectHelper.Fields, selectHelper.Joins));
            sql.Append(" FROM ");
            sql.Append(BuildFrom(selectHelper.ClassMap, selectHelper.Joins, selectHelper.EnabledNoLock));
            sql.Append(" ");

            if (!WhereClip.IsNullOrEmpty(selectHelper.WhereClip)) {
                sql.Append(selectHelper.WhereClip.WhereString);
            }
            if (!GroupByClip.IsNullOrEmpty(selectHelper.GroupByClip)) {
                sql.Append(selectHelper.GroupByClip.GroupByString);
                if (!WhereClip.IsNullOrEmpty(selectHelper.HavingClip)) {
                    sql.Append(" HAVING ");
                    sql.Append(selectHelper.HavingClip.ToString());
                }
            }
            if (!OrderByClip.IsNullOrEmpty(selectHelper.OrderByClip)) {
                sql.Append(selectHelper.OrderByClip.OrderByString);
                sql.Append(" ");
            }

            if (selectHelper.PageIndex > 0 && selectHelper.PageSize > 0 && string.IsNullOrEmpty(topSql)) {
                Dictionary<string, object> pageParameters = new Dictionary<string, object>();
                string pageSql = this.Configuration.Dialect.GetPagingSql(sql.ToString(), Convert.ToInt32(selectHelper.PageIndex), Convert.ToInt32(selectHelper.PageSize), pageParameters);
                foreach (var item in pageParameters) {
                    if (!parameters.ContainsKey(item.Key)) {
                        parameters.Add(item.Key, new Parameter(item.Key, item.Value));
                    }
                }
                return pageSql;
            }
            return sql.ToString();

        }



        public virtual string LambdaSelect(LambdaQueryHelper selectHelper, ref Dictionary<string, Parameter> parameters, int? pageIndex, int? pageSize, bool loadOrderby = true) {
            {
                string topSql = this.Configuration.Dialect.GetTopString(Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT ");
                if (!string.IsNullOrEmpty(selectHelper.DistinctString)) {
                    sql.Append(selectHelper.DistinctString);
                    sql.Append(" ");
                }
                sql.Append(topSql);
                sql.Append(BuildSelectColumns(selectHelper.ClassMap, selectHelper.Fields, selectHelper.Joins));
                sql.Append(" FROM ");
                sql.Append(BuildFrom(selectHelper.ClassMap, selectHelper.Joins, selectHelper.EnabledNoLock));
                sql.Append(" ");

                if (!WhereClip.IsNullOrEmpty(selectHelper.WhereClip)) {
                    sql.Append(selectHelper.WhereClip.WhereString);
                }
                if (!GroupByClip.IsNullOrEmpty(selectHelper.GroupByClip)) {
                    sql.Append(selectHelper.GroupByClip.GroupByString);
                    if (!WhereClip.IsNullOrEmpty(selectHelper.HavingClip)) {
                        sql.Append(" HAVING ");
                        sql.Append(selectHelper.HavingClip.ToString());
                    }
                }
                if (loadOrderby && !OrderByClip.IsNullOrEmpty(selectHelper.OrderByClip)) {
                    sql.Append(selectHelper.OrderByClip.OrderByString);
                    sql.Append(" ");
                }

                if (pageIndex != null && pageSize != null && string.IsNullOrEmpty(topSql)) {
                    Dictionary<string, object> pageParameters = new Dictionary<string, object>();
                    string pageSql = this.Configuration.Dialect.GetPagingSql(sql.ToString(), Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize), pageParameters);
                    foreach (var item in pageParameters) {
                        if (!parameters.ContainsKey(item.Key)) {
                            parameters.Add(item.Key, new Parameter(item.Key, item.Value));
                        }
                    }
                    return pageSql;
                }
                return sql.ToString();

            }

        }

    }
}