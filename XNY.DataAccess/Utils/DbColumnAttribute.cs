using System;

namespace XNY.DataAccess.Utils
{
    /// <summary>
    /// 标示该属性是（或不是）数据列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DbColumnAttribute : Attribute
    {

        /// <summary>
        /// 列名（缺省时使用属性名）
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 是否强制忽略该属性
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// 可选数据列（Reader中不存在该列时，保持默认值）
        /// </summary>
        public bool IsOptional { get; set; }

    }

    /// <summary>
    /// 标示该类为数据库的结果集实体
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class DbResultAttribute : Attribute
    {

        /// <summary>
        /// 默认所有属性是数据列
        /// </summary>
        public DbResultAttribute()
            : this(true) { }

        /// <summary>
        /// 默认所有属性是（或不是）数据列
        /// </summary>
        /// <param name="defaultAsDbColumn">默认是数据列</param>
        public DbResultAttribute(bool defaultAsDbColumn)
        {
            DefaultAsDbColumn = defaultAsDbColumn;
        }

        /// <summary>
        /// 默认是数据列
        /// </summary>
        public bool DefaultAsDbColumn { get; private set; }

    }
}
