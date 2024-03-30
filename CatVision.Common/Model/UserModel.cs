using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CatVision.Common.Enum;
using CatVision.Common.Helper;

namespace CatVision.Common.Model
{
    [Serializable]
    public class UserModel
    {
        /// <summary>
        /// 账号等级 : UserLevel
        /// </summary>
        public string Userlevel { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public UserModel() { }
        public UserModel(string level, string name, string pwd)
        {
            Userlevel = level; UserName = name; UserPwd = pwd;
        }
        public UserModel(uint level, string name, string pwd)
        {
            Userlevel = ((UserLevel)level).ToString(); UserName = name; UserPwd = pwd;
        }
        public static UserLevel GetLevel(string level)
        {
            return (UserLevel)System.Enum.Parse(typeof(UserLevel), level);
        }
    }
}
