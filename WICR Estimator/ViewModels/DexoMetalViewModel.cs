using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    
    public class DexoMetalViewModel:MetalBaseViewModel
    {
        public DexoMetalViewModel(JobSetup js)
        {
            //GetMetalDetailsFromGoogle("Dexotex Barrier Gaurd");
            GetMetalDetailsFromDB(js.ProjectName);
            Metals = this.GetMetalsDB();
            MiscMetals = this.GetMiscMetalsDB();
            AddOnMetals = this.GetAddOnMetalsDB();
            CalculateCost(null);
            js.JobSetupChange += Js_OnJobSetupChange;
        }

        private void Js_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            OnJobSetupChange(js);
        }
    }
}
