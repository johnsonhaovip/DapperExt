using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Data
{
    /// <summary>
    /// DataTable转Model帮助类
    /// </summary>
    public static class DataTableHelper
    {
        /// <summary>
        /// Model转DataTable
        /// </summary>
        public static DataTable ConvertTo<T>(IList<T> list)
        {
            using (DataTable table = CreateTable<T>())
            {
                Type entityType = typeof(T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
                if (list != null)
                {
                    foreach (T item in list)
                    {
                        DataRow row = table.NewRow();
                        foreach (PropertyDescriptor prop in properties)
                            row[prop.Name] = prop.GetValue(item);
                        table.Rows.Add(row);
                    }
                }
                return table;
            }
        }
        /// <summary>
        /// DataRow的List转Model的List
        /// </summary>
        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;
            if (rows != null)
            {
                list = new List<T>();
                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }
            return list;
        }
        /// <summary>
        /// DataTable转Model的List
        /// </summary>
        public static IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
                return null;
            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow row in table.Rows)
                rows.Add(row);
            return ConvertTo<T>(rows);
        }
        /// <summary>
        /// DataRow转Model
        /// </summary>
        public static T CreateItem<T>(DataRow row)
        {
            T obj;
            if (row != null)
            {
                Type entityType = typeof(T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
                obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in row.Table.Columns)
                {
                    string columnName = column.ColumnName;
                    var propitem = properties.Find(columnName, true);
                    if (propitem == null) continue;
                    PropertyInfo prop = obj.GetType().GetProperty(propitem.Name);
                    if (!prop.CanWrite) continue;
                    object value = (row[columnName].GetType() == typeof(DBNull)) ? null : row[columnName];
                    if (value == null) continue;

                    if (prop.PropertyType.Equals(typeof(Boolean)))
                    {
                        prop.SetValue(obj, (value ?? "0").ToString().Equals("1"));
                    }
                    else if (prop.PropertyType.IsGenericType && Type.Equals(prop.PropertyType.GetGenericTypeDefinition(), typeof(Nullable<>)))
                    {
                        prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType.GetGenericArguments()[0], CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType, CultureInfo.InvariantCulture));
                    }
                }
            }
            else
            {
                obj = default(T);
            }
            return obj;
        }
        /// <summary>
        /// 根据Model创建一个空的DataTable
        /// </summary>
        private static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            using (DataTable table = new DataTable(entityType.Name))
            {
                table.Locale = CultureInfo.InvariantCulture;
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
                foreach (PropertyDescriptor prop in properties)
                    table.Columns.Add(prop.Name, prop.PropertyType);
                return table;
            }
        }

        /// <summary>
        /// 转化为DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varlist"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        /// 创建时间：2018-5-29
        public static DataTable ToDataTable<T>(this IEnumerable<T> varlist, CreateRowDelegate<T> fn) {
            DataTable dtReturn = new DataTable();
            // column names
            PropertyInfo[] oProps = null;
            // Could add a check to verify that there is an element 0
            foreach (T rec in varlist) {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null) {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps) {
                        Type colType = pi.PropertyType; if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>))) {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow(); foreach (PropertyInfo pi in oProps) {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return (dtReturn);
        }
        public delegate object[] CreateRowDelegate<T>(T t);
    }
}
