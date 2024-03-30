using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Common.Const;
using CatVision.Common.Helper;

namespace CatVision.Wpf.ViewModel
{
    public class GlobalVarDialogViewModel : ObservableObject
    {
        //private static readonly GlobalVarDialogViewModel instance = new GlobalVarDialogViewModel();
        //public static GlobalVarDialogViewModel Ins { get => instance; }
        public GlobalVarDialogViewModel()
        {
            HostCommand = new RelayCommand<string>(hostCommand);
        }
        public GlobalVarDialogViewModel(GlobalValModel val)
        {
            HostCommand = new RelayCommand<string>(hostCommand);
            type = val.type; mvalue = val.value?.ToString(); address = val.address; record = val.record;
            gVar = new GlobalValModel(type, record, address, val.value);
        }
        public bool? ValueConfirmed = null;

        private List<string> typeEnums = EnumHelper.GetEnumStrs<mValueType>();
        public List<string> TypeEnums
        {
            get => typeEnums;
            set { SetProperty<List<string>>(ref typeEnums, value); }
        }
        private GlobalValModel gVar = new GlobalValModel();
        public GlobalValModel GValue
        {
            get => gVar;
            set { SetProperty<GlobalValModel>(ref gVar, value); }
        }
        private string type;
        public string Type { get => type; set { GValue.type = value; SetProperty<string>(ref type, value); } }
        
        private string record;
        public string Record { get => record; set { GValue.record = value; SetProperty<string>(ref record, value); } }
        
        private string address;
        public string Address { get => address; set { GValue.address = value; SetProperty<string>(ref address, value); } }
        
        private string mvalue;
        public string mValue { get => mvalue;
            set
            {
                if (GValue.SetTypedValue(value))
                {
                    SetProperty<string>(ref mvalue, value);
                }
            }
        }

        private void hostCommand(string para)
        {
            if (para == "Clear")
            {
                Type = null;
                Record = null;
                Address = null;
                mValue = null;
                GValue = new GlobalValModel();
            }
            else if (para == "Confirm") { }
        }
        public IRelayCommand<string> HostCommand { get; }
    }
}
