using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatVision.Communication
{
    public enum ConnectorProvider
    {
        TcpServer = 0,
        TcpClient = 1,
        Udp = 2,
        Siemens7 = 3,
        Serial = 4,
        ModbusMaster = 5,
        MqttClient = 6
    }
    public class ComEnums
    {
        /// <summary>
        ///  int[9] = 19200
        ///  System.IO.Ports.Parity/StopBits
        /// </summary>
        public static int[] BaudRateEnums = new int[15] { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 512000 };
        public static int[] DataBitsEnums = new int[4] { 5, 6, 7, 8 };
    }
}
