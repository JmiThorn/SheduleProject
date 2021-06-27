using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Shedule.Utils
{
    public class BuildingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            if (value is int)
            {
                if ((int)value == 1)
                {
                    return "Старый";
                }
                else if ((int)value == 2)
                {
                    return "Новый";
                }
                else
                {
                    return "Неизвестный";
                }
            }
            else
            {
                return "Неизвестный";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
