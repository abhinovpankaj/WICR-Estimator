using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    [DataContract]
    public class DesertbrandMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private bool? IsJobSpecifiedByArchitect;
        private bool IsSystemOverConcrete;
        public DesertbrandMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            IsJobSpecifiedByArchitect = Js.IsJobSpecifiedByArchitect;
            FillMaterialList();
            
            FetchMaterialValuesAsync(false);

        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js!=null)
            {
                IsJobSpecifiedByArchitect = Js.IsJobSpecifiedByArchitect;
                IsSystemOverConcrete = Js.IsSystemOverConcrete;
            }
            
            base.JobSetup_OnJobSetupChange(sender, e);
        }

        
        private void FillMaterialList()
        {
            materialNames.Add("2.5 Galvanized Lathe (18 s.f.)", "EA");
            materialNames.Add("Staples", "BOX");
            materialNames.Add("Bonder 480", "5 GAL PAIL");
            materialNames.Add("3/4 oz.Fiberglass Matt", "2000 SQFT ROLL");
            materialNames.Add("BASE COAT 50 lb Desert Crete Level Max 20/30", "50 LB BAG");
            materialNames.Add("BASE COAT Desert Crete poly base mixed with water", "50 LB BAG");
            materialNames.Add("SKIM COAT Desert Crete poly Base Underlayment mixed with water", "50 LB BAG");
            materialNames.Add("TEXTURE Desert Crete poly base texture mixed Polymer #550 (1-1/4GAL per BAG)", "50 LB BAG");
            materialNames.Add("Desert Crete Liquid Polymer #550 mixed 50/50 with water", "5 GAL PAIL");
            materialNames.Add("Concrete Masonry Floor paint", "5 GAL PAIL");
            materialNames.Add("Liquid Polymer primer 50/50 with water", "5 GAL PAIL");
            materialNames.Add("Stair Nosing", "LF");
            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");
        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            //base.FetchMaterialValuesAsync(hasSetupChanged);
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Extra stair nosing lf" || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    item.Name == "Stucco Material Remove and replace (LF)")
                {
                    qtyList.Add(item.Name, item.Qty);
                }

            }
            if (materialNames == null)
            {
                materialNames = new Dictionary<string, string>();
                FillMaterialList();
            }
            var sysMat = GetSystemMaterial(materialNames);

            #region  Update Special Material Pricing and QTY on JobSetup change
            if (hasSetupChanged)
            {
                for (int i = 0; i < SystemMaterials.Count; i++)
                {

                    double sp = SystemMaterials[i].SpecialMaterialPricing;
                    bool iscbChecked = SystemMaterials[i].IsMaterialChecked;
                    bool iscbEnabled = SystemMaterials[i].IsMaterialEnabled;
                    //SystemMaterials[i] = sysMat[i];

                    //SystemMaterials[i].SpecialMaterialPricing = sp;
                    //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    

                    UpdateMe(sysMat[i]);

                    SystemMaterials[i].UpdateSpecialPricing(sp);
                    SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);

                    if (SystemMaterials[i].Name == "Extra stair nosing lf" || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                        SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            //SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];
                            SystemMaterials[i].UpdateQuantity(qtyList[SystemMaterials[i].Name]);
                        }
                    }

                }

            }
            #endregion

            else
                SystemMaterials = sysMat;

            setExceptionValues(null);
            if (hasSetupChanged)
            {
                setCheckBoxes();
            }
            

            if (OtherMaterials.Count == 0)
            {
                OtherMaterials = GetOtherMaterials();
                OtherLaborMaterials = OtherMaterials;
            }


            if (SubContractLaborItems.Count == 0)
            {
                SubContractLaborItems = GetLaborItems();
            }
            calculateRLqty();
            CalculateCost(null);
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
        }
        
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return false;
                case "BASE COAT 50 lb Desert Crete Level Max 20/30":
                    return IsSystemOverConcrete == true ? !IsSystemOverConcrete : (bool)IsJobSpecifiedByArchitect;
                case "BASE COAT Desert Crete poly base mixed with water":
                    return IsSystemOverConcrete == true ? !IsSystemOverConcrete : (bool)!IsJobSpecifiedByArchitect;
                case "2.5 Galvanized Lathe (18 s.f.)":
                case "Staples":
                    return !IsSystemOverConcrete;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                //case "Staples":
                //case "BASE COAT 50 lb Desert Crete Level Max 20/30":
                case "BASE COAT Desert Crete poly base mixed with water":
                case "2.5 Galvanized Lathe (18 s.f.)":
                     return true;
                default:
                    return false;
            }
            
        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "Desert Crete Liquid Polymer #550 mixed 50/50 with water":
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Stair Nosing":
                    return riserCount * 4;
                default:
                    return totalSqft + riserCount * stairWidth * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "Desert Crete Liquid Polymer #550 mixed 50/50 with water":
                    return 0;
                case "Concrete Masonry Floor paint":
                    return lfArea / coverage < 0.5 ? 0.5 : lfArea / coverage;
                case "Liquid Polymer primer 50/50 with water":
                    return (lfArea / coverage) / 2;
                default:
                    return lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "Staples":
                case "Stair Nosing":
                case "Extra stair nosing lf":
                    return 0.0000001;
                default:
                    return totalSqft;
            }
            
        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {

                case "Concrete Masonry Floor paint":
                    return riserCount*stairWidth*2;
                case "Stair Nosing":
                    return riserCount * stairWidth;
                default:
                    return 0.0000001;

            }
        }

        public override void calculateLaborHrs()
        {
            calLaborHrs(6,totalSqft);

        }
        //calculate for Desert Crete
        public override void calculateRLqty()
        {
            //base.calculateRLqty();
            double val1 = 0, val2 = 0;
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "BASE COAT 50 lb Desert Crete Level Max 20/30").FirstOrDefault();
            if (sysmat != null)
            {
                val1 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "TEXTURE Desert Crete poly base texture mixed Polymer #550 (1-1/4GAL per BAG)").FirstOrDefault();
            if (sysmat != null)
            {
                val2 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }

            sysmat = SystemMaterials.Where(x => x.Name == "Desert Crete Liquid Polymer #550 mixed 50/50 with water").FirstOrDefault();
            if (sysmat != null)
            {
                double calVal = ((0.31 * val2) + val1 / 2.5 / 2);
                if (IsJobSpecifiedByArchitect!=null)
                {
                    sysmat.Qty = (bool)IsJobSpecifiedByArchitect ? 0.31 * val2 : calVal;
                }
                
            }
            //CalculateLaborMinCharge(false);
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

                    item.SMSqft = item.Qty;
                    item.StairSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }
               
                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty * 32;
                    item.SMSqft = item.Qty;
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

                item = SystemMaterials.Where(x => x.Name == "3/4 oz.Fiberglass Matt").FirstOrDefault();
                if (item != null)
                {
                    
                    item.VerticalSqft = deckPerimeter;
                    item.VerticalProductionRate = 100*(1+prPerc);
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate)+item.VerticalSqft/item.VerticalProductionRate;

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                }
                calculateRLqty();
                //CalculateLaborMinCharge(false);
            }
                
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
            if (obj.ToString() == "2.5 Galvanized Lathe (18 s.f.)")
            {
                bool isChecked = SystemMaterials.Where(x => x.Name == "2.5 Galvanized Lathe (18 s.f.)").FirstOrDefault().IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "Staples").FirstOrDefault().IsMaterialChecked = isChecked;
            }
            calculateRLqty();
            //CalculateLaborMinCharge(false);
        }

        public override void setCheckBoxes()
        {
            //base.setCheckBoxes();
            bool isSpecified = false;
            if (IsJobSpecifiedByArchitect!=null)
            {
                isSpecified = (bool)IsJobSpecifiedByArchitect;
            }
            
            
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "BASE COAT Desert Crete poly base mixed with water").FirstOrDefault();
            sysmat.IsMaterialChecked = isSpecified;
            sysmat.IsMaterialEnabled = isSpecified;

            sysmat = SystemMaterials.Where(x => x.Name == "BASE COAT 50 lb Desert Crete Level Max 20/30").FirstOrDefault();
            sysmat.IsMaterialChecked = !isSpecified;
            sysmat.IsMaterialEnabled = !isSpecified;
            
            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "2.5 Galvanized Lathe (18 s.f.)" || item.Name == "Staples")
                {
                    item.IsMaterialChecked = getCheckboxCheckStatus(item.Name);
                }
            }
        }
    }
}
