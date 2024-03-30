using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.CodeCompletion;
using CatVision.Common.Model;
using HalconDotNet;

namespace CatVision.Wpf.Models
{
    /// <summary>
    /// This is a simple script provider that adds a few using statements to the C# scripts (.csx files)
    /// </summary>
    class ScriptProvider : ICSharpScriptProvider
    {
        public string GetUsing()
        {
            return "" +
                "using System; " +
                "using System.Collections.Generic; " +
                "using System.Linq; " +
                "using System.Text; ";
        }


        public string GetVars()
        {
            return "int age = 25;";
        }

        public string GetNamespace() => null;
    }
}
