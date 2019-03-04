using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    class ParasealMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;

        public ParasealMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            
            FillMaterialList();

            //FetchMaterialValuesAsync(false);

        }

        private void FillMaterialList()
        {
            
        }
    }
}
