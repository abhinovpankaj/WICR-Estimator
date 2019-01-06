using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WICR_Estimator.Converters
{
    
    class UnitTextboxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch (value.ToString())
                {
                    case "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)":
                    case "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH) UPI 7013 SC BASE COAT":
                    case "Striping for small cracKs (less than 1/8\")":
                    case "Route and caulk moving cracks (greater than 1/8\")":
                    case "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC":
                        return true;
                    default:
                        return false;
                }
            }
            else
                return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
