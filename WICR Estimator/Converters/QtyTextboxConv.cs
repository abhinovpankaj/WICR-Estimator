using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WICR_Estimator.Converters
{
    public class QtyTextboxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch (value.ToString())
                {
                    case "Bubble Repair(Measure Sq Ft)":
                    case "Large Crack Repair":
                    case "Extra Stair Nosing Lf":
                    case "Extra stair nosing lf":
                    case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                    case "Plywood 3/4 & Blocking(# Of 4X8 Sheets)":
                    case "Stucco Material Remove And Replace (Lf)":
                    case "Stucco Material Remove and replace (LF)":
                    case "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)":
                    case "LARGE CRACK REPAIR":
                    case "BUBBLE REPAIR (MEASURE SQ FT)":
                    case "System Large cracks Repair":
                    case "EXTRA STAIR NOSING":
                    case "System bubbled and failed texture repair":
                    case "Concrete cracks greater than 1/32 inch (route 1/4 x 1/4) epoxy gel fill, ilica sand, and fiber tape":
                    case "Caulk 1/2 to 3/4 inch control joints (SIKA 2C)":
                    case "Remove and Replace Expansion joints- backer rod and sealant (SIKA 2C)":
                    case "Large cracks with reseal (route, fill with speed bond/sand and spot texture)":
                    case "Large cracks with new system (route, fill with speed bond)":
                    case "Bubbled and failed textured areas (prep, patch,spot textured with resistite)":
                    case "Add for saw cut joints":
                    case "Add for removing and replacing concrete (no more than 100 sq ft)":
                    case "UNIVERSAL OUTLET":
                    case "TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\"":
                    case "VISQUINE PROTECTION FOR INCLEMENT WEATHER":
                    case "UNIVERSAL OUTLETS":
                    case "TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)":
                    case "SIDE OUTLET 6\"":
                    case "MIRADRAIN HC 1\" DRAIN-PUNCHED 12\" X 100'  (QUICK DRAIN)":
                    case "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)":
                    case "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"":
                    case "ADD LF FOR DAMMING @ DRIP EDGE":
                    case "CALCIUM CHLORIDE TEST (MINIMUM OF 3 FOR EACH JOB)":
                    case "PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL WITH SAND BROADCAST":
                    case "Add for penetrations  -customer to determine qty":
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
