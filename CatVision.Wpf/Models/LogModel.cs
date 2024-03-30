using System;
using System.Windows.Media;

namespace CatVision.Wpf.Models
{
    public class LogModel
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public eMsgType LogType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush LogColor { get; set; }
    }
    public enum eMsgType
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 消息
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 报错(不置位报警标志，设备可以继续运行)
        /// </summary>
        Error,
        /// <summary>
        /// 报警(置位报警标志，设备不能继续运行)
        /// </summary>
        Alarm,
    }
}
