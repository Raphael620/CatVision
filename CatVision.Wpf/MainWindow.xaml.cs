using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using HandyControl.Controls;
using HandyControl.Data;
using ControlzEx.Theming;
using WPFLocalizeExtension;
using CatVision.Wpf.Models.Data;
using CatVision.Wpf.Views;
using CatVision.Wpf.ViewModel;

namespace CatVision.Wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow, IDisposable
    {
        // TODO : if singleton used, not disposed after closed
        //private static readonly MainWindow instance = new MainWindow();
        //public static MainWindow Ins => instance;
        public MainWindow()
        {
            InitializeComponent();
            HandyControl.Controls.Screenshot.Snapped += Screenshot_Snapped;
            // should be used after init component
            locanazation(UIConfig.Ins.CurrentCulture);
            if (!UIConfig.Ins.DefaultDarkTheme) ChangeTheme();
        }
        private void locanazation(string languageName)
        {
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.GetCultureInfo(languageName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //checkBox.SetResourceReference(BackgroundProperty, "AccentBrush");
            //Resources["PrimaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff5722"));
            //Resources["PrimaryBrush"] = (Brush)FindResource("BrushDanger");
            ChangeTheme();
        }
        private void ChangeTheme()
        {
            string resourceStr;
            ThemeConfig.DarkTheme = !ThemeConfig.DarkTheme;
            if (ThemeConfig.DarkTheme)
            {
                ThemeManager.Current.ChangeTheme(this, "Dark.Steel");
                resourceStr = "pack://application:,,,/Resources/Themes/HandyDark.xaml";
            }
            else
            {
                ThemeManager.Current.ChangeTheme(this, "Light.Blue");
                resourceStr = "pack://application:,,,/Resources/Themes/HandyLight.xaml";
            }
            UpdataResourceDictionary(resourceStr, 0);
            MainDockView.Ins.ChangeDockTheme();
        }
        private void UpdataResourceDictionary(string resourceStr, int pos)
        {
            if (pos < 0 || pos > 2)
            {
                return;
            }
            ResourceDictionary resource = new ResourceDictionary
            {
                Source = new Uri(resourceStr)
            };
            Resources.MergedDictionaries.RemoveAt(pos);
            Resources.MergedDictionaries.Insert(pos, resource);
        }
        private void ViewTheme_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChangeTheme();
        }
        private void Screenshot_Snapped(object sender, FunctionEventArgs<ImageSource> e)
        {
            var dialog = new HandyControl.Controls.Window
            {
                Content = new Image { Source = e.Info, Stretch = Stretch.None },
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            dialog.ShowDialog();
            // TODO useless
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PNG图像|*.png|所有文件|*.*";
                if (sfd.ShowDialog() == true)
                {
                    if (String.IsNullOrEmpty(sfd.FileName)) return;
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)(((Image)dialog.Content).Source)));
                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        encoder.Save(stream);
                    }
                }
            }
            dialog.Close();
        }
        public void Dispose() => HandyControl.Controls.Screenshot.Snapped -= Screenshot_Snapped;

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainViewModel.Ins.viewCommand("Closing");
        }
    }
}
