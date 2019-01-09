using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            var config = new
            {
                targetSize = new Size(64, 64),
                blocks = new List<object> {
                    new { color = Color.Yellow, size = new Size(2,2) },
                    new { color = Color.Red, size = new Size(2,2) },
                    new { color = Color.Black, size = new Size(2,2) },
                    new { color = Color.White, size = new Size(2,2) },
                }
            };
            // read file
            // resize
            // blocking
            // show result
        }

        private void ShowImage(Bitmap image)
        {
        }
    }
}
