using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Cryption
{
    /// <summary>
    /// 加密转化
    /// </summary>
    /// 创建时间：2018-3-27
    public static class CommonEncrypt
    {
        /// <summary>
        /// 对指定字符串进行 MD5 加密
        /// </summary>
        /// <param name="inputStr">要转换的字符串</param>
        /// <returns>字符串的MD5值</returns>
        public static string GetMD5(string inputStr)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
                string[] strmd = BitConverter.ToString(result).Split('-');
                return string.Concat(strmd);
            }
        }

        /// <summary>
        /// 获取字符串的UTF-8编码
        /// </summary> 
        /// <param name="inputStr">要转换的字符串</param>
        /// <returns>字符串的UTF-8 编码</returns>
        public static string GetUtf8Encode(string inputStr)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inputStr);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x", CultureInfo.CurrentCulture).PadLeft(2, '0'));//将哈希值转换为字符串
            }
            return sb.ToString();
        }



    }
}
