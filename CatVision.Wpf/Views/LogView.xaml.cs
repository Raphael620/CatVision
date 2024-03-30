using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CatVision.Wpf.Views
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class LogView : UserControl
    {
        private static readonly LogView _instance = new LogView();
        public static LogView Ins => _instance;
        public LogView()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
