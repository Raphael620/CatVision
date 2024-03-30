using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CatVision.Wpf.Converter
{
    public class FloatDivTwoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(float))
            {
                return (float)value / 2.0;
            }
            else if (targetType == typeof(double))
            {
                return (double)value / 2.0;
            }
            else
            {
                throw new InvalidOperationException("The target must be a boolean");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(float))
            {
                return (float)value * 2.0;
            }
            else if (targetType == typeof(double))
            {
                return (double)value * 2.0;
            }
            else
            {
                throw new InvalidOperationException("The target must be a boolean");
            }
        }
    }
}
