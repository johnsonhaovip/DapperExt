using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XNY.Helper.Configuration {
    /// <summary>
    /// 站点配置
    /// </summary>
    /// 创建者：蒋浩
    /// 创建时间:2018-5-20
    [Serializable, XmlRoot("site")]
    public class SiteConfig : ConfigLoader<SiteConfig> {
        public SiteConfig(): base(PubConstant.SITE_CONFIG_PATH) {
        }

        /// <summary>
        /// 消息心跳频率
        /// </summary>
        private int _messageInterval = 10;
        [XmlElement("msginterval")]
        public int MessageInterval {
            get { return _messageInterval; }
            set { _messageInterval = value; }
        }

        /// <summary>
        /// 文件上传最大大小
        /// </summary>
        private double _maxFileSize = 2.0;
        [XmlElement("maxfilesize")]
        public double MaxFileSize {
            get { return _maxFileSize; }
            set { _maxFileSize = value; }
        }

        private string _fileRootPath = "uploads";
        /// <summary>
        /// 文件上传保存路径
        /// </summary>
        [XmlElement("filerootpath")]
        public string FileRootPath {
            get { return _fileRootPath; }
            set { _fileRootPath = value; }
        }

        /// <summary>
        /// 默认用户头像
        /// </summary>
        [XmlElement("defaultavatar")]
        public string DefaultAvatar { get; set; }

        /// <summary>
        /// 默认图片
        /// </summary>
        [XmlElement("defaultpic")]
        public string DefaultPicture { get; set; }

        /// <summary>
        /// 对称加密Salt
        /// </summary>
        private string _sha1salt = "swi@2018";
        [XmlElement("sha1salt")]
        public string SHA1Salt {
            get { return _sha1salt; }
            set { _sha1salt = value; }
        }

        #region 邮件设置

        /// <summary>
        /// 邮件
        /// </summary>
        [XmlElement("email")]
        public EmailConfig Email { get; set; }

        #endregion

        #region 消息队列

        /// <summary>
        /// 消息队列服务
        /// </summary>
        [XmlElement("msmq")]
        public string MSMQ { get; set; }

        #endregion

        #region IM

        /// <summary>
        /// IM服务器地址
        /// </summary>
        [XmlElement("im")]
        public IMConfig IM { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        /// <returns></returns>
        protected override SiteConfig initConfig() {
            var config = new SiteConfig() {
                MessageInterval = 10,
                MaxFileSize = 2,
                DefaultAvatar = "/Content/img/avatars/male.png",
                DefaultPicture = "/Content/img/avatars/male.png",
                SHA1Salt = "swi2014",
                FileRootPath = "~/uploads",

                #region Email

                Email = new EmailConfig() {
                    From = "swi@163.com",
                    UserName = "昆明地铁",
                    SMTP = "smtp.163.com",
                    Port = 25,
                    NickName = "昆明地铁",
                    Password = "swi2013",
                    SSL = false,
                },

                #endregion

                #region MSMQ

                MSMQ = @"FormatName:Direct=TCP:10.172.178.218\private$\",

                #endregion

                #region IM

                IM = new IMConfig() {
                    Host = "imhost.test.com",
                    Port = 5222
                }

                #endregion
            };

            return this.saveConifg(config);
        }

        #endregion
    }

    #region  Include

    /// <summary>
    /// 邮件
    /// </summary>
    [Serializable]
    [XmlRoot("email")]
    public sealed class EmailConfig {
        /// <summary>
        /// 邮件smtp服务器地址
        /// </summary>
        [XmlElement("smtp")]
        public string SMTP { get; set; }

        /// <summary>
        /// 是否启用SSL加密连接
        /// </summary>
        private bool _ssl = true;
        [XmlElement("ssl")]
        public bool SSL {
            get { return _ssl; }
            set { _ssl = value; }
        }

        /// <summary>
        /// SMTP端口
        /// </summary>
        private int _port = 25;
        [XmlElement("port")]
        public int Port {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// 发件人地址
        /// </summary>
        [XmlElement("from")]
        public string From { get; set; }

        /// <summary>
        /// 邮箱账号
        /// </summary>
        [XmlElement("username")]
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱密码
        /// </summary>
        [XmlElement("password")]
        public string Password { get; set; }

        /// <summary>
        /// 发件人昵称
        /// </summary>
        [XmlElement("nickname")]
        public string NickName { get; set; }
    }

    /// <summary>
    /// IM
    /// </summary>
    [Serializable]
    [XmlRoot("im")]
    public sealed class IMConfig {
        /// <summary>
        /// 服务器地址
        /// </summary>
        [XmlElement("host")]
        public string Host { get; set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        private int _port = 5222;
        [XmlElement("port")]
        public int Port {
            get { return _port; }
            set { _port = value; }
        }
    }

    #endregion
}
