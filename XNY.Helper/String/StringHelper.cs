using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XNY.Helper.String
{
    /// <summary>
    /// 字符串操作类
    /// </summary>
    /// 创建人：蒋浩
    /// 创建时间：2018-3-26
    public static class StringHelper
    {
        /// <summary>
        /// Sql字符串过滤
        /// </summary>
        public static string SqlSafeString(string vlaue, bool isdel)
        {
            if (!string.IsNullOrWhiteSpace(vlaue))
            {
                if (isdel)
                {
                    vlaue = vlaue.Replace("'", "");
                    vlaue = vlaue.Replace("\"", "");
                    return vlaue;
                }
                else
                {
                    vlaue = vlaue.Replace("'", "&#39;");
                    vlaue = vlaue.Replace("\"", "&#34;");
                    return vlaue;
                }
            }
            else
            {
                return vlaue;
            }
        }

        /// <summary>
        /// 得到字符串长度，一个汉字长度为2
        /// </summary>
        /// <param name="value">参数字符串</param>
        /// <returns></returns>
        public static int StrLength(string value)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(value);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }
            return tempLen;
        }

        /// <summary>
        /// 要处理的字符串
        /// </summary>
        /// <param name="value">要处理的字符串</param>
        /// <param name="len">指定字节长度</param>
        /// <param name="isShowFix">是否显示省略号</param>
        /// <returns></returns>
        public static string ClipString(string value, int len, bool isShowFix)
        {
            if (!string.IsNullOrEmpty(value))
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                int tempLen = 0;
                string tempString = "";
                byte[] s = ascii.GetBytes(value);
                for (int i = 0; i < s.Length; i++)
                {
                    if ((int)s[i] == 63)
                        tempLen += 2;
                    else
                        tempLen += 1;

                    if (i < value.Length)
                        tempString += value.Substring(i, 1);
                    else
                        break;
                    if (tempLen > len)
                        break;
                }

                byte[] mybyte = Encoding.Default.GetBytes(value);
                if (isShowFix && mybyte.Length > len)
                    tempString += "…";
                return tempString;
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 截取指定长度字符串(此方法默认显示省略号)
        /// </summary>
        /// <param name="value">要处理的字符串</param>
        /// <param name="len">指定字节长度</param>
        /// <returns></returns>
        public static string ClipString(string value, int len)
        {
            return ClipString(value, len, true);
        }

        /// <summary>
        /// HTML转行成TEXT
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string SimpleHtmlToText(string strHtml)
        {
            Regex regex1 = new Regex("<.+?>", RegexOptions.IgnoreCase);
            Regex regex2 = new Regex("\\s+", RegexOptions.IgnoreCase);
            string strOutput = "";
            if (strHtml != null)
            {
                strOutput = regex1.Replace(strHtml, "");
                strOutput = regex2.Replace(strOutput, "");
            }
            return strOutput;
        }
        /// <summary>
        /// HTML转行成TEXT,完全过滤
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string HtmlToText(string strHtml)
        {
            string[] aryReg ={
                    @"<script[^>]*?>.*?</script>",
                    @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
                    @"([\r\n])[\s]+",
                    @"&(quot|#34);",
                    @"&(amp|#38);",
                    @"&(lt|#60);",
                    @"&(gt|#62);",
                    @"&(nbsp|#160);",
                    @"&(iexcl|#161);",
                    @"&(cent|#162);",
                    @"&(pound|#163);",
                    @"&(copy|#169);",
                    @"&#(\d+);",
                    @"-->",
                    @"<!--.*\n"
                    };

            //string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, string.Empty);
            }

            strOutput = strOutput.Replace("<", "");
            strOutput = strOutput.Replace(">", "");
            strOutput = strOutput.Replace("\r\n", "");


            return strOutput;
        }

        /// <summary>
        /// 按字母表位序输出字母
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static char NumberToLetter(int index)
        {
            if (index >= 0 && index <= 25)
            {
                return Convert.ToChar(index + 65);
            }
            else
            {
                throw new ArgumentOutOfRangeException("index");
            }
        }

        /// <summary>
        /// 解析C#正则 "(?<=\"errno\":)\\S+?(?=,)";
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">正则</param>
        /// <returns></returns>
        public static string MatchInput(string input, string pattern)
        {
            MatchCollection MC = Regex.Matches(input, pattern);
            if (MC.Count == 0)
            {
                return "";
            }
            else
            {
                return MC[0].Value;
            }
        }

        #region 繁简互转
        /// <summary>
        /// 将字符串转换成简体中文
        /// </summary>
        /// <param name="source">要转换的字符串</param>
        /// <returns>转换完成后的字符串</returns>
        public static string Big5ToGB(string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            source = source.Replace("麼", "么");

            string target = new string(' ', source.Length);
            if (source.Length == NativeMethods.LCMapString(0x0800, 0x02000000, source, source.Length, target, source.Length))
                return target;
            else
                throw new FormatException("无法转换成简体中文");
        }

        /// <summary>
        /// 将字符串转换为繁体中文
        /// </summary>
        /// <param name="source">要转换的字符串</param>
        /// <returns>转换完成后的字符串</returns>
        public static string GBToBig5(string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;

            string target = new string(' ', source.Length);
            if (source.Length == NativeMethods.LCMapString(0x0800, 0x04000000, source, source.Length, target, source.Length))
                return target;
            else
                throw new FormatException("无法转换成繁体中文");
        }
        #endregion

        #region 提取汉字首字母
        /// <summary>
        /// GB2312中的汉字边界值
        /// </summary>
        private static int[] areacode = new int[27] {
            45217,45253,45761,46318,46826,
            47010,47297,47614,47614,48119,
            49062,49324,49896,50371,50614,
            50622,50906,51387,51446,52218,
            52218,52218,52698,52980,53689,54481,
            63487};
        /// <summary>
        /// 取出汉字的编码 cn 汉字
        /// </summary>
        /// <param name="myChar"></param>
        /// <returns></returns>
        private static int GetGbValue(char myChar)
        {
            byte[] bytes = Encoding.GetEncoding("gb2312").GetBytes(new char[] { myChar });
            if (bytes.Length < 2)
                return 0;
            return (bytes[0] << 8 & 0xff00) + (bytes[1] & 0xff);
        }

        /// <summary>
        /// 获取单个汉字的首拼音
        /// </summary>
        /// <param name="myChar">需要转换的字符</param>
        /// <returns>转换结果</returns>
        private static char getSpell(char myChar)
        {
            if (myChar >= 'a' && myChar <= 'z')
                return (char)(myChar - 'a' + 'A');
            if (myChar >= 'A' && myChar <= 'Z')
                return myChar;

            int gb = GetGbValue(myChar);
            if ((gb < areacode[0]) || (gb > areacode[26]))// 在码表区间之前，直接返回
                return myChar;

            switch (myChar)
            {
                case '吖':
                case '嗷':
                    return 'A';
                case '浣':
                case '桦':
                    return 'H';
                case '晟':
                    return 'S';
                case '鑫':
                    return 'X';
                default:
                    for (int i = 0; i < 26; i++)
                    {
                        if (areacode[i] <= gb && gb < areacode[i + 1])
                        {
                            return Encoding.GetEncoding("gb2312").GetString(new byte[] { (byte)(65 + i) })[0];
                        }
                    }
                    break;
            }
            return '?';
        }
        /// <summary>
        /// 提取汉字首字母
        /// </summary>
        /// <param name="strText">需要转换的字</param>
        /// <returns>转换结果</returns>
        public static string GetChineseSpell(string strText)
        {
            strText = Big5ToGB(strText);
            int len = strText.Length;
            string myStr = "";
            for (int i = 0; i < len; i++)
            {
                myStr += getSpell(strText[i]);
            }
            return myStr;
        }
        #endregion

        /// <summary>
        /// Guid转string 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToStringByGuid(Guid guid)
        {
            return Convert.ToString(guid);
        }

        /// <summary>
        /// string转Guid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ToGuidByString(string str)
        {
            return new Guid(str);
        }
    }
    /// <summary>
    /// 原生方法
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true)]
        internal static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);
    }
}
