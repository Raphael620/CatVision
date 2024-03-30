using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CatVision.Common.Enum;

namespace CatVision.Common.Model
{
    [Serializable]
    public class DeviceInfo : ObservableObject, ICloneable
    {
        /// <summary>
        /// equal to nameof(interface/class): ICamera,S7Device etc.
        /// </summary>
        public string DeviceType { get; set; }
        /// <summary>
        /// uuid, should be unique
        /// </summary>
        public string DeviceName { get => deviceName; set => SetProperty(ref deviceName, value); }
        private string deviceName;
        /// <summary>
        /// description of the device
        /// </summary>
        public string DeviceAlias { get => deviceAlias; set => SetProperty(ref deviceAlias, value); }
        private string deviceAlias;
        [JsonIgnore]
        public bool IsConnected { get => DState == DeviceState.Connectted; set => SetProperty(ref isConnected, value); }
        private bool isConnected = false;
        [JsonIgnore]
        public DeviceState DState { get => dState; set { dState = value; IsConnected = dState == DeviceState.Connectted; } }
        private DeviceState dState = DeviceState.UnAvailable;
        [JsonIgnore]
        public WorkState WState { get; set; } = WorkState.Warn;

        public DeviceInfo() { }
        public DeviceInfo(string name) { DeviceName = name; }
        public DeviceInfo(string name, string alias) { DeviceName = name; DeviceAlias = alias; }
        public DeviceInfo(string type, string name, string alias, bool conn, DeviceState dState, WorkState wState)
        {
            DeviceType = type; DeviceName = name; DeviceAlias = alias;
            IsConnected = conn; DState = dState; WState = wState;
        }
        public override string ToString()
        {
            return string.Format(@"[{0}:{1}]{2}:{{{3}.{4}}}", DeviceType, DeviceAlias, DeviceName, DState, WState);
        }
        public object Clone()
        {
            return new DeviceInfo(DeviceType, DeviceName, DeviceAlias, IsConnected, DState, WState);
        }
    }
}
