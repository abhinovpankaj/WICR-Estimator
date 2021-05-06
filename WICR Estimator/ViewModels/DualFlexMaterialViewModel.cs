using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    [DataContract]
    class DualFlexMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private bool hasMortarBed;
        private bool hasQuarterMortarBed;
        private bool hasElastatex;
        private bool hasNoLadder;

        
        private double linearFootage;
        private double totalSqftVertical;
        
        public DualFlexMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();

            FillMaterialList();

            FetchMaterialValuesAsync(false);

        }

        public override void UpdateSumOfSqft()
        {
            double sumVal = totalSqft + totalSqftVertical;
            TotalLaborUnitPrice = sumVal == 0 ? 0 : TotalLaborWithoutDrive / sumVal;
            RaisePropertyChanged("TotalLaborUnitPrice");
        }
        private void FillMaterialList()
        {
            materialNames.Add("OPTIONAL ADD FOR LABOR ON NON-COMPETITIVE QUOTES", "");
            materialNames.Add("RP II FABRIC 10 IN WIDE FOR PERIMETER (300 LF) FROM ACME", "ROLL");
            materialNames.Add("RP FABRIC 40 IN WIDE (1000 S.F.) FROM ACME BY THE YARD", "ROLL");
            materialNames.Add("NEOBOND LIQUID", "5 GAL PAIL");
            materialNames.Add("RESISTITE LIQUID", "5 GAL  PAIL");
            materialNames.Add("RESISTITE POWDER", "55# BAG");
            materialNames.Add("NEOBOND MEMBRANE AND RP FABRIC FOR WALLS  (2 COATS)", "5 GAL PAIL");
            materialNames.Add("INTERLAMINATE PRIMER (XYLENE FROM LOWRYS)", "5 GAL PAIL");
            materialNames.Add("ELASTATEX 500 RESIN AND CATALYST (2 COATS/ HORIZONTAL SURFACES ONLY) 40 MILS", "5 GAL PAIL");
            materialNames.Add("DETAIL TAPE (4 inch PW polyester cloth-PERIMETER)", "4 in x 300 lf roll");
            materialNames.Add("SIKA 1-A CAULKING (PERIMETER)", "10 oz.TUBE");
            materialNames.Add("DETAIL TAPE (4 PW inch polyester-NEW PLYWOOD)", "150 SF ROLL");
            materialNames.Add("SIKA 1-A CAULKING (NEW PLYWOOD)", "TUBE");
            materialNames.Add("ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"", "EACH");
            materialNames.Add("ADD LF FOR DAMMING @ DRIP EDGE", "LF");
            
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");

        }

        public override string GetOperation(string matName)
        {
            switch (matName)
            {
                case "OPTIONAL ADD FOR LABOR ON NON-COMPETITIVE QUOTES":
                    return "ADD FOR LABOR";
                case "RP II FABRIC 10 IN WIDE FOR PERIMETER (300 LF) FROM ACME":
                    return "STAIR & PERIMETER DETAIL";
                case "RP FABRIC 40 IN WIDE (1000 S.F.) FROM ACME BY THE YARD":
                    return "ROLL OUT GLASS";
                case "NEOBOND LIQUID":
                    return "SATURATE GLASS";
                case "RESISTITE LIQUID":
                    return "PREP FLOOR";
                case "RESISTITE POWDER":
                    return "ADD TO FILLER";
                case "NEOBOND MEMBRANE AND RP FABRIC FOR WALLS  (2 COATS)":
                    return "2 COATS WITH GLASS FABRIC";
                case "INTERLAMINATE PRIMER (XYLENE FROM LOWRYS)":
                    return "PRIME AFTER CONSTRUCTION";
                case "ELASTATEX 500 RESIN AND CATALYST (2 COATS/ HORIZONTAL SURFACES ONLY) 40 MILS":
                    return "TROWEL 2 COATS";
                case "DETAIL TAPE (4 inch PW polyester cloth-PERIMETER)":
                    return "STAIR & PERIMETER";
                case "SIKA 1-A CAULKING (PERIMETER)":
                    return "ADD SAND AND TOP COAT";
                case "DETAIL TAPE (4 PW inch polyester-NEW PLYWOOD)":
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                    return "DETAIL JOINTS & SEAMS";
                
                case "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"":
                    return "PLUG DRAIN & FLOOD";
                case "ADD LF FOR DAMMING @ DRIP EDGE":
                    return "DAM UP WITH METAL";
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                    return "REMOVE AND REPLACE DRYROT";
                case "Stucco Material Remove and replace (LF)":
                    return "REMOVE AND REPLACE 12 IN. OF STUCCO";
                default:
                    return "";
            }
        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"" || item.Name == "ADD LF FOR DAMMING @ DRIP EDGE"
                    || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" || item.Name == "Stucco Material Remove and replace (LF)")
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

                    if (SystemMaterials[i].Name == "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"" || SystemMaterials[i].Name == "ADD LF FOR DAMMING @ DRIP EDGE"
                    || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" || SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)")
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
        public override ObservableCollection<OtherItem> GetOtherMaterials()
        {
            ObservableCollection<OtherItem> om = new ObservableCollection<OtherItem>();
            om.Add(new OtherItem { Name = "Access issues?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional prep?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional labor?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Alternate material?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional Move ons?", IsReadOnly = false });
            return om;
        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js != null)
            {
                
                hasMortarBed = Js.HasQuarterLessMortarBed;
                hasQuarterMortarBed = Js.HasQuarterMortarBed;
                hasElastatex = Js.HasElastex;
                linearFootage = Js.LinearCopingFootage;
                hasNoLadder = Js.HasEasyAccess;
                totalSqftVertical = Js.TotalSqftVertical;
                
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override void CalculateCostPerSqFT()
        {
            CostPerSquareFeet = (totalSqft  + totalSqftVertical+riserCount) == 0 ? 0 : Math.Round(TotalMaterialCost / (totalSqft +  totalSqftVertical+riserCount), 2);
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
               // case "ADD LF FOR DAMMING @ DRIP EDGE":
                case "Stucco Material Remove and replace (LF)":
                //case "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"":
                    return false;
                case "INTERLAMINATE PRIMER (XYLENE FROM LOWRYS)":
                case "ELASTATEX 500 RESIN AND CATALYST (2 COATS/ HORIZONTAL SURFACES ONLY) 40 MILS":
                case "DETAIL TAPE (4 inch PW polyester cloth-PERIMETER)":
                case "SIKA 1-A CAULKING (PERIMETER)":
                    return hasElastatex;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "DETAIL TAPE (4 PW inch polyester-NEW PLYWOOD)":
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                case "OPTIONAL ADD FOR LABOR ON NON-COMPETITIVE QUOTES":
                
                    return true;
                default:
                    return false;
            }

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "RP II FABRIC 10 IN WIDE FOR PERIMETER (300 LF) FROM ACME":
                    return linearFootage + riserCount * stairWidth * 2;
                case "RP FABRIC 40 IN WIDE (1000 S.F.) FROM ACME BY THE YARD":
                    return totalSqft + totalSqftVertical + riserCount * stairWidth * 2;
                case "NEOBOND LIQUID":
                case "RESISTITE LIQUID":
                case "RESISTITE POWDER":
                case "INTERLAMINATE PRIMER (XYLENE FROM LOWRYS)":
                case "ELASTATEX 500 RESIN AND CATALYST (2 COATS/ HORIZONTAL SURFACES ONLY) 40 MILS":
                    return totalSqft + 0.5 * linearFootage + riserCount * stairWidth * 2;
                case "ADD LF FOR DAMMING @ DRIP EDGE":
                case "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"":
                    return 1;
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                case "DETAIL TAPE (4 PW inch polyester-NEW PLYWOOD)":
                    return totalSqft / 32 * 12 + riserCount * stairWidth * 2;
                case "DETAIL TAPE (4 inch PW polyester cloth-PERIMETER)":
                case "SIKA 1-A CAULKING (PERIMETER)":
                    return deckPerimeter;
                case "NEOBOND MEMBRANE AND RP FABRIC FOR WALLS  (2 COATS)":
                    return totalSqftVertical;
                default:
                    return 0;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                default:
                    return coverage == 0 ? 0 : lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "OPTIONAL ADD FOR LABOR ON NON-COMPETITIVE QUOTES":
                case "RESISTITE LIQUID":
                case "RESISTITE POWDER":
                    return totalSqft + linearFootage * 0.5;
                case "RP FABRIC 40 IN WIDE (1000 S.F.) FROM ACME BY THE YARD":
                case "NEOBOND LIQUID":
                case "INTERLAMINATE PRIMER (XYLENE FROM LOWRYS)":
                case "ELASTATEX 500 RESIN AND CATALYST (2 COATS/ HORIZONTAL SURFACES ONLY) 40 MILS":
                    return totalSqft;
                case "DETAIL TAPE (4 inch PW polyester cloth-PERIMETER)":
                    return deckPerimeter;
                case "SIKA 1-A CAULKING (PERIMETER)":
                    return linearFootage;
                case "DETAIL TAPE (4 PW inch polyester-NEW PLYWOOD)":
                    return totalSqft / 32 * 12;
                default:
                    return 0;
            }

        }
        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "NEOBOND MEMBRANE AND RP FABRIC FOR WALLS  (2 COATS)":
                case "DETAIL TAPE (4 PW inch polyester-NEW PLYWOOD)":
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                case "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"":
                case "ADD LF FOR DAMMING @ DRIP EDGE":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":

                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "ELASTATEX 500 RESIN AND CATALYST (2 COATS/ HORIZONTAL SURFACES ONLY) 40 MILS":
                    return riserCount;
                case "SIKA 1-A CAULKING (PERIMETER)":
                    return riserCount * 2 * 2;
                default:
                    return riserCount * stairWidth * 2;
            }
        }
        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "RP FABRIC 40 IN WIDE (1000 S.F.) FROM ACME BY THE YARD":
                case "NEOBOND MEMBRANE AND RP FABRIC FOR WALLS  (2 COATS)":
                    return totalSqftVertical;

                case "RP II FABRIC 10 IN WIDE FOR PERIMETER (300 LF) FROM ACME":
                    return deckPerimeter;
                default:
                    return 0;
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
        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0, double sqftStairs = 0, string matName = "")
        {
            double sqft = sqftHor + sqftStairs + sqftVert;
            return sqft == 0 ? 0 : laborExtension / sqft;
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
            bool isExceptionValueSet=false;
            if (SystemMaterials.Count != 0)
            {

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH =hasElastatex ? item.Qty*2:item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate, item.SMSqftV, item.VerticalProductionRate);

                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                    isExceptionValueSet = true;
                }

                item = SystemMaterials.Where(x => x.Name == "ADD LF FOR DAMMING @ DRIP EDGE").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate, item.SMSqftV, item.VerticalProductionRate);

                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                    isExceptionValueSet = true;
                }

               

                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty * 32;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.Hours == 0 ? 0 : item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty / 32;
                    isExceptionValueSet = true;
                }
                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.Hours == 0 ? 0 : item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                    isExceptionValueSet = true;
                }
                //if (isExceptionValueSet ==true)
                    //CalculateLaborMinCharge(false);
            }
            
        }
        
        public override void ApplyCheckUnchecks(object obj)
        {

            
        }

        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {
            if (calhrs == 0)
            {
                return 0;
            }
            else
                return (setupMin + calhrs ) * laborRate;
        }
        public override void setCheckBoxes()
        {
            SystemMaterial sysmat = SystemMaterials.FirstOrDefault(x => x.Name == "MIRADRAIN 6000 XL (VERTICAL ONLY)");
            if (sysmat != null)
            {
                SystemMaterials.FirstOrDefault(x => x.Name == "MIRASTICK ADHESIVE (GLUE DOWN DRAIN MAT)").IsMaterialChecked = sysmat.IsMaterialChecked;
            }
        }
    }
}
