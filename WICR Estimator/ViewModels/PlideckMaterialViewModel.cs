﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    [DataContract]
    public class PlideckMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private bool IsSystemOverConcrete;
        public PlideckMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            
            FillMaterialList();

            FetchMaterialValuesAsync(false);
            resistiteQty();

        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js != null)
            {
                IsSystemOverConcrete = Js.IsSystemOverConcrete;
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        private void FillMaterialList()
        {
            materialNames.Add("2.5 Galvanized Lathe", "EA");
            materialNames.Add("Staples", "BOX");
            materialNames.Add("PD Resin (If dimension exceeds 20 ft in any direction or for below tile)", "5 GAL PAIL");
            materialNames.Add("3/4 oz. Fiberglass", "2000 SQFT ROLL");
            materialNames.Add("Underlay over rough surface (Resistite regular 150 sq ft per mix)", "55 LB BAG");
            materialNames.Add("GU80-1 grey Base Coat", "50LB BAG");
            materialNames.Add("GU80-1 grey skim coat", "50LB BAG");
            materialNames.Add("GU80-1 top coat texture coat semi-smooth or knockdown", "50LB BAG");
            materialNames.Add("GU80-1 liquid", "5 GAL PAIL");
            materialNames.Add("GS88 Sealer", "5 GAL PAIL");
            materialNames.Add("Color Jar Pigment, 1 JAR per PAIL OF GS88", "JAR");
            materialNames.Add("GS13 Clear Sealer", "5 GAL PAIL");
            materialNames.Add("Caulk, dymonic 100", "TUBE 11 OZ.");
            materialNames.Add("Preparation after construction and 50/50 primer", "5 GAL PAIL");
            materialNames.Add("Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)", "50LB BAG");
            materialNames.Add("Resistite Liquid", "5 GAL PAIL");
            materialNames.Add("Stair Nosing", "LF");
            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");



        }

        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Extra stair nosing lf" || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    item.Name == "Stucco Material Remove and replace (LF)")
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

            #region  Update Special Material Pricing and QTY on JobSetup change
            if (hasSetupChanged)
            {
                for (int i = 0; i < SystemMaterials.Count; i++)
                {

                    double sp = SystemMaterials[i].SpecialMaterialPricing;
                    bool iscbChecked = SystemMaterials[i].IsMaterialChecked;
                    bool iscbEnabled = SystemMaterials[i].IsMaterialEnabled;
                    //SystemMaterials[i] = sysMat[i];

                    //SystemMaterials[i].SpecialMaterialPricing = sp;
                    UpdateMe(sysMat[i]);

                    

                    if (iscbEnabled)
                    {
                        //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                        //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                        //SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);
                    }
                    if (SystemMaterials[i].Name == "Extra stair nosing lf" || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                        SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)")
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
            if (hasSetupChanged)
            {
                setCheckBoxes();
            }
            

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
            resistiteQty();
            CalculateCost(null);
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
        }

        bool prevStatus=true;
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "GS13 Clear Sealer":
                case "Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)":
                case "Caulk, dymonic 100":
                    //case "Color Jar Pigment, 1 JAR per PAIL OF GS88":
                    return false;
                case "2.5 Galvanized Lathe":
                case "Staples":
                    return IsSystemOverConcrete == true ? false:prevStatus;
                case "GU80-1 grey Base Coat":
                    return !IsSystemOverConcrete;
                case "Stair Nosing":
                    return riserCount > 0 ? true : false;
                default:
                    return true;
            }
        }
        
        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "Caulk, dymonic 100":
                case "PD Resin (If dimension exceeds 20 ft in any direction or for below tile)":
                case "2.5 Galvanized Lathe":
                case "GS13 Clear Sealer":
                case "Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)":
                case "Underlay over rough surface (Resistite regular 150 sq ft per mix)":
                case "Stair Nosing":
                    
                    return true;
                default:
                    return false;   
            }

        }

        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Stair Nosing":
                    return riserCount * stairWidth;
                case "Caulk, dymonic 100":
                    return totalSqft + deckPerimeter * 4;
                case "Resistite Liquid":
                    return (riserCount * stairWidth * 2) + totalSqft;
                default:
                    return totalSqft + riserCount * stairWidth * 2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                     return 0;
                default:
                    return coverage==0?0:lfArea / coverage;
            }
        }

        public override double getSqFtAreaH(string materialName)
        {
            switch (materialName)
            {
                case "Staples":
                case "Stair Nosing":
                case "Extra stair nosing lf":
                case "Preparation after construction and 50/50 primer":
                case "GU80-1 liquid":
                case "Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)":
                    return 0;
                case "Caulk, dymonic 100":
                    return deckPerimeter;
                default:
                    return totalSqft;
            }

        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {

                case "Staples":
                case "GU80-1 liquid":
                case "Caulk, dymonic 100":
                case "Preparation after construction and 50/50 primer":
                case "Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Stair Nosing":
                    return riserCount * stairWidth;
                case "Resistite Liquid":
                    return 0.0000001;
                default:
                    return riserCount * stairWidth * 2; 

            }
        }

        public override void calculateLaborHrs()
        {
            calLaborHrs(10, totalSqft);

        }
        //calculate for Desert Crete
        public override void calculateRLqty()
        {
            //base.calculateRLqty();
            double val1 = 0, val2 = 0,val3=0,val4=0;
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "GU80-1 grey Base Coat").FirstOrDefault();
            if (sysmat != null)
            {
                val1 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "GU80-1 grey skim coat").FirstOrDefault();
            if (sysmat != null)
            {
                val2 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }

            sysmat = SystemMaterials.Where(x => x.Name == "GU80-1 top coat texture coat semi-smooth or knockdown").FirstOrDefault();
            if (sysmat != null)
            {
                val3=sysmat.IsMaterialChecked ? sysmat.Qty *1.25 : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)").FirstOrDefault();
            if (sysmat != null)
            {
                
                val4 = sysmat.IsMaterialChecked ? sysmat.Qty  : 0;
            }
            sysmat =SystemMaterials.Where(x => x.Name == "GU80-1 liquid").FirstOrDefault();
            if (sysmat != null)
            {
                bool ischecked;
                ischecked = sysmat.IsMaterialChecked;
                sysmat.Qty=(val1+val2+val3+val4) / 5;
                sysmat.IsMaterialChecked = ischecked;
            }
            //CalculateLaborMinCharge(false);

        }

        public override bool canApply(object obj)
        {
            return true;
        }
        public override void setExceptionValues(object s)
        {
            if (SystemMaterials.Count != 0)
            {

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "Extra stair nosing lf").FirstOrDefault();
                if (item != null)
                {

                    item.StairSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);                   
                }

                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty * 32;
                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.MaterialExtension = item.SpecialMaterialPricing == 0 ? item.Qty * item.MaterialPrice : item.Qty * item.SpecialMaterialPricing;
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                calculateRLqty();
                //CalculateLaborMinCharge(false);
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
            return totalSqft + sqftVert + sqftHor==0?0:laborExtension / (totalSqft+ sqftVert+sqftHor);
        }
        public override void ApplyCheckUnchecks(object obj)
        {
            if (obj.ToString() == "Stair Nosing")
            {
                var material = SystemMaterials.FirstOrDefault(x => x.Name == "Stair Nosing");
                if (material != null)
                {
                    stairNosingCheckValue = material.IsMaterialChecked;
                }

            }
            //set RL Qty
            if (obj.ToString() == "Underlay over membrane (Resistite regular 150 sq ft per mix )" ||
                obj.ToString() == "Underlay over rough surface (Resistite regular 150 sq ft per mix)" ||
                obj.ToString() == "Resistite textured knockdown finish (smooth or regular per customer)Gray" ||
                obj.ToString() == "Resistite textured knockdown finish (smooth or regular per customer)White" ||
                obj.ToString() == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)" ||
                obj.ToString() == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)")
            {
                resistiteQty();
            }
            lastCheckedMat = obj.ToString();
            bool isChecked;
            if (obj.ToString() == "2.5 Galvanized Lathe")
            {
                isChecked = SystemMaterials.Where(x => x.Name == "2.5 Galvanized Lathe").FirstOrDefault().IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "Staples").FirstOrDefault().IsMaterialChecked = isChecked;
                prevStatus = isChecked;
            }
            if (obj.ToString()== "PD Resin (If dimension exceeds 20 ft in any direction or for below tile)")
            {
                isChecked = SystemMaterials.Where(x => x.Name == "PD Resin (If dimension exceeds 20 ft in any direction or for below tile)").FirstOrDefault().IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "3/4 oz. Fiberglass").FirstOrDefault().IsMaterialChecked = isChecked;
            }

            if (obj.ToString() == "Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)")
            {
                isChecked = SystemMaterials.Where(x => x.Name == "Select Y for protection coat over membrane below tile (GU80-1 TOP COAT)").FirstOrDefault().IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "GU80-1 top coat texture coat semi-smooth or knockdown").FirstOrDefault().IsMaterialChecked = !isChecked;
                SystemMaterials.Where(x => x.Name == "GS88 Sealer").FirstOrDefault().IsMaterialChecked = !isChecked;
                SystemMaterials.Where(x => x.Name == "Color Jar Pigment, 1 JAR per PAIL OF GS88").FirstOrDefault().IsMaterialChecked = !isChecked;

            }
            calculateRLqty();
            //CalculateLaborMinCharge(false);
        }

        public override void setCheckBoxes()
        {
            foreach (SystemMaterial item in SystemMaterials)
            {
                if ( item.Name == "GU80-1 grey Base Coat"||item.Name== "2.5 Galvanized Lathe"|| item.Name == "Staples")
                {
                    
                    item.IsMaterialChecked = getCheckboxCheckStatus(item.Name);
                }
                //if (item.Name == "Staples")
                //{
                //    bool isChecked = SystemMaterials.Where(x => x.Name == "2.5 Galvanized Lathe").FirstOrDefault().IsMaterialChecked;
                //    SystemMaterials.Where(x => x.Name == "Staples").FirstOrDefault().IsMaterialChecked = isChecked;
                //}
            }
        }
        private void resistiteQty()
        {
            double qty = 0, qty1 = 0;
            foreach (var item in SystemMaterials)
            {
                if (item.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)" ||
                    item.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)" ||
                    item.Name == "Resistite textured knockdown finish (smooth or regular per customer)Gray" ||
                    item.Name == "Underlay over rough surface (Resistite regular 150 sq ft per mix)" ||
                    item.Name == "Resistite textured knockdown finish (smooth or regular per customer)White")
                {
                    if (item.IsMaterialChecked)
                    {
                        qty = qty + item.Qty;
                    }

                }
                //if (item.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)" ||
                //    item.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)" ||
                //    item.Name == "Resistite textured knockdown finish (smooth or regular per customer)Gray" ||
                //    item.Name == "Resistite textured knockdown finish (smooth or regular per customer)White")
                //{
                //    if (item.IsMaterialChecked)
                //    {
                //        qty1 = qty1 + item.Qty;
                //    }

                //}
            }
            double val1 = 0;
            var item150 = SystemMaterials.Where(x => x.Name == "Underlay over rough surface (Resistite regular 150 sq ft per mix)").FirstOrDefault();
            if (item150 != null)
            {
                val1 = item150.Qty;
            }
            var resistiteElement = SystemMaterials.Where(x => x.Name == "Resistite Liquid").FirstOrDefault();
            if (resistiteElement != null)
            {
                resistiteElement.Qty = (qty * 0.33) + val1 / 5;
            }
            //SystemMaterials.Where(x => x.Name == "Weather Seal XL Coat").FirstOrDefault().IsMaterialEnabled = true;

            //SystemMaterial skipMat = SystemMaterials.Where(x => x.Name == "Lip Color").FirstOrDefault();
            //if (skipMat != null)
            //{
            //    bool isChecked = skipMat.IsMaterialChecked;
            //    skipMat.Qty = qty1;
            //    //skipMat.IsMaterialChecked = isChecked;
            //    skipMat.UpdateCheckStatus(isChecked);
            //}
        }

    }
}
