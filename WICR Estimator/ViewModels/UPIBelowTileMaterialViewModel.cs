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
    class UPIBelowTileMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private bool isNewPlywood;
        private double totalSqftPlywood;
        public UPIBelowTileMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();

            FillMaterialList();

            FetchMaterialValuesAsync(false);

        }
        public override void UpdateSumOfSqft()
        {
            double sumVal = totalSqftPlywood;
            TotalLaborUnitPrice = sumVal == 0 ? 0 : TotalLaborWithoutDrive / sumVal;
            OnPropertyChanged("TotalLaborUnitPrice");
        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js!=null)
            {
                isNewPlywood = (bool)js.IsNewPlywood;
                totalSqftPlywood = js.TotalSqftPlywood;
                
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        private void FillMaterialList()
        {
            materialNames.Add("UI - 7012 PRIMER (FOR CONCRETE OR METAL)", "2 GAL KIT");
            materialNames.Add("UI - 7013 SC BASE COAT", "5 GAL PAIL");
            materialNames.Add("WG Fabric 36\"x150", "450 SF ROLL");
            materialNames.Add("DETAIL TAPE (PERIMETER)", "150 SF ROLL");
            materialNames.Add("VULKEM 116 SEALANT (10 OZ CARTRIDGE)", "TUBE");
            materialNames.Add("DETAIL TAPE (NEW PLYWOOD)", "150 SF ROLL");
            materialNames.Add("VULKEM 116 SEALANT (10 OZ CARTRIDGE) 1", "TUBE");
            materialNames.Add("7014 SC MEMBRANE (15 MILS)", "5 GAL PAIL");
            materialNames.Add("16/20 SAND BROADCAST TO REFUSAL", "#50 BAG");
            materialNames.Add("#90 GLASS CAP (OPTIONAL FOR SLIP SHEET)", "100 SF ROLL");
            materialNames.Add("ADD METAL LATHE AND STAPLES", "17 SQ FT SHT.");

            materialNames.Add("Staples (3/4 Inch Crown, Box of 13,500)", "BOX");
            materialNames.Add("SAND/CEMENT AND ACRYLIC UNDERLAY AT 1/2 INCH THICK", "MIXES");
            materialNames.Add("ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"", "EACH");
            materialNames.Add("ADD LF FOR DAMMING @ DRIP EDGE", "LF");
            
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");
            materialNames.Add("2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)", "LF");




        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    item.Name == "Stucco Material Remove and replace (LF)"||
                    item.Name== "ADD LF FOR DAMMING @ DRIP EDGE"||
                    item.Name== "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)"||
                        item.Name== "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"")
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
                    if (iscbEnabled)
                    {
                        SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                        SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    }

                    if (SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                        SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)"||
                        SystemMaterials[i].Name == "ADD LF FOR DAMMING @ DRIP EDGE" ||
                        SystemMaterials[i].Name == "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)" ||
                        SystemMaterials[i].Name == "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"")
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
            //calculateRLqty();
            CalculateCost(null);
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
        }


        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "ADD LF FOR DAMMING @ DRIP EDGE":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)":
                case "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"":
                case "#90 GLASS CAP (OPTIONAL FOR SLIP SHEET)":
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "VULKEM 116 SEALANT (10 OZ CARTRIDGE) 1":
                case "UI - 7012 PRIMER (FOR CONCRETE OR METAL)":
                    return false;
                case "7014 SC MEMBRANE (15 MILS)":
                    return deckPerimeter + totalSqftPlywood > 0 ? true : false;
                default:
                    return true;
            }
        }
        
        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "UI - 7012 PRIMER (FOR CONCRETE OR METAL)":
                case "#90 GLASS CAP (OPTIONAL FOR SLIP SHEET)":
                case "ADD METAL LATHE AND STAPLES":
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
                case "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)":
                    return 0;
                case "DETAIL TAPE (PERIMETER)":
                case "VULKEM 116 SEALANT (10 OZ CARTRIDGE)":
                    return deckPerimeter + riserCount *2* 2;
                case "#90 GLASS CAP (OPTIONAL FOR SLIP SHEET)":
                case "ADD METAL LATHE AND STAPLES":
                case "SAND/CEMENT AND ACRYLIC UNDERLAY AT 1/2 INCH THICK":
                    return totalSqftPlywood + riserCount * stairWidth ;
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "VULKEM 116 SEALANT (10 OZ CARTRIDGE) 1":
                    return totalSqftPlywood / 32 * 12 + stairWidth * riserCount * 2;
                default:
                    return totalSqftPlywood + riserCount * stairWidth * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)":
                case "ADD LF FOR DAMMING @ DRIP EDGE":
                case "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"":
                    return 0;
                default:
                    return lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {

                case "DETAIL TAPE (PERIMETER)":
                    return deckPerimeter;
                case "DETAIL TAPE (NEW PLYWOOD)":
                    return totalSqftPlywood / 32 * 12;
                case "VULKEM 116 SEALANT (10 OZ CARTRIDGE)":
                case "VULKEM 116 SEALANT (10 OZ CARTRIDGE) 1":
                case "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)":
                    return 0;
                default:
                    return totalSqftPlywood;
            }

        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {

                case "UI - 7013 SC BASE COAT":
                case "7014 SC MEMBRANE (15 MILS)":
                    return riserCount;
                case "DETAIL TAPE (PERIMETER)":
                    return riserCount * 2 * 2;
                case "#90 GLASS CAP (OPTIONAL FOR SLIP SHEET)":
                    return stairWidth * riserCount;
                case "VULKEM 116 SEALANT (10 OZ CARTRIDGE)":
                case "VULKEM 116 SEALANT (10 OZ CARTRIDGE) 1":
                case "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)":
                case "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "ADD LF FOR DAMMING @ DRIP EDGE":
                case "ADD METAL LATHE AND STAPLES":
                case "SAND/CEMENT AND ACRYLIC UNDERLAY AT 1/2 INCH THICK":
                case "Staples (3/4 Inch Crown, Box of 13,500)":
                    return 0;
                default:
                    return riserCount * stairWidth * 2;

            }
        }

        public override void calculateLaborHrs()
        {
            calLaborHrs(10, totalSqftPlywood);

        }
        //calculate for Desert Crete
        public override void calculateRLqty()
        {
            //base.calculateRLqty();
        }

        public override bool canApply(object obj)
        {
            return true;
        }
        public override void setExceptionValues(object s)
        {
            if (SystemMaterials.Count != 0)
            {

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
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
                item = SystemMaterials.Where(x => x.Name == "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;

                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.MaterialExtension = item.SpecialMaterialPricing == 0 ? item.Qty * item.MaterialPrice : item.Qty * item.SpecialMaterialPricing;
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                item = SystemMaterials.Where(x => x.Name == "ADD LF FOR DAMMING @ DRIP EDGE").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;

                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.MaterialExtension = item.SpecialMaterialPricing == 0 ? item.Qty * item.MaterialPrice : item.Qty * item.SpecialMaterialPricing;
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                item = SystemMaterials.Where(x => x.Name == "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqft = item.Qty*0.5;
                    item.SMSqftV = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate,item.SMSqftV,item.VerticalProductionRate);
                    item.MaterialExtension = item.SpecialMaterialPricing == 0 ? item.Qty * item.MaterialPrice : item.Qty * item.SpecialMaterialPricing;
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
            }
            CalculateLaborMinCharge(false);
        }
        public override bool IncludedInLaborMin(string matName)
        {
            switch (matName)
            {
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)":
                    return false;
                default:
                    return true;
            }
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            bool isChecked;
            if (obj.ToString() == "ADD METAL LATHE AND STAPLES")
            {
                isChecked = SystemMaterials.Where(x => x.Name == "ADD METAL LATHE AND STAPLES").FirstOrDefault().IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "Staples (3/4 Inch Crown, Box of 13,500)").FirstOrDefault().IsMaterialChecked = isChecked;
                SystemMaterials.Where(x => x.Name == "SAND/CEMENT AND ACRYLIC UNDERLAY AT 1/2 INCH THICK").FirstOrDefault().IsMaterialChecked = isChecked;
                
                if (isChecked)
                {
                    SystemMaterials.Where(x => x.Name == "DETAIL TAPE (NEW PLYWOOD)").FirstOrDefault().IsMaterialChecked = !isChecked;
                    SystemMaterials.Where(x => x.Name == "VULKEM 116 SEALANT (10 OZ CARTRIDGE) 1").FirstOrDefault().IsMaterialChecked = !isChecked;
                }
                else
                {
                    SystemMaterials.Where(x => x.Name == "DETAIL TAPE (NEW PLYWOOD)").FirstOrDefault().IsMaterialChecked = isNewPlywood;
                    SystemMaterials.Where(x => x.Name == "VULKEM 116 SEALANT (10 OZ CARTRIDGE) 1").FirstOrDefault().IsMaterialChecked = isNewPlywood;
                }
            }

            calculateRLqty();
            CalculateLaborMinCharge(false);
        }

        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "UI - 7012 PRIMER (FOR CONCRETE OR METAL)":
                    return stairWidth;
                default:
                    return 0;
            }
        }
        public override void setCheckBoxes()
        {
            ApplyCheckUnchecks("ADD METAL LATHE AND STAPLES");
        }

        public override void CalculateTotalSqFt()
        {
            if ((totalSqftPlywood + riserCount) == 0)
            {
                CostperSqftSlope = 0;
                CostperSqftMetal = 0;
                CostperSqftMaterial = 0;
                CostperSqftSubContract = 0;
            }
            else
            {
                CostperSqftSlope = TotalSlopingPrice / (totalSqftPlywood + riserCount);
                CostperSqftMetal = TotalMetalPrice / (totalSqftPlywood + riserCount);
                CostperSqftMaterial = TotalSystemPrice / (totalSqftPlywood + riserCount);
                CostperSqftSubContract = TotalSubcontractLabor / (totalSqftPlywood + riserCount);
            }
            TotalCostperSqft = CostperSqftSlope + CostperSqftMetal + CostperSqftMaterial + CostperSqftSubContract;
            OnPropertyChanged("CostperSqftSlope");
            OnPropertyChanged("CostperSqftMetal");
            OnPropertyChanged("CostperSqftMaterial");
            OnPropertyChanged("CostperSqftSubContract");
            OnPropertyChanged("TotalCostperSqft");
        }

    }
}
