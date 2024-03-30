using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// DictEditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DictEditDialog : MetroWindow
    {
        public DictEditDialog()
        {
            InitializeComponent();
        }
    }

    public class DictEditViewModel : ObservableObject
    {
        public DictEditViewModel()
        {
            AddCommand = new RelayCommand<string>(addCommand);
        }
        public ObservableCollection<KeyValuePair<string, string>> Dict { get; set; } = new ObservableCollection<KeyValuePair<string, string>>();
        public KeyValuePair<string, string> SelectedPair { get; set; }
        public string Info { get => info; set { SetProperty(ref info, value); } }
        private string info;

        public string NewKey { get => newKey; set { SetProperty(ref newKey, value); } }
        private string newKey;
        public string NewValue { get => newValue; set { SetProperty(ref newValue, value); } }
        private string newValue;

        public IRelayCommand<string> AddCommand { get; }
        public void addCommand(string para)
        {
            if (para == "Add")
            {
                if (string.IsNullOrWhiteSpace(NewKey))
                {
                    Info = "Invalid key."; return;
                }
                if (Dict.Select(o=>o.Key).Contains(NewKey))
                {
                    Info = "Key exits."; return;
                }
                Dict.Add(new KeyValuePair<string, string>(NewKey, NewValue));
            }
            else if (para == "Delete")
            {
                Dict.Remove(SelectedPair);
            }
            OnPropertyChanged(nameof(Dict));
        }
        public void SetRefValue(Dictionary<string, string> dict)
        {
            if (dict is null || dict.Count == 0) { return; }
            foreach(var d in dict) { Dict.Add(d); }
        }
        public Dictionary<string, string> GetRefValue()
        {
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            foreach(var t in Dict) { tmp.Add(t.Key, t.Value); }
            return tmp;
        }
    }
}
