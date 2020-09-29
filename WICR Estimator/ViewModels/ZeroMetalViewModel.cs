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
            GetMetalDetailsFromDB(js.ProjectName);
            if (js.ProjectName == "Paraseal LG")
            {
                Metals = GetMetalsDB("LG");
                AddOnMetals = GetAddOnMetalsDB("LG");
            }
            else
            {
                Metals = GetMetalsDB();
                AddOnMetals = GetAddOnMetalsDB();
            }

            MiscMetals = GetMiscMetalsDB();
            
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
        public override ObservableCollection<Metal> GetMetalsDB(string type="")
        {
            ObservableCollection<Metal> met = new ObservableCollection<Metal>();
            met.Add(new Metal("L - METAL / FLASHING", "4X6", getMetalPR("L-Metal/Flashing 4'X6''"), laborRate, 0, getMetalMP("L-Metal/Flashing 4'X6''"), false));
            met.Add(new Metal("DRIP EDGE METAL", "2X4", getMetalPR("Drip Edge Metal 2\" x 4\""), laborRate, 0, getMetalMP("Drip Edge Metal 2\" x 4\""), false));
            met.Add(new Metal("STAIR METAL", "4X6", getMetalPR("Stair Metal 4X6"), laborRate, getUnits(0), getMetalMP("Stair Metal 4X6"), true));
            met.Add(new Metal("STAIR METAL", "3X3", getMetalPR("Stair Metal 3X3"), laborRate, getUnits(1), getMetalMP("Stair Metal 3X3"), true));
            met.Add(new Metal("DOOR SADDLES", "(4 ft.)", getMetalPR("Door Saddles (4 Ft.)"), laborRate, 0, getMetalMP("Door Saddles (4 Ft.)"), false));
            met.Add(new Metal("DOOR SADDLES", "(6 ft.)", getMetalPR("Door Saddles (6Ft)"), laborRate, 0, getMetalMP("Door Saddles (6Ft)"), false));
            met.Add(new Metal("DOOR SADDLES", "(8 ft.)", getMetalPR("Door Saddles (8Ft)"), laborRate, 0, getMetalMP("Door Saddles (8Ft)"), false));
            met.Add(new Metal("DOOR SADDLES", "(10 ft.)", getMetalPR("Door Saddles (10 Ft)"), laborRate, 0, getMetalMP("Door Saddles (10 Ft)"), false));
            met.Add(new Metal("INSIDE CORNER", "", getMetalPR("Inside Corner"), laborRate, 0, getMetalMP("Inside Corner"), false));
            met.Add(new Metal("OUTSIDE CORNER", "", getMetalPR("Outside Corner"), laborRate, 0, getMetalMP("Outside Corner"), false));
            met.Add(new Metal("INSIDE EDGE CORNER", "", getMetalPR("Inside Edge Corner"), laborRate, 0, getMetalMP("Inside Edge Corner"), false));
            met.Add(new Metal("OUTSIDE EDGE CORNER", "", getMetalPR("Outside Edge Corner"), laborRate, 0, getMetalMP("Outside Edge Corner"), false));
            met.Add(new Metal("DOOR CORNERS SET OF 2 (L&R)", "", getMetalPR("Door Corners Set Of 2 (L&R)"), laborRate, 0, getMetalMP("Door Corners Set Of 2 (L&R)"), false));
            met.Add(new Metal("STRINGER TRANSITION CAP", "", getMetalPR("Stringer Transition Cap"), laborRate, 0, getMetalMP("Stringer Transition Cap"), false));
            met.Add(new Metal("DRIP TERMINATION", "", getMetalPR("Drip Termination"), laborRate, 0, getMetalMP("Drip Termination"), false));
            met.Add(new Metal("2 inch DRAINS", "", getMetalPR("2 Inch Chivon Drains"), laborRate, 0, getMetalMP("2 Inch Chivon Drains"), false));//changed name ,removed Chivon as per mail on 12th Sept 2019.
            met.Add(new Metal("STANDARD SCUPPER", "4x4x9", getMetalPR("4\" x 4\" x 9\" standard scupper"), laborRate, 0, getMetalMP("4\" x 4\" x 9\" standard scupper"), false));
            met.Add(new Metal("SCUPPER WITH A COLLAR", "4x4x9", getMetalPR("Scupper With A Collar 4X4X9"), laborRate, 0, getMetalMP("Scupper With A Collar 4X4X9"), false));
            met.Add(new Metal("POST COLLARS w/  KERF", "4x4", getMetalPR("Post Collars 4X4 W/ Kerf"), laborRate, 0, getMetalMP("Post Collars 4X4 W/ Kerf"), false));

            foreach (Metal metal in met)
            {
                metal.IsStairMetal = System.Windows.Visibility.Visible;
            }
 
            return met;
        }

        public override ObservableCollection<MiscMetal> GetMiscMetalsDB()
        {
            ObservableCollection<MiscMetal> misc = new ObservableCollection<MiscMetal>();
            misc.Add(new MiscMetal
            {
                Name = "Pins & Loads for metal over concrete",
                Units = getUnits(2),
                UnitPrice = getUnitPrice("Pins & Loads for metal over concrete"),
                MaterialPrice = getMetalMP("Pins & Loads for metal over concrete"),
                IsEditable = false
            });
            misc.Add(new MiscMetal { Name = "Nosing for Concrete risers", Units = getUnits(3), UnitPrice = getUnitPrice("Nosing for Concrete risers"), MaterialPrice = getMetalMP("Nosing for Concrete risers"), IsEditable = false });
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
