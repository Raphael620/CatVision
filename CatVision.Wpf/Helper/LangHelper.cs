using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLocalizeExtension;
using WPFLocalizeExtension.Extensions;

namespace CatVision.Wpf.Helper
{
    public static class LangHelper
    {
        /// <summary>
        /// 根据关键字获取当前语言环境下的提示信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resourceFileName"></param>
        /// <param name="addSpaceAfter"></param>
        /// <returns></returns>
        public static string GetLocalizedString(string key, string resourceFileName = "Langs", bool addSpaceAfter = false)
        {
            var localizedString = String.Empty;

            // Build up the fully-qualified name of the key
            var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            var fullKey = assemblyName + ":" + resourceFileName + ":" + key;
            var locExtension = new LocExtension(fullKey);
            locExtension.ResolveLocalizedValue(out localizedString);

            // Add a space to the end, if requested
            if (addSpaceAfter)
            {
                localizedString += " ";
            }

            return localizedString;
        }
    }
}
