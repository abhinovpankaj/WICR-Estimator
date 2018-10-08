using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    class WeatherWearViewModel:BaseViewModel, IPageViewModel
    {
        private JobSetup jobsetUp;
        public  JobSetup JobSetup
        {
            get
            {
                return jobsetUp;
            }
            set

            {
                if (jobsetUp!=value)
                {
                    jobsetUp = value;
                    OnPropertyChanged("JobSetup");
                }
            }
        }

        public string Name
        {
            get
            {
                return "Weather Wear";
            }
        }
    }
}
