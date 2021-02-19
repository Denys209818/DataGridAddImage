using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BlogForm.Service
{
    public static class BitmapCreater
    {
        public static Bitmap ResizeImage(Image firstImage, int width, int height) 
        {
            int maxWidth = width;
            int maxHeight = height;
            bool doWidthResize = (width > 0 && firstImage.Width > width && 
                firstImage.Width - width > firstImage.Height - height);
            bool doHeightResize = (height > 0 && firstImage.Height > height &&
                firstImage.Height - height > firstImage.Width - width);
            if (doWidthResize || doHeightResize)
            {
                try
                {
                    int divider;
                    if (doWidthResize)
                    {
                        divider = (int)Math.Abs((Decimal)firstImage.Width / width);
                        maxWidth = width;
                        maxHeight = (int)Math.Round((Decimal)firstImage.Height / divider);
                    }
                    else
                    {
                        divider = (int)Math.Abs((Decimal)firstImage.Height / height);
                        maxHeight = height;
                        maxWidth = (int)Math.Round((Decimal)firstImage.Width / divider);
                    }
                    //  Створено нове зобрадення з новими розмірами і PixelFormat
                    using (Bitmap newB = new Bitmap(maxWidth, maxHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                    {
                        //  Свторення графчного класу, який малює на фотографії newB
                        using (Graphics graph = Graphics.FromImage(newB))
                        {
                            //  Метод, що малює зображення у newB, взяте у firstImage ,у початкових координатах,
                            //  з вказаними висотою і шириною
                            graph.DrawImage(firstImage, 0, 0, maxWidth, maxHeight);
                            //  Повертає новий Bitmap
                            return new Bitmap(newB);
                        }
                    }
                }
                catch 
                {
                    return null;
                }
            
            } else 
            {
                return null;
            }
        }
    }
}
