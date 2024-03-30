using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CatVision.Common.Enum
{
    public class EnumHelper
    {
        public static string[] GetNames(Type t)
        {
            return System.Enum.GetNames(t);
            //return System.Enum.GetValues(t);
        }
        public static T GetEnum<T>(string str)
        {
            //T res = default(T);
            //return (T)TypeDescriptor.GetConverter(res).ConvertFrom(str);
            return (T)System.Enum.Parse(typeof(T), str);
        }
        public static List<T> GetEnums<T>() where T : System.Enum
        {
            return System.Enum.GetValues(typeof(T)).OfType<T>().ToList();
        }
        public static List<string> GetEnumStrs<T>() where T : System.Enum
        {
            return System.Enum.GetValues(typeof(T)).OfType<T>().ToList().Select(x=>x.ToString()).ToList();
        }
        public static List<T> GetEnumStrs<T>(T value)
        {
            List<T> list = new List<T>();
            if (value is System.Enum)
            {
                var valData = Convert.ToInt32((T)System.Enum.Parse(typeof(T), value.ToString()));
                var tps = System.Enum.GetValues(typeof(T)).OfType<T>().ToList();
                list.AddRange(tps.Where(x => ((int)Convert.ToInt32((T)System.Enum.Parse(typeof(T), x.ToString())) & valData) == valData));
            }
            return list;
        }
        public static string GetEnumStr<T>(T t)
        {
            //string res = string.Empty;
            return TypeDescriptor.GetConverter(t).ConvertTo(t, typeof(string)).ToString();
            //return res;
        }
    }

    public enum UserLevel
    {
        Myself = 0,
        Developer = 1,
        Administrator = 2,
        Operator = 3
    }
    /// <summary>
    /// marray.ToString() = "00-01-02"
    /// </summary>
    public enum mValueType
    {
        mobj = 0,
        mbit = 1,
        mbyte = 8,
        mint = 16,
        mfloat = 32,
        mstr = 100,
        marray = 101
    }
    public enum ProjectFileState
    {
        IsNull = 0,
        IsLoad = 1,
        IsSaved = 2,
        IsNew = 3,
        IsModified = 4
    }
    public enum ProjectState
    {
        IsNull = 0,
        IsLoad = 10,
        IsInit = 11,
        IsRunning = 12,
        IsAuto = 13
    }
    public enum DeviceState
    {
        UnAvailable = 0,
        Available = 1,
        UnInitial = 10,
        Initial = 11,
        DisConnectted = 20,
        Connectted = 21
    }
    public enum WorkState
    {
        Error = 100,
        Warn = 101,
        Busy = 102,
        Ready = 103
    }
    public enum ROIType
    {
        circle = 0,
        rectangle1 = 1,
        rectangle2 = 2
    }
}
