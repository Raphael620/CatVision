using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HandyControl.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Theming;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using AvalonDock.Themes;
using CatVision.Common.Const;
using CatVision.Common.Enum;
using CatVision.Common.Helper;
using CatVision.Wpf.Models;
using CatVision.Wpf.Models.Data;
using CatVision.Wpf.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace CatVision.Wpf.ViewModel
{
    public class MainViewModel : ObservableRecipient
    {
        private static MainViewModel instance = new MainViewModel();
        public static MainViewModel Ins => instance;
        MainModel mainModel = MainModel.Ins;
        public MainViewModel()
        {
            IsActive = true;
            FileCommand = new RelayCommand<string>(fileCommand);
            EditCommand = new RelayCommand<string>(editCommand);
            ViewCommand = new RelayCommand<string>(viewCommand);
            HelpCommand = new RelayCommand<string>(helpCommand);

            LoginCommand = new RelayCommand<string>(loginCommand);
            ProjectCommand = new RelayCommand<string>(projectCommand);
            SettingCommand = new RelayCommand<string>(settingCommand);
            ChartCommand = new RelayCommand<string>(chartCommand);

        }

        public void InitCulture(string languageName)
        {
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.GetCultureInfo(languageName);
        }

        // 文件菜单栏
        private void fileCommand(string para)
        {
            if (para == "Project_New")
            {
                if ((uint)MyProject.Ins.projectState >= (uint)ProjectState.IsInit)
                {
                    // TODO use langs.resx 下同
                    MessageBoxResult res = HandyControl.Controls.MessageBox.Show("项目运行中，是否关闭当前项目", null, MessageBoxButton.YesNoCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        if (MyProject.Ins.projectFileState >= ProjectFileState.IsNew)
                        {
                            MessageBoxResult res1 = HandyControl.Controls.MessageBox.Show("项目未保存，是否保存当前项目", null, MessageBoxButton.YesNoCancel);
                            if (res1 == MessageBoxResult.OK)
                            {
                                mainModel.SaveProject();
                            }
                            else if (res1 == MessageBoxResult.Cancel) return;
                            mainModel.CloseProject();
                        }
                        mainModel.NewProject();
                    }
                    else return;
                }
                else mainModel.NewProject();
            }
            else if (para == "Project_Open")
            {
                if ((uint)MyProject.Ins.projectState >= (uint)ProjectState.IsInit)
                {
                    // TODO use langs.resx
                    MessageBoxResult res = HandyControl.Controls.MessageBox.Show("项目运行中，是否关闭当前项目", null, MessageBoxButton.YesNoCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        if (MyProject.Ins.projectFileState >= ProjectFileState.IsNew)
                        {
                            MessageBoxResult res1 = HandyControl.Controls.MessageBox.Show("项目未保存，是否保存当前项目", null, MessageBoxButton.YesNoCancel);
                            if (res1 == MessageBoxResult.OK)
                            {
                                mainModel.SaveProject();
                            }
                            else if (res1 == MessageBoxResult.Cancel) return;
                            mainModel.CloseProject();
                        }
                        mainModel.OpenProject();
                    }
                    else return;
                }
                else mainModel.OpenProject();
            }
            else if (para == "Project_Save")
            {
                mainModel.SaveProject();
            }
            else if (para == "Project_SaveAs")
            {
                mainModel.SaveProjectAs();
            }
            else if (para == "Project_Close")
            {
                mainModel.CloseProject();
            }
        }
        public IRelayCommand<string> FileCommand { get; }
        // 编辑菜单栏
        private void editCommand(string para)
        {
            if (para == "Project_Config")
            {

            }
            else if (para == "Parameter_Setting")
            {

            }
            else if (para == "System_Setting")
            {

            }
        }
        public IRelayCommand<string> EditCommand { get; }
        // 视图菜单栏
        public void viewCommand(string para)
        {
            if (para == "Dock_Reset")
            {
                try
                {
                    if (File.Exists("default.dock.layout"))
                    {
                        (new XmlLayoutSerializer(MainDockView.Ins.dockManager)).Deserialize("default.dock.layout");
                    }
                    else
                    {
                        (new XmlLayoutSerializer(MainDockView.Ins.dockManager)).Serialize("default.dock.layout");
                    }
                }
                catch { }
            }
            else if (para == "Dock_Save")
            {
                (new XmlLayoutSerializer(MainDockView.Ins.dockManager)).Serialize("user.dock.layout");
            }
            else if (para == "Dock_Load")
            {
                (new XmlLayoutSerializer(MainDockView.Ins.dockManager)).Deserialize("user.dock.layout");
            }
            else if (para == "View_Theme") { /* 直接在.xaml.cs中编写了 */ }
            else if (para == "View_ScreenShot") { /* 直接在.xaml.cs中编写了 */ }
            else if (para == "Closing") { WeakReferenceMessenger.Default.UnregisterAll(this); }
        }
        public IRelayCommand<string> ViewCommand { get; }
        // 帮助菜单栏
        private void helpCommand(string para)
        {
            if (para == "Menu_License")
            {

            }
            else if (para == "Menu_Help")
            {

            }
            else if (para == "Menu_About")
            {

            }
        }
        public IRelayCommand<string> HelpCommand { get; }

        // 登录命令
        private void loginCommand(string para)
        {
            if (para == "UserLogin")
            {

            }
        }
        public IRelayCommand<string> LoginCommand { get; }

        // 工程命令
        private void projectCommand(string para)
        {
            if (para == "MainView")
            {

            }
            else if (para == "RunAuto")
            {

            }
            else if (para == "RunOnce")
            {
                // test
                string a = DateTime.Now.ToString("HH_mm_ss");
                MainDockView.Ins.AddCameraPanel(a);
            }
            else if (para == "Stop")
            {
                // test
                if (UIConfig.Ins.CurrentCulture == LanguageNames.Chinese) { UIConfig.Ins.CurrentCulture = LanguageNames.English; }
                else { UIConfig.Ins.CurrentCulture = LanguageNames.Chinese; }
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.GetCultureInfo(UIConfig.Ins.CurrentCulture);
            }
        }
        public IRelayCommand<string> ProjectCommand { get; }
        // 设置/配置命令
        private void settingCommand(string para)
        {
            // TODO crital
            // TODO
            // error when closed again
            // Cannot set Visibility or call Show, ShowDialog, or WindowInteropHelper.EnsureHandle after a Window has closed.
            // maybe: rewrite closing to e.cancel and this.hide
            // up the same
            if (para == "CameraSet")
            {
                //CameraSetView.Ins.ShowDialog();
                new CameraSetView().ShowDialog();
            }
            else if (para == "GlobalVar")
            {
                new GlobalVarView().ShowDialog();
                //GlobalVarView.Ins.ShowDialog();
            }
            else if (para == "AlgorithmScript")
            {
                new ScriptEditorView().ShowDialog();
                //ScriptEditorView.Ins.ShowDialog();
            }
            else if (para == "CommunicationSet")
            {
                new ConnectorView().ShowDialog();
                //ConnectorView.Ins.ShowDialog();
            }
            else if (para == "TriggerSet")
            {
                new TriggerSetView().ShowDialog();
                //TriggerSetView.Ins.ShowDialog();
            }
            else if (para == "ParaSetting")
            {
                new ParaSetView().ShowDialog();
                //ParaSetView.Ins.ShowDialog();
            }
        }
        public IRelayCommand<string> SettingCommand { get; }
        // 图表命令
        private void chartCommand(string para)
        {
            if (para == "ChartGen")
            {
                //(new DataReportView()).ShowDialog();
                //DataReportView.Ins.ShowDialog();
                WeakReferenceMessenger.Default.Send("Test msg"); // ok
                WeakReferenceMessenger.Default.Send("Test msg1", "Test");
                WeakReferenceMessenger.Default.Send<LogMsgModel, string>(
                    new LogMsgModel("debug") { msg = "Test Send" }, "MainViewModel");
            }
        }
        public IRelayCommand<string> ChartCommand { get; }

        // 全局消息
        protected override void OnActivated()
        {
            /*Messenger.Register<MainViewModel, string>(this, (r, message) =>
            {
                System.Diagnostics.Debug.Print(message);
            });*/
            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                System.Diagnostics.Debug.Print(@"{0}:{1}", r, m);
            });
            WeakReferenceMessenger.Default.Register<LogMsgModel>(this, (r, m) =>
            {
                System.Diagnostics.Debug.Print(@"{0}:{1}", r, m);
                
            });
            WeakReferenceMessenger.Default.Send<LogMsgModel, string>(
                new LogMsgModel("debug") { msg = "Test Send" }, "MainViewModel");
            //WeakReferenceMessenger.Default.Send(new LoggedInUserChangedMessage(user));
            //WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        private string currentUserName = "Dveloper";
        public string CurrentUserName
        {
            get => currentUserName;
            set { SetProperty<string>(ref currentUserName, value); }
        }
        
        private string themeName = "Dark.Cobalt";
        public string ThemeName
        {
            get => themeName;
            set { SetProperty<string>(ref themeName, value); }
        }

        private DateTime currentTime = DateTime.Now;
        public DateTime CurrentTime
        {
            get => currentTime;
            set { SetProperty<DateTime>(ref currentTime, value); }
        }
    }
}
