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
    class ParkingMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> MaterialNames;
        private double TotalSqftPlywood = 0;
        private bool IsReseal;
        private bool? IsNewPlaywood;
        private bool RequireFlashing;
        public ParkingMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            MaterialNames = new Dictionary<string, string>();
            TotalSqftPlywood = Js.TotalSqftPlywood;
            IsReseal = Js.IsReseal;
            IsNewPlaywood = Js.IsNewPlywood;
            RequireFlashing = Js.IsFlashingRequired;
            FillMaterialList();
            SystemMaterial.OnUnitChanged += (s, e) => { setUnitChangeValues(s); };
            FetchMaterialValuesAsync(false);

        }

        private void setUnitChangeValues(object s)
        {
            SystemMaterial item = SystemMaterials.Where(x => x.Name == s.ToString()).FirstOrDefault();
            if (item != null)
            {

                double unit = 0;

                Double.TryParse(item.SMUnits, out unit);
                item.SMSqft = unit;
                item.SMSqftH = unit;
                item.Qty = unit / item.Coverage;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft + TotalSqftPlywood);

            }
            //item = SystemMaterials.Where(x => x.Name == "Striping for small cracKs (less than 1/8\")").FirstOrDefault();
            //if (item != null)
            //{

            //    double unit = 0;

            //    Double.TryParse(item.SMUnits, out unit);
            //    item.SMSqftH = unit;
            //    item.SMSqft = unit;
            //    item.Qty = unit / item.Coverage;
            //    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
            //    item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
            //    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft + TotalSqftPlywood);

            //}
            //item = SystemMaterials.Where(x => x.Name == "Route and caulk moving cracks (greater than 1/8\")").FirstOrDefault();
            //if (item != null)
            //{

            //    double unit = 0;

            //    Double.TryParse(item.SMUnits, out unit);
            //    item.SMSqftH = unit;
            //    item.SMSqft = unit;
            //    item.Qty = unit / item.Coverage;
            //    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
            //    item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
            //    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft + TotalSqftPlywood);

            //}

            //item = SystemMaterials.Where(x => x.Name == "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC").FirstOrDefault();
            //if (item != null)
            //{
            //    double unit = 0;
            //    Double.TryParse(item.SMUnits, out unit);
            //    item.SMSqftH = unit;
            //    item.SMSqft = unit;
            //    item.Qty = unit / item.Coverage;
            //    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
            //    item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
            //    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft + TotalSqftPlywood);

            //}
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            lastCheckedMat = obj.ToString();
            //base.ApplyCheckUnchecks(obj);
            calculateRLqty();
            CalculateLaborMinCharge(false);
        }
        public override void UpdateSumOfSqft()
        {
            double sumVal = totalSqft + TotalSqftPlywood;
            TotalLaborUnitPrice = sumVal == 0 ? 0 : TotalLaborWithoutDrive / sumVal;
            RaisePropertyChanged("TotalLaborUnitPrice");
        }
        
        public void FillMaterialList()
        {
            MaterialNames.Add("SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)", riserCount.ToString());
            MaterialNames.Add("REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH) UPI 7013 SC BASE COAT", "0");
            MaterialNames.Add("Striping for small cracKs (less than 1/8\")",totalSqft.ToString());
            MaterialNames.Add("Route and caulk moving cracks (greater than 1/8\")","0");
            MaterialNames.Add("7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL", "2 GAL KIT");
            MaterialNames.Add("INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS", "0");
            MaterialNames.Add("7013 SC BASE COAT/ 5 GAL PAILS 30 MILS", "5 GAL PAIL");
            MaterialNames.Add("7016 - AR - SC INTERMEDIATE/ 5 GAL PAILS 20 MILS", "5 GAL PAIL");
            MaterialNames.Add("SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC", "0");
            MaterialNames.Add("7016 SC TOP COAT/ 5 GAL PAILS 16 MILS", "5 GAL PAIL");

            MaterialNames.Add("2/16 MESH SAND/ #100 LB", "100 LB BAG");
            MaterialNames.Add("3 IN. WHITE GLASS TAPE (PERIMETER)", "150' ROLL");
            MaterialNames.Add("SIKA 1-A CAULKING (PERIMETER)", "TUBE");
            MaterialNames.Add("DETAIL TAPE (NEW PLYWOOD)", "150' ROLL");
            MaterialNames.Add("SIKA 1-A CAULKING (NEW PLYWOOD)", "TUBE");

            MaterialNames.Add("UI 7118 CONCRETE PRIMER 2 gal kit", "2 GAL KIT");
            MaterialNames.Add("9801 ACCELERATOR", "GALLON");
            //materialNames.Add("STAIR NOSING OVER CONCRETE", " ");
            MaterialNames.Add("INTEGRAL STAIR NOSING (EXCEL STYLE)", "LF");
            MaterialNames.Add("EXTRA STAIR NOSING", "LF");
            MaterialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            MaterialNames.Add("Stucco Material Remove and replace (LF)", "LF");

        }

        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js != null)
            {
                TotalSqftPlywood = Js.TotalSqftPlywood;
                IsReseal = Js.IsReseal;
                IsNewPlaywood = Js.IsNewPlywood;
                RequireFlashing = Js.IsFlashingRequired;
                //hasContingencyDisc = Js.TotalSqft + Js.TotalSqftPlywood > 1000 ? true : false; 
                SystemMaterials.Where(x => x.Name == "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)").
                    FirstOrDefault().SMUnits = Js.RiserCount.ToString();
                //SystemMaterials.Where(x => x.Name == "Striping for small cracKs (less than 1/8\")").
                    //FirstOrDefault().SMUnits = Js.TotalSqft.ToString();

            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override void CalculateCostPerSqFT()
        {
            CostPerSquareFeet = (totalSqft + TotalSqftPlywood + riserCount) == 0 ? 0 : Math.Round(TotalMaterialCost / (totalSqft + TotalSqftPlywood + riserCount), 2);
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
                if (item.Name == "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH) UPI 7013 SC BASE COAT"
                    ||item.Name== "Striping for small cracKs (less than 1/8\")"
                    ||item.Name== "Route and caulk moving cracks (greater than 1/8\")"
                    ||item.Name== "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC")
                {
                    double unit = 0;
                    double.TryParse(item.SMUnits, out unit);
                    qtyList.Add(item.Name, unit);
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

                    SystemMaterials[i].UpdateSpecialPricing(sp);
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
                    if (SystemMaterials[i].Name == "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH) UPI 7013 SC BASE COAT"
                    || SystemMaterials[i].Name == "Striping for small cracKs (less than 1/8\")"
                    || SystemMaterials[i].Name == "Route and caulk moving cracks (greater than 1/8\")"
                    || SystemMaterials[i].Name == "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            //SystemMaterials[i].SMUnits = qtyList[SystemMaterials[i].Name].ToString();
                            SystemMaterials[i].UpdateUnits(qtyList[SystemMaterials[i].Name].ToString());
                        }
                    }

                }
                setUnitChangeValues();
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
        public override void setUnitChangeValues()
        {
            SystemMaterial item = SystemMaterials.Where(x => x.Name == "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH) UPI 7013 SC BASE COAT").FirstOrDefault();
            if (item != null)
            {

                double unit = 0;

                Double.TryParse(item.SMUnits, out unit);
                item.SMSqft = unit;
                item.SMSqftH = unit;
                item.Qty = unit / item.Coverage;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft + TotalSqftPlywood);

            }
            item = SystemMaterials.Where(x => x.Name == "Striping for small cracKs (less than 1/8\")").FirstOrDefault();
            if (item != null)
            {

                double unit = 0;

                Double.TryParse(item.SMUnits, out unit);
                item.SMSqftH = unit;
                item.SMSqft = unit;
                item.Qty = unit / item.Coverage;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft + TotalSqftPlywood);

            }
            item = SystemMaterials.Where(x => x.Name == "Route and caulk moving cracks (greater than 1/8\")").FirstOrDefault();
            if (item != null)
            {

                double unit = 0;

                Double.TryParse(item.SMUnits, out unit);
                item.SMSqftH = unit;
                item.SMSqft = unit;
                item.Qty = unit / item.Coverage;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft + TotalSqftPlywood);

            }

            item = SystemMaterials.Where(x => x.Name == "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC").FirstOrDefault();
            if (item != null)
            {
                double unit = 0;
                Double.TryParse(item.SMUnits, out unit);
                item.SMSqftH = unit;
                item.SMSqft = unit;
                item.Qty = unit / item.Coverage;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft + TotalSqftPlywood);

            }
            //calculateRLqty();
            //CalculateLaborMinCharge(false);
        }
        
        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0, double sqftStairs = 0, string matName = "")
        {
            
            return (TotalSqftPlywood + totalSqft + riserCount)==0?0:laborExtension / (TotalSqftPlywood + totalSqft + riserCount);
        }
        
        public override bool canApply(object obj)
        {
            return true;
        }
        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {

            return calhrs == 0 ? 0 : calhrs > setupMin ? calhrs * laborRate : setupMin * laborRate;
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH) UPI 7013 SC BASE COAT":
                case "EXTRA STAIR NOSING":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS":
                case "Stucco Material Remove and replace (LF)":
                case "Route and caulk moving cracks (greater than 1/8\")":
                case "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC":
                case "Striping for small cracKs (less than 1/8\")":
                    return false;
                case "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL":
                    return IsReseal;
                
                case "7013 SC BASE COAT/ 5 GAL PAILS 30 MILS":
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    return !IsReseal;
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                    return IsNewPlaywood.Value;
                case "UI 7118 CONCRETE PRIMER 2 gal k":
                    return totalSqft >0  ? true : false;
                case "STAIR NOSING OVER CONCRETE":
                    return totalSqft > 0 || RequireFlashing == true ? true : false;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            return false;
            
        }

        public override void setCheckBoxes()
        {
            //base.setCheckBoxes();
            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name== "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL"||
                    item.Name== "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS"||
                    item.Name== "7013 SC BASE COAT/ 5 GAL PAILS 30 MILS"||
                    item.Name== "INTEGRAL STAIR NOSING (EXCEL STYLE)"||item.Name== "DETAIL TAPE (NEW PLYWOOD)"
                    ||item.Name== "SIKA 1-A CAULKING (NEW PLYWOOD)")
                {
                    item.IsMaterialChecked = getCheckboxCheckStatus(item.Name);
                }
                
            }


        }
        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)":
                    
                case "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH) UPI 7013 SC BASE COAT":
                case "9801 ACCELERATOR":
                case "EXTRA STAIR NOSING":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "Route and caulk moving cracks (greater than 1/8\")":
                case "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC":
                    return 0;
                case "3 IN. WHITE GLASS TAPE (PERIMETER)":
                case "SIKA 1-A CAULKING (PERIMETER)":
                    return deckPerimeter + riserCount * 2 * 2;
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                    return TotalSqftPlywood / 32 * 12 + riserCount * stairWidth * 2;
                case "UI 7118 CONCRETE PRIMER 2 gal kit":
                    return totalSqft + riserCount * stairWidth * 2;
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    return riserCount;
                default:
                    return TotalSqftPlywood + totalSqft + riserCount * stairWidth * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)":
                    return 0;
                case "7016 - AR - SC INTERMEDIATE/ 5 GAL PAILS 20 MILS":
                case "7016 SC TOP COAT/ 5 GAL PAILS 16 MILS":
                    return lfArea / coverage;
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    double locVal = 0;
                    locVal = 4 * lfArea;
                    return locVal;
                
                default:
                    return coverage==0?0:lfArea / coverage;
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
                if (item.Name == "7013 SC BASE COAT/ 5 GAL PAILS 30 MILS" ||
                    item.Name == "7016 - AR - SC INTERMEDIATE/ 5 GAL PAILS 20 MILS" ||
                    item.Name == "7016 SC TOP COAT/ 5 GAL PAILS 16 MILS" ||
                    item.Name== "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC")
                {
                    if (item.IsMaterialChecked)
                    {
                        qty = qty + item.Qty;
                    }
                    
                }
            }
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "9801 ACCELERATOR").FirstOrDefault();

            sysmat.Qty = sysmat.Coverage==0?0: qty / sysmat.Coverage;
            sysmat = SystemMaterials.Where(x => x.Name == "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)").FirstOrDefault();
            if (sysmat!=null)
            {
                //double myVal = 0;
                sysmat.SMUnits = riserCount.ToString();
                //double.TryParse(sysmat.SMUnits,out myVal);
                sysmat.Qty = sysmat.Coverage==0?0: riserCount / sysmat.Coverage;
            }
            //CalculateLaborMinCharge(false);
        }
        public override void CalculateTotalSqFt()
        {
            if ((totalSqft + TotalSqftPlywood ) == 0)
            {
                CostperSqftSlope = 0;
                CostperSqftMetal = 0;
                CostperSqftMaterial = 0;
                CostperSqftSubContract = 0;
            }
            else
            {
                CostperSqftSlope = TotalSlopingPrice / (totalSqft + TotalSqftPlywood);
                CostperSqftMetal = TotalMetalPrice / (totalSqft + TotalSqftPlywood);
                CostperSqftMaterial = TotalSystemPrice / (totalSqft + TotalSqftPlywood);
                CostperSqftSubContract = TotalSubcontractLabor / (totalSqft + TotalSqftPlywood);
            }
            TotalCostperSqft = CostperSqftSlope + CostperSqftMetal + CostperSqftMaterial + CostperSqftSubContract;
            RaisePropertyChanged("CostperSqftSlope");
            RaisePropertyChanged("CostperSqftMetal");
            RaisePropertyChanged("CostperSqftMaterial");
            RaisePropertyChanged("CostperSqftSubContract");
            RaisePropertyChanged("TotalCostperSqft");
        }

        public override void setExceptionValues(object s)
        {
            //base.setExceptionValues();
            SystemMaterial item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
            if (item != null)
            {
                item.SMSqftH = item.Qty * 32;

                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, 0, 0);

                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / item.Qty;

            }
            item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
            if (item != null)
            {
                item.SMSqftH = item.Qty;

                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, 0, 0);

                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / item.Qty;

            }

            item = SystemMaterials.Where(x => x.Name == "EXTRA STAIR NOSING").FirstOrDefault();
            if (item != null)
            {
                item.StairSqft = item.Qty;

                item.Hours = CalculateHrs(0, 0, item.StairSqft, item.StairsProductionRate);

                item.LaborExtension = item.Hours == 0 ? 0 : item.Hours > item.SetupMinCharge ? item.Hours * laborRate : item.SetupMinCharge * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (TotalSqftPlywood + totalSqft + riserCount);

            }
            calculateRLqty();
           
        }
        public override void calculateLaborHrs()
        {
            calLaborHrs(6, totalSqft + TotalSqftPlywood);

        }
        public override double getSqFtAreaH(string materialName)
        {
            //return base.getSqFtAreaH(materialName);
            switch (materialName)
            {
                case "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL":
                case "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS":
                case "7013 SC BASE COAT/ 5 GAL PAILS 30 MILS":
                case "7016 - AR - SC INTERMEDIATE/ 5 GAL PAILS 20 MILS":
                case "7016 SC TOP COAT/ 5 GAL PAILS 16 MILS":
                
                    return totalSqft + TotalSqftPlywood;
                case "UI 7118 CONCRETE PRIMER 2 gal kit":
                    return totalSqft;
                case "3 IN. WHITE GLASS TAPE (PERIMETER)":
                    return deckPerimeter;
                case "DETAIL TAPE (NEW PLYWOOD)":
                    return TotalSqftPlywood / 32 * 12;
                default:
                    return 0.0000001;
            }
        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)":
                case "INTEGRAL STAIR NOSING (EXCEL STYLE)":
                    return riserCount * stairWidth;
                case "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL":
                case "INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS":
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "Striping for small cracKs (less than 1/8\")":
                case "Route and caulk moving cracks (greater than 1/8\")":
                case "UI 7118 CONCRETE PRIMER 2 gal kit":
                    return riserCount * stairWidth * 2;
                case "7013 SC BASE COAT/ 5 GAL PAILS 30 MILS":
                case "7016 - AR - SC INTERMEDIATE/ 5 GAL PAILS 20 MILS":
                case "7016 SC TOP COAT/ 5 GAL PAILS 16 MILS":
                case "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC":
                    return riserCount;
                case "3 IN. WHITE GLASS TAPE (PERIMETER)":
                    return riserCount * 2 * 2;              
                
                default:
                    return 0;
            }
        }
    }
}
