using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class ZeroMetalViewModel:MetalBaseViewModel
    {
        public ZeroMetalViewModel()
        { }
        public ZeroMetalViewModel(JobSetup js)
        {
            prevailingWage = js.ActualPrevailingWage == 0 ? 0 : (js.ActualPrevailingWage - laborRate) / laborRate;
            GetMetalDetailsFromGoogle(js.ProjectName);
            if (js.ProjectName == "Paraseal LG")
            {
                Metals = GetMetalsLG();
                AddOnMetals = GetAddOnMetalsLG();
            }
            else
            {
                Metals = GetMetals();
                AddOnMetals = GetAddOnMetals();
            }

            MiscMetals = GetMiscMetals();
            if (js.ProjectName == "Multicoat")
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
            if (!js.IsProjectIndependent)
            {
                js.JobSetupChange += JobSetup_OnJobSetupChange;
            }
            
        }
        public override ObservableCollection<Metal> GetMetals()
        {
            ObservableCollection<Metal> met = new ObservableCollection<Metal>();
            met= base.GetMetals();
            foreach (Metal metal in met)
            {
                metal.IsStairMetal = System.Windows.Visibility.Visible;
            }
            return met;
        }

        public override ObservableCollection<MiscMetal> GetMiscMetals()
        {
            ObservableCollection<MiscMetal> misc = new ObservableCollection<MiscMetal>();
            misc.Add(new MiscMetal { Name = "Pins & Loads for metal over concrete", Units = 0, UnitPrice = getUnitPrice(0), MaterialPrice = getMetalMP(37), IsEditable = true });
            misc.Add(new MiscMetal { Name = "Nosing for Concrete risers", Units = 0, UnitPrice = getUnitPrice(1), MaterialPrice = getMetalMP(38), IsEditable = true });
            misc.Add(new MiscMetal { Name = "OTHER DRAINS TO BE ITEMIZED", Units = 0, UnitPrice = 0, MaterialPrice = 0, IsEditable = true });
            return misc;
        }
    }
}
