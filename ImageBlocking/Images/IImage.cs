using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlocking.Images
{
    public interface IImage
    {
        Bitmap OpenImage(string path);
    }
}
