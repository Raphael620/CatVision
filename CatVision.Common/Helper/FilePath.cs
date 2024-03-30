using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatVision.Common.Helper
{
    public static class FilePath
    {
        private static string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "config");
        public static string ConfigFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(configFilePath))
                {
                    configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "config");
                }
                if (!Directory.Exists(configFilePath))
                {
                    Directory.CreateDirectory(configFilePath);
                }
                return configFilePath;
            }
        }
        
        private static string configFile = Path.Combine(ConfigFilePath, "config.json");
        public static string ConfigFile
        {
            get
            {
                if (!File.Exists(configFile))
                {
                    File.Create(configFile).Close();
                }
                return configFile;
            }
        }
    }
}
