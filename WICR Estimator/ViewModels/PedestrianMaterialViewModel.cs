using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class PedestrianMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;

        public PedestrianMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();

            FetchMaterialValuesAsync(false);

        }
        public override bool canApply(object obj)
        {
            return true;
        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "EXTRA STAIR NOSING" || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    item.Name == "Stucco Material Remove and replace (LF)")
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
                    SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    if (SystemMaterials[i].Name == "EXTRA STAIR NOSING" || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                        SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)")
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

            setExceptionValues();
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
            CalculateAllMaterial();
        }
        private void FillMaterialList()
        {
            materialNames.Add("SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)","0");
            materialNames.Add("REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)", "0");
            materialNames.Add("7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL", "2 GAL KIT");
            materialNames.Add("INTERLAMINATE PRIMER (XYLENE) FROM LOWRYS", "0");
            materialNames.Add("7013 SC BASE COAT/ 5 GAL PAILS 40 MILS", "5 GAL PAIL");
            materialNames.Add("7016 - AR - INTERMEDIATE COAT / 5 GAL PAILS 20 MILS", "5 GAL PAIL");
            materialNames.Add("7016 - AL - SC TOP COAT / 5 GAL PAILS 16 MILS", "5 GAL PAIL");
            materialNames.Add("1/20 SAND/ #100 LB", "100 LB BAG");
            materialNames.Add("3 IN. WHITE GLASS TAPE (PERIMETER)", "150' ROLL");
            materialNames.Add("SIKA 1-A CAULKING (PERIMETER)", "TUBE");
            materialNames.Add("DETAIL TAPE (NEW PLYWOOD)", "150' ROLL");
            materialNames.Add("SIKA 1-A CAULKING (NEW PLYWOOD)", "TUBE");
           
            materialNames.Add("UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT", "1 1/2 GAL KIT");
            materialNames.Add("9801 ACCELERATOR", "GALLON");
            materialNames.Add("STAIR NOSING OVER CONCRETE", " ");
            materialNames.Add("INTEGRAL STAIR NOSING (EXCEL STYLE)", "LF");
            materialNames.Add("EXTRA STAIR NOSING", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");

        }

        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)":
                case "7012 EPOXY PRIMER AND PREPARATION FOR RE-SEAL":
                case "DETAIL TAPE (NEW PLYWOOD)":
                case "SIKA 1-A CAULKING (NEW PLYWOOD)":
                case "UI 7118 CONCRETE PRIMER 1-1/2 GAL KIT":
                case "STAIR NOSING OVER CONCRETE":
                case "EXTRA STAIR NOSING":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return false;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "SLOPING FOR TREADS IF NOT PROVIDED FOR IN FRAMING (MOST CASES NEED SLOPE)":
                    return true;
                default:
                    return false;
            }
        }

        public override double getlfArea(string materialName)
        {
            return base.getlfArea(materialName);
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            return base.getQuantity(materialName, coverage, lfArea);
        }

        public override double getSqFtAreaH(string materialName)
        {
            return base.getSqFtAreaH(materialName);
        }
    }
        
    
}
