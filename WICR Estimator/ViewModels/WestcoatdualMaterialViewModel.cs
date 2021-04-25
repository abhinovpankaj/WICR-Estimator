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
    class WestcoatdualMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;

        public WestcoatdualMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();

            FillMaterialList();

            FetchMaterialValuesAsync(false);

        }

        private void FillMaterialList()
        {
            materialNames.Add("Sheet Membrane; WP-40 for entire deck (10-YEAR MANUFACTURER WARRANTY REQ)", "36\" x 75' ROLL");
            materialNames.Add("2.5 LB Galvanized Metal Lathe", "18 SQ FT");
            materialNames.Add("Staples (1\" Crown x 3 / 4 long 13, 500 qty)", "BOX");
            materialNames.Add("Base Coat:  (1 bag, Gray) TC-1 Base Coat Cement with 1-1/4 Gal WP-81", "MIX");
            materialNames.Add("Slurry Coat: (1 bag, Gray) TC-1 Base Coat Cement with 1 Gal WP-81, 1/2 Gal Water", "MIX");
            materialNames.Add("KD Texture Coat: (1 bag, Grey) TC-3 Texture Coat Cement with 1 Gal WP-81, 1/2 Gal Water", "1.5  GAL KIT");
            materialNames.Add("Top Coat:  SC-10 Acrylic Top Coat", "5 GAL PAIL");
            materialNames.Add("WP-81 Liquid", "5 GAL PAIL");
            materialNames.Add("Westcoat WP-47 Fiberlathe", "475 SQ FT RL");
            materialNames.Add("Westcoat WP-90 Resin", "5 GAL PAIL");
            materialNames.Add("Slurry Coat: (1 bag, White) TC-1 Base Coat Cement with 1 Gal WP-81, 1/2 Gal Water", "MIX");
            
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");



        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
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
                    UpdateMe(sysMat[i]);

                    SystemMaterials[i].UpdateSpecialPricing(sp);

                    if (iscbEnabled)
                    {
                        //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                        //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                        SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);
                    }
                    
                    if (SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
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
            CalculateCost(null);
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
        }


        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "KD Texture Coat: (1 bag, Grey) TC-3 Texture Coat Cement with 1 Gal WP-81, 1/2 Gal Water":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "Slurry Coat: (1 bag, Gray) TC-1 Base Coat Cement with 1 Gal WP-81, 1/2 Gal Water":
                case "Top Coat:  SC-10 Acrylic Top Coat":
                    return false;

                default:
                    return true;
            }
        }
        
        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "KD Texture Coat: (1 bag, Grey) TC-3 Texture Coat Cement with 1 Gal WP-81, 1/2 Gal Water":
                case "Top Coat:  SC-10 Acrylic Top Coat":
                case "Westcoat WP-47 Fiberlathe":
                    return true;
                default:
                    return false;
            }

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                default:
                    return totalSqft + riserCount * stairWidth * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "Sheet Membrane; WP-40 for entire deck (10-YEAR MANUFACTURER WARRANTY REQ)":
                    return (totalSqft + riserCount * 8) /( 225 * 0.9);
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                default:
                    return coverage==0?0:lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                
                case "Staples (1\" Crown x 3 / 4 long 13,500 qty)":
                    return 0;
                
                default:
                    return totalSqft;
            }

        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {

                case "Staples (1\" Crown x 3 / 4 long 13,500 qty)":
                //case "Base Coat:  (1 bag, Gray) TC-1 Base Coat Cement with 1-1/4 Gal WP-81":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "Westcoat WP-90 Resin":
                    return 0;
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
            //base.calculateRLqty();
            double val1 = 0, val2 = 0, val3 = 0, val4 = 0;
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "Base Coat:  (1 bag, Gray) TC-1 Base Coat Cement with 1-1/4 Gal WP-81").FirstOrDefault();
            if (sysmat != null)
            {
                val1 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "Slurry Coat: (1 bag, Gray) TC-1 Base Coat Cement with 1 Gal WP-81, 1/2 Gal Water").FirstOrDefault();
            if (sysmat != null)
            {
                val2 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }

            sysmat = SystemMaterials.Where(x => x.Name == "KD Texture Coat: (1 bag, Grey) TC-3 Texture Coat Cement with 1 Gal WP-81, 1/2 Gal Water").FirstOrDefault();
            if (sysmat != null)
            {
                val3 = sysmat.IsMaterialChecked ? sysmat.Qty  : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "Slurry Coat: (1 bag, White) TC-1 Base Coat Cement with 1 Gal WP-81, 1/2 Gal Water").FirstOrDefault();
            if (sysmat != null)
            {

                val4 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "WP-81 Liquid").FirstOrDefault();
            if (sysmat != null)
            {
                bool ischecked;
                ischecked = sysmat.IsMaterialChecked;
                sysmat.Qty = val1*0.25 +( val2 + val3 + val4) / 5;
                sysmat.IsMaterialChecked = ischecked;
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

                SystemMaterial item = SystemMaterials.FirstOrDefault(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)");
                if (item != null)
                {
                    item.SMSqftH = item.Qty * 32;
                    
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.MaterialExtension = item.SpecialMaterialPricing == 0 ? item.Qty * item.MaterialPrice : item.Qty * item.SpecialMaterialPricing;
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
            }
            calculateRLqty();
            //CalculateLaborMinCharge(false);
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
            bool isChecked;
            if (obj.ToString() == "Westcoat WP-47 Fiberlathe")
            {
                isChecked = SystemMaterials.Where(x => x.Name == "Westcoat WP-47 Fiberlathe").FirstOrDefault().IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "Westcoat WP-90 Resin").FirstOrDefault().IsMaterialChecked = isChecked;
                SystemMaterials.Where(x => x.Name == "Slurry Coat: (1 bag, White) TC-1 Base Coat Cement with 1 Gal WP-81, 1/2 Gal Water").FirstOrDefault().IsMaterialChecked = isChecked;
                SystemMaterials.Where(x => x.Name == "Slurry Coat: (1 bag, Gray) TC-1 Base Coat Cement with 1 Gal WP-81, 1/2 Gal Water").FirstOrDefault().IsMaterialChecked = !isChecked;
            }
            
            calculateRLqty();
            //CalculateLaborMinCharge(false);
        }

        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "Westcoat WP-47 Fiberlathe":
                    return deckPerimeter;
                default:
                    return 0;
            }
        }
        public override void setCheckBoxes()
        {
            ApplyCheckUnchecks("Westcoat WP-47 Fiberlathe");
        }
    }
}
