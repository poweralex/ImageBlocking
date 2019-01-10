using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                blocks = colors.Select(x => new Block { Color = x, Size = new Size(2,2) })
            };

            // read file
            var image = new JpgImage();
            var imageData = image.OpenImage(txtFile.Text.Trim());
            ShowImage(imageData, picOriginal);
            // resize
            ImageHandler handler = new ImageHandler();
            //ShowImage(handler.TestColor2(), picResult);
            //return;

            var handledImageData = handler.HandleImage(imageData, config.targetSize, config.blocks.Select(x => x.Color), chkChangeColor.Checked);
            ShowImage(handledImageData, picResult);
            return;
            // blocking
            var processor = new BlockingProcessor();
            var blockingResult = processor.Blocking(handledImageData, config.blocks);
            // show result
            SHowResult(blockingResult);
        }

        private void ShowImage(Bitmap image, PictureBox pic)
        {
            pic.Image = image;
        }

        private void SHowResult(object o)
        { }

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
