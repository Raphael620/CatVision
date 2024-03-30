using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MvCamCtrl.NET;
using GxIAPINET;
using HalconDotNet;
using CatVision.Common;
using CatVision.Common.Enum;
using CatVision.Common.Model;

namespace CatVision.Camera
{
    public class DahengCamera : ICamera<HObject> , IPubish
    {
        // IDevice
        public DeviceInfo DevInfo { get; set; } = new DeviceInfo() { DeviceType = "ICamera" };
        // ICamera
        public CameraInfo CamInfo { get; private set; }
        public HObject Image { get; set; } = new HObject();
        public Action<HObject> ImageHandle { get; set; }
        // IPubSub : when use line trigger
        public Action<GlobalValModel> Publish { get; set; }

        //public Action<string, mValueType, object> Subscribe { get; set; }
        // private
        private static List<CameraInfo> CamList;
        private AutoResetEvent GrabOneEvent = new AutoResetEvent(false);
        private bool OfflineFlag = false;
        private bool FrameInfoGot = false;
        IntPtr pImage = IntPtr.Zero;
        // daheng
        public static IGXFactory m_objIGXFactory = null;               ///<Factory对像
        IGXStream m_objIGXStream = null;                               ///<流对像
        IGXFeatureControl m_objIGXStreamFeatureControl = null;         ///<流层属性控制器对象
        IGXDevice m_objIGXDevice = null;                               ///<设备对像
        IGXFeatureControl m_objIGXFeatureControl = null;               ///<远端设备属性控制器对像
        GX_FEATURE_CALLBACK_HANDLE m_hFeatureCallback = null;          ///<Feature事件的句柄
        GX_DEVICE_OFFLINE_CALLBACK_HANDLE m_hCB = null;                ///<掉线回调句柄
        public DahengCamera() { }
        public DahengCamera(CameraInfo cam) { CamInfo = cam; }
        public static List<CameraInfo> InitCamList()
        {
            CamList = new List<CameraInfo>();
            try
            {
                List<IGXDeviceInfo> m_listIGXDeviceInfo = new List<IGXDeviceInfo>();
                m_listIGXDeviceInfo.Clear();
                if (null != m_objIGXFactory)
                {
                    m_objIGXFactory.UpdateDeviceList(200, m_listIGXDeviceInfo);
                }
                if (m_listIGXDeviceInfo.Count == 0)
                {
                    Log.Default.Error(@"[{0}]InitCamList.Error: {1}", "Camera", "NotFound");
                    return CamList;
                }
                //获取相机数
                int iCameraCounts = m_listIGXDeviceInfo.Count;
                for (int i = 0; i < iCameraCounts; i++)
                {
                    CameraInfo cam = new CameraInfo();
                    cam.SN = m_listIGXDeviceInfo[i].GetSN();
                    cam.Name = m_listIGXDeviceInfo[i].GetUserID();
                    cam.Provider = CameraProvider.DahengCamera.ToString();
                    CamList.Add(cam);
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, @"[{0}]InitCamList.Error: {1}", "Camera", "DahengCamera");
            }
            return CamList;
        }

        // IDevice
        public void Init(params object[] paras)
        {
            m_objIGXFactory = IGXFactory.GetInstance();
            m_objIGXFactory.Init();
            InitCamList(); 
        }
        public void Uninit(params object[] paras) 
        {
            CamList.Clear();
            m_objIGXFactory.Uninit();
        }
        public void Connect(params object[] paras)
        {
            if (paras.Length > 0) CamInfo = (CameraInfo)paras[0];
            DisConnect(CamInfo);
            int nRet = -1;
            try
            {
                if (!CamList.Select(x => x.SN).Contains(CamInfo.SN))
                {
                    Log.Default.Error(@"[{0}:{1}]Connect.Error: {2}", CamInfo.Provider, CamInfo.SN, "NotFound");
                    return;
                }
                DevInfo.DeviceName = CamInfo.Name;
                if (null != m_objIGXDevice)
                {
                    m_objIGXDevice.Close();
                    m_objIGXDevice = null;
                }
                m_objIGXDevice = m_objIGXFactory.OpenDeviceBySN(CamInfo.SN, GX_ACCESS_MODE.GX_ACCESS_EXCLUSIVE);
                m_objIGXFeatureControl = m_objIGXDevice.GetRemoteFeatureControl();
                //打开流
                if (null != m_objIGXDevice)
                {
                    m_objIGXStream = m_objIGXDevice.OpenStream(0);
                    m_objIGXStreamFeatureControl = m_objIGXStream.GetFeatureControl();
                }
                else
                {
                    Log.Default.Error(@"[{0}:{1}]Connect.Error: {2}", CamInfo.Provider, CamInfo.SN, "NullDevice");
                    return;
                }
                SetGigeNet();
                m_objIGXFeatureControl.GetEnumFeature("AcquisitionMode").SetValue("Continuous");
                SetTriggerMode(CamInfo.TriggerMode);
                SetTriggerSource(CamInfo.MTriggerSource);
                if (null != m_objIGXStreamFeatureControl)
                {
                    //设置流层Buffer处理模式为OldestFirst
                    m_objIGXStreamFeatureControl.GetEnumFeature("StreamBufferHandlingMode").SetValue("OldestFirst");
                }
                //开启采集流通道
                if (null != m_objIGXStream)
                {
                    m_objIGXStream.RegisterCaptureCallback(null, __OnFrameCallback);
                    m_hCB = m_objIGXDevice.RegisterDeviceOfflineCallback(null, __OnDeviceOfflineCallback);
                    m_objIGXStream.StartGrab();
                }
                //发送开采命令
                if (null != m_objIGXFeatureControl)
                {
                    m_objIGXFeatureControl.GetCommandFeature("AcquisitionStart").Execute();
                }
                DevInfo.DState = DeviceState.Connectted;
                DevInfo.WState = WorkState.Ready;
            }
            catch (Exception ex) { Log.Default.Error(ex, @"[{0}:{1}]Connect.Error: {2}", CamInfo.Provider, CamInfo.SN, nRet); }
        }
        public void DisConnect(params object[] paras)
        {
            ImageHandle = null;
            if (DevInfo.IsConnected)
            {
                try
                {
                    if (!OfflineFlag) m_objIGXDevice.UnregisterDeviceOfflineCallback(m_hCB);
                    // 如果未停采则先停止采集
                    if (null != m_objIGXFeatureControl)
                    {
                        m_objIGXFeatureControl.GetCommandFeature("AcquisitionStop").Execute();
                        m_objIGXFeatureControl = null;
                    }
                }
                catch { }
                try
                {
                    //停止流通道、注销采集回调和关闭流
                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.StopGrab();
                        //注销采集回调函数
                        m_objIGXStream.UnregisterCaptureCallback();
                        m_objIGXStream.Close();
                        m_objIGXStream = null;
                        m_objIGXStreamFeatureControl = null;
                    }
                }
                catch { }
                try
                {
                    //关闭设备
                    if (null != m_objIGXDevice)
                    {
                        m_objIGXDevice.Close();
                        m_objIGXDevice = null;

                    }
                }
                catch { Log.Default.Error(@"[{0}:{1}]DisConnect.Error: {2}", CamInfo.Provider, CamInfo.SN, "CloseDevice"); }
                DevInfo.DState = DeviceState.DisConnectted;
            }
        }
        // ICamera
        public void Capture()
        {
            try
            {
                if (null != m_objIGXStream) { m_objIGXStream.FlushQueue(); }
                if (CamInfo.TriggerMode && CamInfo.MTriggerSource == TriggerSource.Software) m_objIGXFeatureControl.GetCommandFeature("TriggerSoftware").Execute();
            }
            catch { }
        }
        public void SetExpoTime(float us)
        {
            if ((null != m_objIGXFeatureControl) && DevInfo.IsConnected)
            {
                /*float dMin = (float)m_objIGXFeatureControl.GetFloatFeature("ExposureTime").GetMin();
                float dMax = (float)m_objIGXFeatureControl.GetFloatFeature("ExposureTime").GetMax();
                if (us > dMax) us = dMax;
                if (us < dMin) us = dMin;*/
                m_objIGXFeatureControl.GetFloatFeature("ExposureTime").SetValue(us);
                CamInfo.Expotime = us;
            }
            else 
            {
                Log.Default.Warn(@"[{0}:{1}]SetExpoTime.Error: {2}", CamInfo.Provider, CamInfo.SN, "DeviceDisconnected");
            }
        }
        public void SetGain(float us)
        {
            if ((null != m_objIGXFeatureControl) && DevInfo.IsConnected)
            {
                /*float dMin = (float)m_objIGXFeatureControl.GetFloatFeature("Gain").GetMin();
                float dMax = (float)m_objIGXFeatureControl.GetFloatFeature("Gain").GetMax();
                if (us > dMax) us = dMax;
                if (us < dMin) us = dMin;*/
                m_objIGXFeatureControl.GetFloatFeature("Gain").SetValue(us);
                CamInfo.Gain = us;
            }
            else
            {
                Log.Default.Warn(@"[{0}:{1}]SetGain.Error: {2}", CamInfo.Provider, CamInfo.SN, "DeviceDisconnected");
            }
        }
        public void SetTriggerMode(bool trigger)
        {
            if (m_objIGXFeatureControl == null)
            {
                Log.Default.Warn(@"[{0}:{1}]SetTriggerMode.Error: {2}", CamInfo.Provider, CamInfo.SN, "DeviceDisconnected");
                return; 
            }
            if (trigger)
            {
                m_objIGXFeatureControl.GetEnumFeature("TriggerMode").SetValue("On");
            }
            else
            {
                m_objIGXFeatureControl.GetEnumFeature("TriggerMode").SetValue("Off");
            }
            CamInfo.TriggerMode = trigger;
        }
        public void SetTriggerSource(TriggerSource trigger)
        {
            if (m_objIGXFeatureControl == null)
            {
                Log.Default.Warn(@"[{0}:{1}]SetTriggerMode.Error: {2}", CamInfo.Provider, CamInfo.SN, "DeviceDisconnected");
                return;
            }

            if (trigger == TriggerSource.Software)
            {
                m_objIGXFeatureControl.GetEnumFeature("TriggerSource").SetValue("Software");
            }
            else if (trigger == TriggerSource.Line0)
            {
                m_objIGXFeatureControl.GetEnumFeature("TriggerSource").SetValue("Line0");
            }
            else if (trigger == TriggerSource.Line1)
            {
                m_objIGXFeatureControl.GetEnumFeature("TriggerSource").SetValue("Line1");
            }
            CamInfo.MTriggerSource = trigger;
        }
        // IPubSub
        public void Subscribe(int time, GlobalValModel val) { }

        // private
        private void SetGigeNet()
        {
            // 建议用户在打开网络相机之后，根据当前网络环境设置相机的流通道包长值，
            // 以提高网络相机的采集性能,设置方法参考以下代码。
            GX_DEVICE_CLASS_LIST objDeviceClass = m_objIGXDevice.GetDeviceInfo().GetDeviceClass();
            if (GX_DEVICE_CLASS_LIST.GX_DEVICE_CLASS_GEV == objDeviceClass)
            {
                // 判断设备是否支持流通道数据包功能
                if (true == m_objIGXFeatureControl.IsImplemented("GevSCPSPacketSize"))
                {
                    // 获取当前网络环境的最优包长值
                    uint nPacketSize = m_objIGXStream.GetOptimalPacketSize();
                    // 将最优包长值设置为当前设备的流通道包长值
                    m_objIGXFeatureControl.GetIntFeature("GevSCPSPacketSize").SetValue(nPacketSize);
                }
            }
        }
        private bool __IsMonoImage(GX_PIXEL_FORMAT_ENTRY format)
        {
            if (format == GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO8 ||
                format == GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO8_SIGNED ||
                format == GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO10 ||
                format == GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO12 ||
                format == GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO14 ||
                format == GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO16)
            { CamInfo.Channel = 1; return true; }
            else { CamInfo.Channel = 3; return false; }
        }

        /// <summary>
        /// 采集回调函数
        /// </summary>
        /// <param name="objUserParam">用户私有参数</param>
        /// <param name="objIFrameData">图像信息对象</param>
        private void __OnFrameCallback(object objUserParam, IFrameData objIFrameData)
        {
            if (null == objIFrameData) { Log.Default.Warn(@"[{0}:{1}]__OnFrameCallback.Error: {2}", CamInfo.Provider, CamInfo.SN, "NullFrame"); }
            try
            {
                if (!FrameInfoGot)
                {
                    //图像获取为完整帧，可以读取图像宽、高、数据格式等
                    UInt64 nWidth = objIFrameData.GetWidth();
                    UInt64 nHeight = objIFrameData.GetHeight();
                    CamInfo.Width = (int)nWidth;
                    CamInfo.Height = (int)nHeight;
                    __IsMonoImage(objIFrameData.GetPixelFormat());
                }
                IntPtr imagebuffer = objIFrameData.GetBuffer();
                HObject _Image = new HObject();
                _Image.Dispose();
                pImage = IntPtr.Zero;
                if (CamInfo.Channel == 1)
                {
                    pImage = objIFrameData.ConvertToRaw8(GX_VALID_BIT_LIST.GX_BIT_0_7);
                    HOperatorSet.GenImage1(out _Image, "byte", CamInfo.Width, CamInfo.Height, pImage);
                }
                else if (CamInfo.Channel == 3)
                {
                    pImage = objIFrameData.ConvertToRGB24(GX_VALID_BIT_LIST.GX_BIT_0_7, GX_BAYER_CONVERT_TYPE_LIST.GX_RAW2RGB_NEIGHBOUR, true);
                    HOperatorSet.GenImageInterleaved(out _Image, pImage, "bgr", CamInfo.Width, CamInfo.Height, -1, "byte", CamInfo.Width, CamInfo.Height, 0, 0, -1, 0);
                }
                pImage = IntPtr.Zero;
                Image.Dispose();
                // TODO HOperatorSet.MirrorImage(_Image, out Image, "row");
                Image = _Image;
                GrabOneEvent.Set();
                ImageHandle?.Invoke(Image);
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, @"[{0}:{1}]ImageCallbackFunc.Error.", CamInfo.Provider, CamInfo.SN);
                GrabOneEvent.Set();
            }
        }
        /// <summary>
        /// 掉线回调函数
        /// </summary>
        /// <param name="pUserParam">用户私有参数</param>
        private void __OnDeviceOfflineCallback(object pUserParam)
        {

        }
    }
}
