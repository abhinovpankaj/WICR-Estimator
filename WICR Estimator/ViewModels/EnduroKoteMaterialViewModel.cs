﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class EnduroKoteMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        public EnduroKoteMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);
            getEKLQnty();
        }

        private void FillMaterialList()
        {

            materialNames.Add("2.5 Galvanized Lathe (18 s.f.) no less than 12 per sq ft.", "EA");
            materialNames.Add("Staples (3/4 Inch Crown, Box of 13,500)", "BOX");
            materialNames.Add("ENDURO ELA-98 BINDER (2 COATS)", "5 GAL PAIL");

            materialNames.Add("3/4 oz. Fiberglass (2000 sq ft rolls Purchased from Hill Brothers )", "ROLL");
            materialNames.Add("Base Coat EKC Cementitious Mix", "50 LB BAG");
            materialNames.Add("Second Coat Skim Coat EKC Cementitious Mix", "50 LB BAG");
            materialNames.Add("Resistite textured knockdown finish (smooth or regular per customer)Gray", "55 LB BAG");
            
            materialNames.Add("Texture Coat EKC Cementitious Mix", "50 LB BAG");
            
            materialNames.Add("EKL Acrylic Emulsion", "5 GAL PAIL");
            materialNames.Add("EKS Acrylic Top Coat", "5 GAL PAIL");
            materialNames.Add("Caulk, dymonic 100", "TUBE 11 OZ.");
            materialNames.Add("Preparation after construction and 50/50 primer", "5 GAL PAIL");
            materialNames.Add("Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)", "50 LB BAG");
            materialNames.Add("Stair Nosing", "LF");
            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");          

        }

        public override ObservableCollection<SystemMaterial> GetSystemMaterial()
        {
            ObservableCollection<SystemMaterial> smCollection = new ObservableCollection<SystemMaterial>();
            int k = 0;
            foreach (string key in materialNames.Keys)
            {

                smCollection.Add(getSMObject(k, key, materialNames[key]));
                k++;
            }
            return smCollection;

        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Stucco Material Remove And Replace (Lf)" || item.Name == "Plywood 3/4 & Blocking(# Of 4X8 Sheets)" ||
                    item.Name == "Extra Stair Nosing Lf")
                {
                    qtyList.Add(item.Name, item.Qty);
                }

            }

            var sysMat = GetSystemMaterial();

            #region  Update Special Material Pricing and QTY
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
                    SystemMaterials[i].Name == "Extra Stair Nosing Lf")
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

            //setExceptionValues();
            //setCheckBoxes();
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

        public override double getSqFtAreaH(string materialName)
        {
            //return base.getlfArea(materialName);
            
            switch (materialName)

            {
                case "Staples(3 / 4 Inch Crown, Box of 13, 500)":
                case "EKL Acrylic Emulsion":
                case "Stair Nosing":
                case "Extra stair nosing lf":
                case "Preparation after construction and 50/50 primer":
                    return 0.00000001;
                case "":
                    return deckPerimeter;
                case "Plywood 3 / 4 & blocking(# of 4x8 sheets)":
                case "Stucco Material Remove and replace(LF)":
                    return -1;
                default:
                    return totalSqft;

            }
        }

        public override double getSqFtStairs(string materialName)
        {
            //return base.getSqFtStairs(materialName);
            switch (materialName)
            {
                case "2.5 Galvanized Lathe (18 s.f.) no less than 12 per sq ft.":
                case "ENDURO ELA-98 BINDER(2 COATS)":
                case "3/4 oz.Fiberglass(2000 sq ft rolls Purchased from Hill Brothers )":
                case "Base Coat EKC Cementitious Mix":
                case "Second Coat Skim Coat EKC Cementitious Mix":
                case "Texture Coat EKC Cementitious Mix":
                case "Select Y for protection coat over membrane below tile(GU80-1 TOP COAT)":
                    return riserCount * 4 * 2;
                case "Staples(3 / 4 Inch Crown, Box of 13, 500)":
                case "EKL Acrylic Emulsion":
                case "Caulk, dymonic 100":
                case "Preparation after construction and 50/50 primer":
                case "Plywood 3 / 4 & blocking(# of 4x8 sheets)":
                case "Stucco Material Remove and replace(LF)":
                    return 0.00000001;
                case "EKS Acrylic Top Coat":
                    return riserCount * 4.5 * 2;
                case "Stair Nosing":
                    return riserCount * 3.5;
                case "Extra stair nosing lf":
                    return 0;
                default:
                    return 0;
            }
        }

        public override double getlfArea(string materialName)
        {
            
            switch (materialName)
            {
                case "Extra stair nosing lf":
                    return 0;
                case "Caulk, dymonic 100":
                    return deckPerimeter + riserCount * 2 * 2;
                case "Stair Nosing":
                    return riserCount * 4;
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return -1;
                default:
                    return totalSqft + riserCount * 4 * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            //return base.getQuantity(materialName, coverage, lfArea);
            switch (materialName)
            {
                
                default:
                    return lfArea/ coverage;
            }
        }

        private double getEKLQnty()
        {
            double qty = 0;
            foreach (var item in SystemMaterials)
            {
                if (item.Name == "Base Coat EKC Cementitious Mix"||
                    item.Name== "Second Coat Skim Coat EKC Cementitious Mix" ||item.Name== "Texture Coat EKC Cementitious Mix")
                {
                    qty = qty + item.Qty;
                }
            }
            return qty / 5;
           
        }

        public override bool getCheckboxCheckStatus(string materialName)
        {
            //return base.getCheckboxCheckStatus(materialName);
            //            2.5 Galvanized Lathe (18 s.f.) no less than 12 per sq ft.
            //Staples(3 / 4 Inch Crown, Box of 13, 500)
            //ENDURO ELA-98 BINDER(2 COATS)
            //3 / 4 oz.Fiberglass(2000 sq ft rolls Purchased from Hill Brothers )
            //Base Coat EKC Cementitious Mix
            //Second Coat Skim Coat EKC Cementitious Mix
            //Texture Coat EKC Cementitious Mix
            //EKL Acrylic Emulsion
            //EKS Acrylic Top Coat
            //Caulk, dymonic 100
            //Preparation after construction and 50 / 50 primer
            //  Select Y for protection coat over membrane below tile(GU80 - 1 TOP COAT)


            //  Stair Nosing
            //  Extra stair nosing lf
            //  ADD LABOR FOR MINIMUM CHARGE
            //  Plywood 3 / 4 & blocking(# of 4x8 sheets)
            //Stucco Material Remove and replace(LF)
            switch (materialName)
            {
                case "Select Y for protection coat over membrane below tile(GU80-1 TOP COAT)":
                case "Stair Nosing":
                case "Extra stair nosing lf":
                case "Plywood 3 / 4 & blocking(# of 4x8 sheets)":
                case "Stucco Material Remove and replace(LF)":
                    return false;
                default:
                    return  true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            //return base.getCheckboxEnabledStatus(materialName);
            switch (materialName)
            {
                case "ENDURO ELA-98 BINDER (2 COATS)":
                case "Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)":
                    return true;
                default:
                    return false;
            }
        }
         
    }
}
