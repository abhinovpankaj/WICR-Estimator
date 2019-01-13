using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class EnduroKoteSlopeViewModel:SlopeBaseViewModel
    {
        public EnduroKoteSlopeViewModel(JobSetup Js)
        {
            GetSlopeDetailsFromGoogle(Js.ProjectName);
            Js.SlopeMaterialName = "ECC Enduro-Crete";
            SlopeMaterialName = Js.IsApprovedForSandCement ? "Sand and Cement" : "ECC Enduro-Crete";
            Slopes = CreateSlopes();
            CalculateAll();
            Js.OnJobSetupChange += JobSetup_OnJobSetupChange;
        }

        public override double getPricePerMix(string thickness, bool isApproved,int addRow=0)
        {
            return base.getPricePerMix(thickness, isApproved);
            //double result;
            //double.TryParse(perMixRates[0][1].ToString(), out result);
            //return result;
        }
    }
}
