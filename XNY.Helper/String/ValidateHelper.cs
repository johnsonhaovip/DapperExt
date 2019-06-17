using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace XNY.Helper.String {
    /// <summary>
    /// 验证帮助类
    /// </summary>
    /// 创建者：蒋浩
    /// 创建时间：2018-3-26
    public static class ValidateHelper {

        /// <summary>
        /// 校验ip地址
        /// </summary>
        /// <param name="ipstr">IP</param>
        public static bool IsIPAddress(string value) {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 7 || value.Length > 15)
                return false;
            return QuickValidate(@"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$", value);
        }

        /// <summary>
        /// 校验是否为中文汉字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsChinese(string value) {
            return QuickValidate(@"^[\u4e00-\u9fa5]+$", value);
        }
        /// <summary>
        /// 快速验证一个字符串是否符合指定的正则表达式。
        /// </summary>
        /// <param name="express">正则表达式的内容。</param>
        /// <param name="value">需验证的字符串。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool QuickValidate(string express, string value) {
            if (value == null)
                return false;
            if (value.Length == 0)
                return false;
            Regex myRegex = new Regex(express);
            return myRegex.IsMatch(value);
        }

        /// <summary>
        /// 判断是否存在危险字符
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static bool ProcessSqlStr(string Str) {
            bool ReturnValue = true;
            try {
                if (Str != "") {
                    string SqlStr =
    @"select*|and'|or'|insertinto|deletefrom|altertable|and|'='|' and|update|delete|insert|char|select|union|waitfor|createtable|createview|dropview|createindex|dropindex|createprocedure|dropprocedure|createtrigger|droptrigger|createschema|dropschema|createdomain|alterdomain|dropdomain|);|select@| declare@| print@| char(| select";
                    string[] anySqlStr = SqlStr.Split('|');
                    foreach (string ss in anySqlStr) {
                        if (Str.ToLower().IndexOf(ss) >= 0) {
                            ReturnValue = false;
                            return ReturnValue;
                        }
                    }
                }
            } catch {
                ReturnValue = false;
            }
            return ReturnValue;
        }

        /// <summary>    
        /// 检测是否有Sql危险字符    
        /// </summary>    
        /// <param name="str">要判断字符串</param>    
        /// <returns>判断结果</returns>    
        public static bool IsSafeSqlString(string str) {
            return !Regex.IsMatch(str, @"[-|;|,|\/|||||\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 过滤字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FilterSql(string s) {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            s = s.Trim().ToLower();
            s = s.Replace("=", "");
            s = s.Replace("'", "");
            s = s.Replace(";", "");
            s = s.Replace(" or ", "");
            s = s.Replace("select", "");
            s = s.Replace("update", "");
            s = s.Replace("insert", "");
            s = s.Replace("delete", "");
            s = s.Replace("declare", "");
            s = s.Replace("exec", "");
            s = s.Replace("drop", "");
            s = s.Replace("create", "");
            s = s.Replace("%", "");
            s = s.Replace("--", "");
            return s;
        }

        /// <summary>
        /// 验证是否存在注入代码(条件语句）
        /// </summary>
        /// <param name="inputData"></param>
        public static bool HasInjectionData(string inputData) {
            if (string.IsNullOrEmpty(inputData))
                return false;
            //里面定义恶意字符集合
            //验证inputData是否包含恶意集合
            if (Regex.IsMatch(inputData.ToLower(), GetRegexString())) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 获取正则表达式
        /// </summary>
        /// <returns></returns>
        private static string GetRegexString() {
            //构造SQL的注入关键字符
            string[] strBadChar =
            {
                //"select\\s",
                //"from\\s",
                "insert\\s",
                "delete\\s",
                "update\\s",
                "drop\\s",
                "truncate\\s",
                "exec\\s",
                "count\\(",
                "declare\\s",
                "asc\\(",
                "mid\\(",
                "char\\(",
                "net user",
                "xp_cmdshell",
                "/add\\s",
                "exec master.dbo.xp_cmdshell",
                "net localgroup administrators"
            };

            //构造正则表达式
            string str_Regex = ".*(";
            for (int i = 0; i < strBadChar.Length - 1; i++) {
                str_Regex += strBadChar[i] + "|";
            }
            str_Regex += strBadChar[strBadChar.Length - 1] + ").*";

            return str_Regex;
        }

        /// <summary>
        /// 验证密码强度
        /// </summary>
        /// <param name="password">待验证的密码</param>
        /// <returns></returns>
        public static Strength PasswordStrength(string password) {
            //空字符串强度值为0
            if (password == "")
                return Strength.Invalid;
            //字符统计
            int iNum = 0, iLtt = 0, iSym = 0;
            foreach (char c in password) {
                if (c >= '0' && c <= '9')
                    iNum++;
                else if (c >= 'a' && c <= 'z')
                    iLtt++;
                else if (c >= 'A' && c <= 'Z')
                    iLtt++;
                else iSym++;
            }
            if (iLtt == 0 && iSym == 0)
                return Strength.WeakNum; //纯数字密码
            if (iNum == 0 && iLtt == 0)
                return Strength.WeakCha; //纯符号密码
            if (iNum == 0 && iSym == 0)
                return Strength.WeakStr; //纯字母密码
            if (password.Length <= 6)
                return Strength.WeakNoLong; //长度不大于6的密码
            if (iLtt == 0)
                return Strength.Normal; //数字和符号构成的密码
            if (iSym == 0)
                return Strength.Normal; //数字和字母构成的密码
            if (iNum == 0)
                return Strength.Normal; //字母和符号构成的密码
            if (password.Length <= 10)
                return Strength.Normal; //长度不大于10的密码
            return Strength.Strong; //由数字、字母、符号构成的密码
        }
        /// <summary>
        /// 密码强度
        /// </summary>
        public enum Strength {
            [Description("无效密码")]
            Invalid = 0,
            [Description("纯数字密码")]
            WeakNum = 1, //纯数字密码-低强度密码
            [Description("纯符号密码")]
            WeakCha = 2,//纯符号密码-低强度密码
            [Description("纯字母密码")]
            WeakStr = 3,//纯字母密码-低强度密码
            [Description("密码长度小于6")]
            WeakNoLong = 4,//密码长度小于6
            [Description("中强度密码")]
            Normal = 5, //中强度密码
            [Description("高强度密码")]
            Strong = 6 //高强度密码
        };
    }
}
