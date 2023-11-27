using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    [DataContract]
    class DexellentIIMaterialViewModel:MaterialBaseViewModel
    {
        
        private Dictionary<string, string> materialNames;
        private bool IsSystemOverConcrete;

        private double linearFootageCoping;
        public DexellentIIMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) 
            : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);
            resistiteQty();
        }
        
        private void FillMaterialList()
        {
            materialNames.Add("2.5 Galvanized Lathe (18 s.f.)","EA");
            materialNames.Add("Staples (3/4 Inch Crown, Box of 13,500)", "BOX");
            materialNames.Add("Dexcellent II Base Resin (saturate glass and detail perimeter)", "5 GAL PAIL");
            materialNames.Add("Fiberglass Matt (2000 sq ft rolls from Hill Brothers)", "ROLL");
            materialNames.Add("Base Coat with Sand Cement and Acrylic binder from HD (MIX)", "MIX");
            materialNames.Add("Base Coat w Dexcelcrete Gray Powder", "50 LB BAG");
            materialNames.Add("Slurry with Dexcelcrete Gray Powder", "50 LB BAG");
            materialNames.Add("Texture with Dexcelcrete Gray Powder", "50 LB BAG");
            materialNames.Add("Underlay over rough surface (Resistite regular 150 sq ft per mix)", "55 LB BAG");
            materialNames.Add("Dexcelcrete Liquid Adhesive", "5 GAL PAIL");
            materialNames.Add("(Stairs Only) Base Coat with Sand Cement and Acrylic Binder", "50LB BAG");
            materialNames.Add("(Stairs Only) Base Coat with Dexcelcrete Gray Powder", "50LB BAG");
            materialNames.Add("(Stairs Only) Dexcelcrete Liquid Adhesive", "5 GAL PAIL");
            materialNames.Add("(Stairs Only) Texture with Dexcelcrete Gray Powder and Liquid Adhesive", "50LB BAG");
            materialNames.Add("Vista Paint Acripoxy (TOPCOAT)", "5 GAL PAIL");
            materialNames.Add("Resistite Liquid", "5 GAL PAIL");
            materialNames.Add("Dexcelent II Final Coat (TOPCOAT)", "5 GAL PAIL");
            materialNames.Add("Stair Nosing", "EA");
            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4X8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");

        }
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                linearFootageCoping = js.DeckPerimeter;
                IsSystemOverConcrete = js.IsSystemOverConcrete;
                riserCount = js.RiserCount;
            }

            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();


            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Stucco Material Remove and replace (LF)" || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    item.Name == "Extra stair nosing lf" )
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

                    UpdateMe(sysMat[i]);

                    

                    if (iscbEnabled)
                    {
                        //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                        //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                        //SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);
                    }                   
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
            else
            {
                SystemMaterials = sysMat;
                setCheckBoxes();
            }

            setCheckBoxes();
            setExceptionValues(null);
                        
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
        }

        
        public override void calculateRLqty()
        {

            double val1 = 0, val2 = 0, val3 = 0;
            double qty = 0;
            SystemMaterial sysmat1 = SystemMaterials.Where(x => x.Name == "Base Coat w Dexcelcrete Gray Powder").FirstOrDefault();
            if (sysmat1 != null)
            {
                val1 = sysmat1.IsMaterialChecked ? sysmat1.Qty : 0;
            }
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "Slurry with Dexcelcrete Gray Powder").FirstOrDefault();
            if (sysmat != null)
            {
                val2 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "Texture with Dexcelcrete Gray Powder").FirstOrDefault();
            if (sysmat != null)
            {
                val3 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }

            qty = val1 / 5 + val2 / 5 * 1.25 + val3 / 5;
            SystemMaterial DL = SystemMaterials.Where(x => x.Name == "Dexcelcrete Liquid Adhesive").FirstOrDefault();
            bool ischecked = DL.IsMaterialChecked;
            
            if (DL != null)
            {
                DL.Qty = qty;
                DL.IsMaterialChecked = ischecked;
            }
            sysmat= SystemMaterials.Where(x => x.Name == "(Stairs Only) Base Coat with Dexcelcrete Gray Powder").FirstOrDefault();
            if (sysmat!=null)
            {
                val1 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "(Stairs Only) Texture with Dexcelcrete Gray Powder and Liquid Adhesive").FirstOrDefault();
            if (sysmat != null)
            {
                val2 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }
            qty = (val1 + val2) / 5;
            DL = SystemMaterials.Where(x => x.Name == "(Stairs Only) Dexcelcrete Liquid Adhesive").FirstOrDefault();
            ischecked = DL.IsMaterialChecked;

            if (DL != null)
            {
                DL.Qty = qty;
                DL.IsMaterialChecked = ischecked;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            //return base.getCheckboxCheckStatus(materialName);

            switch (materialName)
            {
                //case "2.5 Galvanized Lathe (18 s.f.)":
                case "(Stairs Only) Texture with Dexcelcrete Gray Powder and Liquid Adhesive":
                case "Texture with Dexcelcrete Gray Powder":
                case "Vista Paint Acripoxy (TOPCOAT)":
                case "Underlay over rough surface (Resistite regular 150 sq ft per mix)":
                case "Stair Nosing":
                    
                    return true;
                default:
                    return false;
            }
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "Base Coat with Sand Cement and Acrylic binder from HD (MIX)":
                case "(Stairs Only) Base Coat with Sand Cement and Acrylic Binder":
                    return IsSystemOverConcrete==true? !IsSystemOverConcrete:isApprovedforCement;
                case "Base Coat w Dexcelcrete Gray Powder":
                case "(Stairs Only) Base Coat with Dexcelcrete Gray Powder":
                    return IsSystemOverConcrete == true ? !IsSystemOverConcrete : !isApprovedforCement;
                case "2.5 Galvanized Lathe (18 s.f.)":
                case "Staples (3/4 Inch Crown, Box of 13,500)":
                
                    return !IsSystemOverConcrete;
                case "Stair Nosing":
                    return riserCount > 0 ? true : false;
                default:
                    return true;
            }
        }
        public override double getlfArea(string materialName)
        {
            //return base.getlfArea(materialName);

            switch (materialName)
            {
                case "Dexcelcrete Liquid Adhesive":
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                case "Base Coat with Sand Cement and Acrylic binder from HD (MIX)":
                case "Base Coat w Dexcelcrete Gray Powder":
                case "Slurry with Dexcelcrete Gray Powder":
                case "Texture with Dexcelcrete Gray Powder":

                    return totalSqft;
                case "(Stairs Only) Base Coat with Sand Cement and Acrylic Binder":
                case "(Stairs Only) Base Coat with Dexcelcrete Gray Powder":
                case "(Stairs Only) Dexcelcrete Liquid Adhesive":
                case "(Stairs Only) Texture with Dexcelcrete Gray Powder and Liquid Adhesive":

                    return riserCount * stairWidth * 2;
                case "Stair Nosing":
                    return riserCount*stairWidth;
                case "Resistite Liquid":
                    return (riserCount * stairWidth * 2) + totalSqft;
                default:
                    
                    return totalSqft + riserCount * stairWidth * 2;
            }

        }
        public override double getSqFtAreaH(string materialName)
        {

            switch (materialName)
            {
                case "Staples (3/4 Inch Crown, Box of 13,500)":
                case "(Stairs Only) Base Coat with Sand Cement and Acrylic Binder":
                case "(Stairs Only) Base Coat with Dexcelcrete Gray Powder":
                case "(Stairs Only) Dexcelcrete Liquid Adhesive":
                case "(Stairs Only) Texture with Dexcelcrete Gray Powder and Liquid Adhesive":
                case "Extra stair nosing lf":
                case "Stair Nosing":
                    return 0;
                default:
                    return totalSqft;
            }
        }

        public override void calculateLaborHrs()
        {
            base.calculateLaborHrs();
            
        }
        public override double getSqFtStairs(string materialName)
        {
            //return base.getSqFtStairs(materialName);
            switch (materialName)
            {

                case "(Stairs Only) Base Coat with Sand Cement and Acrylic Binder":
                case "(Stairs Only) Base Coat with Dexcelcrete Gray Powder":
                case "Vista Paint Acripoxy (TOPCOAT)":
                case "(Stairs Only) Texture with Dexcelcrete Gray Powder and Liquid Adhesive":
                case "Dexcelent II Final Coat (TOPCOAT)":
                case "Underlay over rough surface (Resistite regular 150 sq ft per mix)":
                    return riserCount*stairWidth*2;
                case "Stair Nosing":
                    return riserCount * stairWidth;
                case "Resistite Liquid":
                    return 0.0000001;
                default:
                    return 0;

            }
        }
        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "Fiberglass Matt (2000 sq ft rolls from Hill Brothers)":
                    return linearFootageCoping;
                default:
                    return 0;
            }
        }
        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            return lfArea / coverage;
            
        }


        public override bool canApply(object obj)
        {
            //return base.canApply(obj);

            return true;

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
            if (obj.ToString() == "Vista Paint Acripoxy (TOPCOAT)")
            {
                bool ischecked = SystemMaterials.Where(x => x.Name == "Vista Paint Acripoxy (TOPCOAT)").FirstOrDefault().IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "Dexcelent II Final Coat (TOPCOAT)").FirstOrDefault().IsMaterialChecked = !ischecked;
            }
            calculateRLqty();
            //CalculateLaborMinCharge(false);
            //CalculateAllMaterial();
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
        
        public override void setExceptionValues(object s)
        {
            //base.setExceptionValues();
            if (SystemMaterials.Count != 0)
            {
                SystemMaterial item = SystemMaterials.Where(x => x.Name == "Extra stair nosing lf").FirstOrDefault();
                if (item != null)
                {
                    item.StairSqft = item.Qty;
                    item.SMSqft = 0;          
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = item.Hours==0 ?0: (item.SetupMinCharge + item.Hours) * laborRate ;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                }

                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
                if (item != null)
                {
                    
                    item.SMSqftH = item.Qty*32;
                    item.SMSqft = 0;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = item.Hours == 0 ? 0 : item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                }

                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
                if (item != null)
                {
                    
                    item.SMSqftH = item.Qty;
                    item.SMSqft = 0;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = item.Hours == 0 ? 0 : item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                }
            }

        }
        
        public override void setCheckBoxes()
        {
            bool ischecked = SystemMaterials.Where(x => x.Name == "Vista Paint Acripoxy (TOPCOAT)").FirstOrDefault().IsMaterialChecked;
            SystemMaterials.Where(x => x.Name == "Dexcelent II Final Coat (TOPCOAT)").FirstOrDefault().IsMaterialChecked = !ischecked;

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
            double val1 = SystemMaterials.Where(x => x.Name == "Underlay over rough surface (Resistite regular 150 sq ft per mix)").FirstOrDefault().Qty;
            SystemMaterials.Where(x => x.Name == "Resistite Liquid").FirstOrDefault().Qty = (qty * 0.33) + val1 / 5;

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
