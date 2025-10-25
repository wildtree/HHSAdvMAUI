using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHSAdvMAUI
{
    internal class IntToBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int intValue && parameter is string paramString && int.TryParse(paramString, out int paramValue))
            {
                return intValue == paramValue;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string paramString && int.TryParse(paramString, out int paramValue))
            {
                return boolValue ? paramValue : Binding.DoNothing;
            }
            return Binding.DoNothing;
        }
    }
}
