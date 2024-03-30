using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using AvalonDock.Themes;
using HalconDotNet;
using CatVision.Wpf.Models.Data;
using CatVision.Wpf.Views.HalconView;

namespace CatVision.Wpf.Views
{
    /// <summary>
    /// MainDockView.xaml 的交互逻辑
    /// </summary>
    public partial class MainDockView : UserControl
    {
        private static MainDockView _instance;
        public static MainDockView Ins
        {
            get
            {
                if (_instance == null) _instance = new MainDockView();
                return _instance;
            }
        }
        public Dictionary<string, VMHWindowControl> HalconWindows = new Dictionary<string, VMHWindowControl>();
        private MainDockView()
        {
            InitializeComponent();
            //this.DataContext = MainDockViewModel.Ins;
        }
        public void AddCameraPanel(string camName)
        {
            if (HalconWindows.ContainsKey(camName)) { return; }
            TextBlock tb = new TextBlock() { Text = camName, TextAlignment = TextAlignment.Center };
            VMHWindowControl win = new VMHWindowControl();
            WindowsFormsHost host = new WindowsFormsHost();
            host.Child = win;
            LayoutDocument doc = new LayoutDocument()
            {
                Title = camName,
                CanClose = false,
                ContentId = camName,
                Content = host,
            };
            vision.Children.Add(doc);
            HalconWindows.Add(camName, win);
        }
        public void DispImage(string camName, HObject img)
        {
            HalconWindows[camName].Image = new HImage(img);
        }
        public void ChangeDockTheme()
        {
            if (ThemeConfig.DarkTheme)
            {
                dockManager.Theme = new Vs2013DarkTheme();
            }
            else
            {
                dockManager.Theme = new Vs2013LightTheme();
            }
        }
        public void loadpanel()
        {
            var currentContentsList = dockManager.Layout.Descendents().OfType<LayoutContent>().Where(c => c.ContentId != null).ToArray();
            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamReader("AvalonDock_Demo.config"))
            {
                serializer.Deserialize(stream);
            }
        }
        public void savepanel()
        {
            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamWriter("AvalonDock_Demo.config"))
            {
                serializer.Serialize(stream);
            }
        }
        private void adddocument()
        {
            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                LayoutDocument doc = new LayoutDocument
                {
                    Title = "Test1"
                };
                firstDocumentPane.Children.Add(doc);

                LayoutDocument doc2 = new LayoutDocument
                {
                    Title = "Test2"
                };
                firstDocumentPane.Children.Add(doc2);
            }

            var leftAnchorGroup = dockManager.Layout.LeftSide.Children.FirstOrDefault();
            if (leftAnchorGroup == null)
            {
                leftAnchorGroup = new LayoutAnchorGroup();
                dockManager.Layout.LeftSide.Children.Add(leftAnchorGroup);
            }

            leftAnchorGroup.Children.Add(new LayoutAnchorable() { Title = "New Anchorable" });
        }
        private void LayoutRoot_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            /*var activeContent = ((LayoutRoot)sender).ActiveContent;
            if (activeContent != null)
            {
                /*MessageBox.Show($"ActiveContent-> {activeContent}");
            }*/
        }
    }
}
