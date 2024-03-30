using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace CatVision.Common.Helper
{
    public static class MemoryHelper
    {
        /// <summary>
        /// 对象深拷贝
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>内存地址不同，数据相同的对象</returns>
        public static T DeepCopy<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象

                retval = bf.Deserialize(ms);
            }
            return (T)retval;

        }
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ReleaseMemory()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(60000);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                    }
                }
            });
        }
    }
}
