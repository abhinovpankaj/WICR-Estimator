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
    public class WWRehabMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        public WWRehabMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);

        }

        #region Overridden Methods
        public override void calculateLaborHrs()
        {
            calLaborHrs(6,totalSqft);
        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();


            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Stucco Material Remove And Replace (Lf)" || item.Name == "Plywood 3/4 & Blocking(# Of 4X8 Sheets)" ||
                    item.Name == "Extra Stair Nosing Lf" || item.Name == "Bubble Repair(Measure Sq Ft)" || item.Name == "Large Crack Repair")
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
                    if (SystemMaterials[i].Name == "Stucco Material Remove And Replace (Lf)" || SystemMaterials[i].Name == "Plywood 3/4 & Blocking(# Of 4X8 Sheets)" ||
                    SystemMaterials[i].Name == "Extra Stair Nosing Lf" || SystemMaterials[i].Name == "Bubble Repair(Measure Sq Ft)"
                            || SystemMaterials[i].Name == "Large Crack Repair")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];

                        }
                    }

                }

            }
            else
            {
                SystemMaterials = sysMat;
                setCheckBoxes();
            }
                

            
            //foreach (var mat in SystemMaterials)
            //{
            //    if (mat.Name == "Lip Color" || mat.Name == "Aj-44A Dressing(Sealer)" || mat.Name == "Vista Paint Acripoxy")
            //    {
            //        if (mat.IsMaterialChecked)
            //        {
            //            ApplyCheckUnchecks(mat.Name);
            //            break;
            //        }
            //    }
            //}
            setExceptionValues(null);
            calculateRLqty();

            if (OtherMaterials.Count == 0)
            {
                OtherMaterials = GetOtherMaterials();
                OtherLaborMaterials = OtherMaterials;
            }


            if (SubContractLaborItems.Count == 0)
            {
                SubContractLaborItems = GetLaborItems();
            }
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
            calculateRLqty();
            CalculateCost(null);
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
        public override string GetOperation(string matName)
        {
            switch (matName)
            {
                case "Resistite Regular Over Texture(#55 Bag)":
                    return "3/32 INCH THICK TROWEL DOWN";
                case "Large Crack Repair":
                    return "CAULK";
                case "Light Crack Repair":
                case "Bubble Repair(Measure Sq Ft)":
                    return "CAULK";
                 
                case "30# Divorcing Felt (200 Sq Ft) From Ford Wholesale":
                

                    return "SLIP SHEET";
                case "Rp Fabric 10 Inch Wide X (300 Lf) From Acme":
                    return "DETAIL STAIRS ONLY";
                case "Glasmat #4 (1200 Sq Ft) From Acme":
                    return "INSTALL FIELD GLASS";
                case "Cpc Membrane":
                    return "SATURATE GLASS & DETAIL PERIMETER";
                case "Neotex-38 Paste":
                    return "ADD TO BODY COAT";
                case "Neotex Standard Powder(Body Coat)":
                case "Neotex Standard Powder(Body Coat) 1":
                case "Resistite Regular White":
                case "Resistite Regular Or Smooth White(Knock Down Or Smooth)":
                case "Custom Texture Skip Trowel(Resistite Smooth White)":
                    return "TROWEL";

                case "Resistite Liquid":
                    return "ADD TO TAN FILLER";

                case "Aj-44A Dressing(Sealer)":
                case "Vista Paint Acripoxy":
                case "Lip Color":
                    return "ROLL 2 COATS";

                case "Resistite Universal Primer(Add 50% Water)":
                    return "PRIMER: SPRAY OR ROLL";

                case "Weather Seal XL two Coats":
                    return "";
                case "Stair Nosing From Dexotex":
                case "Extra Stair Nosing Lf":
                    return "NAIL OR SCREW";
                case "Plywood 3/4 & Blocking(# Of 4X8 Sheets)":
                case "Stucco Material Remove And Replace (Lf)":
                    return "Remove and replace dry rot";

                default:
                    return "";

            }
        }
        private void FillMaterialList()
        {
            materialNames = new Dictionary<string, string>();
            materialNames.Add("Resistite Regular Over Texture(#55 Bag)", "55 LB BAG");
            materialNames.Add("Light Crack Repair", "Sq Ft");
            materialNames.Add("Large Crack Repair", "LF");
            materialNames.Add("Bubble Repair(Measure Sq Ft)", "Sq Ft");
            materialNames.Add("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale", "ROLL");
            materialNames.Add("Rp Fabric 10 Inch Wide X (300 Lf) From Acme", "ROLL");
            materialNames.Add("Glasmat #4 (1200 Sq Ft) From Acme", "ROLL");
            materialNames.Add("Cpc Membrane", "5 GAL PAIL");
            materialNames.Add("Neotex-38 Paste", "5 GAL PAIL");
            materialNames.Add("Neotex Standard Powder(Body Coat)", "45 LB BAG");
            materialNames.Add("Neotex Standard Powder(Body Coat) 1", "45 LB BAG");
            materialNames.Add("Resistite Liquid", "5 GAL PAIL");
            materialNames.Add("Resistite Regular White", "55 LB BAG");

            materialNames.Add("Resistite Regular Or Smooth White(Knock Down Or Smooth)", "40 LB BAG");
            materialNames.Add("Aj-44A Dressing(Sealer)", "5 GAL PAIL");
            materialNames.Add("Vista Paint Acripoxy", "5 GAL PAIL");
            materialNames.Add("Lip Color", "ROLL 2 COATS");
            materialNames.Add("Resistite Universal Primer(Add 50% Water)", "Sq Ft");
            materialNames.Add("Custom Texture Skip Trowel(Resistite Smooth White)", "Sq Ft");
            materialNames.Add("Weather Seal XL two Coats", "Sq Ft");
            materialNames.Add("Stair Nosing From Dexotex", "Sq Ft");
            materialNames.Add("Extra Stair Nosing Lf", "Sq Ft");
            materialNames.Add("Plywood 3/4 & Blocking(# Of 4X8 Sheets)", "Sq Ft");
            materialNames.Add("Stucco Material Remove And Replace (Lf)", "Sq Ft");

        }
        public override ObservableCollection<SystemMaterial> GetSystemMaterial()
        {

            ObservableCollection<SystemMaterial> smP = new ObservableCollection<SystemMaterial>();
            int cov;
            double mp;
            double w;
            double lfArea;
            double setUpMin = 0; // Setup minimum charges from google sheet, col 6
            double pRateStairs = 0; ///Production rate stairs from google sheet, col 5
            double hprRate = 0;///Horizontal Production rate  from google sheet, col 4
            //double vprRate = 0;///Vertical Production rate  from google sheet, col 1
            double sqh = 0;
            double labrExt = 0;
            double calcHrs = 0;
            double sqStairs = 0;
            double qty = 0;
            if (isPrevailingWage)
            {
                double.TryParse(freightData[5][0].ToString(), out prPerc);
            }
            else
                prPerc = 0;
            int.TryParse(materialDetails[3][2].ToString(), out cov);
            double.TryParse(materialDetails[3][0].ToString(), out mp);
            double.TryParse(materialDetails[3][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular Over Texture(#55 Bag)");
            double.TryParse(materialDetails[3][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[3][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[3][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Resistite Regular Over Texture(#55 Bag)");
            sqStairs = getSqFtStairs("Resistite Regular Over Texture(#55 Bag)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs>=setUpMin ? calcHrs * laborRate : setUpMin*laborRate;
            qty = getQuantity("Resistite Regular Over Texture(#55 Bag)", cov, lfArea);
            smP.Add(new SystemMaterial
            {

                IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Over Texture(#55 Bag)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Over Texture(#55 Bag)"),
                Name = "Resistite Regular Over Texture(#55 Bag)",
                SMUnits = "55 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "3/32 INCH THICK TROWEL DOWN",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = sqStairs,
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true

            });
            int.TryParse(materialDetails[0][2].ToString(), out cov);
            double.TryParse(materialDetails[0][0].ToString(), out mp);
                double.TryParse(materialDetails[0][3].ToString(), out w);
                double.TryParse(materialDetails[0][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[0][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[0][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Light Crack Repair");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
                lfArea = getlfArea("Light Crack Repair");
                sqStairs = getSqFtStairs("Light Crack Repair");

            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0; ;
            qty = getQuantity("Light Crack Repair", cov, lfArea);
                smP.Add(new SystemMaterial
                {
                    IsWWR = true,
                    IsMaterialChecked = getCheckboxCheckStatus("Light Crack Repair"),
                    IsMaterialEnabled = getCheckboxEnabledStatus("Light Crack Repair"),
                    Name = "Light Crack Repair",
                    SMUnits = "Sq Ft",
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,

                    SMSqftH = sqh,
                    Operation = "CAULK",
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = getSqFtStairs("Light Crack Repair"),
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs,
                    Qty = qty,
                    LaborExtension = labrExt,
                    LaborUnitPrice = labrExt / (riserCount + totalSqft),
                    FreightExtension = w * qty,
                    MaterialExtension = mp * qty,
                    IncludeInLaborMinCharge = true

                });
                int.TryParse(materialDetails[1][2].ToString(), out cov);
                double.TryParse(materialDetails[1][0].ToString(), out mp);
                double.TryParse(materialDetails[1][3].ToString(), out w);
                lfArea = getlfArea("Large Crack Repair");

                double.TryParse(materialDetails[1][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[1][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[1][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
                sqh = getSqFtAreaH("Large Crack Repair");
                sqStairs = getSqFtStairs("Large Crack Repair");

            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Large Crack Repair", cov, lfArea);
                smP.Add(new SystemMaterial
                {
                    IsWWR = true,
                    IsMaterialChecked = getCheckboxCheckStatus("Large Crack Repair"),
                    IsMaterialEnabled = true, /*getCheckboxEnabledStatus("Large Crack Repair"),*/
                    Name = "Large Crack Repair",
                    SMUnits = "LF",
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,
                    Qty = qty,

                    SMSqftH = sqh,
                    //Operation = "CAULK",
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = sqStairs,
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs,
                    LaborExtension = labrExt,
                    LaborUnitPrice = labrExt / (riserCount + totalSqft),
                    FreightExtension = w * qty,
                    MaterialExtension = mp * qty,
                    IncludeInLaborMinCharge = true

                });
                int.TryParse(materialDetails[2][2].ToString(), out cov);
                double.TryParse(materialDetails[2][0].ToString(), out mp);
                double.TryParse(materialDetails[2][3].ToString(), out w);
                lfArea = getlfArea("Bubble Repair(Measure Sq Ft)");
                double.TryParse(materialDetails[2][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[2][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[2][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Bubble Repair(Measure Sq Ft)");
                sqStairs = getSqFtStairs("Bubble Repair(Measure Sq Ft)");
                calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
                labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Bubble Repair(Measure Sq Ft)", cov, lfArea);
                smP.Add(new SystemMaterial
                {
                    IsWWR = true,
                    IsMaterialChecked = getCheckboxCheckStatus("Bubble Repair(Measure Sq Ft)"),
                    IsMaterialEnabled = true,/*getCheckboxEnabledStatus("Bubble Repair(Measure Sq Ft)"),*/
                    Name = "Bubble Repair(Measure Sq Ft)",
                    SMUnits = "Sq Ft",
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,
                    Qty = qty,
                    SMSqftH = sqh,
                    Operation = "CAULK",
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = sqStairs,
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs,
                    LaborExtension = labrExt,
                    LaborUnitPrice = labrExt / (riserCount + totalSqft),
                    FreightExtension = w * qty,
                    MaterialExtension = mp * qty,
                    IncludeInLaborMinCharge = true
                });
            
            
            int.TryParse(materialDetails[4][2].ToString(), out cov);
            double.TryParse(materialDetails[4][0].ToString(), out mp);
            double.TryParse(materialDetails[4][3].ToString(), out w);
            lfArea = getlfArea("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale");
            double.TryParse(materialDetails[4][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[4][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[4][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale");
            sqStairs = getSqFtStairs("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale", cov, lfArea);
            smP.Add(new SystemMaterial
            {

                IsMaterialChecked = getCheckboxCheckStatus("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale"),
                IsMaterialEnabled = getCheckboxEnabledStatus("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale"),
                Name = "30# Divorcing Felt (200 Sq Ft) From Ford Wholesale",
                SMUnits = "ROLL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "SLIP SHEET",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[5][2].ToString(), out cov);
            double.TryParse(materialDetails[5][0].ToString(), out mp);
            double.TryParse(materialDetails[5][3].ToString(), out w);
            lfArea = getlfArea("Rp Fabric 10 Inch Wide X (300 Lf) From Acme");
            double.TryParse(materialDetails[5][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[5][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[5][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Rp Fabric 10 Inch Wide X (300 Lf) From Acme");
            sqStairs = getSqFtStairs("Rp Fabric 10 Inch Wide X (300 Lf) From Acme");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Rp Fabric 10 Inch Wide X (300 Lf) From Acme", cov, lfArea);
            smP.Add(new SystemMaterial
            {

                IsMaterialChecked = getCheckboxCheckStatus("Rp Fabric 10 Inch Wide X (300 Lf) From Acme"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Rp Fabric 10 Inch Wide X (300 Lf) From Acme"),
                Name = "Rp Fabric 10 Inch Wide X (300 Lf) From Acme",
                SMUnits = "ROLL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "DETAIL STAIRS ONLY",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Rp Fabric 10 Inch Wide X (300 Lf) From Acme"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[6][2].ToString(), out cov);
            double.TryParse(materialDetails[6][0].ToString(), out mp);
            double.TryParse(materialDetails[6][3].ToString(), out w);
            lfArea = getlfArea("Glasmat #4 (1200 Sq Ft) From Acme");
            double.TryParse(materialDetails[6][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[6][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[6][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Glasmat #4 (1200 Sq Ft) From Acme");
            sqStairs = getSqFtStairs("Glasmat #4 (1200 Sq Ft) From Acme");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs>0?calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate:0;
            qty = getQuantity("Glasmat #4 (1200 Sq Ft) From Acme", cov, lfArea);
            smP.Add(new SystemMaterial
            {

                IsMaterialChecked = getCheckboxCheckStatus("Glasmat #4 (1200 Sq Ft) From Acme"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Glasmat #4 (1200 Sq Ft) From Acme"),
                Name = "Glasmat #4 (1200 Sq Ft) From Acme",
                SMUnits = "ROLL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "INSTALL FIELD GLASS",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Glasmat #4 (1200 Sq Ft) From Acme"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[7][2].ToString(), out cov);
            double.TryParse(materialDetails[7][0].ToString(), out mp);
            double.TryParse(materialDetails[7][3].ToString(), out w);
            double.TryParse(materialDetails[7][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[7][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[7][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Cpc Membrane");
            lfArea = getlfArea("Cpc Membrane");
            sqStairs = getSqFtStairs("Cpc Membrane");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Cpc Membrane", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Cpc Membrane"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Cpc Membrane"),
                Name = "Cpc Membrane",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "SATURATE GLASS & DETAIL PERIMETER",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Cpc Membrane"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[8][2].ToString(), out cov);
            double.TryParse(materialDetails[8][0].ToString(), out mp);
            double.TryParse(materialDetails[8][3].ToString(), out w);
            lfArea = getlfArea("Neotex-38 Paste");
            double.TryParse(materialDetails[8][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[8][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[8][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Neotex-38 Paste");
            sqStairs = getSqFtStairs("Neotex-38 Paste");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs == 0 ? 0: ( calcHrs +setUpMin ) * laborRate ;
            qty = getQuantity("Neotex-38 Paste", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Neotex-38 Paste"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Neotex-38 Paste"),
                Name = "Neotex-38 Paste",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ADD TO BODY COAT",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Neotex-38 Paste"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[9][2].ToString(), out cov);
            double.TryParse(materialDetails[9][0].ToString(), out mp);
            double.TryParse(materialDetails[9][3].ToString(), out w);
            lfArea = getlfArea("Neotex Standard Powder(Body Coat)");
            double.TryParse(materialDetails[9][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[9][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[9][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Neotex Standard Powder(Body Coat)");
            sqStairs = getSqFtStairs("Neotex Standard Powder(Body Coat)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Neotex Standard Powder(Body Coat)", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Neotex Standard Powder(Body Coat)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Neotex Standard Powder(Body Coat)"),
                Name = "Neotex Standard Powder(Body Coat)",
                SMUnits = "45 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Neotex Standard Powder(Body Coat)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[10][2].ToString(), out cov);
            double.TryParse(materialDetails[10][0].ToString(), out mp);
            double.TryParse(materialDetails[10][3].ToString(), out w);
            lfArea = getlfArea("Neotex Standard Powder(Body Coat) 1");
            double.TryParse(materialDetails[10][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[10][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[10][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Neotex Standard Powder(Body Coat) 1");
            sqStairs = getSqFtStairs("Neotex Standard Powder(Body Coat) 1");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Neotex Standard Powder(Body Coat) 1", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Neotex Standard Powder(Body Coat) 1"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Neotex Standard Powder(Body Coat) 1"),
                Name = "Neotex Standard Powder(Body Coat) 1",
                SMUnits = "45 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Neotex Standard Powder(Body Coat) 1"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[11][2].ToString(), out cov);
            double.TryParse(materialDetails[11][0].ToString(), out mp);
            double.TryParse(materialDetails[11][3].ToString(), out w);
            lfArea = getlfArea("Resistite Liquid");
            double.TryParse(materialDetails[11][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[11][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[11][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Resistite Liquid");
            sqStairs = getSqFtStairs("Resistite Liquid");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs == 0 ? 0 : (calcHrs + setUpMin) * laborRate;
            qty = getQuantity("Resistite Liquid", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Liquid"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Liquid"),
                Name = "Resistite Liquid",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ADD TO TAN FILLER",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Resistite Liquid"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            
            int.TryParse(materialDetails[13][2].ToString(), out cov);
            double.TryParse(materialDetails[13][0].ToString(), out mp);
            double.TryParse(materialDetails[13][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular White");
            double.TryParse(materialDetails[13][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[13][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[13][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Resistite Regular White");
            sqStairs = getSqFtStairs("Resistite Regular White");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs == 0 ? 0 : (calcHrs + setUpMin) * laborRate;
            qty = getQuantity("Resistite Regular White", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular White"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular White"),
                Name = "Resistite Regular White",
                SMUnits = "55 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Resistite Regular White"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });


            int.TryParse(materialDetails[15][2].ToString(), out cov);
            double.TryParse(materialDetails[15][0].ToString(), out mp);
            double.TryParse(materialDetails[15][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular Or Smooth White(Knock Down Or Smooth)");
            double.TryParse(materialDetails[15][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[15][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[15][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Resistite Regular Or Smooth White(Knock Down Or Smooth)");
            sqStairs = getSqFtStairs("Resistite Regular Or Smooth White(Knock Down Or Smooth)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs == 0 ? 0 : (calcHrs + setUpMin) * laborRate;
            qty = getQuantity("Resistite Regular Or Smooth White(Knock Down Or Smooth)", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Or Smooth White(Knock Down Or Smooth)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Or Smooth White(Knock Down Or Smooth)"),
                Name = "Resistite Regular Or Smooth White(Knock Down Or Smooth)",
                SMUnits = "40 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Resistite Regular Or Smooth White(Knock Down Or Smooth)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[16][2].ToString(), out cov);
            double.TryParse(materialDetails[16][0].ToString(), out mp);
            double.TryParse(materialDetails[16][3].ToString(), out w);
            lfArea = getlfArea("Aj-44A Dressing(Sealer)");
            double.TryParse(materialDetails[16][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[16][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[16][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Aj-44A Dressing(Sealer)");
            sqStairs = getSqFtStairs("Aj-44A Dressing(Sealer)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Aj-44A Dressing(Sealer)", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = false,//getCheckboxCheckStatus("Aj-44A Dressing(Sealer)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Aj-44A Dressing(Sealer)"),
                Name = "Aj-44A Dressing(Sealer)",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ROLL 2 COATS",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Aj-44A Dressing(Sealer)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[17][2].ToString(), out cov);
            double.TryParse(materialDetails[17][0].ToString(), out mp);
            double.TryParse(materialDetails[17][3].ToString(), out w);
            lfArea = getlfArea("Vista Paint Acripoxy");
            double.TryParse(materialDetails[17][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[17][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[17][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Vista Paint Acripoxy");
            sqStairs = getSqFtStairs("Vista Paint Acripoxy");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Vista Paint Acripoxy", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = false,
                IsMaterialEnabled = getCheckboxEnabledStatus("Vista Paint Acripoxy"),
                Name = "Vista Paint Acripoxy",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ROLL 2 COATS",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Vista Paint Acripoxy"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[18][2].ToString(), out cov);
            double.TryParse(materialDetails[18][0].ToString(), out mp);
            double.TryParse(materialDetails[18][3].ToString(), out w);
            lfArea = getlfArea("Lip Color");
            double.TryParse(materialDetails[18][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[18][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[18][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Lip Color");
            sqStairs = getSqFtStairs("Lip Color");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Lip Color", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Lip Color"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Lip Color"),
                Name = "Lip Color",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ROLL 2 COATS",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Lip Color"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });

            int.TryParse(materialDetails[19][2].ToString(), out cov);
            double.TryParse(materialDetails[19][0].ToString(), out mp);
            double.TryParse(materialDetails[19][3].ToString(), out w);
            lfArea = getlfArea("Resistite Universal Primer(Add 50% Water)");
            double.TryParse(materialDetails[19][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[19][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[19][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Resistite Universal Primer(Add 50% Water)");
            sqStairs = getSqFtStairs("Resistite Universal Primer(Add 50% Water)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Resistite Universal Primer(Add 50% Water)", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Universal Primer(Add 50% Water)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Universal Primer(Add 50% Water)"),
                Name = "Resistite Universal Primer(Add 50% Water)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "PRIMER: SPRAY OR ROLL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Resistite Universal Primer(Add 50% Water)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });

                      
            int.TryParse(materialDetails[21][2].ToString(), out cov);
            double.TryParse(materialDetails[21][0].ToString(), out mp);
            double.TryParse(materialDetails[21][3].ToString(), out w);
            lfArea = getlfArea("Custom Texture Skip Trowel(Resistite Smooth White)");
            double.TryParse(materialDetails[21][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[21][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[21][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Custom Texture Skip Trowel(Resistite Smooth White)");
            sqStairs = getSqFtStairs("Custom Texture Skip Trowel(Resistite Smooth White)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Custom Texture Skip Trowel(Resistite Smooth White)", cov, lfArea);
            bool isch = getCheckboxCheckStatus("Custom Texture Skip Trowel(Resistite Smooth White)");

            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IncludeInLaborMinCharge = true,

                IsMaterialEnabled = getCheckboxEnabledStatus("Custom Texture Skip Trowel(Resistite Smooth White)"),
                Name = "Custom Texture Skip Trowel(Resistite Smooth White)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Custom Texture Skip Trowel(Resistite Smooth White)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                Qty = qty,
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IsMaterialChecked = isch,
                

            });
            int.TryParse(materialDetails[22][2].ToString(), out cov);
            double.TryParse(materialDetails[22][0].ToString(), out mp);
            double.TryParse(materialDetails[22][3].ToString(), out w);
            lfArea = getlfArea("Weather Seal XL two Coats");
            double.TryParse(materialDetails[22][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[22][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[22][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Weather Seal XL two Coats");
            sqStairs = getSqFtStairs("Weather Seal XL two Coats");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Weather Seal XL two Coats", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Weather Seal XL two Coats"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Weather Seal XL two Coats"),
                Name = "Weather Seal XL two Coats",
                IncludeInLaborMinCharge = true,
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Weather Seal XL two Coats"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[23][2].ToString(), out cov);
            double.TryParse(materialDetails[23][0].ToString(), out mp);
            double.TryParse(materialDetails[23][3].ToString(), out w);
            double.TryParse(materialDetails[23][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[23][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[23][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Stair Nosing From Dexotex");
            lfArea = getlfArea("Stair Nosing From Dexotex");
            sqStairs = getSqFtStairs("Stair Nosing From Dexotex");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            qty = getQuantity("Stair Nosing From Dexotex", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Stair Nosing From Dexotex"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Stair Nosing From Dexotex"),
                Name = "Stair Nosing From Dexotex",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                IncludeInLaborMinCharge = true,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "NAIL OR SCREW",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Stair Nosing From Dexotex"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[24][2].ToString(), out cov);
            double.TryParse(materialDetails[24][0].ToString(), out mp);
            double.TryParse(materialDetails[24][3].ToString(), out w);
            lfArea = getlfArea("Extra Stair Nosing Lf");

            setUpMin = 0;
            double.TryParse(materialDetails[24][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[24][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Extra Stair Nosing Lf");
            sqStairs = getSqFtStairs("Extra Stair Nosing Lf"); //getvalue from systemMaterial
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            //qty = 1;
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Extra Stair Nosing Lf"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Extra Stair Nosing Lf"),
                Name = "Extra Stair Nosing Lf",
                IncludeInLaborMinCharge = true,
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "NAIL OR SCREW",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Extra Stair Nosing Lf"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });

            int.TryParse(materialDetails[25][2].ToString(), out cov);
            double.TryParse(materialDetails[25][0].ToString(), out mp);
            double.TryParse(materialDetails[25][3].ToString(), out w);
            lfArea = getlfArea("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
            double.TryParse(materialDetails[25][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[25][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[25][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
            sqStairs = getSqFtStairs("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
                Name = "Plywood 3/4 & Blocking(# Of 4X8 Sheets)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                IncludeInLaborMinCharge = false,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                SMSqftH = sqh,
                Operation = "Remove and replace dry rot",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                Qty = qty,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / qty,
                FreightExtension = w * qty,
                MaterialExtension = mp * qty

            });
            int.TryParse(materialDetails[26][2].ToString(), out cov);
            double.TryParse(materialDetails[26][0].ToString(), out mp);
            double.TryParse(materialDetails[26][3].ToString(), out w);
            lfArea = getlfArea("Stucco Material Remove And Replace (Lf)");
            double.TryParse(materialDetails[26][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[26][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[26][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Stucco Material Remove And Replace (Lf)");
            sqStairs = getSqFtStairs("Stucco Material Remove And Replace (Lf)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = calcHrs > 0 ? calcHrs >= setUpMin ? calcHrs * laborRate : setUpMin * laborRate : 0;
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Stucco Material Remove And Replace (Lf)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Stucco Material Remove And Replace (Lf)"),
                Name = "Stucco Material Remove And Replace (Lf)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                IncludeInLaborMinCharge = false,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "Remove and replace dry rot",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Stucco Material Remove And Replace (Lf)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / qty,
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });

            LaborMinChargeMinSetup = smP.Where(x => x.IsMaterialChecked && x.IncludeInLaborMinCharge == true&&x.LaborExtension!=0).Select(x => x.SetupMinCharge).Sum();
            
            LaborMinChargeHrs = smP.Where(x => x.IsMaterialChecked && x.IncludeInLaborMinCharge == true&&x.LaborExtension!=0).ToList().Select(x => x.Hours).Sum();
            LaborMinChargeLaborExtension = LaborMinChargeMinSetup + LaborMinChargeHrs > 20 ? 0 : (20 - (LaborMinChargeMinSetup + LaborMinChargeHrs) * laborRate);
            LaborMinChargeLaborUnitPrice = LaborMinChargeLaborExtension / (riserCount + totalSqft);

            return smP;
        }
        public override void ApplyCheckUnchecks(object obj)
        {
            base.ApplyCheckUnchecks(obj);
            //CalculateLaborMinCharge(false);

        }
        public override void CalculateLaborMinCharge(bool hasSetupChanged)
        {
            //update Add labor for minimum cost
            base.CalculateLaborMinCharge(hasSetupChanged);
            //LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == false &&
            //                            x.IsMaterialChecked && x.LaborExtension != 0).ToList().Select(x => x.Hours).Sum();
            //LaborMinChargeMinSetup = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == false &&
            //                             x.IsMaterialChecked && x.LaborExtension != 0).ToList().Select(x => x.SetupMinCharge).Sum();
            //LaborMinChargeLaborExtension = (LaborMinChargeMinSetup + LaborMinChargeHrs) > 20 ? 0 :
            //                                    (20 - LaborMinChargeMinSetup - LaborMinChargeHrs) * laborRate;

            //LaborMinChargeLaborUnitPrice = (riserCount + totalSqft) == 0 ? 0 : LaborMinChargeLaborExtension / (riserCount + totalSqft);
            //if(!hasSetupChanged)
            //{
            //    if (LaborMinChargeMinSetup + LaborMinChargeHrs < 20)
            //    {
            //        AddLaborMinCharge = true;
            //    }
            //    else
            //        AddLaborMinCharge = false;
            //}
            //OnPropertyChanged("LaborMinChargeMinSetup");
            //OnPropertyChanged("AddLaborMinCharge");
            //OnPropertyChanged("LaborMinChargeHrs");
            //OnPropertyChanged("LaborMinChargeLaborExtension");
            //OnPropertyChanged("LaborMinChargeLaborUnitPrice");

        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            //return base.getCheckboxCheckStatus(materialName);
            switch (materialName.ToUpper())
            {
                case "LIGHT CRACK REPAIR":
                //case "LARGE CRACK REPAIR":
                //case "BUBBLE REPAIR(MEASURE SQ FT)":
                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                case "CPC MEMBRANE":
                case "NEOTEX-38 PASTE":
                case "NEOTEX STANDARD POWDER(BODY COAT)":
                case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                case "RESISTITE LIQUID":
                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR WHITE":
                case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                //case "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)":
                case "LIP COLOR":
                //case "AJ-44A DRESSING (SEALER)":
                case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                //case "VISTA PAINT ACRIPOXY":
                case "STAIR NOSING FROM DEXOTEX":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                case "WEATHER SEAL XL TWO COATS":
                    return true;
                default:
                    return false;
            }
        }
        public override bool canApply(object obj)
        {
            return true;
            //if (obj != null)
            //{
            //    if (obj.ToString() == "Custom Texture Skip Trowel(Resistite Smooth Gray)")
            //    {
            //        return true;
            //    }
            //    else
            //        return base.canApply(obj);
            //}
            //else
            //    return base.canApply(obj);
            
            
        }
        public override bool getCheckboxEnabledStatus(string materialName)
        {
            //return base.getCheckboxEnabledStatus(materialName);
            switch (materialName.ToUpper())
            {
                case "LIGHT CRACK REPAIR":
                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                case "CPC MEMBRANE":
                case "NEOTEX STANDARD POWDER(BODY COAT)":
                case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                case "RESISTITE REGULAR WHITE":
                case "RESISTITE REGULAR GRAY":
                case "LIP COLOR":
                case "AJ-44A DRESSING(SEALER)":
                case "VISTA PAINT ACRIPOXY":
                case "STAIR NOSING FROM DEXOTEX":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                case "WEATHER SEAL XL TWO COATS":
                    return true;
                default:
                    return false;
            }
        }

        public override void calculateRLqty()
        {
            //base.calculateRLqty();
            //Logic to get LR qty
            double val1 = 0, val2 = 0, val3 = 0,val4=0;
            double qty = 0;
            SystemMaterial skipMat;
            skipMat = SystemMaterials.Where(x => x.Name.ToUpper() == "RESISTITE REGULAR OVER TEXTURE(#55 BAG)").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val1 = skipMat.Qty;
                }
                else
                    val1 = 0;
            }
            skipMat = SystemMaterials.Where(x => x.Name.ToUpper() == "RESISTITE REGULAR OR SMOOTH GRAY(KNOCK DOWN OR SMOOTH)").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val2 = skipMat.Qty;
                }
                else
                    val2 = 0;
            }
            skipMat = SystemMaterials.Where(x => x.Name.ToUpper() == "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val2 = skipMat.Qty;
                }
                else
                    val2 = 0;
            }
            skipMat = SystemMaterials.Where(x => x.Name.ToUpper() == "RESISTITE REGULAR WHITE").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val3 = skipMat.Qty;
                }
                else
                    val3 = 0;
            }
            skipMat = SystemMaterials.Where(x => x.Name.ToUpper() == "RESISTITE REGULAR GRAY").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val3 = skipMat.Qty;
                }
                else
                    val3 = 0;
            }
            skipMat = SystemMaterials.Where(x => x.Name == "Custom Texture Skip Trowel(Resistite Smooth White)").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val4 = skipMat.Qty;
                }
                else
                    val4 = 0;
            }
            skipMat = SystemMaterials.Where(x => x.Name == "Custom Texture Skip Trowel(Resistite Smooth Gray)").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val4 = skipMat.Qty;
                }
                else
                    val4 = 0;
            }
            qty = val3 == 0 ? 0 : (val2 + val3 + val4) * 0.3 + val1 / 5;

            skipMat = SystemMaterials.Where(x => x.Name == "Resistite Liquid").FirstOrDefault();
            if (skipMat != null)
            {
                skipMat.Qty = qty;
            }
            //Neotex 38paste
            skipMat = SystemMaterials.Where(x => x.Name == "Neotex Standard Powder(Body Coat)").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val1 = skipMat.Qty;
                }
                else
                    val1 = 0;
            }
            

            skipMat = SystemMaterials.Where(x => x.Name == "Lip Color").FirstOrDefault();
            if (skipMat != null)
            {
                bool isChecked = skipMat.IsMaterialChecked;
                skipMat.Qty = val4 + val2;
                skipMat.IsMaterialChecked = isChecked;
            }
            skipMat = SystemMaterials.Where(x => x.Name == "Neotex Standard Powder(Body Coat) 1").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val2 = skipMat.Qty;
                }
                else
                    val2 = 0;
            }
            qty = (val1 * 1.25 + val2 * 1.5) / 5;

            skipMat = SystemMaterials.Where(x => x.Name == "Neotex-38 Paste").FirstOrDefault();
            if (skipMat != null)
            {
                skipMat.Qty = qty;
            }

        }

        public override void setExceptionValues(object s)
        {
            if (SystemMaterials.Count==0)
            {
                return;
            }
            //base.setExceptionValues();
            SystemMaterial item = SystemMaterials.Where(x => x.Name == "Large Crack Repair").FirstOrDefault();
            if (item != null)
            {
                item.SMSqftH = item.Qty;
                item.SMSqft = item.Qty;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
            }

            item = SystemMaterials.Where(x => x.Name == "Bubble Repair(Measure Sq Ft)").FirstOrDefault();
            if (item != null)
            {
                item.SMSqft = item.Qty;
                item.SMSqftH = item.Qty;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
            }

            item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & Blocking(# Of 4X8 Sheets)").FirstOrDefault();
            if (item != null)
            {
                item.SMSqftH = item.Qty*32;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                item.LaborUnitPrice = item.LaborExtension / item.Qty;
            }

            item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove And Replace (Lf)").FirstOrDefault();
            if (item != null)
            {
                item.SMSqftH = item.Qty;
                item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                item.LaborUnitPrice = item.LaborExtension / item.Qty;
            }
            item = SystemMaterials.Where(x => x.Name == "Extra Stair Nosing Lf").FirstOrDefault();
            if (item != null)
            {
                item.StairSqft = item.Qty;
                item.Hours = CalculateHrs(0, 0, item.StairSqft, item.StairsProductionRate);
                item.LaborExtension = (item.Hours + item.SetupMinCharge) * laborRate;
                item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
            }

        }
        #endregion
    }
}
