using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Camera;
using CatVision.Wpf.ViewModel;

namespace CatVision.Wpf.Views
{
    /// <summary>
    /// ParaSetView.xaml 的交互逻辑
    /// </summary>
    public partial class ParaSetView : MetroWindow
    {
        //private static ParaSetView instance = new ParaSetView();
        //public static ParaSetView Ins => instance;
        private ParaSetViewModel vm;

        public ParaSetView()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            vm = (ParaSetViewModel)this.DataContext;
            JsonEditor.FontSize = 15;
            JsonEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Json"); // JavaScript
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        // file command
        private void btnSerialize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.fileCommand("SerializeContent");
                JsonEditor.Text = vm.JsonText;
            }
            catch { }
        }
        private void btnDeserialize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.fileCommand("DeserializeContent");
                JsonEditor.Text = vm.JsonText;
            }
            catch { }
        }
        private void btnSaveJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.fileCommand("SaveJson");
                JsonEditor.Text = vm.JsonText;
            }
            catch { }
        }
        private void btnLoadJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.fileCommand("LoadJson");
                JsonEditor.Text = vm.JsonText;
            }
            catch { }
        }
        // combobox command
        private void cbCamParaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //vm.CamIndex = cbCamParaList.SelectedIndex;
            if (cbCamParaList.SelectedIndex >= 0)
            {
                vm.ProdParaList = new ObservableCollection<ProductPara>(vm.CamPara.prod_paras);
                vm.ResetAllValue(false, true, false);
            }
        }
        private void cbProdList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //vm.ProdIndex = cbProdList.SelectedIndex;
            vm.CamPara.prod_paras = vm.ProdParaList.ToList();
            if (cbProdList.SelectedIndex >= 0)
            {
                vm.RoiList = new ObservableCollection<ROI>(vm.ProdPara.rois);
                vm.ResetAllValue(false, false, true);
                htbExpoTime.Value = (cbProdList.SelectedItem as ProductPara).expo_time;
            }
        }
        private void cbRoiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //vm.RoiIndex = cbRoiList.SelectedIndex;
            if (cbRoiList.SelectedIndex >= 0)
            {
                vm.ProdPara.rois = vm.RoiList.ToList();
            }
        }

    }
}
