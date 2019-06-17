/****************************************************************
 * 项目名称：XNY.Helper.WeChatMsg
 * 类 名 称：SendTempletMessge
 * 版 本 号：v1.0.0.0
 * 作    者：Johnson
 * 所在的域：DESKTOP-4903FQH
 * 命名空间：XNY.Helper.WeChatMsg
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/8/3 14:05:44
 * 更新时间：2018/8/6 08:53:44
 * 
 * 描述说明：微信公众号发送模板消息
 *
 * 修改历史：修改部分可能存在的Bug
 *
*****************************************************************
 * Copyright @ Johnson 2018 All rights reserved
*****************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.WeChatMsg {

    /// <summary>
    /// 微信模板消息
    /// </summary>
    public class TempletMessge {

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="getTokenUrl">获取微信公众号token的url;例：https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1} </param>
        /// <param name="sendMsgUrl">发送模板消息的url；例：https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0} </param>
        /// <param name="appid">微信公众号的appid</param>
        /// <param name="secret">秘钥</param>
        /// <param name="tempJsonData">消息模板</param>
        /// <returns></returns>
        public static ResultJson<bool> Send(string getTokenUrl, string sendMsgUrl, string appid, string secret, string tempJsonData) {
            var result = new ResultJson<bool>() {
                code = 0,
                message = "发送失败",
                data = false
            };
            try {

                #region 获取Token

                string requstUrl = string.Format(getTokenUrl, appid, secret);
                WebRequest request = WebRequest.Create(requstUrl);
                request.Method = "Post";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encode = Encoding.UTF8;
                StreamReader reader = new StreamReader(stream, encode);
                string detail = reader.ReadToEnd();
                var jd = JsonExtension.ToModel<WXApi>(detail) as WXApi;
                string token = (string)jd.access_token;

                #endregion

                #region 组装信息，推送

                sendMsgUrl = string.Format(sendMsgUrl, token);
                var responseData = GetResponseData(tempJsonData, sendMsgUrl);
                if (responseData.code == 1) {
                    ResponseModel model = JsonExtension.ToModel<ResponseModel>(responseData.data) as ResponseModel;
                    if (model != null && model.errcode == 0) {
                        result.code = 1;
                        result.data = true;
                        result.message = "发送成功";
                        return result;
                    } else {
                        result.message = model == null ? "发送失败" : model.errmsg;
                        return result;
                    }
                } else {
                    return result;
                }

                #endregion

            } catch (Exception ex) {
                return result;
            }

        }
        /// <summary>
        /// 返回JSon数据
        /// </summary>
        /// <param name="JSONData">要处理的JSON数据</param>
        /// <param name="Url">要提交的URL</param>
        /// <returns>返回的JSON处理字符串</returns>
        private static ResultJson<string> GetResponseData(string JSONData, string Url) {
            var result = new ResultJson<string>() {
                code = 0,
            };
            try {
                byte[] bytes = Encoding.UTF8.GetBytes(JSONData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentLength = bytes.Length;
                request.ContentType = "json";
                Stream reqstream = request.GetRequestStream();
                reqstream.Write(bytes, 0, bytes.Length);
                //声明一个HttpWebRequest请求
                request.Timeout = 90000;
                //设置连接超时时间
                request.Headers.Set("Pragma", "no-cache");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamReceive = response.GetResponseStream();
                Encoding encoding = Encoding.UTF8;
                StreamReader streamReader = new StreamReader(streamReceive, encoding);
                string strResult = streamReader.ReadToEnd();
                streamReceive.Dispose();
                streamReader.Dispose();

                result.code = 1;
                result.data = strResult;
                result.message = "程序成功";
                return result;
            } catch (Exception ex) {
                result.message = ex.Message;
                return result;
            }
        }

        private class WXApi {
            public string access_token { set; get; }
        }

        /// <summary>
        /// 微信发送消息后返回的JsonModel
        /// </summary>
        private class ResponseModel {

            /// <summary>
            /// 微信端返回code
            /// </summary>
            public int errcode { set; get; }

            /// <summary>
            /// 微信端返回消息提示
            /// </summary>
            public string errmsg { set; get; }

            /// <summary>
            /// 微信端返回消息id
            /// </summary>
            public string msgid { set; get; }
        }
    }
}
