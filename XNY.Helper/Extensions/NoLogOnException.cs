using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Extensions
{
    /// <summary>
    /// 表示在未登录而使用用户信息中发生的错误。
    /// </summary>
    [Serializable]
    public class NoLogOnException : Exception
    {
        /// <summary> 
        ///  初始化 NoLogonException 类的新实例。
        /// </summary> 
        public NoLogOnException() { }
        /// <summary>
        /// 使用指定的错误信息初始化 NoLogonException 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public NoLogOnException(string message)
            : base(message) { }
        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 NoLogonException 类的新实例。
        /// </summary>
        /// <param name="message">解释异常原因的错误信息。</param>
        /// <param name="inner">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public NoLogOnException(string message, Exception inner)
            : base(message, inner) { }
        /// <summary>
        /// 用序列化数据初始化 System.Exception 类的新实例。
        /// </summary>
        /// <param name="info">System.Runtime.Serialization.SerializationInfo，它存有有关所引发的异常的序列化对象数据。</param>
        /// <param name="context">System.Runtime.Serialization.StreamingContext，它包含有关源或目标的上下文信息。</param>
        protected NoLogOnException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
