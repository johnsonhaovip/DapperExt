using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Cryption
{
    /// <summary>
    /// 对称加密算法
    /// </summary>
    public static class AesEncrypt
    {
        /// <summary>
        /// 定义西南院对称加密初始化向量
        /// </summary>
        const string XnyIV = "iv28HhTrt+NoxXVoI9+GDQ==";

        #region 加密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="inputStr">待加密的串</param>
        /// <param name="key">密钥</param>
        /// <returns>经过加密的串</returns>
        public static string Encrypt(string inputStr, string key)
        {
            return Encrypt(inputStr, key, XnyIV);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="inputStr">待加密串</param>
        /// <param name="key">秘钥</param>
        /// <param name="iv">向量</param>
        /// <returns>返回加密后的字符串</returns>
        public static string Encrypt(string inputStr, string key, string iv)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    MemoryStream ms = new MemoryStream();
                    CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                    byte[] bytes = Encoding.UTF8.GetBytes(inputStr);
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    ms.Position = 0;
                    bytes = new byte[ms.Length];
                    ms.Read(bytes, 0, bytes.Length);

                    return Convert.ToBase64String(bytes);
                }
            }
        }
        #endregion

        #region 解密
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="inputStr">待解密的串</param>
        /// <param name="key">密钥</param>
        /// <returns>经过解密的串</returns>
        public static string Decrypt(string inputStr, string key)
        {
            return Decrypt(inputStr, key, XnyIV);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="inputStr">待解密的串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>经过解密的串</returns>
        public static string Decrypt(string inputStr, string key, string iv)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    byte[] bytes = Convert.FromBase64String(inputStr);
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    ms.Position = 0;
                    bytes = new byte[ms.Length];
                    ms.Read(bytes, 0, bytes.Length);
                    return Encoding.UTF8.GetString(bytes);
                }
            };
        }

        #endregion

    }
}
