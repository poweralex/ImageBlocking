using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace ImageBlocking.Models
{
    public class Solution
    {
        public Size Size { get; set; }
        public List<SolutionItem> Items { get; set; } = new List<SolutionItem>();

        public SolutionItem Put(int x, int y, Block b, bool rotate)
        {
            if (Items == null)
            {
                Items = new List<SolutionItem>();
            }
            var item = new SolutionItem
            {
                Position = new Position { Left = x, Top = y },
                Block = b,
                Rotate = rotate
            };
            if (item.Right < Size.Width && item.Bottom < Size.Height)
            {
                Items.Add(item);
                Debug.WriteLine($"put {Items.Count()} items");

                return item;
            }
            else
            {
                return null;
            }
        }

        public bool TestPut(int x, int y, Block b, bool rotate)
        {
            for (int iy = y; iy < y+b.Size.Height; iy++)
            {
                for (int ix = x; ix < x+b.Size.Width; ix++)
                {
                    if (Get(ix, iy) != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public SolutionItem Get(int x, int y)
        {
            var item = Items.FirstOrDefault(i => i.Position.Left == x && i.Position.Top == y);
            if (item != null)
            {
                return item;
            }

            item = Items.FirstOrDefault(i => x >= i.Left && x <= i.Right && y >= i.Top && y <= i.Bottom);
            if (item != null)
            {
                return item;
            }

            return null;
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
