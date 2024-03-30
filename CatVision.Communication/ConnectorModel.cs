using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CatVision.Common.Enum;
using CatVision.Common.Model;

namespace CatVision.Communication
{
    [Serializable]
    public class ConnectorInfo : ObservableObject, ICloneable
    {
        /// <summary>
        /// DeviceProvider.TcpServer TcpClient Udp Siemens7 Serial ModbusMaster MqttClient 
        /// </summary>
        public string Provider { get => provider; set => SetProperty(ref provider, value); }
        private string provider;
        public string Ip { get => ip; set => SetProperty(ref ip, value); }
        private string ip;
        public int Port { get => port; set => SetProperty(ref port, value); }
        private int port;
        public List<GlobalValModel> AddinPara { get; set; } = new List<GlobalValModel>();
        public List<GlobalValModel> PublishList { get; set; } = new List<GlobalValModel>();
        public List<GlobalValModel> SubscribeList { get; set; } = new List<GlobalValModel>();
        public ConnectorInfo() { }
        public ConnectorInfo(string provider) { Provider = provider; }
        public ConnectorInfo(string provider, string ip, int port) { Provider = provider; Ip = ip; Port = port; }
        public ConnectorInfo(string provider, string ip, int port, List<GlobalValModel> addinPara, List<GlobalValModel> pubList, List<GlobalValModel> subList)
        {
            Provider = provider; Ip = ip; Port = port;
            AddinPara = addinPara; PublishList = pubList; SubscribeList = subList;
        }
        // public void OnSubscribeListChanged() { OnPropertyChanged(nameof(SubscribeList)); }
        public object Clone()
        {
            return new ConnectorInfo(Provider, Ip, Port, AddinPara, PublishList, SubscribeList);
        }
        public override string ToString()
        {
            return string.Format(@"[{0}:{1}]{2}", Ip, Port, Provider);
        }
    }
}
