using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class PedestrianMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double totalSqftPlywood=0;
        private bool isReseal;
        private bool? isNewPlaywood;
        private bool requireFlashing;
        public PedestrianMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            totalSqftPlywood = Js.TotalSqftPlywood;
            isReseal = Js.IsReseal;
            isNewPlaywood = Js.IsNewPlywood;
            requireFlashing = Js.IsFlashingRequired;
            FillMaterialList();
            SystemMaterial.OnUnitChanged += (s, e) => { setUnitChangeValues(); };
            FetchMaterialValuesAsync(false);

        }

        private void setUnitChangeValues()
        {
            SystemMaterial item = SystemMaterials.Where(x => x.Name == "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)").FirstOrDefault();
            if (item != null)
            {
                
                double unit = 0;
                
                Double.TryParse(item.SMUnits,out unit);
                item.SMSqftH = unit;
                item.Qty = unit / item.Coverage;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, 0, 0);
                item.LaborExtension = item.Hours==0?0:item.Hours> item.SetupMinCharge? item.Hours * laborRate: item.SetupMinCharge*laborRate;
                item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft+totalSqftPlywood);

            }
        }
        public override void CalculateLaborMinCharge()
        {
            LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
                                        x.IsMaterialChecked).ToList().Select(x => x.Hours).Sum();

            LaborMinChargeLaborExtension = LaborMinChargeMinSetup + LaborMinChargeHrs > 20 ? 0 :
                                                (20 - LaborMinChargeMinSetup + LaborMinChargeHrs) * laborRate;
            base.CalculateLaborMinCharge();
        }

        public override bool IncludedInLaborMin(string matName)
        {
            switch (matName)
            {
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                case "EXTRA STAIR NOSING":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return false;
                default:
                    return true;
            }
        }
        public override void calculateLaborHrs()
        {
            calLaborHrs(6);

        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js!=null)
            {
                totalSqftPlywood = Js.TotalSqftPlywood;
                isReseal = Js.IsApprovedForSandCement;
                isNewPlaywood = Js.IsNewPlywood;
                requireFlashing = Js.IsFlashingRequired;
                hasContingencyDisc = Js.TotalSqft + Js.TotalSqftPlywood > 1000 ? true : false;
                
            }
            
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override void ApplyCheckUnchecks(object obj)
        {
            //base.ApplyCheckUnchecks(obj);
        }
        public override bool canApply(object obj)
        {
            return true;
        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "EXTRA STAIR NOSING" || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    item.Name == "Stucco Material Remove and replace (LF)")
                {
                    qtyList.Add(item.Name, item.Qty);
                }
                if (item.Name == "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)")
                {
                    double unit = 0;
                    double.TryParse(item.SMUnits, out unit);
                    qtyList.Add(item.Name,unit);
                }

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
                    if (SystemMaterials[i].Name == "EXTRA STAIR NOSING" || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                        SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];

                        }
                    }
                    if (SystemMaterials[i].Name == "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            SystemMaterials[i].SMUnits = qtyList[SystemMaterials[i].Name].ToString();

                        }
                    }

                }

            }
            #endregion

            else
                SystemMaterials = sysMat;

            setExceptionValues();
            setCheckBoxes();

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
        private void FillMaterialList()
        {
            materialNames.Add("SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)","0");
            materialNames.Add("REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)", "0");
            materialNames.Add("7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL", "2 GAL KIT");
            materialNames.Add("INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS", "0");
            materialNames.Add("7013 SC BASE COAT/ 5 GAL PAILS 40 MILS", "5 GAL PAIL");
            materialNames.Add("7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS", "5 GAL PAIL");
            materialNames.Add("7016 - AL - SC TOP COAT / 5 GAL PAILS 16 MILS", "5 GAL PAIL");
            materialNames.Add("1/20 SAND/ #100 LB", "100 LB BAG");
            materialNames.Add("3 IN. WHITE GLASS TAPE (PERIMETER)", "150' ROLL");
            materialNames.Add("SIKA 1-A CAULKING (PERIMETER)", "TUBE");
            materialNames.Add("DETAIL TAPE (NEW PLYWOOD)", "150' ROLL");
            materialNames.Add("SIKA 1-A CAULKING (NEW PLYWOOD)", "TUBE");
           
            materialNames.Add("UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT", "1 1/2 GAL KIT");
            materialNames.Add("9801 ACCELERATOR", "GALLON");
            //materialNames.Add("STAIR NOSING OVER CONCRETE", " ");
            materialNames.Add("INTEGRAL STAIR NOSING (EXCEL STYLE)", "LF");
            materialNames.Add("EXTRA STAIR NOSING", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");

        }

        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0, double sqftStairs = 0, string matName = "")
        {
            //return base.getLaborUnitPrice(laborExtension, riserCount, totalSqft, sqftVert, sqftHor, sqftStairs, matName);
            return laborExtension / (totalSqftPlywood + totalSqft + riserCount);
        }

        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {
            
            return calhrs == 0 ? 0 : calhrs > setupMin ?calhrs * laborRate : setupMin * laborRate;
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)":
                
                case "EXTRA STAIR NOSING":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return false;
                case "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL":
                    return isReseal;
                case "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS":
                case "7013 SC BASE COAT/ 5 GAL PAILS 40 MILS":
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    return !isReseal;
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                    return isNewPlaywood.Value;
                case "UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT":
                    return totalSqft < 1 ? false : !isReseal;
                case "STAIR NOSING OVER CONCRETE":
                    return totalSqft > 0 || requireFlashing == true ? true : false;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)":
                    return true;
                default:
                    return false;
            }
        }

        public override void setCheckBoxes()
        {
            //base.setCheckBoxes();
            foreach (SystemMaterial item in SystemMaterials)
            {
                item.IsMaterialChecked = getCheckboxCheckStatus(item.Name);
            }

        }
        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)":
                case "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)":
                case "9801 ACCELERATOR":
                case "EXTRA STAIR NOSING":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "3 IN. WHITE GLASS TAPE (PERIMETER)":
                case "SIKA 1-A CAULKING (PERIMETER)":
                    return deckPerimeter + riserCount * 2 * 2;
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                    return totalSqftPlywood / 32 * 12 + riserCount * stairWidth * 2;
                case "UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT":
                    return totalSqft + riserCount * stairWidth * 2;
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    return riserCount;
                default:
                    return totalSqftPlywood+totalSqft+riserCount*stairWidth*2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)":
                    return 1;
                case "7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS":
                case "7016 - AL - SC TOP COAT / 5 GAL PAILS 16 MILS":
                    return lfArea / coverage < 0.5 ? 0.5 : lfArea / coverage;
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    double locVal = 0;
                    locVal = 4 * lfArea;
                    return locVal;
                default:
                    return lfArea/coverage;
            }
        }
        //calculateQTY for 9801 ACCELERATOR
        public override void calculateRLqty()
        {            
            double qty = 0;
            foreach (var item in SystemMaterials)
            {
                if (item.Name == "7013 SC BASE COAT/ 5 GAL PAILS 40 MILS" ||
                    item.Name == "7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS" || item.Name == "7016 - AL - SC TOP COAT / 5 GAL PAILS 16 MILS")
                {
                    qty = qty + item.Qty;
                }
            }
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "9801 ACCELERATOR").FirstOrDefault();
            
            sysmat.Qty = qty / sysmat.Coverage;
        }

        public override void setExceptionValues()
        {
            //base.setExceptionValues();
            SystemMaterial item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
            if (item != null)
            {
                item.SMSqftH = item.Qty * 32;
                
                item.Hours = CalculateHrs(item.SMSqftH,item.HorizontalProductionRate, 0, 0);
                
                item.LaborExtension=item.Hours==0?0:item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / item.Qty;

            }
            item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
            if (item != null)
            {
                item.SMSqftH = item.Qty ;

                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, 0, 0);
                
                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / item.Qty;

            }

            item = SystemMaterials.Where(x => x.Name == "EXTRA STAIR NOSING").FirstOrDefault();
            if (item != null)
            {
                item.StairSqft = item.Qty ;

                item.Hours = CalculateHrs(0, 0, item.StairSqft, item.StairsProductionRate);
                
                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (totalSqftPlywood+totalSqft+riserCount);

            }
        }
        public override double getSqFtAreaH(string materialName)
        {
            //return base.getSqFtAreaH(materialName);
            switch (materialName)
            {
                case "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL":
                case "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS":
                case "7013 SC BASE COAT/ 5 GAL PAILS 40 MILS":
                case "7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS":
                case "7016 - AL - SC TOP COAT / 5 GAL PAILS 16 MILS":
                    return totalSqft + totalSqftPlywood;
                case "3 IN. WHITE GLASS TAPE (PERIMETER)":
                    return deckPerimeter;
                case "DETAIL TAPE (NEW PLYWOOD)":
                    return totalSqftPlywood / 32 * 12;
                default:
                    return 0.0000001;
            }
        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)":
                    return riserCount*stairWidth;
                case "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL":
                case "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS":
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT":
                    return riserCount * 4.5 * 2;
                case "7013 SC BASE COAT/ 5 GAL PAILS 40 MILS":
                case "7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS":
                case "7016 - AL - SC TOP COAT / 5 GAL PAILS 16 MILS":
                    return riserCount;
                case "3 IN. WHITE GLASS TAPE (PERIMETER)":
                    return riserCount * 2 * 2;
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    return riserCount * 3.5;
                default:
                    return 0.0000001;
            }
        }

    }
        
    
}
