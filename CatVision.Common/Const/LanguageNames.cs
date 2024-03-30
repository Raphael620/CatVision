using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatVision.Common.Const
{
    public static class LanguageNames
    {
        public const string Chinese = "zh";
        public const string English = "en";
        public static List<CultureInfo> AvailableCultureInfos { get; } = new List<CultureInfo>
        {
            new CultureInfo("en"),
            new CultureInfo("zh"),
        };

    }
}
