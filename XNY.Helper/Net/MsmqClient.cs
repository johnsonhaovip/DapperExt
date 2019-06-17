using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using XNY.Helper.Extensions;
using XNY.Helper.Configuration;

namespace XNY.Helper.Net {
    public partial class MsmqClient : IMsmqClient {
        #region Fileds

        private string _queueName = "push_message";
        private readonly SiteConfig _siteCfg;

        #endregion

        #region Ctor
        public MsmqClient(string queueName) {
            this._queueName = queueName;
            this._siteCfg = SiteConfig.Instance;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 1.通过Create方法创建使用指定路径的新消息队列
        /// </summary>
        /// <param name="queuePath"></param>
        public void Createqueue(string queuePath) {
            try {
                if (!MessageQueue.Exists(queuePath)) {
                    MessageQueue.Create(queuePath);
                }
            } catch (MessageQueueException e) {
                LogHelper.Error(e);
            }
        }

        /// <summary>
        /// 连接消息队列并发送消息到队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">插入实体类型</param>
        /// <param name="queueName">消息队列名称</param>
        /// <returns></returns>
        public virtual bool Insert<T>(T model, string queueName = "") {
            var flag = false;

            if (!string.IsNullOrEmpty(queueName)) {
                _queueName = queueName;
            }

            try {
                var queuePath = string.Concat(this._siteCfg.MSMQ, _queueName);
                //连接到本地的队列
                using (var messageQueue = new MessageQueue(queuePath) {
                    //Formatter = new XmlMessageFormatter(new[] {typeof (Email)}),
                    //Authenticate = true,
                    //EncryptionRequired = EncryptionRequired.None
                }) {
                    var message = new Message() {
                        Body = model,
                        Recoverable = true,
                        Formatter = new BinaryMessageFormatter()
                    };

                    //发送消息到队列中
                    messageQueue.Send(message);
                }
                flag = true;

                LogHelper.Info("插入消息队列：" + queuePath + " 。entity：" + model.ToJson());
            } catch (Exception ex) {
                try {
                    LogHelper.Error(ex);
                } catch { }

                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 获取一条消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">消息队列名称</param>
        /// <returns></returns>
        public virtual T PickOne<T>(string queueName = "") {
            if (!string.IsNullOrEmpty(queueName)) {
                _queueName = queueName;
            }

            using (var queue = new MessageQueue(this._siteCfg.MSMQ + _queueName)) {
                queue.Formatter = new BinaryMessageFormatter();
                try {
                    var message = queue.Receive(new TimeSpan(0, 0, 20));
                    var model = message.Body == null ? default(T) : (T)message.Body;

                    LogHelper.Info("接收消息队列" + this._siteCfg.MSMQ + _queueName + " 。entity：" + model.ToJson());
                    return model;
                } catch (MessageQueueException ex) {
                    if (ex.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout) {
                        LogHelper.Error(ex);
                    }
                } catch (InvalidCastException ex) {
                    LogHelper.Error(ex);
                } catch (Exception ex) {
                    LogHelper.Error(ex);
                }
                return default(T);
            }
        }
        #endregion
    }
}
