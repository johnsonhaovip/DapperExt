using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace XNY.Helper.Web
{
    /// <summary>
    /// 公共方法HttpHelper
    /// </summary>
    /// 创建时间：2018-3-27
    public class HttpHelper
    {
        //默认的编码
        private Encoding encoding = Encoding.Default;
        //Post数据编码
        private Encoding postencoding = Encoding.Default;
        //HttpWebRequest对象用来发起请求
        private HttpWebRequest request = null;
        //获取影响流的数据对象
        private HttpWebResponse response = null;

        /// <summary>
        /// 根据相关参数，得到相关页面数据
        /// </summary>
        /// <param name="objhttpitem"></param>
        /// <param name="_cookie"></param>
        /// <returns></returns>
        public HttpResult GetWebHtml(HttpItem objhttpitem, ref CookieContainer _cookie)
        {
            //返回参数
            GC.Collect();
            HttpResult result = new HttpResult();
            try
            {
                //准备参数
                SetRequest(objhttpitem, ref _cookie);
            }
            catch (Exception ex)
            {
                result = new HttpResult()
                {
                    Cookie = "",
                    Header = null,
                    Html = ex.Message,
                    StatusDescription = "配置参数时出错：" + ex.Message
                };
                return result;
            }
            try
            {
                #region 得到请求的response
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    result.StatusCode = response.StatusCode;
                    result.StatusDescription = response.StatusDescription;
                    result.Header = response.Headers;
                    if (response.Cookies != null)
                        result.CookieCollection = response.Cookies;
                    if (response.Headers["set-cookie"] != null)
                        result.Cookie = response.Headers["set-cookie"];
                    MemoryStream _stream = new MemoryStream();
                    //GZIIP处理
                    if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //开始读取流并设置编码方式
                        //new GZipStream(response.GetResponseStream(), CompressionMode.Decompress).CopyTo(_stream, 10240);
                        //.net4.0以下写法
                        _stream = GetMemoryStream(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
                    }
                    else
                    {
                        //开始读取流并设置编码方式
                        //response.GetResponseStream().CopyTo(_stream, 10240);
                        //.net4.0以下写法
                        _stream = GetMemoryStream(response.GetResponseStream());
                    }
                    //获取Byte
                    byte[] RawResponse = _stream.ToArray();
                    _stream.Close();
                    //是否返回Byte类型数据
                    if (objhttpitem.ResultType == ResultType.Byte)
                        result.ResultByte = RawResponse;
                    //从这里开始我们要无视编码了
                    if (encoding == null)
                    {
                        Match meta = Regex.Match(Encoding.Default.GetString(RawResponse), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                        string charter = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToLower() : string.Empty;
                        if (charter.Length > 2)
                            encoding = Encoding.GetEncoding(charter.Trim().Replace("\"", "").Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk"));
                        else
                        {
                            if (string.IsNullOrEmpty(response.CharacterSet))
                                encoding = Encoding.UTF8;
                            else
                                encoding = Encoding.GetEncoding(response.CharacterSet);
                        }
                    }
                    //得到返回的HTML
                    result.Html = encoding.GetString(RawResponse);
                }
                #endregion
            }
            catch (WebException ex)
            {
                //这里是在发生异常时返回的错误信息
                response = (HttpWebResponse)ex.Response;
                result.Html = ex.Message;
                if (response != null)
                {
                    result.StatusCode = response.StatusCode;
                    result.StatusDescription = response.StatusDescription;
                    result.Header = response.Headers;
                }
            }
            catch (Exception ex)
            {
                result.Html = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 4.0以下.net版本取数据使用
        /// </summary>
        /// <param name="streamResponse">流</param>
        private static MemoryStream GetMemoryStream(Stream streamResponse)
        {
            MemoryStream _stream = new MemoryStream();
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = streamResponse.Read(buffer, 0, Length);
            while (bytesRead > 0)
            {
                _stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, Length);
            }
            return _stream;
        }


        /// <summary>
        /// 为请求准备参数
        /// </summary>
        ///<param name="objhttpItem">参数列表</param>
        private void SetRequest(HttpItem objhttpItem, ref CookieContainer _cookie)
        {
            request = (HttpWebRequest)WebRequest.Create(objhttpItem.URL);
            // 设置代理
            if (objhttpItem.Header != null && objhttpItem.Header.Count > 0)
            {
                foreach (string item in objhttpItem.Header.AllKeys)
                {
                    request.Headers.Add(item, objhttpItem.Header[item]);
                }
            }
            SetProxy(objhttpItem);
            //请求方式Get或者Post
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = objhttpItem.Method;
            if (!string.IsNullOrEmpty(objhttpItem.ContentType))
            {
                //ContentType返回类型
                request.ContentType = objhttpItem.ContentType;
            }
            else
            {
                if (objhttpItem.Method == "POST")
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
            }
            request.Timeout = objhttpItem.Timeout;
            request.UseDefaultCredentials = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            //Accept
            request.Accept = objhttpItem.Accept;
            //UserAgent客户端的访问类型，包括浏览器版本和操作系统信息
            request.UserAgent = objhttpItem.UserAgent;
            // 编码
            encoding = objhttpItem.Encoding;
            //设置Cookie
            SetCookie(objhttpItem);
            if (_cookie == null)
            {
                CookieContainer container = new CookieContainer();//存储cookies、
                container.SetCookies(new Uri(objhttpItem.Referer), objhttpItem.Cookie);
                request.CookieContainer = container;
                _cookie = container;
            }
            else
            {
                request.CookieContainer = _cookie;
            }
            //来源地址
            request.Referer = objhttpItem.Referer;
            //是否执行跳转功能
            request.AllowAutoRedirect = objhttpItem.Allowautoredirect;
            //设置Post数据
            SetPostData(objhttpItem);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="objhttpItem">Http参数</param>
        private void SetCookie(HttpItem objhttpItem)
        {
            if (!string.IsNullOrEmpty(objhttpItem.Cookie))
                //Cookie
                request.Headers[HttpRequestHeader.Cookie] = objhttpItem.Cookie;
            //设置Cookie
            if (objhttpItem.CookieCollection != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(objhttpItem.CookieCollection);
            }
        }

        /// <summary>
        /// 设置Post数据
        /// </summary>
        /// <param name="objhttpItem">Http参数</param>
        private void SetPostData(HttpItem objhttpItem)
        {
            //验证在得到结果时是否有传入数据
            if (request.Method.Trim().ToLower().Contains("post"))
            {
                byte[] buffer = null;
                //写入Byte类型
                if (objhttpItem.PostDataType == PostDataType.Byte && objhttpItem.PostdataByte != null && objhttpItem.PostdataByte.Length > 0)
                {
                    //验证在得到结果时是否有传入数据
                    buffer = objhttpItem.PostdataByte;
                }//写入文件
                else if (objhttpItem.PostDataType == PostDataType.FilePath && !string.IsNullOrEmpty(objhttpItem.Postdata))
                {
                    StreamReader r = new StreamReader(objhttpItem.Postdata, postencoding);
                    buffer = postencoding.GetBytes(r.ReadToEnd());
                    r.Close();
                } //写入字符串
                else if (!string.IsNullOrEmpty(objhttpItem.Postdata))
                {
                    buffer = postencoding.GetBytes(objhttpItem.Postdata);
                }
                if (buffer != null)
                {
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="objhttpItem">参数对象</param>
        private void SetProxy(HttpItem objhttpItem)
        {
            //if (!string.IsNullOrEmpty(objhttpItem.IProxy.IPConect))
            //{
            //    //设置代理服务器
            //    if (!string.IsNullOrEmpty(objhttpItem.IProxy.IPPort))
            //    {
            //        WebProxy myProxy = new WebProxy(objhttpItem.IProxy.IPConect, Convert.ToInt32(objhttpItem.IProxy.IPPort));
            //        //建议连接
            //        myProxy.Credentials = new NetworkCredential(objhttpItem.IProxy.UserName, objhttpItem.IProxy.UserPwd);
            //        //给当前请求对象
            //        request.Proxy = myProxy;
            //    }
            //    else
            //    {
            //        WebProxy myProxy = new WebProxy(objhttpItem.IProxy.IPConect, false);
            //        //建议连接
            //        myProxy.Credentials = new NetworkCredential(objhttpItem.IProxy.UserName, objhttpItem.IProxy.UserPwd);
            //        //给当前请求对象
            //        request.Proxy = myProxy;
            //    }
            //    //设置安全凭证
            //    request.Credentials = CredentialCache.DefaultNetworkCredentials;
            //}
        }

        /// <summary>
        /// 根据指定的编码对RUl进行解码
        /// </summary>
        /// <param name="text">要解码的字符串</param>
        /// <param name="encoding">要进行解码的编码方式</param>
        /// <returns></returns>
        public string URLDecode(string text, Encoding encoding)
        {
            return HttpUtility.UrlDecode(text, encoding);
        }

        /// <summary>
        /// 根据指定的编码对URL进行编码
        /// </summary>
        /// <param name="text">要编码的URL</param>
        /// <param name="encoding">要进行编码的编码方式</param>
        /// <returns></returns>
        public string URLEncode(string text, Encoding encoding)
        {
            return HttpUtility.UrlEncode(text, encoding);
        }

        /// <summary>
        /// 获取Cookie的值
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        /// <param name="cc">Cookie集合对象</param>
        /// <returns>返回Cookie名称对应值</returns>
        public string GetCookie(string cookieName, CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c1 in colCookies) lstCookies.Add(c1);
            }
            var model = lstCookies.Find(p => p.Name == cookieName);
            if (model != null)
            {
                return model.Value;
            }
            return string.Empty;
        }
    }

    public class HttpItem
    {
        string _URL = string.Empty;
        /// <summary>
        /// 链接地址
        /// </summary>
        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }
        string _Method = "GET";
        /// <summary>
        /// 请求方式默认为GET方式,当为POST方式时必须设置Postdata的值
        /// </summary>
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        int _Timeout = 90000;
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        string _Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        /// <summary>
        /// 请求标头值 默认为text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        string _ContentType = "text/html";
        /// <summary>
        /// 请求返回类型默认 text/html
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        string _UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.85 Safari/537.36";
        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.85 Safari/537.36
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        Encoding _Encoding = null;

        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding
        {
            get { return _Encoding; }
            set { _Encoding = value; }
        }
        private PostDataType _PostDataType = PostDataType.String;

        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType
        {
            get { return _PostDataType; }
            set { _PostDataType = value; }
        }
        string _Postdata = string.Empty;

        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata
        {
            get { return _Postdata; }
            set { _Postdata = value; }
        }
        private byte[] _PostdataByte = null;

        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte
        {
            get { return _PostdataByte; }
            set { _PostdataByte = value; }
        }
        CookieCollection cookiecollection = null;

        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return cookiecollection; }
            set { cookiecollection = value; }
        }
        string _Cookie = string.Empty;

        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _Cookie; }
            set { _Cookie = value; }
        }
        string _Referer = string.Empty;

        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer
        {
            get { return _Referer; }
            set { _Referer = value; }
        }
        private Boolean allowautoredirect = false;

        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }

        private WebHeaderCollection header = new WebHeaderCollection();

        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }

        private Version protocolVersion = HttpVersion.Version11;

        public Version ProtocolVersion
        {
            get { return protocolVersion; }
            set { protocolVersion = value; }
        }

        private ResultType resulttype = ResultType.String;

        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType
        {
            get { return resulttype; }
            set { resulttype = value; }
        }

        //private userProxy iProxy = new userProxy();

        //public userProxy IProxy
        //{
        //    get { return iProxy; }
        //    set { iProxy = value; }
        //}

    }

    /// <summary>
    /// Post的数据格式默认为string
    /// </summary>
    public enum PostDataType
    {
        /// <summary>
        /// 字符串类型，这时编码Encoding可不设置
        /// </summary>
        String,
        /// <summary>
        /// Byte类型，需要设置PostdataByte参数的值编码Encoding可设置为空
        /// </summary>
        Byte,
        /// <summary>
        /// 传文件，Postdata必须设置为文件的绝对路径，必须设置Encoding的值
        /// </summary>
        FilePath
    }

    /// <summary>
    /// Http返回参数类
    /// </summary>
    public class HttpResult
    {
        public string ResponeUri { get; set; }
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; }

        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte { get; set; }

        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header { get; set; }

        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// 返回状态码,默认为OK
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }

    /// 返回类型
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 表示只返回字符串 只有Html有数据
        /// </summary>
        String,
        /// <summary>
        /// 表示返回字符串和字节流 ResultByte和Html都有数据返回
        /// </summary>
        Byte
    }
}
