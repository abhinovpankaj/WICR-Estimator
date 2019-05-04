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
            
            materialNames.Add("Add labor for additional injection material", "");
            materialNames.Add("Add material/labor for patch and plug holes/EA HOLE", "");
            materialNames.Add("set up and clean up", "HOURS");
              
        }
        public override double getSqFtStairs(string materialName)
        {
            return 0;
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
                case "Add labor for additional injection material":
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
        public override void CalculateLaborMinCharge()
        {
           //LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
           //                             x.IsMaterialChecked&&x.LaborExtension!=0).ToList().Select(x => x.Hours).Sum();
           // LaborMinChargeMinSetup = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
           //                              x.IsMaterialChecked&&x.LaborExtension!=0).ToList().Select(x => x.SetupMinCharge).Sum();

           // LaborMinChargeLaborExtension = LaborMinChargeMinSetup + LaborMinChargeHrs > 20 ? 0 :
                                                //(20 - LaborMinChargeMinSetup - LaborMinChargeHrs) * laborRate;
            base.CalculateLaborMinCharge();
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
                case "Supplies (veg. oil, pump wash, rub gloves, tyvex suit, sheeting, goggles, buckets)":
                case "Labor to drill holes and install packers":
                case "Inject resin":
                case "Add labor for additional injection material":
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
           
        }
        public override bool canApply(object obj)
        {
            return true;
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Add labor for additional injection material":
                case "Add material/labor for patch and plug holes/EA HOLE":
                
                    return false;
                case "Inject resin":
                    return totalVerticalSqft + totalSqft + riserCount > 0 ? true : false;
                default:
                    return  true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            //return base.getCheckboxEnabledStatus(materialName);
            switch (materialName)
                
            {
                case "Add labor for additional injection material":
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
            ////base.setExceptionValues();
            //if (SystemMaterials.Count != 0)
            //{
            //    SystemMaterial item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
            //    if (item != null)
            //    {
            //        item.SMSqftH = item.Qty;
            //        item.SMSqft = item.Qty;
            //        item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
            //        item.LaborExtension = item.Hours >= item.SetupMinCharge?item.Hours * laborRate:item.SetupMinCharge*laborRate;
            //        item.LaborUnitPrice = item.LaborExtension / (item.SMSqftH + item.SMSqftV+item.StairSqft);

            //    }
                
            //    CalculateLaborMinCharge();
            //}
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            ////base.ApplyCheckUnchecks(obj);

            //if (obj.ToString()== "ENDURO ELA-98 BINDER (2 COATS)")
            //{
            //    bool isChecked = SystemMaterials.Where(x => x.Name == "ENDURO ELA-98 BINDER (2 COATS)").FirstOrDefault().IsMaterialChecked;
            //    SystemMaterials.Where(x => x.Name == "3/4 oz. Fiberglass (2000 sq ft rolls Purchased from Hill Brothers )").FirstOrDefault().IsMaterialChecked = isChecked;
            //    if (!isChecked)
            //    {
            //        SystemMaterials.Where(x => x.Name == "Caulk, dymonic 100").FirstOrDefault().IsMaterialChecked = true;
            //    }
            //}
            //if (obj.ToString() == "2.5 Galvanized Lathe (18 s.f.) no less than 12 per sq ft.")
            //{
            //    bool isChecked = SystemMaterials.Where(x => x.Name == "2.5 Galvanized Lathe (18 s.f.) no less than 12 per sq ft.").FirstOrDefault().IsMaterialChecked;
            //    SystemMaterials.Where(x => x.Name == "Staples (3/4 Inch Crown, Box of 13,500)").FirstOrDefault().IsMaterialChecked = isChecked;                
            //}
            //getEKLQnty();
            ////update Add labor for minimum cost
            //CalculateLaborMinCharge();

        }
    }
}
