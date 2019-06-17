using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper
{
    /// <summary>
    /// 返回Json类型
    /// </summary>
    [DataContract]
    public class ResultJson
    {
        public ResultJson() { }
        /// <summary>
        /// 编号
        /// 1：表示操作正常；0：表示操作失败
        /// </summary>
        [DataMember]
        public int code { get; set; }

        /// <summary>
        /// 结果
        /// 操作消息（比如异常消息，或者提示消息）
        /// </summary>
        [DataMember]
        public string message { get; set; }

        /// <summary>
        /// 数据结果
        /// 获取时数据结果
        /// </summary>
        [DataMember]
        public object data { get; set; }
    }

    [DataContract]
    public class ResultJson<T>
    {
        public ResultJson() { }
        /// <summary>
        /// 编号
        /// 1：表示操作正常；0：表示操作失败
        /// </summary>
        [DataMember]
        public int code { get; set; }

        /// <summary>
        /// 结果
        /// 操作消息（比如异常消息，或者提示消息）
        /// </summary>
        [DataMember]
        public string message { get; set; }

        /// <summary>
        /// 数据结果
        /// 获取时数据结果
        /// </summary>
        [DataMember]
        public T data { get; set; }
    }
}
