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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Common.Const;
using CatVision.Common.Helper;
using CatVision.Wpf.ViewModel;

namespace CatVision.Wpf.Views
{
    /// <summary>
    /// GlobalValDialog.xaml 的交互逻辑
    /// </summary>
    public partial class GlobalValDialog : MetroWindow
    {
        public List<string> TypeEnums = EnumHelper.GetEnumStrs<mValueType>();
        private GlobalVarDialogViewModel vm;
        public GlobalValDialog()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            vm = (GlobalVarDialogViewModel)DataContext;
            vm.ValueConfirmed = true;
            this.Close();
        }
    }

    
}
