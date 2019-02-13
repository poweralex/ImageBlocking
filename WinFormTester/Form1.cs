using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ImageBlocking;
using ImageBlocking.Images;
using ImageBlocking.Models;
using Newtonsoft.Json;

namespace WinFormTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            txtWidth.Text = "120";
            txtHeight.Text = "120";
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            //var pseudoResult = new Solution
            //{
            //    Size = new Size(4, 2),
            //    SolutionItems = new List<SolutionItem>
            //    {
            //        new SolutionItem{ Block = new Block{ Color = Color.Red, Size = new Size(2,1) }, Position = new Position{ Left = 0, Top = 0 }, Rotate = false },
            //        new SolutionItem{ Block = new Block{ Color = Color.Green, Size =new Size(1,1) }, Position = new Position{ Left = 2, Top = 0 }, Rotate = false },
            //        new SolutionItem{ Block = new Block{ Color = Color.Yellow, Size = new Size(2,1)}, Position = new Position{ Left = 3, Top = 0 }, Rotate = true },
            //        new SolutionItem{ Block = new Block{ Color = Color.Green, Size =new Size(1,1) }, Position = new Position{ Left = 0, Top = 1 }, Rotate = false },
            //        new SolutionItem{ Block = new Block{ Color = Color.Blue, Size = new Size(2,1) }, Position = new Position{ Left = 1, Top = 1  }, Rotate = false }
            //    }
            //};
            //ShowImage(DrawResult(pseudoResult), picResult);
            //return;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string filename = "inventory.json";
            var config = new Config
            {
                Size = new Size(Convert.ToInt32(txtWidth.Text.Trim()), Convert.ToInt32(txtHeight.Text.Trim()))
            };
            if (File.Exists(filename))
            {
                var str = File.ReadAllText(filename);
                var inv = JsonConvert.DeserializeObject<Inventory>(str);
                config.Inventory = inv;
            }
            else
            {
                var colors = typeof(Color).GetProperties().Where(x => x.PropertyType == typeof(Color)).Select(x => (Color)x.GetValue(null));
                var inventory = new Inventory();
                inventory.Items.AddRange(colors.Select(x => new InventoryUnit { Block = new Block { Color = x, Size = new Size(4, 2) }, Qty = int.MaxValue }));
                inventory.Items.AddRange(colors.Select(x => new InventoryUnit { Block = new Block { Color = x, Size = new Size(2, 2) }, Qty = int.MaxValue }));
                config.Inventory = inventory;
            }
            sw.Stop();
            Debug.WriteLine($"prepare data took {sw.ElapsedMilliseconds}ms.");
            sw.Reset();
            sw.Start();
            // read file
            var image = new JpgImage();
            var imageData = image.OpenImage(txtFile.Text.Trim());
            ShowImage(imageData, picOriginal);
            sw.Stop();
            Debug.WriteLine($"read image took {sw.ElapsedMilliseconds}ms");
            sw.Reset();
            sw.Start();
            // resize
            ImageHandler handler = new ImageHandler();
            var handledImageData = handler.HandleImage(imageData, config.Size, config.Inventory.Items.Select(x => x.Block.Color), chkChangeColor.Checked);
            ShowImage(handledImageData, picProcessed);
            sw.Stop();
            Debug.WriteLine($"resize took {sw.ElapsedMilliseconds}ms");
            sw.Reset();
            sw.Start();
            // blocking
            var processor = new BlockingProcessor();
            var blockingResult = processor.Blocking(handledImageData, config.Inventory);
            sw.Stop();
            Debug.WriteLine($"blocking took {sw.ElapsedMilliseconds}ms");
            sw.Reset();
            sw.Start();
            // show result
            ShowImage(DrawResult(blockingResult), picResult);
            sw.Stop();
            Debug.WriteLine($"show result took {sw.ElapsedMilliseconds}ms");
        }

        private void ShowImage(Bitmap image, PictureBox pic)
        {
            Debug.WriteLine($"showing image({image.Width}x{image.Height}) on {pic.Name}");
            pic.Image = image;
        }

        private int ratio = 10;
        private Bitmap DrawResult(Solution solution)
        {
            Bitmap image = new Bitmap(solution.Size.Width * (ratio), solution.Size.Height * (ratio));

            foreach (var item in solution.SolutionItems)
            {
                //for (int x = item.Left * (ratio + 1); x < (item.Right + 1) * (ratio + 1) - 1; x++)
                //{
                //    for (int y = item.Top * (ratio + 1); y < (item.Bottom + 1) * (ratio + 1) - 1; y++)
                //    {
                //        image.SetPixel(x, y, item.Block.Color);
                //    }
                //}
                //if ((item.Right + 1) * (ratio + 1) - 1 < image.Width)
                //{
                //    for (int i = item.Top * (ratio + 1); i < (item.Bottom + 1) * (ratio + 1); i++)
                //    {
                //        image.SetPixel((item.Right + 1) * (ratio + 1) - 1, i, Color.Black);
                //    }
                //}
                //if ((item.Bottom + 1) * (ratio + 1) - 1 < image.Height)
                //{
                //    for (int i = item.Left * (ratio + 1); i < (item.Right + 1) * (ratio + 1); i++)
                //    {
                //        image.SetPixel(i, (item.Bottom + 1) * (ratio + 1) - 1, Color.Black);
                //    }
                //}
                DrawOneBlock(image, item.Left, item.Top, item.Block, item.Rotate);
            }

            return image;
        }

        private Bitmap DrawOneBlock(Bitmap map, int left, int top, Block block, bool rotate)
        {
            var width = rotate ? block.Size.Height : block.Size.Width;
            var height = rotate ? block.Size.Width : block.Size.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    DrawOneUnit(map, left + x, top + y, block.Color, x == width - 1 ? Color.Black : block.Color, y == height - 1 ? Color.Black : block.Color);
                }
            }
            return map;
        }
        private Bitmap DrawOneUnit(Bitmap map, int blockX, int blockY, Color c, Color lineRightC, Color lineBottomC)
        {
            for (int x = 0; x < ratio; x++)
            {
                for (int y = 0; y < ratio; y++)
                {
                    if (x == ratio - 1)
                    {
                        map.SetPixel(x + (blockX * ratio), y + (blockY * ratio), lineRightC);
                    }
                    else if (y == ratio - 1)
                    {
                        map.SetPixel(x + (blockX * ratio), y + (blockY * ratio), lineBottomC);
                    }
                    else
                    {
                        map.SetPixel(x + (blockX * ratio), y + (blockY * ratio), c);
                    }
                }
            }
            return map;
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = "jpg";
            open.RestoreDirectory = true;
            if (open.ShowDialog() == DialogResult.OK)
            {
                this.txtFile.Text = open.FileName;
            }
        }
    }
}
