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
    public class IndependentMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        public IndependentMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();

            FillMaterialList();

            FetchMaterialValuesAsync(false);

        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Stucco Material Remove and replace (LF)" || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    item.Name == "Extra stair nosing lf")
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

                    //SystemMaterials[i] = sysMat[i];

                    //SystemMaterials[i].SpecialMaterialPricing = sp;

                    //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    //SystemMaterials[i].IsMaterialChecked = iscbChecked;

                    UpdateMe(sysMat[i]);

                    //SystemMaterials[i].UpdateSpecialPricing(sp);
                    SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);

                    if (SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)" || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    SystemMaterials[i].Name == "Extra stair nosing lf")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            //SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];
                            SystemMaterials[i].UpdateQuantity(qtyList[SystemMaterials[i].Name]);
                        }
                    }
                    SystemMaterials[i].UpdateSpecialPricing(sp);
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
                OtherLaborMaterials = OtherMaterials;
            }
            if (SubContractLaborItems.Count == 0)
            {
                SubContractLaborItems = GetLaborItems();
            }
            calculateRLqty();
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
            CalculateCost(null);
        }
        public override void CalculateLaborMinCharge(bool hasSetupChanged)
        {
            
        }
        public override ObservableCollection<OtherItem> GetOtherMaterials()
        {
            ObservableCollection<OtherItem> om = new ObservableCollection<OtherItem>();
            om.Add(new OtherItem { Name = "Access issues?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional prep?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional labor?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Alternate material?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional Move ons?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            om.Add(new OtherItem { Name = "", IsReadOnly = false });
            return om;
        }
        public void FillMaterialList()
        {
            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");
        }

        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return false;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            return false;
        }

        public override void ApplyCheckUnchecks(object obj)
        {
            lastCheckedMat = obj.ToString();
        }
        public override void calculateLaborHrs()
        {
            calLaborHrs(6, totalSqft);
        }
        public override bool canApply(object obj)
        {
            return true;
        }
        
        public override void calculateRLqty()
        {
            
        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                default:
                    return totalSqft + stairWidth * riserCount * 2;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                 
                default:
                    return totalSqft;
            }
        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "Staples (1\" Crown x 3/4 long Box of 13,500 qty)":
                case "Grout and Texture Color Option: TC-40 Liquid Colorant":
                case "Pattern:  1/4 inch grout tape, Standard 12 x 12 tile pattern, tape and labor":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "WP-81 Liquid":
                    return 0;
                case "Stair Nosing":
                    return riserCount * stairWidth;
                default:
                    return riserCount * stairWidth * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "Sheet Membrane; 6\" WP-40 for plywood seams only":
                    return lfArea / 3 / coverage * 0.9;
                case "Sheet Membrane; WP-40 for entire deck (10-YEAR MANUFACTURER WARRANTY REQ)":
                    return (totalSqft + (riserCount * 8)) / (225 * 0.9);
                default:
                    return lfArea / coverage;
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
    }
}
