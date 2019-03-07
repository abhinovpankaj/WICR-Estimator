using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    class ParasealLGMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double additionalTermBarLF;
        private double insideOutsideCornerDetails;
        private bool superStopFooting;
        private double rakerCornerBases;
        private double cementBoardDetail;
        private double rockPockets;
        private double parasealFoundation;
        private double rearMidLagging;
        public ParasealLGMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();

            FillMaterialList();

            FetchMaterialValuesAsync(false);
            
        }

        private void FillMaterialList()
        {
            materialNames.Add("PARATERM BAR LF", "LF");

            materialNames.Add("PARA STICK AND DRY (REAR LAG, BLOCK OUTS, RAKERS)", "ROLLS");
            materialNames.Add("DYMONIC 100 OR VULKEM 116 CAULK (FOR TERM BAR)", "10 oz.TUBE");
            materialNames.Add("PARAMASTIC (1000 LF PER PAIL FOR PREP & TERMINATIONS)", "5 GAL PAIL");
            materialNames.Add("PARAGRANULAR (FOR CANT AT FOOTING)", "LF");
            materialNames.Add("PARASEAL LG ROLLS (4X24)", "96 SQ FT/ROLL");
            materialNames.Add("PARASEAL LG CORNER DETAILS & LABOR FOR STARTER STRIP", "ROLLS");
            materialNames.Add("LABOR FOR ALL PENETRATIONS, CEMENT BOARD, LAGGING PREP", "");
            materialNames.Add("NON-POUROUS PRIMER", "1GAL");
            materialNames.Add("SUPER STOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT\"", "ROLL");
            materialNames.Add("PINS & LOADS", "EACH");

            materialNames.Add("**VULKEM 201 T CAN SOMETIMES BE USED IN LIEU OF PARAMASTIC ON LARGE JOBS.  CHECK WITH MANUFACTURER", "5 GAL PAIL");
            materialNames.Add("DUROROCK 1/4\" x 3ft x5ft for LAGING BEAMS AND ROCK POCKETS", "EACH");
            materialNames.Add("3/4\" GRAVEL FOR ROCK POCKETS", "EACH");
            materialNames.Add("TREMDRAIN 1000 (VERTICAL ONLY)", "");
            materialNames.Add("VISQUINE PROTECTION FOR INCLEMENT WEATHER", "SQFT");
            materialNames.Add("UNIVERSAL OUTLETS", "EACH");
            materialNames.Add("TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\"", "LINEAR FEET");
            materialNames.Add("4 INCH SCHEDULE 40 PIPE FOR ROCK POCKETS", "20 LF");          
        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "UNIVERSAL OUTLETS" || item.Name == "TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\""
                    ||item.Name== "VISQUINE PROTECTION FOR INCLEMENT WEATHER")
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

                    if (SystemMaterials[i].Name == "UNIVERSAL OUTLETS" || 
                        SystemMaterials[i].Name == "TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\""||
                        SystemMaterials[i].Name == "VISQUINE PROTECTION FOR INCLEMENT WEATHER")
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
            om.Add(new OtherItem { Name = "Add for 2nd move on paraseal", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Add for 2nd move on superstop", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Add for additional move ons", IsReadOnly = false });

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
            JobsetupTremco Js = sender as  JobsetupTremco;
            if (Js != null)
            {
                additionalTermBarLF = Js.AdditionalTermBarLF;
                insideOutsideCornerDetails = Js.InsideOutsideCornerDetails;
                superStopFooting = Js.SuperStopAtFooting;
                rearMidLagging = Js.RearMidLagging;
                rakerCornerBases = Js.RakerCornerBases;
                cementBoardDetail = Js.CementBoardDetail;
                rockPockets = Js.RockPockets;
                parasealFoundation = Js.ParasealFoundation;
    }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                //case "PROTECTION MAT (HORIZONTAL ONLY)":
                case "PARA STICK AND DRY (REAR LAG, BLOCK OUTS, RAKERS)":
                case "DYMONIC 100 OR VULKEM 116 CAULK (FOR TERM BAR)":
                case "PARASEAL LG ROLLS (4X24)":
                case "PARASEAL LG CORNER DETAILS & LABOR FOR STARTER STRIP":
                case "LABOR FOR ALL PENETRATIONS, CEMENT BOARD, LAGGING PREP":
                    return true;
                case "PARAGRANULAR (FOR CANT AT FOOTING)":
                case "**VULKEM 201 T CAN SOMETIMES BE USED IN LIEU OF PARAMASTIC ON LARGE JOBS.  CHECK WITH MANUFACTURER":
                    return false;
                case "PARATERM BAR LF":
                    return deckPerimeter>0? true:false;
                case "NON-POUROUS PRIMER":
                    return rakerCornerBases+ rearMidLagging > 0 ? true : false;
                case "SUPER STOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT\"":
                    return superStopFooting;
                case "PINS & LOADS":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                    return totalSqft > 0 ? true : false;
                case "DUROROCK 1/4\" x 3ft x5ft for LAGING BEAMS AND ROCK POCKETS":
                case "3/4\" GRAVEL FOR ROCK POCKETS":
                    return cementBoardDetail+rockPockets> 0 ? true : false;
                case "4 INCH SCHEDULE 40 PIPE FOR ROCK POCKETS":
                    return  rockPockets > 0 ? true : false;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "PARAGRANULAR (FOR CANT AT FOOTING)":
                case "**VULKEM 201 T CAN SOMETIMES BE USED IN LIEU OF PARAMASTIC ON LARGE JOBS.  CHECK WITH MANUFACTURER":
                    return true;
                default:
                    return false;
            }

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "PARA STICK AND DRY (REAR LAG, BLOCK OUTS, RAKERS)":
                    return rearMidLagging + rakerCornerBases * 12;
                
                case "PARASEAL LG ROLLS (4X24)":
                case "PINS & LOADS":
                case "**VULKEM 201 T CAN SOMETIMES BE USED IN LIEU OF PARAMASTIC ON LARGE JOBS.  CHECK WITH MANUFACTURER":
                    return totalSqft + stairWidth;
                case "PARASEAL LG CORNER DETAILS & LABOR FOR STARTER STRIP":
                    return insideOutsideCornerDetails;
                case "PARAGRANULAR (FOR CANT AT FOOTING)":
                    return deckPerimeter;
               
                case "PARATERM BAR LF":
                case "DYMONIC 100 OR VULKEM 116 CAULK (FOR TERM BAR)":
                    return deckPerimeter+additionalTermBarLF;
                case "NON-POUROUS PRIMER":
                    return rearMidLagging + rakerCornerBases * 15; 
                case "SUPER STOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT\"":
                    return deckPerimeter+deckCount;
                
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                    return totalSqft;
                
                case "4 INCH SCHEDULE 40 PIPE FOR ROCK POCKETS":
                    return rockPockets ;
                default:
                    return 0;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "PARAMASTIC (1000 LF PER PAIL FOR PREP & TERMINATIONS)":
                    return totalSqft / 2500 + riserCount * .1 + rearMidLagging / 100 + rakerCornerBases / 25;
                case "DUROROCK 1/4\" x 3ft x5ft for LAGING BEAMS AND ROCK POCKETS":
                    return cementBoardDetail / 2 + rockPockets / 8;
                case "3/4\" GRAVEL FOR ROCK POCKETS":
                    return rockPockets * 2;
                default:
                    return lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "PARATERM BAR LF":
                    return deckPerimeter;
                case "SUPER STOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT\"":
                    return deckPerimeter + deckCount;
                default:
                    return 0;
            }

        }
        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "PARAMASTIC (1000 LF PER PAIL FOR PREP & TERMINATIONS)":
                case "PARASEAL LG ROLLS (4X24)":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                    return totalSqft;
                    
                case "PARAGRANULAR (FOR CANT AT FOOTING)":
                    return deckPerimeter;
                case "PARASEAL LG CORNER DETAILS & LABOR FOR STARTER STRIP":
                    return parasealFoundation;
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
            double val1 = 0, val2 = 0;
            SystemMaterial sysMat = SystemMaterials.Where(x => x.Name == "PARATERM BAR LF (TOP ONLY- STANDARD INSTALL)").FirstOrDefault();
            if (sysMat != null)
            {
                val1 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "EXTRA PARATERM BAR LF (BOTTOM OR SIDES)").FirstOrDefault();
            if (sysMat != null)
            {
                val2 = sysMat.IsMaterialChecked ? sysMat.Qty : 0;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "PINS & LOADS").FirstOrDefault();
            if (sysMat != null)
            {
                bool ischecked;
                ischecked = sysMat.IsMaterialChecked;
                sysMat.Qty = sysMat.SMSqft / sysMat.Coverage + (val1 + val2) / 2;
                sysMat.IsMaterialChecked = ischecked;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "PARASEAL \"STANDARD\" ROLLS (4X24)").FirstOrDefault();
            if (sysMat != null)
            {
                SystemMaterial myMat = SystemMaterials.Where(x => x.Name == "SEAM TAPE").FirstOrDefault();
                myMat.SMSqft = sysMat.Qty * 28;
                myMat.Qty = myMat.SMSqft / myMat.Coverage;
                OtherMaterials.Where(x => x.Name == "Linear footage for seams if needed for submerged conditions").FirstOrDefault().Quantity = sysMat.Qty * 28;
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
            SystemMaterial sysmat = null;
            if (obj.ToString() == "PROTECTION MAT (HORIZONTAL ONLY)")
            {
                SystemMaterials.Where(x => x.Name == "PROTECTION MAT (HORIZONTAL ONLY)").FirstOrDefault().IsMaterialEnabled = false;
                sysmat = SystemMaterials.Where(x => x.Name == "PB-4 (VERTICAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;

                sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (VERTICAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;

                sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (HORIZONTAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;
            }
            if (obj.ToString() == "PB-4 (VERTICAL ONLY)")
            {
                sysmat = SystemMaterials.Where(x => x.Name == "PROTECTION MAT (HORIZONTAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;

                sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (VERTICAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;

                sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (HORIZONTAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;
            }
            if (obj.ToString() == "TREMDRAIN 1000 (VERTICAL ONLY)")
            {
                sysmat = SystemMaterials.Where(x => x.Name == "PROTECTION MAT (HORIZONTAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;

                sysmat = SystemMaterials.Where(x => x.Name == "PB-4 (VERTICAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;

                sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (HORIZONTAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;

                SystemMaterials.Where(x => x.Name == "PINS & LOADS").FirstOrDefault().Coverage = pinsCoverage + sysmat.Qty * 200 / 500;
            }
            if (obj.ToString() == "TREMDRAIN 1000 (HORIZONTAL ONLY)")
            {
                sysmat = SystemMaterials.Where(x => x.Name == "PROTECTION MAT (HORIZONTAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;

                sysmat = SystemMaterials.Where(x => x.Name == "PB-4 (VERTICAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;

                sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (VERTICAL ONLY)").FirstOrDefault();
                sysmat.IsMaterialEnabled = true;
                sysmat.IsMaterialChecked = false;
            }
            calculateRLqty();
            CalculateLaborMinCharge();
        }

        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {
            return setupMin > calhrs ? setupMin * laborRate : calhrs * laborRate;
        }
        public override void setCheckBoxes()
        {

        }
    }
}
