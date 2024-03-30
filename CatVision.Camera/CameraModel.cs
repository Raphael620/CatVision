using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CatVision.Camera
{
    [Serializable]
    public class CameraInfo : object, ICloneable
    {
        public string Provider { get; set; }
        public string Name { get; set; }
        public string SN { get; set; }
        public string Alias { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Channel { get; set; }
        //
        public bool TriggerMode { get; set; } = true;
        public TriggerSource MTriggerSource { get; set; } = TriggerSource.Software;
        public float Expotime { get; set; }
        public float Gain { get; set; }
        [JsonIgnore]
        public int BufSize => Width * Height * Channel;
        public CameraInfo() { }
        public CameraInfo(string provider, string name, string sn)
        {
            Provider = provider; Name = name; SN = sn;
        }
        public CameraInfo(string provider, string name, string sn, string alias)
        {
            Provider = provider; Name = name; SN = sn; Alias = alias;
        }
        public CameraInfo(string provider, string name, string sn, string alias, int width, int height, int channel)
        {
            Provider = provider; Name = name; SN = sn; Alias = alias; Width = width; Height = height; Channel = channel;
        }
        public CameraInfo(string provider, string name, string sn, string alias, int width, int height, int channel, 
            bool trigger, TriggerSource source, float gain, float expo)
        {
            Provider = provider; Name = name; SN = sn; Alias = alias; Width = width; Height = height; Channel = channel;
            TriggerMode = trigger; MTriggerSource = source; Gain = gain; Expotime = expo;
        }
        public object Clone()
        {
            return new CameraInfo(Provider, Name, SN, Alias, Width, Height, Channel, TriggerMode, MTriggerSource, Gain, Expotime);
        }
        public override string ToString()
        {
            return string.Format("[{0}]{1}:W({2})*H({3})*C({4});", Provider, Name, Width, Height, Channel);
        }
    }
}
