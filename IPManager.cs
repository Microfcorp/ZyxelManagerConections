using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zyxel
{
    class IPManager
    {
        public static bool IsLANIP(string ip)
        {
            var k = ip.Split('.');
            if (k[0] == "192" & k[1] == "168")
                return true;
            return false;
        }
    }
}
