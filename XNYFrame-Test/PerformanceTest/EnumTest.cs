using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNYFrame_Test.PerformanceTest
{
    public enum EnumTest
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        Create = 0,

        /// <summary>
        /// 已接受
        /// </summary>
        [Description("处理中")]
        Receiving = 1,

        /// <summary>
        /// 处理完成
        /// </summary>
        [Description("处理完成")]
        Processed = 2,
    }
}
