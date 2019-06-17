using DapperExtensions;
using DapperExtensions.Lambda;
using DapperExtensions.ValueObject;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;


namespace XNY.DataAccess.Utils
{

    /// <summary>
    /// 帮助类
    /// </summary>
    public class DataUtils
    {
        /// <summary>
        /// 格式化sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="leftToken"></param>
        /// <param name="rightToken"></param>
        /// <returns></returns>
        internal static string FormatSQL(string sql, char leftToken, char rightToken)
        {
            if (sql == null)
            {
                return string.Empty;
            }

            sql = sql.Replace("{0}", leftToken.ToString()).Replace("{1}", rightToken.ToString());

            return sql;
        }

        /// <summary>
        /// 格式化数据为数据库通用格式
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string FormatValue(object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return "null";
            }
            Type type = val.GetType();
            if (type == typeof(DateTime) || type == typeof(Guid))
            {
                return string.Format("'{0}'", val);
            }
            else if (type == typeof(TimeSpan))
            {
                DateTime baseTime = new DateTime(1900, 01, 01);
                return string.Format("(cast('{0}' as datetime) - cast('{1}' as datetime))", baseTime + ((TimeSpan)val), baseTime);
            }
            else if (type == typeof(bool))
            {
                return ((bool)val) ? "1" : "0";
            }
            else if (val is Field)
            {
                return ((Field)val).TableFieldName;
            }
            else if (type.IsEnum)
            {
                return Convert.ToInt32(val).ToString();
            }
            else if (type.IsValueType)
            {
                return val.ToString();
            }
            else
            {
                return string.Concat("N'", val.ToString().Replace("'", "''"), "'");
            }
        }

        /// <summary>
        /// CheckStuct
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool CheckStruct(Type type)
        {
            return ((type.IsValueType && !type.IsEnum) && (!type.IsPrimitive && !type.IsSerializable));
        }


        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ConvertValue(Type type, object value)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return null;
            }
            if (CheckStruct(type))
            {
                string data = value.ToString();
                return SerializationManager.Deserialize(type, data);
            }
            Type type2 = value.GetType();
            if (type == type2)
            {
                return value;
            }
            if (((type == typeof(Guid)) || (type == typeof(Guid?))) && (type2 == typeof(string)))
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return null;
                }
                return new Guid(value.ToString());
            }
            if (((type == typeof(DateTime)) || (type == typeof(DateTime?))) && (type2 == typeof(string)))
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return null;
                }
                return Convert.ToDateTime(value);
            }
            if (type.IsEnum)
            {
                try
                {
                    return Enum.Parse(type, value.ToString(), true);
                }
                catch
                {
                    return Enum.ToObject(type, value);
                }
            }
            if (((type == typeof(bool)) || (type == typeof(bool?))))
            {
                bool tempbool = false;
                if (bool.TryParse(value.ToString(), out tempbool))
                {
                    return tempbool;
                }
                else
                {
                    //处理  Request.Form  的 checkbox  如果没有返回值就是没有选中false  
                    if (string.IsNullOrEmpty(value.ToString()))
                        return false;
                    else
                    {
                        if (value.ToString() == "0")
                        {
                            return false;
                        }
                        return true;
                    }
                }

            }

            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }

            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TResult ConvertValue<TResult>(object value)
        {
            if (Convert.IsDBNull(value) || value == null)
                return default(TResult);

            object obj = ConvertValue(typeof(TResult), value);
            if (obj == null)
            {
                return default(TResult);
            }
            return (TResult)obj;
        }

        private static System.Text.RegularExpressions.Regex keyReg = new System.Text.RegularExpressions.Regex("[^a-zA-Z]", System.Text.RegularExpressions.RegexOptions.Compiled);
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static byte[] bt = new byte[5];
        private static Random rd = new Random();
        private static object obj = new object();
        private static readonly ThreadLocal<Random> appRandom = new ThreadLocal<Random>(() => new Random());


        [ThreadStatic]
        public static int paramCount = 0;
        public static int GetNewParamCount()
        {
            if (paramCount >= 99999)
            {
                paramCount = 0;
            }
            paramCount++;
            return paramCount;
        }

        /// <summary>
        /// 生成唯一字符串
        /// </summary>
        /// <returns></returns>
        public static string MakeUniqueKey(Field field)//string prefix,
        {
            return string.Concat(DapperExtension.DapperImplementor.SqlGenerator.Configuration.Dialect.ParameterPrefix, field.Name, "_", GetNewParamCount());
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static string ToString(QueryOperator op)
        {
            switch (op)
            {
                case QueryOperator.Add:
                    return " + ";
                case QueryOperator.BitwiseAND:
                    return " & ";
                case QueryOperator.BitwiseNOT:
                    return " ~ ";
                case QueryOperator.BitwiseOR:
                    return " | ";
                case QueryOperator.BitwiseXOR:
                    return " ^ ";
                case QueryOperator.Divide:
                    return " / ";
                case QueryOperator.Equal:
                    return " = ";
                case QueryOperator.Greater:
                    return " > ";
                case QueryOperator.GreaterOrEqual:
                    return " >= ";
                case QueryOperator.IsNULL:
                    return " IS NULL ";
                case QueryOperator.IsNotNULL:
                    return " IS NOT NULL ";
                case QueryOperator.Less:
                    return " < ";
                case QueryOperator.LessOrEqual:
                    return " <= ";
                case QueryOperator.Like:
                    return " LIKE ";
                case QueryOperator.Modulo:
                    return " % ";
                case QueryOperator.Multiply:
                    return " * ";
                case QueryOperator.NotEqual:
                    return " <> ";
                case QueryOperator.Subtract:
                    return " - ";
            }

            throw new NotSupportedException("Unknown QueryOperator: " + op.ToString() + "!");
        }

    }
}
