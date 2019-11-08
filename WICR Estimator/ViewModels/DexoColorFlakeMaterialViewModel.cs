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
    class DexoColorFlakeMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        
        private double linearCoping;
        public DexoColorFlakeMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);

        }

        private void FillMaterialList()
        {

            materialNames.Add("MURIATIC ACID WASH", "GAL");
            materialNames.Add("CALCIUM CHLORIDE TEST (MINIMUM OF 3 FOR EACH JOB)", "KIT(EACH)");
            materialNames.Add("Dustless grind", "SQFT");
            materialNames.Add("EC-12 Epoxy Primer", "1.5 GAL KIT");
            materialNames.Add("EC-34 Epoxy 3 Gal", "3 GAL KIT");
            materialNames.Add("EC-95 Clear Polyurethane top coat", "1 GAL KIT");
            materialNames.Add("TC-60 COLOR CHIPS 10 sq ft per lb.", "LBS");
            materialNames.Add("A-81 underlayment for coved base (non-wet areas)", "75# regular gray");
            materialNames.Add("BASE COAT & BROADCAST SAND", "");
            materialNames.Add("TOP COAT", "");
            materialNames.Add("ADD LABOR FOR COVE BASE", "");
            materialNames.Add("1 COAT (6 MILS) OF VC 200 PRIMER IN LIEU OF BONDCOAT (FOR HYDROSTATIC up to 10 lbs.)", "2.5 GAL KIT");

        }

        public override string GetOperation(string matName)
        {
            switch (matName)
            {
                case "MURIATIC ACID WASH":
                    return "ROLL";
                case "CALCIUM CHLORIDE TEST (MINIMUM OF 3 FOR EACH JOB)":
                    return "EACH";
                case "Dustless grind":
                    return "SQFT";
                case "EC-12 Epoxy Primer":
                    return "SQUEEGE";
                case "BASE COAT & BROADCAST SAND":
                    return "ROLL & BROADCAST";
                case "TOP COAT":
                    return "ROLL";
                case "ADD LABOR FOR COVE BASE":
                    return "ALL COATS";
                case "1 COAT (6 MILS) OF VC 200 PRIMER IN LIEU OF BONDCOAT (FOR HYDROSTATIC up to 10 lbs.)":
                    return "ROLL ONE COAT";
                default:
                    return "";
            }
        }
        public override double getSqFtAreaH(string materialName)
        {

            switch (materialName)

            {               
                case "ADD LABOR FOR COVE BASE":
                    return 0;
                default:
                    return totalSqft;

            }
        }
        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "ADD LABOR FOR COVE BASE":
                    return 0;
                default:
                    return riserCount*stairWidth*2;
            }

        }
        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "ADD LABOR FOR COVE BASE":           
                case "1 COAT (6 MILS) OF VC 200 PRIMER IN LIEU OF BONDCOAT (FOR HYDROSTATIC up to 10 lbs.)":
                    return linearCoping;
                default:
                    return 0;
            }
        }

        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName)
        {
            if (matName == "Dustless grind")
            {
                SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == matName).FirstOrDefault();
                if (sysMat!=null)
                {
                    sysMat.LaborUnitPrice = 0.25;

                }

                return totalSqft * 0.25;
            }
            else
            {
                if (calhrs == 0)
                {
                    return 0;
                }
                else
                    return setupMin > calhrs ? setupMin * laborRate : calhrs * laborRate;
            }
            

        }
        public override void calculateLaborHrs()
        {
            calLaborHrs(6, totalSqft); ;
        }
        
        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0,
            double sqftStairs = 0, string materialName = "")
        {
            //return base.getLaborUnitPrice(laborExtension, riserCount, totalSqft);
            if (materialName == "Dustless grind")
            {
                return 0.25;
            }
            else
                return laborExtension / (sqftVert + sqftHor + riserCount);


        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                
                linearCoping = js.DeckPerimeter;
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();
            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "CALCIUM CHLORIDE TEST (MINIMUM OF 3 FOR EACH JOB)")
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
                    SystemMaterials[i] = sysMat[i];
                    if (iscbEnabled)
                    {
                        SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                        SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    }
                    SystemMaterials[i].SpecialMaterialPricing = sp;
                    
                    if (SystemMaterials[i].Name == "CALCIUM CHLORIDE TEST (MINIMUM OF 3 FOR EACH JOB)")
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

            CalculateCost(null);
        }
        public override bool IncludedInLaborMin(string matName)
        {
            return true;
        }

        public override void setCheckBoxes()
        {
           SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "1 COAT (6 MILS) OF VC 200 PRIMER IN LIEU OF BONDCOAT (FOR HYDROSTATIC up to 10 lbs.)").FirstOrDefault();
           bool ischecked = sysmat.IsMaterialChecked;
            SystemMaterials.Where(x => x.Name == "EC-12 Epoxy Primer").FirstOrDefault().IsMaterialChecked = !ischecked;
        }



        public override double getlfArea(string materialName)
        {

            switch (materialName)
            {
                case "CALCIUM CHLORIDE TEST (MINIMUM OF 3 FOR EACH JOB)":
                    return 1;
                case "A-81 underlayment for coved base (non-wet areas)":

                    return linearCoping*0.5;
               
                default:
                    return riserCount*stairWidth*2+linearCoping*0.5+totalSqft;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            //return base.getQuantity(materialName, coverage, lfArea);
            switch (materialName)
            {
                case "EC-95 Clear Polyurethane top coat":
                case "1 COAT (6 MILS) OF VC 200 PRIMER IN LIEU OF BONDCOAT (FOR HYDROSTATIC up to 10 lbs.)":
                    return Math.Ceiling(lfArea/coverage);

                default:
                    return coverage==0 ? 0:lfArea / coverage;
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
                case "MURIATIC ACID WASH":
                case "CALCIUM CHLORIDE TEST (MINIMUM OF 3 FOR EACH JOB)":
                case "EC-12 Epoxy Primer":
                    return false;
                case "A-81 underlayment for coved base (non-wet areas)":
                    return linearCoping > 0 ? true : false;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            //return base.getCheckboxEnabledStatus(materialName);
            switch (materialName)

            {
                case "MURIATIC ACID WASH":
                case "1 COAT (6 MILS) OF VC 200 PRIMER IN LIEU OF BONDCOAT (FOR HYDROSTATIC up to 10 lbs.)":

                    return true;
                default:
                    return false;
            }
        }

        public override void setExceptionValues(object s)
        {
            if (SystemMaterials.Count != 0)
            {

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "CALCIUM CHLORIDE TEST (MINIMUM OF 3 FOR EACH JOB)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate, item.SMSqftV, item.VerticalProductionRate);

                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
            }
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            SystemMaterial sysmat = null;
            bool ischecked = false;
            if (obj.ToString() == "1 COAT (6 MILS) OF VC 200 PRIMER IN LIEU OF BONDCOAT (FOR HYDROSTATIC up to 10 lbs.)")
            {
                sysmat = SystemMaterials.Where(x => x.Name == "1 COAT (6 MILS) OF VC 200 PRIMER IN LIEU OF BONDCOAT (FOR HYDROSTATIC up to 10 lbs.)").FirstOrDefault();
                ischecked = sysmat.IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "EC-12 Epoxy Primer").FirstOrDefault().IsMaterialChecked = !ischecked;
               
            }
            
            calculateRLqty();
            //CalculateLaborMinCharge(false);
        }
        public override void calculateRLqty()
        {
            
        }
    }
}
