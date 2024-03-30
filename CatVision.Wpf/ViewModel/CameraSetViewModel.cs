using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HalconDotNet;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Camera;
using CatVision.Wpf.Models;
using CatVision.Wpf.Views;

namespace CatVision.Wpf.ViewModel
{
    public class CameraSetViewModel : ObservableObject
    {
        //private static readonly CameraSetViewModel instance = new CameraSetViewModel();
        //public static CameraSetViewModel Ins { get => instance; }
        public Action<HObject> DispImgFunc;
        public CameraSetViewModel() 
        {
            CamsCommand = new RelayCommand<string>(camsCommand);
            OperateCommand = new RelayCommand<string>(operateCommand);
            HostCommand = new RelayCommand<string>(hostCommand);
        }
        
        // 从enum得到的厂商列表，不可变
        private List<string> cameraTypes = EnumHelper.GetEnumStrs<CameraProvider>();
        public List<string> CameraTypes
        {
            get => cameraTypes;
            set { SetProperty<List<string>>(ref cameraTypes, value); }
        }
        // 下拉栏选中的厂商名称
        private string selectedCameraType;
        public string SelectedCameraType
        {
            get => selectedCameraType;
            set { SetProperty<string>(ref selectedCameraType, value); }
        }

        // datagrid表格中绑定的相机信息列表
        //private List<CameraInfo> cameraInfos = new List<CameraInfo>();
        /*public List<CameraInfo> CameraInfos
        {
            get => cameraInfos;
            set { SetProperty<List<CameraInfo>>(ref cameraInfos, value); }
        }*/
        public ObservableCollection<CameraInfo> CameraInfos { get; set; } = new ObservableCollection<CameraInfo>();
        // datagrid表格中选中的相机信息
        private CameraInfo selectedCameraInfo;
        public CameraInfo SelectedCameraInfo
        {
            get => selectedCameraInfo;
            set { SetProperty<CameraInfo>(ref selectedCameraInfo, value); }
        }
        
        // 下拉栏中绑定的相机信息列表
        private List<CameraInfo> cameraCBInfoList = new List<CameraInfo>();
        public List<CameraInfo> CameraCBInfoList
        {
            get => cameraCBInfoList;
            set { SetProperty<List<CameraInfo>>(ref cameraCBInfoList, value); }
        }
        // 下拉栏中选中的相机的SN
        private CameraInfo selectedCBCamItem;
        public CameraInfo SelectedCBCamItem
        {
            get => selectedCBCamItem;
            set { SetProperty<CameraInfo>(ref selectedCBCamItem, value); }
        }
        
        // 操作栏中绑定的当前相机
        private ICamera<HObject> selectedCamera;
        public ICamera<HObject> SelectedCamera
        {
            get => selectedCamera;
            set 
            { 
                SetProperty<ICamera<HObject>>(ref selectedCamera, value);
                //OnPropertyChanged(nameof(SelectedCamera));
            }
        }
        // 从enum中枚举的触发源，不可变
        private List<string> triggerSources = EnumHelper.GetEnumStrs<TriggerSource>();
        public List<string> TriggerSources
        {
            get => triggerSources;
            set { SetProperty<List<string>>(ref triggerSources, value); }
        }
        // 底部的提示信息
        private string info;
        public string Info { get => info; set { SetProperty<string>(ref info, value); } }

        // 相机增删
        private void camsCommand(string para)
        {
            if (para == "Add")
            {
                if (CameraInfos.Contains(selectedCBCamItem)) return;
                try
                {
                    CameraInfos.Add(selectedCBCamItem);
                    OnPropertyChanged(nameof(CameraInfos));
                }
                catch { Info = "Add error"; }
            }
            else if (para == "Delete")
            {
                if (!CameraInfos.Contains(selectedCBCamItem)) return;
                try
                {
                    CameraInfos.Remove(selectedCBCamItem);
                    OnPropertyChanged(nameof(CameraInfos));
                }
                catch { Info = "Add error"; }
            }
        }
        public IRelayCommand<string> CamsCommand { get; }
        // 相机操作
        private void operateCommand(string para)
        {
            if (para == "Connect")
            {
                if (!(SelectedCamera is null)) SelectedCamera.DisConnect();
                SelectedCamera = CameraFactory.GetInstance(SelectedCameraInfo);
                SelectedCamera.Connect();
                Info = "Camera connected.";
                SelectedCamera.ImageHandle = this.DispImage;
            }
            else if (para == "Disconnect")
            {
                SelectedCamera.DisConnect();
                Info = "Camera disconnected.";
            }
            else if (para == "Trigger")
            {
                SelectedCamera.Capture();
            }
            else if (para == "Continuous")
            {
                SelectedCamera.SetTriggerMode(!SelectedCamera.CamInfo.TriggerMode);
            }
        }
        public IRelayCommand<string> OperateCommand { get; }
        private void DispImage(HObject img)
        {
            //CameraSetView.Ins.DispImage(img);
            DispImgFunc?.Invoke(img);
        }
        // 保存配置
        private void hostCommand(string para)
        {
            try
            {
                if (para == "SaveImage")
                {
                    // TODO
                }
                else if (para == "SaveProject")
                {
                    MainModel.Ins.EditProject(CameraInfos.ToList(), "Cameras");
                }
                else if (para == "LoadProject")
                {
                    List<CameraInfo> cams = (List<CameraInfo>)MainModel.Ins.LoadProject("Cameras");
                    CameraInfos = new ObservableCollection<CameraInfo>(cams);
                    OnPropertyChanged(nameof(CameraInfos));
                }
            }
            catch { }
        }
        public IRelayCommand<string> HostCommand { get; }

    }
}
