using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class WWResealMaterialViewModel : MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        public WWResealMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            //laborRate = 19.36;
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
                    return setupMin > calhrs ? setupMin * laborRate : calhrs * laborRate; ;
            }
            
        }
        public override void populateCalculation()
        {
            //base.populateCalculation();
            LCostBreakUp = new ObservableCollection<CostBreakup>();
            double facValue = 0;
            double totalJobCostM = 0;
            double totalJobCostS = 0;
            double totalJobCostSy = 0;

            double.TryParse(laborDetails[0][0].ToString(), out facValue);
            double fac1 = isPrevailingWage ? facValue : 0;
            double.TryParse(laborDetails[1][0].ToString(), out facValue);
            double fac2 = isDiscounted ? facValue : 0;
            double calbackfactor = 1 + fac1 + fac2;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Total Labor",
                CalFactor = 0,
                MetalCost = 0,
                SlopeCost = 0,
                SystemCost = (TotalLaborExtension / calbackfactor),
                HideCalFactor = System.Windows.Visibility.Hidden
            });

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Prevailing Wage (Including Marina's Salary)",
                CalFactor = fac1,
                MetalCost =0,
                SlopeCost =  0,
                SystemCost = (TotalLaborExtension / calbackfactor) * fac1
            });



            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Deduct on Labor for large jobs",
                CalFactor = fac2,
                MetalCost =  0,
                SlopeCost =  0,
                SystemCost = (TotalLaborExtension / calbackfactor) * fac2
            });
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Total Labor including prevailing wage",
                CalFactor = 0,
                MetalCost =  0,
                SlopeCost = 0,
                SystemCost = TotalLaborExtension,
                HideCalFactor = System.Windows.Visibility.Hidden
            });

            double.TryParse(laborDetails[2][0].ToString(), out facValue);
            double actVal = isPrevailingWage ? 0 : facValue;
            double metalLabor = 0;
            double slopeLabor = 0;
            double systemLabor = (TotalLaborExtension / calbackfactor + (TotalLaborExtension / calbackfactor * fac2));
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Workers Comp TL > $24.00",
                CalFactor = facValue,
                MetalCost = actVal * metalLabor,
                SlopeCost = actVal * slopeLabor,
                SystemCost = actVal * systemLabor
            });

            double.TryParse(laborDetails[3][0].ToString(), out facValue);
            actVal = isPrevailingWage ? 0 : facValue;
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Workers Comp All < $23.99",
                CalFactor = facValue,
                MetalCost = actVal * metalLabor,
                SlopeCost = actVal * slopeLabor,
                SystemCost = actVal * systemLabor
            });

            double.TryParse(laborDetails[4][0].ToString(), out facValue);

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Workers Comp Prevailing Wage",
                CalFactor = facValue,
                MetalCost = 0,
                SlopeCost =  0,
                SystemCost = isPrevailingWage ? facValue * TotalLaborExtension : 0
            });
            double.TryParse(laborDetails[5][0].ToString(), out facValue);

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Payroll Expense (SS, ET, Uemp, Dis, Medicare)",
                CalFactor = facValue,
                MetalCost = facValue * metalLabor,
                SlopeCost = facValue * slopeLabor,
                SystemCost = facValue * systemLabor
            });
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Materials",
                CalFactor = 0,
                MetalCost = 0,
                SlopeCost = 0,
                SystemCost = TotalMaterialCostbrkp,
                HideCalFactor = System.Windows.Visibility.Hidden

            });
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Freight",
                CalFactor = 0,
                MetalCost = 0,
                SlopeCost = 0,
                SystemCost = TotalFreightCostBrkp,
                HideCalFactor = System.Windows.Visibility.Hidden
            });
            double.TryParse(laborDetails[6][0].ToString(), out facValue);

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Tax",
                CalFactor = facValue,
                MetalCost = 0,
                SlopeCost = 0,
                SystemCost = facValue * (TotalMaterialCostbrkp + TotalFreightCostBrkp)
            });

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "SubContract Labor",
                CalFactor = 0,
                MetalCost = 0,
                SlopeCost = 0,
                SystemCost = 0,
                SubContractLaborCost = TotalSubContractLaborCostBrkp,
                HideCalFactor = System.Windows.Visibility.Hidden
            });
            for (int i = 3; i < LCostBreakUp.Count; i++)
            {
                totalJobCostM = totalJobCostM + LCostBreakUp[i].MetalCost;
                totalJobCostS = totalJobCostS + LCostBreakUp[i].SlopeCost;
                totalJobCostSy = totalJobCostSy + LCostBreakUp[i].SystemCost;

            }
            //totalJobCostM = LCostBreakUp.Select(x => x.MetalCost).Sum()- MetalTotals.LaborExtTotal;
            //totalJobCostS = LCostBreakUp.Select(x => x.SlopeCost).Sum()+SlopeTotals.LaborExtTotal;
            //totalJobCostSy = LCostBreakUp.Select(x => x.SystemCost).Sum()+ TotalLaborExtension;


            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Total Job Cost",
                CalFactor = 0,
                MetalCost = totalJobCostM,
                SlopeCost = totalJobCostS,
                SystemCost = totalJobCostSy,
                HideCalFactor = System.Windows.Visibility.Hidden
            });

            double.TryParse(laborDetails[17][0].ToString(), out facValue);
            double profitMetal = facValue;
            double.TryParse(laborDetails[19][0].ToString(), out facValue);
            double profitSlope = facValue;
            double.TryParse(laborDetails[18][0].ToString(), out facValue);
            double profitMaterial = facValue;


            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Individual Profit Margin",
                CalFactor = 0,
                MetalCost = profitMetal,
                SlopeCost = profitSlope,
                SystemCost = profitMaterial,
                HideCalFactor = System.Windows.Visibility.Hidden
            });

            double psy1 = SubContractMarkup * (TotalSubContractLaborCostBrkp);
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Profit Margin on subcontract labor",
                CalFactor = SubContractMarkup,
                MetalCost = 0,
                SlopeCost = 0,
                SubContractLaborCost = psy1,
                SystemCost = 0
            });


            double.TryParse(laborDetails[8][0].ToString(), out facValue);
            double pm2, ps2, psy2;
            pm2 = totalJobCostM * facValue * (1 + facValue);
            ps2 = totalJobCostS * facValue * (1 + facValue);
            psy2 = totalJobCostSy * facValue * (1 + facValue);
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Profit Margin add",
                CalFactor = facValue,
                MetalCost = pm2,
                SlopeCost = ps2,
                SystemCost = psy2
            });

            double.TryParse(laborDetails[9][0].ToString(), out facValue);

            double pm3 =  0;
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Profit deduct for special metal",
                CalFactor = facValue,
                MetalCost = pm3,
                SlopeCost = 0,
                SystemCost = 0
            });
            //double pm;
            //double.TryParse(laborDetails[10][0].ToString(), out pm);

            double totalCostM = 0;
            double totalCostS = 0;
            double totalCostSy = (totalJobCostSy) / profitMaterial + (psy2); //psy4+ psy6
            double totalCostSbLabor = TotalSubContractLaborCostBrkp + psy1;
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Total Cost",
                CalFactor = 0,
                MetalCost = totalCostM,
                SlopeCost = totalCostS,
                SystemCost = totalCostSy,
                SubContractLaborCost = totalCostSbLabor,
                HideCalFactor = System.Windows.Visibility.Hidden
            });


            double.TryParse(laborDetails[11][0].ToString(), out facValue);

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "General Liability",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue / profitMetal,
                SlopeCost = totalCostS * facValue / profitSlope,
                SystemCost = totalCostSy * facValue / profitMaterial,
                SubContractLaborCost = (totalCostSbLabor * facValue / SubContractMarkup)

            });
            double finalMCost = totalCostM + (totalCostM * facValue / profitMetal);
            double finalSCost = totalCostS + (totalCostS * facValue / profitSlope);
            double finalSyCost = totalCostSy + (totalCostSy * facValue / profitMaterial);
            double finalSubLabCost = totalCostSbLabor + (totalCostSbLabor * facValue / SubContractMarkup);
            double.TryParse(laborDetails[12][0].ToString(), out facValue);

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Direct Expense (Gas, Small Tools, Etc,)",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue / profitMetal,
                SlopeCost = totalCostS * facValue / profitSlope,
                SystemCost = totalCostSy * facValue / profitMaterial,
                SubContractLaborCost = (totalCostSbLabor * facValue / SubContractMarkup)

            });
            finalMCost = finalMCost + (totalCostM * facValue / profitMetal);
            finalSCost = finalSCost + (totalCostS * facValue / profitSlope);
            finalSyCost = finalSyCost + (totalCostSy * facValue / profitMaterial);
            finalSubLabCost = finalSubLabCost + (totalCostSbLabor * facValue / SubContractMarkup);
            if (!hasContingencyDisc)
                double.TryParse(laborDetails[13][0].ToString(), out facValue);

            else
                facValue = 0;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Contingency",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue / profitMetal,
                SlopeCost = totalCostS * facValue / profitSlope,
                SystemCost = totalCostSy * facValue / profitMaterial,
                SubContractLaborCost = totalCostSbLabor * facValue / SubContractMarkup
            });
            finalMCost = finalMCost + (totalCostM * facValue / profitMetal);
            finalSCost = finalSCost + (totalCostS * facValue / profitSlope);
            finalSyCost = finalSyCost + (totalCostSy * facValue / profitMaterial);
            finalSubLabCost = finalSubLabCost + (totalCostSbLabor * facValue / SubContractMarkup);
            double.TryParse(laborDetails[14][0].ToString(), out facValue);

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Insurance Increase Fund (5 % on total sale)",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue,
                SlopeCost = totalCostS * facValue,
                SystemCost = totalCostSy * facValue

            });
            finalMCost = finalMCost + (totalCostM * facValue);
            finalSCost = finalSCost + (totalCostS * facValue);
            finalSyCost = finalSyCost + (totalCostSy * facValue);
            double.TryParse(laborDetails[15][0].ToString(), out facValue);

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Fuel/Sur Chg (1-PRICE OF GAS) /-40",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue,
                SlopeCost = totalCostS * facValue,
                SystemCost = totalCostSy * facValue
            });
            finalMCost = finalMCost + (totalCostM * facValue);
            finalSCost = finalSCost + (totalCostS * facValue);
            finalSyCost = finalSyCost + (totalCostSy * facValue);
            finalSubLabCost = finalSubLabCost + (totalCostSbLabor * facValue);


            double.TryParse(laborDetails[16][0].ToString(), out facValue);
            if (MarkUpPerc != 0)
            {
                facValue = MarkUpPerc / 100;
            }
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Add mark up to total job price",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue,
                SlopeCost = totalCostS * facValue,
                SystemCost = totalCostSy * facValue,
                SubContractLaborCost = totalCostSbLabor * facValue
            });
            finalMCost = finalMCost + (totalCostM * facValue);
            finalSCost = finalSCost + (totalCostS * facValue);
            finalSyCost = finalSyCost + (totalCostSy * facValue);
            finalSubLabCost = finalSubLabCost + (totalCostSbLabor * facValue);
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Profit Margin",
                CalFactor = 0,
                MetalCost = totalCostM - totalJobCostM,
                SlopeCost = totalCostS - totalJobCostS,
                SystemCost = totalCostSy - totalJobCostSy,
                HideCalFactor = System.Windows.Visibility.Hidden
            });

            TotalMetalPrice = finalMCost; //*(1+markUpPerc/100);
            TotalSlopingPrice = finalSCost; //* (1 + markUpPerc / 100);
            TotalSystemPrice = finalSyCost; //* (1 + markUpPerc / 100);
            TotalSubcontractLabor = finalSubLabCost; // * (1 + markUpPerc / 100);
            TotalSale = TotalMetalPrice + TotalSlopingPrice + TotalSystemPrice + TotalSubcontractLabor;
            OnPropertyChanged("TotalMetalPrice");
            OnPropertyChanged("TotalSlopingPrice");
            OnPropertyChanged("TotalSystemPrice");
            OnPropertyChanged("TotalSubcontractLabor");
            OnPropertyChanged("TotalSale");
        
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
                    if (SystemMaterials[i].Name == "LARGE CRACK REPAIR" || SystemMaterials[i].Name == "BUBBLE REPAIR (MEASURE SQ FT)")
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
            CalculateLaborMinCharge();
            CalculateAllMaterial();
        }
        public override void CalculateLaborMinCharge()
        {
            LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
                                        x.IsMaterialChecked).ToList().Select(x => x.Hours).Sum();

            LaborMinChargeLaborExtension = LaborMinChargeMinSetup + LaborMinChargeHrs > 17 ? 0 :
                                                (17 - LaborMinChargeMinSetup + LaborMinChargeHrs) * laborRate;
            base.CalculateLaborMinCharge();
        }
        public override void calculateRLqty()
        {
            //base.calculateRLqty();
            double val1, val2, val3, val4 = 0;
            double qty = 0;
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "RESISTITE REGULAR GRAY").FirstOrDefault();

            val1 =SystemMaterials.Where(x => x.Name == "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)").FirstOrDefault().Qty;
            val2=SystemMaterials.Where(x => x.Name == "SLURRY COAT (RESISTITE) OVER TEXTURE").FirstOrDefault().Qty;
            val3=sysmat.Qty;
            val4=SystemMaterials.Where(x => x.Name == "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)").FirstOrDefault().Qty;
            qty= sysmat.IsMaterialChecked ? (val3 + val4 + val1) * 0.33 + val2 / 5 : val1 * 0.33 + val2 / 5;
            SystemMaterial RL = SystemMaterials.Where(x => x.Name == "RESISTITE LIQUID").FirstOrDefault();
            if (RL!=null)
            {
                RL.Qty = qty;
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
                    return totalSqft + riserCount * 4 * 2;
                case "LIGHT CRACK REPAIR":
                    return totalSqft;
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

            TotalHrsDriveLabor = totalSqft < 1001 ? 2 : Math.Ceiling(totalSqft / 1000 * 10);
            TotalHrsFreightLabor = Math.Round(AllTabsFreightTotal / laborRate, 1);
            OnPropertyChanged("TotalHrsFreightLabor");
            TotalHrsSystemLabor = Math.Round(isPrevailingWage ? (TotalLaborExtension / laborRate) * .445 - TotalHrsDriveLabor :
                                                  (TotalLaborExtension / laborRate) - TotalHrsDriveLabor, 1);

            OnPropertyChanged("TotalHrsSystemLabor");
            if (SlopeTotals != null && MetalTotals != null)
            {
                TotalHrsMetalLabor = isPrevailingWage ? (MetalTotals.LaborExtTotal / laborRate) * .445 :
                                                  (MetalTotals.LaborExtTotal / laborRate);
                OnPropertyChanged("TotalHrsMetalLabor");
                TotalHrsSlopeLabor = isPrevailingWage ? (SlopeTotals.LaborExtTotal / laborRate) * .445 :
                                                      (SlopeTotals.LaborExtTotal / laborRate);
                OnPropertyChanged("TotalHrsSlopeLabor");
            }

            TotalHrsLabor = TotalHrsSystemLabor + TotalHrsMetalLabor + TotalHrsSlopeLabor +
                TotalHrsFreightLabor + TotalHrsDriveLabor;
            OnPropertyChanged("TotalHrsLabor");
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
            
            switch (materialName)
            {
                case "VISTA PAINT ACRAPOXY SEALER":
                case "RESISTITE REGULAR GRAY":
                case "DEXOTEX AJ-44":
                case "WESTCOAT SC-10":
                case "UPI PERMASHIELD":
                case "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL":
                case "OPTIONAL FOR WEATHER SEAL XL":
                    return lfArea / coverage < 1 ? 2 / 5 : lfArea / coverage;
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

                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialChecked = false;
                    return;
                }
            }
            if (obj.ToString()== "DEXOTEX AJ-44")
            {
                mat = SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First();
                if (mat?.IsMaterialChecked == true)
                {

                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialChecked = false;
                    return;
                }
            }

            if (obj.ToString()== "WESTCOAT SC-10")
            {
                mat = SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First();
                if (mat?.IsMaterialChecked == true)
                {

                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialChecked = false;
                    return;
                }
            }

            if (obj.ToString()== "UPI PERMASHIELD")
            {
                mat = SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First();
                if (mat?.IsMaterialChecked == true)
                {

                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First().IsMaterialChecked = false;
                    return;
                }
            }
            if (obj.ToString()== "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL")
            {
                mat = SystemMaterials.Where(x => x.Name == "PLI DEK GS88 WITH COLOR JAR 1 PER PAIL").First();
                if (mat?.IsMaterialChecked == true)
                {

                    SystemMaterials.Where(x => x.Name == "VISTA PAINT ACRAPOXY SEALER").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "DEXOTEX AJ-44").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "WESTCOAT SC-10").First().IsMaterialChecked = false;
                    SystemMaterials.Where(x => x.Name == "UPI PERMASHIELD").First().IsMaterialChecked = false;
                    return;
                }
            }
            if (obj.ToString()== "SLURRY COAT (RESISTITE) OVER TEXTURE"|| obj.ToString()== "RESISTITE REGULAR GRAY")
            {
                calculateRLqty();
                
            }
            CalculateLaborMinCharge();
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
        public override void setExceptionValues()
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
