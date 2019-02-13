using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace ImageBlocking.Models
{
    public class Solution
    {
        public Bitmap Image { get; set; }
        public Size Size { get; set; }
        public List<SolutionItem> SolutionItems { get; set; } = new List<SolutionItem>();

        public SolutionItem Put(int x, int y, Block b, bool rotate)
        {
            if (SolutionItems == null)
            {
                SolutionItems = new List<SolutionItem>();
            }
            var item = new SolutionItem
            {
                Position = new Position { Left = x, Top = y },
                Block = b,
                Rotate = rotate
            };
            if (item.Right < Size.Width && item.Bottom < Size.Height)
            {
                SolutionItems.Add(item);
                Debug.WriteLine($"put {SolutionItems.Count()} items");

                return item;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// test if position(x,y) can put a block with rotate
        /// </summary>
        /// <param name="x">position x</param>
        /// <param name="y">position y</param>
        /// <param name="b">block</param>
        /// <param name="rotate">rotate</param>
        /// <returns>true or false</returns>
        public bool TestPut(int x, int y, Block b, bool rotate)
        {
            var blockWidth = rotate ? b.Size.Height : b.Size.Width;
            var blockHeight = rotate ? b.Size.Width : b.Size.Height;
            for (int iy = y; iy < y + blockHeight; iy++)
            {
                for (int ix = x; ix < x + blockWidth; ix++)
                {
                    if (GetSolutionItem(ix, iy) != null)
                    {
                        return false;
                    }

                    if (GetColor(ix, iy).ToArgb() != b.Color.ToArgb())
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public SolutionItem GetSolutionItem(int x, int y)
        {
            var item = SolutionItems.FirstOrDefault(i => i.Position.Left == x && i.Position.Top == y);
            if (item != null)
            {
                return item;
            }

            item = SolutionItems.FirstOrDefault(i => x >= i.Left && x <= i.Right && y >= i.Top && y <= i.Bottom);
            if (item != null)
            {
                return item;
            }

            return null;
        }

        public Color GetColor(int x, int y)
        {
            if (x >= Image.Width || y >= Image.Height)
            {
                return default(Color);
            }
            return Image.GetPixel(x, y);
        }
    }

    public class SolutionItem
    {
        public Position Position { get; set; }
        public Block Block { get; set; }
        public bool Rotate { get; set; }

        public int Width { get { return Rotate ? Block.Size.Height : Block.Size.Width; } }
        public int Height { get { return Rotate ? Block.Size.Width : Block.Size.Height; } }
        public int Top { get { return Position.Top; } }
        public int Left { get { return Position.Left; } }
        public int Right { get { return Position.Left + Width - 1; } }
        public int Bottom { get { return Position.Top + Height - 1; } }
    }

    public class Position
    {
        public int Left { get; set; }
        public int Top { get; set; }
    }
}
