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
    class ALXMaterialViewModel: MaterialBaseViewModel
    {

        private Dictionary<string, string> materialNames;
        public ALXMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) :base(metalTotals,slopeTotals,Js)
        {
            materialNames = new Dictionary<string, string>();
            
            FillMaterialList();

            FetchMaterialValuesAsync(false);

        }
        
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Stucco Material Remove and replace (LF)" || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    item.Name == "Extra stair nosing lf")
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

            #region  Update Special Material Pricing and QTY
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

                    //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);

                    if (SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)" || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    SystemMaterials[i].Name == "Extra stair nosing lf")
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
            //setCheckBoxes();
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
            //CalculateLaborMinCharge(false);
            //CalculateAllMaterial();
            CalculateCost(null);
        }
        public void FillMaterialList()
        {
            materialNames.Add("Sand, Hand wash, or Pressure wash to prepare area","SQ FT");
            materialNames.Add("Sheet Membrane; 6\" WP-40 for plywood seams only", "6\" x 75' ROLL");
            materialNames.Add("Sheet Membrane; WP-40 for entire deck (10-YEAR MANUFACTURER WARRANTY REQ)", "36\" x 75' ROLL");

            materialNames.Add("2.5 LB Galvanized Metal Lathe", "18 SQFT");
            materialNames.Add("Staples (1\" Crown x 3 / 4 long Box of 13, 500 qty)", "BOX");
            materialNames.Add("Base Coat:  Grey TC-1 Base Coat Cement", "#50 BAG");
            materialNames.Add("Slurry Coat:  Grey TC-1 Base Coat Cement", "#50 BAG");
            materialNames.Add("KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down)", "#50 BAG");
            materialNames.Add("Slurry Coat Grout Coat (Pattern): Grey TC-5 Grout Texture Cement", "#50 BAG");
            materialNames.Add("Grout and Texture Color Option: TC-40 Liquid Colorant", "10 OZ TUBE");
            materialNames.Add("Pattern:  1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor", "LF");
            materialNames.Add("Skip Trowel Texture:  White TC-2 Smooth Texture Cement", "#50 BAG");

            materialNames.Add("KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down) 1", "#50 BAG");
            materialNames.Add("Water Base Stain in Lieu of Liquid Colorant:  SC-35X", "1 GAL");
            materialNames.Add("Top Coat:  SC-70 clear acrylic lacquer", "5 GAL PAIL");
            materialNames.Add("Top Coat:  SC-10 Acrylic Top Coat", "5 GAL PAIL");
            materialNames.Add("WP-81 Liquid", "5 GAL PAIL");
            materialNames.Add("Stair Nosing", "LF");

            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");

        }

        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Sand, Hand wash, or Pressure wash to prepare area":
                case "Sheet Membrane; 6\" WP-40 for plywood seams only":
                case "Slurry Coat Grout Coat (Pattern): Grey TC-5 Grout Texture Cement":
                case "Grout and Texture Color Option: TC-40 Liquid Colorant":
                case "Pattern:  1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor":
                case "Skip Trowel Texture:  White TC-2 Smooth Texture Cement":
                //case "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down)":
                case "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down) 1":
                case "Water Base Stain in Lieu of Liquid Colorant:  SC-35X":
                case "Top Coat:  SC-70 clear acrylic lacquer":
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                
                    return false;
                case "Stair Nosing":
                    return riserCount > 0;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "Sheet Membrane; WP-40 for entire deck (10-YEAR MANUFACTURER WARRANTY REQ)":
                case "Slurry Coat Grout Coat (Pattern): Grey TC-5 Grout Texture Cement":
                case "Grout and Texture Color Option: TC-40 Liquid Colorant":
                case "Skip Trowel Texture:  White TC-2 Smooth Texture Cement":
                //case "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down)":
                case "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down) 1":
                case "Water Base Stain in Lieu of Liquid Colorant:  SC-35X":
                case "Top Coat:  SC-70 clear acrylic lacquer":
                case "Stair Nosing":
                    //case "Extra stair nosing lf":
                    return true;
                default:
                    return false;
            }
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            lastCheckedMat = obj.ToString();
            if (obj.ToString()== "Sheet Membrane; WP-40 for entire deck (10-YEAR MANUFACTURER WARRANTY REQ)")
            {
                SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == "Sheet Membrane; WP-40 for entire deck (10-YEAR MANUFACTURER WARRANTY REQ)").FirstOrDefault();
                if (sysMat != null)
                {
                    SystemMaterials.Where(x => x.Name == "Sheet Membrane; 6\" WP-40 for plywood seams only").FirstOrDefault().IsMaterialChecked
                        = !sysMat.IsMaterialChecked;
                }
            }

            if (obj.ToString() == "Slurry Coat Grout Coat (Pattern): Grey TC-5 Grout Texture Cement" ||
                obj.ToString() == "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down) 1"||
                obj.ToString()== "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down)")
            {
                SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == "Slurry Coat Grout Coat (Pattern): Grey TC-5 Grout Texture Cement").FirstOrDefault();
                SystemMaterial sysMat1= SystemMaterials.Where(x => x.Name == "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down) 1").FirstOrDefault();
                if (sysMat != null && sysMat1!=null)
                {
                    SystemMaterial sysMat2 = SystemMaterials.Where(x => x.Name == "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down)").FirstOrDefault();
                    if (sysMat.IsMaterialChecked|| sysMat1.IsMaterialChecked)
                    {
                        sysMat2.IsMaterialChecked  = false;
                    }
                    else
                        sysMat2.IsMaterialChecked = true;

                }
                SystemMaterials.Where(x => x.Name == "Pattern:  1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor").FirstOrDefault().IsMaterialChecked = sysMat.IsMaterialChecked;
            }
            if (obj.ToString()== "Grout and Texture Color Option: TC-40 Liquid Colorant" || obj.ToString()== "Water Base Stain in Lieu of Liquid Colorant:  SC-35X" ||
                obj.ToString()== "Top Coat:  SC-70 clear acrylic lacquer")
            {
                SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == "Grout and Texture Color Option: TC-40 Liquid Colorant").FirstOrDefault();
                SystemMaterial sysMat1 = SystemMaterials.Where(x => x.Name == "Water Base Stain in Lieu of Liquid Colorant:  SC-35X").FirstOrDefault();
                SystemMaterial sysMat2 = SystemMaterials.Where(x => x.Name == "Top Coat:  SC-70 clear acrylic lacquer").FirstOrDefault();
                SystemMaterial sysMat3 = SystemMaterials.Where(x => x.Name == "Top Coat:  SC-10 Acrylic Top Coat").FirstOrDefault();

                if (sysMat.IsMaterialChecked || sysMat1.IsMaterialChecked || sysMat2.IsMaterialChecked)
                {
                    sysMat3.IsMaterialChecked = false;
                }
                else
                    sysMat3.IsMaterialChecked = true;

            }
            calculateRLqty();
        }
        public override void calculateLaborHrs()
        {
            calLaborHrs(6, totalSqft);
        }
        public override bool canApply(object obj)
        {
            return true;
        }
        
        public override void calculateRLqty()
        {
            #region Grout and Texture Color Option: TC-40 Liquid Colorant
            double val1 =0, val2=0;
            SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == "Slurry Coat Grout Coat (Pattern): Grey TC-5 Grout Texture Cement").FirstOrDefault();
            if (sysMat != null)
            {
                if (sysMat.IsMaterialChecked)
                {
                    val1 = sysMat.Qty;
                }
            }
            sysMat = SystemMaterials.Where(x => x.Name == "Skip Trowel Texture:  White TC-2 Smooth Texture Cement").FirstOrDefault();
            if (sysMat != null)
            {
                if (sysMat.IsMaterialChecked)
                {
                    val2 = sysMat.Qty;
                }
            }
            sysMat = SystemMaterials.Where(x => x.Name == "Grout and Texture Color Option: TC-40 Liquid Colorant").FirstOrDefault();
            bool ischecked = sysMat.IsMaterialChecked;
            sysMat.Qty = val1 + val2;
            sysMat.IsMaterialChecked = ischecked;

            #endregion

            #region WP-81 Liquid
            double val3 = 0, val4 = 0, val5 = 0, val6 = 0;
            sysMat = SystemMaterials.Where(x => x.Name == "Base Coat:  Grey TC-1 Base Coat Cement").FirstOrDefault();
            if (sysMat!=null)
            {
                val1 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "Slurry Coat:  Grey TC-1 Base Coat Cement").FirstOrDefault();
            if (sysMat != null)
            {
                val2 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }

            sysMat = SystemMaterials.Where(x => x.Name == "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down)").FirstOrDefault();
            if (sysMat != null)
            {
                val3 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "Slurry Coat Grout Coat (Pattern): Grey TC-5 Grout Texture Cement").FirstOrDefault();
            if (sysMat != null)
            {
                val4 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "Skip Trowel Texture:  White TC-2 Smooth Texture Cement").FirstOrDefault();
            if (sysMat != null)
            {
                val5 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "KD Texture Coat: Grey TC-3 Texture Coat Cement (Semi-Smooth or Knock Down) 1").FirstOrDefault();
            if (sysMat != null)
            {
                val6 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "WP-81 Liquid").FirstOrDefault();
            if (sysMat!=null)
            {
                sysMat.Qty=((val1*1.25) +val2+val3+val4+val5+val6)/ 5;
            }
            #endregion
            //CalculateLaborMinCharge(false);

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "Pattern:  1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor":
                    return (totalSqft + stairWidth * riserCount * 2)*2;
                case "Stair Nosing":
                    return riserCount * stairWidth;
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                default:
                    return totalSqft+stairWidth*riserCount*2;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "Sheet Membrane; 6\" WP-40 for plywood seams only":
                    return totalSqft / 32 * 12;
                case "Staples (1\" Crown x 3/4 long Box of 13,500 qty)":
                case "Grout and Texture Color Option: TC-40 Liquid Colorant":
                    return 0.0000001;
                default:
                    return totalSqft;
            }
        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "Staples (1\" Crown x 3/4 long Box of 13,500 qty)":
                case "Grout and Texture Color Option: TC-40 Liquid Colorant":
                case "Pattern:  1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "WP-81 Liquid":
                    return 0;
                case "Stair Nosing":               
                    return riserCount * stairWidth;
                default:
                    return riserCount*stairWidth*2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "Sheet Membrane; 6\" WP-40 for plywood seams only":
                    return lfArea / 3 / coverage * 0.9;
                case "Sheet Membrane; WP-40 for entire deck (10-YEAR MANUFACTURER WARRANTY REQ)":
                    return (totalSqft+(riserCount*8))/ ( 225 * 0.9);
                default:
                    return lfArea/coverage;
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
    }
}
