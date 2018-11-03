using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //public event EventHandler JobPropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                //switch (propertyName)
                //{
                //    case "SpecialProductName":
                //    case "HasContingencyDisc":
                //    case "WeatherWearType":
                //    case "JobName":
                //    case "VendorName":
                //    case "TotalSqft":
                //    case "DeckPerimeter":
                //    case "RiserCount":
                //    case "IsApprovedForSandCement":
                //    case "IsPrevalingWage":
                //    case "HasSpecialMaterial":
                //    case "IsFlashingRequired":
                //    case "HasSpecialPricing":
                //    case "HasDiscount":
                //    case "MarkupPercentage":
                //    case "StairWidth":
                //    case "MaterialName":
                //        JobPropertyChanged.Invoke(this, EventArgs.Empty);
                //    default:
                //        break;
                //}
            }
        }
    }
}
