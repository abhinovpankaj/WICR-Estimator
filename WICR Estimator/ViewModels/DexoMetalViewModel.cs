using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class DexoMetalViewModel:MetalBaseViewModel
    {
        public DexoMetalViewModel(JobSetup js)
        {
            GetMetalDetailsFromGoogle("Dexotex Barrier Gaurd");
            Metals = this.GetMetals();
            MiscMetals = this.GetMiscMetals();
            AddOnMetals = this.GetAddOnMetals();
            CalculateCost(null);
            js.OnJobSetupChange += Js_OnJobSetupChange;
        }

        private void Js_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            OnJobSetupChange(js);
        }
    }
}
