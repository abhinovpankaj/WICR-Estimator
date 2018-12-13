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

            GetMetalDetailsFromGoogle("Dexotex Weather Wear");
            Metals =GetMetals();
            MiscMetals=GetMiscMetals();
            AddOnMetals = GetAddOnMetals();
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
                stairWidth = Js.StairWidth;
                isFlash=Js.IsFlashingRequired;
                if (Js.HasSpecialPricing)
                {
                    ShowSpecialPriceColumn = System.Windows.Visibility.Visible;
                }
                else
                    ShowSpecialPriceColumn = System.Windows.Visibility.Hidden;
            }
            
            var met = GetMetals();
            for (int i = 0; i < Metals.Count; i++)
            {
                double units = Metals[i].Units;
                double sp = Metals[i].SpecialMetalPricing;
                Metals[i] = met[i];
                if (!Metals[i].Name.Contains("STAIR METAL"))
                {
                    Metals[i].Units = units;
                }
                Metals[i].SpecialMetalPricing = sp;
            }
            var addOnMet = GetAddOnMetals();
            for (int i = 0; i < AddOnMetals.Count; i++)
            {
                double units = AddOnMetals[i].Units;
                double sp = AddOnMetals[i].SpecialMetalPricing;
                bool ischecked = AddOnMetals[i].IsMetalChecked;
                AddOnMetals[i] = addOnMet[i];
                if (!AddOnMetals[i].Name.Contains("STAIR METAL"))
                {
                    AddOnMetals[i].Units = units;
                }
                AddOnMetals[i].IsMetalChecked = ischecked;
                AddOnMetals[i].SpecialMetalPricing = sp;
            }
            
            CalculateCost(null);
        }
     

        public ObservableCollection<Metal> GetMetals()
        {
            ObservableCollection<Metal> met = new ObservableCollection<Metal>();
            met.Add(new Metal("L - METAL / FLASHING","4X6",getMetalPR(2), laborRate,0, getMetalMP(2),false));
            met.Add(new Metal("DRIP EDGE METAL","2X4", getMetalPR(5), laborRate, 0, getMetalMP(5), false));
            met.Add(new Metal("STAIR METAL", "4X6", getMetalPR(8), laborRate, getUnits(0), getMetalMP(8), true));
            met.Add(new Metal("STAIR METAL", "3X3", getMetalPR(9), laborRate, getUnits(1), getMetalMP(9), true));
            met.Add(new Metal("DOOR SADDLES", "(4 ft.)", getMetalPR(16), laborRate, 0, getMetalMP(16), false));
            met.Add(new Metal("DOOR SADDLES", "(6 ft.)", getMetalPR(17) ,laborRate, 0, getMetalMP(17), false));
            met.Add(new Metal("DOOR SADDLES", "(8 ft.)", getMetalPR(18), laborRate, 0, getMetalMP(18), false));
            met.Add(new Metal("DOOR SADDLES", "(10 ft.)", getMetalPR(19), laborRate, 0, getMetalMP(19), false));
            met.Add(new Metal("INSIDE CORNER","", getMetalPR(20), laborRate, 0, getMetalMP(20), false));
            met.Add(new Metal("OUTSIDE CORNER","", getMetalPR(21), laborRate, 0, getMetalMP(21), false));
            met.Add(new Metal("INSIDE EDGE CORNER", "", getMetalPR(22), laborRate, 0, getMetalMP(22), false));
            met.Add(new Metal("OUTSIDE EDGE CORNER", "", getMetalPR(23), laborRate, 0, getMetalMP(23), false));
            met.Add(new Metal("DOOR CORNERS SET OF 2 (L&R)", "", getMetalPR(24), laborRate, 0, getMetalMP(24), false));
            met.Add(new Metal("STRINGER TRANSITION CAP", "", getMetalPR(25), laborRate, 0, getMetalMP(25), false));
            met.Add(new Metal("DRIP TERMINATION", "", getMetalPR(40), laborRate, 0, getMetalMP(40), false));
            met.Add(new Metal("2 inch CHIVON DRAINS", "", getMetalPR(29), laborRate,0, getMetalMP(29), false));
            met.Add(new Metal("STANDARD SCUPPER","4x4x9", getMetalPR(32), laborRate, 0, getMetalMP(32), false));
            met.Add(new Metal("SCUPPER WITH A COLLAR","4x4x9", getMetalPR(33), laborRate, 0, getMetalMP(33), false));
            met.Add(new Metal("POST COLLARS w/  KERF", "4x4", getMetalPR(36), laborRate, 0, getMetalMP(36), false));       
            return met;
        }
        

        public ObservableCollection<MiscMetal> GetMiscMetals()
        {
            ObservableCollection<MiscMetal> misc = new ObservableCollection<MiscMetal>();
            misc.Add(new MiscMetal { Name = "Pins & Loads for metal over concrete", Units = getUnits(2), UnitPrice = getUnitPrice(0), MaterialPrice = getMetalMP(37), IsEditable = false });
            misc.Add(new MiscMetal { Name = "Nosing for Concrete risers", Units = getUnits(3), UnitPrice = getUnitPrice(1), MaterialPrice = getMetalMP(38), IsEditable = false });
            misc.Add(new MiscMetal { Name = "OTHER DRAINS TO BE ITEMIZED", Units = 1, UnitPrice = 8, MaterialPrice =0, IsEditable = true });
            return misc;
        } 
        
        
        public ObservableCollection<AddOnMetal>  GetAddOnMetals()
        {
            ObservableCollection<AddOnMetal> met = new ObservableCollection<AddOnMetal>();
            met.Add(new AddOnMetal("L - METAL / FLASHING","4X10", getMetalPR(0), laborRate, 0, getMetalMP(0), false));
            met.Add(new AddOnMetal("L - METAL / FLASHING", "4X8", getMetalPR(1), laborRate, 0, getMetalMP(1), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "4X4", getMetalPR(3), laborRate, 1, getMetalMP(3), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "3X4", getMetalPR(4), laborRate, 1, getMetalMP(4), false));
            met.Add(new AddOnMetal("STAIR METAL", "4X10", getMetalPR(6), laborRate, getUnits(1), getMetalMP(6), true));
            met.Add(new AddOnMetal("STAIR METAL", "4X8", getMetalPR(7), laborRate, getUnits(1), getMetalMP(7), true));
            met.Add(new AddOnMetal("Door Pan", "10' - 12'", getMetalPR(11), laborRate, 1, getMetalMP(11), false));
            met.Add(new AddOnMetal("Door Pan", "8'", getMetalPR(12), laborRate, 2, getMetalMP(12), false));
            met.Add(new AddOnMetal("Door Pan", "6'", getMetalPR(13), laborRate, 3, getMetalMP(13), false));
            met.Add(new AddOnMetal("Door Pan", "4'", getMetalPR(14), laborRate, 6, getMetalMP(14), false));
            met.Add(new AddOnMetal("Door Pan", "3'", getMetalPR(15), laborRate, 6, getMetalMP(15), false));
            met.Add(new AddOnMetal("CORNER DRIP TERMINATION","", getMetalPR(26), laborRate, 1, getMetalMP(26), false));
            met.Add(new AddOnMetal("OFFSET DRIP TERMINATION", "", getMetalPR(27), laborRate, 1, getMetalMP(27), false));
            met.Add(new AddOnMetal("SRAIGHT DRIP TERMINATION", "", getMetalPR(28), laborRate, 1, getMetalMP(28), false));
            met.Add(new AddOnMetal("STANDARD SCUPPER", "2x4x9", getMetalPR(30), laborRate, 1, getMetalMP(30), false));
            met.Add(new AddOnMetal("STANDARD SCUPPER", "3x4x9",  getMetalPR(31), laborRate, 1, getMetalMP(31), false));
            met.Add(new AddOnMetal("OVERFLOW SCUPPER", "", getMetalPR(34), laborRate, 1, getMetalMP(34), false));
            met.Add(new AddOnMetal("TWO STAGE SCUPPER", "", getMetalPR(35), laborRate, 1, getMetalMP(35), false));
            return met;
        }    
    }
}
