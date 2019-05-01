﻿using System;
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
                    return totalVerticalSqft +totalSqft;
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
        public override void CalculateLaborMinCharge()
        {
            LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
                                         x.IsMaterialChecked && x.LaborExtension != 0).ToList().Select(x => x.Hours).Sum();
            LaborMinChargeMinSetup = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
                                         x.IsMaterialChecked && x.LaborExtension != 0).ToList().Select(x => x.SetupMinCharge).Sum();

            LaborMinChargeLaborExtension = LaborMinChargeMinSetup + LaborMinChargeHrs > 20 ? 0 :
                                                (20 - LaborMinChargeMinSetup - LaborMinChargeHrs) * laborRate;
            base.CalculateLaborMinCharge();
        }
        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0,
            double sqftStairs = 0, string materialName = "")
        {
            //return base.getLaborUnitPrice(laborExtension, riserCount, totalSqft);

            return laborExtension / (sqftVert + sqftHor + riserCount);


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
                    if (SystemMaterials[i].Name == "Add for penetrations  -customer to determine qty")
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
            getEKLQnty();
            CalculateLaborMinCharge();
            CalculateAllMaterial();
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
            
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            
        }
        public override void CalculateCostPerSqFT()
        {
            CostPerSquareFeet = (totalSqft + totalVerticalSqft + riserCount) == 0 ? 0 : Math.Round(TotalMaterialCost / (totalSqft + totalVerticalSqft + riserCount), 2);
        }
    }
}