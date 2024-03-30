using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using CatVision.Wpf.Models.Data;
using CatVision.Common.Const;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Camera;
using CatVision.Common.Helper;
using CatVision.PluginSystem;
using CatVision.Communication;
using CatVision.Wpf.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HalconDotNet;
using Microsoft.Win32;

namespace CatVision.Wpf.Models
{
    public delegate void DefaultDelegate(HObject Image, ProductPara para, out List<GlobalValModel> Result, HTuple WindowHandle = null);
    public class MainModel
    {
        private static readonly MainModel instance = new MainModel();
        public static MainModel Ins => instance;
        //
        public List<GlobalValModel> GlobalVarList; // 全局变量
        public List<CameraInfo> CamInfoList; // 相机配置
        List<ICamera<HObject>> CamList = new List<ICamera<HObject>>();
        public List<CameraPara> ParaInfoList; // 产品参数
        public List<ConnectorInfo> ConnInfoList; // 通信参数
        public List<DeviceInfo> DevInfoList; // 设备参数：和通信参数一一对应
        List<IConnector> ConnectorList = new List<IConnector>();
        public Dictionary<string, Dictionary<string, DefaultDelegate>> TypeMethodDict { get; set; }
        public MainModel()
        {
            /*string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), SoftwareInfo.SoftwareName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            new ProjectDb(Path.Combine(path, "CatVisionTest.pro")).CreateProject(string.Empty);*/
        }
        public void InitConfig()
        {
            UIConfig.Ins.LoadConfigFile();
            // 授权
            LisenceHelper.MyVerifyLisence(UIConfig.Ins.License);
            // uiconfig添加用户 读取工程文件
            // 登录
            // todo: 先验证初始账户
            LoginModel.Ins.AddOperator("operator", "");
            //LoginModel.Ins.AddUser(2, "admin", LisenceHelper.MyGetUUIDShort());
            
            if (UIConfig.Ins.ProjectAutoLoad && File.Exists(UIConfig.Ins.ProjectFileFullName))
            {
                // 加载工程文件
                new ProjectDb(UIConfig.Ins.ProjectFileFullName).CreateProject(string.Empty);
                // 从工程文件读取配置
                GlobalVarList = (List<GlobalValModel>)LoadProject("GlobalVar");
                CamInfoList = (List<CameraInfo>)LoadProject("Cameras");
                ParaInfoList = (List<CameraPara>)LoadProject("CamPara");
                DevInfoList = (List<DeviceInfo>)LoadProject("DeviceInfo");
                ConnInfoList = (List<ConnectorInfo>)LoadProject("ConnectorInfo");
                LoadProject("Compiler");
                if (UIConfig.Ins.ProjectAutoRun)
                {
                    // 初始化相机
                    foreach (CameraInfo info in CamInfoList)
                    {
                        CamList.Add(CameraFactory.GetInstance(info));
                        // todo avalondock
                    }
                    // 初始化通信设备
                    for (int i = 0; i < ConnInfoList.Count; i++)
                    {
                        IConnector conn = ConnectorFactory.GetDevice(ConnInfoList[i]);
                        conn.DevInfo = DevInfoList[i];
                        ConnectorList.Add(conn);
                    }
                    // 编译算法脚本
                    if (!RoslynCompiler.Ins.CompileSuccess)
                    {
                        List<Type> type = new List<Type> { typeof(CameraPara) };
                        string halconRoot = Environment.GetEnvironmentVariable("HALCONROOT");
                        if (!string.IsNullOrEmpty(halconRoot))
                        {
                            List<string> third = new List<string> { System.IO.Path.Combine(halconRoot, "bin\\dotnet35\\halcondotnet.dll") };
                            RoslynCompiler.Ins.InitCompiler(type, third);
                            RoslynCompiler.Ins.CompileCode(RoslynCompiler.Ins.RowCode);
                            RoslynCompiler.Ins.EnumResult(typeof(DefaultDelegate));
                        }
                        // 保存方法引用到本地
                        TypeMethodDict = new Dictionary<string, Dictionary<string, DefaultDelegate>>();
                        foreach (var t in RoslynCompiler.Ins.TypeMethodDict)
                        {
                            TypeMethodDict.Add(t.Key, new Dictionary<string, DefaultDelegate>());
                            foreach (var f in t.Value)
                            {
                                TypeMethodDict[t.Key].Add(f.Key, (DefaultDelegate)f.Value);
                            }
                        }
                    }
                    // 打开相机
                    foreach (ICamera<HObject> cam in CamList) { cam.Init(); cam.Connect(cam.CamInfo); }
                    // 连接设备
                    foreach (IConnector conn in ConnectorList) { conn.Init(); conn.Connect(conn.ConnInfo); }
                    // 全线开启 todo test
                    foreach (ICamera<HObject> cam in CamList)
                    {
                        ProductPara para = ParaInfoList[0].prod_paras[0];
                        cam.ImageHandle += (img) =>
                        {
                            List<GlobalValModel> vals = new List<GlobalValModel>();
                            try
                            {
                                TypeMethodDict.FirstOrDefault().Value.FirstOrDefault().Value.Invoke(img, para, out vals);
                            }
                            catch { }
                            MainDockView.Ins.DispImage(cam.CamInfo.Name, img);
                        };
                    };
                }
                // UI 配置
                if (UIConfig.Ins.AutoLoadLayout) { MainDockView.Ins.loadpanel(); }
                else
                {
                    foreach (CameraInfo info in CamInfoList)
                    {
                        MainDockView.Ins.AddCameraPanel(info.Name);
                    }
                }
                // TODO 全局消息通知 日志记录 纠错
            }
        }
        // todo log message
        
        // 子页面 的 工程编辑和加载
        public void EditProject(object obj, string name)
        {
            string str;
            if (name == "Compiler")
            {
                if (string.IsNullOrWhiteSpace(RoslynCompiler.Ins.RowCode)) { return; }
                else { str = RoslynCompiler.Ins.RowCode; }
            }
            else { str = JsonConvert.SerializeObject(obj, Formatting.Indented); }
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buf = Encoding.Default.GetBytes(str);
                ms.Write(buf, 0, buf.Length);
                ms.Position = 0;
                if (name == "Cameras") { ProjectDb.Ins.UploadFile("Cameras", ms, cam_para: true); }
                else if (name == "CamPara") { ProjectDb.Ins.UploadFile("CamPara", ms, product_para: true); }
                else if (name == "GlobalVar") { ProjectDb.Ins.UploadFile("GlobalVar", ms, global_val: true); }
                else if (name == "ConnectorInfo") { ProjectDb.Ins.UploadFile("ConnectorInfo", ms, device_para: true); }
                else if (name == "DeviceInfo") { ProjectDb.Ins.UploadFile("DeviceInfo", ms, device_para: true); }
                else if (name == "Compiler") { ProjectDb.Ins.UploadFile("Code.cs", ms, csharp: true); }
            }
            if (name == "Compiler" && RoslynCompiler.Ins.CompileSuccess)
            {
                using (MemoryStream ms = new MemoryStream(RoslynCompiler.Ins.rawAssemblyBytes))
                {
                    //ms.Write(RoslynCompiler.Ins.rawAssemblyBytes, 0, RoslynCompiler.Ins.rawAssemblyBytes.Length);
                    //ms.Position = 0;
                    ProjectDb.Ins.UploadFile("Code.dll", ms, dll: true);
                }
            }
        }
        public object LoadProject(string name)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (name == "Cameras") { ProjectDb.Ins.DownloadFile("Cameras", ms, cam_para: true); }
                else if (name == "CamPara") { ProjectDb.Ins.DownloadFile("CamPara", ms, product_para: true); }
                else if (name == "GlobalVar") { ProjectDb.Ins.DownloadFile("GlobalVar", ms, global_val: true); }
                else if (name == "ConnectorInfo") { ProjectDb.Ins.DownloadFile("ConnectorInfo", ms, device_para: true); }
                else if (name == "DeviceInfo") { ProjectDb.Ins.DownloadFile("DeviceInfo", ms, device_para: true); }
                else if (name == "Compiler") { ProjectDb.Ins.DownloadFile("Code.cs", ms, csharp: true); }
                byte[] buf = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buf, 0, buf.Length);
                string str = Encoding.Default.GetString(buf);
                if (string.IsNullOrEmpty(str)) { return null; }
                if (name == "Compiler")
                {
                    RoslynCompiler.Ins.RowCode = str;
                    using (MemoryStream ms1 = new MemoryStream())
                    {
                        ms1.Position = 0;
                        ProjectDb.Ins.DownloadFile("Code.dll", ms1, dll: true);
                        ms1.Seek(0, SeekOrigin.Begin);
                        if (ms1.Length != 0)
                        {
                            RoslynCompiler.Ins.rawAssemblyBytes = ms1.ToArray();
                            RoslynCompiler.Ins.assemblyResult = Assembly.Load(RoslynCompiler.Ins.rawAssemblyBytes);
                            RoslynCompiler.Ins.CompileSuccess = true;
                            RoslynCompiler.Ins.EnumResult(typeof(DefaultDelegate));
                        }
                        return 1;
                    }
                }
                object obj;
                if (name == "Cameras") { obj = JsonConvert.DeserializeObject<List<CameraInfo>>(str); }
                else if (name == "CamPara") { obj = JsonConvert.DeserializeObject<List<CameraPara>>(str); }
                else if (name == "GlobalVar") { obj = JsonConvert.DeserializeObject<List<GlobalValModel>>(str); }
                else if (name == "ConnectorInfo") { obj = JsonConvert.DeserializeObject<List<ConnectorInfo>>(str); }
                else if (name == "DeviceInfo") { obj = JsonConvert.DeserializeObject<List<DeviceInfo>>(str); }
                else { obj = JsonConvert.DeserializeObject(str); }
                return obj;
            }
        }

        // 主界面菜单栏 的工程打开关闭等
        public void NewProject()
        {
            ProjectDb.Ins.Dispose();
            MyProject.CreateNewProject();
        }
        public void OpenProject()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有项目文件 | *.pro; *.db;";
                if (openFileDialog.ShowDialog() == true)
                {
                    ProjectDb.Ins.CreateProject(openFileDialog.FileName);
                    MyProject.CreateNewProject();
                }
            }
            catch { }
        }
        public void SaveProject()
        {
            if (MyProject.Ins.projectFileState == ProjectFileState.IsNew)
            {
                SaveProjectAs();
            }
            else { }
        }
        public void SaveProjectAs()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = Path.GetDirectoryName(ProjectDb.Ins.GetFilePath());
                sfd.Filter = "所有项目文件 | *.pro; *.db;";
                if (sfd.ShowDialog() == true)
                {
                    ProjectDb.Ins.SaveAs(sfd.FileName);
                }
            }
            catch { }
        }
        public void CloseProject()
        {

        }
    }
}
