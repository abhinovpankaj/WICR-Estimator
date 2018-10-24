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
        private ICommand _calculateCostCommand;
        public ICommand CalculateCostCommand
        {
            get
            {
                if (_calculateCostCommand == null)
                {
                    _calculateCostCommand = new DelegateCommand(CalculateCost, CanCalculate);
                }
                return _calculateCostCommand;
            }
        }
        private bool CanCalculate(object obj)
        {
            return true;
        }
        public MetalViewModel()
        {
            
            MetalViewModelAsync();
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
        private void MetalViewModelAsync()
        {
            if (pWage == null)
            {
                //pWage = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "E60:E61");
                GSData gsData = DataSerializer.DSInstance.deserializeGoogleData("Weather Wear");
                pWage = gsData.LaborData;
                double.TryParse(gsData.LaborRate[0][0].ToString(),out laborRate);
                double nails;
                double.TryParse(gsData.MetalData[21][1].ToString(),out nails);
                Nails = nails;
                metalDetails = gsData.MetalData;
            }
            
            double.TryParse(pWage[0][0].ToString(), out prevailingWage);    
            double.TryParse(pWage[1][0].ToString(), out deductionOnLargeJob);

        }       

        private void CalculateCost(object obj)
        {
            updateLaborCost();
            updateMaterialCost();
            MetalTotals.MaterialExtTotal = TotalMaterialCost;
            MetalTotals.LaborExtTotal = TotalLaborCost;
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

        private double getUnitPrice(int unit)
        {
            double val = 0;
            if (unit==0)
            {
                double.TryParse(metalDetails[19][3].ToString(), out val);
            }
            else
                double.TryParse(metalDetails[20][3].ToString(), out val);

            return val;
        }
        private void updateMaterialCost()
        {
            double stairCost = 0;
            double nl = Nails / 100;
            IEnumerable<Metal> stairMetals = Metals.Where(x => x.IsStairMetal == false && x.Name.Contains("STAIR"));
            if (stairMetals != null)
            {
                stairCost = stairMetals.Select(x => x.MaterialExtension).Sum();
            }
            if (Metals.Count>0 && MiscMetals.Count>0)
            {
                double misSum = Metals.Select(x => x.MaterialExtension).Sum() * nl;
                TotalMaterialCost = Math.Round(((Metals.Select(x => x.MaterialExtension).Sum()) +
                MiscMetals.Select(x => x.MaterialExtension).Sum() - stairCost ) + misSum,2); 
            }
            
        }
    
        private void updateLaborCost()
        {
           
            double stairCost = 0;
            IEnumerable<Metal> stairMetals = Metals.Where(x => x.IsStairMetal == false && x.Name.Contains("STAIR"));
            if (stairMetals != null)
            {
                stairCost = stairMetals.Select(x => x.LaborExtension).Sum();
            }
            ////Calculate Labor Cost
            double misSum = MiscMetals.Select(x => x.LaborExtension).Sum();
            misSum = (Metals.Select(x => x.LaborExtension).Sum() +
            MiscMetals.Select(x => x.LaborExtension).Sum() - stairCost);
                        
            if (isPrevailingWage  )
            {
                TotalLaborCost = Math.Round(misSum * (1 + prevailingWage + deductionOnLargeJob),2);
            }
            else
                TotalLaborCost = Math.Round(misSum * (1 + deductionOnLargeJob),2);

            if (!isDiscount && !isPrevailingWage)
            {
                if (Metals.Count > 0 && MiscMetals.Count > 0)
                {
                    TotalLaborCost = Math.Round(misSum,2);
                }
            }
        }
    }
}
