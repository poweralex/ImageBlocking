using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlocking.Images
{
    class JpgImage : IImage
    {
        public Bitmap OpenImage(string path)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(path, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }
    }
}
