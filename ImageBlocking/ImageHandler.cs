using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageBlocking
{
    public class ImageHandler
    {
        public Bitmap HandleImage(Bitmap image, Size targetSize, IEnumerable<Color> availableColors, bool changeColor)
        {
            Bitmap newImage = new Bitmap(image, targetSize);
            if (changeColor)
            {
                for (int ix = 0; ix < newImage.Width; ix++)
                {
                    for (int iy = 0; iy < newImage.Height; iy++)
                    {
                        var originalColor = newImage.GetPixel(ix, iy);

                        newImage.SetPixel(ix, iy, FindClosestColor(originalColor, availableColors));
                    }
                }
            }
            return newImage;
        }

        public Color FindClosestColor(Color c, IEnumerable<Color> colors)
        {
            Color best = Color.Black;
            var ib = Math.Abs(best.ToArgb());
            foreach (var t in colors)
            {
                var it = Math.Abs(t.ToArgb());
                var ic = Math.Abs(c.ToArgb());

                if (Math.Abs(it - ic) < Math.Abs(ib - ic))
                {
                    best = t;
                    ib = it;
                }
            }
            if (c.Name != "ffffffff")
            {
                Console.WriteLine($"the original color is {c.Name}|{c.ToArgb()}, the closest color available is {best.Name}|{best.ToArgb()}");
            }

            return best;
        }

        public Bitmap TestColor()
        {
            int step = 20;
            int width = (int)Math.Ceiling(Math.Sqrt((255 / step) * (255 / step) * (255 / step)));
            Bitmap image = new Bitmap(width, width);
            int ix = 0; int iy = 0;
            for (int ir = 0; ir < 255; ir = ir + step)
            {
                for (int ig = 0; ig < 255; ig = ig + step)
                {
                    for (int ib = 0; ib < 255; ib = ib + step)
                    {
                        image.SetPixel(ix, iy, Color.FromArgb(ir, ig, ib));
                        ix++;
                        if (ix >= image.Width)
                        {
                            ix = 0;
                            iy++;
                            if (iy >= image.Height)
                            {
                                return image;
                            }
                        }
                    }
                }
            }

            return image;
        }

        public Bitmap TestColor2()
        {
            var colors = typeof(Color).GetProperties();
            int width = (int)Math.Ceiling(Math.Sqrt(colors.Length));
            Bitmap image = new Bitmap(width, width);
            int i = -1;
            for (int iy = 0; iy < width; iy++)
            {
                for (int ix = 0; ix < width; ix++)
                {
                    i++;
                    if (i >= colors.Length)
                    {
                        return image;
                    }

                    if (colors[i].PropertyType != typeof(Color))
                    {
                        continue;
                    }

                    var x = colors[i].GetValue(null);
                    if (x is Color c)
                    {
                        image.SetPixel(ix, iy, c);
                    }
                }
            }

            return image;
        }
    }
}
