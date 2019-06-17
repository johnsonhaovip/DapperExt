using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Extensions {

    /// <summary>
    /// 针对网络请求的扩展方法
    /// </summary>
    /// 创建者：蒋浩
    /// 创建时间：2018-5-21
    public static class NetworkExtension {

        /// <summary>
        /// 设置请求头信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WebClient AddHeader(this WebClient req, string name, string value) {
            req.Headers.Add(name, value);
            return req;
        }

        /// <summary>
        /// 设置请求头信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static WebClient AddHeader(this WebClient req, params NameValueCollection[] headers) {
            if (headers.Length > 0) {
                foreach (var header in headers) {
                    req.Headers.Add(header);
                }
            }
            return req;
        }

        /// <summary>
        /// Get请求，返回服务器响应内容
        /// </summary>
        /// <param name="req"></param>
        /// <param name="address"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        public static string Get(this WebClient req, string address, params KeyValuePair<string, string>[] pair) {
            using (req) {
                var formBodyPair = pair.Select(q => string.Format("{0}={1}", q.Key, q.Value));
                return req.DownloadString(string.Concat(address, "?", string.Join("&", formBodyPair)));
            }
        }

        /// <summary>
        /// Get请求，返回服务器响应内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="req"></param>
        /// <param name="address"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        public static T Get<T>(this WebClient req, string address, params KeyValuePair<string, string>[] pair) {
            using (req) {
                var formBodyPair = pair.Select(q => string.Format("{0}={1}", q.Key, q.Value));
                var srcResponse = req.DownloadString(string.Concat(address, "?", string.Join("&", formBodyPair)));
                return JsonConvert.DeserializeObject<T>(srcResponse);
            }
        }

        /// <summary>
        /// Post请求，返回服务器响应内容
        /// </summary>
        /// <param name="req"></param>
        /// <param name="address"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        public static string Post(this WebClient req, string address, params KeyValuePair<string, string>[] pair) {
            using (req) {
                req.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var formBodyPair = pair.Select(q => string.Format("{0}={1}", q.Key, q.Value));
                var response = req.UploadData(address, "POST", Encoding.UTF8.GetBytes(string.Join("&", formBodyPair)));
                return Encoding.UTF8.GetString(response);
            }
        }

        /// <summary>
        /// Post请求，返回服务器响应内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="req"></param>
        /// <param name="address"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        public static T Post<T>(this WebClient req, string address, params KeyValuePair<string, string>[] pair) {
            using (req) {
                req.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var formBodyPair = pair.Select(q => string.Format("{0}={1}", q.Key, q.Value));
                var response = req.UploadData(address, "POST", Encoding.UTF8.GetBytes(string.Join("&", formBodyPair)));
                var srcResponse = Encoding.UTF8.GetString(response);
                return JsonConvert.DeserializeObject<T>(srcResponse);
            }
        }


    }
}
