using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class SlopeBaseViewModel:BaseViewModel
    {
        #region Private Properties
        public Totals SlopeTotals;
        private ObservableCollection<Slope> slopes;
        public bool isApprovedForCement;
        public bool isPrevailingWage;
        public double laborRate;
        private ICommand calculateCostCommand;
        private IList<IList<object>> perMixRates;
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
        public double prevailingWage;
        public double deductionOnLargeJob;
        public bool overrideManually;
        public bool hasDiscount;
        private string slopeMaterialName;
        private double manualAvgMixPrice;
        #endregion

        public SlopeBaseViewModel()
        {
            Slopes = new ObservableCollection<Slope>();
            SlopeTotals = new Totals { TabName = "Slope" };
            isApprovedForCement = false;
            SlopeMaterialName = "Dexotex A-81 Underlayment";
        }

        #region public properties
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
        private void CalculateManual()
        {
            SumTotalMixes = TotalMixesMan;
            SumTotalMatExt = AverageMixesPrice * TotalMixesMan;
            TotalMaterialCost = SumTotalMatExt;
            TotalWeight = Math.Round(50 * TotalMixesMan, 2);
            TotalFrightCost = Math.Round(FreightCalculator(TotalWeight), 2);
            SumTotalLaborExt = Math.Round(TotalMixesMan * manualAvgMixPrice, 2);
            MinimumLaborCost = 6 * laborRate;
            TotalLaborCost = MinimumLaborCost > SumTotalLaborExt ? MinimumLaborCost : SumTotalLaborExt;


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
                if (overrideManually && value != totalmixesman)
                {
                    totalmixesman = value;
                    CalculateManual();
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
                if (overrideManually && averagemixesprice != value)
                {
                    averagemixesprice = value;
                    CalculateManual();
                    OnPropertyChanged("AverageMixesPrice");
                }
                else
                    averagemixesprice = 0;


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
                
            }
            double.TryParse(perMixRates[6][1].ToString(), out manualAvgMixPrice);
            double.TryParse(pWage[0][0].ToString(), out prevailingWage);
            double.TryParse(pWage[1][0].ToString(), out deductionOnLargeJob);

        }
        public void CalculateAll()
        {
            CalculateGridTotal();
            CalculateTotalMixes();

            SlopeTotals.LaborExtTotal = TotalLaborCost;
            SlopeTotals.MaterialExtTotal = TotalMaterialCost;
            SlopeTotals.MaterialFreightTotal = TotalFrightCost;
        }
        public void reCalculate()
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
        private void CalculateTotalMixes()
        {
            if (Slopes.Count > 0)
            {
                LaborCost = Math.Round(SumTotalLaborExt, 2);

                MinimumLaborCost = 6 * laborRate;

                double lCost = LaborCost > MinimumLaborCost ? LaborCost : MinimumLaborCost;
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
        public double getGSLaborRate(string thickness)
        {
            double result;
            switch (thickness)
            {
                case "1/4 inch Average":
                    double.TryParse(perMixRates[1][0].ToString(), out result);
                    return result;
                case "1/2 inch Average":
                    double.TryParse(perMixRates[2][0].ToString(), out result);
                    return result;
                case "3/4 inch Average":
                    double.TryParse(perMixRates[3][0].ToString(), out result);
                    return result;
                case "1 1/4 inch Average":
                    double.TryParse(perMixRates[4][0].ToString(), out result);
                    return result;
                case "1 inch Average":
                    double.TryParse(perMixRates[5][0].ToString(), out result);
                    return result;
                default:
                    return 0;
            }
        }

        public double getPricePerMix(string thickness, bool isApproved)
        {
            double result;
            if (isApproved)
            {

                switch (thickness)
                {
                    case "1/4 inch Average":
                        double.TryParse(perMixRates[1][1].ToString(), out result);
                        return result;
                    case "1/2 inch Average":
                        double.TryParse(perMixRates[2][1].ToString(), out result);
                        return result;
                    case "3/4 inch Average":
                        double.TryParse(perMixRates[3][1].ToString(), out result);
                        return result;
                    case "1 1/4 inch Average":
                        double.TryParse(perMixRates[5][1].ToString(), out result);
                        return result;
                    case "1 inch Average":
                        double.TryParse(perMixRates[4][1].ToString(), out result);
                        return result;
                    default:
                        return 0;
                }
            }
            else
            {
                double.TryParse(perMixRates[0][1].ToString(), out result);
                return result;

            }
        }
        private double FreightCalculator(double weight)
        {
            double result;
            double frCalc = 0;
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
                        frCalc = 0.03 * weight;
                    }

                    else
                    {
                        if (weight > 5000)
                        {
                            frCalc = 0.04 * weight;
                        }
                        else
                        {
                            if (weight > 2000)
                            {
                                frCalc = 0.09 * weight;
                            }
                            else
                            {
                                if (weight > 1000)
                                {
                                    frCalc = 0.12 * weight;
                                }
                                else
                                {
                                    if (weight > 400)
                                    {
                                        frCalc = 75;
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
            result = frCalc;
            return result;
        }

        private void CalculateGridTotal()
        {

            if (Slopes.Count > 0)
            {
                ///sumtotal              
                SumTotal = Math.Round(Slopes.Select(x => x.Total).Sum(), 2);

                if (OverrideManually == false)
                {
                    ///sumtotalmixes
                    SumTotalMixes = Math.Round(Slopes.Select(x => x.TotalMixes).Sum(), 2);

                    ///sumtotalmatext

                    SumTotalMatExt = Math.Round(Slopes.Select(x => x.MaterialExtensionSlope).Sum(), 2);
                    //sumtotallaborext

                    SumTotalLaborExt = Math.Round(Slopes.Select(x => x.LaborExtensionSlope).Sum(), 2);
                }
                
            }

        }
        #endregion

        #region commands
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
    }
}
