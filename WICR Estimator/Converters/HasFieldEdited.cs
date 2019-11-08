using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WICR_Estimator.Converters
{
    public class HasFieldEdited : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            //DataGridCellInfo info =(DataGridCellInfo) value;

            if (value != null)
            {
                Double.TryParse(value.ToString(), out val);
                if (val != 0)
                {
                    return true;
                }
            }
            return false;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
