using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using CatVision.Common.Helper;

namespace CatVision.Wpf.Models.Data
{
    public class GlobalStat
    {
        private static GlobalStat instance = new GlobalStat();
        public static GlobalStat Ins => instance;
    }
    public class MyProject
    {
        private static MyProject instance = new MyProject();
        public static MyProject Ins => instance;
        public ProjectState projectState { get; set; } = ProjectState.IsNull;
        public ProjectFileState projectFileState { get; set; } = ProjectFileState.IsNull;
        // TODO: where to init projectDb
        public ProjectDb projectDb = ProjectDb.Ins;
        public MyProject() { }
        public static MyProject CreateNewProject()
        {
            instance = new MyProject();
            return Ins;
        }
    }
}
