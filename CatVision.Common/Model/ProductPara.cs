using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CatVision.Common.Enum;

namespace CatVision.Common.Model
{
    [Serializable]
    public class ROI
    {
        public string roi_name { get; set; }
        public ROIType roi_type { get; set; }
        public double row { get; set; }
        public double column { get; set; }
        public double phi { get; set; }
        public double length1 { get; set; }
        public double length2 { get; set; }
        public ROI() { }
        public ROI(string _roi_name, ROIType _roi_type, double _row, double _column, double _phi, double _length1, double _length2)
        {
            roi_name = _roi_name; roi_type = _roi_type; row = _row; column = _column; phi = _phi; length1 = _length1; length2 = _length2;
        }
        public ROI(string _roi_name, int _roi_type, double _row, double _column, double _phi, double _length1, double _length2)
        {
            roi_name = _roi_name; roi_type = (ROIType)_roi_type; row = _row; column = _column; phi = _phi; length1 = _length1; length2 = _length2;
        }
        public ROI(string _roi_name, string _roi_type, double _row, double _column, double _phi, double _length1, double _length2)
        {
            roi_name = _roi_name;
            roi_type = (ROIType)System.Enum.Parse(typeof(ROIType), _roi_type);
            row = _row;
            column = _column;
            phi = _phi;
            length1 = _length1;
            length2 = _length2;
        }
        public string GetValueArrayStr()
        {
            switch (roi_type)
            {
                case ROIType.circle:
                    return string.Format(@"[{0:0.###},{1:0.###},{2:0.###}]", row, column, phi);
                case ROIType.rectangle1:
                    return string.Format(@"[{0:0.###},{1:0.###},{2:0.###},{3:0.###}]", row, column, length1, length2);
                case ROIType.rectangle2:
                    return string.Format(@"[{0:0.###},{1:0.###},{2:0.###},{3:0.###},{4:0.###}]", row, column, phi, length1, length2);
            }
            return string.Format(@"[{0:0.###},{1:0.###},{2:0.###},{3:0.###},{4:0.###}]", row, column, phi, length1, length2);
        }
    }
    [Serializable]
    public class ProductPara
    {
        public string prod_name { get; set; }
        public string prod_alias { get; set; } = string.Empty;
        public int expo_time { get; set; } = 0;
        public List<ROI> rois { get; set; } = new List<ROI>();
        public Dictionary<string, string> addin_dict { get; set; } = new Dictionary<string, string>();
        public ProductPara() { }
    }
    [Serializable]
    public class CameraPara
    {
        public string camera_name { get; set; }
        public string camera_alias { get; set; } = string.Empty; // TODO 和camerainfo的alias重复 -> 用caminfo来初始化
        public List<ProductPara> prod_paras { get; set; } = new List<ProductPara>();
        public Dictionary<string, string> addin_dict { get; set; } = new Dictionary<string, string>();
        public CameraPara() { }
    }
    
    [Serializable]
    public class ResultData
    {
        public DateTime datetime { get; set; }
        public string camera { get; set; }
        public string product { get; set; }
        public object[] values { get; set; }
        [JsonIgnore]
        public int FullCount => ValueCount + 3;
        [JsonIgnore]
        public int ValueCount => values.Length;
        public ResultData() { }
        public ResultData(int valueCount) { values = new object[valueCount]; }
        public ResultData(object[] val) { values = val; }
        public ResultData(string cam, string prod, object[] val)
        {
            datetime = DateTime.Now; camera = cam; product = prod; values = val;
        }
        public ResultData(DateTime dt, string cam, string prod, object[] val)
        {
            datetime = dt; camera = cam; product = prod; values = val;
        }
        public string GetSpliteStr(string split = ";")
        {
            return string.Join(split, GetStr());
        }
        public string[] GetStr(string dtFormat = "yyyy-MM-dd HH:mm:ss.fff")
        {
            List<string> str = new List<string>() { datetime.ToString(dtFormat), camera, product };
            foreach (object o in values) { str.Add(o.ToString()); }
            return str.ToArray();
        }
    }

    [Serializable]
    public class SystemConfig
    {
        public bool save_data = true;
        public bool save_image = true;
        public bool save_log = true;
        public bool show_image = true;
        public string log_dir;
        public string img_dir;
        public string data_dir;
        public SystemConfig()
        {
            log_dir = Path.Combine(Directory.GetCurrentDirectory(), "log");
            img_dir = Path.Combine(Directory.GetCurrentDirectory(), "image");
            data_dir = Path.Combine(Directory.GetCurrentDirectory(), "data");
        }
    }
}
