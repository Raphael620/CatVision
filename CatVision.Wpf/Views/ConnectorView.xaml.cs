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
using CatVision.Communication;
using CatVision.Wpf.ViewModel;

namespace CatVision.Wpf.Views
{
    /// <summary>
    /// ConnectionView.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectorView : MetroWindow
    {
        //private static ConnectorView instance = new ConnectorView();
        //public static ConnectorView Ins => instance;
        private ConnectorViewModel vm;
        public ConnectorView()
        {
            InitializeComponent();
            vm = (ConnectorViewModel)this.DataContext;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (IConnector conn in vm.ConnectorInfos)
            {
                if (conn.DevInfo.IsConnected) conn.DisConnect();
            }
        }
    }
}
