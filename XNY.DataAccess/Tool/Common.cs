using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XNY.DataAccess.Tool
{
    internal class Common
    {

        public static T ExecuteDataReader<T>(IDataReader reader)
        {
            List<T> list = new List<T>();
            try
            {
                //获取传入的数据类型
                Type modelType = typeof(T);

                //使用与指定参数匹配最高的构造函数，来创建指定类型的实例
                T model = Activator.CreateInstance<T>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    //判断字段值是否为空或不存在的值
                    if (!IsNullOrDBNull(reader[i]))
                    {
                        //匹配字段名
                        PropertyInfo pi = modelType.GetProperty(reader.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (pi != null)
                        {
                            //绑定实体对象中同名的字段  
                            pi.SetValue(model, CheckType(reader[i], pi.PropertyType), null);
                        }
                    }
                }
                list.Add(model);
            }
            catch (Exception)
            {

                throw;
            }
            if (list.Count > 0)
                return list[0];
            return default(T);

        }

        /// <summary>
        /// 判断指定对象是否是有效值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsNullOrDBNull(object obj)
        {
            return (obj == null || (obj is DBNull)) ? true : false;
        }


        /// <summary>
        /// 对可空类型进行判断转换(*要不然会报错)
        /// </summary>
        /// <param name="value">DataReader字段的值</param>
        /// <param name="conversionType">该字段的类型</param>
        /// <returns></returns>
        private static object CheckType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;
                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;            }
            return Convert.ChangeType(value, conversionType);
        }
    }
}
