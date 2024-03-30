using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Camera;
using CatVision.Wpf.Models;
using CatVision.Wpf.Views;
using CatVision.Communication;

namespace CatVision.Wpf.ViewModel
{
    public class ConnectorViewModel : ObservableObject
    {
        //private static readonly ConnectorViewModel instance = new ConnectorViewModel();
        //public static ConnectorViewModel Ins { get => instance; }
        public ConnectorViewModel()
        {
            AddDeviceCommand = new RelayCommand<string>(addDeviceCommand);
            ConnectCommand = new RelayCommand<string>(connectCommand);
            ParamCommand = new RelayCommand<string>(paramCommand);
            WriteCommand = new RelayCommand<string>(writeCommand);
            ReadCommand = new RelayCommand<string>(readCommand);
            HostCommand = new RelayCommand<string>(hostCommand);
        }
        public string Info { get => info; set { SetProperty<string>(ref info, value); } }
        private string info;
        // 
        public List<string> DeviceTypeList { get; } = EnumHelper.GetEnumStrs<ConnectorProvider>();
        // private List<string> deviceTypeList = EnumHelper.GetEnumStrs<DeviceProvider>();
        public string SelectedType { get => selectedType; set { SetProperty<string>(ref selectedType, value); } }
        private string selectedType;
        public string AddDevName { get => addDevName; set { SetProperty<string>(ref addDevName, value); } }
        private string addDevName;
        public DeviceInfo SelectedDevInfo { get => selectedDevInfo; set => SetProperty(ref selectedDevInfo, value); }
        private DeviceInfo selectedDevInfo = new DeviceInfo();
        
        // datagrid中绑定的设备列表
        public ObservableCollection<IConnector> ConnectorInfos { get; set; } = new ObservableCollection<IConnector>();
        //private List<IConnection> ConnectionInfos = new List<IConnection>();
        //public ObservableCollection<ObsDeviceInfo> DeviceInfos { get; set; } = new ObservableCollection<ObsDeviceInfo>();

        // datagrid中选中的device,IConnection用于参数配置和连接，ObsDeviceInfo仅用于datagrid中的参数
        public IConnector SeletedConnectorInfo
        {
            get => seletedConnectorInfo;
            set
            {
                SetProperty<IConnector>(ref seletedConnectorInfo, value);
                WriteDataList = new ObservableCollection<GlobalValModel>(seletedConnectorInfo.ConnInfo.PublishList);
                OnPropertyChanged(nameof(WriteDataList));
                ReadDataList = new ObservableCollection<GlobalValModel>(seletedConnectorInfo.ConnInfo.SubscribeList);
                OnPropertyChanged(nameof(ReadDataList));
                SetS7AddinPara();
            } 
        }
        private IConnector seletedConnectorInfo;
        //public IConnection SeletedDeviceInfo;
        //public ObsDeviceInfo SeletedObsDeviceInfo { get => seletedObsDeviceInfo; set { SetProperty<ObsDeviceInfo>(ref seletedObsDeviceInfo, value); } }
        //private ObsDeviceInfo seletedObsDeviceInfo;


        // selected ip and port
        //public string SelectedName { get => name; set { SetProperty(ref name, value); } }
        //private string name;
        public string SelectedIp { get => ip; set { SetProperty(ref ip, value); } }
        private string ip;
        public int SelectedPort { get => port; set { SetProperty(ref port, value); } }
        private int port;
        //public bool SelectedIsConnected { get => isConnected; set { SetProperty(ref isConnected, value); } }
        //private bool isConnected;

        // TODO: how to use
        //public object SelectedTabItem { get => selectedTabItem; set { SetProperty(ref selectedTabItem, value); } }
        //private object selectedTabItem;
        // for serial
        public List<int> BaudRateList { get => baudRateList; set { SetProperty(ref baudRateList, value); } }
        private List<int> baudRateList = ComEnums.BaudRateEnums.ToList();
        public List<int> DataBitsList { get => dataBitsList; set { SetProperty(ref dataBitsList, value); } }
        private List<int> dataBitsList = ComEnums.DataBitsEnums.ToList();
        public List<string> StopBitsList { get => stopBitsList; set { SetProperty(ref stopBitsList, value); } }
        private List<string> stopBitsList =EnumHelper.GetEnumStrs<System.IO.Ports.StopBits>();
        public List<string> ParityList { get => parityList; set { SetProperty(ref parityList, value); } }
        private List<string> parityList = EnumHelper.GetEnumStrs<System.IO.Ports.Parity>();
        private int baudRate, dataBits;
        private string parity, stopBits;
        public int BaudRate { get => baudRate; set { SetProperty(ref baudRate, value); } }
        public int DataBits { get => dataBits; set { SetProperty(ref dataBits, value); } }
        public string Parity { get => parity; set { SetProperty(ref parity, value); } }
        public string StopBits { get => stopBits; set { SetProperty(ref stopBits, value); } }

        // for s7
        public List<string> CpuTypeList { get => cpuTypeList; set { SetProperty(ref cpuTypeList, value); } }
        private List<string> cpuTypeList = S7Device.CpuTypeList;
        private string cpuType;
        private int rack, slot;
        public string CpuType { get => cpuType; set { SetProperty(ref cpuType, value); } }
        public int Rack { get => rack; set { SetProperty(ref rack, value); } }
        public int Slot { get => slot; set { SetProperty(ref slot, value); } }
        public void SetS7AddinPara()
        {
            try
            {
                SelectedIp = SeletedConnectorInfo.ConnInfo.Ip;
                SelectedPort = SeletedConnectorInfo.ConnInfo.Port;
                if (SeletedConnectorInfo.ConnInfo.Provider == "Siemens7")
                {
                    if (SeletedConnectorInfo.ConnInfo.AddinPara.Count < 1) { return; }
                    CpuType = (string)SeletedConnectorInfo.ConnInfo.AddinPara[0].GetTypedValue();
                    Rack = (int)SeletedConnectorInfo.ConnInfo.AddinPara[1].GetTypedValue();
                    Slot = (int)SeletedConnectorInfo.ConnInfo.AddinPara[2].GetTypedValue();
                }
                else if (SeletedConnectorInfo.ConnInfo.Provider == "Serial")
                {
                    if (SeletedConnectorInfo.ConnInfo.AddinPara.Count < 1) { return; }
                    BaudRate = (int)SeletedConnectorInfo.ConnInfo.AddinPara[0].GetTypedValue();
                    DataBits = (int)SeletedConnectorInfo.ConnInfo.AddinPara[1].GetTypedValue();
                    Parity = (string)SeletedConnectorInfo.ConnInfo.AddinPara[2].GetTypedValue();
                    StopBits = (string)SeletedConnectorInfo.ConnInfo.AddinPara[3].GetTypedValue();
                }
            }
            catch { }
        }
        // data
        public ObservableCollection<GlobalValModel> WriteDataList { get; set; } = new ObservableCollection<GlobalValModel>();
        public ObservableCollection<GlobalValModel> ReadDataList { get; set; } = new ObservableCollection<GlobalValModel>();
        //public List<GlobalValModel> WriteDataList { get => writeDataList; set => SetProperty(ref writeDataList, value); }
        //private List<GlobalValModel> writeDataList = new List<GlobalValModel>();
        public GlobalValModel SelectedData { get => selectedData;
            set { SetProperty(ref selectedData, value); SelectedValue = value.value.ToString(); } }
        private GlobalValModel selectedData;
        public string SelectedValue { get => selectedValue; set => SetProperty(ref selectedValue, value); }
        private string selectedValue;

        private GlobalValModel AddValueDialog(GlobalValModel oldVal = null)
        {
            GlobalVarDialogViewModel varVm;
            if (oldVal is null) { varVm = new GlobalVarDialogViewModel(); }
            else { varVm = new GlobalVarDialogViewModel(oldVal); }
            GlobalValDialog dialog = new GlobalValDialog() { DataContext = varVm };
            dialog.ShowDialog();
            return varVm.GValue;
        }
        //
        private void addDeviceCommand(string para)
        {
            if (para == "Add")
            {
                try
                {
                    SelectedDevInfo.DeviceType = SelectedType;
                    SelectedDevInfo.DeviceName = AddDevName;
                    if (string.IsNullOrEmpty(SelectedDevInfo.DeviceType) || string.IsNullOrEmpty(SelectedDevInfo.DeviceName)) { return; }
                    if (ConnectorInfos.Select(o => o.DevInfo.DeviceName).Contains(SelectedDevInfo.DeviceName)) { return; }
                    DeviceInfo dev = new DeviceInfo() { DeviceType = SelectedDevInfo.DeviceType, DeviceName = SelectedDevInfo.DeviceName };
                    ConnectorInfo connInfo = new ConnectorInfo(SelectedDevInfo.DeviceType);
                    IConnector conn = ConnectorFactory.GetDevice(connInfo);
                    conn.DevInfo = dev;
                    ConnectorInfos.Add(conn);
                    OnPropertyChanged(nameof(ConnectorInfos));
                }
                catch { Info = "add error."; }
            }
            else if (para == "Delete")
            {
                //if (!DeviceInfos.Contains(SeletedDeviceInfo)) { return; }
                if (!ConnectorInfos.Contains(SeletedConnectorInfo)) { return; }
                try
                {
                    //DeviceInfos.Remove(SeletedDeviceInfo);
                    ConnectorInfos.Remove(SeletedConnectorInfo);
                    OnPropertyChanged(nameof(ConnectorInfos));
                }
                catch { Info = "delete error."; }
            }
        }
        public IRelayCommand<string> AddDeviceCommand { get; }
        private void paramCommand(string para)
        {
            if (para == "Modify")
            {
                if (IPAddress.TryParse(SelectedIp, out _))
                {
                    SeletedConnectorInfo.ConnInfo.Ip = SelectedIp;
                }
                else { SelectedIp = string.Empty; }
                if (UInt16.MinValue < SelectedPort && UInt16.MaxValue > SelectedPort)
                {
                    SeletedConnectorInfo.ConnInfo.Port = SelectedPort;
                }
            }
            if (para == "Confirm")
            {
                if (SeletedConnectorInfo is null) { return; }
                //if (SeletedConnectorInfo.ConnInfo.Provider != selectedTabItem.ToString()) return;
                if (SeletedConnectorInfo.ConnInfo.Provider == ConnectorProvider.Siemens7.ToString())
                {
                    SeletedConnectorInfo.ConnInfo.AddinPara = new List<GlobalValModel>()
                    {
                        new GlobalValModel(mValueType.mstr, "CpuType", "CpuType", CpuType),
                        new GlobalValModel(mValueType.mint, "Rack", "Rack", Rack),
                        new GlobalValModel(mValueType.mint, "Slot", "Slot", Slot)
                    };
                }
                else if (SeletedConnectorInfo.ConnInfo.Provider == ConnectorProvider.Serial.ToString())
                {
                    SeletedConnectorInfo.ConnInfo.AddinPara = new List<GlobalValModel>()
                    {
                        new GlobalValModel(mValueType.mint, "BaudRate", "BaudRate", BaudRate),
                        new GlobalValModel(mValueType.mint, "DataBits", "DataBits", DataBits),
                        new GlobalValModel(mValueType.mstr, "Parity", "Parity", Parity),
                        new GlobalValModel(mValueType.mstr, "StopBits", "StopBits", StopBits)
                    };
                }
            }
        }
        public IRelayCommand<string> ParamCommand { get; }
        private void connectCommand(string para)
        {
            // TODO binding useless
            //seletedDeviceInfo.DevInfo.Ip
            if (para == "Modify")
            {
                //if (SeletedConnectorInfo is null) { return; }
                //SeletedConnectorInfo.DevInfo = selectedDevInfo;
            }
            else if (para == "Connect")
            {
                try
                {
                    if (SeletedConnectorInfo.DevInfo.IsConnected)
                    {
                        SeletedConnectorInfo.DisConnect();
                    }
                    else
                    {
                        SeletedConnectorInfo.Connect();
                        if (SeletedConnectorInfo.DevInfo.IsConnected)
                        {
                            SeletedConnectorInfo.ConnInfo.SubscribeList.ForEach(o => ReadDataList.Add(o));
                            // TODO binding 
                            OnPropertyChanged(nameof(ReadDataList));
                            // TODO test recv
                            if (SeletedConnectorInfo.DevInfo.DeviceType == ConnectorProvider.TcpServer.ToString())
                            {
                                (SeletedConnectorInfo as TcpServerDevice).Publish += TestTcpRecv;
                            }
                        }
                    }
                    Info = "connect success";
                }
                catch { Info = "connect error."; }
            }
        }
        // data
        private void TestTcpRecv(GlobalValModel val)
        {
            if (ReadDataList.Select(o => o.address).Contains(val.address))
            {
                int index = ReadDataList.ToList().FindIndex(o => o.address == val.address);
                ReadDataList[index].value = val.value;
            }
            else
            {
                ReadDataList.Add(val);
            }
            OnPropertyChanged(nameof(ReadDataList));
            // TODO 界面不更新ReadDataList
        }
        public IRelayCommand<string> ConnectCommand { get; }
        private void writeCommand(string para)
        {
            if (para == "Add")
            {
                GlobalValModel val = AddValueDialog();
                if (WriteDataList.Select(o => o.address).Contains(val.address))
                {
                    Info = "Value allready exit."; return;
                }
                else
                {
                    WriteDataList.Add(val);
                    SeletedConnectorInfo.ConnInfo.PublishList.Add(val);
                    OnPropertyChanged(nameof(WriteDataList));
                }
            }
            else if (para == "Edit")
            {
                if (SelectedData is null) { return; }
                GlobalValModel val = AddValueDialog((GlobalValModel)SelectedData.Clone());
                WriteDataList.Remove(SelectedData);
                WriteDataList.Add(val);
                SeletedConnectorInfo.ConnInfo.PublishList.Remove(SelectedData);
                SeletedConnectorInfo.ConnInfo.PublishList.Add(val);
                OnPropertyChanged(nameof(WriteDataList));
            }
            else if (para == "Delete")
            {
                try
                {
                    if (!(SelectedData is null))
                    {
                        WriteDataList.Remove(SelectedData);
                        SeletedConnectorInfo.ConnInfo.PublishList.Remove(SelectedData);
                        OnPropertyChanged(nameof(WriteDataList));
                    }
                }
                catch { }
            }
            else if (para == "Write")
            {
                try
                {
                    if (SeletedConnectorInfo is null) { return; }
                    if (SelectedData is null) { return; }
                    GlobalValModel val = (GlobalValModel)SelectedData.Clone();
                    if (val.SetTypedValue(SelectedValue))
                    {
                        SelectedData.value = val.value;
                    }
                    OnPropertyChanged(nameof(WriteDataList));
                    SeletedConnectorInfo.SendAsync(val);
                }
                catch (Exception ex) { Info = ex.Message; }
            }
            if (para == "AddRead")
            {
                GlobalValModel val = AddValueDialog();
                if (ReadDataList.Select(o => o.address).Contains(val.address))
                {
                    Info = "Value allready exit."; return;
                }
                else
                {
                    ReadDataList.Add(val);
                    SeletedConnectorInfo.ConnInfo.SubscribeList.Add(val);
                    OnPropertyChanged(nameof(ReadDataList));
                }
            }
            else if (para == "EditRead")
            {
                if (SelectedData is null) { return; }
                GlobalValModel val = AddValueDialog((GlobalValModel)SelectedData.Clone());
                ReadDataList.Remove(SelectedData);
                ReadDataList.Add(val);
                SeletedConnectorInfo.ConnInfo.SubscribeList.Remove(SelectedData);
                SeletedConnectorInfo.ConnInfo.SubscribeList.Add(val);
                OnPropertyChanged(nameof(ReadDataList));
            }
            else if (para == "DeleteRead")
            {
                try
                {
                    if (!(SelectedData is null))
                    {
                        ReadDataList.Remove(SelectedData);
                        SeletedConnectorInfo.ConnInfo.SubscribeList.Remove(SelectedData);
                        OnPropertyChanged(nameof(ReadDataList));
                    }
                }
                catch { }
            }
            else if (para == "Read")
            {
                try
                {
                    if (SeletedConnectorInfo is null) { return; }
                    if (SelectedData is null) { return; }
                    GlobalValModel val = new GlobalValModel();
                    SeletedConnectorInfo.Recieve(ref val);
                    SelectedData = val;
                }
                catch (Exception ex) { Info = ex.Message; }
            }
        }
        public IRelayCommand<string> WriteCommand { get; }
        private void readCommand(string para)
        {
            if (para == "SaveProject")
            {
                System.Diagnostics.Debug.Print(AddValueDialog().ToString());
            }
        }
        public IRelayCommand<string> ReadCommand { get; }
        // 保存配置
        private void hostCommand(string para)
        {
            if (para == "SaveProject")
            {
                List<ConnectorInfo> list = ConnectorInfos.Select(o=>o.ConnInfo).ToList();
                List<DeviceInfo> list1 = ConnectorInfos.Select(o => o.DevInfo).ToList();
                MainModel.Ins.EditProject(list, "ConnectorInfo");
                MainModel.Ins.EditProject(list1, "DeviceInfo");
            }
            else if (para == "LoadProject")
            {
                List<ConnectorInfo> infoc = (List<ConnectorInfo>)MainModel.Ins.LoadProject("ConnectorInfo");
                List<DeviceInfo> infod = (List<DeviceInfo>)MainModel.Ins.LoadProject("DeviceInfo");

                for (int i = 0; i < infoc.Count; i++)
                {
                    IConnector conn = ConnectorFactory.GetDevice(infoc[i]);
                    conn.DevInfo = infod[i];
                    ConnectorInfos.Add(conn);
                }
                OnPropertyChanged(nameof(ConnectorInfos));
            }
        }
        public IRelayCommand<string> HostCommand { get; }
    }

    // TODO useless ?
    /*public class ObsDeviceInfo : ObservableObject
    {
        public string Name { get => name; set { SetProperty(ref name, value); } }
        private string name;
        public string Ip { get => ip; set { SetProperty(ref ip, value); } }
        private string ip;
        public int Port { get => port; set { SetProperty(ref port, value); } }
        private int port;
        public bool IsConnected { get => isConnected; set { SetProperty(ref isConnected, value); } }
        private bool isConnected;

        public IConnection Conn;
        public ObsDeviceInfo(ref IConnection conn) { Conn = conn; }
    }*/
}
