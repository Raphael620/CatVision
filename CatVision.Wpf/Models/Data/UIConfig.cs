using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CatVision.Common.Const;
using CatVision.Common.Helper;

namespace CatVision.Wpf.Models.Data
{
    public static class ThemeConfig
    {
        public static bool DarkTheme { get; set; } = true;
    }

    [Serializable]
    public class UIConfig : ObservableObject
    {
        private static UIConfig _instance = new UIConfig();
        public static UIConfig Ins
        {
            set { _instance = value; }
            get { return _instance; }
        }
        /// <summary>
        /// temp to be deleted
        /// </summary>
        public bool openEnabled { get; } = true;

        private readonly object LockLoadSaveConfig = new object();
        
        private string license = "xxxxxxxx=";
        /// <summary>
        /// 授权秘钥
        /// </summary>
        public string License
        {
            get { return license; }
            set { SetProperty<string>(ref license, value); }
        }

        private string currentCulture = LanguageNames.Chinese;
        /// <summary>
        /// 当前语言
        /// </summary>
        public string CurrentCulture
        {
            get { return currentCulture; }
            set { SetProperty<string>(ref currentCulture, value); }
        }

        [JsonIgnore]
        public string Title { get => SoftwareInfo.SoftwareName; }

        private string companeName = "xxxx";
        /// <summary>
        /// 设备名称
        /// </summary>
        public string CompaneName
        {
            get { return companeName; }
            set { SetProperty<string>(ref companeName, value); }
        }

        private string projectName = "2024001";
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName
        {
            get { return projectName; }
            set { SetProperty<string>(ref projectName, value); }
        }

        private string deviceName = "视觉01机";
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName
        {
            get { return deviceName; }
            set { SetProperty<string>(ref deviceName, value); }
        }

        private bool softwareAutoStartup = false;
        /// <summary>
        /// 软件开机启动
        /// </summary>
        public bool SoftwareAutoStartup
        {
            get { return softwareAutoStartup; }
            set { SetProperty<bool>(ref softwareAutoStartup, value); }
        }

        private string autoLoginUser = "default";
        /// <summary>
        /// 自动登录账户：default等于缺省operator账户，密码空
        /// </summary>
        public string AutoLoginUser
        {
            get { return autoLoginUser; }
            set { SetProperty<string>(ref autoLoginUser, value); }
        }

        private string autoLoginPwd;
        /// <summary>
        /// 自动登录密码：default账户密码为空
        /// </summary>
        public string AutoLoginPwd
        {
            get { return autoLoginPwd; }
            set { SetProperty<string>(ref autoLoginPwd, value); }
        }

        /// <summary>
        /// 暗色主题（默认）
        /// </summary>
        public bool DefaultDarkTheme
        {
            get { return defaultDarkTheme; }
            set { SetProperty<bool>(ref defaultDarkTheme, value); }
        }
        private bool defaultDarkTheme = true;

        private bool autoLoadLayout = false;
        /// <summary>
        /// 自动加载布局
        /// </summary>
        public bool AutoLoadLayout
        {
            get { return autoLoadLayout; }
            set { SetProperty<bool>(ref autoLoadLayout, value); }
        }

        /// <summary>
        /// 自动加载工程
        /// </summary>
        private bool projectAutoLoad = false;
        public bool ProjectAutoLoad
        {
            get { return projectAutoLoad; }
            set { SetProperty<bool>(ref projectAutoLoad, value); }
        }
        /// <summary>
        /// 自动运行工程
        /// </summary>
        private bool projectAutoRun = false;
        public bool ProjectAutoRun
        {
            get { return projectAutoRun; }
            set { SetProperty<bool>(ref projectAutoRun, value); }
        }
        /// <summary>
        /// 工程文件全名
        /// </summary>
        private string projectFileFullName;
        public string ProjectFileFullName
        {
            get { return projectFileFullName; }
            set { SetProperty<string>(ref projectFileFullName, value); }
        }

        private string currentProjectName = "default";
        /// <summary>
        /// 当前工程名称
        /// Path.GetFileNameWithoutExtension(projectFileFullName);
        /// </summary>
        public string CurrentProjectName
        {
            get { return currentProjectName; }
            set { SetProperty<string>(ref currentProjectName, value); }
        }

        public UIConfig() { }
        public void LoadConfigFile(string fullname = "")
        {
            if (string.IsNullOrEmpty(fullname)) Ins = SerializeHelp.Deserialize<UIConfig>(FilePath.ConfigFile, true);
            else Ins = SerializeHelp.Deserialize<UIConfig>(fullname, true);
            if (Ins == null)
            {
                Ins = new UIConfig();
                SaveConfigFile();
            }
        }
        public void SaveConfigFile()
        {
            lock (LockLoadSaveConfig)
            {
                SerializeHelp.SerializeAndSaveFile(UIConfig.Ins, FilePath.ConfigFile, true);
            }
        }
    }
}
