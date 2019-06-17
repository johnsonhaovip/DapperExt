using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace XNY.Helper.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回该类型默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败返回类型的默认值 </returns>
        public static T CastTo<T>(this object value)
        {
            object result;
            Type type = typeof(T);
            try
            {
                if (type.IsEnum)
                    result = Enum.Parse(type, value.ToString());
                else if (type == typeof(Guid))
                    result = Guid.Parse(value.ToString());
                else
                    result = Convert.ChangeType(value, type);
            }
            catch
            {
                result = default(T);
            }

            return (T)result;
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            object result;
            Type type = typeof(T);
            try
            {
                result = type.IsEnum ? Enum.Parse(type, value.ToString()) : Convert.ChangeType(value, type);
            }
            catch
            {
                result = defaultValue;
            }
            return (T)result;
        }

        /// <summary>
        /// 圆角切换半角
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>切换后字符</returns>
        public static string ToEnglishNumber(this string str)
        {
            char[] chars = str.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == 12288)
                {
                    chars[i] = (char)32;
                    continue;
                }

                if (chars[i] > 65280 && chars[i] < 65375)
                    chars[i] = (char)(chars[i] - 65248);
            }

            return new string(chars);
        }

        /// <summary>
        /// 获取类型和对象名称
        /// </summary>
        /// <param name="value">对象值</param>
        /// <returns></returns>
        public static string ToTypeString(this object value)
        {
            return value.GetType().Name + "." + value.ToString();
        }

        /// <summary>
        /// 除去HTML标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveHtml(this string html)
        {
            var noHtml = Regex.Replace(html, "<[^>]+>", "");
            noHtml = Regex.Replace(noHtml, "&[^;]+;", "");

            return noHtml;
        }

        /// <summary>
        /// 获得字符串中开始和结束字符串中间得值
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="start">开始</param>
        /// <param name="end">结束</param>
        /// <returns></returns> 
        public static string GetValue(this string text, string start, string end)
        {
            try
            {
                var rg = new Regex("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                if (rg.IsMatch(text))
                    return rg.Match(text).Value;
            }
            catch
            {

            }
            return string.Empty;
        }

        /// <summary>
        /// 返回处理后的十六进制字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns>hex</returns>
        public static string ToHex(this string input)
        {
            return BitConverter.ToString(ASCIIEncoding.Default.GetBytes(input)).Replace("-", "");
        }

        /// <summary>
        /// 返回十六进制代表的字符串
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string HexToString(this string hex)
        {
            hex = hex.Replace(" ", "");
            if (hex.Length == 0) return "";

            byte[] vBytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                if (!byte.TryParse(hex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                {
                    vBytes[i / 2] = 0;
                }
            }
            return ASCIIEncoding.Default.GetString(vBytes);
        }

    }

}
