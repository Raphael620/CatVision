using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatVision.Common.Const
{
    public class UserType
    {
        public const string Developer = "开发者";
        public const string Administrator = "管理员";
        public const string Operator = "操作员";

        public const string Developer_en = "Developer";
        public const string Administrator_en = "Administrator";
        public const string Operator_en = "Operator";

        public static List<string> UserTypes { get; } = new List<string>
        {
            Developer, Administrator, Operator
        };
        public static List<string> UserTypes_en { get; } = new List<string>
        {
            Developer_en, Administrator_en, Operator_en
        };
    }
}
