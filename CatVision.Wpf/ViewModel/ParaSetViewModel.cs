using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CatVision.Common.Enum;
using CatVision.Common.Helper;
using CatVision.Common.Model;
using CatVision.Camera;
using CatVision.Wpf.Models;
using CatVision.Wpf.Views;

namespace CatVision.Wpf.ViewModel
{
    public class ParaSetViewModel : ObservableObject
    {
        //private static readonly ParaSetViewModel instance = new ParaSetViewModel();
        //public static ParaSetViewModel Ins { get => instance; }
        public ParaSetViewModel()
        {
            ProdParaList = new ObservableCollection<ProductPara>(CamPara.prod_paras);
            RoiList = new ObservableCollection<ROI>(ProdPara.rois);
            AddCommand = new RelayCommand<string>(addCommand);
            FileCommand = new RelayCommand<string>(fileCommand);
            HostCommand = new RelayCommand<string>(hostCommand);
        }

        public string JsonText = string.Empty;
        public string Info { get => info; set { SetProperty(ref info, value); } }
        private string info = string.Empty;
        // 只有下拉之后才能正确写入子项，使用index来手动更新
        //public int CamIndex = -1, ProdIndex = -1, RoiIndex = -1;
        public int CamIndex { get => camIndex; set { SetProperty(ref camIndex, value); } }
        private int camIndex = -1;
        public int ProdIndex { get => prodIndex; set { SetProperty(ref prodIndex, value); } }
        private int prodIndex = -1;
        public int RoiIndex { get => roiIndex; set { SetProperty(ref roiIndex, value); } }
        private int roiIndex = -1;
        // camera
        public CameraPara CamPara { get => camPara; set { SetProperty(ref camPara, value); } }
        private CameraPara camPara = new CameraPara();
        public ObservableCollection<CameraPara> CamParaList { get; set; } = new ObservableCollection<CameraPara>() { new CameraPara() { camera_name = "eq", camera_alias = "rq" } };
        public string NewCamName { get => newCamName; set { SetProperty(ref newCamName, value); } }
        private string newCamName;
        public string NewCamAlias { get => newCamAlias; set { SetProperty(ref newCamAlias, value); } }
        private string newCamAlias;
        // product
        public ProductPara ProdPara { get => prodPara; set { SetProperty(ref prodPara, value); } }
        private ProductPara prodPara = new ProductPara();
        public ObservableCollection<ProductPara> ProdParaList { get; set; } = new ObservableCollection<ProductPara>();
        public string NewProdName { get => newProdName; set { SetProperty(ref newProdName, value); } }
        private string newProdName;
        public string NewProdAlias { get => newProdAlias; set { SetProperty(ref newProdAlias, value); } }
        private string newProdAlias;
        public int ExpoTime {
            get { expoTime = prodPara.expo_time; return expoTime; }
            set { SetProperty(ref expoTime, value); prodPara.expo_time = expoTime; } }
        private int expoTime;
        // roi
        public ROI Roi { get => roi; set { SetProperty(ref roi, value); } }
        private ROI roi = new ROI();
        public ObservableCollection<ROI> RoiList { get; set; } = new ObservableCollection<ROI>();
        public string NewRoiName { get => newRoiName; set { SetProperty(ref newRoiName, value); } }
        private string newRoiName;
        public ROIType NewRoiType { get => newRoiType; set { SetProperty(ref newRoiType, value); } }
        private ROIType newRoiType;

        // 添加
        private void addCommand(string para)
        {
            // camera
            if (para == "AddCamName")
            {
                if (!CamParaList.Select(o => o.camera_name).Contains(NewCamName))
                {
                    CamParaList.Add(new CameraPara() { camera_name = NewCamName, camera_alias = NewCamAlias });
                    OnPropertyChanged(nameof(CamParaList));
                }
            }
            else if (para == "DeleteCamName")
            {
                CamParaList.Remove(CamPara);
                OnPropertyChanged(nameof(CamParaList));
            }
            else if (para == "EditCamAddin")
            {
                if (CamPara is null || string.IsNullOrEmpty(CamPara.camera_name))
                {
                    Info = "Please select camera first"; return;
                }
                DictEditViewModel dictVm = new DictEditViewModel();
                dictVm.SetRefValue(CamPara.addin_dict);
                DictEditDialog dictEdit = new DictEditDialog() { DataContext = dictVm };
                dictEdit.ShowDialog();
                CamPara.addin_dict = dictVm.GetRefValue();
            }
            // product
            else if (para == "AddProductName")
            {
                if (!ProdParaList.Select(o => o.prod_name).Contains(NewProdName))
                {
                    ProdParaList.Add(new ProductPara() { prod_name = newProdName, prod_alias = NewProdAlias });
                    CamParaList[CamIndex].prod_paras = ProdParaList.ToList();
                    OnPropertyChanged(nameof(ProdParaList));
                }
            }
            else if (para == "DeleteProductName")
            {
                ProdParaList.Remove(ProdPara);
                OnPropertyChanged(nameof(ProdParaList));
            }
            else if (para == "EditProductAddin")
            {
                if (ProdPara is null || string.IsNullOrEmpty(ProdPara.prod_name))
                {
                    Info = "Please select product first"; return;
                }
                DictEditViewModel dictVm = new DictEditViewModel();
                dictVm.SetRefValue(ProdPara.addin_dict);
                DictEditDialog dictEdit = new DictEditDialog() { DataContext = dictVm };
                dictEdit.ShowDialog();
                ProdPara.addin_dict = dictVm.GetRefValue();
            }
            // roi
            else if (para == "AddRoiName")
            {
                ROI roi = new ROI();
                DrawROIForm roiForm = new DrawROIForm(roi);
                roiForm.ShowDialog();
                if (string.IsNullOrWhiteSpace(roi.roi_name) || RoiList.Select(o => o.roi_name).Contains(roi.roi_name))
                {
                    Info = "Invalid ROI name."; return;
                }
                else
                {
                    RoiList.Add(roi);
                    CamParaList[CamIndex].prod_paras[ProdIndex].rois = RoiList.ToList();
                    OnPropertyChanged(nameof(RoiList));
                }
            }
            else if (para == "DeleteRoiName")
            {
                RoiList.Remove(Roi);
                OnPropertyChanged(nameof(RoiList));
            }
            else if (para == "EditRoiName")
            {
                if (string.IsNullOrWhiteSpace(roi.roi_name) || !RoiList.Select(o => o.roi_name).Contains(roi.roi_name))
                {
                    Info = "Select ROI first."; return;
                }
                DrawROIForm roiForm = new DrawROIForm(roi);
                roiForm.Show();
                if (string.IsNullOrWhiteSpace(roi.roi_name) || !RoiList.Select(o => o.roi_name).Contains(roi.roi_name))
                {
                    Info = "Invalid ROI name."; return;
                }
                else
                {
                    Roi = roi;
                    OnPropertyChanged(nameof(RoiList));
                }
            }
        }
        public IRelayCommand<string> AddCommand { get; }

        public void EditDict(ref Dictionary<string, string> dict)
        {
            DictEditViewModel dictVm = new DictEditViewModel();
            dictVm.SetRefValue(dict);
            DictEditDialog dictEdit = new DictEditDialog() { DataContext = dictVm };
            dictEdit.ShowDialog();
            dict = dictVm.GetRefValue();
        }
        public void ResetAllValue(bool cam = false, bool prod = false, bool roi = false)
        {
            if (cam) OnPropertyChanged(nameof(CamParaList));
            if (prod) OnPropertyChanged(nameof(ProdParaList));
            if (roi) OnPropertyChanged(nameof(RoiList));
        }
        // 文件命令
        public void fileCommand(string para)
        {
            if (para == "SerializeContent") // 从左边栏生成json文本
            {
                JsonText = JsonConvert.SerializeObject(CamParaList, Formatting.Indented);
            }
            else if (para == "DeserializeContent") // 从json文本生成配置
            {
                try
                {
                    List<CameraPara> camlist = JsonConvert.DeserializeObject<List<CameraPara>>(JsonText);
                    CamParaList = new ObservableCollection<CameraPara>(camlist);
                    OnPropertyChanged(nameof(CamParaList));
                    // to test
                    CamPara = new CameraPara();
                    ProdParaList = new ObservableCollection<ProductPara>(CamPara.prod_paras);
                    RoiList = new ObservableCollection<ROI>(ProdPara.rois);
                    OnPropertyChanged(nameof(ProdParaList));
                    OnPropertyChanged(nameof(RoiList));
                }
                catch { Info = "Deserialize load error."; }
            }
            else if (para == "LoadJson") // 读取json文件
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = Directory.GetCurrentDirectory(),
                    Filter = "Config File (*.json)|*.json|所有文件 (*.*)|*.*",
                    RestoreDirectory = true,
                    FilterIndex = 1
                };
                if (true == openFileDialog.ShowDialog())
                {
                    string fName = openFileDialog.FileName;
                    using (FileStream fs = new FileStream(fName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            JsonText = sr.ReadToEnd().ToString();
                        }
                    }
                    Info = Path.GetFileName(fName);
                }
            }
            else if (para == "SaveJson") // 生成json文件
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    InitialDirectory = Directory.GetCurrentDirectory(),
                    Filter = "配置文件 (*.json)|*.json|所有文件 (*.*)|*.*"
                };
                if (true == sfd.ShowDialog())
                {
                    string fName = sfd.FileName;
                    using (FileStream fs = new FileStream(fName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.SetLength(0);
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                        {
                            sw.WriteLine(JsonText);
                        }
                    }
                }
            }
        }
        public IRelayCommand<string> FileCommand { get; }
        // 保存配置
        private void hostCommand(string para) 
        {
            if (para == "SaveProject")
            {
                MainModel.Ins.EditProject(CamParaList.ToList(), "CamPara");
            }
            else if (para == "LoadProject")
            {
                // TODO 
                List<CameraPara> list = (List<CameraPara>)MainModel.Ins.LoadProject("CamPara");
                CamParaList = new ObservableCollection<CameraPara>(list);
                OnPropertyChanged(nameof(CamParaList));
                // to test : same as deserilize
                CamPara = new CameraPara();
                 ProdParaList = new ObservableCollection<ProductPara>(CamPara.prod_paras);
                RoiList = new ObservableCollection<ROI>(ProdPara.rois);
                OnPropertyChanged(nameof(ProdParaList));
                OnPropertyChanged(nameof(RoiList));
            }
            else if (para == "Exit")
            {

            }
        }
        public IRelayCommand<string> HostCommand { get; }
    }
}
