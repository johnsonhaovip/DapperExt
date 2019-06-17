using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNY.Helper.Web
{    /// <summary>
     /// 图像处理方式
     /// </summary>
    public enum ImageThumbnailMode
    {
        /// <summary>
        /// 指定高宽缩放(可能变形)
        /// </summary>
        FixedWidthAndHeight,
        /// <summary>
        /// 指定宽，高按比例
        /// </summary>
        FixedWidth,
        /// <summary>
        /// 指定高，宽按比例
        /// </summary>
        FixedHeight,
        /// <summary>
        /// 指定高宽裁减(不变比例)
        /// </summary>
        Cut,
        /// <summary>
        /// 指定高宽裁减
        /// </summary>
        CustomCut,
        /// <summary>
        /// 自动缩放
        /// </summary>
        AutoFit
    }
    /// <summary>
    /// 图像处理
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// 图像处理
        /// </summary>
        private static MemoryStream MakeThumbnail(Stream originalimage, Point point, int width, int height, ImageThumbnailMode mode)
        {
            using (Image originalImage = Image.FromStream(originalimage))
            {
                int destrect_width, destrect_height, srcrect_width, srcrect_height, x, y;

                switch (mode)
                {
                    case ImageThumbnailMode.FixedWidthAndHeight://指定高宽缩放(可能变形)
                        destrect_width = width;
                        destrect_height = height;
                        srcrect_width = originalImage.Width;
                        srcrect_height = originalImage.Height;
                        x = y = 0;
                        break;
                    case ImageThumbnailMode.FixedWidth://指定宽，高按比例
                        destrect_width = width;
                        destrect_height = originalImage.Height * width / originalImage.Width;
                        srcrect_width = originalImage.Width;
                        srcrect_height = originalImage.Height;
                        x = y = 0;
                        break;
                    case ImageThumbnailMode.FixedHeight://指定高，宽按比例
                        destrect_width = originalImage.Width * height / originalImage.Height;
                        destrect_height = height;
                        srcrect_width = originalImage.Width;
                        srcrect_height = originalImage.Height;
                        x = y = 0;
                        break;
                    case ImageThumbnailMode.Cut://指定高宽裁减(不变比例)
                        destrect_width = width;
                        destrect_height = height;
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)destrect_width / (double)destrect_height)
                        {
                            srcrect_width = originalImage.Height;
                            srcrect_height = originalImage.Height * destrect_width / destrect_height;
                            y = 0;
                            x = (originalImage.Width - srcrect_width) / 2;
                        }
                        else
                        {
                            srcrect_width = originalImage.Width;
                            srcrect_height = originalImage.Width * height / destrect_width;
                            x = 0;
                            y = (originalImage.Height - srcrect_height) / 2;
                        }
                        break;
                    case ImageThumbnailMode.CustomCut://裁减
                        destrect_width = originalImage.Width;
                        destrect_height = originalImage.Height;
                        srcrect_width = width;
                        srcrect_height = height;
                        x = point.X;
                        y = point.Y;
                        break;
                    case ImageThumbnailMode.AutoFit://自动缩放
                        if (originalImage.Height > originalImage.Width)
                        {
                            destrect_width = originalImage.Width * height / originalImage.Height;
                            destrect_height = height;
                        }
                        else
                        {
                            destrect_width = width;
                            destrect_height = originalImage.Height * width / originalImage.Width;
                        }
                        srcrect_width = originalImage.Width;
                        srcrect_height = originalImage.Height;
                        x = y = 0;
                        break;
                    default:
                        throw new ArgumentException("不正确的生成缩略图方式", "mode");
                }
                var destRect = new Rectangle(0, 0, destrect_width, destrect_height);
                var srcRect = new Rectangle(x, y, srcrect_width, srcrect_height);
                //新建一个bmp图片
                using (Image bitmap = new Bitmap(destRect.Width, destRect.Height))
                {
                    //新建一个画板
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        //设置高质量插值法
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //设置高质量,低速度呈现平滑程度
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        //清空画布并以透明背景色填充
                        g.Clear(Color.Transparent);
                        //在指定位置并且按指定大小绘制原图片的指定部分
                        g.DrawImage(originalImage, destRect, srcRect, GraphicsUnit.Pixel);
                        //输出
                        MemoryStream output = new MemoryStream();
                        try
                        {
                            bitmap.Save(output, originalImage.RawFormat);
                            output.Seek(0, SeekOrigin.Begin);//uploadstream.Position = 0;
                            return output;
                        }
                        catch
                        {
                            output.Dispose();
                            throw;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        public static MemoryStream MakeThumbnail(Stream originalimage, int width, int height, ImageThumbnailMode mode)
        {
            return MakeThumbnail(originalimage, new Point(0, 0), width, height, mode);
        }
        /// <summary>
        /// 裁剪
        /// </summary>
        /// <param name="originalimage"></param>
        /// <param name="point"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static MemoryStream MakeThumbnail(Stream originalimage, Point point, int width, int height)
        {
            return MakeThumbnail(originalimage, point, width, height, ImageThumbnailMode.CustomCut);
        }

        /// <summary>
        /// 在图片上增加文字水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_sy">生成的带文字水印的图片路径</param>
        /// <param name="addText">生成的文字</param>
        public static void AddWater(string Path, string Path_sy, string addText)
        {
            Image image = Image.FromFile(Path);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(image, 0, 0, image.Width, image.Height);
            Font f = new Font("Verdana", 60);
            Brush b = new SolidBrush(Color.Green);

            g.DrawString(addText, f, b, 35, 35);
            g.Dispose();

            image.Save(Path_sy);
            image.Dispose();
        }

        /// <summary>
        /// 在图片上生成图片水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_syp">生成的带图片水印的图片路径</param>
        /// <param name="Path_sypf">水印图片路径</param>
        public static void AddWaterPic(string Path, string Path_syp, string Path_sypf)
        {
            Image image = Image.FromFile(Path);
            Image copyImage = Image.FromFile(Path_sypf);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(copyImage, new Rectangle(image.Width - copyImage.Width, image.Height - copyImage.Height, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
            g.Dispose();

            image.Save(Path_syp);
            image.Dispose();
        }
    }
}
