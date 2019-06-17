using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Cryption
{
    /// <summary>
    /// Base64加密解密
    /// </summary>
    /// 创建时间：2018-3-27
    public static class Base64Encrypt
    {
        #region 加密
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="str">需加密字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Base64Encode(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
        #endregion

        #region 解密
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="str">需解密字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
        #endregion
    }
}
