using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MvCamCtrl.NET;
using HalconDotNet;
using CatVision.Common;
using CatVision.Common.Enum;
using CatVision.Common.Model;

namespace CatVision.Camera
{
    public class HikCamera : ICamera<HObject>, IPubish
    {
        // IDevice
        public DeviceInfo DevInfo { get; set; } = new DeviceInfo() { DeviceType = "ICamera" };
        // ICamera
        public CameraInfo CamInfo { get; private set; }
        public HObject Image { get; set; } = new HObject();
        public Action<HObject> ImageHandle { get; set; }
        // IPubSub , when use line trigger
        public Action<GlobalValModel> Publish { get; set; }
        //public Action<string, mValueType, object> Subscribe { get; set; }
        // private
        private static List<CameraInfo> CamList;
        private static Dictionary<string, MyCamera.MV_CC_DEVICE_INFO> DevList;
        private MyCamera.cbOutputExdelegate ImageCallback; 
        private AutoResetEvent GrabOneEvent = new AutoResetEvent(false);
        IntPtr pImage = IntPtr.Zero;
        MyCamera mCamera;
        MyCamera.MV_CC_DEVICE_INFO mDevice;
        public HikCamera() { }
        public HikCamera(CameraInfo cam) { CamInfo = cam; }
        public static List<CameraInfo> InitCamList()
        {
            CamList = new List<CameraInfo>();
            DevList = new Dictionary<string, MyCamera.MV_CC_DEVICE_INFO>();
            MyCamera.MV_CC_DEVICE_INFO_LIST mDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref mDeviceList);
            if (nRet != 0)
            {
                Log.Default.Error(@"[{0}]InitCamList.Error: {1}", "Camera", nRet);
                return CamList;
            }
            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < mDeviceList.nDeviceNum; i++)
            {
                CameraInfo camInfo;
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(mDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    camInfo = new CameraInfo("HikCamera", gigeInfo.chUserDefinedName, gigeInfo.chSerialNumber);
                    CamList.Add(camInfo);
                    DevList.Add(gigeInfo.chSerialNumber, device);
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    camInfo = new CameraInfo("HikCamera", usbInfo.chUserDefinedName, usbInfo.chSerialNumber);
                    CamList.Add(camInfo);
                    DevList.Add(usbInfo.chSerialNumber, device);
                }
            }
            return CamList;
        }

        // IDevice
        public void Init(params object[] paras) { InitCamList(); }
        public void Uninit(params object[] paras) { CamList.Clear(); }
        public void Connect(params object[] paras) 
        {
            if (paras.Length > 0) CamInfo = (CameraInfo)paras[0];
            DisConnect(CamInfo);
            int nRet = 0;
            try
            {
                if (DevList.Keys.Contains(CamInfo.SN)) mDevice = DevList[CamInfo.SN];
                else return;
                DevInfo.DeviceName = CamInfo.Name;
                if (mCamera == null)
                {
                    mCamera = new MyCamera();
                    if (null == mCamera) { return; }
                }
                nRet = mCamera.MV_CC_CreateDevice_NET(ref mDevice);
                if (MyCamera.MV_OK != nRet)
                {
                    Log.Default.Error(@"[{0}:{1}]Connect.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet);
                    return;
                }
                nRet = mCamera.MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    mCamera.MV_CC_DestroyDevice_NET();
                    Log.Default.Error(@"[{0}:{1}]Connect.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet);
                    return;
                }
                SetGigeNet();
                SetTriggerMode(true);
                SetTriggerSource(TriggerSource.Software);
                // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
                //MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", 2);// ch:工作在连续模式 | en:Acquisition On Continuous Mode
                // ch:注册回调函数 | en:Register image callback
                ImageCallback = new MyCamera.cbOutputExdelegate(ImageCallbackFunc);
                nRet = mCamera.MV_CC_RegisterImageCallBackEx_NET(ImageCallback, IntPtr.Zero);
                if (MyCamera.MV_OK != nRet)
                {
                    Log.Default.Error(@"[{0}:{1}]Connect.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet);
                }
                // ch:开启抓图 || en: start grab image
                nRet = mCamera.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    Log.Default.Error(@"[{0}:{1}]Connect.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet);
                }
                DevInfo.DState = DeviceState.Connectted;
                DevInfo.WState = WorkState.Ready;
            }
            catch(Exception ex) { Log.Default.Error(ex, @"[{0}:{1}]Connect.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet); }
        }
        public void DisConnect(params object[] paras) 
        {
            ImageHandle = null;
            if (DevInfo.IsConnected)
            {
                int nRet = mCamera.MV_CC_CloseDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    Log.Default.Error(@"[{0}:{1}]DisConnect.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet);
                    return;
                }
                nRet = mCamera.MV_CC_DestroyDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    Log.Default.Error(@"[{0}:{1}]DisConnect.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet);
                    return;
                }
                DevInfo.DState = DeviceState.DisConnectted;
            }
        }
        // ICamera
        public void Capture() { mCamera.MV_CC_TriggerSoftwareExecute_NET(); }
        public void SetExpoTime(float us)
        {
            int nRet = mCamera.MV_CC_SetFloatValue_NET("ExposureTime", us);
            if (nRet != MyCamera.MV_OK)
            {
                Log.Default.Warn(@"[{0}:{1}]SetExpoTime.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet);
            }
            CamInfo.Expotime = us;
        }
        public void SetGain(float us)
        {
            int nRet = mCamera.MV_CC_SetFloatValue_NET("Gain", us);
            if (nRet != MyCamera.MV_OK)
            {
                Log.Default.Warn(@"[{0}:{1}]SetGain.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet);
            }
            CamInfo.Gain = us;
        }
        public void SetTriggerMode(bool trigger) 
        {
            int nRet = 0;
            if (trigger)
            {
                nRet = mCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                // mCamera.MV_CC_SetEnumValue_NET("TriggerActivation", 0); // 0=上升沿
            }
            else
            {
                nRet = mCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            }
            if (nRet != MyCamera.MV_OK) { Log.Default.Warn(@"[{0}:{1}]SetTriggerMode.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet); }
            CamInfo.TriggerMode = trigger;
        }
        public void SetTriggerSource(TriggerSource trigger) 
        {
            int nRet = 0;
            if (trigger == TriggerSource.Software)
            {
                nRet = mCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
            }
            else if (trigger == TriggerSource.Line0)
            {
                nRet = mCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0);
            }
            else if (trigger == TriggerSource.Line1)
            {
                nRet = mCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE1);
            }
            if (nRet != MyCamera.MV_OK) { Log.Default.Warn(@"[{0}:{1}]SetTriggerSource.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet); }
            CamInfo.MTriggerSource = trigger;
        }
        // IPubSub
        public void Subscribe(int time, GlobalValModel val) { }

        // private
        private void SetGigeNet()
        {
            // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
            if (mDevice.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                int nPacketSize = mCamera.MV_CC_GetOptimalPacketSize_NET();
                if (nPacketSize > 0)
                {
                    mCamera.MV_CC_SetIntValueEx_NET("GevSCPSPacketSize", (long)nPacketSize);
                }
            }
        }
        /// <summary>采集回调</summary>
        private void ImageCallbackFunc(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            try
            {
                MyCamera.MvGvspPixelType enDstPixelType;
                if (IsMonoImage(pFrameInfo.enPixelType))
                {
                    enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                    CamInfo.Channel = 1;
                }
                else if (IsColorImage(pFrameInfo.enPixelType))
                {
                    enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
                    CamInfo.Channel = 3;
                }
                else
                {
                    Log.Default.Warn(@"[{0}:{1}]ImageCallbackFunc.Error: PixelFormat not found", CamInfo.Provider, CamInfo.SN);
                    return;
                }
                MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

                stConverPixelParam.nWidth = pFrameInfo.nWidth;
                stConverPixelParam.nHeight = pFrameInfo.nHeight;
                CamInfo.Width = pFrameInfo.nWidth;
                CamInfo.Height = pFrameInfo.nHeight;
                stConverPixelParam.pSrcData = pData;
                stConverPixelParam.nSrcDataLen = pFrameInfo.nFrameLen;
                stConverPixelParam.enSrcPixelType = pFrameInfo.enPixelType;
                stConverPixelParam.enDstPixelType = enDstPixelType;

                if (pImage != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pImage);
                    pImage = IntPtr.Zero;
                }
                pImage = Marshal.AllocHGlobal(CamInfo.BufSize);
                if (IntPtr.Zero == pImage)
                {
                    Log.Default.Warn(@"[{0}:{1}]ImageCallbackFunc.Error: AllocHGlobal error", CamInfo.Provider, CamInfo.SN);
                }
                stConverPixelParam.nDstBufferSize = (uint)CamInfo.BufSize;
                stConverPixelParam.pDstBuffer = pImage;
                int nRet = mCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
                if (MyCamera.MV_OK != nRet)
                {
                    Log.Default.Warn(@"[{0}:{1}]ImageCallbackFunc.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet);
                    return;
                }

                HObject hImage = new HObject();
                try
                {
                    if (CamInfo.Channel == 1)
                    {
                        HOperatorSet.GenImage1Extern(out hImage, "byte", CamInfo.Width, CamInfo.Height, pImage, IntPtr.Zero);
                    }
                    else if (CamInfo.Channel == 3)
                    {
                        HOperatorSet.GenImageInterleaved(out hImage, pImage, "rgb", CamInfo.Width, CamInfo.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                    }
                }
                catch { Log.Default.Warn(@"[{0}:{1}]ImageCallbackFunc.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet); }
                // TODO
                // mCamera.MV_CC_FreeImageBuffer_NET(ref pFrameInfo);
                if (pImage != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pImage);
                    pImage = IntPtr.Zero;
                }

                Image.Dispose();
                Image = hImage;
                GrabOneEvent.Set();
                ImageHandle?.Invoke(Image);
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, @"[{0}:{1}]ImageCallbackFunc.Error.", CamInfo.Provider, CamInfo.SN);
                GrabOneEvent.Set();
            }
        }
        private bool IsMonoImage(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsColorImage(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }
    }
}
