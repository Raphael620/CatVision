using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLocalizeExtension;
using WPFLocalizeExtension.Extensions;

namespace CatVision.Wpf.Localization
{
    public class LangHelper
    {
        // TODO resourceName
        public static string GetLocStr(string key, string resourceFileName = "Langs", bool addSpaceAfter = false)
        {
            string res = string.Empty;
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string fullKey = $"{assemblyName}:{resourceFileName}:{key}";
            LocExtension locExtension = new LocExtension(fullKey);
            locExtension.ResolveLocalizedValue(out res);
            if (addSpaceAfter) res += " ";
            return res;
        }
    }
}
