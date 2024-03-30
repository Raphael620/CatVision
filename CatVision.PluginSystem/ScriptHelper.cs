using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CatVision.Common;

namespace CatVision.PluginSystem
{
    public class ScriptHelper
    {
        public static Assembly[] AssemblySample = new Assembly[]
        {
                typeof(object).Assembly, //mscorlib
                typeof(Uri).Assembly, //System.dll
                typeof(Enumerable).Assembly, //System.Core.dll
                //typeof(HalconDotNet.HalconAPI).Assembly,
                typeof(Log).Assembly,
        };
        public static string ScriptSample = @"using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HalconDotNet;
using CatVision.Common.Enum;
using CatVision.Common.Model;

namespace CatVision.PluginSystem
{
    public class ScriptExample
    {
        // 必须是静态方法，参数表定义和示例完全相同
        public static void ExampleFunc(HObject Image, ProductPara para, out List<GlobalValModel> Result, HTuple WindowHandle = null)
        {
            // 初始化结果列表
            Result = new List<GlobalValModel>();
            // 算法主体变量定义
            HTuple width = new HTuple(), height = new HTuple();
            try
            {
                // 算法主体
                // string roi_name = para.rois[0].roi_name; // 参数使用方法
                // double threshold = double.Parse(para.addin_dict[""my_addin_key""]); // 自定义参数使用方法
                HOperatorSet.GetImageSize(Image, out width, out height);
                // 结果转换和赋值
                // value和ValueType要对应，HTuple等类型不受支持
                Result.Add(new GlobalValModel(mValueType.mint, ""图像宽度"", ""Image.Width"", width.I));
                Result.Add(new GlobalValModel(""Image.Height"", height.I));
                // 结果显示
                // 追求极致性能时，脚本结果不作显示
                if (!(WindowHandle is null))
                {
                    HOperatorSet.WriteString(WindowHandle, $""图像尺寸：{width.I}x{height.I}"");
                }
            }
			finally
            {
        	    // 清理内存
        	    width.Dispose(); height.Dispose();
            }
        }
    }
}
";
        public static string ValueTypeScript = "";
        public static string ParameterScript = "";
    }
}
