using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatVision.Common.Helper
{
    public class StringHelper
    {
        public static string Dump<T>(IList<T> v)
        {
            StringBuilder sb = new StringBuilder("Debug: ");
            sb.Append(nameof(v));
            sb.Append(": ");
            if (null == v) sb.Append("Null.");
            else if (v.Count == 0) sb.Append("Nothing.");
            else
            {
                for (int i = 0; i < v.Count; i++)
                {
                    // todo: T is IFormattable?
                    sb.Append(v[i].ToString());
                    sb.Append(";");
                }
            }
            return sb.ToString();
        }
    }
}
