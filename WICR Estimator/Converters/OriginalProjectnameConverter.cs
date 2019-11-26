using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WICR_Estimator.Models;

namespace WICR_Estimator.Converters
{
    public class OriginalProjectnameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Project prj = value as Project;
            if (prj != null)
            {
                if (prj.GrpName == "Copied")
                {
                    return prj.Name;
                }
            }
            
            return prj.OriginalProjectName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
