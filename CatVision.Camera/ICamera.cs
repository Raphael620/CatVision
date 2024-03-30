using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using CatVision.Common.Model;

namespace CatVision.Camera
{
    /// <summary>
    /// 相机设备接口
    /// </summary>
    public interface ICamera<T> : IDevice
    {
        // IPubSub
        CameraInfo CamInfo { get; }
        T Image { get; set; }
        Action<T> ImageHandle { get; set; }
        void Capture();
        void SetExpoTime(float us);
        void SetGain(float us);
        void SetTriggerMode(bool trigger);
        void SetTriggerSource(TriggerSource trigger);
    }
}
