using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CatVision.Common;

namespace CatVision.Wpf.Helper
{
    public class CommonHelper
    {
        /// <summary>
        /// RGB转int
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int RgbToInt(System.Windows.Media.Color color)
        {
            return (int)(((int)color.B << 16) | (ushort)(((ushort)color.G << 8) | color.R));
        }

        /// <summary>
        /// int转RGB
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static System.Windows.Media.Color IntToRGB(int color)
        {
            int r = 0xFF & color;
            int g = 0xFF00 & color;
            g >>= 8;
            int b = 0xFF0000 & color;
            b >>= 16;
            return System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b);
        }
        static Bitmap image = new Bitmap(
            Screen.PrimaryScreen.Bounds.Width,
            Screen.PrimaryScreen.Bounds.Height
        );

        public static Image ScreenCapture()
        {
            //Bitmap image = new Bitmap(DispViewID.PrimaryScreen.Bounds.Width, DispViewID.PrimaryScreen.Bounds.Height);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域
            imgGraphics.CopyFromScreen(0, 0, 0, 0,
                new System.Drawing.Size(
                    Screen.PrimaryScreen.Bounds.Width,
                    Screen.PrimaryScreen.Bounds.Height)
            );
            return image;
        }
        /// <summary>
        /// 读取字节某一位的值
        /// </summary>
        /// <param name="data">要读取的字节byte</param>
        /// <param name="index">要读取的位， 值从低到高为 1-8</param>
        /// <returns></returns>
        public static bool GetBitValue(byte data, int index)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index == 1 ? 1 : 2 << (index - 2);
            return ((data & v) != 0);
        }

        public static bool GetBitValue(ushort data, int index)
        {
            if (index > 16 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index == 1 ? 1 : 2 << (index - 2);
            return ((data & v) != 0);
        }

        /// <summary>
        /// UI同步操作
        /// </summary>
        /// <param name="action"></param>
        public static void UISync(Action action)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => action?.Invoke());
        }

        /// <summary>
        /// UI异步操作
        /// </summary>
        /// <param name="action"></param>
        public static void UIAsync(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
