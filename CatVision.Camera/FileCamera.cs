using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HalconDotNet;
using CatVision.Common;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using System.Text.RegularExpressions;

namespace CatVision.Camera
{
    public class FileCamera : ICamera<HObject>, IPubish
    {
        // IDevice
        public DeviceInfo DevInfo { get; set; } = new DeviceInfo() { DeviceType = "ICamera" };
        // ICamera
        public CameraInfo CamInfo { get; private set; }
        public HObject Image { get; set; } = new HObject();
        public Action<HObject> ImageHandle { get; set; }
        // IPubSub , when use line trigger
        public Action<GlobalValModel> Publish { get; set; }
        // private
        private static List<CameraInfo> CamList;
        private static Dictionary<string, string> FileList;
        public FileCamera() { }
        public FileCamera(CameraInfo cam) { CamInfo = cam; }
        public static List<CameraInfo> InitCamList()
        {
            CamList = new List<CameraInfo>();
            string localpath = Path.Combine(Directory.GetCurrentDirectory(), "TestImage");
            string desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TestImage");
            string userimage = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "TestImage");
            FileList = new Dictionary<string, string>();
            FileList.Add("LocalPath", localpath);
            FileList.Add("Desktop", desktop);
            FileList.Add("UserImage", userimage);
            CamList.Add(new CameraInfo("FileCamera", "LocalPath", localpath));
            CamList.Add(new CameraInfo("FileCamera", "Desktop", desktop));
            CamList.Add(new CameraInfo("FileCamera", "UserImage", userimage));
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
                if (!CamList.Select(o => o.SN).Contains(CamInfo.SN)) return;
                DevInfo.DeviceName = CamInfo.Name;
                if (!Directory.Exists(FileList[CamInfo.Name]))
                {
                    Directory.CreateDirectory(FileList[CamInfo.Name]);
                }
                SetTriggerMode(CamInfo.TriggerMode);
                SetTriggerSource(CamInfo.MTriggerSource);
                
                //

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
                //
                DevInfo.DState = DeviceState.DisConnectted;
            }
        }
        // ICamera
        public void Capture() 
        {
            try
            {
                // TODO : use caminfo.sn as path or not?
                string p = @"(\.png$|\.PNG$|\.jpg$|\.jpeg$|\.bmp$|\.tif$|\.tiff$)";
                string[] list = Directory.GetFiles(FileList[CamInfo.Name])
                    .Where(f => Regex.IsMatch(f, p, RegexOptions.IgnoreCase)).ToArray();
                if (list.Length > 0)
                {
                    string file = list.OrderBy(x => new Guid()).FirstOrDefault();
                    HObject img;
                    HTuple w = new HTuple(), h = new HTuple(), c = new HTuple();
                    HOperatorSet.GenEmptyObj(out img);
                    HOperatorSet.ReadImage(out img, file);
                    HOperatorSet.GetImageSize(img, out w, out h);
                    HOperatorSet.CountChannels(img, out c);
                    CamInfo.Width = w.I; CamInfo.Height = h.I; CamInfo.Channel = c.I;
                    Image.Dispose();
                    Image = img;
                    ImageHandle?.BeginInvoke(Image, null, null);
                }
            }
            catch { }
        }
        public void SetExpoTime(float us)
        {
            CamInfo.Expotime = us;
        }
        public void SetGain(float us)
        {
            CamInfo.Gain = us;
        }
        public void SetTriggerMode(bool trigger)
        {
            CamInfo.TriggerMode = trigger;
        }
        public void SetTriggerSource(TriggerSource trigger)
        {
            CamInfo.MTriggerSource = trigger;
        }
        // IPubSub
        public void Subscribe(int time, GlobalValModel val) { }
    }
}
