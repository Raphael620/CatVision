using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;

namespace CatVision.Common
{
    public class Log : IDisposable
    {
        // TODO : use factory, not default
        private static Log instance = new Log(true, false, false, "");
        public static Log Default => instance;
        
        public Logger logger;
        public Log(bool debug = true, bool console = false, bool file = true, string path = "")
        {
            LoggerConfiguration config = new LoggerConfiguration();
            config.MinimumLevel.Information();
            if (debug) config.WriteTo.Debug();
            if (console) config.WriteTo.Console();
            if (file)
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "log");
                }
                config.WriteTo.File(path, rollingInterval: RollingInterval.Day);
            }
            logger = config.CreateLogger();
        }
        public static Log CreateLogger()
        {
            if (instance is null)
            {
                instance = new Log();
            }
            return instance;
        }
        public void Info(string msg) { logger.Information(msg); }
        public void Info(string msg, params object[] args) { logger.Information(string.Format(msg, args)); }
        public void Warn(string msg) { logger.Warning(msg); }
        public void Warn(string msg, params object[] args) { logger.Warning(string.Format(msg, args)); }
        public void Error(string msg) { logger.Error(msg); }
        public void Error(string msg, params object[] args) { logger.Error(string.Format(msg, args)); }
        public void Error(Exception ex, string msg) { logger.Error(ex, msg); }
        public void Error(Exception ex, string msg, params object[] args) { logger.Error(ex, string.Format(msg, args)); }
        public void Fatal(Exception ex, string msg, params object[] args) { logger.Fatal(ex, string.Format(msg, args)); }

        public void Dispose()
        {
            Serilog.Log.CloseAndFlush();
        }
    }
}
