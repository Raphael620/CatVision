using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace CatVision.Common.Helper
{
    public class SerializeHelp
    {
        public static T Deserialize<T>(string fileName, bool isLoadCopyFile = false)
        {
            T t = default(T);
            try
            {
                if (!File.Exists(fileName))
                {
                    File.Create(fileName).Close();
                }
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Length == 0 && isLoadCopyFile)//文件内容为空
                {
                    int startIndex = fileName.LastIndexOf(".");
                    string fileCopyName = fileName.Insert(startIndex, "_Copy");
                    if (File.Exists(fileCopyName))
                    {
                        File.Copy(fileCopyName, fileName, true);
                        return (T)JsonConvert.DeserializeObject<T>(File.ReadAllText(fileCopyName));
                    }
                    else
                    {
                        return t;
                    }
                }
                else
                {
                    return (T)JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
                return t;
            }
        }
        public static void SerializeAndSaveFile<T>(T obj, string fileName, bool isCreatCopyFile = false)
        {
            try
            {
                //当项目比较大的时候保存耗时较长，这个时候如果异常断电，那么项目文件会全部丢失，为解决此问题：先序列化一个临时项目文件，序列化成功后再移动替换原文件
                if (isCreatCopyFile)
                {
                    int startIndex = fileName.LastIndexOf(".");
                    string fileCopyName = fileName.Insert(startIndex, "_Copy");
                    File.WriteAllText(fileCopyName, JsonConvert.SerializeObject(obj));
                    File.Copy(fileCopyName, fileName, true);
                }
                else
                {
                    File.WriteAllText(fileName, JsonConvert.SerializeObject(obj));
                }
            }
            catch (Exception e)
            {
                Log.Default.Error(e, "SerializeAndSaveFile");
            }
        }
        public static T Clone<T>(T obj)
        {
            return (T)JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }
        public static void BinSerializeAndSaveFile<T>(T obj, string fileName)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();
            }
            catch (Exception e)
            {
                Log.Default.Error(e, "BinSerializeAndSaveFile");
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
        public static T BinDeserialize<T>(string fileName)
        {
            T t = default(T);
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    return (T)new BinaryFormatter().Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Log.Default.Error(e, "BinDeserialize");
                return t;
            }
        }

    }
}
