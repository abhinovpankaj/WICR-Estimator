using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    class _201MaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double totalSqftVertical;
        private double termBar;
        private double rebarPrepWalls;
        private double superStop;
        private double penetrations;
        //private double termBar;
        public _201MaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup js) : base(metalTotals, slopeTotals, js)
        {
            materialNames = new Dictionary<string, string>();

            FillMaterialList();

            FetchMaterialValuesAsync(false);
        }

        private void FillMaterialList()
        {
            materialNames.Add("191 QD PRIMER AND PREPARATION FOR RE-SURFACE", "1 GALLON");
            materialNames.Add("TREMPRIME MULTI SURFACE (CONCRETE & OTHER)", "3 GAL KIT");
            materialNames.Add("#191 QD INTERLAMINATE PRIMER ", "1 GALLON");
            materialNames.Add("Vulkem Tremproof 250 GC L", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 250 GC R", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 250 GC T", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 201 L", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 201 R", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 201 T", "5 GAL PAIL");
            materialNames.Add("GLASSMAT #II (FROM MERKOTE / LOWRYS) WALLS", "1200 SF ROLL");
            materialNames.Add("GLASSMAT #II (FROM MERKOTE / LOWRYS) FLOORS YES/NO", "1200 SF ROLL");
            materialNames.Add("PW POLYESTER FABRIC FROM UPI 4\"(PERIMETER)", "150 SF ROLL");
            materialNames.Add("TREMCO DYMONIC 100 OR VULKEM 116 (PERIMETER JOINTS)", "20OZ SAUSAGE");
            materialNames.Add("PW POLYESTER FABRIC FROM UPI 4\"(PLYWOOD SEAMS)", "150 SF ROLL");
            materialNames.Add("TREMCO DYMONIC 100 OR VULKEM 116 (PLYWOOD JOINTS)", "20OZ SAUSAGE");
            materialNames.Add("PROTECTION MAT (HORIZONTAL ONLY)", "667 SF ROLL");
            materialNames.Add("PB-4 (VERTICAL ONLY)", "200 SF ROLL");
            materialNames.Add("TREMDRAIN 1000 (VERTICAL ONLY)", "200 SF ROLL");
            materialNames.Add("CALIFORNIA SEALER FROM LOWRYS (GLUING DRAIN MAT)", "5 GAL PAIL");
            materialNames.Add("TREMDRAIN 1000 (HORIZONTAL ONLY)", "200 SF ROLL");
            materialNames.Add("TERM BAR, VULKEM 116, PINS AND LOADS", "LF");
            materialNames.Add("SUPERSTOP(LF)", "LF");
            materialNames.Add("PENETRATIONS", "EACH");
            materialNames.Add("UNIVERSAL OUTLET", "EACH");
            materialNames.Add("TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)", "LINEAR FEET");
            materialNames.Add("Vulkem Tremproof 250 GC L 30 MILS", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 250 GC R 30 MILS", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 201 L 30 MILS", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 201 R 30 MILS", "5 GAL PAIL");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");
            materialNames.Add("PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL (LF DECK TO WALL) WITH SAND BROADCAST", "LF");
           
        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)"
                    || item.Name == "PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL (LF DECK TO WALL) WITH SAND BROADCAST")
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

                    if (SystemMaterials[i].Name == "TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)" ||
                        SystemMaterials[i].Name == "PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL (LF DECK TO WALL) WITH SAND BROADCAST")
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

        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobsetupTremco Js = sender as JobsetupTremco;
            if (Js != null)
            {
                termBar = Js.TermBarLF;
                rebarPrepWalls = Js.RebarPrepWallsLF;
                superStop = Js.SuperStopLF;
                penetrations = Js.Penetrations;
                totalSqftVertical = Js.TotalSqftVertical;               
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                
                default:
                    return false;
            }

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                
                default:
                    return 0;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                
                default:
                    return coverage == 0 ? 0 : lfArea / coverage;
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
            sysMat = SystemMaterials.Where(x => x.Name == "PARASEAL LG ROLLS (4X24)").FirstOrDefault();
            if (sysMat != null)
            {
                OtherMaterials.Where(x => x.Name == "Linear footage for seams if needed for submerged conditions")
                    .FirstOrDefault().Quantity = Math.Round(sysMat.Qty * 28, 2);
                OtherLaborMaterials.Where(x => x.Name == "Linear footage for seams if needed for submerged conditions").FirstOrDefault().LQuantity = Math.Round(sysMat.Qty * 28, 2);
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
