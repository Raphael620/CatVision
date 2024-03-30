using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using HalconDotNet;
using CatVision.Common.Enum;
using CatVision.Camera;
using CatVision.Wpf.ViewModel;
using CatVision.Wpf.Views.HalconView;

namespace CatVision.Wpf.Views
{
    /// <summary>
    /// CameraSetView.xaml 的交互逻辑
    /// </summary>
    public partial class CameraSetView : MetroWindow
    {
        //private static CameraSetView instance = new CameraSetView();
        //public static CameraSetView Ins => instance;
        private CameraSetViewModel vm;// = CameraSetViewModel.Ins;

        public VMHWindowControl mWindowH;
        public CameraSetView()
        {
            InitializeComponent();
            vm = (CameraSetViewModel)this.DataContext;
            vm.DispImgFunc = this.DispImage;
        }
        public void DispImage(HObject img)
        {
            mWindowH.Image = new HImage(img);
        }
        private void cmbCameraType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.SelectedCameraType = cmbCameraType.SelectedItem.ToString();
            CameraProvider provider = EnumHelper.GetEnum<CameraProvider>(vm.SelectedCameraType);
            vm.CameraCBInfoList = CameraFactory.InitCamList(provider);
        }

        private void camListDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (!(vm.SelectedCamera is null)) vm.SelectedCamera.DisConnect();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (mWindowH == null)
            {
                mWindowH = new VMHWindowControl();
                winFormHost.Child = mWindowH;
            }
        }
        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            vm.DispImgFunc = null;
            if (!(vm.SelectedCamera is null))
            {
                // TODO
                vm.SelectedCamera.ImageHandle = null;
                vm.SelectedCamera.DisConnect();
                vm.SelectedCamera.Uninit();
            }
        }
    }
}
