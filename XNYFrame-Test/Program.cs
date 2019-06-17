using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using XNY.Helper;
using XNY.Helper.Extensions;
using XNY.Helper.String.NPinyin;
using XNY.Helper.WeChatMsg;
using XNYFrame_Test.PerformanceTest;

namespace XNYFrame_Test {
    class Program {
        static Stopwatch _watch = new Stopwatch();
        static int insertNum = 1000;
        static int selectNum = 100;
        static bool isClearData = false; //测试前是否清楚数据


        static void Main(string[] args) {
            #region 测试微信公众号发送模板消息
            string getTokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
            string sendMsgUrl = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
            string appid = "wxe20362d7938c78b6";
            string secert = "5f8429779f6782d4b6ca5cacff9df1d1";
            string temp = "{\"touser\":\"oiucv1SMMxcz18m4OpGtkqDO3bVQ\"," +
                   "\"template_id\":\"ejcEQNmehFgJCgjTrEzdD-nbj622QXIDTPMzJf0GjJo\"," +
                   "\"url\":" +
                   "\"\",\"topcolor\":\"#FF0000\"," +
                   "\"data\":{\"first\": {\"value\":\"昆明地铁存在红色预警\",\"color\":\"#173177\"}," +
                   "\"keyword1\": " +
                   "{\"value\":\"DK11+890DBC-R\",\"color\":\"#173177\"}}}";
            var data = TempletMessge.Send(getTokenUrl, sendMsgUrl, appid, secert, temp);
            Console.WriteLine(data);
            #endregion
            #region 测试Redis
            #endregion
            #region 测试主键为Guid

            //StudentService stuSer = new StudentService("DefaultConnection");
            //var model = new Student() {
            //    Number = Guid.NewGuid(),
            //    Sex="男",
            //    Phone="12345679"            
            //};
            //stuSer.Insert(model);
            #endregion
            //验证sql语句
            //var str = "delete ";
            //var iss= XNY.Helper.String.ValidateHelper.HasInjectionData(str);
            //验证密码强度
            //var pas = "Jh.55161725480496";
            //var value = XNY.Helper.String.ValidateHelper.PasswordStrength(pas).ToDescription();
            //Console.WriteLine(value);
            #region 测试汉字转拼音
            //string[] maxims = new string[]{
            //           "事常与人违，事总在人为",
            //           "骏马是跑出来的，强兵是打出来的",
            //           "驾驭命运的舵是奋斗。不抱有一丝幻想，不放弃一点机会，不停止一日努力。 ",
            //           "如果惧怕前面跌宕的山岩，生命就永远只能是死水一潭",
            //           "懦弱的人只会裹足不前，莽撞的人只能引为烧身，只有真正勇敢的人才能所向披靡"
            //        };

            //string[] medicines = new string[] {
            //                                "聚维酮碘溶液",
            //                                "开塞露",
            //                                "炉甘石洗剂",
            //                                "苯扎氯铵贴",
            //                                "鱼石脂软膏",
            //                                "莫匹罗星软膏",
            //                                "红霉素软膏",
            //                                "氢化可的松软膏",
            //                                "曲安奈德软膏",
            //                                "丁苯羟酸乳膏",
            //                                "双氯芬酸二乙胺乳膏",
            //                                "冻疮膏",
            //                                "克霉唑软膏",
            //                                "特比奈芬软膏",
            //                                "酞丁安软膏",
            //                                "咪康唑软膏、栓剂",
            //                                "甲硝唑栓",
            //                                "复方莪术油栓"
            //                              };

            //Console.WriteLine("中国" + Pinyin.GetPinyin("中国"));
            //Console.WriteLine("UTF8句子拼音：");
            //foreach (string s in maxims)
            //{
            //    Console.WriteLine("汉字：{0}\n拼音：{1}\n", s, Pinyin.GetPinyin(s));
            //}

            //Encoding gb2312 = Encoding.GetEncoding("GB2312");
            //Console.WriteLine("GB2312拼音简码：");
            //foreach (string m in medicines)
            //{
            //    string s = Pinyin.ConvertEncoding(m, Encoding.UTF8, gb2312);
            //    Console.WriteLine("药品：{0}\n简码：{1}\n", s, Pinyin.GetInitials(s, gb2312));
            //}

            //Console.ReadKey();
            #endregion

            #region 测试控制台背景颜色
            //_watch.Restart();
            //// Get a string array with the names of ConsoleColor enumeration members.
            //String[] colorNames = ConsoleColor.GetNames(typeof(ConsoleColor));

            //// Display each foreground color except black on a constant black background.
            //Console.WriteLine("All the foreground colors (except Black) on a constant black background:");

            //foreach (string colorName in colorNames)
            //{
            //    // Convert the string representing the enum name to the enum value.
            //    ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorName);

            //    if (color == ConsoleColor.Black) continue;

            //    Console.Write("{0,11}: ", colorName);
            //    Console.BackgroundColor = ConsoleColor.Black;
            //    Console.ForegroundColor = color;
            //    Console.WriteLine("This is foreground color {0}.", colorName);
            //    // Restore the original foreground and background colors.
            //    Console.ResetColor();
            //}
            //Console.WriteLine();

            //// Display each background color except white with a constant white foreground.
            //Console.WriteLine("All the background colors (except White) with a constant white foreground:");

            //foreach (string colorName in colorNames)
            //{
            //    // Convert the string representing the enum name to the enum value.
            //    ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorName);

            //    if (color == ConsoleColor.White) continue;

            //    Console.Write("{0,11}: ", colorName);
            //    Console.ForegroundColor = ConsoleColor.White;
            //    Console.BackgroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorName);
            //    Console.WriteLine("This is background color {0}.", colorName);
            //    Console.ResetColor();
            //}
            //_watch.Stop();
            //Console.WriteLine(mSecondToHour(_watch.ElapsedMilliseconds));
            #endregion

            //PerformanceTest();
            //LambdaQueryHelper<UsersEntity> from = DapperExtension.DapperImplementor.LambdaQuery<UsersEntity>((IDbConnection)Common.GetConnByKey(), null, null);

            //int id = 10;
            //int records = 0;
            //var where = new Where<UsersEntity>();
            //where.And(o => o.Status == 1);
            //where.And<UserInfoEntity>((a, b) => b.CardId != "" && b.Age > 0);
            //if (id > 0)
            //    where.And(o => o.UserId > 0);

            //var lambda = from.InnerJoin<UserInfoEntity>((a, b) => a.UserId == b.UserId).Select<UserInfoEntity>((a, b) => new
            //{
            //    a.UserId,
            //    a.LoginName,
            //    a.CreateTime,
            //    b.Age,
            //    b.Name,
            //    b.Sex,
            //    b.CardId,
            //    b.Email,
            //    b.Mobile,
            //    b.Remark
            //}).Where(where);
            //records = lambda.Count();
            //var list = lambda.OrderByDescending(o => o.UserId).Page(2, 2).ToList<UsersEntity>().ToList();




            //var list = from.Select(p => p.UserId)
            //        .AddSelect(p => p.LoginName)
            //        .Page(1, 2).ToList();

            //list = from.Page(2, 2).ToList();

            //Console.WriteLine(from.SqlString);

            //foreach (var item in from.Parameters)
            //{
            //    Console.WriteLine("参数： " + item.Key + "  " + item.Value.ParameterValue.ToString());
            //}



            Console.WriteLine("测试结束");
            Console.ReadLine();
        }


        public static string mSecondToHour(long time) {
            string str = "";
            double mtime = time / 1000;
            int hour = 0;
            int minute = 0;
            int second = 0;
            second = Convert.ToInt32(mtime);

            if (second > 60) {
                minute = second / 60;
                second = second % 60;
            }
            if (minute > 60) {
                hour = minute / 60;
                minute = minute % 60;
            }
            return (hour + "小时" + minute + "分钟" + second + "秒");
        }
        static void BeginTest(IPerformanceTest test) {
            //插入
            //Console.WriteLine();
            //_watch.Restart();
            //int num = test.InsertData(insertNum);
            //_watch.Stop();
            //if (num > 0)
            //{
            //    Console.WriteLine(string.Format("{0}插入{1}条数据用时：{2}毫秒,平均每条耗时{3}毫秒", test.TestName, insertNum, _watch.ElapsedMilliseconds, _watch.ElapsedMilliseconds / insertNum));
            //}

            //return;

            #region 测试各种数据访问
            ////查询
            //_watch.Restart();
            //int mapNum = 0;
            //for (int i = 0; i < selectNum; i++)
            //{
            //    //test.InsertData(1);
            //    mapNum = test.GetData(1);
            //}
            ////test.BulkCopy();
            //_watch.Stop();
            //Console.WriteLine(string.Format("{0}查询测试：映射{1}条数据,执行{2}次总耗时{3}毫秒,平均耗时{4}毫秒", test.TestName, mapNum, selectNum, _watch.ElapsedMilliseconds, _watch.ElapsedMilliseconds / selectNum));
            #endregion

            #region 测试Json扩展方法
            //ResultJson res = new ResultJson()
            //{
            //    code = 1,
            //    message = "操作成功"
            //};
            //var jsonStr = JsonExtension.ToJson(res);
            //Console.WriteLine(jsonStr);
            #endregion

            #region 测试枚举扩展方法
            //var str = EnumTest.Create.ToDescription();
            //Console.WriteLine(str);
            #endregion

            //LogHelper.Test();
            //LogHelper.Info("ASASAS");
            _watch.Restart();
            test.BulkCopy();
            _watch.Stop();
            Console.WriteLine(string.Format("耗时{0}毫秒", _watch.ElapsedMilliseconds));
        }

        static void PerformanceTest() {

            IPerformanceTest test;

            //Common.TruncateData();
            //Common.InsertTestData(10000);
            //Console.WriteLine("造数完成");
            //return;


            //test = new NormalMapping(isClearData);
            //BeginTest(test);

            //test = new AdoTeset(isClearData);
            //BeginTest(test);


            //test = new DapperTest(isClearData);
            //BeginTest(test);

            test = new DapperExtensionsTest(isClearData);
            BeginTest(test);

            ////test = new DapperExtensions_LambdaTest(isClearData);
            ////BeginTest(test);


            //test = new EF_Test(isClearData);
            //BeginTest(test);

        }


    }
}
