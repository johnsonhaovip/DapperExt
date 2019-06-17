using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XNY.Helper.Web
{
    /// <summary>
    /// 上传文件基类
    /// </summary>
    /// 创建者：蒋浩
    /// 创建时间：2018-4-28
    public abstract class BaseFileUploader
    {
        /// <summary>
        /// 默认限制20M=20971520字节
        /// </summary>
        long _uploadmaxsize = 20971520;
        string _fileextension;
        private bool m_disposed = false;//标识资源是否被释放过
        private readonly List<string> _uploadwhitelist = new List<string>();
        private readonly List<string> _uploadblacklist = new List<string>();

        #region 文件属性
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtension
        {
            get { return _fileextension; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _fileextension = value.TrimStart('.').ToLower(CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// 文件流
        /// </summary>
        public Stream FileStream
        {
            get;
            private set;
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize
        {
            get { return FileStream.Length; }
        }
        #endregion

        #region 上传属性

        /// <summary>
        /// 扩展名-白名单
        /// </summary>
        public IList<string> UploadWhiteList
        {
            get
            {
                return _uploadwhitelist;
            }
        }

        /// <summary>
        /// 添加白名单
        /// </summary>
        /// <param name="exts"></param>
        public void AddToUploadWhiteList(params string[] exts)
        {
            if (exts != null)
            {
                foreach (var ext in exts)
                {
                    _uploadwhitelist.Add(ext);
                }
            }
        }

        /// <summary>
        /// 扩展名-黑名单（默认过滤EXE文件）
        /// </summary>
        public IList<string> UploadBlackList
        {
            get
            {
                return _uploadblacklist;
            }
        }

        /// <summary>
        /// 添加黑名单
        /// </summary>
        /// <param name="exts"></param>
        public void AddToUploadBlackList(params string[] exts)
        {
            if (exts != null)
            {
                foreach (var ext in exts)
                {
                    _uploadblacklist.Add(ext);
                }
            }
        }

        /// <summary>
        /// 上传大小限制(B)
        /// </summary>
        public long UploadMaxSize
        {
            get
            {
                return _uploadmaxsize;
            }
            set
            {
                _uploadmaxsize = value;
            }
        }
        #endregion

        #region 文件上传
        /// <summary>
        /// 上传文件初始化
        /// </summary>
        /// <param name="fliestream"></param>
        protected BaseFileUploader(Stream fliestream)
        {
            if (fliestream == null)
            {
                throw new FileNotFoundException("上传文件为空");
            }
            FileStream = fliestream;
            AddToUploadBlackList("exe");
        }

        /// <summary>
        /// 上传文件初始化
        /// </summary>
        /// <param name="postfile"></param>
        protected BaseFileUploader(HttpPostedFileBase postfile) : this(postfile == null ? null : postfile.InputStream)
        {
            if (postfile == null)
            {
                throw new FileNotFoundException("上传文件为空");
            }
            this.FileName = Path.GetFileNameWithoutExtension(postfile.FileName);//获取上传文件的名称（不包含扩展名称）
            this.FileExtension = Path.GetExtension(postfile.FileName);//获取上传文件的扩展名
        }

        /// <summary>
        /// 上传文件初始化
        /// </summary>
        /// <param name="postfile"></param>
        protected BaseFileUploader(HttpPostedFile postfile) : this(new HttpPostedFileWrapper(postfile) as HttpPostedFileBase)
        { }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~BaseFileUploader()
        {
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    //释放托管资源
                    FileStream.Dispose();
                }
                //释放非托管资源
                m_disposed = true;
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);//防止Finalize调用
        }

        /// <summary>
        /// 文件大小显示格式
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string FormatSize(long size)
        {
            float x = 0;
            string y = "";
            if (size >= 524288)
            {
                x = (float)size / 1048576;
                y = Math.Round(x, 2).ToString(CultureInfo.InvariantCulture) + "MB";
            }
            else
            {
                x = (float)size / 1024;
                y = Math.Round(x, 2).ToString(CultureInfo.InvariantCulture) + "KB";
            }
            return y;

        }
        /// <summary>
        /// 检查文件
        /// </summary>
        public void UploadCheck()
        {
            if (FileStream == null || FileSize < 0)
            {
                throw new FileNotFoundException("上传文件为空");
            }
            if (string.IsNullOrWhiteSpace(FileName))
            {
                throw new FileNotFoundException("上传文件为空");
            }
            if (string.IsNullOrWhiteSpace(FileExtension))
            {
                throw new FileNotFoundException("上传文件为空");
            }
            else if (
                (UploadBlackList != null && UploadBlackList.Count != 0 && UploadBlackList.Contains(FileExtension, StringComparer.OrdinalIgnoreCase)) ||
                (UploadWhiteList != null && UploadWhiteList.Count != 0 && !UploadWhiteList.Contains(FileExtension, StringComparer.OrdinalIgnoreCase))
                )
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "不允许上传{0}的文件格式", FileExtension));
            }
            //文件大小检查
            if (FileSize > UploadMaxSize)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "文件大小不允许超过{0}", FormatSize(UploadMaxSize)));
            }
        }

        /// <summary>
        /// 开始上传
        /// </summary>
        /// <param name="uploadpath"></param>
        /// <returns></returns>
        public abstract Uri Upload(string uploadpath);
    }

    /// <summary>
    /// 上传文件到Web
    /// </summary>
    public class WebFileUploader : BaseFileUploader
    {
        /// <summary>
        /// 上传文件初始化
        /// </summary>
        /// <param name="filestream">文件流</param>
        public WebFileUploader(Stream filestream)
            : base(filestream)
        {
        }
        /// <summary>
        /// 上传文件初始化
        /// </summary>
        /// <param name="postfile">Http文件流</param>
        public WebFileUploader(HttpPostedFile postfile)
            : base(postfile)
        {
        }
        /// <summary>
        /// 上传文件初始化
        /// </summary>
        /// <param name="postfile">Http文件流</param>
        public WebFileUploader(HttpPostedFileBase postfile)
            : base(postfile)
        {
        }

        /// <summary>
        /// 上传文件到Web
        /// </summary>
        /// <param name="uploadpath">上传路径</param>
        /// <returns></returns>
        public override Uri Upload(string uploadpath)
        {
            if (string.IsNullOrWhiteSpace(uploadpath))
            {
                throw new ArgumentException("上传路径为空", "uploadpath");
            }
            UploadCheck();//调用检查
            string _dirpath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + uploadpath);//文件路径
            //文件目录检查
            if (!Directory.Exists(_dirpath))//不存在
            {
                Directory.CreateDirectory(_dirpath);//新创建
            }
            using (Stream uploadstream = FileStream)
            {
                using (FileStream fs = new FileStream(string.Format(CultureInfo.InvariantCulture, "{0}/{1}.{2}", _dirpath, FileName, FileExtension), FileMode.Create, FileAccess.ReadWrite))
                {
                    int bufferlen = 1024;
                    byte[] buffer = new byte[bufferlen];
                    uploadstream.Seek(0, SeekOrigin.Begin);
                    while (uploadstream.Read(buffer, 0, bufferlen) != 0)
                    {
                        fs.Write(buffer, 0, bufferlen);
                        fs.Flush();
                    }
                }
            }
            return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}/{2}/{3}.{4}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, uploadpath.Trim('/'), this.FileName, this.FileExtension));
        }
    }

    /// <summary>
    /// 上传文件到Ftp
    /// </summary>
    public class FtpFileUploader : BaseFileUploader
    {
        /// <summary>
        /// Ftp用户名
        /// </summary>
        public string FtpUserid
        {
            get;
            private set;
        }

        /// <summary>
        /// Ftp密码
        /// </summary>
        public string FtpPassword
        {
            get;
            private set;
        }

        /// <summary>
        /// Ftp代理
        /// </summary>
        public WebProxy Proxy
        {
            get; set;
        }
        /// <summary>
        /// 上传文件初始化
        /// </summary>
        /// <param name="filestream">文件流</param>
        /// <param name="ftpuser">Ftp用户名</param>
        /// <param name="ftppassword">Ftp密码</param>
        public FtpFileUploader(Stream filestream, string ftpuser, string ftppassword): base(filestream)
        {
            FtpUserid = ftpuser;
            FtpPassword = ftppassword;
        }
        /// <summary>
        /// 上传文件初始化
        /// </summary>
        /// <param name="postfile">Http文件流</param>
        /// <param name="ftpuser">Ftp用户名</param>
        /// <param name="ftppassword">Ftp密码</param>
        public FtpFileUploader(HttpPostedFile postfile, string ftpuser, string ftppassword): base(postfile)
        {
            FtpUserid = ftpuser;
            FtpPassword = ftppassword;
        }

        /// <summary>
        /// 上传文件初始化
        /// </summary>
        /// <param name="postfile">Http文件流</param>
        /// <param name="ftpuser">Ftp用户名</param>
        /// <param name="ftppassword">Ftp密码</param>
        public FtpFileUploader(HttpPostedFileBase postfile, string ftpuser, string ftppassword): base(postfile)
        {
            FtpUserid = ftpuser;
            FtpPassword = ftppassword;
        }

        /// <summary>
        /// 上传文件到Ftp
        /// </summary>
        /// <param name="uploadpath">
        /// Ftp路径,不带协议的
        /// 比如:10.197.10.187/images/im75/
        /// </param>
        /// <returns></returns>
        public override Uri Upload(string uploadpath)
        {
            if (string.IsNullOrWhiteSpace(uploadpath))
            {
                throw new ArgumentException("上传路径为空", "uploadpath");
            }
            UploadCheck();
            var ftpuri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}/", Uri.UriSchemeFtp, uploadpath.TrimEnd('/')));

            MakeDir(ftpuri);

            var reqFTP = GetFtpWebRequest(string.Format(
                CultureInfo.InvariantCulture, "{0}://{1}/{2}.{3}", Uri.UriSchemeFtp, uploadpath.TrimEnd('/'), this.FileName, this.FileExtension),
                WebRequestMethods.Ftp.UploadFile);

            using (Stream poststream = FileStream, ftpstream = reqFTP.GetRequestStream())
            {
                byte[] fileContent = new byte[poststream.Length];
                poststream.Position = 0L;
                poststream.Read(fileContent, 0, fileContent.Length);
                ftpstream.Write(fileContent, 0, fileContent.Length);
            }

            return new Uri(string.Format(
                CultureInfo.InvariantCulture, "{0}://{1}/{2}.{3}", Uri.UriSchemeHttp, uploadpath.TrimEnd('/'), this.FileName, this.FileExtension));
        }
        /// <summary>
        /// 获取Ftp传输对象
        /// </summary>
        /// <param name="uri">Ftp路径</param>
        /// <param name="webrequestmethods">设置要发送到 FTP 服务器的命令</param>
        /// <returns></returns>
        FtpWebRequest GetFtpWebRequest(Uri uri, string webrequestmethods)
        {
            FtpWebRequest reqFTP = FtpWebRequest.Create(uri) as FtpWebRequest;
            reqFTP.Proxy = Proxy;
            reqFTP.KeepAlive = false;
            reqFTP.Credentials = new NetworkCredential(FtpUserid, FtpPassword);
            reqFTP.Method = webrequestmethods;
            reqFTP.UseBinary = true;
            return reqFTP;
        }
        /// <summary>
        /// 获取Ftp传输对象
        /// </summary>
        /// <param name="uristr">Ftp路径</param>
        /// <param name="webrequestmethods">设置要发送到 FTP 服务器的命令</param>
        /// <returns></returns>
        FtpWebRequest GetFtpWebRequest(string uristr, string webrequestmethods)
        {
            return GetFtpWebRequest(new Uri(uristr), webrequestmethods);
        }
        /// <summary>
        /// 检查Ftp目录
        /// </summary>
        /// <param name="uri">Ftp路径</param>
        /// <returns>是否存在</returns>
        bool IsExistDirectory(Uri uri)
        {
            try
            {
                var reqFTP = GetFtpWebRequest(uri, WebRequestMethods.Ftp.ListDirectoryDetails);
                using (var response = reqFTP.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="uri">Ftp路径</param>
        /// <returns>是否创建成功</returns>
        void MakeDir(Uri uri)
        {
            try
            {
                if (!IsExistDirectory(uri))
                {
                    string uristr = uri.ToString();
                    MakeDir(new Uri(uristr.Remove(uristr.IndexOf(uri.Segments.Last(), StringComparison.OrdinalIgnoreCase))));
                    var reqFTP = GetFtpWebRequest(uri, WebRequestMethods.Ftp.MakeDirectory);
                    using (var response = reqFTP.GetResponse()) { }
                }
            }
            catch
            {
                throw new AggregateException("创建Ftp目录" + uri.ToString() + "失败");
            }
        }
    }
}
