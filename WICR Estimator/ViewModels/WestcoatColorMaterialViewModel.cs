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
    public class WestcoatColorMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;

        public WestcoatColorMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);
        }

        private void FillMaterialList()
        {
            materialNames.Add("Sand or pressure wash to prepare area", "SQ FT");

            materialNames.Add("System Large cracks Repair", "SQ FT");
            materialNames.Add("System bubbled and failed texture repair", "SQ FT");
            materialNames.Add("Add for hand wash, hard, stains, other prep.  CLR or xylene", "QTS");
            materialNames.Add("Add for hand masking of smaller patios, (other than large pool decks) tape and masking", "SQ FT");
            materialNames.Add("SC-70 clear acrylic lacquer 200-300 sq ft per gallon", "5 GAL PAIL");
            materialNames.Add("SC-10 solid color sealer IN LIEU of SC-70", "5 GAL PAIL");
            materialNames.Add("Add for Safe Grip Additive", "32OZ");
            materialNames.Add("Concrete cracks greater than 1/32 inch (route 1/4 x 1/4) epoxy gel fill, ilica sand, and fiber tape", "EC-72 epoxy & tape");
            
        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            //base.FetchMaterialValuesAsync(hasSetupChanged);
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "System Large cracks Repair" || item.Name == "System bubbled and failed texture repair"||
                    item.Name== "Concrete cracks greater than 1/32 inch (route 1/4 x 1/4) epoxy gel fill, ilica sand, and fiber tape")
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
                    SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    if (SystemMaterials[i].Name == "System Large cracks Repair" || SystemMaterials[i].Name == "System bubbled and failed texture repair" ||
                        SystemMaterials[i].Name== "Concrete cracks greater than 1/32 inch (route 1/4 x 1/4) epoxy gel fill, ilica sand, and fiber tape")
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
            {
                SystemMaterials = sysMat;
                setCheckBoxes();
            }
                

            setExceptionValues(null);
            

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
        
        public override bool IncludedInLaborMin(string matName)
        {
            return true;
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Sand or pressure wash to prepare area":
                case "Add for hand wash, hard, stains, other prep.  CLR or xylene":
                case "Add for hand masking of smaller patios, (other than large pool decks) tape and masking":
                case "SC-10 solid color sealer IN LIEU of SC-70":
                case "Add for Safe Grip Additive":

                    return true;
                default:
                    return false;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                
                case "Add for hand wash, hard, stains, other prep.  CLR or xylene":
                case "Add for hand masking of smaller patios, (other than large pool decks) tape and masking":
                case "SC-10 solid color sealer IN LIEU of SC-70":
                case "Add for Safe Grip Additive":
                    return true;
                default:
                    return false;
            }
        }

        public override double getlfArea(string materialName)
        {
            //return base.getlfArea(materialName);
            switch (materialName)
            {
                case "Sand or pressure wash to prepare area":
                case "SC-10 solid color sealer IN LIEU of SC-70":
                case "SC-70 clear acrylic lacquer 200-300 sq ft per gallon":
                    return totalSqft + riserCount * (stairWidth * 2);

                case "Add for hand wash, hard, stains, other prep.  CLR or xylene":
                case "Add for hand masking of smaller patios, (other than large pool decks) tape and masking":
                    return totalSqft;

                default:
                    return 0;
            }
        }
        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            //return base.getQuantity(materialName, coverage, lfArea);
            switch (materialName)
            {
                case "Sand or pressure wash to prepare area":
                    return 0;
                
                case "Add for hand wash, hard, stains, other prep.  CLR or xylene":
                case "Add for hand masking of smaller patios, (other than large pool decks) tape and masking":
                case "SC-10 solid color sealer IN LIEU of SC-70":
                case "SC-70 clear acrylic lacquer 200-300 sq ft per gallon":
                    return lfArea/coverage;
                default:
                    return 0;
            }
        }

        

        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {
            switch (matName)
            {
                case "Sand or pressure wash to prepare area":
                case "Concrete cracks greater than 1/32 inch (route 1/4 x 1/4) epoxy gel fill, ilica sand, and fiber tape":
                    return calhrs==0 ?0:setupMin > calhrs ?setupMin * laborRate : calhrs * laborRate;
                default:
                    return base.CalculateLabrExtn(calhrs,setupMin,"") ;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            return totalSqft;
        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {

                case "Concrete cracks greater than 1/32 inch (route 1/4 x 1/4) epoxy gel fill, ilica sand, and fiber tape":
                    return 0.0000001;

                default:
                    return riserCount * stairWidth * 2;

            }
        }

        public override void calculateLaborHrs()
        {
            calLaborHrs(4,totalSqft);
            
        }
        //Calculate for "Add for Safe Grip Additive"
        public override void calculateRLqty()
        {
            //base.calculateRLqty();
            double val1=0, val2 = 0;
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "SC-70 clear acrylic lacquer 200-300 sq ft per gallon").FirstOrDefault();
            if (sysmat!=null)
            {
                val1 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "SC-10 solid color sealer IN LIEU of SC-70").FirstOrDefault();
            if (sysmat != null)
            {
                val2 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;

            }

            sysmat = SystemMaterials.Where(x => x.Name == "Add for Safe Grip Additive").FirstOrDefault();
            if (sysmat!=null)
            {
                bool ischecked = sysmat.IsMaterialChecked;
                sysmat.Qty = val1 + val2;
                sysmat.IsMaterialChecked = ischecked;
            }
            CalculateLaborMinCharge(false);
        }

        public override bool canApply(object obj)
        {
            return true;
        }
        public override void setExceptionValues(object s)
        {
            if (SystemMaterials.Count != 0)
            {
                SystemMaterial item = SystemMaterials.Where(x => x.Name == "System Large cracks Repair").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.MaterialExtension = item.Qty / item.Coverage * item.MaterialPrice;
                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }

                item = SystemMaterials.Where(x => x.Name == "System bubbled and failed texture repair").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.MaterialExtension = item.Qty / item.Coverage * item.MaterialPrice;
                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                }
                item = SystemMaterials.Where(x => x.Name == "Concrete cracks greater than 1/32 inch (route 1/4 x 1/4) epoxy gel fill, ilica sand, and fiber tape").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                }
            }
            calculateRLqty();
            //CalculateLaborMinCharge(false);
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            if (obj.ToString() == "SC-10 solid color sealer IN LIEU of SC-70")
            {
                SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "SC-70 clear acrylic lacquer 200-300 sq ft per gallon").FirstOrDefault();
                if (sysmat != null)
                {
                    sysmat.IsMaterialChecked = !SystemMaterials.Where(x => x.Name == "SC-10 solid color sealer IN LIEU of SC-70").FirstOrDefault().IsMaterialChecked;
                    
                    calculateRLqty();
                }

            }
            calculateRLqty();
           // CalculateLaborMinCharge(false);
        }

        public override void setCheckBoxes()
        {
            SystemMaterials.Where(x => x.Name == "SC-10 solid color sealer IN LIEU of SC-70").First().IsMaterialEnabled = true;
            SystemMaterials.Where(x => x.Name == "Sand or pressure wash to prepare area").First().IsMaterialChecked = true;
            //ApplyCheckUnchecks("SC-10 solid color sealer IN LIEU of SC-70");
        }
    }
}
