using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatVision.Common;
using CatVision.Common.Enum;
using CatVision.Common.Model;

namespace CatVision.Communication
{
    public class ConnectorFactory
    {
        private static ConnectorFactory instance = new ConnectorFactory();
        public static ConnectorFactory Ins { get => instance; }
        public static IConnector GetDevice(ConnectorInfo info)
        {
            ConnectorProvider provider = EnumHelper.GetEnum<ConnectorProvider>(info.Provider);
            switch (provider)
            {
                case ConnectorProvider.Siemens7:
                    return new S7Device() { ConnInfo = info };
                case ConnectorProvider.TcpServer:
                    return new TcpServerDevice() { ConnInfo = info };
                default:
                    return null;
            }
        }
    }
}
