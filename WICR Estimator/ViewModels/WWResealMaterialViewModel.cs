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
    public class WWResealMaterialViewModel : MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        public WWResealMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);
        }

        private void FillMaterialList()
        {
            
            materialNames.Add("SLURRY COAT (RESISTITE) OVER TEXTURE", "SQ FT");

            materialNames.Add("LIGHT CRACK REPAIR", "SQ FT");
            materialNames.Add("LARGE CRACK REPAIR", "LF");
            materialNames.Add("BUBBLE REPAIR (MEASURE SQ FT)", "SQ FT");
            materialNames.Add("RESISTITE LIQUID", "5 GAL PAIL");
            materialNames.Add("RESISTITE REGULAR GRAY", "55 LB BAG");
            materialNames.Add("RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)", "40 LB BAG");
            materialNames.Add("CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)", "40 LB BAG");
            materialNames.Add("RESISTITE UNIVERSAL PRIMER (ADD 50% WATER)", "5 GAL PAIL");
            materialNames.Add("VISTA PAINT ACRAPOXY SEALER", "5 GAL PAIL");
            materialNames.Add("DEXOTEX AJ-44", "5 GAL PAIL");
            materialNames.Add("WESTCOAT SC-10", "5 GAL PAIL");
            materialNames.Add("UPI PERMASHIELD", "5 GAL PAIL");
            materialNames.Add("PLI DEK GS88 WITH COLOR JAR 1 PER PAIL", "5 GAL PAIL");
            materialNames.Add("OPTIONAL FOR WEATHER SEAL XL", "5 GAL PAIL");
        }

        
        public override double CalculateLabrExtn(double calhrs, double setupMin,string matName)
        {
            //return base.CalculateLabrExtn(calhrs, setupMin);
            switch (matName)
            {   
                case "RESISTITE LIQUID":
                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)":
                    return (calhrs+setupMin)*laborRate;
                default:
                    return calhrs==0 ? 0:setupMin > calhrs ? setupMin * laborRate : calhrs * laborRate; ;
            }
            
        }
       
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            //base.FetchMaterialValuesAsync(hasSetupChanged);
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "LARGE CRACK REPAIR" || item.Name == "BUBBLE REPAIR (MEASURE SQ FT)")
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
                try
                {
                    for (int i = 0; i < SystemMaterials.Count; i++)
                    {

                        double sp = SystemMaterials[i].SpecialMaterialPricing;
                        bool iscbChecked = SystemMaterials[i].IsMaterialChecked;
                        bool iscbEnabled = SystemMaterials[i].IsMaterialEnabled;

                        //SystemMaterials[i] = sysMat[i];

                        //SystemMaterials[i].SpecialMaterialPricing = sp;
                        UpdateMe(sysMat[i]);

                        SystemMaterials[i].UpdateSpecialPricing(sp);

                        //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                        //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                        SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);

                        if (SystemMaterials[i].Name == "LARGE CRACK REPAIR" || SystemMaterials[i].Name == "BUBBLE REPAIR (MEASURE SQ FT)")
                        {
                            if (qtyList.ContainsKey(SystemMaterials[i].Name))
                            {
                                //SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];
                                SystemMaterials[i].UpdateQuantity(qtyList[SystemMaterials[i].Name]);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                

            }
            

            else
            {
                SystemMaterials = sysMat;
                setCheckBoxes();
            }
            #endregion

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
            CalculateCost(null);
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
        }
        
        public override void calculateRLqty()
        {
            //base.calculateRLqty();
            double val1=0, val2=0, val3=0, val4 = 0;
            double qty = 0;
            SystemMaterial sysmat1 = SystemMaterials.Where(x => x.Name == "RESISTITE REGULAR GRAY").FirstOrDefault();
            if (sysmat1!=null)
            {
                val3 = sysmat1.IsMaterialChecked?sysmat1.Qty:0;
            }
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)").FirstOrDefault();
            if (sysmat!=null)
            {
                val1 =sysmat.IsMaterialChecked?sysmat.Qty:0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "SLURRY COAT (RESISTITE) OVER TEXTURE").FirstOrDefault();
            if (sysmat != null)
            {
                val2 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }
            sysmat = SystemMaterials.Where(x => x.Name == "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)").FirstOrDefault();
            if (sysmat != null)
            {
                val4 = sysmat.IsMaterialChecked ? sysmat.Qty : 0;
            }

            
            qty= sysmat1.IsMaterialChecked ? (val3 + val4 + val1) * 0.33 + val2 / 5 : val1 * 0.33 + val2 / 5;
            bool ischecked = SystemMaterials.Where(x => x.Name == "RESISTITE LIQUID").FirstOrDefault().IsMaterialChecked;
            SystemMaterial RL = SystemMaterials.Where(x => x.Name == "RESISTITE LIQUID").FirstOrDefault();
            if (RL!=null)
            {
                RL.Qty = qty;
                //RL.IsMaterialChecked = ischecked;
            }
            //bool ischecked = SystemMaterials.Where(x => x.Name == "RESISTITE REGULAR GRAY").FirstOrDefault().IsMaterialChecked;
            //SystemMaterials.Where(x => x.Name == "RESISTITE LIQUID").FirstOrDefault().IsMaterialChecked = ischecked;


        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            //return base.getCheckboxCheckStatus(materialName);
            
            switch (materialName)
            {
                case "SLURRY COAT (RESISTITE) OVER TEXTURE":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                case "LIGHT CRACK REPAIR":
                case "VISTA PAINT ACRAPOXY SEALER":
                case "RESISTITE REGULAR GRAY":
                case "DEXOTEX AJ-44":
                case "WESTCOAT SC-10":
                case "UPI PERMASHIELD":
                case "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL":
                case "OPTIONAL FOR WEATHER SEAL XL":
                    return true;
                default:
                    return false;
            }
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            switch (materialName)
            {
                case "SLURRY COAT (RESISTITE) OVER TEXTURE":
                case "LARGE CRACK REPAIR":
                case "BUBBLE REPAIR (MEASURE SQ FT)":
                case "RESISTITE LIQUID":
                case "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)":
                case "RESISTITE REGULAR GRAY":
                    return false;
                default:
                    return true;
            }
        }
        public override double getlfArea(string materialName)
        {
            //return base.getlfArea(materialName);
            
            switch (materialName)
            {
                case "SLURRY COAT (RESISTITE) OVER TEXTURE":
                    return totalSqft + riserCount * stairWidth * 2; //changed again : Small issue with Reseal sheet
                //case "LIGHT CRACK REPAIR":
                    //return totalSqft;
                case "LARGE CRACK REPAIR":
                case "BUBBLE REPAIR (MEASURE SQ FT)":
                case "RESISTITE LIQUID":
                    return 0;
                default:
                    return totalSqft+riserCount*stairWidth*2;
            }

        }
        public override double getSqFtAreaH(string materialName)
        {

            switch (materialName)
            {
                case "RESISTITE LIQUID":
                    return 0.0000001;
                default:
                    return totalSqft;
            }
        }

        public override void calculateLaborHrs()
        {
            base.calculateLaborHrs();
            //TotalHrsDriveLabor = totalSqft < 1001 ? 2 : Math.Ceiling(totalSqft / 1000 * 10);
            //DriveLaborValue = Math.Round(TotalHrsDriveLabor * laborRate, 2);
            //OnPropertyChanged("DriveLaborValue");
            ////TotalHrsFreightLabor = Math.Round(AllTabsFreightTotal / laborRate, 1);
            //OnPropertyChanged("TotalHrsFreightLabor");
            //TotalHrsSystemLabor = Math.Round(isPrevailingWage ? (TotalLaborExtension / actualPreWage)  :
            //                                      (TotalLaborExtension / laborRate), 1);

            //OnPropertyChanged("TotalHrsSystemLabor");
            //if (SlopeTotals != null && MetalTotals != null)
            //{
            //    TotalHrsMetalLabor = isPrevailingWage ? (MetalTotals.LaborExtTotal / actualPreWage) :
            //                                      (MetalTotals.LaborExtTotal / laborRate);
            //    OnPropertyChanged("TotalHrsMetalLabor");
            //    TotalHrsSlopeLabor = isPrevailingWage ? (SlopeTotals.LaborExtTotal / actualPreWage) :
            //                                          (SlopeTotals.LaborExtTotal / laborRate);
            //    OnPropertyChanged("TotalHrsSlopeLabor");
            //}

            //TotalHrsLabor = TotalHrsSystemLabor + TotalHrsMetalLabor + TotalHrsSlopeLabor + TotalHrsDriveLabor;
            //OnPropertyChanged("TotalHrsLabor");
        }
        public override double getSqFtStairs(string materialName)
        {
            //return base.getSqFtStairs(materialName);
            switch (materialName)
            {
                case "RESISTITE LIQUID":
                case "LIGHT CRACK REPAIR":
                case "LARGE CRACK REPAIR":
                case "BUBBLE REPAIR (MEASURE SQ FT)":
                    return 0;
                default:
                    return riserCount*stairWidth*2;

            }
        }
        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            double minVal = 0.4;
            switch (materialName)
            {
                case "VISTA PAINT ACRAPOXY SEALER":
                case "DEXOTEX AJ-44":
                case "WESTCOAT SC-10":
                case "UPI PERMASHIELD":
                case "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL":
                case "OPTIONAL FOR WEATHER SEAL XL":
                    return lfArea / coverage < 0.4 ? minVal : lfArea / coverage;//changed from 1 to 0.4
                default:
                    return lfArea/coverage;
            }
        }

        
        public override bool canApply(object obj)
        {
            //return base.canApply(obj);
            
                return true;
            
        }
        public override void ApplyCheckUnchecks(object obj)
        {
            lastCheckedMat = obj.ToString();
            //base.ApplyCheckUnchecks(obj);
            if (obj.ToString()== "RESISTITE REGULAR GRAY")
            {
                bool ischecked= SystemMaterials.Where(x => x.Name == "RESISTITE REGULAR GRAY").FirstOrDefault().IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "RESISTITE LIQUID").FirstOrDefault().IsMaterialChecked = ischecked;
                SystemMaterials.Where(x => x.Name == "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)").FirstOrDefault().IsMaterialChecked = ischecked;

            }
            SystemMaterial mat;
            if (obj.ToString()== "VISTA PAINT ACRAPOXY SEALER")
            {
                mat = SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First();
                if (mat?.IsMaterialChecked == true)
                {
                    mat.IsMaterialEnabled = false;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialEnabled = true;
                    return;
                }
            }
            if (obj.ToString()== "DEXOTEX AJ-44")
            {
                mat = SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First();
                if (mat?.IsMaterialChecked == true)
                {
                    mat.IsMaterialEnabled = false;
                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialEnabled = true;
                    return;
                }
            }

            if (obj.ToString()== "WESTCOAT SC-10")
            {
                mat = SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First();
                if (mat?.IsMaterialChecked == true)
                {
                    mat.IsMaterialEnabled = false;
                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialEnabled = true;
                    return;
                }
            }

            if (obj.ToString()== "UPI PERMASHIELD")
            {
                mat = SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First();
                if (mat?.IsMaterialChecked == true)
                {
                    mat.IsMaterialEnabled = false;
                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialEnabled = true;
                    return;
                }
            }
            if (obj.ToString()== "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL")
            {
                mat = SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First();
                if (mat?.IsMaterialChecked == true)
                {
                    mat.IsMaterialEnabled = false;
                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialEnabled = true;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialEnabled = true;
                    return;
                }
            }
            if (obj.ToString()== "SLURRY COAT (RESISTITE) OVER TEXTURE"|| obj.ToString()== "RESISTITE REGULAR GRAY"
                ||obj.ToString()== "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)"
                ||obj.ToString()== "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)")
            {
                calculateRLqty();
                
            }
            //CalculateLaborMinCharge(false);
        }
        public override bool IncludedInLaborMin(string matName)
        {
            return true;
        }
        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0, double sqftStairs = 0, string matName = "")
        {
            //return base.getLaborUnitPrice(laborExtension, riserCount, totalSqft, sqftVert, sqftHor, sqftStairs, matName);
            return laborExtension / (riserCount + totalSqft);
        }
        public override void setExceptionValues(object s)
        {


            //base.setExceptionValues();
            if (SystemMaterials.Count != 0)
            {
                SystemMaterial item = SystemMaterials.Where(x => x.Name == "LARGE CRACK REPAIR").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;

                    //item.IsMaterialChecked = item.Qty>0?true:false;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate; 
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                }

                item = SystemMaterials.Where(x => x.Name == "BUBBLE REPAIR (MEASURE SQ FT)").FirstOrDefault();
                if (item != null)
                {
                    //item.IsMaterialChecked = item.Qty > 0 ? true : false;
                    item.SMSqftH = item.Qty;
                    item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                }
            }

        }
        public override void setCheckBoxes()
        {
            SystemMaterials.Where(x => x.Name == "SLURRY COAT (RESISTITE) OVER TEXTURE").First().IsMaterialChecked = false;
            SystemMaterials.Where(x => x.Name == "RESISTITE LIQUID").First().IsMaterialEnabled = true;
            SystemMaterials.Where(x => x.Name == "RESISTITE LIQUID").First().IsMaterialChecked = false;
            SystemMaterials.Where(x => x.Name == "RESISTITE LIQUID").First().IsMaterialEnabled = false;
            SystemMaterials.Where(x => x.Name == "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)").First().IsMaterialChecked = false;
            SystemMaterials.Where(x => x.Name == "RESISTITE REGULAR GRAY").First().IsMaterialChecked = false;
            SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialChecked = false;
            SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialChecked = false;
            SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialChecked = false;
            SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialChecked = false;
        }
    }
}
