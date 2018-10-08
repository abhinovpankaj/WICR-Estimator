using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WICR_Estimator.Converters
{
    class CheckBoxVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value!=null)
            {
                if (value.ToString().Contains("STAIR METAL"))
                {
                    if (value.ToString().Length!=0)
                    {
                        return System.Windows.Visibility.Visible;
                    }
                    else
                        return System.Windows.Visibility.Hidden;

                }
                else
                {
                    try
                    {
                        if ((bool)value)
                        {
                            return System.Windows.Visibility.Visible;
                        }
                        else
                            return System.Windows.Visibility.Hidden;
                    }
                    catch
                    {
                        return System.Windows.Visibility.Hidden;
                    }
                }
                
                
            }
            else
            {
                return System.Windows.Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
