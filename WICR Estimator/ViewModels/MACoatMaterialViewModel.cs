using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    class MACoatMaterialViewModel:MaterialBaseViewModel
    {
        public Dictionary<string, string> materialNames;
        public MACoatMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);      
        }

        public virtual void FillMaterialList()
        {
            materialNames.Add("Sand,hand wash, or pressure wash to prepare area", "SQ FT");
            materialNames.Add("Primer if needed:  EC-11 primer", "1.5 GAL KIT");
            materialNames.Add("#30 or #60 Silica Sand (With Primer)", "#100 BAG");

            materialNames.Add("Underlay over rough surface: Grey TC-1 Cement", "#50 BAG");
            materialNames.Add("WP-47 fiber lathe (475 SQ FT ROLL)", "ROLL");
            materialNames.Add("Membrane Grey TC-1 Cement", "#50 BAG");
            materialNames.Add("Slurry Coat / Standard Semi-Smooth Texture: Grey TC-1 Cement", "#50 BAG");
            materialNames.Add("Grout Coat for Tile Pattern:  Grey TC-5 Texture Cement", "#50 BAG");
            materialNames.Add("1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor", "LF");
            materialNames.Add("Smooth Texture:  Grey TC-1 Cement", "#50 BAG");
            materialNames.Add("Knock Down Texture: Grey TC-3 Cement", "#50 BAG");
            materialNames.Add("Skip Trowel:  White TC-2 Smooth Cement", "#50 BAG");
            materialNames.Add("SC-35 Water based stain", "1GAL");
            materialNames.Add("TC-40 Liquid Colorant", "10OZ TUBE");
            materialNames.Add("SC-70 clear acrylic lacquer", "5 GAL PAIL");

            materialNames.Add("SC-10 Topcoat", "5 GAL PAIL");
            materialNames.Add("WP-81 Liquid", "5 GAL PAIL");
            materialNames.Add("WP-90 Liquid", "5 GAL PAIL");
            materialNames.Add("Stair Nosing", "LF");
            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");           
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
            var sysMat = GetSystemMaterial(materialNames);

            #region  Update Special Material Pricing and QTY
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
                    if (SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)" || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    SystemMaterials[i].Name == "Extra stair nosing lf")
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
            //setCheckBoxes();
            if (OtherMaterials.Count == 0)
            {
                OtherMaterials = GetOtherMaterials();
                OtherLaborMaterials = GetOtherMaterials();
            }
            if (SubContractLaborItems.Count == 0)
            {
                SubContractLaborItems = GetLaborItems();
            }
            calculateRLqty();
            CalculateLaborMinCharge();
            CalculateAllMaterial();
        }
        public override void CalculateLaborMinCharge()
        {
            LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
                                        x.IsMaterialChecked).ToList().Select(x => x.Hours).Sum();
            LaborMinChargeMinSetup = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
                                         x.IsMaterialChecked).ToList().Select(x => x.SetupMinCharge).Sum();

            LaborMinChargeLaborExtension =( LaborMinChargeMinSetup + LaborMinChargeHrs) > 17 ? 0 :
                                                (17 - LaborMinChargeMinSetup - LaborMinChargeHrs) * laborRate;
            base.CalculateLaborMinCharge();
        }
        public override bool IncludedInLaborMin(string matName)
        {
            switch (matName)
            {
                case "Stair Nosing":
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "SC-10 Topcoat":
                case "WP-81 Liquid":
                case "WP-90 Liquid":
                
                    return false;
                default:
                    return true;
            }
        }
        public override bool canApply(object obj)
        {
            return true;
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "Primer if needed:  EC-11 primer":
                case "Underlay over rough surface: Grey TC-1 Cement":
                case "Grout Coat for Tile Pattern:  Grey TC-5 Texture Cement":
                case "Smooth Texture:  Grey TC-1 Cement":
                case "Knock Down Texture: Grey TC-3 Cement":
                case "Skip Trowel:  White TC-2 Smooth Cement":
                case "SC-35 Water based stain":
                case "TC-40 Liquid Colorant":
                case "SC-70 clear acrylic lacquer":
                    return true;
                default:
                    return false;
            }
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Sand,hand wash, or pressure wash to prepare area":
                case "Primer if needed:  EC-11 primer":
                case "#30 or #60 Silica Sand (With Primer)":
                case "Underlay over rough surface: Grey TC-1 Cement":
                case "WP-47 fiber lathe (475 SQ FT ROLL)":
                case "Membrane Grey TC-1 Cement":
                case "Slurry Coat / Standard Semi-Smooth Texture: Grey TC-1 Cement":
                case "Knock Down Texture: Grey TC-3 Cement":
                case "WP-81 Liquid":
                case "WP-90 Liquid":
                case "SC-10 Topcoat":
                case "Stair Nosing":
                    return true;
                    
                default:
                    return false;
            }
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            if (obj.ToString()== "Primer if needed:  EC-11 primer")
            {
                SystemMaterials.Where(x => x.Name == "#30 or #60 Silica Sand (With Primer)").FirstOrDefault().IsMaterialChecked=
                    SystemMaterials.Where(x => x.Name == "Primer if needed:  EC-11 primer").FirstOrDefault().IsMaterialChecked;
            }
            if (obj.ToString() == "Membrane Grey TC-1 Cement")
            {
                SystemMaterials.Where(x => x.Name == "Slurry Coat / Standard Semi-Smooth Texture: Grey TC-1 Cement").FirstOrDefault().IsMaterialChecked =
                    SystemMaterials.Where(x => x.Name == "Membrane Grey TC-1 Cement").FirstOrDefault().IsMaterialChecked;
            }
            if (obj.ToString() == "Grout Coat for Tile Pattern:  Grey TC-5 Texture Cement")
            {
                SystemMaterials.Where(x => x.Name == "1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor").FirstOrDefault().IsMaterialChecked =
                    SystemMaterials.Where(x => x.Name == "Grout Coat for Tile Pattern:  Grey TC-5 Texture Cement").FirstOrDefault().IsMaterialChecked;
            }
            if (obj.ToString()== "SC-35 Water based stain"||obj.ToString()== "TC-40 Liquid Colorant"
                ||obj.ToString()== "SC-70 clear acrylic lacquer")
            {
                bool sc35 = SystemMaterials.Where(x => x.Name == "SC-35 Water based stain").FirstOrDefault().IsMaterialChecked;
                bool tc40 = SystemMaterials.Where(x => x.Name == "TC-40 Liquid Colorant").FirstOrDefault().IsMaterialChecked;
                bool sc70 = SystemMaterials.Where(x => x.Name == "SC-70 clear acrylic lacquer").FirstOrDefault().IsMaterialChecked;
                if (!sc35 && !tc40 &&!sc35)
                {
                    SystemMaterials.Where(x => x.Name == "SC-10 Topcoat").FirstOrDefault().IsMaterialChecked = true;
                }
                else
                    SystemMaterials.Where(x => x.Name == "SC-10 Topcoat").FirstOrDefault().IsMaterialChecked = false;
            }
            calculateRLqty();
            CalculateLaborMinCharge();
        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "Membrane Grey TC-1 Cement":
                    return totalSqft +deckPerimeter/2+ riserCount * stairWidth * 2;
                case "1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor":
                    return   (totalSqft + riserCount * stairWidth *2)* 2;
                case "Stair Nosing":
                    return riserCount * stairWidth;
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                default:
                    return (totalSqft + riserCount * stairWidth * 2);
            }
        }
        //TC-40 Liquid Colorant
        public override void calculateRLqty()
        {
            double val1=0, val2=0, val3=0,val4=0;
            SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == "Grout Coat for Tile Pattern:  Grey TC-5 Texture Cement").FirstOrDefault();
            if (sysMat.IsMaterialChecked)
            {
                val1 = sysMat.Qty;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "Smooth Texture:  Grey TC-1 Cement").FirstOrDefault();
            if (sysMat.IsMaterialChecked)
            {
                val2 = sysMat.Qty;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "Skip Trowel:  White TC-2 Smooth Cement").FirstOrDefault();
            if (sysMat.IsMaterialChecked)
            {
                val3 = sysMat.Qty;
            }
            //set qty
            sysMat = SystemMaterials.Where(x => x.Name == "TC-40 Liquid Colorant").FirstOrDefault();
            bool ischecked=sysMat.IsMaterialChecked;
            sysMat.Qty = val1 + val2 + val3;
            sysMat.IsMaterialChecked=ischecked;


            sysMat = SystemMaterials.Where(x => x.Name == "WP-81 Liquid").FirstOrDefault();
            ischecked = sysMat.IsMaterialChecked;
            sysMat.Qty =( val1 + val3)/5;
            sysMat.IsMaterialChecked = ischecked;


            sysMat = SystemMaterials.Where(x => x.Name == "Membrane Grey TC-1 Cement").FirstOrDefault();
            if (sysMat.IsMaterialChecked)
            {
                val1 = sysMat.Qty;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "Slurry Coat / Standard Semi-Smooth Texture: Grey TC-1 Cement").FirstOrDefault();
            if (sysMat.IsMaterialChecked)
            {
                val3 = sysMat.Qty;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "Knock Down Texture: Grey TC-3 Cement").FirstOrDefault();
            if (sysMat.IsMaterialChecked)
            {
                val4 = sysMat.Qty;
            }


            sysMat = SystemMaterials.Where(x => x.Name == "WP-90 Liquid").FirstOrDefault();
            ischecked = sysMat.IsMaterialChecked;
            sysMat.Qty = val1+val2+val3+val4/5;
            sysMat.IsMaterialChecked = ischecked;

        }

        public override void setCheckBoxes()
        {
            
        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "Stair Nosing":
                    return riserCount * stairWidth;
                case "Extra stair nosing lf":
                case "Stucco Material Remove and replace (LF)":
                case "1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor":
                    return 0.0000001;
                
                default:
                    return riserCount*stairWidth*2;
            }
        }
        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "Stair Nosing":
                case "Extra stair nosing lf":
                    return 0.0000001;

                default:
                    return totalSqft;
            }
        }

        public override void setExceptionValues(object s)
        {
            base.setExceptionValues(s);

            //if (SystemMaterials.Count != 0)
            //{
            //    SystemMaterial item = SystemMaterials.Where(x => x.Name == "Extra stair nosing lf").FirstOrDefault();
            //    if (item != null)
            //    {
            //        item.StairSqft = item.Qty;
            //        item.SMSqftH = 0;
            //        item.Hours = CalculateHrs(0, 0, item.StairSqft, item.StairsProductionRate);
            //        item.LaborExtension = (item.Hours + item.SetupMinCharge) * laborRate;
            //        item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

            //    }

            //    item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
            //    if (item != null)
            //    {
            //        item.SMSqftH = item.Qty * 32;
            //        item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, 0, item.StairsProductionRate);
            //        item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
            //        item.LaborUnitPrice = item.LaborExtension / item.Qty;
            //    }

            //    item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
            //    if (item != null)
            //    {
            //        item.SMSqftH = item.Qty;
            //        item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, 0, 0);
            //        item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
            //        item.LaborUnitPrice = item.LaborExtension / item.Qty;
            //    }
            //}
        }

        public override void calculateLaborHrs()
        {
            calLaborHrs(6, totalSqft);
        }
        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            return coverage == 0 ? 0 : lfArea / coverage;

        }
    }
}
