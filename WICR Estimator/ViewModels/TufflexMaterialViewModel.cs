using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    class TufflexMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double TotalSqftPlywood = 0;
        
        private bool? IsNewPlaywood;
        
        public TufflexMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            TotalSqftPlywood = Js.TotalSqftPlywood;
            
            IsNewPlaywood = Js.IsNewPlywood;
            FillMaterialList();

            FetchMaterialValuesAsync(false);

        }

        public override void FetchMaterialValuesAsync(bool v)
        {
            base.FetchMaterialValuesAsync(v);
        }

        private void FillMaterialList()
        {
            materialNames.Add("Prep to remove existing urethane", "");
            materialNames.Add("Tuff Poxy #3 Primer", "3gal kit");
            materialNames.Add("2.5 Galvanized Metal Lathe (18 sq ft)", "17 SQ FT SHT.");
            materialNames.Add("Staples (3/4\" crown, Box of 13,500)", "5 GAL");
            materialNames.Add("Tufflex Solvent Free \"Tuff\" Base Coat", "5 GAL");
            materialNames.Add("1/20 Mesh Sand", "#50 BAG");
            materialNames.Add("Tufflex Solvent Free \"Tuff\" Intermediate Coat", "150 SF ROLL");
            materialNames.Add("1/20 Mesh Sand Broadcast to Refusal", "#50 BAG");
            materialNames.Add("Elasta-Tuff #6000-AL-SC Top Coat", "5 GAL PAIL");
            materialNames.Add("Stair Nosing", "LF");
            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");
        }

        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js != null)
            {
                TotalSqftPlywood = Js.TotalSqftPlywood;                
                IsNewPlaywood = Js.IsNewPlywood;
                               
                SystemMaterials.Where(x => x.Name == "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)").
                    FirstOrDefault().SMUnits = Js.RiserCount.ToString();
                //SystemMaterials.Where(x => x.Name == "Striping for small cracKs (less than 1/8\")").
                //FirstOrDefault().SMUnits = Js.TotalSqft.ToString();

            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "Prep to remove existing urethane":
                case "Tuff Poxy #3 Primer":
                    return false;
                case "Stair Nosing":
                    return true;
                default:
                    return riserCount+TotalSqftPlywood>0?true:false;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "Prep to remove existing urethane":
                case "Tuff Poxy #3 Primer":
                    return true;
                default:
                    return false;
            }

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Stair Nosing":
                    return riserCount ;
                
                default:
                    return TotalSqftPlywood + riserCount * stairWidth * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Stair Nosing":
                    return riserCount*stairWidth;
                default:
                    return lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "Stair Nosing":
                case "Extra stair nosing lf":
                case "Staples(3/4\" crown, Box of 13,500)":
                case "1/20 Mesh Sand":
                case "1/20 Mesh Sand Broadcast to Refusal":
                    return 0;
                default:
                    return TotalSqftPlywood;
            }

        }
        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "Staples(3/4\" crown, Box of 13,500)":
                case "1/20 Mesh Sand":
                case "1/20 Mesh Sand Broadcast to Refusal":
                case "Elasta-Tuff #6000-AL-SC Top Coat":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Stair Nosing":
                    return riserCount * stairWidth;
                default:
                    return riserCount * stairWidth * 2;
            }
        }

        public override void calculateLaborHrs()
        {
            calLaborHrs(10, totalSqft);

        }
        //calculate for Desert Crete
        public override void calculateRLqty()
        {            

        }

        public override bool canApply(object obj)
        {
            return true;
        }
        public override void setExceptionValues(object s)
        {
            if (SystemMaterials.Count != 0)
            {

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "Extra stair nosing lf").FirstOrDefault();
                if (item != null)
                {

                    item.StairSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                    
                }

                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty * 32;
                    //item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.MaterialExtension = item.SpecialMaterialPricing == 0 ? item.Qty * item.MaterialPrice : item.Qty * item.SpecialMaterialPricing;
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
            }
            CalculateLaborMinCharge();
        }
        public override bool IncludedInLaborMin(string matName)
        {
            switch (matName)
            {
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return false;
                default:
                    return true;
            }
        }
        public override void ApplyCheckUnchecks(object obj)
        {
            
            calculateRLqty();
            CalculateLaborMinCharge();
        }

        public override void setCheckBoxes()
        {

        }
    }
}
