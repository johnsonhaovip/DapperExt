using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XNY.Helper.UI.Drawing {

    /// <summary>
    /// 生成验证码
    /// </summary>
    /// 创建者：蒋浩
    /// 创建时间：2018-5-20
    public static class CaptchaGenerator {
        #region 生成验证码：可指定图片宽，高，字符长度，字符大小，是否加粗，字符宽度，干扰点数量，噪音线数量，边框绘制对象

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="processCode">处理验证码字节流委托</param>
        /// <param name="sessionName">保存验证码的session名</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <param name="len">验证码长度</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="isbold">是否加粗</param>
        /// <param name="charW">每个字符宽度</param>
        /// <param name="pixelcount">干扰点数量</param>
        /// <param name="beziercount">噪音线数量</param>
        /// <param name="pen">绘制边框的对象（表现)</param>
        public static byte[] Generate(Action<string, string> processCode, string sessionName, int width, int height, int len, int fontsize, bool isbold, int charW, int pixelcount, int beziercount, Pen pen) {
            Bitmap img = new Bitmap(width, height);
            MemoryStream ms = new MemoryStream();
            Graphics g = Graphics.FromImage(img);
            Rectangle r = new Rectangle(0, 0, img.Width, img.Height);
            Font font;
            SolidBrush mybrush; //画笔定义 
            //LinearGradientBrush brush = new LinearGradientBrush(r, Color.Red, Color.Orange, 90);
            Random random = new Random();
            StringBuilder code = new StringBuilder(len);

            //定义 10 种颜色
            Color[] fontcolor = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Red, Color.Brown, Color.DarkCyan, Color.Purple, Color.Blue, Color.OrangeRed };
            //定义 3 种字体 
            string[] fontname = { "Verdana", "Arial", "Consolas" };

            for (int i = 0; i < len; i++) code.Append(random.Next(10));

            processCode(code.ToString(), sessionName);

            g.Clear(Color.White);

            int drawY = (img.Height - fontsize) / 2 - 1;

            //验证字符串
            for (short i = 0; i <= code.Length - 1; i++) {
                font = new Font(fontname[random.Next(0, fontname.Length)], fontsize, isbold ? FontStyle.Bold : FontStyle.Regular); //随机字体,42号，加粗
                mybrush = new SolidBrush(fontcolor[random.Next(0, fontcolor.Length)]); //随机颜色 
                g.DrawString(code.ToString().Substring(i, 1), font, mybrush, i * charW + 1, drawY);//在矩形内画出字符串 
            }

            //干扰点
            for (int i = 0; i < pixelcount; i++) {
                int x = random.Next(img.Width);
                int y = random.Next(img.Height);

                img.SetPixel(x, y, Color.FromArgb(random.Next()));
            }

            //画图片的背景噪音线
            for (int i = 0; i < beziercount; i++) {
                float x1 = random.Next(1, width);
                float x2 = random.Next(1, width);
                float x3 = random.Next(1, width);
                float x4 = random.Next(1, width);
                float y1 = random.Next(1, height);
                float y2 = random.Next(1, height);
                float y3 = random.Next(1, height);
                float y4 = random.Next(1, height);

                PointF f1 = new PointF(x1, y1);
                PointF f2 = new PointF(x2, y2);
                PointF f3 = new PointF(x3, y3);
                PointF f4 = new PointF(x4, y4);

                g.DrawBezier(new Pen(Color.Blue), f1, f2, f3, f4);
            }

            //画图形边框
            if (null != pen) {
                g.DrawRectangle(pen, 0, 0, img.Width - 1, img.Height - 1);
            }

            img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

            g.Dispose();
            ms.Dispose();
            img.Dispose();

            return ms.ToArray();
        }

        #endregion

        /// <summary>
        /// 创建并生成验证码
        /// </summary>
        /// <param name="processCode">处理验证码字节流委托</param>
        /// <param name="sessionName">保存验证码的session名</param>
        /// <param name="pen">绘制边框的对象（表现),为null时不绘制</param>
        public static byte[] GenerateImage(Action<string, string> processCode, string sessionName, Pen pen) {
            return GenerateImage(processCode, sessionName, 45, 18, 4, 12, false, 10, 40, 10, pen);
        }
        
        /// <summary>
        /// 创建并生成验证码
        /// </summary>
        /// <param name="processCode">处理验证码字节流委托</param>
        /// <param name="sessionName">保存验证码的session名</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <param name="len">验证码长度</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="pen">画笔</param>
        /// <returns></returns>
        public static byte[] GenerateImage(Action<string, string> processCode, string sessionName, int width, int height, int len,int fontSize, Pen pen) {
            return GenerateImage(processCode, sessionName, width, height, len, fontSize, false, 12, 40, 10, pen);
        }

        /// <summary>
        /// 创建并生成验证码
        /// </summary>
        /// <param name="processCode">处理验证码字节流委托</param>
        /// <param name="sessionName">保存验证码的session名</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <param name="len">验证码长度</param>
        /// <param name="charW">每个字符宽</param>
        /// <param name="pixelcount">干扰点数量</param>
        /// <param name="pen">绘制边框的对象（表现),为null时不绘制</param>
        public static byte[] GenerateImage(Action<string, string> processCode, string sessionName, int width, int height, int len, int charW, int pixelcount, Pen pen) {
            return GenerateImage(processCode, sessionName, width, height, len, 12, false, charW, pixelcount, 10, pen);
        }

        /// <summary>
        /// 创建并生成验证码
        /// </summary>
        /// <param name="processCode">处理验证码字节流委托</param>
        /// <param name="sessionName">保存验证码的session名</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <param name="len">验证码长度</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="isbold">是否加粗</param>
        /// <param name="charW">每个字符宽度</param>
        /// <param name="pen">绘制边框的对象（表现)</param>
        public static byte[] GenerateImage(Action<string, string> processCode, string sessionName, int width, int height, int len, int fontsize, bool isbold, int charW, Pen pen) {
            return GenerateImage(processCode, sessionName, width, height, len, fontsize, isbold, charW, 40, 10, pen);
        }

        /// <summary>
        /// 创建并生成验证码
        /// </summary>
        /// <param name="processCode">处理验证码字节流委托</param>
        /// <param name="sessionName">保存验证码的session名</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <param name="len">验证码长度</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="isbold">是否加粗</param>
        /// <param name="charW">每个字符宽度</param>
        /// <param name="pixelcount">干扰点数量</param>
        /// <param name="beziercount">噪音线数量</param>
        /// <param name="pen">绘制边框的对象（表现)</param>
        public static byte[] GenerateImage(Action<string, string> processCode, string sessionName, int width, int height, int len, int fontsize, bool isbold, int charW, int pixelcount, Pen pen) {
            return GenerateImage(processCode, sessionName, width, height, len, fontsize, isbold, charW, pixelcount, 15, pen);
        }

        /// <summary>
        /// 创建并生成验证码
        /// </summary>
        /// <param name="processCode">处理验证码字节流委托</param>
        /// <param name="sessionName">保存验证码的session名</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <param name="len">验证码长度</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="isbold">是否加粗</param>
        /// <param name="charW">每个字符宽度</param>
        /// <param name="pixelcount">干扰点数量</param>
        /// <param name="beziercount">噪音线数量</param>
        /// <param name="pen">绘制边框的对象（表现)</param>
        public static byte[] GenerateImage(Action<string, string> processCode, string sessionName, int width, int height, int len, int fontsize, bool isbold, int charW, int pixelcount, int beziercount, Pen pen) {
            return Generate(processCode, sessionName, width, height, len, fontsize, isbold, charW, pixelcount, beziercount, pen);
        }
    }
}
