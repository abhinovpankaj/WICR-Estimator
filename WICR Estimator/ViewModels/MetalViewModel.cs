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

        public MetalViewModel()
        { }
        public MetalViewModel(JobSetup js)
        {
            prevailingWage = js.ActualPrevailingWage==0?0:(js.ActualPrevailingWage - laborRate) / laborRate;
            GetMetalDetailsFromGoogle(js.ProjectName);
            if (js.ProjectName== "Paraseal LG")
            {
                Metals = GetMetalsLG();
                AddOnMetals = GetAddOnMetalsLG();
            }
            else
            {
                Metals = GetMetals();
                AddOnMetals = GetAddOnMetals();
            }
                
            MiscMetals =GetMiscMetals();
            if (js.ProjectName=="Multicoat")
            {
                MiscMetals.Where(x => x.Name == "Nosing for Concrete risers").FirstOrDefault().Units = 0;
            }
            //if (js.ProjectName == "Paraseal LG")
            //{
            //    foreach (Metal item in Metals.Where(x=>x.Name.Contains("STAIR METAL")))
            //    {
            //        item.Units = 0;
            //    }
            //}
            CalculateCost(null);
            js.JobSetupChange += JobSetup_OnJobSetupChange;
        }        
        

    }
}
