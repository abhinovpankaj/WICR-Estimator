using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WICR_Estimator.ViewModels
{
    
    [DataContract]
    public class BaseViewModel : INotifyPropertyChanged
    {
        public static bool IsDirty;

        

        public BaseViewModel()
        { }
        
        public event PropertyChangedEventHandler PropertyChanged;

        //public event EventHandler JobPropertyChanged;
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                Models.JobSetup js = this as Models.JobSetup;
                if (js!=null)
                {                   
                    IsDirty = true;
                }
                
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

        public static event EventHandler TaskStarted;
        public static event EventHandler TaskCompleted;
        public static event EventHandler UpdateTask;

        
        public void OnTaskStarted(string msg)
        {
            TaskStarted?.Invoke(msg, EventArgs.Empty);
        }
        public void OnTaskCompleted(string msg)
        {
            TaskCompleted?.Invoke(msg, EventArgs.Empty);
        }

        public void UpdateTaskStatus(string msg)
        {
            UpdateTask?.Invoke(msg, EventArgs.Empty);
        }

       
    }
}
