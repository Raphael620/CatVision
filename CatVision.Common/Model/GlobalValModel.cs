using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CatVision.Common.Enum;

namespace CatVision.Common.Model
{
    [Serializable]
    public class GlobalValModel : object, ICloneable
    {
        public string type { get; set; } /// 类型，详见 enum.mValueType
        public string record { get; set; } /// 描述，.Net8中将升级为record
        public string address { get; set; } /// 地址
        public object value { get; set; }
            /*get {
                if (this.value.GetType() == typeof(byte[])) { return BitConverter.ToString((byte[])this.value); }
                else { return this.value; }
            } set { } }*/ /// 值，将仅支持BsonValue，详见 enum.mValueType // TODO 新打包project方式

        public Type valueType;
        public GlobalValModel() { }
        public GlobalValModel(string add, object val) { address = add; value = val; }
        public GlobalValModel(string _type, string description, string add, object val)
        {
            type = _type; record = description; address = add; value = val;
        }
        public GlobalValModel(uint _type, string description, string add, object val)
        {
            type = ((mValueType)_type).ToString(); record = description; address = add; value = val;
        }
        /// <summary>
        /// 完整的自定义数据结构
        /// </summary>
        /// <param name="_type">数据类型，可空，为空时value按照string处理</param>
        /// <param name="description">描述，可空</param>
        /// <param name="add">名称/地址，变量的唯一标识</param>
        /// <param name="val">值，仅mValueType枚举中的类型受支持</param>
        public GlobalValModel(mValueType _type, string description, string add, object val)
        {
            type = _type.ToString(); record = description; address = add; value = val;
        }
        public object GetTypedValue()
        {
            if (value is null) return null;
            switch (EnumHelper.GetEnum<mValueType>(type))
            {
                case mValueType.mobj:
                    return value;
                case mValueType.mbit:
                    return (bool)value;
                case mValueType.mbyte:
                    return (byte)value;
                case mValueType.mint:
                    return (Int16)value;
                case mValueType.mfloat:
                    return (Single)value;
                case mValueType.mstr:
                    return (string)value;
                case mValueType.marray:
                    return (byte[])value;
                default:
                    return value;
            }
        }
        public bool SetTypedValue(string val)
        {
            if (string.IsNullOrEmpty(val)) { return false; }
            try
            {
                switch (EnumHelper.GetEnum<mValueType>(type))
                {
                    case mValueType.mobj:
                        value = val;
                        break;
                    case mValueType.mbit:
                        value = bool.Parse(val);
                        break;
                    case mValueType.mbyte:
                        value = byte.Parse(val);
                        break;
                    case mValueType.mint:
                        value = int.Parse(val);
                        break;
                    case mValueType.mfloat:
                        value = float.Parse(val);
                        break;
                    case mValueType.mstr:
                        value = val;
                        break;
                    case mValueType.marray:
                        if (string.IsNullOrEmpty(val)) { value = new byte[] { }; break; }
                        string[] strs = val.Split('-');
                        byte[] buf = new byte[strs.Length];
                        try
                        {
                            for (int i = 0; i < strs.Length; i++) { buf[i] = Convert.ToByte(strs[i], 16); }
                        }
                        catch { }
                        value = buf;
                        break;
                    default:
                        value = val;
                        break;
                }
                return true;
            }
            catch { return false; }
        }
        public Type GetmType() 
        {
            switch (EnumHelper.GetEnum<mValueType>(type))
            {
                case mValueType.mobj:
                    valueType = typeof(object);
                    break;
                case mValueType.mbit:
                    valueType = typeof(bool);
                    break;
                case mValueType.mbyte:
                    valueType = typeof(byte);
                    break;
                case mValueType.mint:
                    valueType = typeof(Int16);
                    break;
                case mValueType.mfloat:
                    valueType = typeof(Single);
                    break;
                case mValueType.mstr:
                    valueType = typeof(string);
                    break;
                case mValueType.marray:
                    valueType = typeof(byte[]);
                    break;
                default:
                    valueType = typeof(object);
                    break;
            }
            return valueType;
        }
        public void SetFullValue(GlobalValModel val)
        {
            type = val.type; value = val.value; address = val.address; record = val.record;
        }
        /// <summary>
        /// TODO 重写value.tostring方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (EnumHelper.GetEnum<mValueType>(type) == mValueType.marray)
            {
                return string.Format(@"[{0}]{1}:{2}({3})", type, address, BitConverter.ToString((byte[])value), record);
            }
            return string.Format(@"[{0}]{1}:{2}({3})", type, address, value, record);
        }
        public object Clone()
        {
            return new GlobalValModel(type, record, address, value);
        }
    }
    public class IpModel
    {
        public string Protocol { get; set; }
        public string IpAddress { get; set; }
        public int EndPoint { get; set; }
        public object Addin { get; set; }
    }
}
