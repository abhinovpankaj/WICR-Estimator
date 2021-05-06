using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;
using System.Windows.Input;
using System.ComponentModel;

namespace WICR_Estimator.ViewModels
{

    public class MaterialViewModel : MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        public MaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);
            ApplyCheckUnchecks("Aj-44A Dressing(Sealer)");
        }

        private void FillMaterialList()
        {
            materialNames = new Dictionary<string, string>();
            materialNames.Add("Resistite Regular Over Texture(#55 Bag)", "55 LB BAG");
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

        public override string GetOperation(string matName)
        {
            switch (matName)
            {
                case "Resistite Regular Over Texture(#55 Bag)":
                    return "55 LB BAG";
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
                    //SystemMaterials[i] = sysMat[i];

                    //SystemMaterials[i].SpecialMaterialPricing = sp;
                    //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    UpdateMe(sysMat[i]);

                    SystemMaterials[i].UpdateSpecialPricing(sp);
                    SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);

                    if (SystemMaterials[i].Name == "Stucco Material Remove And Replace (Lf)" || SystemMaterials[i].Name == "Plywood 3/4 & Blocking(# Of 4X8 Sheets)" ||
                    SystemMaterials[i].Name == "Extra Stair Nosing Lf" || SystemMaterials[i].Name == "Bubble Repair(Measure Sq Ft)"
                            || SystemMaterials[i].Name == "Large Crack Repair")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            //SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];
                            SystemMaterials[i].UpdateQuantity(qtyList[SystemMaterials[i].Name]);
                        }
                    }

                }

            }
            else
            {
                SystemMaterials = sysMat;
                setCheckBoxes();
            }

            foreach (var mat in SystemMaterials)
            {
                if (mat.Name == "Lip Color" || mat.Name == "Aj-44A Dressing(Sealer)" || mat.Name == "Vista Paint Acripoxy")
                {
                    if (mat.IsMaterialChecked)
                    {
                        ApplyCheckUnchecks(mat.Name);
                        break;
                    }
                }
            }

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

            //CalculateAllMaterial();
            CalculateLaborMinCharge(hasSetupChanged);
            //calculateLaborTotalsWithMinLabor();
            CalculateCost(null);

        }
        public MaterialViewModel()
        { }
    }
}
