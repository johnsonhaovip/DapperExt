﻿using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using DapperExtensions.ValueObject;
using XNY.DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.Text;


namespace DapperExtensions.Lambda
{
    [Serializable]
    public class Field : PropertyMap
    {

        public Field() { }

        public Field(string fieldName) : this(fieldName, "", "") { }


        public Field(IPropertyMap pm)
            : base(pm)
        {
        }

        public Field(IPropertyMap pm, string aliasName)
            : base(pm)
        {
            this.AliasName = aliasName;
        }

        public Field(string fieldName, string tableName = "", string aliasName = "")
        {
            this.Name = fieldName;
            this.ColumnName = fieldName;
            if (!string.IsNullOrEmpty(tableName))
            {
                this.TableName = tableName;
            }
            if (!string.IsNullOrEmpty(aliasName))
            {
                this.AliasName = aliasName;
            }
        }


        public Field(string key, Type entityType)
            : base(key, entityType)
        {
        }

        public Field(string key, Type entityType, string aliasName)
            : base(key, entityType)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                this.AliasName = aliasName;
            }
        }


        public static Field GetField(string key, Type entityType, string aliasName)
        {
            IClassMapper classMapper = DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap(entityType);
            if (classMapper.Properties.ContainsKey(key))
            {
                return new Field(classMapper.Properties[key], aliasName);
            }
            return null;
        }

        public static Field GetField<T>(string key, string aliasName = "") where T : class
        {
            IClassMapper classMapper = DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap<T>();
            if (classMapper.Properties.ContainsKey(key))
            {
                return new Field(classMapper.Properties[key], aliasName);
            }
            return null;
        }



        public string TableFieldName
        {
            get
            {
                return DapperExtension.DapperImplementor.SqlGenerator.Configuration.Dialect.GetColumnName(
                         DapperExtension.DapperImplementor.SqlGenerator.GetTableName(TableName),
                        ColumnName, "");
            }
        }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(AliasName))
                    return TableFieldName;

                return string.Concat(TableFieldName, " AS {0}", AliasName, "{1}");
            }
        }


        /// <summary>
        /// AliasName
        /// </summary>
        public string AliasName { get; set; }


        public void SetAliasName(string aliasName)
        {
            this.AliasName = aliasName;
        }




        #region 属性



        /// <summary>
        /// datepart   year
        /// </summary>
        public Field Year
        {
            get
            {
                return new Field(string.Concat("datepart(year,", this.TableFieldName, ")")).As(this);
            }
        }

        /// <summary>
        /// datepart   month
        /// </summary>
        public Field Month
        {
            get
            {
                return new Field(string.Concat("datepart(month,", this.TableFieldName, ")")).As(this);
            }
        }

        /// <summary>
        /// datepart  day
        /// </summary>
        public Field Day
        {
            get
            {
                return new Field(string.Concat("datepart(day,", this.TableFieldName, ")")).As(this);
            }
        }

        /// <summary>
        /// 倒叙
        /// </summary>
        public OrderByClip Desc
        {
            get
            {
                return new OrderByClip(this, OrderByType.DESC);
            }
        }


        /// <summary>
        /// 正序
        /// </summary>
        public OrderByClip Asc
        {
            get
            {
                return new OrderByClip(this, OrderByType.ASC);
            }
        }


        /// <summary>
        /// 分组
        /// </summary>
        public GroupByClip GroupBy
        {
            get
            {
                return new GroupByClip(this);
            }
        }

        #endregion



        #region 方法

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool Equals(Field field)
        {
            if (null == (object)field)
                return false;
            return this.FullName.Equals(field.FullName);
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(Field field)
        {
            if (null == (object)field || string.IsNullOrEmpty(field.ColumnName))
                return true;
            return false;
        }


        /// <summary>
        /// 设置所属表名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        //public Field SetTableName(string tableName)
        //{
        //    return new Field(this.ColumnName, tableName, this.AliasName);
        //}

        /// <summary>
        /// AS
        /// </summary>
        /// <param name="AliasName"></param>
        /// <returns></returns>
        public Field As(string AliasName)
        {
            return new Field(this.ColumnName, this.TableName, AliasName);
        }


        /// <summary>
        /// AS
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public Field As(Field field)
        {
            return As(field.Name);
        }

        /// <summary>
        /// 判断字段是否为Null
        /// </summary>
        /// <param name="field">字段实体</param>
        /// <returns></returns>
        public Field IsNull(Field field)
        {
            return new Field(string.Concat("isnull(", this.TableFieldName, ",", field.TableFieldName, ")")).As(this);
        }

        /// <summary>
        /// 判断字段是否为Null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Field IsNull(object value)
        {
            return new Field(string.Concat("isnull(", this.TableFieldName, ",", DataUtils.FormatValue(value), ")")).As(this);
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <returns></returns>
        public Field Count()
        {
            if (this.ColumnName.Trim().Equals("*"))
                return new Field("count(*)").As("cnt");
            return new Field(string.Concat("count(", this.TableFieldName, ")")).As(this);
        }

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        public Field Distinct()
        {
            if (this.ColumnName.Trim().Equals("*"))
                return this;
            return new Field(string.Concat("distinct ", this.TableFieldName)).As(this);
        }

        /// <summary>
        /// Sum
        /// </summary>
        /// <returns></returns>
        public Field Sum()
        {
            return new Field(string.Concat("sum(", this.TableFieldName, ")")).As(this);
        }

        /// <summary>
        /// Avg
        /// </summary>
        /// <returns></returns>
        public Field Avg()
        {
            return new Field(string.Concat("avg(", this.TableFieldName, ")")).As(this);
        }

        /// <summary>
        /// len
        /// </summary>
        /// <returns></returns>
        public Field Len()
        {
            return new Field(string.Concat("len(", this.TableFieldName, ")")).As(this);
        }


        /// <summary>
        /// ltrim and  rtrim
        /// </summary>
        /// <returns></returns>
        public Field Trim()
        {
            return new Field(string.Concat("ltrim(rtrim(", this.TableFieldName, "))")).As(this);
        }

        /// <summary>
        /// Max
        /// </summary>
        /// <returns></returns>
        public Field Max()
        {
            return new Field(string.Concat("max(", this.TableFieldName, ")")).As(this);
        }

        /// <summary>
        /// Min
        /// </summary>
        /// <returns></returns>
        public Field Min()
        {
            return new Field(string.Concat("min(", this.TableFieldName, ")")).As(this);
        }

        /// <summary>
        /// Left
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Field Left(int length)
        {
            return new Field(string.Concat("left(", this.TableFieldName, ",", length.ToString(), ")")).As(this);
        }


        /// <summary>
        /// Right
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Field Right(int length)
        {
            return new Field(string.Concat("right(", this.TableFieldName, ",", length.ToString(), ")")).As(this);
        }


        /// <summary>
        /// substring
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public Field Substring(int startIndex, int endIndex)
        {
            return new Field(string.Concat("substring(", this.TableFieldName, ",", startIndex.ToString(), ",", endIndex.ToString(), ")")).As(this);
        }


        /// <summary>
        /// charindex
        /// </summary>
        /// <param name="subString"></param>
        /// <returns></returns>
        public Field IndexOf(string subString)
        {
            return new Field(string.Concat("charindex(", this.TableFieldName, ",", DataUtils.FormatValue(subString), ") - 1")).As(this);
        }

        /// <summary>
        /// replace
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public Field Replace(string oldValue, string newValue)
        {
            return new Field(string.Concat("replace(", this.TableFieldName, ",", DataUtils.FormatValue(oldValue), ",", DataUtils.FormatValue(newValue), ")")).As(this);
        }

        /// <summary>
        /// 创建whereclip
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="rightField"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        private static WhereClip createWhereClip(Field leftField, Field rightField, QueryOperator oper)
        {
            if (IsNullOrEmpty(leftField) || IsNullOrEmpty(rightField))
                return null;

            return new WhereClip(leftField.TableFieldName + DataUtils.ToString(oper) + rightField.TableFieldName);
        }

        /// <summary>
        /// 创建Field
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="rightField"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        private static Field createField(Field leftField, Field rightField, QueryOperator oper)
        {
            if (IsNullOrEmpty(leftField) && IsNullOrEmpty(rightField))
                return null;

            if (IsNullOrEmpty(leftField))
                return rightField;

            if (IsNullOrEmpty(rightField))
                return leftField;

            return new Field(leftField.TableFieldName + DataUtils.ToString(oper) + rightField.TableFieldName);
        }


        /// <summary>
        /// &
        /// </summary>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public Field BitwiseAND(Field rightField)
        {
            return createField(this, rightField, QueryOperator.BitwiseAND).As(this);
        }

        /// <summary>
        /// |
        /// </summary>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public Field BitwiseOR(Field rightField)
        {
            return createField(this, rightField, QueryOperator.BitwiseOR).As(this);
        }

        /// <summary>
        /// ^
        /// </summary>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public Field BitwiseXOR(Field rightField)
        {
            return createField(this, rightField, QueryOperator.BitwiseXOR).As(this);
        }

        /// <summary>
        /// ~
        /// </summary>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public Field BitwiseNOT(Field rightField)
        {
            return createField(this, rightField, QueryOperator.BitwiseNOT).As(this);
        }


        private static string formatValue(object value)
        {
            if (null == value)
                return string.Empty;

            return value.ToString();

            //return value.ToString().Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
        }

        /// <summary>
        /// like '%value%' 模糊查询，同Like
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereClip Contain(object value)
        {
            return new WhereClip(this, string.Concat(likeString, value, likeString), QueryOperator.Like);
        }
        /// <summary>
        /// like '%value%' 模糊查询，同  Contain
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereClip Like(object value)
        {
            return Contain(value);
        }
        /// <summary>
        /// BeginWith  
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereClip BeginWith(object value)
        {
            return new WhereClip(this, string.Concat(value, likeString), QueryOperator.Like);
        }

        /// <summary>
        /// EndWith  
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereClip EndWith(object value)
        {
            return new WhereClip(this, string.Concat(likeString, value), QueryOperator.Like);
        }

        /// <summary>
        /// LIKE %
        /// </summary>
        private const string likeString = "%";

        /// <summary>
        /// IN
        /// </summary>
        private const string selectInString = " IN ";

        /// <summary>
        /// NOT IN
        /// </summary>
        private const string selectNotInString = " NOT IN ";

        /// <summary>
        /// 子查询
        /// </summary>
        /// <param name="field"></param>
        /// <param name="join"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private WhereClip selectInOrNotIn<T>(Field field, string join, params T[] values)
        {
            return selectInOrNotIn<T>(field, join, true, values);
        }


        /// <summary>
        /// 子查询
        /// </summary>
        /// <param name="field"></param>
        /// <param name="join"></param>
        /// <param name="isParameter">是否参数化</param>
        /// <param name="values"></param>
        /// <returns></returns>
        private WhereClip selectInOrNotIn<T>(Field field, string join, bool isParameter, params T[] values)
        {
            if (values.Length == 0)
            {
                return new WhereClip("1=2");
                //2015-09-22注释
                //return WhereClip.All;
            }


            StringBuilder whereString = new StringBuilder(field.TableFieldName);
            whereString.Append(join);
            whereString.Append("(");
            List<Parameter> ps = new List<Parameter>();
            StringBuilder inWhere = new StringBuilder();
            var i = 0;
            foreach (T value in values)
            {
                i++;
                string paraName = null;

                if (isParameter)
                {
                    paraName = DataUtils.MakeUniqueKey(field);
                    // paraName = field.tableName + field.Name + i;
                    Parameter p = new Parameter(paraName, value);
                    ps.Add(p);
                }
                else
                {
                    if (value == null)
                        continue;


                    paraName = value.ToString();


                    if (string.IsNullOrEmpty(paraName))
                        continue;

                }


                inWhere.Append(",");
                inWhere.Append(paraName);


            }
            whereString.Append(inWhere.ToString().Substring(1));
            whereString.Append(")");

            return new WhereClip(whereString.ToString(), ps.ToArray());
        }



        /// <summary>
        /// SelectIn  
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip SelectIn(params object[] values)
        {
            return selectInOrNotIn<object>(this, selectInString, values);
        }
        /// <summary>
        /// 同SelectIn。
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip In(params object[] values)
        {
            return SelectIn(values);
        }



        /// <summary>
        /// where field in (value,value,value)。传入Array或List&lt;T>。
        /// </summary>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public WhereClip SelectIn<T>(params T[] values)
        {
            if (typeof(T) == typeof(int))
            {
                return selectInOrNotIn<T>(this, selectInString, false, values);
            }

            return selectInOrNotIn<T>(this, selectInString, values);
        }
        /// <summary>
        /// where field in (value,value,value)。同SelectIn。传入Array或List&lt;T>。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip In<T>(params T[] values)
        {
            return SelectIn(values);
        }
        /// <summary>
        /// where field in (value,value,value)。 传入Array或List&lt;T>。
        /// </summary>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public WhereClip SelectIn<T>(List<T> values)
        {
            return SelectIn(values.ToArray());
        }
        /// <summary>
        /// where field in (value,value,value)。同SelectIn。传入Array或List&lt;T>。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip In<T>(List<T> values)
        {
            return SelectIn(values.ToArray());
        }

        /// <summary>
        /// where field in (value,value,value)。同In.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip SelectNotIn(params object[] values)
        {
            return selectInOrNotIn<object>(this, selectNotInString, values);
        }
        /// <summary>
        /// 同SelectNotIn。
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip NotIn(params object[] values)
        {
            return SelectNotIn(values);
        }

        /// <summary>
        /// SelectNotIn  。传入Array或List&lt;T>。
        /// </summary>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public WhereClip SelectNotIn<T>(params T[] values)
        {

            if (typeof(T) == typeof(int))
            {
                return selectInOrNotIn<T>(this, selectNotInString, false, values);
            }

            return selectInOrNotIn<T>(this, selectNotInString, values);
        }
        /// <summary>
        /// 同SelectNotIn。传入Array或List&lt;T>。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip NotIn<T>(params T[] values)
        {
            return SelectNotIn(values);
        }
        /// <summary>
        /// SelectNotIn。传入Array或List&lt;T>。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip SelectNotIn<T>(List<T> values)
        {
            return SelectNotIn(values.ToArray());
        }
        /// <summary>
        /// 同SelectNotIn。传入Array或List&lt;T>。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip NotIn<T>(List<T> values)
        {
            return SelectNotIn(values.ToArray());
        }
        /// <summary>
        /// SubQueryIn   子查询  
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public WhereClip SubQueryIn(LambdaQueryHelper from)
        {
            return subQuery(this, from, selectInString);
        }

        /// <summary>
        /// SubQueryNotIn   子查询  
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public WhereClip SubQueryNotIn(LambdaQueryHelper from)
        {
            return subQuery(this, from, selectNotInString);
        }

        /// <summary>
        /// 组合 子查询
        /// </summary>
        /// <param name="field"></param>
        /// <param name="from"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        private WhereClip subQuery(Field field, LambdaQueryHelper from, QueryOperator oper)
        {
            return subQuery(field, from, DataUtils.ToString(oper));
        }

        /// <summary>
        /// 组合 子查询    TODO
        /// </summary>
        /// <param name="field"></param>
        /// <param name="from"></param>
        /// <param name="join"></param>
        /// <returns></returns>
        private WhereClip subQuery(Field field, LambdaQueryHelper from, string join)
        {
            if (Field.IsNullOrEmpty(field))
                return null;
            //if (from.DbProvider.dbType == dbType.MySql)
            //{
            //    return new WhereClip(string.Concat(field.TableFieldName, join, "(SELECT * FROM (", from.getPagedFromSection().SqlString, ") AS TEMP" + DataUtils.GetNewParamCount() + ")"), from.Parameters.ToArray());
            //}
            //return new WhereClip(string.Concat(field.TableFieldName, join, "(", from.getPagedFromSection().SqlString, ")"), from.Parameters.ToArray());
            return null;
        }

        /// <summary>
        /// SubQueryEqual   子查询  
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public WhereClip SubQueryEqual(LambdaQueryHelper from)
        {
            return subQuery(this, from, QueryOperator.Equal);
        }

        /// <summary>
        /// SubQueryNotEqual   子查询  
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public WhereClip SubQueryNotEqual(LambdaQueryHelper from)
        {
            return subQuery(this, from, QueryOperator.NotEqual);
        }

        /// <summary>
        /// SubQueryLess   子查询  
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public WhereClip SubQueryLess(LambdaQueryHelper from)
        {
            return subQuery(this, from, QueryOperator.Less);
        }

        /// <summary>
        /// SubQueryLessOrEqual   子查询  
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public WhereClip SubQueryLessOrEqual(LambdaQueryHelper from)
        {
            return subQuery(this, from, QueryOperator.LessOrEqual);
        }

        /// <summary>
        /// SubQueryGreater   子查询  
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public WhereClip SubQueryGreater(LambdaQueryHelper from)
        {
            return subQuery(this, from, QueryOperator.Greater);
        }

        /// <summary>
        /// SubQueryGreaterOrEqual   子查询  
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public WhereClip SubQueryGreaterOrEqual(LambdaQueryHelper from)
        {
            return subQuery(this, from, QueryOperator.GreaterOrEqual);
        }


        /// <summary>
        /// 字段 为null <example>field is null</example>
        /// </summary>
        /// <returns></returns>
        public WhereClip IsNull()
        {
            return new WhereClip(string.Concat(this.TableFieldName, " is null "));
        }
        /// <summary>
        /// 字段 为null <example>field is not null</example>
        /// </summary>
        /// <returns></returns>
        public WhereClip IsNotNull()
        {
            return new WhereClip(string.Concat(this.TableFieldName, " is not null "));
        }

        /// <summary>
        /// Between操作
        /// </summary>
        /// <example>
        /// <![CDATA[ a>=value and a<=value ]]>
        /// </example>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public WhereClip Between(object leftValue, object rightValue)
        {
            return this >= leftValue && this <= rightValue;
        }


        #region 操作符重载

        #region WhereClip

        public static WhereClip operator ==(Field leftField, Field rightField)
        {
            return createWhereClip(leftField, rightField, QueryOperator.Equal);
        }

        public static WhereClip operator !=(Field leftField, Field rightField)
        {
            return createWhereClip(leftField, rightField, QueryOperator.NotEqual);
        }

        public static WhereClip operator >(Field leftField, Field rightField)
        {
            return createWhereClip(leftField, rightField, QueryOperator.Greater);
        }

        public static WhereClip operator >=(Field leftField, Field rightField)
        {
            return createWhereClip(leftField, rightField, QueryOperator.GreaterOrEqual);
        }

        public static WhereClip operator <(Field leftField, Field rightField)
        {
            return createWhereClip(leftField, rightField, QueryOperator.Less);
        }

        public static WhereClip operator <=(Field leftField, Field rightField)
        {
            return createWhereClip(leftField, rightField, QueryOperator.LessOrEqual);
        }


        public static WhereClip operator ==(Field field, object value)
        {
            return new WhereClip(field, value, QueryOperator.Equal);
        }
        public static WhereClip operator ==(object value, Field field)
        {
            return new WhereClip(field, value, QueryOperator.Equal);
        }
        public static WhereClip operator !=(Field field, object value)
        {
            return new WhereClip(field, value, QueryOperator.NotEqual);
        }
        public static WhereClip operator !=(object value, Field field)
        {
            return new WhereClip(field, value, QueryOperator.NotEqual);
        }

        public static WhereClip operator >(Field field, object value)
        {
            return new WhereClip(field, value, QueryOperator.Greater);
        }
        public static WhereClip operator >(object value, Field field)
        {
            return new WhereClip(field, value, QueryOperator.Less);
        }


        public static WhereClip operator >=(Field field, object value)
        {
            return new WhereClip(field, value, QueryOperator.GreaterOrEqual);
        }
        public static WhereClip operator >=(object value, Field field)
        {
            return new WhereClip(field, value, QueryOperator.LessOrEqual);
        }

        public static WhereClip operator <(Field field, object value)
        {
            return new WhereClip(field, value, QueryOperator.Less);
        }
        public static WhereClip operator <(object value, Field field)
        {
            return new WhereClip(field, value, QueryOperator.Greater);
        }

        public static WhereClip operator <=(Field field, object value)
        {
            return new WhereClip(field, value, QueryOperator.LessOrEqual);
        }
        public static WhereClip operator <=(object value, Field field)
        {
            return new WhereClip(field, value, QueryOperator.GreaterOrEqual);
        }


        #endregion


        /// <summary>
        /// +
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Field operator +(Field leftField, Field rightField)
        {
            return createField(leftField, rightField, QueryOperator.Add);
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Field operator -(Field leftField, Field rightField)
        {
            return createField(leftField, rightField, QueryOperator.Subtract);
        }

        /// <summary>
        /// *
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Field operator *(Field leftField, Field rightField)
        {
            return createField(leftField, rightField, QueryOperator.Multiply);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Field operator /(Field leftField, Field rightField)
        {
            return createField(leftField, rightField, QueryOperator.Divide);
        }

        /// <summary>
        /// %
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Field operator %(Field leftField, Field rightField)
        {
            return createField(leftField, rightField, QueryOperator.Modulo);
        }


        /// <summary>
        /// +
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression operator +(Field leftField, object value)
        {
            return new Expression(leftField, value, QueryOperator.Add);
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression operator -(Field leftField, object value)
        {
            return new Expression(leftField, value, QueryOperator.Subtract);
        }


        /// <summary>
        /// *
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression operator *(Field leftField, object value)
        {
            return new Expression(leftField, value, QueryOperator.Multiply);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression operator /(Field leftField, object value)
        {
            return new Expression(leftField, value, QueryOperator.Divide);
        }

        /// <summary>
        /// %
        /// </summary>
        /// <param name="leftField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression operator %(Field leftField, object value)
        {
            return new Expression(leftField, value, QueryOperator.Modulo);
        }


        /// <summary>
        /// +
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Expression operator +(object value, Field rightField)
        {
            return new Expression(rightField, value, QueryOperator.Add, false);
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Expression operator -(object value, Field rightField)
        {
            return new Expression(rightField, value, QueryOperator.Subtract, false);
        }

        /// <summary>
        /// *
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Expression operator *(object value, Field rightField)
        {
            return new Expression(rightField, value, QueryOperator.Multiply, false);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Expression operator /(object value, Field rightField)
        {
            return new Expression(rightField, value, QueryOperator.Divide, false);
        }

        /// <summary>
        /// %
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rightField"></param>
        /// <returns></returns>
        public static Expression operator %(object value, Field rightField)
        {
            return new Expression(rightField, value, QueryOperator.Modulo, false);
        }

        #endregion

        #endregion
    }
}
