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
    public class PedestrianMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> MaterialNames;
        private double TotalSqftPlywood=0;
        private bool IsReseal;
        private bool? IsNewPlaywood;
        private bool RequireFlashing;
        public PedestrianMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            MaterialNames = new Dictionary<string, string>();
            TotalSqftPlywood = Js.TotalSqftPlywood;
            IsReseal = Js.IsReseal;
            IsNewPlaywood = Js.IsNewPlywood;
            RequireFlashing = Js.IsFlashingRequired;
            FillMaterialList();
            SystemMaterial.OnUnitChanged += (s, e) => { setUnitChangeValues(); };
            FetchMaterialValuesAsync(false);

        }
        public override void UpdateSumOfSqft()
        {
            double sumVal = totalSqft + TotalSqftPlywood;
            TotalLaborUnitPrice = sumVal == 0 ? 0 : TotalLaborWithoutDrive / sumVal;
            RaisePropertyChanged("TotalLaborUnitPrice");
        }
        public override void setUnitChangeValues()
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
                item.LaborUnitPrice = (riserCount + totalSqft + TotalSqftPlywood)==0?0:item.LaborExtension / (riserCount + totalSqft+TotalSqftPlywood);

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
        public override void calculateLaborHrs()
        {
            calLaborHrs(6,totalSqft+TotalSqftPlywood);

        }
        
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
                JobSetup Js = sender as JobSetup;
            if (Js!=null)
            {
                TotalSqftPlywood = Js.TotalSqftPlywood;
                IsReseal = Js.IsReseal;
                IsNewPlaywood = Js.IsNewPlywood;
                RequireFlashing = Js.IsFlashingRequired;
                
                SystemMaterials.Where(x => x.Name == "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)").
                    FirstOrDefault().SMUnits = Js.RiserCount.ToString();
            }
            
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override void ApplyCheckUnchecks(object obj)
        {
            lastCheckedMat = obj.ToString();
            //base.ApplyCheckUnchecks(obj);
            calculateRLqty();
            CalculateLaborMinCharge(false);
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
            if (MaterialNames == null)
            {
                MaterialNames = new Dictionary<string, string>();
                FillMaterialList();
            }
            var sysMat = GetSystemMaterial(MaterialNames);
                
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

                    //    SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    //    SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    UpdateMe(sysMat[i]);

                    
                    SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);

                    if (SystemMaterials[i].Name == "EXTRA STAIR NOSING" || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                        SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            //SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];
                            SystemMaterials[i].UpdateQuantity(qtyList[SystemMaterials[i].Name]);
                        }
                    }
                    if (SystemMaterials[i].Name == "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            //SystemMaterials[i].SMUnits = qtyList[SystemMaterials[i].Name].ToString();
                            SystemMaterials[i].UpdateUnits(qtyList[SystemMaterials[i].Name].ToString());
                        }
                    }
                    SystemMaterials[i].UpdateSpecialPricing(sp);
                }
            }
            #endregion

            else
            {
                SystemMaterials = sysMat;
                
            }
            setExceptionValues(null);
            if (hasSetupChanged)
            {
                setCheckBoxes();
                setUnitChangeValues();
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
        public virtual void FillMaterialList()
        {
            MaterialNames.Add("SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)",riserCount.ToString());
            MaterialNames.Add("REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)", "0");
            MaterialNames.Add("7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL", "2 GAL KIT");
            MaterialNames.Add("INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS", "0");
            MaterialNames.Add("7013 SC BASE COAT/ 5 GAL PAILS 40 MILS", "5 GAL PAIL");
            MaterialNames.Add("7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS", "5 GAL PAIL");
            MaterialNames.Add("7016 - AL - SC TOP COAT / 5 GAL PAILS 16 MILS", "5 GAL PAIL");
            MaterialNames.Add("1/20 SAND/ #100 LB", "100 LB BAG");
            MaterialNames.Add("3 IN. WHITE GLASS TAPE (PERIMETER)", "150' ROLL");
            MaterialNames.Add("SIKA 1-A CAULKING (PERIMETER)", "TUBE");
            MaterialNames.Add("DETAIL TAPE (NEW PLYWOOD)", "150' ROLL");
            MaterialNames.Add("SIKA 1-A CAULKING (NEW PLYWOOD)", "TUBE");
           
            MaterialNames.Add("UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT", "1 1/2 GAL KIT");
            MaterialNames.Add("9801 ACCELERATOR", "GALLON");
            //materialNames.Add("STAIR NOSING OVER CONCRETE", " ");
            MaterialNames.Add("INTEGRAL STAIR NOSING (EXCEL STYLE)", "LF");
            MaterialNames.Add("EXTRA STAIR NOSING", "LF");
            MaterialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            MaterialNames.Add("Stucco Material Remove and replace (LF)", "LF");

        }

        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0, double sqftStairs = 0, string matName = "")
        {
            //return base.getLaborUnitPrice(laborExtension, riserCount, totalSqft, sqftVert, sqftHor, sqftStairs, matName);
            return (TotalSqftPlywood + totalSqft + riserCount)==0?0:laborExtension / (TotalSqftPlywood + totalSqft + riserCount);
        }

        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {
            
            return calhrs == 0 ? 0 : calhrs > setupMin ?calhrs * laborRate : setupMin * laborRate;
        }

        public override void CalculateCostPerSqFT()
        {           
            CostPerSquareFeet = (totalSqft + TotalSqftPlywood+riserCount) == 0 ? 0 : Math.Round(TotalMaterialCost / (totalSqft + TotalSqftPlywood+riserCount), 2);
        }

        public override void CalculateTotalSqFt()
        {
            CostperSqftSlope = (totalSqft + riserCount + TotalSqftPlywood) == 0 ?0: TotalSlopingPrice / (totalSqft + riserCount+TotalSqftPlywood);
            CostperSqftMetal = (totalSqft + riserCount + TotalSqftPlywood) == 0 ? 0 : TotalMetalPrice / (totalSqft + riserCount + TotalSqftPlywood);
            CostperSqftMaterial = (totalSqft + riserCount + TotalSqftPlywood) == 0 ? 0 : TotalSystemPrice / (totalSqft + riserCount + TotalSqftPlywood);
            CostperSqftSubContract = (totalSqft + riserCount + TotalSqftPlywood) == 0 ? 0 : TotalSubcontractLabor / (totalSqft + riserCount + TotalSqftPlywood);
            TotalCostperSqft = CostperSqftSlope + CostperSqftMetal + CostperSqftMaterial + CostperSqftSubContract;
            RaisePropertyChanged("CostperSqftSlope");
            RaisePropertyChanged("CostperSqftMetal");
            RaisePropertyChanged("CostperSqftMaterial");
            RaisePropertyChanged("CostperSqftSubContract");
            RaisePropertyChanged("TotalCostperSqft");
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
                    return IsReseal;
                
                case "7013 SC BASE COAT/ 5 GAL PAILS 40 MILS":
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    return !IsReseal;
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                    return IsNewPlaywood.Value;
                case "UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT":
                    return totalSqft < 1 ? false : !IsReseal;
                case "STAIR NOSING OVER CONCRETE":
                    return totalSqft > 0 || RequireFlashing == true ? true : false;
                case "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)":
                    return riserCount > 0;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS":
                case "UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT":
                case "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL":
                case "1/20 SAND/ #100 LB":
                case "7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS":
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
                if (item.Name == "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)"
                    
                    || item.Name == "EXTRA STAIR NOSING"
                    || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)"
                    || item.Name == "1/20 SAND/ #100 LB"
                    ||item.Name== "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS"
                    || item.Name== "7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS"
                    || item.Name == "Stucco Material Remove and replace (LF)"                   
                    )
                {

                }
                else
                {
                    //if (item.Name == "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL")
                    //{
                    //    bool ischecked = item.IsMaterialChecked;
                    //    if (IsReseal)
                    //    {
                    //        item.IsMaterialChecked = IsReseal;
                    //    }
                    //}
                    //else
                        item.IsMaterialChecked = getCheckboxCheckStatus(item.Name);        
                }

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
                    return TotalSqftPlywood / 32 * 12 + riserCount * stairWidth * 2;
                case "UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT":
                    return totalSqft + riserCount * stairWidth * 2;
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    return riserCount;
                default:
                    return TotalSqftPlywood+totalSqft+riserCount*stairWidth*2;
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
                    return  lfArea / coverage;
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
            if (SystemMaterials.Count==0)
            {
                return;
            }
            double qty = 0;
            foreach (var item in SystemMaterials)
            {
                if (item.Name == "7013 SC BASE COAT/ 5 GAL PAILS 40 MILS" ||
                    item.Name == "7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS" || item.Name == "7016 - AL - SC TOP COAT / 5 GAL PAILS 16 MILS")
                {
                    if (item.IsMaterialChecked)
                    {
                        qty = qty + item.Qty;
                    }
                    
                }
            }
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "9801 ACCELERATOR").FirstOrDefault();
            bool ischecked = sysmat.IsMaterialChecked;
            sysmat.Qty = qty / sysmat.Coverage;
            sysmat.IsMaterialChecked = ischecked;

            sysmat = SystemMaterials.Where(x => x.Name == "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)").FirstOrDefault();
            if (sysmat != null)
            {
                ischecked = sysmat.IsMaterialChecked;
                sysmat.SMUnits = riserCount.ToString();
                //double.TryParse(sysmat.SMUnits, out myVal);
                sysmat.Qty = riserCount / sysmat.Coverage;
                sysmat.IsMaterialChecked = ischecked;
            }
        }

        public override void setExceptionValues(object s)
        {
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
                item.LaborUnitPrice = (TotalSqftPlywood + totalSqft + riserCount)==0?0:item.LaborExtension / (TotalSqftPlywood+totalSqft+riserCount);

            }
            calculateRLqty();

        }
        public override double getSqFtAreaH(string materialName)
        {
            //return base.getSqFtAreaH(materialName);
            switch (materialName)
            {
                case "UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT":
                    return totalSqft;
                case "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL":
                case "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS":
                case "7013 SC BASE COAT/ 5 GAL PAILS 40 MILS":
                case "7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS":
                case "7016 - AL - SC TOP COAT / 5 GAL PAILS 16 MILS":
                    return (totalSqft + TotalSqftPlywood);
                case "3 IN. WHITE GLASS TAPE (PERIMETER)":
                    return deckPerimeter;
                case "DETAIL TAPE (NEW PLYWOOD)":
                    return TotalSqftPlywood / 32 * 12;
                default:
                    return 0;
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
                    return riserCount * stairWidth * 2;
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
