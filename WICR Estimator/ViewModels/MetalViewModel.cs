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
        {

            GetMetalDetailsFromGoogle("Weather Wear");
            Metals =GetMetals();
            MiscMetals=GetMiscMetals();
            CalculateCost(null);
            JobSetup.OnJobSetupChange += JobSetup_OnJobSetupChange;
        }

        private void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            OnJobSetupChange(js);
        }

        public void OnJobSetupChange(JobSetup Js)
        {
            
            if (Js!=null)
            {
                MetalName = Js.MaterialName;
                isPrevailingWage = Js.IsPrevalingWage;
                isDiscount = Js.HasDiscount;
                vendorName = Js.VendorName;
                if (Js.HasSpecialPricing)
                {
                    ShowSpecialPriceColumn = System.Windows.Visibility.Visible;
                }
                else
                    ShowSpecialPriceColumn = System.Windows.Visibility.Hidden;
            }
            int i = 0;
            foreach (Metal metal in Metals)
            {
                metal.ProductionRate = getMetalPR(i);
                i++;
            }
            CalculateCost(null);

        }
     

        public ObservableCollection<Metal> GetMetals()
        {
            ObservableCollection<Metal> met = new ObservableCollection<Metal>();
            met.Add(new Metal("L - METAL / FLASHING",getMetalPR(0), laborRate,1, getMetalMP(0),false));
            met.Add(new Metal("DRIP EDGE METAL", getMetalPR(1), laborRate, 1, getMetalMP(1), false));
            met.Add(new Metal("STAIR METAL 4X6", getMetalPR(2), laborRate, getUnits(0), getMetalMP(2), true));
            met.Add(new Metal("STAIR METAL 3X3", getMetalPR(3), laborRate, getUnits(1), getMetalMP(3), true));
            met.Add(new Metal("DOOR SADDLES (4 ft.)", getMetalPR(4), laborRate, 1, getMetalMP(4), false));
            met.Add(new Metal("DOOR SADDLES (6 ft.)", getMetalPR(5) ,laborRate, 2, getMetalMP(5), false));
            met.Add(new Metal("DOOR SADDLES (8 ft.)", getMetalPR(6), laborRate, 3, getMetalMP(6), false));
            met.Add(new Metal("DOOR SADDLES (10 ft.)", getMetalPR(7), laborRate, 6, getMetalMP(7), false));
            met.Add(new Metal("INSIDE CORNER", getMetalPR(8), laborRate, 1, getMetalMP(8), false));
            met.Add(new Metal("OUTSIDE CORNER", getMetalPR(9), laborRate, 1, getMetalMP(9), false));
            met.Add(new Metal("INSIDE EDGE CORNER", getMetalPR(10), laborRate, 1, getMetalMP(10), false));
            met.Add(new Metal("OUTSIDE EDGE CORNER", getMetalPR(11), laborRate, 1, getMetalMP(11), false));
            met.Add(new Metal("DOOR CORNERS SET OF 2 (L&R)", getMetalPR(12), laborRate, 1, getMetalMP(12), false));
            met.Add(new Metal("STRINGER TRANSITION CAP", getMetalPR(13), laborRate, 1, getMetalMP(13), false));
            met.Add(new Metal("DRIP TERMINATION", getMetalPR(14), laborRate, 1, getMetalMP(14), false));
            met.Add(new Metal("2 inch CHIVON DRAINS", getMetalPR(15), laborRate, 1, getMetalMP(15), false));
            met.Add(new Metal("STANDARD SCUPPER 4x4x9", getMetalPR(16), laborRate, 1, getMetalMP(16), false));
            met.Add(new Metal("SCUPPER WITH A COLLAR 4x4x9", getMetalPR(17), laborRate, 1, getMetalMP(17), false));
            met.Add(new Metal("POST COLLARS 4x4 w/  KERF", getMetalPR(18), laborRate, 1, getMetalMP(18), false));       
            return met;
        }
        

        public ObservableCollection<MiscMetal> GetMiscMetals()
        {
            ObservableCollection<MiscMetal> misc = new ObservableCollection<MiscMetal>();
            misc.Add(new MiscMetal { Name = "Pins & Loads for metal over concrete", Units = getUnits(2), UnitPrice = getUnitPrice(0), MaterialPrice = getMetalMP(19), IsEditable = false });
            misc.Add(new MiscMetal { Name = "Nosing for Concrete risers", Units = getUnits(3), UnitPrice = getUnitPrice(1), MaterialPrice = getMetalMP(20), IsEditable = false });
            misc.Add(new MiscMetal { Name = "OTHER DRAINS TO BE ITEMIZED", Units = 1, UnitPrice = 8, MaterialPrice = 15, IsEditable = true });
            return misc;
        }        
    }
}
