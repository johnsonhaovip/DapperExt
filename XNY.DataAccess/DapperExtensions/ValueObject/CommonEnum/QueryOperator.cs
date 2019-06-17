using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.ValueObject
{

    /// <summary>
    /// 比较类型
    /// </summary>
    public enum QueryOperator : byte
    {
        /// <summary>
        /// ==
        /// </summary>
        Equal,

        /// <summary>
        /// &lt;&gt; 、 !=、不等于
        /// </summary>
        NotEqual,

        /// <summary>
        /// >
        /// </summary>
        Greater,

        /// <summary>
        /// &lt; 小于
        /// </summary>
        Less,

        /// <summary>
        /// >=
        /// </summary>
        GreaterOrEqual,

        /// <summary>
        /// &lt;= 小于等于
        /// </summary>
        LessOrEqual,

        /// <summary>
        /// LIKE
        /// </summary>
        Like,

        /// <summary>
        /// &
        /// </summary>
        BitwiseAND,

        /// <summary>
        /// |
        /// </summary>
        BitwiseOR,

        /// <summary>
        /// ^
        /// </summary>
        BitwiseXOR,

        /// <summary>
        /// ~
        /// </summary>
        BitwiseNOT,

        /// <summary>
        /// IS NULL
        /// </summary>
        IsNULL,

        /// <summary>
        /// IS NOT NULL
        /// </summary>
        IsNotNULL,

        /// <summary>
        ///  +
        /// </summary>
        Add,

        /// <summary>
        /// -
        /// </summary>
        Subtract,


        /// <summary>
        /// *
        /// </summary>
        Multiply,

        /// <summary>
        /// /
        /// </summary>
        Divide,

        /// <summary>
        /// %
        /// </summary>
        Modulo,
    }
}
