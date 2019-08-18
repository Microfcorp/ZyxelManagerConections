using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zyxel
{
    class FileSize
    {
        public static string Calculate(decimal bytesize)
        {
            string[] l = { "B", "KB", "MB", "GB", "TB", "PB" };
            int pos = 0;
            while (bytesize >= 1024) {
                bytesize /= 1024;
                pos++;
            }
            return Math.Round(bytesize, 2) + " " + l[pos];
        }
        public static string Calculate(string bytesize)
        {
            return Calculate(int.Parse(bytesize));
        }
    }
}
