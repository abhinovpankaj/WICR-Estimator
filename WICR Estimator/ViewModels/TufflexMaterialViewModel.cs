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
    class TufflexMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double TotalSqftPlywood = 0;
        
        private bool? IsNewPlaywood;
        
        public TufflexMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            TotalSqftPlywood = Js.TotalSqftPlywood;
            
            IsNewPlaywood = Js.IsNewPlywood;
            FillMaterialList();

            FetchMaterialValuesAsync(false);

        }
        public override void UpdateSumOfSqft()
        {
            double sumVal =  totalSqft+TotalSqftPlywood;
            TotalLaborUnitPrice = sumVal == 0 ? 0 : TotalLaborWithoutDrive / sumVal;
            RaisePropertyChanged("TotalLaborUnitPrice");
        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Extra stair nosing lf" || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
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


                    if (iscbEnabled)
                    {
                        //SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);
                        //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                        //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    }
                    if (SystemMaterials[i].Name == "Extra stair nosing lf" || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                        SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            //SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];
                            SystemMaterials[i].UpdateQuantity(qtyList[SystemMaterials[i].Name]);

                        }
                    }
                    SystemMaterials[i].UpdateSpecialPricing(sp);
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
       
        private void FillMaterialList()
        {
            materialNames.Add("Prep to remove existing urethane", "");
            materialNames.Add("Tuff Poxy #3 Primer", "3gal kit");
            materialNames.Add("2.5 Galvanized Metal Lathe (18 sq ft)", "17 SQ FT SHT.");
            materialNames.Add("Staples (3/4\" crown, Box of 13,500)", "5 GAL");
            materialNames.Add("Tufflex Solvent Free \"Tuff\" Base Coat", "5 GAL");
            materialNames.Add("1/20 Mesh Sand", "#50 BAG");
            materialNames.Add("Tufflex Solvent Free \"Tuff\" Intermediate Coat", "150 SF ROLL");
            materialNames.Add("1/20 Mesh Sand Broadcast to Refusal", "#50 BAG");
            materialNames.Add("Elasta-Tuff #6000-AL-SC Top Coat", "5 GAL PAIL");
            materialNames.Add("Stair Nosing", "LF");
            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");
        }

        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js != null)
            {
                TotalSqftPlywood = Js.TotalSqftPlywood;                
                IsNewPlaywood = Js.IsNewPlywood;
                
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "Prep to remove existing urethane":
                case "Tuff Poxy #3 Primer":
                    return false;
                case "Stair Nosing":
                    return riserCount > 0 ;
                default:
                    return riserCount+TotalSqftPlywood>0?true:false;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "Stair Nosing":
                case "Prep to remove existing urethane":
                case "Tuff Poxy #3 Primer":
                    return true;
                default:
                    return false;
            }

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Stair Nosing":
                    return riserCount ;
                
                default:
                    return TotalSqftPlywood + riserCount * stairWidth * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Stair Nosing":
                    return riserCount*stairWidth;
                default:
                    return coverage== 0 ? 0:lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "Stair Nosing":
                case "Extra stair nosing lf":
                case "Staples (3/4\" crown, Box of 13,500)":
                case "1/20 Mesh Sand":
                case "1/20 Mesh Sand Broadcast to Refusal":
                    return 0;
                default:
                    return TotalSqftPlywood;
            }

        }
        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "Staples (3/4\" crown, Box of 13,500)":
                case "1/20 Mesh Sand":
                case "1/20 Mesh Sand Broadcast to Refusal":
                //case "Elasta-Tuff #6000-AL-SC Top Coat":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Stair Nosing":
                    return riserCount * stairWidth;
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

        }

        public override bool canApply(object obj)
        {
            return true;
        }
        public override void setExceptionValues(object s)
        {
            if (SystemMaterials.Count != 0)
            {

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "Extra stair nosing lf").FirstOrDefault();
                if (item != null)
                {

                    item.StairSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                    
                }

                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty * 32;
                    //item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.MaterialExtension = item.SpecialMaterialPricing == 0 ? item.Qty * item.MaterialPrice : item.Qty * item.SpecialMaterialPricing;
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
            }
           // CalculateLaborMinCharge(false);
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
            lastCheckedMat = obj.ToString();
            if (obj.ToString() == "Stair Nosing")
            {
                var material = SystemMaterials.FirstOrDefault(x => x.Name == "Stair Nosing");
                if (material != null)
                {
                    stairNosingCheckValue = material.IsMaterialChecked;
                }

            }
            calculateRLqty();
            //CalculateLaborMinCharge(false);
        }
        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {
            if (calhrs==0)
            {
                return 0;
            }
            else
            {
                if (matName== "Stair Nosing")
                {
                    return (setupMin + calhrs) * laborRate;
                }
                else
                    return setupMin > calhrs ? setupMin * laborRate : calhrs * laborRate;
            }
                
             
        }
        public override void CalculateCostPerSqFT()
        {
            CostPerSquareFeet = (TotalSqftPlywood + deckCount) == 0 ? 0 : Math.Round(TotalMaterialCost / (TotalSqftPlywood + deckCount), 2);
            //base.CalculateCostPerSqFT();
        }
        public override void CalculateTotalSqFt()
        {
            CostperSqftSlope = (TotalSqftPlywood + riserCount)==0?0:TotalSlopingPrice / (TotalSqftPlywood + riserCount);
            CostperSqftMetal = (TotalSqftPlywood + riserCount) == 0 ? 0 : TotalMetalPrice / (TotalSqftPlywood + riserCount);
            CostperSqftMaterial = (TotalSqftPlywood + riserCount) == 0 ? 0 : TotalSystemPrice / (TotalSqftPlywood + riserCount);
            CostperSqftSubContract = (TotalSqftPlywood + riserCount) == 0 ? 0 : TotalSubcontractLabor / (TotalSqftPlywood + riserCount);
            TotalCostperSqft = CostperSqftSlope + CostperSqftMetal + CostperSqftMaterial + CostperSqftSubContract;
            RaisePropertyChanged("CostperSqftSlope");
            RaisePropertyChanged("CostperSqftMetal");
            RaisePropertyChanged("CostperSqftMaterial");
            RaisePropertyChanged("CostperSqftSubContract");
            RaisePropertyChanged("TotalCostperSqft");
        }
        public override void setCheckBoxes()
        {

        }
    }
}
