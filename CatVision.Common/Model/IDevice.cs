using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatVision.Common.Enum;

namespace CatVision.Common.Model
{
    /// <summary>
    /// 通用设备接口
    /// </summary>
    public interface IDevice
    {
        DeviceInfo DevInfo { get; set; }
        void Init(params object[] paras);
        void Uninit(params object[] paras);
        void Connect(params object[] paras);
        void DisConnect(params object[] paras);
    }
    /// <summary>
    /// 发布者、订阅者 数据接口
    /// </summary>
    public interface IPubish
    {
        Action<GlobalValModel> Publish { get; set; }
    }
    public interface ISubscribe
    {
        // Action<string, mValueType, object> Subscribe { get; set; }
        void Subscribe(int timeSpan, GlobalValModel val);
        void DisSubscribe(GlobalValModel val);
    }
}
