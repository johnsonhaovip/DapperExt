using DapperExtensions.Lambda;
using DapperExtensions.Sql;
using DapperExtensions.ValueObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DapperExtensions.Mapper
{
    /// <summary>
    /// Maps an entity property to its corresponding column in the database.
    /// </summary>
    public interface IPropertyMap
    {
        string DbName { get; }

        string SchemaName { get; }

        string TableName { get; }


        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 字段名称
        /// </summary>
        string ColumnName { get; }

        /// <summary>
        /// 是否忽略
        /// </summary>
        bool Ignored { get; }

        /// <summary>
        /// 是否只读
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// 字段类型
        /// </summary>
        KeyType KeyType { get; }

        /// <summary>
        /// 信息
        /// </summary>
        PropertyInfo PropertyInfo { get; }

    }

    /// <summary>
    /// Maps an entity property to its corresponding column in the database.
    /// </summary>
    [Serializable]
    public class PropertyMap : IPropertyMap
    {
        public PropertyMap() { }

        public PropertyMap(IPropertyMap pm)
        {
            this.PropertyInfo = pm.PropertyInfo;
            this.Name = PropertyInfo.Name;
            this.TableName = pm.TableName;
            this.SchemaName = pm.SchemaName;
            this.ColumnName = pm.ColumnName;
            this.Ignored = pm.Ignored;
            this.IsReadOnly = pm.IsReadOnly;
            this.DbName = pm.DbName;
        }

        public PropertyMap(string key, Type entityType)
        {
            IClassMapper classMapper = DapperExtension.DapperImplementor.SqlGenerator.Configuration.GetMap(entityType);
            if (classMapper.Properties.ContainsKey(key))
            {
                IPropertyMap pm = classMapper.Properties[key];
                this.PropertyInfo = pm.PropertyInfo;
                this.Name = PropertyInfo.Name;
                this.TableName = pm.TableName;
                this.SchemaName = pm.SchemaName;
                this.ColumnName = pm.ColumnName;
                this.Ignored = pm.Ignored;
                this.IsReadOnly = pm.IsReadOnly;
                this.DbName = pm.DbName;

            }
        }


        public PropertyMap(string name, string tableName = "", string dbName = "", string schemaName = "", string columnName = "", bool? ignored = null, bool? isReadOnly = null)
        {
            this.Name = name;
            this.TableName = tableName;
            this.DbName = dbName;
            this.SchemaName = schemaName;
            if (string.IsNullOrEmpty(columnName))
            {
                this.ColumnName = name;
            }
            else
            {
                this.ColumnName = columnName;
            }
            if (null != ignored)
            {
                this.Ignored = Convert.ToBoolean(ignored);
            }
            if (null != isReadOnly)
            {
                this.IsReadOnly = Convert.ToBoolean(isReadOnly);
            }
        }


        public PropertyMap(PropertyInfo propertyInfo, string tableName, string dbName = "", string schemaName = "", string columnName = "", bool? ignored = null, bool? isReadOnly = null)
        {
            this.PropertyInfo = propertyInfo;
            this.Name = PropertyInfo.Name;
            this.TableName = tableName;
            this.DbName = dbName;
            this.SchemaName = schemaName;
            if (string.IsNullOrEmpty(columnName))
            {
                this.ColumnName = PropertyInfo.Name;
            }
            else
            {
                this.ColumnName = columnName;
            }
            if (null != ignored)
            {
                this.Ignored = Convert.ToBoolean(ignored);
            }
            if (null != isReadOnly)
            {
                this.IsReadOnly = Convert.ToBoolean(isReadOnly);
            }
        }

        public string DbName { get; protected set; }

        public string SchemaName { get; protected set; }

        public string TableName { get; protected set; }



        /// <summary>
        /// Gets the name of the property by using the specified propertyInfo.
        /// </summary>
        public string Name { get; protected set; }


        public string ColumnName { get; protected set; }

        /// <summary>
        /// Gets the key type for the current property.
        /// </summary>
        public KeyType KeyType { get; protected set; }

        public bool Ignored { get; protected set; }

        public bool IsReadOnly { get; protected set; }

        public PropertyInfo PropertyInfo { get; protected set; }

        public PropertyMap Column(string columnName)
        {
            ColumnName = columnName;
            return this;
        }

        /// <summary>
        /// Fluently sets the key type of the property.
        /// </summary>
        /// <param name="columnName">The column name as it exists in the database.</param>
        public PropertyMap Key(KeyType keyType)
        {
            if (Ignored)
            {
                throw new ArgumentException(string.Format("'{0}' is ignored and cannot be made a key field. ", Name));
            }
            KeyType = keyType;
            return this;
        }

        /// <summary>
        /// Fluently sets the ignore status of the property.
        /// </summary>
        public PropertyMap Ignore()
        {
            if (KeyType != KeyType.NotAKey)
            {
                throw new ArgumentException(string.Format("'{0}' is a key field and cannot be ignored.", Name));
            }
            Ignored = true;
            return this;
        }


        public PropertyMap ReadOnly()
        {
            if (KeyType != KeyType.NotAKey)
            {
                throw new ArgumentException(string.Format("'{0}' is a key field and cannot be marked readonly.", Name));
            }
            IsReadOnly = true;
            return this;
        }

    }

    /// <summary>
    /// Used by ClassMapper to determine which entity property represents the key.
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// The property is not a key and is not automatically managed.
        /// </summary>
        NotAKey,

        /// <summary>
        /// The property is an integery-based identity generated from the database.
        /// </summary>
        Identity,

        /// <summary>
        /// The property is a Guid identity which is automatically managed.
        /// </summary>
        Guid,

        /// <summary>
        /// The property is a key that is not automatically managed.
        /// </summary>
        Assigned
    }
}