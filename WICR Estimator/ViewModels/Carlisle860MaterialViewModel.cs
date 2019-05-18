using System;
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
    class Carlisle860MaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double additionalTermBarLF;
        private double insideOutsideCornerDetails;
        private bool superStopFooting;
        
        private double penetrations;
        private double linearFootage;
        private double totalSqftVertical;
        private double totalSqftPlywood;
        public Carlisle860MaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();

            FillMaterialList();

            FetchMaterialValuesAsync(false);
            
        }


        private void FillMaterialList()
        {
            materialNames.Add("CCW 702 WB PRIMER", "5 GAL PAIL");
            materialNames.Add("CCW 703V FOR PLYWOOD SEAMS AND DETAILS", "4 GAL KIT");
            materialNames.Add("CCW 703V (45 LF PER TUBE FOR CANT & TERMINATATIONS)", "4 GAL KIT");
            materialNames.Add("CCW 703V (45 LF PER TUBE FOR SEAMS)", "4 GAL KIT");
            materialNames.Add("CCW MIRADRI 860 (3 FT X 66.7 FT ROLLS)", "200 SQ FT/ROLL");
            materialNames.Add("CCW 703V FOR PERIMETER DETAIL L METAL (45 LF PER TUBE) AND LIQUID MEMBRANE FOR DECK TO WALL", "4 GAL KIT");
            materialNames.Add("PENETRATIONS (DETAIL SHEET AND MASTIC)", "EACH");
            materialNames.Add("FIX LEAKS AFTER WATER TEST BY OTHERS", "");
            materialNames.Add("200 V PROTECTION (VERTICAL ONLY)", "670 SQ FT ROLL");
            materialNames.Add("300 HV PROTECTION (HORIZONTAL ONLY)", "670 SQ FT ROLL");
            materialNames.Add("MIRADRAIN 6000 XL (VERTICAL ONLY)", "200 SF ROLL");
            materialNames.Add("MIRADRAIN 6000 XL  (HORIZONTAL ONLY)", "200 SF ROLL");
            materialNames.Add("MIRASTICK ADHESIVE (GLUE DOWN DRAIN MAT)", "5 GAL PAIL");
            materialNames.Add("SIDE OUTLET 6\"", "EACH");
            materialNames.Add("MIRADRAIN HC 1\" DRAIN - PUNCHED 12\" X 100'  (QUICK DRAIN)", "LINEAR FEET");
            materialNames.Add("TERM BAR", "LINEAR FEET");
            materialNames.Add("WATERSTOP (MIRASTOP)", "LINEAR FEET");
            materialNames.Add("INSIDE AND OUTSIDE CORNER DETAILS (MIRADRI 860)", "ROLLS");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");

        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "SIDE OUTLET 6\"" || item.Name == "MIRADRAIN HC 1\" DRAIN - PUNCHED 12\" X 100'  (QUICK DRAIN)"
                    ||item.Name== "Plywood 3/4 & blocking (# of 4x8 sheets)" || item.Name == "Stucco Material Remove and replace (LF)")
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

                    if (SystemMaterials[i].Name == "SIDE OUTLET 6\"" || SystemMaterials[i].Name == "MIRADRAIN HC 1\" DRAIN - PUNCHED 12\" X 100'  (QUICK DRAIN)"
                    || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" || SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)")
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
            return om;
        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js != null)
            {
                additionalTermBarLF = Js.TermBarLF;
                insideOutsideCornerDetails = Js.InsideOutsideCornerDetails;
                superStopFooting = Js.SuperStopAtFooting;
                linearFootage = Js.LinearCopingFootage;
                penetrations = Js.Penetrations;
                totalSqftVertical = Js.TotalSqftVertical;
                totalSqftPlywood = Js.TotalSqftPlywood;

            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override void CalculateCostPerSqFT()
        {
            CostPerSquareFeet = (totalSqft + totalSqftPlywood+totalSqftVertical) == 0 ? 0 : Math.Round(TotalMaterialCost / (totalSqft + totalSqftPlywood + totalSqftVertical), 2);
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {                
                case "CCW 702 WB PRIMER":
                    return totalSqft+totalSqftVertical > 0 ? true : false;
                case "CCW 703V FOR PERIMETER DETAIL L METAL (45 LF PER TUBE) AND LIQUID MEMBRANE FOR DECK TO WALL":
                    return deckPerimeter> 0 ? true : false;
                case "PENETRATIONS (DETAIL SHEET AND MASTIC)":
                    return penetrations > 0 ? true : false;
                case "FIX LEAKS AFTER WATER TEST BY OTHERS":
                    return totalSqft+totalSqftPlywood > 0 ? true : false;
                case "TERM BAR":
                    return additionalTermBarLF > 0 ? true : false;
                case "WATERSTOP (MIRASTOP)":
                    return superStopFooting;
                case "INSIDE AND OUTSIDE CORNER DETAILS (MIRADRI 860)":
                    return insideOutsideCornerDetails > 0 ? true : false;
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "200 V PROTECTION (VERTICAL ONLY)":
                case "300 HV PROTECTION (HORIZONTAL ONLY)":
                case "SIDE OUTLET 6\"":
                case "MIRADRAIN HC 1\" DRAIN - PUNCHED 12\" X 100'  (QUICK DRAIN)":
                    return false;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "200 V PROTECTION (VERTICAL ONLY)":
                case "300 HV PROTECTION (HORIZONTAL ONLY)":
                case "MIRADRAIN 6000 XL (VERTICAL ONLY)":
                case "MIRADRAIN 6000 XL  (HORIZONTAL ONLY)":
                    return true;
                default:
                    return false;
            }

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "CCW 702 WB PRIMER":
                    return totalSqft + totalSqftVertical + deckPerimeter;
                case "CCW 703V FOR PLYWOOD SEAMS AND DETAILS":
                    return totalSqftPlywood/32*12;
                case "CCW 703V (45 LF PER TUBE FOR CANT & TERMINATATIONS)":
                    return linearFootage * 2 + deckPerimeter;
                case "CCW 703V (45 LF PER TUBE FOR SEAMS)":
                    return (totalSqftPlywood + totalSqft + totalSqftVertical) / 2.5 / 2 + riserCount*(stairWidth * 2 + 4);
                case "CCW MIRADRI 860 (3 FT X 66.7 FT ROLLS)":
                    return (totalSqftPlywood + totalSqft + totalSqftVertical);
                case "CCW 703V FOR PERIMETER DETAIL L METAL (45 LF PER TUBE) AND LIQUID MEMBRANE FOR DECK TO WALL":
                    return deckPerimeter+ riserCount * (stairWidth * 2 + 4);
                case "PENETRATIONS (DETAIL SHEET AND MASTIC)":
                    return penetrations;
                case "FIX LEAKS AFTER WATER TEST BY OTHERS":
                    return totalSqftPlywood + totalSqft;
                case "200 V PROTECTION (VERTICAL ONLY)":
                case "MIRASTICK ADHESIVE (GLUE DOWN DRAIN MAT)":
                    return totalSqftVertical;
                case "300 HV PROTECTION (HORIZONTAL ONLY)":
                case "MIRADRAIN 6000 XL  (HORIZONTAL ONLY)":
                    return totalSqftPlywood + totalSqft + riserCount * stairWidth;
                case "MIRADRAIN 6000 XL (VERTICAL ONLY)":
                    return totalSqftVertical + stairWidth * riserCount;
                case "INSIDE AND OUTSIDE CORNER DETAILS (MIRADRI 860)":
                    return insideOutsideCornerDetails;
                default:
                    return 0;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "TERM BAR":
                case "WATERSTOP (MIRASTOP)":
                    return additionalTermBarLF;
                
                default:
                    return lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "CCW 702 WB PRIMER":
                    return totalSqft + totalSqftVertical + deckPerimeter;
                case "CCW 703V FOR PLYWOOD SEAMS AND DETAILS":
                    return totalSqftPlywood / 32 * 12*1.2;
                case "CCW 703V (45 LF PER TUBE FOR CANT & TERMINATATIONS)":
                    return  deckPerimeter;
                case "CCW 703V (45 LF PER TUBE FOR SEAMS)":
                    return (totalSqftPlywood + totalSqft ) / 2.5 / 2 ;
                case "CCW MIRADRI 860 (3 FT X 66.7 FT ROLLS)":
                    return (totalSqftPlywood + totalSqft );
                case "CCW 703V FOR PERIMETER DETAIL L METAL (45 LF PER TUBE) AND LIQUID MEMBRANE FOR DECK TO WALL":
                    return deckPerimeter;
                case "PENETRATIONS (DETAIL SHEET AND MASTIC)":
                case "FIX LEAKS AFTER WATER TEST BY OTHERS":
                case "MIRADRAIN 6000 XL (VERTICAL ONLY)":
                case "MIRASTICK ADHESIVE (GLUE DOWN DRAIN MAT)":
                    return 0;
                case "200 V PROTECTION (VERTICAL ONLY)":
                case "300 HV PROTECTION (HORIZONTAL ONLY)":
                case "MIRADRAIN 6000 XL  (HORIZONTAL ONLY)":
                    return totalSqftPlywood + totalSqft;
                case "INSIDE AND OUTSIDE CORNER DETAILS (MIRADRI 860)":
                    return insideOutsideCornerDetails;
                default:
                    return 0;
            }

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
        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0, double sqftStairs = 0, string matName = "")
        {
            double sqft = sqftHor + sqftStairs + sqftVert;
            return sqft == 0 ? 0 : laborExtension / sqft;
        }
        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "CCW 702 WB PRIMER":
                case "CCW MIRADRI 860 (3 FT X 66.7 FT ROLLS)":
                case "300 HV PROTECTION (HORIZONTAL ONLY)":
                case "MIRADRAIN 6000 XL (VERTICAL ONLY)":
                case "MIRASTICK ADHESIVE (GLUE DOWN DRAIN MAT)":
                    return  totalSqftVertical;
                
                case "CCW 703V (45 LF PER TUBE FOR CANT & TERMINATATIONS)":
                    return linearFootage*2;
                case "CCW 703V (45 LF PER TUBE FOR SEAMS)":
                    return (totalSqftVertical) / 2.5 / 2;
                case "PENETRATIONS (DETAIL SHEET AND MASTIC)":
                    return penetrations;
               
                case "INSIDE AND OUTSIDE CORNER DETAILS (MIRADRI 860)":
                    return insideOutsideCornerDetails;
                default:
                    return 0;
            }
        }
        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "CCW 703V FOR PLYWOOD SEAMS AND DETAILS":
                case "CCW MIRADRI 860 (3 FT X 66.7 FT ROLLS)":
                case "CCW 703V (45 LF PER TUBE FOR SEAMS)":
                    return riserCount * stairWidth * 2;
                case "200 V PROTECTION (VERTICAL ONLY)":
                case "300 HV PROTECTION (HORIZONTAL ONLY)":
                case "MIRADRAIN 6000 XL (VERTICAL ONLY)":
                case "MIRADRAIN 6000 XL  (HORIZONTAL ONLY)":
                    return riserCount * stairWidth;
                
                case "CCW 703V FOR PERIMETER DETAIL L METAL (45 LF PER TUBE) AND LIQUID MEMBRANE FOR DECK TO WALL":
                case "CCW 703V (45 LF PER TUBE FOR CANT & TERMINATATIONS)":
                    return riserCount*2 * 2;
                
                
                default:
                    return 0;
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

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "MIRADRAIN HC 1\" DRAIN - PUNCHED 12\" X 100'  (QUICK DRAIN)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftV = item.Qty;                   
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate,item.SMSqftV,item.VerticalProductionRate);

                    item.LaborExtension =item.Hours==0 ?0: item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }

                item = SystemMaterials.Where(x => x.Name == "TERM BAR").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftV = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate, item.SMSqftV, item.VerticalProductionRate);

                    item.LaborExtension = item.Hours == 0 ? 0 : item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }

                item = SystemMaterials.Where(x => x.Name == "WATERSTOP (MIRASTOP)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftV = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate, item.SMSqftV, item.VerticalProductionRate);

                    item.LaborExtension = item.Hours == 0 ? 0 : item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }

                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty * 32;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.Hours == 0 ? 0 : item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty/32;
                }
                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.Hours == 0 ? 0 : item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
            }
            CalculateLaborMinCharge();
        }
        public override void CalculateLaborMinCharge()
        {
            //LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
            //                            x.IsMaterialChecked).ToList().Select(x => x.Hours).Sum();
            //LaborMinChargeMinSetup = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
            //                             x.IsMaterialChecked).ToList().Select(x => x.SetupMinCharge).Sum();
            //LaborMinChargeLaborExtension = (LaborMinChargeMinSetup + LaborMinChargeHrs) > 20 ? 0 :
            //                                    (20 - LaborMinChargeMinSetup - LaborMinChargeHrs) * laborRate;
            base.CalculateLaborMinCharge();
        }
        
        public override void ApplyCheckUnchecks(object obj)
        {

            SystemMaterial sysmat = null;
            bool ischecked = false, ischecked1 = false;
            if (obj.ToString() == "MIRADRAIN 6000 XL (VERTICAL ONLY)" || obj.ToString() == "MIRADRAIN 6000 XL  (HORIZONTAL ONLY)")
            {
                sysmat = SystemMaterials.Where(x => x.Name == "MIRADRAIN 6000 XL (VERTICAL ONLY)").FirstOrDefault();
                ischecked = sysmat.IsMaterialChecked;
                sysmat = SystemMaterials.Where(x => x.Name == "MIRADRAIN 6000 XL  (HORIZONTAL ONLY)").FirstOrDefault();
                ischecked1 = sysmat.IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "MIRASTICK ADHESIVE (GLUE DOWN DRAIN MAT)").FirstOrDefault().IsMaterialChecked = ischecked || ischecked1;
            }
            
   
            calculateRLqty();
            CalculateLaborMinCharge();
        }

        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {
            if (calhrs == 0)
            {
                return 0;
            }
            else
                return setupMin > calhrs ? setupMin * laborRate : calhrs * laborRate;
        }
        public override void setCheckBoxes()
        {
            SystemMaterial sysmat = SystemMaterials.FirstOrDefault(x => x.Name == "MIRADRAIN 6000 XL (VERTICAL ONLY)");
            SystemMaterial sysmat1 = SystemMaterials.FirstOrDefault(x => x.Name == "MIRADRAIN 6000 XL  (HORIZONTAL ONLY)");

            if (sysmat != null && sysmat1!=null)
            {
                SystemMaterials.FirstOrDefault(x => x.Name == "MIRASTICK ADHESIVE (GLUE DOWN DRAIN MAT)").IsMaterialChecked = sysmat.IsMaterialChecked||sysmat1.IsMaterialChecked;
            }
        }
    }
}
