using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using CatVision.Common;
using CatVision.Common.Helper;
using CatVision.Wpf.Models;

namespace CatVision.Wpf
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static SplashScreen splashScreen;
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                /// 0. 检测程序进程
                Process currentProcess = Process.GetCurrentProcess();
                Process[] myProcess = Process.GetProcessesByName(currentProcess.ProcessName);
                if (myProcess.Length > 1)
                {
                    MessageBox.Show("检测到程序已经运行，请先关闭多余的程序和进程!");
                    Application.Current.Shutdown();
                    return;
                }
                /// 1. 全局异常捕获
                /*this.DispatcherUnhandledException += App_DispatcherUnhandledException;
                Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDoShell_UnhandledException);
                this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;*/
                /// 2. 开始画面
                //splashScreen = new SplashScreen(@"/Resources/Images/start.png");
                //splashScreen.Show(false);
                //MainModel.Ins.InitConfig();
                base.OnStartup(e);
                //splashScreen.Close(new TimeSpan(0, 0, 1));
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "App.OnStartup");
            }
        }

        private void InitCulture()
        {
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.GetCultureInfo("zh");
        }
        private void InitCultureFromXaml()
        {
            
            // 获取当前系统语言
            //var currentLanguage = CultureInfo.CurrentCulture.Name;
            var currentLanguage = "zh-CN";

            // 仅保留需要的语言
            var supportedLanguages = new List<string>() { "en-US", "zh-CN" };

            // 遍历资源文件夹下的每个语言目录
            var resourcesPath = "";
            var languageDirectories = Directory.GetDirectories(resourcesPath);
            foreach (var languageDirectory in languageDirectories)
            {
                var language = Path.GetFileName(languageDirectory);

                // 如果不是需要的语言，则从 MergedDictionaries 中移除对应的资源字典
                if (!supportedLanguages.Contains(language))
                {
                    var resourceFile = $"{languageDirectory}\\Strings.xaml";
                    var mergedDictionary = Resources.MergedDictionaries.FirstOrDefault(d => d.Source.OriginalString == resourceFile);
                    if (mergedDictionary != null)
                    {
                        Resources.MergedDictionaries.Remove(mergedDictionary);
                    }
                }
            }
        }

        /// <summary>
        /// 显示未捕获的App异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                if (e != null && e.Exception != null)
                {
                    Log.Default.Error(e.Exception, "App_DispatcherUnhandledException");
                }
                e.Handled = true;
            }
            catch { }
        }

        /// <summary>
        /// 显示未捕获的Current异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                if (e != null && e.Exception != null)
                {
                    Log.Default.Error(e.Exception, "Current_DispatcherUnhandledException");
                }
                e.Handled = true;
            }
            catch { }
        }

        /// <summary>
        /// 显示未捕获的CurrentDoShell异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentDoShell_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                //记录dump文件
                MiniDump.TryDump($"dumps\\VM_{DateTime.Now.ToString("HH-mm-ss-ms")}.dmp");
                Exception ex = null;
                if (e != null)
                    ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    Log.Default.Fatal(new Exception("Current_DispatcherUnhandledException"), @"App.Current_DispatcherUnhandledException with {0}", e);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 显示未捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = null;
                if (e != null)
                    ex = e.Exception;

                if (ex != null)
                {
                    Log.Default.Error(ex, "DispatcherUnhandledExceptionEventArgs");
                }
                e.Handled = true;
            }
            catch { }
        }

        /// <summary>
        /// Task线程内未捕获异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                if (e != null && e.Exception != null)
                {
                    Log.Default.Error(e.Exception, "UnobservedTaskExceptionEventArgs");
                }
            }
            catch { }
        }
    }
}
