using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HandyControl;
using HandyControl.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HalconDotNet;
using CatVision.Common;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Camera;
using CatVision.PluginSystem;
using CatVision.Wpf.Views;
using CatVision.Wpf.Models;
using CatVision.Wpf.Localization;

namespace CatVision.Wpf.ViewModel
{
    public class ScriptEditorViewModel : ObservableObject
    {
        //private static readonly ScriptEditorViewModel instance = new ScriptEditorViewModel();
        //public static ScriptEditorViewModel Ins { get => instance; }
        HObject Image;
        public ScriptEditorViewModel()
        {
            CompileCommand = new RelayCommand<string>(compileCommand);
            HostCommand = new RelayCommand<string>(hostCommand);
        }

        public Action<string> CodeChanged = null;
        //public EventHandler<string> CodeChanged = null;
        public string CsharpText { get => csharpText; set { SetProperty<string>(ref csharpText, value); } }
        private string csharpText = ScriptHelper.ScriptSample;
        
        public string Info { get => info; set { SetProperty(ref info, value); } }
        private string info = "info";
        /*public string Info
        {
            get => info;
            set { SetProperty<string>(ref info, value); ScriptEditorView.Ins.InfoEditor.Text = info; }
        }*/
        private string selectedType;
        public string SelectedType
        {
            get => selectedType;
            set { SetProperty<string>(ref selectedType, value); }
        }
        private string selectedMethod;
        public string SelectedMethod
        {
            get => selectedMethod;
            set { SetProperty<string>(ref selectedMethod, value); }
        }
        // TODO full name or name
        public ObservableCollection<string> TypeNameList { get; set; }
        public ObservableCollection<string> MethodNameList { get; set; }
        public void cmbTypeName_SelectionChanged()
        {
            var methodNameList = new ObservableCollection<string>();
            RoslynCompiler.Ins.TypeMethodDict[SelectedType].Keys.ToList().ForEach(o => methodNameList.Add(o));
            MethodNameList = methodNameList;
            OnPropertyChanged(nameof(MethodNameList));
        }
        public ObservableCollection<GlobalValModel> ResultData { get; set; } = new ObservableCollection<GlobalValModel>();
        // 编译功能区按钮
        public void compileCommand(string para)
        {
            if (para == "Compiled")
            {
                RoslynCompiler.Ins.EnumResult(typeof(DefaultDelegate));
                ObservableCollection<string> typeNameList = new ObservableCollection<string>();
                RoslynCompiler.Ins.TypeMethodDict.Keys.ToList().ForEach(o => typeNameList.Add(o));
                TypeNameList = typeNameList;
                OnPropertyChanged(nameof(TypeNameList));
                /*else
                {
                    var func = RoslynCompiler.Ins.FuncCollection["ScriptExample.ExampleFunc"];
                    object Image = 0, WindowHandle = 0;
                    CameraPara para = new CameraPara();
                    List<GlobalValModel> result = new List<GlobalValModel>();
                    // todo: out/ref/return
                    func.DynamicInvoke(Image, para.prod_paras[0], result, WindowHandle);
                    // todo: result 从list改为dictionary?
                    if (result.Count > 0)
                    {
                        System.Diagnostics.Debug.Print($"Result: {result.Find(x => x.address == "Image.Height").value}");
                    }
                }*/
            }
            else if (para == "Open")
            {
                Image = new HObject();
                Image.Dispose();
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        //Info = ofd.FileName;
                        HOperatorSet.ReadImage(out Image, ofd.FileName);
                    }
                }
            }
            else if (para == "Test")
            {
                try
                {
                    //RoslynCompiler.Ins.EnumFunc("ScriptExample", "ExampleFunc", typeof(DefaultDelegate));
                    //RoslynCompiler.Ins.EnumResult();
                    //DefaultDelegate func = (DefaultDelegate)RoslynCompiler.Ins.FuncCollection["ScriptExample.ExampleFunc"];
                    if (string.IsNullOrEmpty(SelectedType) || string.IsNullOrEmpty(selectedMethod))
                    {
                        HandyControl.Controls.MessageBox.Show(LangHelper.GetLocStr("NullTypeOrMethod"));
                        return;
                    }
                    if (Image is null)
                    {
                        HandyControl.Controls.MessageBox.Show(LangHelper.GetLocStr("NullImage"));
                        return;
                    }
                    //var tmp = RoslynCompiler.Ins.TypeMethodDict[SelectedType][selectedMethod];
                    DefaultDelegate func = (DefaultDelegate)RoslynCompiler.Ins.TypeMethodDict[SelectedType][selectedMethod];
                    HTuple WindowHandle = null;
                    CameraPara par = new CameraPara() { prod_paras = new List<ProductPara>() { new ProductPara() } };
                    List<GlobalValModel> result = new List<GlobalValModel>();
                    // todo: out/ref/return
                    func(Image, par.prod_paras[0], out result, WindowHandle);
                    ObservableCollection<GlobalValModel> tmp = new ObservableCollection<GlobalValModel>();
                    result.ForEach(o => tmp.Add(o));
                    ResultData = tmp;
                    OnPropertyChanged(nameof(ResultData));
                    /*if (result.Count > 0)
                    {
                        System.Diagnostics.Debug.Print("----------------");
                        foreach (var res in result)
                        {
                            System.Diagnostics.Debug.Print(res.ToString());
                        }
                    }*/
                }
                catch (Exception ex) { System.Diagnostics.Debug.Print(ex.Message); }
            }
        }
        public IRelayCommand<string> CompileCommand { get; }
        // 保存配置
        private void hostCommand(string para)
        {
            if (para == "SaveProject")
            {
                MainModel.Ins.EditProject(1, "Compiler");
            }
            else if (para == "LoadProject")
            {
                MainModel.Ins.LoadProject("Compiler");
                CsharpText = RoslynCompiler.Ins.RowCode;
                CodeChanged?.BeginInvoke(CsharpText, null, null);
                if (RoslynCompiler.Ins.CompileSuccess)
                {
                    TypeNameList = new ObservableCollection<string>(RoslynCompiler.Ins.TypeMethodDict.Keys.ToList());
                    OnPropertyChanged(nameof(TypeNameList));
                }
            }
        }
        public IRelayCommand<string> HostCommand { get; }
    }
}
