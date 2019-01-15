using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ImageBlocking;
using ImageBlocking.Images;
using ImageBlocking.Models;

namespace WinFormTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            txtWidth.Text = "64";
            txtHeight.Text = "64";
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var colors = typeof(Color).GetProperties().Where(x => x.PropertyType == typeof(Color)).Select(x => (Color)x.GetValue(null));
            var config = new
            {
                targetSize = new Size(Convert.ToInt32(txtWidth.Text.Trim()), Convert.ToInt32(txtHeight.Text.Trim())),
                //blocks = new List<Block> {
                //    new Block { Color = Color.Yellow, Size = new Size(2,2) },
                //    new Block { Color = Color.Red, Size = new Size(2,2) },
                //    new Block { Color = Color.Black, Size = new Size(2,2) },
                //    new Block { Color = Color.White, Size = new Size(2,2) },
                //    new Block { Color = Color.Green, Size = new Size(2,2) },
                //    new Block { Color = Color.Pink, Size = new Size(2,2) },
                //}
                blocks = colors.Select(x => new Block { Color = x, Size = new Size(2, 2) })
            };
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
            var handledImageData = handler.HandleImage(imageData, config.targetSize, config.blocks.Select(x => x.Color), chkChangeColor.Checked);
            ShowImage(handledImageData, picProcessed);
            sw.Stop();
            Debug.WriteLine($"resize took {sw.ElapsedMilliseconds}ms");
            sw.Reset();
            sw.Start();
            // blocking
            var processor = new BlockingProcessor();
            var blockingResult = processor.Blocking(handledImageData, config.blocks);
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

        private Bitmap DrawResult(Solution solution)
        {
            int ratio = 10;
            Bitmap image = new Bitmap(solution.Size.Width * (ratio + 1), solution.Size.Height * (ratio + 1));

            foreach (var item in solution.Items)
            {
                for (int x = item.Left * (ratio + 1); x < (item.Right + 1) * (ratio + 1) - 1; x++)
                {
                    for (int y = item.Top * (ratio + 1); y < (item.Bottom + 1) * (ratio + 1) - 1; y++)
                    {
                        image.SetPixel(x, y, item.Block.Color);
                    }
                }
                if ((item.Right + 1) * (ratio + 1) - 1 < image.Width)
                {
                    for (int i = item.Top * (ratio + 1); i < (item.Bottom + 1) * (ratio + 1); i++)
                    {
                        image.SetPixel((item.Right + 1) * (ratio + 1) - 1, i, Color.Black);
                    }
                }
                if ((item.Bottom + 1) * (ratio + 1) - 1 < image.Height)
                {
                    for (int i = item.Left * (ratio + 1); i < (item.Right + 1) * (ratio + 1); i++)
                    {
                        image.SetPixel(i, (item.Bottom + 1) * (ratio + 1) - 1, Color.Black);
                    }
                }
            }

            return image;
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
