using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using ImageBlocking.Models;

namespace ImageBlocking
{
    public class BlockingProcessor
    {
        public Solution Blocking(Bitmap image, IEnumerable<Block> blocks)
        {
            var solution = new Solution {Size= image.Size };
            for (int iy = 0; iy < image.Height; iy++)
            {
                for (int ix = 0; ix < image.Width; )
                {
                    var item = solution.Get(ix, iy);
                    if (item == null)
                    {
                        (Block b, bool r) = FindBlock(image, solution, ix, iy, blocks);
                        if (b == null)
                        {
                            ix++;
                        }
                        else
                        {
                            item = solution.Put(ix, iy, b, r);
                        }
                    }

                    if (item == null)
                    {
                        ix++;
                    }
                    else
                    {
                        (ix, iy) = GetNextPosition(image, solution, ix, iy, item);
                        Debug.WriteLine($"next position {ix}:{iy}");
                    }

                    if (ix >= image.Width || iy >= image.Height)
                    {
                        return solution;
                    }
                }
            }
            // get solution
            return solution;
        }

        private (int, int) GetNextPosition(Bitmap image, Solution solution, int fromX, int fromY, SolutionItem fromItem)
        {
            int x = 0;
            int y = 0;

            if (fromItem == null)
            {
                return (x, y);
            }

            x = fromX + 1;
            y = fromY;
            if (x >= image.Width)
            {
                x = 0;
                y++;

                var item = solution.Get(x, y);
                if (item != null)
                {
                    return GetNextPosition(image, solution,x, y, item);
                }
            }

            return (x, y);

        }

        private (Block, bool) FindBlock(Bitmap image, Solution solution, int x, int y, IEnumerable<Block> blocks)
        {
            // sort by block size, put biggest block first
            var validBlocks = blocks.Where(b => b.Color.ToArgb() == image.GetPixel(x, y).ToArgb());
            if (validBlocks?.Any() != true)
            {
                return (null, false);
            }

            // try put block on picture
            List<bool> rotates = new List<bool> { false, true };
            foreach (var block in validBlocks.OrderBy(b => b.Size.Width * b.Size.Width * b.Size.Height))
            {
                foreach (var rotate in rotates)
                {
                    if (solution.TestPut(x, y, block, rotate))
                    {
                        return (block, rotate);
                    }
                }
            }

            // TODO: consider inventory

            return (null, false);
        }
    }
}
