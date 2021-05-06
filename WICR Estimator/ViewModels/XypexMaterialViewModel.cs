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
    class XypexMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double totalVerticalSqft;
        private double linearCoping;
        public XypexMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);

        }
        public override void UpdateSumOfSqft()
        {
            double sumVal = totalSqft + totalVerticalSqft;
            TotalLaborUnitPrice = sumVal == 0 ? 0 : TotalLaborWithoutDrive / sumVal;
            RaisePropertyChanged("TotalLaborUnitPrice");
        }
        private void FillMaterialList()
        {

            materialNames.Add("Add for Xypex Concentrate material", "#60 PAIL");
            materialNames.Add("Add for Xypex patch and plug material for cant", "PAIL");
            materialNames.Add("Add for Xypex Mega Mix 1 (2nd coat)", "PAIL");
            materialNames.Add("Add for Xypex modified (2nd coat)", "PAIL");
            materialNames.Add("Add for penetrations  -customer to determine qty", "");
            materialNames.Add("Add labor for 2 coats hopper spray or brush", "SQFT");

            materialNames.Add("Add for 1 part admix and 2 parts water", "1 GAL");

            materialNames.Add("Add for Gamma Cure in lieu of wet cure", "1 GAL");
            

        }
        public override double getSqFtAreaH(string materialName)
        {

            switch (materialName)

            {

                case "Add for Xypex Concentrate material":
                    return totalSqft;
                case "Add for Xypex Mega Mix 1 (2nd coat)":
                case "Add for Xypex modified (2nd coat)":
                case "Add for Gamma Cure in lieu of wet cure":
                    return totalSqft;
                case "Add for 1 part admix and 2 parts water":
                    return linearCoping;
                default:
                    return 0;

            }
        }
        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "Add for 1 part admix and 2 parts water":
                    return riserCount * 2 * 2;
                default:
                    return 0;
            }
            
        }
        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "Add for Xypex patch and plug material for cant":
                case "Add for Xypex modified (2nd coat)":
                    return deckPerimeter;
                default:
                    return totalVerticalSqft;
            }
            
        }
        
        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName)
        {
            if (calhrs == 0)
            {
                return 0;
            }
            else
                return setupMin > calhrs ? setupMin * laborRate : calhrs * laborRate;
            

        }
        public override void calculateLaborHrs()
        {
            calLaborHrs(6, totalSqft); ;
        }
        
        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0,
            double sqftStairs = 0, string materialName = "")
        {
            //return base.getLaborUnitPrice(laborExtension, riserCount, totalSqft);
            
            return (totalSqft + totalVerticalSqft + riserCount) == 0 ? 0 : laborExtension / (totalSqft + totalVerticalSqft + riserCount);


        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                totalVerticalSqft = js.TotalSqftVertical;
                linearCoping = js.DeckPerimeter;
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();
            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Add for penetrations  -customer to determine qty")
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
                    //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    UpdateMe(sysMat[i]);

                    SystemMaterials[i].UpdateSpecialPricing(sp);
                    SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);

                    if (SystemMaterials[i].Name == "Add for penetrations  -customer to determine qty")
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
                case "Add for penetrations  -customer to determine qty":
                case "Add labor for 2 coats hopper spray or brush":
                
                    return 0;
                case "Add for Xypex patch and plug material for cant":
                    return  linearCoping *1.1;
                case "Add for Gamma Cure in lieu of wet cure":
                    return (totalSqft + totalVerticalSqft) ;
                default:
                    return (totalSqft + totalVerticalSqft)* 1.1;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            //return base.getQuantity(materialName, coverage, lfArea);
            switch (materialName)
            {
                case "Add for penetrations  -customer to determine qty":
                    return 0;
               
                default:
                    return coverage==0 ? 0 : lfArea / coverage;
            }
        }

        private void getEKLQnty()
        {

            SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == "Add for Gamma Cure in lieu of wet cure").FirstOrDefault();
            if (sysMat!=null)
            {
                sysMat.Hours = sysMat.IsMaterialChecked ? (linearCoping + totalVerticalSqft) * 0.2 / 25:0;
                sysMat.LaborExtension = sysMat.Hours == 0 ? 0 : sysMat.Hours > sysMat.SetupMinCharge ? sysMat.Hours * laborRate : sysMat.SetupMinCharge * laborRate;

                sysMat.LaborUnitPrice = (totalSqft + totalVerticalSqft + riserCount)==0 ? 0:sysMat.LaborExtension / (totalSqft + totalVerticalSqft + riserCount);
            }
        }
        public override bool canApply(object obj)
        {
            return true;
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            return true;
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            //return base.getCheckboxEnabledStatus(materialName);
            switch (materialName)

            {
                case "Add for Xypex modified (2nd coat)":
                case "Add for Gamma Cure in lieu of wet cure":

                    return true;
                default:
                    return false;
            }
        }

        public override void setExceptionValues(object s)
        {
            //if (s!=null)
            //{
            //    SystemMaterial item = SystemMaterials.Where(x => x.Name == "Add for penetrations  -customer to determine qty").FirstOrDefault();
            //    if (item != null)
            //    {
            //        item.MaterialExtension =item.Qty * item.MaterialPrice;
            //        item. FreightExtension = item.Qty * item.Weight;
            //    }
            //}
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            
        }
        public override void CalculateCostPerSqFT()
        {
            CostPerSquareFeet = (totalSqft + totalVerticalSqft ) == 0 ? 0 : Math.Round(TotalMaterialCost / (totalSqft + totalVerticalSqft ), 2);
        }

        public override void CalculateTotalSqFt()
        {
            if ((totalSqft +  totalVerticalSqft) == 0)
            {
                CostperSqftSlope = 0;
                CostperSqftMetal = 0;
                CostperSqftMaterial = 0;
                CostperSqftSubContract = 0;
            }
            else
            {
                CostperSqftSlope = TotalSlopingPrice / (totalSqft + totalVerticalSqft );
                CostperSqftMetal = TotalMetalPrice / (totalSqft + totalVerticalSqft);
                CostperSqftMaterial = TotalSystemPrice / (totalSqft + totalVerticalSqft);
                CostperSqftSubContract = TotalSubcontractLabor / (totalSqft + totalVerticalSqft);
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
