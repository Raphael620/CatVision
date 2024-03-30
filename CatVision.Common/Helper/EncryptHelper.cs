using System;
using System.Management;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace CatVision.Common.Helper
{

    public class LisenceHelper
    {
        private static LisenceHelper instance = new LisenceHelper();
        public static LisenceHelper Ins => instance;
        public bool IsAuthorized { get; private set; }  = false;
        public static string GenerateLisence(string code = "xxxxxxxx")
        {
            string uuid = ComputerInfo.GetUUID();
            string lis1 = DesEncrypt.MyEncrypt(code, uuid.Substring(uuid.Length - 8, 8));
            // string lis2 = MD5Helper.GetMD5String(lis1);
            return lis1;
        }
        public static string MyGetUUIDShort()
        {
            return ComputerInfo.GetUUID().Substring(36 - 8, 8);
        }
        public static bool MyVerifyLisence(string code = "xxxxxxxxxxx=")
        {
            string uuid = MyGetUUIDShort();
            string str1 = MD5Helper.GetMD5String(uuid).Substring(32 - 8, 8);
            string str2 = MD5Helper.GetMD5String(new string(uuid.Reverse().ToArray())).Substring(32 - 8, 8);
            byte[] buf = XorBytes(Encoding.ASCII.GetBytes(str1), Encoding.ASCII.GetBytes(str2));
            string res = Convert.ToBase64String(buf);
            LisenceHelper.Ins.IsAuthorized = String.Equals(res, code, StringComparison.OrdinalIgnoreCase);
            return LisenceHelper.Ins.IsAuthorized;
        }
        public static byte[] XorBytes(byte[] a, byte[] b)
        {
            int length = Math.Min(a.Length, b.Length);
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = (byte)(a[i] ^ b[i]);
            }
            return result;
        }
    }

    public class ComputerInfo
    {
        public static string GetUUID()
        {
            string uuid = string.Empty;
            using (ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct"))
            {
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    uuid = mo.Properties["UUID"].Value.ToString();
                    break;
                }
            }
            return uuid;
        }
        public static string GetCpuID()
        {
            string strCpu = string.Empty;
            using (ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct"))
            {
                ManagementObjectCollection myCpuConnection = mc.GetInstances();
                foreach (ManagementObject myObject in myCpuConnection)
                {
                    strCpu = myObject.Properties["Processorid"].Value.ToString();
                    break;
                }
            }
            return strCpu;
        }
        /// 742F526C
        public static string GetDiskSN()
        {
            string disk = string.Empty;
            using (ManagementObject mc = new ManagementObject("Win32_NetworkAdapterConfiguration"))
            {
                disk = mc.GetPropertyValue("VolumeSerialNumber").ToString();
            }
            return disk;
        }
    }

    public class DesEncrypt
    {
        private const String KEY = "CatVision";
        private static byte[] Key = ASCIIEncoding.ASCII.GetBytes(KEY.Substring(0, 8));
        private static byte[] IV = ASCIIEncoding.ASCII.GetBytes(KEY.Substring(KEY.Length - 8, 8));
        /// <summary>
        /// DES 加密
        /// </summary>
        /// <param name="text">需要加密的值</param>
        /// <returns>加密后的结果</returns>
        public static string Encrypt(string text)
        {
            try
            {
                DESCryptoServiceProvider dsp = new DESCryptoServiceProvider();
                using (MemoryStream memStream = new MemoryStream())
                {
                    CryptoStream crypStream = new CryptoStream(memStream, dsp.CreateEncryptor(Key, IV), CryptoStreamMode.Write);
                    StreamWriter sWriter = new StreamWriter(crypStream);
                    sWriter.Write(text);
                    sWriter.Flush();
                    crypStream.FlushFinalBlock();
                    memStream.Flush();
                    return Convert.ToBase64String(memStream.GetBuffer(), 0, (int)memStream.Length);
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "DesEncrypt.Encrypt");
                return string.Empty;
            }
        }
        public static string MyEncrypt(string text, string key)
        {
            try
            {
                byte[] IV = ASCIIEncoding.ASCII.GetBytes(key.Substring(0, 8));
                DESCryptoServiceProvider dsp = new DESCryptoServiceProvider();
                using (MemoryStream memStream = new MemoryStream())
                {
                    CryptoStream crypStream = new CryptoStream(memStream, dsp.CreateEncryptor(IV, IV), CryptoStreamMode.Write);
                    StreamWriter sWriter = new StreamWriter(crypStream);
                    sWriter.Write(text);
                    sWriter.Flush();
                    crypStream.FlushFinalBlock();
                    memStream.Flush();
                    return Convert.ToBase64String(memStream.GetBuffer(), 0, (int)memStream.Length);
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "DesEncrypt.Encrypt");
                return string.Empty;
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="encryptText"></param>
        /// <returns>解密后的结果</returns>
        public static string Decrypt(string encryptText)
        {
            DESCryptoServiceProvider dsp = new DESCryptoServiceProvider();
            byte[] buffer = Convert.FromBase64String(encryptText);

            using (MemoryStream memStream = new MemoryStream())
            {
                CryptoStream crypStream = new CryptoStream(memStream, dsp.CreateDecryptor(Key, IV), CryptoStreamMode.Write);
                crypStream.Write(buffer, 0, buffer.Length);
                crypStream.FlushFinalBlock();
                return ASCIIEncoding.UTF8.GetString(memStream.ToArray());
            }
        }
        public static string MyDecrypt(string encryptText, string key)
        {
            byte[] IV = ASCIIEncoding.ASCII.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider dsp = new DESCryptoServiceProvider();
            byte[] buffer = Convert.FromBase64String(encryptText);

            using (MemoryStream memStream = new MemoryStream())
            {
                CryptoStream crypStream = new CryptoStream(memStream, dsp.CreateDecryptor(IV, IV), CryptoStreamMode.Write);
                crypStream.Write(buffer, 0, buffer.Length);
                crypStream.FlushFinalBlock();
                return ASCIIEncoding.UTF8.GetString(memStream.ToArray());
            }
        }
    }

    public class MD5Helper
    {
        public static string GetMD5String(string str)
        {
            byte[] psb;
            StringBuilder sb = new StringBuilder(32);
            using (MD5 md = MD5.Create())
            {
                psb = md.ComputeHash(Encoding.UTF8.GetBytes(str));
                for (int i = 0; i < psb.Length; i++)
                {
                    sb.Append(psb[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

    }
}
