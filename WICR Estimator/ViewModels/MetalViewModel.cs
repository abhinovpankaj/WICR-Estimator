using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WICR_Estimator.Models;
using WICR_Estimator.Views;

namespace WICR_Estimator.ViewModels
{
    public class MetalViewModel:MetalBaseViewModel
    {
        
        public MetalViewModel(JobSetup js)
        {

            GetMetalDetailsFromGoogle(js.ProjectName);
            Metals =GetMetals();
            AddOnMetals = GetAddOnMetals();
            MiscMetals =GetMiscMetals();
            
            CalculateCost(null);
            js.OnJobSetupChange += JobSetup_OnJobSetupChange;
        }
   
        
    }
}
