using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shedule.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainScheduleItemControl.xaml
    /// </summary>
    public partial class MainScheduleItemControl : UserControl
    {
        public MainScheduleItemControl()
        {
            InitializeComponent();
        }
    }

    public class FirstLetterFromStrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            return value.ToString().Substring(0, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class BuildingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            if(value is int)
            {
                if((int)value == 0)
                {
                    return "Старый";
                }else if((int)value == 1)
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
