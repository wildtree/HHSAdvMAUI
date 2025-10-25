using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHSAdvMAUI
{
    internal class StringToBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string strValue && parameter is string paramString)
            {
                return strValue == paramString;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string paramString)
            {
                return boolValue ? paramString : Binding.DoNothing;
            }
            return Binding.DoNothing;
        }
    }
}
