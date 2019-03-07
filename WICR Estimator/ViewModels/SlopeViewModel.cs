using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    
    public class SlopeViewModel : SlopeBaseViewModel
    {
        public SlopeViewModel()
        { }
        public SlopeViewModel(JobSetup js)
        {
            prevailingWage = js.ActualPrevailingWage == 0 ? 0 : (js.ActualPrevailingWage - laborRate) / laborRate; 
            GetSlopeDetailsFromGoogle(js.ProjectName);
            SlopeMaterialName = js.IsApprovedForSandCement ? "Sand and Cement" : "Dexotex A-81 Underlayment";
            isApprovedForCement = js.IsApprovedForSandCement;
            Slopes = CreateSlopes();
            js.SlopeMaterialName = "Dexotex A-81 Underlayment";
            
            CalculateAll();        
            js.JobSetupChange += JobSetup_OnJobSetupChange;                   
        }
        
    }
}
