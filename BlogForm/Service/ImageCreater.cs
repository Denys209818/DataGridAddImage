using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BlogForm.Service
{
    public static class ImageCreater
    {
        public static Bitmap CreateImage(Image image, int newWidth, int newHeight) 
        {
            int maxHeight = newHeight;
            int maxWidth = newWidth;
            bool doWidthResize = (newWidth > 0 && image.Width > newWidth &&
                image.Width - newWidth > image.Height - newHeight);
            bool doHeightResize = (newHeight > 0 && image.Height > newHeight &&
                image.Height - newHeight > image.Width - newWidth);
            if (doWidthResize || doHeightResize)
            {
                try
                {
                    Decimal divider;
                    if (doWidthResize)
                    {
                        divider = Math.Abs((Decimal)image.Width / newWidth);
                        maxWidth = newWidth;
                        maxHeight = (int)Math.Round((Decimal)image.Height / divider);
                    }
                    else 
                    {
                        divider = Math.Abs((Decimal)image.Height / newHeight);
                        maxHeight = newHeight;
                        maxWidth = (int)Math.Round((Decimal)image.Width / newWidth);
                    }

                    using (Bitmap bmp = new Bitmap(maxWidth, maxHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)) 
                    {
                        using (Graphics graph = Graphics.FromImage(bmp)) 
                        {
                            graph.DrawImage(image, 0, 0, maxWidth, maxHeight);
                            return new Bitmap(bmp);
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            else 
            {
                return null; 
            }
        }
    }
}
