using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WICR_Estimator.Converters
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null)
                    if ((bool)value)
                    {
                        return Visibility.Visible;
                    }
                    else
                        return Visibility.Hidden;

                else
                    return System.Windows.Visibility.Hidden;
            }
            catch (Exception)
            {

                return System.Windows.Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
