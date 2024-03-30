using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Camera;

namespace CatVision.Wpf.Models.Data
{
    public class GlobalDataFlow
    {
        public static Dictionary<string, GlobalValModel> DataList => dataList;
        private static Dictionary<string, GlobalValModel> dataList = new Dictionary<string, GlobalValModel>();
        public static void AddGlobalVar(GlobalValModel var)
        {
            if (dataList.ContainsKey(var.address)) return;
            dataList.Add(var.address, var);
        }
        public static void EditGlobalVar(GlobalValModel var)
        {
            if (!dataList.ContainsKey(var.address)) return;
            dataList.Remove(var.address);
            dataList.Add(var.address, var);
        }
        public static void DeleteGlobalVar(string address)
        {
            if (!dataList.ContainsKey(address)) return;
            dataList.Remove(address);
        }
        public static void ClearGlobalVar()
        {
            dataList.Clear();
        }
    }
}
