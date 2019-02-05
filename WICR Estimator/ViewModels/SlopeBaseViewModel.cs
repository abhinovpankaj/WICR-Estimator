using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    [XmlInclude(typeof(SlopeViewModel))]
    [XmlInclude(typeof(DexoSlopeViewModel))]
    //[XmlInclude(typeof(PedestrianSlopeViewModel))]
    //[XmlInclude(typeof(EnduroKoteSlopeViewModel))]
    
    public class SlopeBaseViewModel:BaseViewModel
    {
        
        #region Private Properties

        private ObservableCollection<Slope> slopes;
        
        private ICommand calculateCostCommand;
        
        private IList<IList<object>> pWage;
        private double totalmixesman;
        private double averagemixesprice;
        private double laborcost;
        private double totalmaterialCost;
        private double minimumlaborcost;
        private double totallaborcost;
        private double totalweight;
        private double totalfreightcost;
        private double sumtotal;
        private double sumtotalmixes;
        private double sumtotalmatext;
        private double sumtotallaborext;
        
        private string slopeMaterialName;
        private double manualAvgMixPrice;
        #endregion

        public SlopeBaseViewModel()
        {
            Slopes = new ObservableCollection<Slope>();
            SlopeTotals = new Totals { TabName = "Slope" };
            
        }

        #region public properties
        public Totals SlopeTotals;
        public bool isApprovedForCement;
        public bool isPrevailingWage;
        public double laborRate;
        protected IList<IList<object>> perMixRates;
        protected IList<IList<object>> freightData;
        public double prevailingWage;
        public double deductionOnLargeJob;
        public bool overrideManually;
        public bool hasDiscount;

        public string SlopeMaterialName
        {
            get
            {
                return slopeMaterialName;
            }
            set
            {
                if (value!=slopeMaterialName)
                {
                    slopeMaterialName = value;
                    OnPropertyChanged("SlopeMaterialName");
                }
            }
        }
        private System.Windows.Visibility isUrethaneVisible= System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility IsUrethaneVisible
        {
            get { return isUrethaneVisible; }
            set
            {
                isUrethaneVisible = value;
                OnPropertyChanged("IsUrethaneVisible");
            }
        }
        public bool OverrideManually
        {
            get
            {
                return overrideManually;
            }
            set
            {
                if (value != overrideManually)
                {
                    overrideManually = value;
                    if (!overrideManually)
                    {

                        totalmixesman = 0;
                        averagemixesprice = 0;
                        //CalculateGridTotal();
                        //CalculateTotalMixes();
                        OnPropertyChanged("TotalMixesMan");
                        OnPropertyChanged("AverageMixesPrice");
                    }
                    

                    OnPropertyChanged("OverrideManually");
                }
            }
        }
        
        public ObservableCollection<Slope> Slopes
        {
            get
            {
                return slopes;
            }
            set
            {
                if (slopes != value)
                {
                    slopes = value;
                    OnPropertyChanged("Slopes");
                }
            }
        }
        public double TotalMixesMan
        {
            get
            {
                return totalmixesman;
            }
            set
            {
                //if (overrideManually && value != totalmixesman)
                //{
                //    totalmixesman = value;
                //    CalculateManual();
                //    OnPropertyChanged("TotalMixesMan");
                //}
                if(value != totalmixesman)
                {
                    totalmixesman = value;
                    OnPropertyChanged("TotalMixesMan");
                }
            }
        }
        public double AverageMixesPrice
        {
            get
            {
                return averagemixesprice;
            }
            set
            {
                //if (overrideManually && averagemixesprice != value)
                //{
                //    averagemixesprice = value;
                //    //CalculateManual();
                //    OnPropertyChanged("AverageMixesPrice");
                //}
                //else
                //    averagemixesprice = 0;
                if (averagemixesprice != value)
                {
                    averagemixesprice = value;
                    OnPropertyChanged("AverageMixesPrice");
                }

            }
        }

        public double TotalWeight
        {
            get
            {
                return totalweight;
            }
            set
            {
                if (value != totalweight)
                {
                    totalweight = value;
                    OnPropertyChanged("TotalWeight");
                }
            }
        }
        public double TotalFrightCost
        {
            get
            {
                return totalfreightcost;
            }
            set
            {
                if (value != totalfreightcost)
                {
                    totalfreightcost = value;
                    OnPropertyChanged("TotalFrightCost");
                }
            }
        }
        public double LaborCost
        {
            get
            {
                return laborcost;
            }
            set
            {
                if (value != laborcost)
                {
                    laborcost = value;
                    OnPropertyChanged("LaborCost");

                }
            }
        }
        public double MinimumLaborCost
        {
            get
            {
                return minimumlaborcost;
            }
            set
            {
                if (value != minimumlaborcost)
                {
                    minimumlaborcost = value;
                    OnPropertyChanged("MinimumLaborCost");
                }
            }
        }
        public double TotalLaborCost
        {
            get
            {
                return totallaborcost;
            }
            set
            {
                if (value != totallaborcost)
                {
                    totallaborcost = value;
                    OnPropertyChanged("TotalLaborCost");
                }
            }
        }
        public double TotalMaterialCost
        {
            get
            {
                return totalmaterialCost;
            }
            set
            {
                if (value != totalmaterialCost)
                {
                    totalmaterialCost = value;
                    OnPropertyChanged("TotalMaterialCost");
                }
            }
        }

        public double SumTotal
        {
            get
            {
                return sumtotal;
            }
            set
            {
                if (value != sumtotal)
                {
                    sumtotal = value;
                    OnPropertyChanged("SumTotal");
                }
            }
        }
        public double SumTotalMixes
        {
            get
            {
                return sumtotalmixes;
            }
            set
            {
                if (value != sumtotalmixes)
                {
                    sumtotalmixes = value;

                    OnPropertyChanged("SumTotalMixes");
                }
            }
        }
        public double SumTotalMatExt
        {
            get
            {
                return sumtotalmatext;
            }
            set
            {
                if (value != sumtotalmatext)
                {
                    sumtotalmatext = value;
                    OnPropertyChanged("SumTotalMatExt");
                }
            }
        }
        public double SumTotalLaborExt
        {
            get
            {
                return sumtotallaborext;
            }
            set
            {
                if (value != sumtotallaborext)
                {
                    sumtotallaborext = value;
                    OnPropertyChanged("SumTotalLaborExt");
                }
            }
        }
        #endregion

        #region Methods

        public virtual void CalculateManual()
        {
            double minLabVal = 0;
            SumTotalMixes = TotalMixesMan;
            SumTotalMatExt = AverageMixesPrice * TotalMixesMan;
            TotalMaterialCost = SumTotalMatExt;
            TotalWeight = Math.Round(50 * TotalMixesMan, 2);
            TotalFrightCost = Math.Round(FreightCalculator(TotalWeight), 2);
            SumTotalLaborExt = Math.Round(TotalMixesMan * manualAvgMixPrice, 2);
            double.TryParse(perMixRates[8][0].ToString(), out minLabVal);
            MinimumLaborCost = minLabVal * laborRate;
            TotalLaborCost = SumTotalLaborExt==0?0:MinimumLaborCost > SumTotalLaborExt ? MinimumLaborCost : SumTotalLaborExt;

            if (isPrevailingWage)
            {
                if (hasDiscount)
                    TotalLaborCost = TotalLaborCost * (1 + prevailingWage + deductionOnLargeJob);
                else
                    TotalLaborCost = TotalLaborCost * (1 + prevailingWage);

            }
            else
            {
                if (hasDiscount)
                    TotalLaborCost = TotalLaborCost * (1 + deductionOnLargeJob);
                else
                    TotalLaborCost = TotalLaborCost;
            }


        }
        public void GetSlopeDetailsFromGoogle(string projectName)
        {
            if (perMixRates == null)
            {
                //perMixRates = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "P25:Q30");
                perMixRates = DataSerializer.DSInstance.deserializeGoogleData(DataType.Slope, projectName);
                double.TryParse(DataSerializer.DSInstance.deserializeGoogleData(DataType.Rate, projectName)[0][0].ToString(), out laborRate);
            }
            if (pWage == null)
            {
                //pWage = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "E60:E61");
                pWage = DataSerializer.DSInstance.deserializeGoogleData(DataType.Labor, projectName);
                GSData gsData = DataSerializer.DSInstance.deserializeGoogleData(projectName);
                freightData = gsData.FreightData;

            }
            double.TryParse(perMixRates[6][1].ToString(), out manualAvgMixPrice);
            //double.TryParse(pWage[0][0].ToString(), out prevailingWage);
            double.TryParse(pWage[1][0].ToString(), out deductionOnLargeJob);

            

        }
        public void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                if (isApprovedForCement != js.IsApprovedForSandCement)
                {
                    isApprovedForCement = js.IsApprovedForSandCement;
                    SlopeMaterialName = isApprovedForCement ? "Sand and Cement" : js.SlopeMaterialName;
                    reCalculate();
                }

                isPrevailingWage = js.IsPrevalingWage;
                laborRate = js.LaborRate;
                hasDiscount = js.HasDiscount;
                prevailingWage= js.ActualPrevailingWage == 0 ? 0 : (js.ActualPrevailingWage - laborRate) / laborRate;
            }
            CalculateAll();

        }
        public virtual ObservableCollection<Slope> CreateSlopes(int RowN=0)
        {
            ObservableCollection<Slope> slopes = new ObservableCollection<Slope>();
            slopes.Add(new Slope
            {
                Thickness = "1/4 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRate("1/4 inch Average", RowN),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("1/4 inch Average", isApprovedForCement, RowN),
                SlopeType = RowN == 0 ? "" : "URI"
            });
            slopes.Add(new Slope
            {
                Thickness = "1/2 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRate("1/2 inch Average", RowN),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("1/2 inch Average", isApprovedForCement, RowN),
                SlopeType = RowN == 0 ? "" : "URI"
            });

            slopes.Add(new Slope
            {
                Thickness = "3/4 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRate("3/4 inch Average", RowN),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("3/4 inch Average", isApprovedForCement, RowN),
                SlopeType = RowN == 0 ? "" : "URI"
            });
            slopes.Add(new Slope
            {
                Thickness = "1 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRate("1 inch Average", RowN),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("1 inch Average", isApprovedForCement, RowN),
                SlopeType = RowN == 0 ? "" : "URI"
            });
            slopes.Add(new Slope
            {
                Thickness = "1 1/4 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRate("1 1/4 inch Average", RowN),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("1 1/4 inch Average", isApprovedForCement, RowN),
                SlopeType = RowN == 0 ? "" : "URI"
            });

            return slopes;
        }
        public virtual void CalculateAll()
        {
            if (OverrideManually)
            {
                CalculateManual();
            }
            CalculateGridTotal();
            CalculateTotalMixes();

            SlopeTotals.LaborExtTotal = TotalLaborCost;
            SlopeTotals.MaterialExtTotal = TotalMaterialCost;
            SlopeTotals.MaterialFreightTotal = TotalFrightCost;
        }
        public virtual void reCalculate()
        {
            foreach (Slope slp in Slopes)
            {
                slp.PricePerMix = getPricePerMix(slp.Thickness, isApprovedForCement);
            }
            
            CalculateGridTotal();
            CalculateTotalMixes();

            SlopeTotals.LaborExtTotal = TotalLaborCost;
            SlopeTotals.MaterialExtTotal = TotalMaterialCost;
            SlopeTotals.MaterialFreightTotal = TotalFrightCost;
        }

        public virtual void CalculateTotalMixes()
        {
            double minLabVal = 0;
            if (Slopes.Count > 0)
            {
                LaborCost = Math.Round(SumTotalLaborExt, 2);
               double.TryParse(perMixRates[8][0].ToString(), out minLabVal);
                
                MinimumLaborCost = minLabVal * laborRate;

                double lCost = SumTotalLaborExt == 0 ? 0 : LaborCost > MinimumLaborCost ? LaborCost : MinimumLaborCost;
                if (isPrevailingWage)
                {
                    if (hasDiscount)
                        TotalLaborCost = lCost * (1 + prevailingWage + deductionOnLargeJob);
                    else
                        TotalLaborCost = lCost * (1 + prevailingWage);

                }
                else
                {
                    if (hasDiscount)
                        TotalLaborCost = lCost * (1 + deductionOnLargeJob);
                    else
                        TotalLaborCost = lCost;
                }
                TotalMaterialCost = Math.Round(SumTotalMatExt, 2);
                TotalWeight = Math.Round(50 * SumTotalMixes, 2);
                TotalFrightCost = Math.Round(FreightCalculator(TotalWeight), 2);
            }
        }

        public virtual double getGSLaborRate(string thickness,int addRow=0)
        {
            double result;
            switch (thickness)
            {
                case "1/4 inch Average":
                    double.TryParse(perMixRates[1+addRow][0].ToString(), out result);
                    return result;
                case "1/2 inch Average":
                    double.TryParse(perMixRates[2 + addRow][0].ToString(), out result);
                    return result;
                case "3/4 inch Average":
                    double.TryParse(perMixRates[3 + addRow][0].ToString(), out result);
                    return result;
                case "1 1/4 inch Average":
                    double.TryParse(perMixRates[4 + addRow][0].ToString(), out result);
                    return result;
                case "1 inch Average":
                    double.TryParse(perMixRates[5 + addRow][0].ToString(), out result);
                    return result;
                default:
                    return 0;
            }
        }

        public virtual double getPricePerMix(string thickness, bool isApproved,int addRow=0)
        {
            double result;
            if (isApproved)
            {

                switch (thickness)
                {
                    case "1/4 inch Average":
                        double.TryParse(perMixRates[1 + addRow][1].ToString(), out result);
                        return result;
                    case "1/2 inch Average":
                        double.TryParse(perMixRates[2 + addRow][1].ToString(), out result);
                        return result;
                    case "3/4 inch Average":
                        double.TryParse(perMixRates[3 + addRow][1].ToString(), out result);
                        return result;
                    case "1 1/4 inch Average":
                        double.TryParse(perMixRates[5 + addRow][1].ToString(), out result);
                        return result;
                    case "1 inch Average":
                        double.TryParse(perMixRates[4 + addRow][1].ToString(), out result);
                        return result;
                    default:
                        return 0;
                }
            }
            else
            {
                double.TryParse(perMixRates[0+addRow][1].ToString(), out result);
                return result;

            }
        }
        
        public double FreightCalculator(double weight)
        {
            //double result;
            double frCalc = 0;
            double factor = 0;
            if (weight != 0)
            {

                if (weight == 0)
                {
                    frCalc = 0;
                }

                else
                {
                    if (weight > 10000)
                    {
                        double.TryParse(freightData[0][1].ToString(), out factor);
                        frCalc =  factor* weight; /*0.03*/
                    }

                    else
                    {
                        if (weight > 5000)
                        {
                            double.TryParse(freightData[1][1].ToString(), out factor);
                            frCalc = factor * weight; /*0.04*/
                        }
                        else
                        {
                            if (weight > 2000)
                            {
                                double.TryParse(freightData[2][1].ToString(), out factor);
                                frCalc = factor * weight; /*0.09*/
                            }
                            else
                            {
                                if (weight > 1000)
                                {
                                    double.TryParse(freightData[3][1].ToString(), out factor);
                                    frCalc = factor * weight; /*0.12*/
                                }
                                else
                                {
                                    if (weight > 400)
                                    {
                                        double.TryParse(freightData[4][1].ToString(), out factor);
                                        frCalc = factor;/*75*/
                                    }
                                    else
                                    {
                                        frCalc = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return frCalc;
        }

        public virtual void CalculateGridTotal()
        {

            if (Slopes.Count > 0)
            {
                ///sumtotal              
                SumTotal = Slopes.Select(x => x.Total).Sum();

                if (OverrideManually == false)
                {
                    ///sumtotalmixes
                    SumTotalMixes = Slopes.Select(x => x.TotalMixes).Sum();

                    ///sumtotalmatext

                    SumTotalMatExt = Slopes.Select(x => x.MaterialExtensionSlope).Sum();
                    //sumtotallaborext

                    SumTotalLaborExt = Slopes.Select(x => x.LaborExtensionSlope).Sum();
                }
                
            }

        }
        #endregion

        #region commands
        [XmlIgnore]
        public ICommand CalculateCostCommand
        {
            get
            {
                if (calculateCostCommand == null)
                {
                    calculateCostCommand = new DelegateCommand(CalculateCost, CanCalculate);
                }
                return calculateCostCommand;
            }
        }

        private bool CanCalculate(object obj)
        {
            return true;
        }

        private void CalculateCost(object obj)
        {
            CalculateAll();
        }
        #endregion

        #region  Temporary
        private ICommand fillValues;
        [XmlIgnore]
        public ICommand FillValues
        {
            get
            {
                if (fillValues == null)
                {
                    fillValues = new DelegateCommand(AutoFill, CanAutoFill);
                }

                return fillValues;
            }
        }

        private bool CanAutoFill(object obj)
        {
            return true;
        }

        private void AutoFill(object obj)
        {
            int i = 0;
            foreach (Slope item in Slopes)
            {
                item.Sqft = i + 1;
                item.DeckCount = i + 6;
                i++;
            }
           
        }
        #endregion
    }
}
