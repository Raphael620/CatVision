using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CatVision.Common.Helper
{
    class MessageHelper
    {
    }
    public class LogMsgModel
    {
        public LogEventLevel Level { get; set; }
        public string level { get; set; }
        public string msg { get; set; }
        public Exception ex { get; set; }
        public object addin { get; set; }
        public LogMsgModel() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l">"debug" "info" "warn" "error" "fatal"</param>
        public LogMsgModel(string _level)
        {
            if (_level == "debug") { Level = LogEventLevel.Debug; }
            else if (_level == "info") { Level = LogEventLevel.Information; }
            else if (_level == "warn") { Level = LogEventLevel.Warning; }
            else if (_level == "error") { Level = LogEventLevel.Error; }
            else if (_level == "fatal") { Level = LogEventLevel.Fatal; }
            else { Level = LogEventLevel.Information; }
        }
    }
    public class HostMessage : RequestMessage<bool> { }
}
