using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WICR_Estimator.Converters
{
    class ProjectIDConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case 1:
                    return "Weather Wear";
                    
                case 2:
                    return "Weather Wear Rehab";
                    
                case 3:
                    return "Barrier Guard";
                    
                    
                case 4:
                    return "Endurokote";
                    
                case 5:
                    return "Desert Crete";
                    
                case 6:
                    return "Paraseal";
                    
                case 7:
                    return "Paraseal LG";
                    
                case 8:
                    return "860 Carlisle";
             
                    
                case 9:
                    return "201";
                    
                case 10:
                    return "250 GC";
                    
                case 11:
                    return "Pli-Dek";

                    
                case 12:
                    return "Pedestrian Systems";
                case 13:
                    return "Parking Garage";
                case 14:
                    return "Tufflex";
                    
                case 15:
                    return "Color Wash Reseal";
                    
                case 16:
                    return "ALX";
                    
                case 17:
                    return "MACoat";
                
                case 18:
                    return "Reseal All Systems";
                    
                case 19:
                    return "Resistite";
                    
                case 20:
                    return "Multicoat";

                    
                case 21:
                    return "Dexellent II";
                    
                case 22:
                    return "Westcoat BT";
                    
                case 23:
                    return "UPI BT";
                    
                case 24:
                    return "Dual Flex";
                    
                case 25:
                    return "Westcoat Epoxy";
                case 26:
                    return "Polyurethane Injection Block";

                case 27:
                    return "Xypex";

                case 28:
                    return "Blank";

                
                default:
                    return "";
                   
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
