using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using CatVision.Common.Model;

namespace CatVision.PluginSystem
{
    public class CsharpCompiler
    {
        public static void example()
        {
            List<Type> type = new List<Type> { typeof(CameraPara) };
            List<string> third = new List<string> { "$HALCONROOT\\bin\\dotnet35\\halcondotnet.dll" };
            // TODO:不用单例模式
            RoslynCompiler.Ins.InitCompiler(type, third);
            if (!RoslynCompiler.Ins.CompileCode(ScriptHelper.ScriptSample))
            {
                System.Diagnostics.Debug.Print(RoslynCompiler.Ins.ErrorStrs);
            }
            else
            {
                //RoslynCompiler.Ins.EnumResult(); // todo
                var func = RoslynCompiler.Ins.TypeMethodDict["ScriptExample"]["ExampleFunc"];
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
            }
        }
    }
    public class RoslynCompiler
    {
        private static readonly Lazy<RoslynCompiler> instance = new Lazy<RoslynCompiler>(() => new RoslynCompiler());
        public static RoslynCompiler Ins { get => instance.Value; }
        public string RowCode;
        //private List<string> listSystemReferences = new List<string>();
        private List<string> listThirdReferences = new List<string>();
        //private ArrayList thirdReferences = new ArrayList { typeof(object), typeof(Console) };
        private List<Type> listCustomReferences = new List<Type>();
        //private ArrayList customReferences = new ArrayList { "mscorlib.dll", "System.Runtime.dll", };
        private StringBuilder ResultStrs = new StringBuilder();
        private CSharpCompilationOptions options;
        private List<MetadataReference> references = new List<MetadataReference>();
        public Assembly assemblyResult = null;
        public byte[] rawAssemblyBytes = null;
        private bool CompilerInited = false;
        public bool CompileSuccess = false;
        // 弃用
        private Dictionary<string, Delegate> funcCollection = new Dictionary<string, Delegate>();
        private Dictionary<string, Dictionary<string, Delegate>> typeMethodDict = new Dictionary<string, Dictionary<string, Delegate>>();

        //public List<string> SystemReferences => listSystemReferences;
        public List<string> ThirdReferences => listThirdReferences;
        public List<Type> CustomReferences => listCustomReferences;
        // 弃用
        // public Dictionary<string, Delegate> FuncCollection => funcCollection;
        public Dictionary<string, Dictionary<string, Delegate>> TypeMethodDict => typeMethodDict;
        public string ErrorStrs => ResultStrs?.ToString();
        private RoslynCompiler() { }
        // TODO: try catch
        public bool InitCompiler(List<Type> customRef, List<string> thirdRef, bool reCompile = false)
        {
            if ((!reCompile) && CompilerInited) return true;
            listCustomReferences = customRef;
            listThirdReferences = thirdRef;
            // 创建CSharpCompilationOptions实例，设置编译选项
            options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithWarningLevel(4);
#if DEBUG
            options.WithOptimizationLevel(OptimizationLevel.Debug);
#else
                options.WithOptimizationLevel(OptimizationLevel.Release);
#endif
            // 添加引用程序集
            // system reference
            // equal to System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
            DirectoryInfo coreDir = Directory.GetParent(typeof(object).Assembly.Location);
            references = new List<MetadataReference>()
                {
                    MetadataReference.CreateFromFile(Path.Combine(coreDir?.FullName, "mscorlib.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(coreDir?.FullName, "System.Runtime.dll")),
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // 引用System.dll(System.Private.CoreLib.dll)
                    MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location), // 引用System.Console.dll
                    MetadataReference.CreateFromFile(typeof(IList).Assembly.Location), // 引用System.Collections
                    MetadataReference.CreateFromFile(typeof(List<string>).Assembly.Location), // 引用System.Collections
                };
            // custom types such as typeof(MyClass)
            foreach (Type type in listCustomReferences)
            {
                references.Add(MetadataReference.CreateFromFile(type.Assembly.Location));
            }
            // third party dlls such as NewtonSoft.Json.dll
            foreach (string dll in listThirdReferences)
            {
                references.Add(MetadataReference.CreateFromFile(dll));
            }
            // todo load & unload dll file
            if (!CompilerInited) AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            CompilerInited = true;
            return true;
        }
        public bool CompileCode(string code)
        {
            RowCode = code;
            // 创建SyntaxTree实例
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            // 创建编译对象
            CSharpCompilation compilation = CSharpCompilation.Create("MyAssembly", new SyntaxTree[] { syntaxTree }, references, options);
            // 检查编译错误
            if (compilation.GetDiagnostics().Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
            {
                foreach (Diagnostic diagnostic in compilation.GetDiagnostics())
                {
                    if (!diagnostic.IsWarningAsError) ResultStrs.AppendLine(diagnostic.ToString());
                    //Console.WriteLine(diagnostic.ToString());
                }
                CompileSuccess = false;
                return false;
            }
            // 编译
            using (MemoryStream ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    rawAssemblyBytes = ms.ToArray();
                    assemblyResult = Assembly.Load(rawAssemblyBytes);
                    // TODO : UnLoad?
                }
                else
                {
                    foreach (Diagnostic diagnostic in result.Diagnostics)
                    {
                        if (!diagnostic.IsWarningAsError) ResultStrs.AppendLine(diagnostic.ToString());
                        //Console.WriteLine(diagnostic.ToString());
                    }
                    CompileSuccess = false;
                    return false;
                }
            }
            CompileSuccess = true;
            return true;
        }
        public void EnumResult(Type funcType)
        {
            if (!(assemblyResult is null))
            {
                List<Type> types = assemblyResult.ExportedTypes.TakeWhile(x => x.FullName.StartsWith("CatVision")).ToList();
                foreach (Type type in types)
                {
                    if (!typeMethodDict.ContainsKey(type.Name))
                    {
                        typeMethodDict.Add(type.Name, new Dictionary<string, Delegate>());
                    }
                    List<MethodInfo> methods = type.GetMethods().TakeWhile(x => x.IsStatic).ToList();
                    foreach (MethodInfo method in methods)
                    {
                        if (typeMethodDict[type.Name].ContainsKey(method.Name))
                        {
                            typeMethodDict[type.Name].Remove(method.Name);
                        }
                        typeMethodDict[type.Name].Add(method.Name, Delegate.CreateDelegate(funcType, null, method));
                    }
                }
            }
        }
        private bool EnumFunc(string typeName, string funcName, Type type)
        {
            if (assemblyResult != null)
            {
                Type scriptType = assemblyResult.GetType("CatVision.PluginSystem." + typeName);
                object scriptInstance = Activator.CreateInstance(scriptType);

                MethodInfo method = scriptType.GetMethod(funcName);
                if (null == method) return false;
                string funcname = $"{typeName}.{funcName}";
                if (funcCollection.ContainsKey(funcname))
                {
                    funcCollection.Remove(funcname);
                }
                funcCollection.Add($"{typeName}.{funcName}", Delegate.CreateDelegate(type, null, method));
                return true;
            }
            return false;
        }
        public bool SaveDllSream(Stream stream)
        {
            if (null == rawAssemblyBytes) return false;
            stream.Write(rawAssemblyBytes, 0, rawAssemblyBytes.Length);
            return true;
        }
        public bool SaveDllFile(string fileName)
        {
            if (null == rawAssemblyBytes) return false;
            if (File.Exists(fileName)) File.Delete(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            {
                fs.Write(rawAssemblyBytes, 0, rawAssemblyBytes.Length);
            }
            return true;
        }
        public bool LoadDllStream(Stream stream)
        {
            rawAssemblyBytes = new byte[stream.Length];
            stream.Read(rawAssemblyBytes, 0, rawAssemblyBytes.Length);
            // TODO
            //EnumResult();
            return true;
        }
        public bool LoadDllFile(string fileName)
        {
            if (!File.Exists(fileName)) return false;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                rawAssemblyBytes = new byte[fs.Length];
                fs.Read(rawAssemblyBytes, 0, rawAssemblyBytes.Length);
            }
            //EnumResult();
            return true;
        }
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly[] select = assemblies.Where(x => x.FullName.Equals(args.Name, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (select.Length == 0)
            {
                try
                {
                    string strDLLPath;
                    string strDLLName = $"{args.Name.Split(',')[0]}.dll";
                    Assembly assembly = null;
                    strDLLPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strDLLName);
                    if (File.Exists(strDLLPath))
                    {
                        assembly = Assembly.LoadFile(strDLLPath);
                        return assembly;
                    }
                    return null;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return select[0];
            }
        }
    }
}
