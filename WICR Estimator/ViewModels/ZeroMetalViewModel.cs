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
            met.Add(new Metal("L - METAL / FLASHING", "4X6", getMetalPR(2), laborRate, 0, getMetalMP(2), false));
            met.Add(new Metal("DRIP EDGE METAL", "2X4", getMetalPR(5), laborRate, 0, getMetalMP(5), false));
            met.Add(new Metal("STAIR METAL", "4X6", getMetalPR(8), laborRate, getUnits(0), getMetalMP(8), false));
            met.Add(new Metal("STAIR METAL", "3X3", getMetalPR(9), laborRate, getUnits(1), getMetalMP(9), false));
            met.Add(new Metal("DOOR SADDLES", "(4 ft.)", getMetalPR(16), laborRate, 0, getMetalMP(16), false));
            met.Add(new Metal("DOOR SADDLES", "(6 ft.)", getMetalPR(17), laborRate, 0, getMetalMP(17), false));
            met.Add(new Metal("DOOR SADDLES", "(8 ft.)", getMetalPR(18), laborRate, 0, getMetalMP(18), false));
            met.Add(new Metal("DOOR SADDLES", "(10 ft.)", getMetalPR(19), laborRate, 0, getMetalMP(19), false));
            met.Add(new Metal("INSIDE CORNER", "", getMetalPR(20), laborRate, 0, getMetalMP(20), false));
            met.Add(new Metal("OUTSIDE CORNER", "", getMetalPR(21), laborRate, 0, getMetalMP(21), false));
            met.Add(new Metal("INSIDE EDGE CORNER", "", getMetalPR(22), laborRate, 0, getMetalMP(22), false));
            met.Add(new Metal("OUTSIDE EDGE CORNER", "", getMetalPR(23), laborRate, 0, getMetalMP(23), false));
            met.Add(new Metal("DOOR CORNERS SET OF 2 (L&R)", "", getMetalPR(24), laborRate, 0, getMetalMP(24), false));
            met.Add(new Metal("STRINGER TRANSITION CAP", "", getMetalPR(25), laborRate, 0, getMetalMP(25), false));
            met.Add(new Metal("DRIP TERMINATION", "", getMetalPR(40), laborRate, 0, getMetalMP(40), false));
            met.Add(new Metal("2 inch DRAINS", "", getMetalPR(29), laborRate, 0, getMetalMP(29), false));//changed name ,removed Chivon as per mail on 12th Sept 2019.
            met.Add(new Metal("STANDARD SCUPPER", "4x4x9", getMetalPR(32), laborRate, 0, getMetalMP(32), false));
            met.Add(new Metal("SCUPPER WITH A COLLAR", "4x4x9", getMetalPR(33), laborRate, 0, getMetalMP(33), false));
            met.Add(new Metal("POST COLLARS w/  KERF", "4x4", getMetalPR(36), laborRate, 0, getMetalMP(36), false));
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

        public override void updateLaborCost()
        {
            double stairCost = 0;
            double addOnMetalCost = 0;
            double normCost = 0;
            IEnumerable<Metal> stairMetals = Metals.Where(x => x.IsStairMetalChecked == true);
            if (stairMetals.Count() > 0)
            {
                stairCost = stairMetals.Select(x => x.LaborExtension).Sum();
            }

            //IEnumerable<Metal> normMetals = Metals.Where(x => x.Name.Contains("STAIR") == false);
            //if (normMetals.Count() > 0)
            //{
            //    normCost = normMetals.Select(x => x.LaborExtension).Sum();
            //}

            //New Changes for Addon Metals
            IEnumerable<AddOnMetal> selectedAddOnMetals = AddOnMetals.Where(x => x.IsMetalChecked);
            if (selectedAddOnMetals.Count() > 0)
            {
                addOnMetalCost = selectedAddOnMetals.Select(x => x.LaborExtension).Sum();
            }

            normCost = Math.Round(normCost + stairCost + addOnMetalCost + MiscMetals.Select(x => x.LaborExtension).Sum(), 2);

            if (isPrevailingWage)
            {
                if (isDiscount)
                {
                    TotalLaborCost = Math.Round(normCost * (1 + prevailingWage + deductionOnLargeJob), 2);
                }
                else
                    TotalLaborCost = Math.Round(normCost * (1 + prevailingWage), 2);

            }
            else
            {
                if (isDiscount)
                {
                    TotalLaborCost = Math.Round(normCost * (1 + deductionOnLargeJob), 2);
                }
                else
                    TotalLaborCost = Math.Round(normCost, 2);
            }
        }

        protected override void updateMaterialCost()
        {
            double stairCost = 0;
            double normCost = 0;
            double nl = Nails / 100;
            double addOnMetalCost = 0;
            if (Metals.Count > 0 && MiscMetals.Count > 0)
            {
                IEnumerable<Metal> stairMetals = Metals.Where(x => x.IsStairMetalChecked == true);
                if (stairMetals.Count() > 0)
                {
                    stairCost = stairMetals.Select(x => x.MaterialExtension).Sum();
                }
                IEnumerable<AddOnMetal> selectedAddOnMetals = AddOnMetals.Where(x => x.IsMetalChecked == true);
                if (selectedAddOnMetals.Count() > 0)
                {
                    addOnMetalCost = selectedAddOnMetals.Select(x => x.MaterialExtension).Sum();
                }

                //IEnumerable<Metal> normMetals = Metals.Where(x => x.Name.Contains("STAIR") == false);
                //if (normMetals.Count() > 0)
                //{
                //    normCost = normMetals.Select(x => x.MaterialExtension).Sum();
                //}

                //double misSum = Metals.Select(x => x.MaterialExtension).Sum() * nl;

                TotalMaterialCost = Math.Round((normCost + stairCost) * (1 + nl) + addOnMetalCost * (1 + nl) + MiscMetals.Select(x => x.MaterialExtension).Sum(), 2);
            }

        }
    }
}
