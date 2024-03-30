using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;

namespace CatVision.Communication
{
    public class SerialDevice
    {
        SerialPort sp = new SerialPort("", 1, Parity.Odd, 8, StopBits.None);
    }
}
