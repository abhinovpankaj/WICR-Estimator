﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    [DataContract]
    class ParasealMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double additionalTermBarLF;
        private double insideOutsideCornerDetails;
        private bool superStopFooting;
        private double pinsCoverage;
        private double linearFootage;
        public ParasealMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            
            FillMaterialList();

            FetchMaterialValuesAsync(false);
            pinsCoverage = SystemMaterials.Where(x => x.Name == "PINS & LOADS").FirstOrDefault().Coverage;
        }


        private void FillMaterialList()
        {
            materialNames.Add("PARATERM BAR LF (TOP ONLY- STANDARD INSTALL)", "LF");
            materialNames.Add("EXTRA PARATERM BAR LF (BOTTOM OR SIDES)", "LF");
            materialNames.Add("VULKEM 116 CAULK (FOR TERM BAR)", "TUBE");
            materialNames.Add("PARAMASTIC (1000 LF PER PAIL FOR PREP & TERMINATIONS)", "5 GAL PAIL");
            materialNames.Add("PARAGRANULAR (FOR CANT AT FOOTING)", "50 LB BAG");
            materialNames.Add("PARASEAL \"STANDARD\" ROLLS (4X24)", "96 SQ FT/ROLL");
            materialNames.Add("SEAM TAPE", "75 LF ROLL");
            materialNames.Add("PARAMASTIC AND PARASTICK AND DRY (FOR PENETRATIONS)", "EACH");
            materialNames.Add("INSIDE AND OUTSIDE CORNER DETAILS (PARASEAL)", "96 SQ FT/ROLL");
            materialNames.Add("SUPERSTOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT", "ROLL");
            materialNames.Add("PINS & LOADS", "EACH");
            materialNames.Add("PROTECTION MAT (HORIZONTAL ONLY)", "667 SF ROLL");
            materialNames.Add("PB-4 (VERTICAL ONLY)", "200 SF ROLL");
            materialNames.Add("TREMDRAIN 1000 (VERTICAL ONLY)", "200 SF ROLL");
            materialNames.Add("TREMDRAIN 1000 (HORIZONTAL ONLY)", "200 SF ROLL");
            materialNames.Add("UNIVERSAL OUTLET", "EACH");
            materialNames.Add("TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\"", "LINEAR FEET");
            materialNames.Add("UV PROTECTION DETAIL (PRIME AND COAT WITH VULKEM 801)", "LINEAR FEET");
            
        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "UNIVERSAL OUTLET" || item.Name == "TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\"" )
                {
                    qtyList.Add(item.Name, item.Qty);
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
                    if (iscbEnabled)
                    {
                        SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                        SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    }
                    
                    if (SystemMaterials[i].Name == "UNIVERSAL OUTLET" || SystemMaterials[i].Name == "TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\"")
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
        public override ObservableCollection<OtherItem> GetOtherMaterials()
        {
            ObservableCollection<OtherItem> om = new ObservableCollection<OtherItem>();
            om.Add(new OtherItem { Name = "Access issues?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional prep?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional labor?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Alternate material?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional Move ons?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Linear footage for seams if needed for submerged conditions", IsReadOnly = false });
            return om;
        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js != null)
            {
                additionalTermBarLF = Js.AdditionalTermBarLF;
                insideOutsideCornerDetails = Js.InsideOutsideCornerDetails;
                superStopFooting = Js.SuperStopAtFooting;
                linearFootage = Js.LinearCopingFootage;
                
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                //case "PROTECTION MAT (HORIZONTAL ONLY)":
                case "PB-4 (VERTICAL ONLY)":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                case "TREMDRAIN 1000 (HORIZONTAL ONLY)":
                case "TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\"":
                
                    return false;
                case "EXTRA PARATERM BAR LF (BOTTOM OR SIDES)":
                    return additionalTermBarLF>0 ? true : false;
                case "PARATERM BAR LF (TOP ONLY- STANDARD INSTALL)":
                case "PARAGRANULAR (FOR CANT AT FOOTING)":
                    return deckPerimeter > 0 ? true : false;
                case "PARAMASTIC AND PARASTICK AND DRY (FOR PENETRATIONS)":
                    return riserCount > 0 ? true : false;
                case "INSIDE AND OUTSIDE CORNER DETAILS (PARASEAL)":
                    return  insideOutsideCornerDetails> 0 ? true : false;
                case "SUPERSTOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT":
                    return superStopFooting;
                case "PINS & LOADS":
                    return totalSqft > 0 ? true : false;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "PARATERM BAR LF (TOP ONLY- STANDARD INSTALL)":
                case "PARAMASTIC AND PARASTICK AND DRY (FOR PENETRATIONS)":
                case "PINS & LOADS":
                case "PB-4 (VERTICAL ONLY)":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                case "PROTECTION MAT (HORIZONTAL ONLY)":
                case "TREMDRAIN 1000 (HORIZONTAL ONLY)":
                    return true;
                default:
                    return false;
            }

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "PARATERM BAR LF (TOP ONLY- STANDARD INSTALL)":
                case "PARAGRANULAR (FOR CANT AT FOOTING)":
                case "SUPERSTOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT":
                    return deckPerimeter;
                case "VULKEM 116 CAULK (FOR TERM BAR)":
                    return deckPerimeter + additionalTermBarLF;
                case "EXTRA PARATERM BAR LF (BOTTOM OR SIDES)":
                    return additionalTermBarLF;
                case "PARAMASTIC (1000 LF PER PAIL FOR PREP & TERMINATIONS)":
                case "PARASEAL \"STANDARD\" ROLLS (4X24)":
                case "PINS & LOADS":
                    return totalSqft + deckCount;
                case "PARAMASTIC AND PARASTICK AND DRY (FOR PENETRATIONS)":
                    return riserCount;
                case "INSIDE AND OUTSIDE CORNER DETAILS (PARASEAL)":
                    return insideOutsideCornerDetails /4;
                case "PB-4 (VERTICAL ONLY)":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                    return totalSqft;
                case "UV PROTECTION DETAIL (PRIME AND COAT WITH VULKEM 801)":
                   return 0;
                default:
                    return deckCount;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "EXTRA PARATERM BAR LF (BOTTOM OR SIDES)":
                    return additionalTermBarLF;
                case "UV PROTECTION DETAIL (PRIME AND COAT WITH VULKEM 801)":
                    return linearFootage;
                default:
                    return lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "PARATERM BAR LF (TOP ONLY- STANDARD INSTALL)":
                case "SUPERSTOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT":
                
                    return deckPerimeter;
                case "EXTRA PARATERM BAR LF (BOTTOM OR SIDES)":
                    return additionalTermBarLF;
                case "PARAMASTIC (1000 LF PER PAIL FOR PREP & TERMINATIONS)":
                case "PARASEAL \"STANDARD\" ROLLS (4X24)":
                case "PROTECTION MAT (HORIZONTAL ONLY)":
                case "TREMDRAIN 1000 (HORIZONTAL ONLY)":
                case "SEAM TAPE":
                    return deckCount;
                case "PARAMASTIC AND PARASTICK AND DRY (FOR PENETRATIONS)":
                    return riserCount;
                case "UV PROTECTION DETAIL (PRIME AND COAT WITH VULKEM 801)":
                    return linearFootage;
                default:
                    return 0;
            }

        }
        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "INSIDE AND OUTSIDE CORNER DETAILS (PARASEAL)":
                    return insideOutsideCornerDetails;
                case "PARAMASTIC(1000 LF PER PAIL FOR PREP & TERMINATIONS)":
                case "PARASEAL \"STANDARD\" ROLLS (4X24)":
                case "SEAM TAPE":
                case "PB-4 (VERTICAL ONLY)":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                    return totalSqft;
                case "PARAGRANULAR (FOR CANT AT FOOTING)":
                    return deckPerimeter;
                default:
                    return 0;
            }
        }
        public override double getSqFtStairs(string materialName)
        {
            return 0;            
        }

        public override void calculateLaborHrs()
        {
            calLaborHrs(10, totalSqft);

        }
        //calculate for Desert Crete
        public override void calculateRLqty()
        {
            double val1=0, val2=0;
            SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == "PARATERM BAR LF (TOP ONLY- STANDARD INSTALL)").FirstOrDefault();
            if (sysMat!=null)
            {
                val1 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "EXTRA PARATERM BAR LF (BOTTOM OR SIDES)").FirstOrDefault();
            if (sysMat != null)
            {
                val2 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "PINS & LOADS").FirstOrDefault();
            if (sysMat!=null)
            {
                bool ischecked;
                ischecked= sysMat.IsMaterialChecked;
                SystemMaterial sm=SystemMaterials.Where(x=>x.Name== "TREMDRAIN 1000 (VERTICAL ONLY)").FirstOrDefault();
                if (sm!=null)
                {
                    sysMat.Coverage = sm.IsMaterialChecked?  6 + sm.Qty * 200 / 500:6;
                }
                sysMat.Qty = sysMat.SMSqft / sysMat.Coverage + (val1 + val2) / 2;
                sysMat.IsMaterialChecked = ischecked;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "PARASEAL \"STANDARD\" ROLLS (4X24)").FirstOrDefault();
            if (sysMat!=null)
            {
                SystemMaterial myMat = SystemMaterials.Where(x => x.Name == "SEAM TAPE").FirstOrDefault();
                myMat.SMSqft = sysMat.Qty * 28;
                myMat.Qty = myMat.SMSqft / myMat.Coverage;
                OtherMaterials.Where(x => x.Name == "Linear footage for seams if needed for submerged conditions").FirstOrDefault().Quantity= Math.Round(sysMat.Qty * 28,2);
                OtherLaborMaterials.Where(x => x.Name == "Linear footage for seams if needed for submerged conditions").FirstOrDefault().LQuantity = Math.Round(sysMat.Qty * 28,2);
            }
        }

        public override bool canApply(object obj)
        {
            return true;
        }
        public override void setExceptionValues(object s)
        {
            if (SystemMaterials.Count != 0)
            {

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "UNIVERSAL OUTLET").FirstOrDefault();
                if (item != null)
                {

                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }

                item = SystemMaterials.Where(x => x.Name == "TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\"").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                
            }
            CalculateLaborMinCharge();
        }
        public override void CalculateLaborMinCharge()
        {
            LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
                                        x.IsMaterialChecked).ToList().Select(x => x.Hours).Sum();
            LaborMinChargeMinSetup = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
                                         x.IsMaterialChecked).ToList().Select(x => x.SetupMinCharge).Sum();
            LaborMinChargeLaborExtension = (LaborMinChargeMinSetup + LaborMinChargeHrs) > 20 ? 0 :
                                                (20 - LaborMinChargeMinSetup - LaborMinChargeHrs) * laborRate;
            base.CalculateLaborMinCharge();
        }
        public override bool IncludedInLaborMin(string matName)
        {
            return true;
        }
        public override void ApplyCheckUnchecks(object obj)
        {
            //SystemMaterial sysmat=null;
            //if (obj.ToString()== "PROTECTION MAT (HORIZONTAL ONLY)")
            //{
            //    SystemMaterials.Where(x => x.Name == "PROTECTION MAT (HORIZONTAL ONLY)").FirstOrDefault().IsMaterialEnabled = false;
            //    sysmat = SystemMaterials.Where(x => x.Name == "PB-4 (VERTICAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;

            //    sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (VERTICAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;

            //    sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (HORIZONTAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;
            //}
            //if (obj.ToString() == "PB-4 (VERTICAL ONLY)")
            //{
            //    sysmat = SystemMaterials.Where(x => x.Name == "PROTECTION MAT (HORIZONTAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;

            //    sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (VERTICAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;

            //    sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (HORIZONTAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;
            //}
            //if (obj.ToString() == "TREMDRAIN 1000 (VERTICAL ONLY)")
            //{
            //    sysmat = SystemMaterials.Where(x => x.Name == "PROTECTION MAT (HORIZONTAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;

            //    sysmat = SystemMaterials.Where(x => x.Name == "PB-4 (VERTICAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;

            //    sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (HORIZONTAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;

            //    SystemMaterials.Where(x => x.Name == "PINS & LOADS").FirstOrDefault().Coverage = pinsCoverage + sysmat.Qty * 200 / 500;
            //}
            //if (obj.ToString() == "TREMDRAIN 1000 (HORIZONTAL ONLY)")
            //{
            //    sysmat = SystemMaterials.Where(x => x.Name == "PROTECTION MAT (HORIZONTAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;

            //    sysmat = SystemMaterials.Where(x => x.Name == "PB-4 (VERTICAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;

            //    sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (VERTICAL ONLY)").FirstOrDefault();
            //    sysmat.IsMaterialEnabled = true;
            //    sysmat.IsMaterialChecked = false;
            //}
            calculateRLqty();
            CalculateLaborMinCharge();
        }

        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {
            if (calhrs==0)
            {
                return 0;
            }
            else
                return  setupMin > calhrs ? setupMin * laborRate : calhrs * laborRate;
        }
        public override void setCheckBoxes()
        {

        }
    }
}
