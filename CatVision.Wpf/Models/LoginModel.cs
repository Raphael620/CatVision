using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatVision.Common.Model;
using CatVision.Common.Helper;

namespace CatVision.Wpf.Models
{
    public class LoginModel
    {
        private static LoginModel instance = new LoginModel();
        public static LoginModel Ins => instance;
        public UserModel CurrentUser;
        public static List<UserModel> UserList = new List<UserModel>();
        public static bool LoginFlag = false;
        public bool AddOperator(string name, string pwd)
        {
            if (UserList.Select(x => x.UserName).Contains(name)) return false;
            else UserList.Add(new UserModel(3, name, pwd));
            if (CurrentUser is null) CurrentUser = UserList[UserList.Count];
            return true;
        }
        public bool AddUser(uint level, string name, string pwd)
        {
            if (level >= (uint)UserModel.GetLevel(CurrentUser.Userlevel)) return false;
            else if (UserList.Select(x => x.UserName).Contains(name)) return false;
            else UserList.Add(new UserModel(level, name, pwd));
            return true;
        }
        public bool CheckPwd(string name, string pwd)
        {
            if (!UserList.Select(x => x.UserName).Contains(name)) return false;
            if (UserList.Find(x => x.UserName == name).UserPwd == pwd) return true;
            return false;
        }
    }
}
