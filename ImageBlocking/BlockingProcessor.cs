using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using ImageBlocking.Models;

namespace ImageBlocking
{
    public class BlockingProcessor
    {
        public Solution Blocking(Bitmap image, Inventory blocks)
        {
            var solution = new Solution { Image = image, Size= image.Size };
            for (int iy = 0; iy < image.Height; iy++)
            {
                for (int ix = 0; ix < image.Width; )
                {
                    var item = solution.GetSolutionItem(ix, iy);
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

                var item = solution.GetSolutionItem(x, y);
                if (item != null)
                {
                    return GetNextPosition(image, solution,x, y, item);
                }
            }

            return (x, y);

        }

        private (Block, bool) FindBlock(Bitmap image, Solution solution, int x, int y, Inventory blocks)
        {
            // sort by block size, put biggest block first
            // TODO: consider inventory
            var validBlocks = blocks.Items.Where(b => b.Block.Color.ToArgb() == image.GetPixel(x, y).ToArgb() && b.Qty > 0);
            if (validBlocks?.Any() != true)
            {
                return (null, false);
            }

            // try put block on picture
            List<bool> rotates = new List<bool> { false, true };
            foreach (var block in validBlocks.OrderByDescending(b => b.Block.Size.Width * b.Block.Size.Width * b.Block.Size.Height))
            {
                foreach (var rotate in rotates)
                {
                    if (solution.TestPut(x, y, block.Block, rotate))
                    {
                        block.Qty--;
                        return (block.Block, rotate);
                    }
                }
            }

            return (null, false);
        }
    }
}
