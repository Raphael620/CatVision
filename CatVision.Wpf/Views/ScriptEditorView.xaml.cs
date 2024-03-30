using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
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
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.CodeCompletion;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.PluginSystem;
using CatVision.Wpf.Models;
using CatVision.Wpf.ViewModel;

namespace CatVision.Wpf.Views
{
    /// <summary>
    /// GlobalValView.xaml 的交互逻辑
    /// </summary>
    public partial class ScriptEditorView : MetroWindow
    {
        //private static ScriptEditorView instance = new ScriptEditorView();
        //public static ScriptEditorView Ins => instance;
        private ScriptEditorViewModel vm; // = ScriptEditorViewModel.Ins;
        private CSharpCompletion completion;
        public ScriptEditorView()
        {
            InitializeComponent();
            vm = (ScriptEditorViewModel)this.DataContext;
            vm.CodeChanged += (code) => { Dispatcher.Invoke(() => { editor.Text = code; }); };
            //vm.CodeChanged += (sender, text) => { editor.Text = Code; };
        }
        public string Code;
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            completion = new CSharpCompletion(new ScriptProvider(), ScriptHelper.AssemblySample);
            editor.Completion = completion;
            editor.FontSize = 15;
            editor.FontFamily = new FontFamily("Consolas");
            editor.Foreground = Brushes.White;
            editor.Background = new SolidColorBrush(Color.FromArgb(1, 25, 25, 25));
            editor.Options.EnableEmailHyperlinks = false;
            editor.Options.EnableHyperlinks = false;

            XmlReader reader = XmlReader.Create(CatVision.Wpf.Resources.OpenStream("CSharp-Mode-Dark.xshd"));
            //editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            InfoEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
            InfoEditor.Options.EnableEmailHyperlinks = false;
            InfoEditor.Options.EnableHyperlinks = false;
            editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            // editor.SetCsharpText(vm.CsharpText);
            
            editor.Document.FileName = "随便给个名字";
            // TODO 引用是个问题 -> GetCodeStrings?
            editor.Text = vm.CsharpText;
            editor.IsModified = true;
        }

        private void btnCompile_Click(object sender, RoutedEventArgs e)
        {
            CompileCode();
        }
        public void CompileCode()
        {
            List<Type> type = new List<Type> { typeof(CameraPara) };
            string halconRoot = Environment.GetEnvironmentVariable("HALCONROOT");
            if (string.IsNullOrEmpty(halconRoot))
            {
                InfoEditor.Text = "HALCON not found";
                return;
            }
            List<string> third = new List<string> { System.IO.Path.Combine(halconRoot, "bin\\dotnet35\\halcondotnet.dll") };
            InfoEditor.Text = "Compiling...";
            RoslynCompiler.Ins.InitCompiler(type, third);
            vm.CsharpText = editor.Text;
            if (!RoslynCompiler.Ins.CompileCode(vm.CsharpText))
            {
                //System.Diagnostics.Debug.Print(RoslynCompiler.Ins.ErrorStrs);
                InfoEditor.Text = RoslynCompiler.Ins.ErrorStrs;
            }
            else
            {
                InfoEditor.Text = "Success";
                vm.compileCommand("Compiled");
            }
        }
        private void cmbTypeName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.cmbTypeName_SelectionChanged();
        }
        private void cmbMethodName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (!(vm is null)) { vm.CodeChanged = null; }
            }
            catch { }
        }
    }
}
