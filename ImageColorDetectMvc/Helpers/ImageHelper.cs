using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;

namespace ImageColorDetectMvc.Helpers
{
    public class ImageHelper
    {
        /// <summary>
        /// Get image with colors
        /// </summary>
        /// <param name="imageUrl">Example:http://dribbble.com/system/users/28212/screenshots/421638/jsos.png?1328971781</param>
        /// <returns></returns>
        public static byte[] GetResultImg(string imageUrl)
        {
            const int height = 400;

            using (var img = GetImgFromUrl(imageUrl))
            {
                return
                    FromBitmapToByte(DrawResultPicture(ResizeImage(img, height),
                                                       DrawColorsBlock(GetColors(img), 200, height),
                                                       height));
            }
        }

        private static Bitmap GetImgFromUrl(string url)
        {
            var myReq = (HttpWebRequest)WebRequest.Create(url);

            using (WebResponse myResp = myReq.GetResponse())
            {
                Stream stream = myResp.GetResponseStream();

                if (stream == null)
                {
                    throw new NullReferenceException("Cannot get image by given url");
                }

                return new Bitmap(stream);
            }
        }

        private static List<Color> GetColors(Bitmap img)
        {
            img = ResizeImage(img, 400);

            const int pixelStep = 10;

            var colors = new List<Color>();

            for (int x = 0; x < img.Width; x = x + pixelStep)
            {
                for (int y = 0; y < img.Height; y = y + pixelStep)
                {
                    colors.Add(img.GetPixel(x, y));
                }
            }

            var countedColorList = new Dictionary<Color, int>();

            foreach (Color color in colors)
            {
                if (!countedColorList.ContainsKey(color))
                {
                    int occurrenceCount = 0;

                    foreach (Color color2 in colors)
                    {
                        if (color2 == color)
                        {
                            occurrenceCount++;
                        }
                        else
                        {
                            if (GetEucledianDistance(color, color2) < 15)
                            {
                                occurrenceCount++;
                            }
                        }
                    }

                    countedColorList.Add(color, occurrenceCount);
                }
            }

            Dictionary<Color, int> orderedColors = countedColorList.OrderByDescending(v => v.Value).ToDictionary(
                d => d.Key, d => d.Value);

            var resultColors = new List<Color>();

            foreach (var keyValuePair in orderedColors)
            {
                //only 10 first most relevant colors
                if (resultColors.Count == 10)
                {
                    break;
                }


                if (resultColors.Count > 0)
                {
                    bool result = true;

                    foreach (Color color in resultColors)
                    {
                        if (GetEucledianDistance(color, keyValuePair.Key) < 30)
                        {
                            result = false;
                        }
                    }

                    if (result)
                    {
                        resultColors.Add(keyValuePair.Key);
                    }
                }
                else
                {
                    resultColors.Add(keyValuePair.Key);
                }
            }

            return resultColors;
        }

        private static byte[] FromBitmapToByte(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);

                return ms.GetBuffer();
            }
        }

        private static Bitmap ResizeImage(Bitmap img, int newHeight)
        {
            int origWidth = img.Width;
            int origHeight = img.Height;

            int newWidth = (((origWidth * 100) / origHeight) * newHeight) / 100;

            var newBitmap = new Bitmap(newWidth, newHeight);

            Graphics oGraphics = Graphics.FromImage(newBitmap);

            oGraphics.CompositingQuality = CompositingQuality.HighQuality;
            oGraphics.SmoothingMode = SmoothingMode.HighQuality;
            oGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            oGraphics.DrawImage(img, new Rectangle(0, 0, newWidth, newHeight));

            return newBitmap;
        }

        private static Bitmap DrawResultPicture(Bitmap img, Bitmap colorsPalitra, int height)
        {
            const int borderWidth = 4;

            int realHeight = height + borderWidth * 2;
            int realWidth = (img.Width + colorsPalitra.Width) + borderWidth * 3;

            var newBitmap = new Bitmap(realWidth, realHeight);
            Graphics oGraphics = Graphics.FromImage(newBitmap);

            oGraphics.FillRectangle(new SolidBrush(Color.White), 0, 0, newBitmap.Width, newBitmap.Height);

            oGraphics.DrawImage(img, new Rectangle(borderWidth, borderWidth, img.Width, img.Height));
            oGraphics.DrawImage(colorsPalitra,
                                new Rectangle(img.Width + borderWidth * 2, borderWidth, colorsPalitra.Width,
                                              colorsPalitra.Height));

            return newBitmap;
        }

        private static Bitmap DrawColorsBlock(List<Color> colors, int width, int height)
        {
            var newBitmap = new Bitmap(width, height);
            Graphics oGraphics = Graphics.FromImage(newBitmap);
            int lineHeight = height / colors.Count;

            for (int i = 0; i < colors.Count; i++)
            {
                oGraphics.FillRectangle(new SolidBrush(colors[i]), 0, i * lineHeight, width, lineHeight);
            }

            return newBitmap;
        }

        private static double GetEucledianDistance(Color color1, Color color2)
        {
            return Math.Sqrt(Math.Pow(Convert.ToDouble(color1.R) - Convert.ToDouble(color2.R), 2.0) +
                             Math.Pow(Convert.ToDouble(color1.B) - Convert.ToDouble(color2.B), 2.0) +
                             Math.Pow(Convert.ToDouble(color1.G) - Convert.ToDouble(color2.G), 2.0));
        }
    }
}