using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
            foreach (var t in colors)
            {
                if (CompareColorDiff(c, best, t) > 0)
                {
                    best = t;
                }
            }

            return best;
        }

        /// <summary>
        /// compare diff between c1-target and c2-target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>1:c2 is closer; -1:c1 is closer; 0: very equal</returns>
        private int CompareColorDiff(Color target, Color c1, Color c2)
        {
            // compare r+g+b
            var rdiff1 = Math.Abs(c1.R - target.R);
            var gdiff1 = Math.Abs(c1.G - target.G);
            var bdiff1 = Math.Abs(c1.B - target.B);
            var rdiff2 = Math.Abs(c2.R - target.R);
            var gdiff2 = Math.Abs(c2.G - target.G);
            var bdiff2 = Math.Abs(c2.B - target.B);
            if ((rdiff1 + gdiff1 + bdiff1) > (rdiff2 + gdiff2 + bdiff2))
                return 1;
            else if ((rdiff1 + gdiff1 + bdiff1) < (rdiff2 + gdiff2 + bdiff2))
                return -1;
            
            // compare a
            var adiff1 = Math.Abs(c1.A - target.A);
            var adiff2 = Math.Abs(c2.A - target.A);
            if (adiff1 > adiff2)
                return 1;
            else if (adiff1 < adiff2)
                return -1;

            // compare variance
            var variance1 = Variance(c1.R, c1.G, c1.B);
            var variance2 = Variance(c2.R, c2.G, c2.B);
            if (variance1 > variance2)
                return 1;
            else if (variance1 < variance2)
                return -1;

            return 0;
        }

        private double Variance(params double[] nums)
        {
            var avaerage = nums.Sum() / nums.Length;
            double up = 0;
            foreach (var x in nums)
            {
                up += Math.Pow(x - avaerage, 2);
            }
            return up / nums.Length;
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
