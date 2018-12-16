using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class DexoMaterialViewModel:MaterialBaseViewModel
    {
        public  DexoMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            FetchMaterialValuesAsync(false);
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

            var sysMat = GetSystemMaterial();
            if (hasSetupChanged)
            {
                for (int i = 0; i < SystemMaterials.Count; i++)
                {

                    double sp = SystemMaterials[i].SpecialMaterialPricing;
                    SystemMaterials[i] = sysMat[i];

                    SystemMaterials[i].SpecialMaterialPricing = sp;

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


            CalculateAllMaterial();
        }

        public override ObservableCollection<SystemMaterial> GetSystemMaterial()
        {
            return base.GetSystemMaterial();
        }

        public override bool getCheckboxCheckStatus(string materialName)
        {
            return base.getCheckboxCheckStatus(materialName);
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            return base.getCheckboxEnabledStatus(materialName);
        }

        public override ObservableCollection<LaborContract> GetLaborItems()
        {
            return base.GetLaborItems();
        }

        public override ObservableCollection<OtherItem> GetOtherMaterials()
        {
            return base.GetOtherMaterials();
        }

        public override void setCheckBoxes()
        {
            base.setCheckBoxes();
        }
        public override void setExceptionValues()
        {
            base.setExceptionValues();
        }
    }
}
