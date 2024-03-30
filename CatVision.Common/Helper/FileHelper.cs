using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CatVision.Common.Helper
{
    public class TxtHelper
    {
        /// <summary>
        /// 写入string数组到txt文件中
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="Strs">字符串</param>
        public static void WriteTxtToFile(string path, string fileName, string[] strs)
        {
            try
            {
                string fileFullName = Path.Combine(path, fileName);
                if (!File.Exists(fileFullName)) return;
                using (StreamWriter sw = new StreamWriter(fileFullName))
                {
                    foreach (string s in strs)
                    {
                        sw.WriteLine(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "TxtHelper.WriteTxtToFile");
            }
        }
        public static string ReadContentFromTxt(string fileFullName)
        {
            string str = "";
            try
            {
                if (!File.Exists(fileFullName)) return str;
                using (StreamReader sr = new StreamReader(fileFullName, Encoding.Default))
                {
                    str = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "ReadContentFromTxt");
            }
            return str;
        }
    }

    public static class CsvHelper
    {
        public static void CreateCSV(string fileName, string filePath, string[] headers, bool useHead = false)
        {
            string fileRoute = Path.Combine(filePath, fileName);
            StreamWriter sw = null;
            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                if (!File.Exists(fileRoute))
                {
                    sw = new StreamWriter(fileRoute, true, Encoding.GetEncoding("gb2312"));
                    if (useHead)
                    {
                        string head = "";
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (i != headers.Length - 1) { head += headers[i] + ","; }
                            else { head += headers[i]; }
                        }
                        sw.WriteLine(head);
                    }
                }
            }
            catch { }
            finally { if (sw != null) sw.Close(); }
        }

        public static void SaveCSV(string fileName, string filePath, string[] contents)
        {
            string fileRoute = Path.Combine(filePath, fileName);
            StreamWriter sw = null;
            try
            {
                if (File.Exists(fileRoute))
                {
                    sw = new StreamWriter(fileRoute, true, Encoding.GetEncoding("gb2312"));
                    string data = "";
                    for (int i = 0; i < contents.Length; i++)
                    {
                        if (i != contents.Length - 1)
                        {
                            data += contents[i] + ",";
                        }
                        else
                        {
                            data += contents[i];
                        }
                    }
                    sw.WriteLine(data);
                }
            }
            catch { }
            finally { if (sw != null) { sw.Close(); } }
        }

        public static void ReadCSV(string fileName, string filePath, List<string> contents, bool useHead = true)
        {
            string fileRoute = Path.Combine(filePath, fileName);
            StreamReader sr = null;
            try
            {
                if (File.Exists(fileRoute))
                {
                    Encoding encoding = Encoding.Default;
                    FileStream fs = new FileStream(fileRoute, FileMode.Open, FileAccess.Read);
                    sr = new System.IO.StreamReader(fs, encoding);
                    string strLine = "";
                    bool IsFirst = true;
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        if (IsFirst == true)
                        {
                            if (useHead) { IsFirst = false; continue; }
                            else { IsFirst = false; contents.Add(strLine); }
                        }
                        else
                        {
                            contents.Add(strLine);
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        public static void writeData(string fileName, string filePath, string[] contents)
        {
            if (!Path.HasExtension(fileName)) fileName = fileName + ".csv";
            if (!File.Exists(Path.Combine(filePath, fileName)))
            {
                CreateCSV(fileName, filePath, contents, false);
            }
            if (File.Exists(Path.Combine(filePath, fileName)))
            {
                SaveCSV(fileName, filePath, contents);
            }
        }
    }
}
