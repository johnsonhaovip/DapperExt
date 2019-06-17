using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.ValueObject
{
    public enum JoinType : byte
    {
        /// <summary>
        /// InnerJoin
        /// </summary>
        InnerJoin,
        /// <summary>
        /// LeftJoin
        /// </summary>
        LeftJoin,
        /// <summary>
        /// RightJoin
        /// </summary>
        RightJoin,
        /// <summary>
        /// CrossJoin
        /// </summary>
        CrossJoin,
        /// <summary>
        /// FullJoin
        /// </summary>
        FullJoin
    }
}
