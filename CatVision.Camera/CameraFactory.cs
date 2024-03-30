using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using CatVision.Common.Enum;
using CatVision.Common.Model;

namespace CatVision.Camera
{
    public class CameraFactory
    {
        public static ICamera<HObject> GetInstance(CameraInfo cam)
        {
            CameraProvider provider = EnumHelper.GetEnum<CameraProvider>(cam.Provider);
            if (provider == CameraProvider.HikCamera)
            {
                return new HikCamera(cam);
            }
            else if (provider == CameraProvider.DahengCamera)
            {
                return new DahengCamera(cam);
            }
            else if (provider == CameraProvider.FileCamera)
            {
                return new FileCamera(cam);
            }
            return default(ICamera<HObject>);
        }
        public static List<CameraInfo> InitCamList(CameraProvider provider)
        {
            if (provider == CameraProvider.HikCamera)
            {
                return HikCamera.InitCamList();
            }
            else if (provider == CameraProvider.DahengCamera)
            {
                return DahengCamera.InitCamList();
            }
            else if (provider == CameraProvider.FileCamera)
            {
                return FileCamera.InitCamList();
            }
            else
            {
                return new List<CameraInfo>();
            }
        }
    }
}
