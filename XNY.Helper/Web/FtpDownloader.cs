/****************************************************************
 * 项目名称：XNY.Helper.Web
 * 类 名 称：FtpDownloader
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.Web
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/7/6 14:34:01
 * 更新时间：2018/7/6 15:34:01
 * 
 * 描述说明：Ftp下载类
 * 范例：
 * /// <summary>
    /// ftp文件下载
    /// </summary>
    /// <returns></returns>
    public ActionResult FtpDownload()
    {
        var ftpdownloader = new FtpDownloader("不带协议的路径,如:10.0.0.1/file.ext", "ftp用户名", "ftp密码");
        return File(ftpdownloader.FileStream(), "application/octet-stream", "下载文件名称");
    }
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Johnson 2018 All rights reserved
*****************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Web {

    /// <summary>
    /// Ftp下载类
    /// </summary>
    public class FtpDownloader : IDisposable {
        /// <summary>
        /// 标识资源是否被释放过
        /// </summary>
        bool m_disposed = false;
        /// <summary>
        /// Ftp文件流
        /// </summary>
        Stream filestream;
        /// <summary>
        /// 下载文件初始化
        /// </summary>
        /// <param name="ftpfilepath">ftp文件路径,不带ftp</param>
        /// <param name="ftpuser">Ftp用户名</param>
        /// <param name="ftppassword">Ftp密码</param>
        public FtpDownloader(string ftpfilepath, string ftpuser, string ftppassword) {
            if (string.IsNullOrWhiteSpace(ftpfilepath))
                throw new ArgumentException("下载文件的Ftp路径");
            FtpFilePath = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}", Uri.UriSchemeFtp, ftpfilepath));
            FtpUserid = ftpuser;
            FtpPassword = ftppassword;
        }
        /// <summary>
        /// ftp文件释放
        /// </summary>
        ~FtpDownloader() {
            Dispose(false);
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);//防止Finalize调用
        }
        /// <summary>
        /// 释放
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (!m_disposed) {
                if (disposing) {
                    //释放托管资源
                    FileStream.Dispose();
                }
                //释放非托管资源
                m_disposed = true;
            }
        }

        /// <summary>
        /// 下载文件的Ftp路径
        /// </summary>
        public Uri FtpFilePath {
            get;
            private set;
        }
        /// <summary>
        /// Ftp用户名
        /// </summary>
        public string FtpUserid {
            get;
            private set;
        }
        /// <summary>
        /// Ftp密码
        /// </summary>
        public string FtpPassword {
            get;
            private set;
        }
        /// <summary>
        /// Ftp代理 new WebProxy("192.168.0.B:808", true);
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        /// 获取Ftp文件流
        /// </summary>
        public Stream FileStream {
            get {
                if (filestream != null) { return filestream; }
                var reqFTP = GetFtpWebRequest(FtpFilePath, WebRequestMethods.Ftp.DownloadFile);
                filestream = reqFTP.GetResponseStream();
                return FileStream;
            }
        }

        /// <summary>
        /// 获取Ftp传输对象
        /// </summary>
        /// <param name="uri">Ftp路径</param>
        /// <param name="webrequestmethods">设置要发送到 FTP 服务器的命令</param>
        /// <returns></returns>
        FtpWebResponse GetFtpWebRequest(Uri uri, string webrequestmethods) {
            FtpWebRequest reqFTP = FtpWebRequest.Create(uri) as FtpWebRequest;
            reqFTP.Proxy = Proxy;
            reqFTP.KeepAlive = false;
            reqFTP.Credentials = new NetworkCredential(FtpUserid, FtpPassword);
            reqFTP.Method = webrequestmethods;
            reqFTP.UseBinary = true;
            return reqFTP.GetResponse() as FtpWebResponse;
        }
    }
}
