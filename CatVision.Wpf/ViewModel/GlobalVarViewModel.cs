using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Camera;
using CatVision.Wpf.Views;
using CatVision.Wpf.Models;
using CatVision.Wpf.Models.Data;

namespace CatVision.Wpf.ViewModel
{
    public class GlobalVarViewModel : ObservableObject
    {
        //private static readonly GlobalVarViewModel instance = new GlobalVarViewModel();
        //public static GlobalVarViewModel Ins { get => instance; }
        public GlobalVarViewModel()
        {
            EditCommand = new RelayCommand<string>(editCommand);
            HostCommand = new RelayCommand<string>(hostCommand);
        }

        public ObservableCollection<GlobalValModel> DataList { get; set; } = new ObservableCollection<GlobalValModel>(); 
        public GlobalValModel SelectedData
        {
            get => selectedData;
            set { SetProperty(ref selectedData, value); }
        }
        private GlobalValModel selectedData;

        public string Info { get => info; set { SetProperty(ref info, value); } }
        private string info = string.Empty;
        public IRelayCommand<string> EditCommand { get; }
        private void editCommand(string cmd)
        {
            if (cmd == "Add")
            {
                GlobalValModel val = AddValueDialog();
                if (val is null) return;
                if (GlobalDataFlow.DataList.ContainsKey(val.address)) return;
                GlobalDataFlow.AddGlobalVar(val);
                DataList.Add(val);
                OnPropertyChanged(nameof(DataList));
            }
            else if (cmd == "Delete")
            {
                if (!GlobalDataFlow.DataList.ContainsKey(SelectedData.address)) return;
                GlobalDataFlow.DeleteGlobalVar(SelectedData.address);
                DataList.Remove(SelectedData);
                OnPropertyChanged(nameof(DataList));
            }
            else if (cmd == "Edit")
            {
                if (SelectedData is null) { return; }
                GlobalValModel old = (GlobalValModel)SelectedData.Clone();
                GlobalValModel val = AddValueDialog((GlobalValModel)SelectedData.Clone());
                if (val is null) return;
                if (val.address != old.address) return; // warning
                if (GlobalDataFlow.DataList.ContainsKey(val.address))
                {
                    GlobalDataFlow.DataList[val.address] = (GlobalValModel)val.Clone();
                    //DataList.Where(o => o.address == val.address).First().SetFullValue(val); // bug : will not update until click and check for value
                    DataList.Remove(SelectedData);
                    DataList.Add(val);
                    SelectedData = val;
                }
                else
                {
                    /*GlobalDataFlow.DeleteGlobalVar(old.address);
                    GlobalDataFlow.AddGlobalVar(val);
                    DataList.Remove(old);
                    DataList.Add(val);
                    SelectedData = val;*/
                }
                OnPropertyChanged(nameof(DataList));
                // TODO : bug here will not update until click and check for value
            }
            else if (cmd == "Clear")
            {
                GlobalDataFlow.ClearGlobalVar();
                DataList.Clear();
                OnPropertyChanged(nameof(DataList));
            }
        }
        // 保存配置
        private void hostCommand(string para)
        {
            if (para == "SaveProject")
            {
                MainModel.Ins.EditProject(DataList.ToList(), "GlobalVar");
            }
            else if (para == "LoadProject")
            {
                List<GlobalValModel> list = (List<GlobalValModel>)MainModel.Ins.LoadProject("GlobalVar");
                DataList = new ObservableCollection<GlobalValModel>(list);
                OnPropertyChanged(nameof(DataList));
            }
        }
        public IRelayCommand<string> HostCommand { get; }

        private GlobalValModel AddValueDialog(GlobalValModel oldVal = null)
        {
            GlobalVarDialogViewModel varVm;
            if (oldVal is null) { varVm = new GlobalVarDialogViewModel(); }
            else { varVm = new GlobalVarDialogViewModel(oldVal); }
            GlobalValDialog dialog = new GlobalValDialog() { DataContext = varVm };
            dialog.ShowDialog();
            if (true == varVm.ValueConfirmed) return varVm.GValue;
            else return null;
        }
    }
}
