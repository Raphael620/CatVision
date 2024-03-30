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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using CatVision.Common.Enum;
using CatVision.Camera;
using CatVision.Wpf.ViewModel;

namespace CatVision.Wpf.Views
{
    /// <summary>
    /// GlobalValView.xaml 的交互逻辑
    /// </summary>
    public partial class GlobalVarView : MetroWindow
    {
        //private static GlobalVarView instance = new GlobalVarView();
        //public static GlobalVarView Ins => instance;
        public GlobalVarView()
        {
            InitializeComponent();
        }
    }
}
