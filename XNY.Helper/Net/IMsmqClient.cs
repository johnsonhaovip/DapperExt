using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Net {
    public partial interface IMsmqClient {
        /// <summary>
        /// 插入消息队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="queueName">消息队列名称</param>
        /// <returns></returns>
        bool Insert<T>(T model, string queueName = "");

        /// <summary>
        /// 获取一条消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">消息队列名称</param>
        /// <returns></returns>
        T PickOne<T>(string queueName = "");
    }
}
