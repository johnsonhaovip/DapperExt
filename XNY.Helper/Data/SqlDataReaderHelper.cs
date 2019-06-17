/****************************************************************
 * 项目名称：XNY.Helper.Data
 * 类 名 称：SqlDataReaderHelper
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.Data
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/7/12 15:49:33
 * 更新时间：2018/7/12 15:49:33
 * 
 * 描述说明：
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Johnson 2018 All rights reserved
*****************************************************************/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Data {

    /// <summary>
    /// SqlDataReader帮助类
    /// </summary>
    public static class SqlDataReaderHelper {

        /// <summary>
        /// 利用反射将SqlDataReader转换成List模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IList<T> ExecuteQueryList<T>(SqlDataReader reader) where T : new() {
            try {
                IList<T> list;
                Type type = typeof(T);
                string tempName = string.Empty;
                if (reader.HasRows) {
                    list = new List<T>();
                    while (reader.Read()) {
                        T t = new T();
                        PropertyInfo[] propertys = t.GetType().GetProperties();
                        foreach (PropertyInfo pi in propertys) {
                            tempName = pi.Name;
                            if (readerExists(reader, tempName)) {
                                if (!pi.CanWrite) {
                                    continue;
                                }
                                var value = reader[tempName];
                                if (value != DBNull.Value) {
                                    pi.SetValue(t, value, null);
                                }
                            }
                        }
                        list.Add(t);
                    }
                    return list;
                }
                return null;
            } catch (Exception ex) {
                return null;
            }
        }


        /// <summary>
        /// 判断SqlDataReader是否存在某列
        /// </summary>
        /// <param name="dr">SqlDataReader</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        private static bool readerExists(SqlDataReader dr, string columnName) {
            dr.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + columnName + "'";
            return (dr.GetSchemaTable().DefaultView.Count > 0);
        }

    }
}
