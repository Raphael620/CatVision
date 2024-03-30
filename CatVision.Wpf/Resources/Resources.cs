using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatVision.Wpf
{
    static class Resources
    {
        static readonly string Prefix = typeof(Resources).FullName + ".";
        public static Stream OpenStream(string name)
        {
            Stream s = typeof(Resources).Assembly.GetManifestResourceStream(Prefix + name);
            if (s == null)
                throw new FileNotFoundException("The resource file '" + name + "' was not found.");
            return s;
        }
    }
}
