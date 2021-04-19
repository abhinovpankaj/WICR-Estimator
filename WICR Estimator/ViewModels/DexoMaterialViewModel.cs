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
    public class DexoMaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        public  DexoMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);
            //set  resititeLiquid QTY
            resistiteQty();
        }

        public override bool canApply(object obj)
        {
            return true;
        }
        private void FillMaterialList()
        {

            materialNames.Add("Sand or pressure wash to prepare area", "SQ FT");
            materialNames.Add("Barrier Guard membrane over smooth surface", "5 GAL PAIL");
            materialNames.Add("Underlay over membrane (Resistite regular 150 sq ft per mix )", "55 LB BAG");
            
            materialNames.Add("R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)", "LF");
            materialNames.Add("Underlay over rough surface (Resistite regular 150 sq ft per mix)", "55 LB BAG");
            materialNames.Add("RESISTITE UNIVERSAL PRIMER (ADD 50% WATER)", "5 GAL PAIL");
            materialNames.Add("Resistite textured knockdown finish (smooth or regular per customer)Gray", "55 LB BAG");
            //materialNames.Add("Resistite textured knockdown finish (smooth or regular per customer)White", "55 LB BAG");
            materialNames.Add("CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)", "40 LB BAG");
            //materialNames.Add("CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)", "40 LB BAG");
            materialNames.Add("Aj-44A Dressing(Sealer)", "5 GAL PAIL");
            materialNames.Add("Lip Color", "5 GAL PAIL");
            materialNames.Add("Vista Paint Acripoxy", "5 GAL PAIL");
            materialNames.Add("Weather Seal XL Coat", "SQ FT");
            materialNames.Add("Glass mat #4 1200 SF ROLL FROM ACME", "ROLL");
            materialNames.Add("Resistite Liquid ", "5 GAL PAIL");
            materialNames.Add("RP FABRIC 10 INCH WIDE X (300 LF)", "ROLL");
            materialNames.Add("Stair Nosing From Dexotex", "LF");
            materialNames.Add("Extra stair nosing lf", "LF");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 Sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");
            

        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Stucco Material Remove and replace (LF)" || item.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    item.Name == "Extra stair nosing lf" || item.Name== "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)")
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
                    SystemMaterials[i] = sysMat[i];

                    SystemMaterials[i].SpecialMaterialPricing = sp;
                    SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    if (SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)" || 
                        SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)" ||
                    SystemMaterials[i].Name == "Extra stair nosing lf"
                            || SystemMaterials[i].Name == "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)")
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
                OtherLaborMaterials = OtherMaterials;
            }


            if (SubContractLaborItems.Count == 0)
            {
                SubContractLaborItems = GetLaborItems();
            }
            //set  resititeLiquid QTY
            resistiteQty();
            CalculateCost(null);
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
        }
        private DBData dbData;
        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {          
            base.JobSetup_OnJobSetupChange(sender, e);
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                if (js.DeckPerimeter > 0)
                {
                    SystemMaterials.Where(x => x.Name == "RP FABRIC 10 INCH WIDE X (300 LF)").First().IsMaterialChecked = true;
                }
                else
                {
                    SystemMaterials.Where(x => x.Name == "RP FABRIC 10 INCH WIDE X (300 LF)").First().IsMaterialChecked = false;
                }
                dbData = js.dbData;
            }
        }
        public override ObservableCollection<SystemMaterial> GetSystemMaterial()
        {
            ObservableCollection<SystemMaterial> smCollection = new ObservableCollection<SystemMaterial>();
            int k = 0;
            foreach (string key in materialNames.Keys)
            {
                SystemMaterial sm = getSMObject(k, key, materialNames[key]);
                
                smCollection.Add(sm);
                if (key == "Resistite textured knockdown finish (smooth or regular per customer)Gray"||
                    key== "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)")
                {
                    k = k + 2;
                }
                else
                    k++;
                if (key== "RP FABRIC 10 INCH WIDE X (300 LF)")
                {
                    sm.VerticalProductionRate = 100*(1+prPerc);
                    sm.VerticalSqft = deckPerimeter;
                    sm.Hours = deckPerimeter / sm.VerticalProductionRate;
                    sm.LaborExtension = (sm.SetupMinCharge + sm.Hours) * laborRate;
                    sm.LaborUnitPrice = sm.LaborExtension / (riserCount+totalSqft);
                }
            }
            double minLCharge = 0;
            double.TryParse(materialDetails[k][6].ToString(), out minLCharge);
            LaborMinChargeMinSetup = minLCharge;

            return smCollection;

        }

        public override bool IncludedInLaborMin(string matName)
        {
            switch (matName)
            {
                //case "Stair Nosing From Dexotex":
                //case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return false;
                default:
                    return true;
            }
        }
        
        public override void calculateLaborHrs()
        {
            calLaborHrs(6,totalSqft);

        }
        public override double getSqFtAreaH(string materialName)
        {
            //return base.getSqFtAreaH(materialName);
            switch (materialName)
            {

                case "Sand or pressure wash to prepare area":
                case "Barrier Guard membrane over smooth surface":
                case "Underlay over membrane (Resistite regular 150 sq ft per mix )":
                case "Underlay over rough surface (Resistite regular 150 sq ft per mix)":
                case "RESISTITE UNIVERSAL PRIMER (ADD 50% WATER)":
                case "Resistite textured knockdown finish (smooth or regular per customer)Gray":
                case "Resistite textured knockdown finish (smooth or regular per customer)White":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)":
                case "Aj-44A Dressing(Sealer)":
                case "Vista Paint Acripoxy":
                case "Lip Color":
                case "Weather Seal XL Coat":
                case "Glass mat #4 1200 SF ROLL FROM ACME":
                    return totalSqft;
                case "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)":
                    return -1;
                default:
                    return 0;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            //return base.getQuantity(materialName, coverage, lfArea);
            switch (materialName)
            {

                case "Sand or pressure wash to prepare area":
                case "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)":
                    return 0;
                case "Barrier Guard membrane over smooth surface":
                case "Underlay over membrane (Resistite regular 150 sq ft per mix )":
                case "Underlay over rough surface (Resistite regular 150 sq ft per mix)":
                case "RESISTITE UNIVERSAL PRIMER (ADD 50% WATER)":
                case "Resistite textured knockdown finish (smooth or regular per customer)Gray":
                case "Resistite textured knockdown finish (smooth or regular per customer)White":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)":
                
                case "RP FABRIC 10 INCH WIDE X (300 LF)":
                
                case "Weather Seal XL Coat":
                case "Glass mat #4 1200 SF ROLL FROM ACME":
                    return lfArea/coverage;
                case "Aj-44A Dressing(Sealer)":
                case "Vista Paint Acripoxy":
                case "Lip Color":
                    return lfArea / coverage < 0.5 ? 0.5 : lfArea / coverage;
                case "Stair Nosing From Dexotex":
                    return lfArea * 4;
                default:
                    return 0;
            }
        }

        private void setValuesRPFabric()
        {

        }
        private void resistiteQty()
        {
            double qty = 0,qty1=0;
            foreach (var item in SystemMaterials)
            {
                if (item.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)" ||
                    item.Name== "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)"||
                    item.Name == "Resistite textured knockdown finish (smooth or regular per customer)Gray" ||
                    item.Name == "Underlay over rough surface (Resistite regular 150 sq ft per mix)" ||
                    item.Name == "Resistite textured knockdown finish (smooth or regular per customer)White")
                {
                    if (item.IsMaterialChecked)
                    {
                        qty = qty + item.Qty;
                    }

                }
                if (item.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)" ||
                    item.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)" ||
                    item.Name == "Resistite textured knockdown finish (smooth or regular per customer)Gray" ||
                    item.Name == "Resistite textured knockdown finish (smooth or regular per customer)White")
                {
                    if (item.IsMaterialChecked)
                    {
                        qty1 = qty1 + item.Qty;
                    }

                }
            }
            double val1 = SystemMaterials.Where(x => x.Name == "Underlay over membrane (Resistite regular 150 sq ft per mix )").FirstOrDefault().Qty;
            SystemMaterials.Where(x=>x.Name== "Resistite Liquid ").FirstOrDefault().Qty= (qty *0.33)+val1/5;

            SystemMaterials.Where(x => x.Name == "Weather Seal XL Coat").FirstOrDefault().IsMaterialEnabled = true;

            SystemMaterial skipMat = SystemMaterials.Where(x => x.Name == "Lip Color").FirstOrDefault();
            if (skipMat != null)
            {
                bool isChecked = skipMat.IsMaterialChecked;
                skipMat.Qty = qty1;
                skipMat.IsMaterialChecked = isChecked;
            }
        }

        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {

                case "Sand or pressure wash to prepare area":
                case "Barrier Guard membrane over smooth surface":
                case "Underlay over membrane (Resistite regular 150 sq ft per mix )":
                case "Underlay over rough surface (Resistite regular 150 sq ft per mix)":
                case "RESISTITE UNIVERSAL PRIMER (ADD 50% WATER)":
                case "Resistite textured knockdown finish (smooth or regular per customer)Gray":
                case "Resistite textured knockdown finish (smooth or regular per customer)White":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)":
                case "Aj-44A Dressing(Sealer)":
                case "Vista Paint Acripoxy":
                case "Lip Color":
                case "Weather Seal XL Coat":
                case "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)":
                     return riserCount*stairWidth*2;
                case "Glass mat #4 1200 SF ROLL FROM ACME":
                case "Resistite Liquid":
                case "RP FABRIC 10 INCH WIDE X (300 LF)":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0.0000001;
                case "Extra stair nosing lf":
                    return -1;
                case "Stair Nosing From Dexotex":
                    return riserCount * stairWidth;
                default:
                    return 0;
            }
        }
        public override double getlfArea(string materialName)
        {
            //string upp = materialName.ToUpper();
            switch (materialName)
            {
                
                case "Underlay over rough surface (Resistite regular 150 sq ft per mix)":
                case "RESISTITE UNIVERSAL PRIMER (ADD 50% WATER)":
                case "Resistite textured knockdown finish (smooth or regular per customer)Gray":
                case "Resistite textured knockdown finish (smooth or regular per customer)White":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)":
                case "Aj-44A Dressing(Sealer)":
                case "Vista Paint Acripoxy":
                case "Lip Color":
                case "Weather Seal XL Coat":
                case "Glass mat #4 1200 SF ROLL FROM ACME":
                case "Resistite Liquid ":
                case "Underlay over membrane (Resistite regular 150 sq ft per mix )":
                case "Sand or pressure wash to prepare area":
                    return (riserCount * stairWidth * 2) + totalSqft;
                case "Barrier Guard membrane over smooth surface":
                    return (riserCount * stairWidth * 2) + totalSqft+deckPerimeter;
                case "RP FABRIC 10 INCH WIDE X (300 LF)":
                    return deckPerimeter + stairWidth * 2;
                case "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)":
                    return -1;
                case "Stair Nosing From Dexotex":
                    return riserCount; ;
                default:
                    return 0;
            }
        }

        public override bool getCheckboxCheckStatus(string materialName)
        {
            //return base.getCheckboxCheckStatus(materialName);
            switch (materialName)
            {

                case "Sand or pressure wash to prepare area":
                case "Barrier Guard membrane over smooth surface":
                case "Underlay over membrane (Resistite regular 150 sq ft per mix )":
                case "Underlay over rough surface (Resistite regular 150 sq ft per mix)":
                case "RESISTITE UNIVERSAL PRIMER (ADD 50% WATER)":
                case "Resistite textured knockdown finish (smooth or regular per customer)Gray":
                case "Resistite textured knockdown finish (smooth or regular per customer)White":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)":
                case "Aj-44A Dressing(Sealer)":
               
                case "RP FABRIC 10 INCH WIDE X (300 LF)":
                case "Glass mat #4 1200 SF ROLL FROM ACME":
                case "Stair Nosing From Dexotex":
                case "Resistite Liquid":
                case "Weather Seal XL Coat":
                    return true;
                case "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "Extra stair nosing lf":
                case "Vista Paint Acripoxy":
                case "Lip Color":
                
                    return false;
                default:
                    return true;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            //return base.getCheckboxEnabledStatus(materialName);
            switch (materialName)
            {

                
                case "Barrier Guard membrane over smooth surface":
                
                case "Underlay over rough surface (Resistite regular 150 sq ft per mix)":
                
                case "Resistite textured knockdown finish (smooth or regular per customer)Gray":
                case "Resistite textured knockdown finish (smooth or regular per customer)White":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)":
                case "Stair Nosing From Dexotex":
                case "Lip Color":
                case "Weather Seal XL Coat":
                case "Vista Paint Acripoxy":
                    return true;
                case "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                case "Extra stair nosing lf":
                    return false;
                default:
                    return false;
            }
        }

        //Handler to rest values of materials on basis of Lipcolor,Vista Aj44 seletion
        public override void ApplyCheckUnchecks(object obj)
        {
            if (obj == null)
            {
                return;
            }
            //set RL Qty
            if (obj.ToString() == "Underlay over membrane (Resistite regular 150 sq ft per mix )" ||
                obj.ToString() == "Underlay over rough surface (Resistite regular 150 sq ft per mix)" ||
                obj.ToString() == "Resistite textured knockdown finish (smooth or regular per customer)Gray"||
                obj.ToString() == "Resistite textured knockdown finish (smooth or regular per customer)White"||
                obj.ToString()== "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)"||
                obj.ToString() == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)")
            {
                resistiteQty();
            }
            #region LipColorVistaAj44 checkbox logic
            if (obj.ToString() == "Lip Color")
            {
                //var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();

                foreach (SystemMaterial mat in SystemMaterials)
                {
                    if (mat.Name == "Lip Color")
                    {
                        mat.IsMaterialEnabled = false;

                    }

                    if (mat.Name == "Vista Paint Acripoxy" || mat.Name == "Aj-44A Dressing(Sealer)")
                    {
                        mat.IsMaterialChecked = false;
                        mat.IsMaterialEnabled = true;
                    }

                    if (mat.Name == "Resistite textured knockdown finish (smooth or regular per customer)Gray")
                    {
                        SystemMaterial matWhite;
                        if (dbData == null)
                        {
                            matWhite = getSMObject(7, "Resistite textured knockdown finish (smooth or regular per customer)White", "55 LB BAG");
                        }
                        else
                            matWhite = createSMObjectDB("Resistite textured knockdown finish (smooth or regular per customer)White", "55 LB BAG");
                        
                        
                        mat.MaterialPrice = matWhite.MaterialPrice;
                        mat.Name = matWhite.Name;
                        mat.Weight = matWhite.Weight;
                        
                        mat.HorizontalProductionRate = matWhite.HorizontalProductionRate*(1+prPerc);
                        
                        mat.StairsProductionRate = matWhite.StairsProductionRate * (1 + prPerc);
                        
                        mat.SetupMinCharge = mat.SetupMinCharge;
                        RaisePropertyChanged("SetupMinCharge");
                        mat.Hours = matWhite.Hours;
                        mat.LaborExtension = matWhite.LaborExtension;
                        mat.LaborUnitPrice = matWhite.LaborUnitPrice;

                    }
                    if (mat.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)")
                    {
                        SystemMaterial matWhite;
                        if (dbData==null)
                        {
                            matWhite = getSMObject(9, "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)", "40 LB BAG");
                        }
                        else
                            matWhite = createSMObjectDB("CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)", "40 LB BAG");

                        mat.MaterialPrice = matWhite.MaterialPrice;
                        mat.Name = matWhite.Name;
                        mat.Weight = matWhite.Weight;

                        mat.HorizontalProductionRate = matWhite.HorizontalProductionRate * (1 + prPerc);

                        mat.StairsProductionRate = matWhite.StairsProductionRate * (1 + prPerc);

                        mat.SetupMinCharge = mat.SetupMinCharge;
                        RaisePropertyChanged("SetupMinCharge");
                        mat.Hours = matWhite.Hours;
                        mat.LaborExtension = matWhite.LaborExtension;
                        mat.LaborUnitPrice = matWhite.LaborUnitPrice;
                    }
                    

                }
            }
            if (obj.ToString() == "Vista Paint Acripoxy")
            {
                
                foreach (SystemMaterial mat in SystemMaterials)
                {
                    if (mat.Name == "Vista Paint Acripoxy")
                    {
                        mat.IsMaterialEnabled = false;

                    }
                    if (mat.Name == "Lip Color" || mat.Name == "Aj-44A Dressing(Sealer)")
                    {
                        mat.IsMaterialChecked = false;
                        mat.IsMaterialEnabled = true;
                    }

                    if (mat.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)")
                    {
                        SystemMaterial matWhite;
                        if (dbData==null)
                        {
                            matWhite = getSMObject(8, "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)", "40 LB BAG");
                        }
                        else
                            matWhite = createSMObjectDB( "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)", "40 LB BAG");


                        mat.MaterialPrice = matWhite.MaterialPrice;
                        mat.Name = matWhite.Name;
                        mat.Weight = matWhite.Weight;

                        mat.HorizontalProductionRate = matWhite.HorizontalProductionRate * (1 + prPerc);

                        mat.StairsProductionRate = matWhite.StairsProductionRate * (1 + prPerc);

                        mat.SetupMinCharge = mat.SetupMinCharge;
                        RaisePropertyChanged("SetupMinCharge");
                        mat.Hours = matWhite.Hours;
                        mat.LaborExtension = matWhite.LaborExtension;
                        mat.LaborUnitPrice = matWhite.LaborUnitPrice;
                    }
                    if (mat.Name == "Resistite textured knockdown finish (smooth or regular per customer)White")
                    {
                        SystemMaterial matWhite;
                        if (dbData==null)
                        {
                            matWhite = getSMObject(6, "Resistite textured knockdown finish (smooth or regular per customer)Gray", "55 LB BAG");
                        }
                        else
                            matWhite = createSMObjectDB( "Resistite textured knockdown finish (smooth or regular per customer)Gray", "55 LB BAG");


                        mat.MaterialPrice = matWhite.MaterialPrice;

                        mat.Weight = matWhite.Weight;
                        mat.Name = matWhite.Name;
                        mat.HorizontalProductionRate = matWhite.HorizontalProductionRate * (1 + prPerc);

                        mat.StairsProductionRate = matWhite.StairsProductionRate * (1 + prPerc);

                        mat.SetupMinCharge = mat.SetupMinCharge;
                        RaisePropertyChanged("SetupMinCharge");
                        mat.Hours = matWhite.Hours;
                        mat.LaborExtension = matWhite.LaborExtension;
                        mat.LaborUnitPrice = matWhite.LaborUnitPrice;
                    }
                    
                }
            }
            if (obj.ToString() == "Aj-44A Dressing(Sealer)")
            {

                
                foreach (SystemMaterial mat in SystemMaterials)
                {
                    if (mat.Name == "Aj-44A Dressing(Sealer)")
                    {
                        mat.IsMaterialEnabled = false;

                    }

                    if (mat.Name == "Lip Color" || mat.Name == "Vista Paint Acripoxy")
                    {
                        mat.IsMaterialChecked = false;
                        mat.IsMaterialEnabled = true;
                    }

                    if (mat.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)")
                    {
                        SystemMaterial matWhite;
                        if (dbData==null)
                        {
                            matWhite = getSMObject(8, "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)", "40 LB BAG");
                        }
                        else
                            matWhite = createSMObjectDB( "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)", "40 LB BAG");

                        mat.MaterialPrice = matWhite.MaterialPrice;

                        mat.Weight = matWhite.Weight;
                        mat.Name = matWhite.Name;
                        mat.HorizontalProductionRate = matWhite.HorizontalProductionRate * (1 + prPerc);

                        mat.StairsProductionRate = matWhite.StairsProductionRate * (1 + prPerc);

                        mat.SetupMinCharge = mat.SetupMinCharge;
                        RaisePropertyChanged("SetupMinCharge");
                        mat.Hours = matWhite.Hours;
                        mat.LaborExtension = matWhite.LaborExtension;
                        mat.LaborUnitPrice = matWhite.LaborUnitPrice;
                    }
                    if (mat.Name == "Resistite textured knockdown finish (smooth or regular per customer)White")
                    {
                        SystemMaterial matWhite;
                        if (dbData==null)
                        {
                             matWhite = getSMObject(6, "Resistite textured knockdown finish (smooth or regular per customer)Gray", "55 LB BAG");
                        }
                        else
                            matWhite = createSMObjectDB("Resistite textured knockdown finish (smooth or regular per customer)Gray", "55 LB BAG");


                        mat.MaterialPrice = matWhite.MaterialPrice;
                        mat.Name = matWhite.Name;
                        mat.Weight = matWhite.Weight;

                        mat.HorizontalProductionRate = matWhite.HorizontalProductionRate * (1 + prPerc);

                        mat.StairsProductionRate = matWhite.StairsProductionRate * (1 + prPerc);

                        mat.SetupMinCharge = mat.SetupMinCharge;
                        RaisePropertyChanged("SetupMinCharge");
                        mat.Hours = matWhite.Hours;
                        mat.LaborExtension = matWhite.LaborExtension;
                        mat.LaborUnitPrice = matWhite.LaborUnitPrice;
                    }


                }
            }
            #endregion

            if (obj.ToString() == "Barrier Guard membrane over smooth surface")
            {
                SystemMaterials.Where(x => x.Name == "Glass mat #4 1200 SF ROLL FROM ACME").First().IsMaterialChecked=
                    SystemMaterials.Where(x => x.Name == "Barrier Guard membrane over smooth surface").First().IsMaterialChecked;

            }
            //CalculateLaborMinCharge(false);
        }


        //for lipcolor vista,AJ44
        public override void setCheckBoxes()
        {
            //base.setCheckBoxes();
            
            foreach (SystemMaterial mat in SystemMaterials)
            {
                if (mat.Name == "Lip Color")
                {
                    mat.IsMaterialChecked = false;
                }
                if (mat.Name == "Aj-44A Dressing(Sealer)")
                {                    
                    mat.IsMaterialChecked = true;
                }
                if (mat.Name == "Vista Paint Acripoxy")
                {                   
                    mat.IsMaterialChecked = false;
                }
                if (mat.Name == "Sand or pressure wash to prepare area")
                {
                    mat.IsMaterialChecked =true;
                }
                //if (mat.Name == "Weather Seal XL Coat")
                //{
                //    mat.IsMaterialChecked = false;
                //    mat.IsMaterialEnabled = true;
                //}
            }

        }

        public override void setExceptionValues(object s)
        {
            base.setExceptionValues(s);
            if (SystemMaterials.Count != 0)
            {
                
                SystemMaterial item = SystemMaterials.Where(x => x.Name == "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqft = item.Qty;
                    item.SMSqftH = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqft,item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = (item.Hours + item.SetupMinCharge) * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;

                }
                //CalculateLaborMinCharge(false);
            }
            
        }

    }
}
