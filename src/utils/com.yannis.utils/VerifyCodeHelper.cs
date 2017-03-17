using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.yannis.utils
{
    /// <summary>
    /// 验证码生成类
    /// </summary>
    public static class VerifyCodeHelper
    {
        private static FontFamily[] _families = { };

        private static Color[] _colors = { Color.ForestGreen, Color.FromArgb(0, 68, 0xA3), Color.FromArgb(10, 0x1f, 28), Color.FromArgb(0xe8, 0, 0), Color.FromArgb(84, 0x0b, 0x0b), Color.FromArgb(0x1f, 84, 0x0b), Color.FromArgb(11, 65, 01), Color.FromArgb(79, 0x2c, 0x8f), Color.FromArgb(0x0d, 01, 65) };

        /// <summary>
        /// 生成验证码[空心3D字体]
        /// </summary>
        /// <param name="len">验证码字符位数</param>
        /// <param name="w">验证码宽度</param>
        /// <param name="h">验证码高度</param>
        /// <param name="code">返回验证码文字</param>
        public static byte[] VerifyCode(int len, int w, int h, out string code)
        {
            string resourcePath = CustomerConfigure.VerifyCodeResourcePath;

            PrivateFontCollection privateFontCollection = new PrivateFontCollection();

            privateFontCollection.AddFontFile(resourcePath + @"\\font\\swisscbo.ttf");
            privateFontCollection.AddFontFile(resourcePath + @"\\font\\LARSON.ttf");
            privateFontCollection.AddFontFile(resourcePath + @"\\font\\AgentRed.ttf");

            _families = privateFontCollection.Families;

            if (len <= 0)
                len = 5;
            code = RandomCode(len);
            var bmpOut = GenerateImage(code, new Size(90, 42));
            var ms = new MemoryStream();
            bmpOut.Save(ms, ImageFormat.Png);
            var bmpBytes = ms.GetBuffer();
            bmpOut.Dispose();
            ms.Close();
            privateFontCollection.Dispose();
            return bmpBytes;
        }

        /// <summary>Generates the challenge image.</summary>
        /// <param name="text">The text to be rendered into the image.</param>
        /// <param name="size">The size of the image to generate.</param>
        /// <returns>A dynamically-generated challenge image.</returns>
        private static Bitmap GenerateImage(string text, Size size)
        {
            Random rand = new Random();
            // Create the new Bitmap of the specified size and render to it
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;

                //Draw the background with a white brush
                using (Brush b = new SolidBrush(Color.FromArgb(0xec, 0xf8, 0xff)))
                {
                    g.FillRectangle(b, 0, 0, bmp.Width, bmp.Height);
                }

                // Select a font family and create the default sized font.  We then need to shrink
                // the font size until the text fits.
                FontFamily ff = _families[rand.Next(_families.Length)];
                int emSize = (int)(size.Width * 2.5 / text.Length);
                Font f = new Font(ff, emSize);
                try
                {
                    // Make sure that the font size we have will fit with the selected text.
                    SizeF measured = new SizeF(0, 0);
                    SizeF workingSize = new SizeF(size.Width, size.Height);
                    while (emSize > 2 &&
                        (measured = g.MeasureString(text, f)).Width > workingSize.Width ||
                        measured.Height > workingSize.Height)
                    {
                        f.Dispose();
                        f = new Font(ff, emSize -= 2, FontStyle.Bold);
                    }

                    // Select a color and draw the string into the center of the image
                    using (StringFormat fmt = new StringFormat())
                    {
                        fmt.Alignment = fmt.LineAlignment = StringAlignment.Center;

                        using (Brush b = new LinearGradientBrush(
                                  new Rectangle(0, 0, size.Width / 2, size.Height / 2),
                                  _colors[rand.Next(_colors.Length)],
                                  _colors[rand.Next(_colors.Length)],
                                  (float)(rand.NextDouble() * 360), false))
                        {
                            g.DrawString(text, f, b, new Rectangle(0, 5, bmp.Width, bmp.Height), fmt);
                        }
                    }
                }
                finally
                {
                    // Clean up
                    f.Dispose();
                }
            }

            // Distort the final image and return it.  This distortion amount is fairly arbitrary.
            DistortImage(bmp, rand.Next(5, 10) * (rand.Next(2) == 1 ? 1 : -1));

            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (var pen = new Pen(Color.FromArgb(0xd7, 0xed, 0xfa)))
                {
                    g.DrawRectangle(pen, new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1));
                }
            }
            return bmp;
        }


        /// <summary>Distorts the image.</summary>
        /// <param name="b">The image to be transformed.</param>
        /// <param name="distortion">An amount of distortion.</param>
        private static void DistortImage(Bitmap b, double distortion)
        {
            int width = b.Width, height = b.Height;

            // Copy the image so that we're always using the original for source color
            using (Bitmap copy = (Bitmap)b.Clone())
            {
                // Iterate over every pixel
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Adds a simple wave
                        int newX = (int)(x + (distortion * Math.Sin(Math.PI * y / 64.0)));
                        int newY = (int)(y + (distortion * Math.Cos(Math.PI * x / 64.0)));
                        if (newX < 0 || newX >= width) newX = 0;
                        if (newY < 0 || newY >= height) newY = 0;
                        b.SetPixel(x, y, copy.GetPixel(newX, newY));
                    }
                }

            }
        }

        /// <summary>
        /// 生成指定长度的随机字符串
        /// </summary>
        /// <param name="count">字符串长度</param>
        /// <returns></returns>
        private static string RandomCode(int? count)
        {
            Random rand = new Random((int)DateTime.Now.Ticks / 20000);
            String allChar = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Char[] allCharArray = allChar.ToCharArray();

            String randomCode = "";

            for (int i = 0; i < count; i++)
            {
                int t = rand.Next(35);
                randomCode = String.Concat(randomCode, Char.ToString(allCharArray[t]));
            }
            return randomCode;
        }
    }
}
