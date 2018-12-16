using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;
using System.Windows.Input;
using System.ComponentModel;

namespace WICR_Estimator.ViewModels
{

    public class MaterialViewModel : MaterialBaseViewModel
    {

        public MaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            FetchMaterialValuesAsync(false);
        }
    }
}
