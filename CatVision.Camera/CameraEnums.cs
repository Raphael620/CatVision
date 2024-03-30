using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatVision.Camera
{
    public enum CameraProvider
    {
        FileCamera = 0,
        HikCamera = 1,
        DahengCamera = 2
    }
    public enum TriggerSource
    {
        Software = 0,
        Line0 = 1,
        Line1 = 2
    }
}
