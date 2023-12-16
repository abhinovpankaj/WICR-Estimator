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
    class DeneefMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double totalVerticalSqft;
        private double linearCoping;
        public DeneefMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);            
        }

        private void FillMaterialList()
        {

            materialNames.Add("Supplies (veg. oil, pump wash, rub gloves, tyvex suit, sheeting, goggles, buckets)", "");
            materialNames.Add("1/2 inch drill bits", "EACH");
            materialNames.Add("DeNeef Flex catalist", "CAN");
            materialNames.Add("DeNeef Flex LV/SLV urethane resin", "PAIL");
            materialNames.Add("1/2 inch yellow packers (3000 psi)", "EACH");
            materialNames.Add("Labor to drill holes and install packers", "HOURS");
            
            materialNames.Add("Inject resin", "HOURS");
            
            materialNames.Add("Add labor for additional injection material (enter quantity of pails)", "");
            materialNames.Add("Add material/labor for patch and plug holes/EA HOLE", "");
            materialNames.Add("set up and clean up", "HOURS");
              
        }
        public override double getSqFtStairs(string materialName)
        {
            return 0;
        }
        public override void UpdateSumOfSqft()
        {
            double sumVal = totalSqft  + totalVerticalSqft;
            TotalLaborUnitPrice = sumVal == 0 ? 0 : TotalLaborWithoutDrive / sumVal;
            RaisePropertyChanged("TotalLaborUnitPrice");
        }
        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)

            {
                case "Inject resin":
                case "Labor to drill holes and install packers":
                    return totalSqft + totalVerticalSqft;
                case "Add material/labor for patch and plug holes/EA HOLE":
                    return (totalSqft + totalVerticalSqft)*0.1;
                case "set up and clean up":
                    return (totalSqft + totalVerticalSqft) * 2;
                default:
                    return 0;
            }
        }
        public override double getSqFtAreaH(string materialName)
        {

            switch (materialName)

            {
                case "Supplies (veg. oil, pump wash, rub gloves, tyvex suit, sheeting, goggles, buckets)":
                case "1/2 inch drill bits":
                case "DeNeef Flex catalist":
                case "DeNeef Flex LV/SLV urethane resin":
                case "1/2 inch yellow packers (3000 psi)":
                case "Add labor for additional injection material (enter quantity of pails)":
                    return 0;
                case "Inject resin":
                case "Labor to drill holes and install packers":
                    return linearCoping;
                case "Add material/labor for patch and plug holes/EA HOLE":
                    return linearCoping / 1.5;
                case "set up and clean up":
                    return linearCoping*2;
                default:
                    return 0;

            }
        }
        public override double CalculateLabrExtn(double calhrs, double setupMin,string matName)
        {
            return base.CalculateLabrExtn(calhrs, setupMin);

        }
        public override void calculateLaborHrs()
        {
            calLaborHrs(6,totalSqft); ;
        }
        
        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft,double sqftVert=0,double sqftHor=0,
            double sqftStairs=0,string materialName="")
        {
            //return base.getLaborUnitPrice(laborExtension, riserCount, totalSqft);
            
            return laborExtension / (sqftVert + sqftHor + riserCount);


        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js!=null)
            {
                totalVerticalSqft = js.TotalSqftVertical;
                linearCoping = js.DeckPerimeter;
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();
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


                    if (iscbEnabled)
                    {
                        //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                        //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                        SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);

                    }

                    SystemMaterials[i].UpdateSpecialPricing(sp);
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
            getEKLQnty();
            CalculateCost(null);
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
        }
        public override bool IncludedInLaborMin(string matName)
        {
            return true;
        }
        

        

        public override double getlfArea(string materialName)
        {
            
            switch (materialName)
            {
                case "Supplies (veg. oil, pump wash, rub gloves, tyvex suit, sheeting, goggles, buckets)":
                case "Labor to drill holes and install packers":
                case "Inject resin":
                case "Add labor for additional injection material (enter quantity of pails)":
                case "set up and clean up":
                    return 0;
                case "1/2 inch yellow packers (3000 psi)":
                    return totalSqft + totalVerticalSqft + linearCoping / 8 * 2;
                case "Add material/labor for patch and plug holes/EA HOLE":
                    return (totalSqft + totalVerticalSqft) * 0.1 + linearCoping / 1.5;
                default:
                    return totalSqft + totalVerticalSqft + linearCoping * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            //return base.getQuantity(materialName, coverage, lfArea);
            switch (materialName)
            {
                case "Supplies (veg. oil, pump wash, rub gloves, tyvex suit, sheeting, goggles, buckets)":
                    return coverage;
                case "DeNeef Flex LV/SLV urethane resin":
                    return coverage == 0 ? 0 : Math.Ceiling(lfArea / coverage);
                default:
                    return coverage==0 ? 0 : lfArea / coverage;
            }
        }

        private void getEKLQnty()
        {
            double qty = 0;
            foreach (var item in SystemMaterials)
            {
                if (item.Name == "DeNeef Flex LV/SLV urethane resin")
                {
                    if (item.IsMaterialChecked)
                    {
                        qty = item.Qty;
                    }
                    break;
                }

            }
            SystemMaterials.Where(x=>x.Name== "DeNeef Flex catalist").FirstOrDefault().Qty= Math.Ceiling(qty *4);
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "Inject resin").FirstOrDefault();
            if (sysmat!=null)
            {
                SystemMaterial mat = SystemMaterials.Where(x => x.Name == "Add labor for additional injection material (enter quantity of pails)").FirstOrDefault();
                bool ischecked = mat.IsMaterialChecked;
                mat.Hours =sysmat.LaborExtension*0.3 /25;
                mat.LaborExtension = mat.Hours== 0 ? 0:mat.Hours >= mat.SetupMinCharge ? mat.Hours * laborRate : mat.SetupMinCharge * laborRate;
                mat.LaborUnitPrice = mat.LaborExtension / (totalSqft+totalVerticalSqft+riserCount);
                mat.IsMaterialChecked = ischecked;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "Add material/labor for patch and plug holes/EA HOLE").FirstOrDefault();
            if (sysmat != null)
            {
                double vertSqft = (totalSqft + totalVerticalSqft) * 0.1;
                double horSqft = linearCoping / 1.5;
                sysmat.Hours = sysmat.IsMaterialChecked ? (vertSqft+horSqft) * 0.5 / 25 : 0;
                sysmat.LaborExtension = sysmat.Hours == 0 ? 0 : sysmat.Hours >= sysmat.SetupMinCharge ? sysmat.Hours * laborRate : sysmat.SetupMinCharge * laborRate;
                sysmat.LaborUnitPrice = sysmat.LaborExtension / (totalSqft + totalVerticalSqft + riserCount);
            }
        }
        public override bool canApply(object obj)
        {
            return true;
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Add labor for additional injection material (enter quantity of pails)":
                case "Add material/labor for patch and plug holes/EA HOLE":
                
                    return false;
                case "Inject resin":
                    return totalVerticalSqft + totalSqft + riserCount +linearCoping> 0 ? true : false;
                default:
                    return  true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            //return base.getCheckboxEnabledStatus(materialName);
            switch (materialName)
                
            {
                case "Add labor for additional injection material (enter quantity of pails)":
                case "Add material/labor for patch and plug holes/EA HOLE":
                
                    return true;
                default:
                    return false;
            }
        }
        public override void CalculateCostPerSqFT()
        {
            CostPerSquareFeet = (totalSqft  + totalVerticalSqft + riserCount) == 0 ? 0 : Math.Round(TotalMaterialCost / (totalSqft  + totalVerticalSqft + riserCount), 2);
        }
        public override void setExceptionValues(object s)
        {
            
        }

        public override void ApplyCheckUnchecks(object obj)
        {            
            lastCheckedMat = obj.ToString();
            getEKLQnty();         
        }
        public override void CalculateTotalSqFt()
        {
            if ((totalSqft + totalVerticalSqft + linearCoping) == 0)
            {
                CostperSqftSlope = 0;
                CostperSqftMetal = 0;
                CostperSqftMaterial = 0;
                CostperSqftSubContract = 0;
            }
            else
            {
                CostperSqftSlope = TotalSlopingPrice / (totalSqft + totalVerticalSqft + linearCoping);
                CostperSqftMetal = TotalMetalPrice / (totalSqft + totalVerticalSqft + linearCoping);
                CostperSqftMaterial = TotalSystemPrice / (totalSqft + totalVerticalSqft + linearCoping);
                CostperSqftSubContract = TotalSubcontractLabor / (totalSqft + totalVerticalSqft + linearCoping);
            }
            TotalCostperSqft = CostperSqftSlope + CostperSqftMetal + CostperSqftMaterial + CostperSqftSubContract;
            RaisePropertyChanged("CostperSqftSlope");
            RaisePropertyChanged("CostperSqftMetal");
            RaisePropertyChanged("CostperSqftMaterial");
            RaisePropertyChanged("CostperSqftSubContract");
            RaisePropertyChanged("TotalCostperSqft");
        }

    }
}
