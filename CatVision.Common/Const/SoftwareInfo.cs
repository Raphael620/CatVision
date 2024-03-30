using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatVision.Common.Const
{
    public static class SoftwareInfo
    {
        public static string SoftwareName { get; } = "CatVision";
        /// <summary>
        /// 版本
        /// </summary>
        public static string SoftwareVersion { get; } = "0.0.1";
        /// <summary>
        /// 认证
        /// </summary>
        public static string Authentication { get; } = "xxxx";
        /// <summary>
        /// 授权
        /// </summary>
        public static string Authorization { get; } = "Everything";
        public static string FullNameVersion { get; } = string.Format(@"{0} V{1}", SoftwareName, SoftwareVersion);
    }
}
