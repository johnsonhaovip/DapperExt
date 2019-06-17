using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.Lambda
{
    public static class FieldExtend
    {
        private const string Tips = "该方法只能用于DapperExtensions.Lambda表达式！";


        /// <summary>
        /// like '%value%' 模糊查询，同Contains。
        /// </summary>
        public static bool Like(this object key, string values)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// where field in (value,value,value)。传入Array或List&lt;T>。
        /// </summary>
        public static bool In<T>(this object key, params T[] values)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// where field in (value,value,value)。传入Array或List&lt;T>。
        /// </summary>
        public static bool In<T>(this object key, List<T> values)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// where field not in (value,value,value)。传入Array或List&lt;T>。
        /// </summary>
        public static bool NotIn<T>(this object key, params T[] values)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// where field not in (value,value,value)。传入Array或List&lt;T>。
        /// </summary>
        public static bool NotIn<T>(this object key, List<T> values)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// IS NULL
        /// </summary>
        public static bool IsNull(this object key)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// IS NOT NULL
        /// </summary>
        public static bool IsNotNull(this object key)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// As
        /// </summary>
        public static bool As(this object key, string AsName)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// Sum
        /// </summary>
        public static decimal Sum(this object key)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// Count
        /// </summary>
        public static int Count(this object key)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// Avg
        /// </summary>
        public static decimal Avg(this object key)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// Len
        /// </summary>
        public static int Len(this object key)
        {
            throw new Exception(Tips);
        }
    }
}
