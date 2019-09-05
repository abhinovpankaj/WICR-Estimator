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
    class MulticoatMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double linearFootageCoping;
        public MulticoatMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            linearFootageCoping = Js.LinearCopingFootage;
            FillMaterialList();
            
            FetchMaterialValuesAsync(false);

        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                linearFootageCoping = js.LinearCopingFootage;
            }

            base.JobSetup_OnJobSetupChange(sender, e);
        }
        private void FillMaterialList()
        {
            materialNames.Add("Pressure Aggressively to wash to clean existing concrete", "SQ FT");
            materialNames.Add("Light crack and repairs- speed bond (no more than 1% of area)", "65 LB BAG");
            materialNames.Add("Slurry Coat for repairs", "55 LB BAG");
            materialNames.Add("Caulk 1/2 to 3/4 inch control joints (SIKA 2C)", "LF");
            materialNames.Add("Remove and Replace Expansion joints- backer rod and sealant (SIKA 2C)", "LF");
            materialNames.Add("Large cracks with reseal (route, fill with speed bond/sand and spot texture)", "LF");
            materialNames.Add("Large cracks with new system (route, fill with speed bond)", "SQ FT");
            materialNames.Add("Bubbled and failed textured areas (prep, patch,spot textured with resistite)", "SQ FT");
           
            materialNames.Add("Texture for repairs", "55 LB BAG");
            materialNames.Add("Acrathane premium sealer for walking or driving surfaces", "5 GAL PAIL");
            
            //materialNames.Add("1st coat thin 30% with water, 2nd coat full strength when dry", "LF");
            materialNames.Add("Add for masking and spraying coping from the water", "LF");
            materialNames.Add("Add for saw cut joints", "LF");
            materialNames.Add("Add for removing and replacing concrete (no more than 100 sq ft)", "SQ FT");
            materialNames.Add("DETAIL PERIMETER - RP FABRIC 10\" x 300' (FROM ACME BAG)", "ROLL");
            materialNames.Add("DETAIL PERIMETER - CPC MEMBRANE", "5 GAL PAIL");
        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            //base.FetchMaterialValuesAsync(hasSetupChanged);
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Caulk 1/2 to 3/4 inch control joints (SIKA 2C)" || item.Name == "Remove and Replace Expansion joints- backer rod and sealant (SIKA 2C)" ||
                    item.Name == "Large cracks with reseal (route, fill with speed bond/sand and spot texture)" ||
                    item.Name == "Large cracks with new system (route, fill with speed bond)" ||
                    item.Name == "Bubbled and failed textured areas (prep, patch,spot textured with resistite)" ||
                    item.Name == "Add for saw cut joints" || item.Name == "Add for removing and replacing concrete (no more than 100 sq ft)")
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
                    SystemMaterials[i] = sysMat[i];

                    SystemMaterials[i].SpecialMaterialPricing = sp;
                    SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    if (SystemMaterials[i].Name == "Caulk 1/2 to 3/4 inch control joints (SIKA 2C)" ||
                        SystemMaterials[i].Name == "Remove and Replace Expansion joints- backer rod and sealant (SIKA 2C)" ||
                        SystemMaterials[i].Name == "Large cracks with reseal (route, fill with speed bond/sand and spot texture)" ||
                        SystemMaterials[i].Name == "Large cracks with new system (route, fill with speed bond)" ||
                        SystemMaterials[i].Name == "Bubbled and failed textured areas (prep, patch,spot textured with resistite)" ||
                        SystemMaterials[i].Name == "Add for saw cut joints" ||
                        SystemMaterials[i].Name == "Add for removing and replacing concrete (no more than 100 sq ft)")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];
                        }
                    }

                }

            }
            #endregion

            else
                SystemMaterials = sysMat;

            setExceptionValues(null);
            setCheckBoxes();

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
            CalculateLaborMinCharge();
            CalculateAllMaterial();
        }

        public override void setCheckBoxes()
        {
            SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == "Add for masking and spraying coping from the water").FirstOrDefault();
            if (sysMat != null)
            {
                sysMat.IsMaterialChecked = linearFootageCoping > 0 ? true : false;
            }
            ApplyCheckUnchecks("Slurry coat over texture (Krete Kote 120 sq ft per mix)");
            sysMat = SystemMaterials.Where(x => x.Name == "DETAIL PERIMETER - RP FABRIC 10\" x 300' (FROM ACME BAG)").FirstOrDefault();
            if (sysMat != null)
            {
                sysMat.IsMaterialChecked = deckPerimeter > 0 ? true : false;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "DETAIL PERIMETER - CPC MEMBRANE").FirstOrDefault();
            if (sysMat != null)
            {
                sysMat.IsMaterialChecked = deckPerimeter > 0 ? true : false;
            }

        }
        public override void calculateRLqty()
        {
            
            SystemMaterial sysMat1, sysMat2;
            if (SystemMaterials.Count > 0)
            {
                sysMat1 = SystemMaterials.Where(x => x.Name == "Slurry Coat for repairs").FirstOrDefault();
                if (sysMat1 == null)
                {
                    sysMat1 = SystemMaterials.Where(x => x.Name == "Slurry coat over texture (Krete Kote 120 sq ft per mix)").FirstOrDefault();
                }
                sysMat2 = SystemMaterials.Where(x => x.Name == "Texture for repairs").FirstOrDefault();
                if (sysMat2 == null)
                {
                    sysMat2 = SystemMaterials.Where(x => x.Name == "Krete Kote or Top Cote texture").FirstOrDefault();
                }
                bool ischecked = sysMat1.IsMaterialChecked;
                sysMat1.Qty = sysMat1.Coverage == 0 ? 0 : sysMat1.SMSqft / sysMat1.Coverage;
                sysMat1.IsMaterialChecked = ischecked;
                ischecked = sysMat2.IsMaterialChecked;
                sysMat2.Qty = sysMat2.Coverage == 0 ? 0 : sysMat2.SMSqft / sysMat2.Coverage;
                sysMat2.IsMaterialChecked = ischecked;
            }
            CalculateLaborMinCharge();

        }
        public override bool canApply(object obj)
        {
            return true;
        }

        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Pressure Aggressively to wash to clean existing concrete":
                case "Texture for repairs":
                case "Acrathane premium sealer for walking or driving surfaces":
                case "Slurry Coat for repairs":
                
                    return true;
                case "Add for masking and spraying coping from the water":
                    return linearFootageCoping > 0 ? true : false;
                case "DETAIL PERIMETER - RP FABRIC 10\" x 300' (FROM ACME BAG)":
                case "DETAIL PERIMETER - CPC MEMBRANE":
                    return deckPerimeter > 0 ? true : false;
                default:
                    return false;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "Slurry Coat for repairs":
                case "Texture for repairs":
                    return true;
                default:
                    return false;
            }

        }
        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "Pressure Aggressively to wash to clean existing concrete":
                case "RESISTITE UNIVERSAL PRIMER ADD 50% WATER)":
                case "AJ -44 Sealer":
                    return totalSqft + (riserCount * stairWidth * 2);
                case "Light crack and repairs- speed bond (no more than 1% of area)":
                    return totalSqft * 0.1;

                case "Add for masking and spraying coping from the water":
                    return linearFootageCoping;
                case "DETAIL PERIMETER - RP FABRIC 10\" x 300' (FROM ACME BAG)":
                case "DETAIL PERIMETER - CPC MEMBRANE":
                    return deckPerimeter;
                case "Add for saw cut joints":
                case "Add for removing and replacing concrete (no more than 100 sq ft)":
                    return 0;
                default:
                    return totalSqft + (riserCount * stairWidth * 2);
            }
        }
        public override void ApplyCheckUnchecks(object obj)
        {
            if (obj.ToString() == "Slurry Coat for repairs" || obj.ToString() == "Slurry coat over texture (Krete Kote 120 sq ft per mix)")
            {
                SystemMaterial sysMat1 = SystemMaterials.Where(x => x.Name == "Slurry Coat for repairs").FirstOrDefault();
                if (sysMat1 == null)
                {
                    sysMat1 = SystemMaterials.Where(x => x.Name == "Slurry coat over texture (Krete Kote 120 sq ft per mix)").FirstOrDefault();
                }
                SystemMaterials.Where(x => x.Name == "Light crack and repairs- speed bond (no more than 1% of area)").FirstOrDefault().IsMaterialChecked = !sysMat1.IsMaterialChecked;
            }
            calculateRLqty();
        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "DETAIL PERIMETER - RP FABRIC 10\" x 300' (FROM ACME BAG)":
                case "DETAIL PERIMETER - CPC MEMBRANE":
                    return 0;
                default:
                    return riserCount * stairWidth * 2;
            }

        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                
                case "Add for masking and spraying coping from the water":
                    return linearFootageCoping;
                case "DETAIL PERIMETER - RP FABRIC 10\" x 300' (FROM ACME BAG)":
                    return deckPerimeter;
                case "DETAIL PERIMETER - CPC MEMBRANE":
                    return 0;
                default:
                    return totalSqft;
            }
        }
        public override void setExceptionValues(object s)
        {
            if (s==null)
            {
                return;
            }
            if (s.ToString()== "Slurry Coat for repairs" || s.ToString()== "Slurry coat over texture (Krete Kote 120 sq ft per mix)"
                ||s.ToString()== "Texture for repairs" || s.ToString()== "Krete Kote or Top Cote texture")
            {
                return;
            }
            double val1 = 0, val2 = 0;
            if (SystemMaterials.Count > 0)
            {
                SystemMaterial sysMat1 = SystemMaterials.Where(x => x.Name == "Slurry Coat for repairs").FirstOrDefault();
                if (sysMat1 == null)
                {
                    sysMat1 = SystemMaterials.Where(x => x.Name == "Slurry coat over texture (Krete Kote 120 sq ft per mix)").FirstOrDefault();
                }
                SystemMaterial sysMat2 = SystemMaterials.Where(x => x.Name == "Texture for repairs").FirstOrDefault();
                if (sysMat2 == null)
                {
                    sysMat2 = SystemMaterials.Where(x => x.Name == "Krete Kote or Top Cote texture").FirstOrDefault();
                }

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "Caulk 1/2 to 3/4 inch control joints (SIKA 2C)").FirstOrDefault();
                if (item != null)
                {

                    item.SMSqft = item.Qty;
                    item.SMSqftH = item.Qty;
                    //item.StairSqft = item.Qty;
                    item.Coverage = item.Qty / 115;
                    item.MaterialExtension =item.MaterialPrice * item.Coverage;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }
                item = SystemMaterials.Where(x => x.Name == "Remove and Replace Expansion joints- backer rod and sealant (SIKA 2C)").FirstOrDefault();
                if (item != null)
                {

                    item.SMSqft = item.Qty;
                    //item.StairSqft = item.Qty;
                    item.SMSqftH = item.Qty;
                    item.Coverage = item.Qty / 60;
                    item.MaterialExtension = item.MaterialPrice * item.Coverage;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }
                item = SystemMaterials.Where(x => x.Name == "Add for saw cut joints").FirstOrDefault();
                if (item != null)
                {

                    item.SMSqft = item.Qty;
                    //item.StairSqft = item.Qty;
                    item.SMSqftH = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }
                item = SystemMaterials.Where(x => x.Name == "Add for removing and replacing concrete (no more than 100 sq ft)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    //item.StairSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }
                item = SystemMaterials.Where(x => x.Name == "Large cracks with reseal (route, fill with speed bond/sand and spot texture)").FirstOrDefault();
                if (item != null)
                {
                    val1 = item.Qty;
                    
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    //item.StairSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.Coverage = item.Qty / 15;
                    item.MaterialExtension = item.MaterialPrice * item.Coverage;
                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }
                item = SystemMaterials.Where(x => x.Name == "Large cracks with new system (route, fill with speed bond)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    //item.StairSqft = item.Qty;
                    item.Coverage = item.Qty / 15;
                    item.MaterialExtension = item.MaterialPrice * item.Coverage;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }
                item = SystemMaterials.Where(x => x.Name == "Bubbled and failed textured areas (prep, patch,spot textured with resistite)").FirstOrDefault();
                if (item != null)
                {
                    val2 = item.Qty;
           
                    item.SMSqft = item.Qty;
                    //item.StairSqft = item.Qty;
                    item.SMSqftH = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    
                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }
                if (val1+val2 > 0)
                {

                    sysMat1.IsMaterialChecked = true;
                    sysMat1.IsMaterialEnabled = false;
                    SystemMaterials.Where(x => x.Name == "Light crack and repairs- speed bond (no more than 1% of area)").FirstOrDefault().IsMaterialChecked
                        = false;
                    sysMat1.SMSqft = val2+val1;
                    sysMat2.IsMaterialChecked = true;
                    sysMat2.IsMaterialEnabled = false;
                    sysMat2.SMSqft = val2 + val1;
                }
                else
                {
                    if (!sysMat1.IsMaterialEnabled)
                    {
                        sysMat1.IsMaterialChecked = false;
                        sysMat1.IsMaterialEnabled = true;
                        SystemMaterials.Where(x => x.Name == "Light crack and repairs- speed bond (no more than 1% of area)").FirstOrDefault().IsMaterialChecked
                            = true;
                        sysMat1.SMSqft = totalSqft + (stairWidth * riserCount * 2);
                    }
                    if (!sysMat2.IsMaterialEnabled)
                    {
                        sysMat2.IsMaterialChecked = false;
                        sysMat2.IsMaterialEnabled = true;
                        sysMat2.SMSqft = totalSqft + (riserCount * stairWidth * 2);
                    }
                   
                }

                if (val2 > 0 || val1 > 0)
                {
                    sysMat2.Name = "Texture for repairs";
                    sysMat1.Name = "Slurry Coat for repairs";
                    
                }
                else
                {
                    sysMat2.Name = "Krete Kote or Top Cote texture";
                    sysMat1.Name = "Slurry coat over texture (Krete Kote 120 sq ft per mix)";
                    
                }
                //new updates formula
                item = SystemMaterials.Where(x => x.Name == "Caulk 1/2 to 3/4 inch control joints (SIKA 2C)").FirstOrDefault();
                if (item != null)
                {
                    item.MaterialExtension = item.SpecialMaterialPricing == 0 ? item.SMSqft * item.MaterialPrice : item.Qty * item.SpecialMaterialPricing;
                }

                item = SystemMaterials.Where(x => x.Name == "Remove and Replace Expansion joints- backer rod and sealant (SIKA 2C)").FirstOrDefault();
                if (item != null)
                {
                    item.MaterialExtension = item.SpecialMaterialPricing == 0 ? item.SMSqft * item.MaterialPrice : item.Qty * item.SpecialMaterialPricing;
                }
                calculateRLqty();
            }
            CalculateLaborMinCharge();
        }
        public override bool IncludedInLaborMin(string matName)
        {
            return true;
        }
        public override void CalculateLaborMinCharge()
        {
            //LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
            //                            x.IsMaterialChecked).ToList().Select(x => x.Hours).Sum();
            //LaborMinChargeMinSetup = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
            //                             x.IsMaterialChecked).ToList().Select(x => x.SetupMinCharge).Sum();
            //LaborMinChargeLaborExtension = LaborMinChargeMinSetup + LaborMinChargeHrs > 20 ? 0 :
            //                                    (20 - LaborMinChargeMinSetup - LaborMinChargeHrs) * laborRate;
            base.CalculateLaborMinCharge();
        }
        public override void calculateLaborHrs()
        {
            //calculateRLqty();
            calLaborHrs(4, totalSqft);
        }
        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            if (materialName== "Large cracks with reseal (route, fill with speed bond/sand and spot texture)"
                || materialName == "Large cracks with new system (route, fill with speed bond)")
            {
                return 0;
            }
            else
                return coverage == 0 ? 0 : lfArea / coverage;
            
        }
    }
}
