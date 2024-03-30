using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S7.Net;
using CatVision.Common;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using Newtonsoft.Json;

namespace CatVision.Communication
{
    public class S7Device : IConnector, IPubish, ISubscribe
    {
        // IDevice
        public DeviceInfo DevInfo { get; set; } = new DeviceInfo() { DeviceType = ConnectorProvider.Siemens7.ToString() };
        // IConnector
        public ConnectorInfo ConnInfo { get; set; } = new ConnectorInfo { Provider = ConnectorProvider.Siemens7.ToString() };
        // IPubSub
        public Action<GlobalValModel> Publish { get; set; }
        // private
        [JsonIgnore]
        public static List<string> CpuTypeList = EnumHelper.GetEnumStrs<CpuType>();
        Plc plc;
        Dictionary<string, System.Timers.Timer> timers = new Dictionary<string, System.Timers.Timer>();
        public int reconnTimeSpan = 100;
        [JsonIgnore]
        public int ReconnTimeSpan
        {
            get
            {
                if (reconnTimeSpan < int.MaxValue) { reconnTimeSpan *= 2; }
                else { reconnTimeSpan = 200; }
                return reconnTimeSpan;
            }
        }
        public void Init(params object[] paras) { }
        public void Uninit(params object[] paras) { }
        /// <summary>
        /// DevInfo.AddinPara[]{(string)CpuType?, (int)rack?, (int)slot?}
        /// </summary>
        /// <param name="paras">DevInfo</param>
        public void Connect(params object[] paras)
        {
            if (paras.Length > 0) ConnInfo = (ConnectorInfo)paras[0];
            try
            {
                if ((ConnInfo.AddinPara is null) || ConnInfo.AddinPara.Count < 1)
                {
                    if (ConnInfo.Port == default) plc = new Plc(CpuType.S71200, ConnInfo.Ip, 0, 0);
                    else plc = new Plc(CpuType.S71200, ConnInfo.Ip, ConnInfo.Port, 0, 0);
                }
                else
                {
                    CpuType cpu = EnumHelper.GetEnum<CpuType>((string)ConnInfo.AddinPara[0].GetTypedValue());
                    int rack = 0, slot = 0;
                    if (ConnInfo.AddinPara.Count > 1) rack = (int)ConnInfo.AddinPara[1].GetTypedValue();
                    if (ConnInfo.AddinPara.Count > 2) slot = (int)ConnInfo.AddinPara[2].GetTypedValue();
                    if (ConnInfo.Port == default) plc = new Plc(cpu, ConnInfo.Ip, (short)rack, (short)slot);
                    else plc = new Plc(CpuType.S71200, ConnInfo.Ip, ConnInfo.Port, (short)rack, (short)slot);
                }
                plc.Open();
                ConnInfo.Port = plc.Port;
                DevInfo.DState = DeviceState.Connectted;
                DevInfo.WState = WorkState.Ready;
            }
            catch (Exception ex) { Log.Default.Error(ex, @"[{0}]Connect.Error: [{1}:{2}]", ConnInfo.Provider, ConnInfo.Ip, ConnInfo.Port); }
        }
        public void DisConnect(params object[] paras)
        {
            try
            {
                plc.Close();
            }
            catch { }
        }
        /// read
        public void Recieve(ref GlobalValModel val)
        {
            try
            {
                // TODO : need test ; TODO : multithread lock
                // val.value = plc.Read(val.address);
                val.GetmType();
                if (val.valueType == typeof(Single))
                {
                    val.value = ReadDBaddin("Real", val.address);
                }
                else if (val.valueType == typeof(string))
                {
                    val.value = ReadDBaddin("String", val.address);
                }
                else
                {
                    val.value = ReadDB(val.address);
                }
            }
            catch { val.value = null; }
        }
        /// write
        public void SendAsync(GlobalValModel val)
        {
            try
            {
                //plc.Write(val.address, val.value);
                val.GetmType();
                if (val.valueType == typeof(Single))
                {
                    WriteDBaddin("Real", val.address, val.value);
                }
                else if (val.valueType == typeof(string))
                {
                    WriteDBaddin("String", val.address, val.value);
                }
                else
                {
                    WriteDB(val.address, val.value);
                }
            }
            catch { }
        }
        // IPubSub
        public void Subscribe(int timeSpan, GlobalValModel val)
        {
            System.Timers.Timer timer = new System.Timers.Timer(timeSpan) { AutoReset = true };
            GlobalValModel startVal = new GlobalValModel();
            timer.Elapsed += (s, e) =>
            {
                try
                {
                    Recieve(ref val);
                    if (null != startVal && startVal.value != val.value)
                    {
                        Publish.BeginInvoke(val, null, null);
                    }
                    startVal = val;
                }
                catch { }
            };
            timers.Add(val.address, timer);
            ConnInfo.SubscribeList.Add(val);
            timer.Start();
        }
        public void DisSubscribe(GlobalValModel val)
        {
            if (timers.ContainsKey(val.address))
            {
                try
                {
                    timers[val.address].Stop();
                    timers[val.address].Dispose();
                    ConnInfo.SubscribeList.Remove(val);
                }
                catch { }
            }
        }


        /// <summary>
        /// DBB DBW DBD DBX | Byte Word DWord Bit | byte Int16 Int32? bool
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public object ReadDB(string val)
        {
            try
            {
                return plc.Read(val);
            }
            catch { }
            return 0;
        }
        /// <summary>
        /// DBB DBW DBD DBX | Byte Word DWord Bit | byte Int16 Int32? bool
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="val"></param>
        public bool WriteDB(string addr, object val)
        {
            try
            {
                plc.Write(addr, val);
                return true;
            }
            catch { }
            return false;
        }
        /// <summary>
        /// DR(DBR) DSTRING(DSTR) | Real String | Single String
        /// </summary>
        /// <param name="type"> "Real" or "String" </param>
        /// <param name="addr"></param>
        /// <returns> (Single) or (string) </returns>
        public object ReadDBaddin(string type, string addr)
        {
            try
            {
                if (type == "Real")
                {
                    (int, int) address = GetDBAddress(addr);
                    Single res = (Single)plc.Read(DataType.DataBlock, address.Item1, address.Item2, VarType.Real, 1);
                }
                else if (type == "String")
                {
                    (int, int) address = GetDBAddress(addr);
                    byte strLen = (byte)plc.Read(DataType.DataBlock, address.Item1, address.Item2 + 1, VarType.Byte, 1);
                    string str = (string)plc.Read(DataType.DataBlock, address.Item1, address.Item2 + 2, VarType.String, strLen);
                    return str.TrimEnd();
                }
            }
            catch { }
            return 0;
        }
        /// <summary>
        /// DR(DBR) DSTRING(DSTR) | Real String | Single String
        /// </summary>
        /// <param name="type"> "Real" or "String" </param>
        /// <param name="addr"></param>
        /// <param name="val"> (Single) or (string) </param>
        /// <returns></returns>
        public bool WriteDBaddin(string type, string addr, object val)
        {
            try
            {
                if (type == "Real")
                {
                    (int, int) address = GetDBAddress(addr);
                    plc.Write(DataType.DataBlock, address.Item1, address.Item2, (Single)val);
                    return true;
                }
                else if (type == "String")
                {
                    (int, int) address = GetDBAddress(addr);
                    plc.Write(DataType.DataBlock, address.Item1, address.Item2, GetStrBytes((string)val));
                    return true;
                }
            }
            catch { }
            return false;
        }
        public object Read(DataType blockType, int dbAddr, int startAddr, VarType varType, int varCount = 1, byte bitAddr = 0)
        {
            return plc.Read(blockType, dbAddr, startAddr, varType, varCount, bitAddr);
        }
        public void Write(DataType blockType, int dbAddr, int startAddr, object val, int bitAddr = -1)
        {
            plc.Write(blockType, dbAddr, startAddr, val, bitAddr);
        }
        public byte[] GetStrBytes(string str, int totalLen = 254)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Array.Empty<byte>();
            }
            else
            {
                // todo : zh-cn : use GBK
                byte[] val = Encoding.Default.GetBytes(str);
                byte[] head = new byte[2];
                head[0] = Convert.ToByte(totalLen);
                head[1] = Convert.ToByte(str.Length);
                val = head.Concat(val).ToArray();
                return val;
            }
        }
        public byte[] GetWStrBytes(string str, short totalLen = 508)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Array.Empty<byte>();
            }
            else
            {
                byte[] value = Encoding.BigEndianUnicode.GetBytes(str);
                byte[] head = BitConverter.GetBytes(totalLen);
                byte[] length = BitConverter.GetBytes((short)str.Length);
                Array.Reverse(head);
                Array.Reverse(length);
                head = head.Concat(length).ToArray();
                value = head.Concat(value).ToArray();
                return value;
            }
        }
        public (int, int) GetDBAddress(string addr)
        {
            try
            {
                string[] splite = addr.Split('.');
                string dbStr = System.Text.RegularExpressions.Regex.Replace(splite[0], @"[^0-9]+", "");
                string adStr = System.Text.RegularExpressions.Regex.Replace(splite[1], @"[^0-9]+", "");
                int db = -1, start = -1;
                if (int.TryParse(dbStr, out db) && int.TryParse(adStr, out start))
                {
                    return (db, start);
                }
                else { return (-1, -1); }
            }
            catch { }
            return (-1, -1);
        }
    }
}
