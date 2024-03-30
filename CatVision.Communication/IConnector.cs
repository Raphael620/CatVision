using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatVision.Common.Model;

namespace CatVision.Communication
{
    /// <summary>
    /// 通信设备接口
    /// </summary>
    public interface IConnector : IDevice
    {
        ConnectorInfo ConnInfo { get; set; }
        /// read get
        void Recieve(ref GlobalValModel val);
        /// write set
        void SendAsync(GlobalValModel val);
    }
}
