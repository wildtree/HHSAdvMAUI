using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHSAdvMAUI
{
    internal class EnumToBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null && parameter is string paramString)
            {
                return value.ToString() == paramString;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string paramString)
            {
                if (boolValue)
                {
                    return Enum.Parse(targetType, paramString);
                }
                return Binding.DoNothing;
            }
            return Binding.DoNothing;
        }
    }
}
