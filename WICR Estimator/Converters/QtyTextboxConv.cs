﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WICR_Estimator.Converters
{
    class QtyTextboxConverter : IValueConverter
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
