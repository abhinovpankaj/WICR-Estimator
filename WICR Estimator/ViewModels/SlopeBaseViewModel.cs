using MyToolkit.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using WICR_Estimator.Models;
using WICR_Estimator.Services;

namespace WICR_Estimator.ViewModels
{
    [KnownType(typeof(SlopeViewModel))]
    [KnownType(typeof(DexoSlopeViewModel))]
    [KnownType(typeof(PedestrianSlopeViewModel))]
    [KnownType(typeof(EnduroKoteSlopeViewModel))]
    [KnownType(typeof(DualFlexSlopeViewModel))]
    [DataContract]
    public class SlopeBaseViewModel:GraphObservableObject
    {
        
        #region Private Properties

        private ObservableCollection<Slope> slopes;
        
        private ICommand calculateCostCommand;
        double productionRate = 0;
        public IList<IList<object>> pWage;
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
            IsOverrridable = true;
            UrethaneText="Slope Sand and Urethane Fill";
            SlopeHeaderText = "Slope Sand Cement Scrim";
        }

        public SlopeBaseViewModel(DBData dbData)
        {
            this.dbData = dbData;
            Slopes = new ObservableCollection<Slope>();
            SlopeTotals = new Totals { TabName = "Slope" };
            IsOverrridable = true;
            UrethaneText = "Slope Sand and Urethane Fill";
            SlopeHeaderText = "Slope Sand Cement Scrim";
            laborRate = dbData.FreightDBData.First(x => x.FactorName == "LaborRate").FactorValue;
        }

        #region public properties
        [DataMember]
        public Totals SlopeTotals;

        [DataMember]
        public bool isApprovedForCement;
        [DataMember]
        public bool isPrevailingWage;
        [DataMember]
        public double laborRate;
        [DataMember]
        public IList<IList<object>> perMixRates;
        [DataMember]
        public IList<IList<object>> freightData;
        [DataMember]
        public double prevailingWage;
        [DataMember]
        public double deductionOnLargeJob;
        [DataMember]
        public bool overrideManually;
        [DataMember]
        public bool hasDiscount;
        [DataMember]
        public double materialPerc;
        [DataMember]
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
                    RaisePropertyChanged("SlopeMaterialName");
                }
            }
        }
        [DataMember]
        private System.Windows.Visibility isUrethaneVisible= System.Windows.Visibility.Collapsed;
        [DataMember]
        public System.Windows.Visibility IsUrethaneVisible
        {
            get { return isUrethaneVisible; }
            set
            {
                isUrethaneVisible = value;
                RaisePropertyChanged("IsUrethaneVisible");
            }
        }
        public bool IsOverrridable
        {
            get;set;
        }
        [DataMember]
        public string SlopeHeaderText { get;  set; }
        [DataMember]
        public string UrethaneText { get;  set; }

        [DataMember]
        public bool OverrideManually
        {
            get
            {
                return overrideManually;
            }
            set
            {
                //if (value != overrideManually)
                //{
                //    overrideManually = value;
                //    if (!overrideManually)
                //    {

                //        totalmixesman = 0;
                //        averagemixesprice = 0;
                //        //CalculateGridTotal();
                //        //CalculateTotalMixes();
                //        OnPropertyChanged("TotalMixesMan");
                //        OnPropertyChanged("AverageMixesPrice");
                //    }


                //    OnPropertyChanged("OverrideManually");
                    Set(ref overrideManually, value);
                    if (!overrideManually)
                    {

                        totalmixesman = 0;
                        averagemixesprice = 0;
                        
                        RaisePropertyChanged("TotalMixesMan");
                        RaisePropertyChanged("AverageMixesPrice");
                    }

            }
        
        }
        [DataMember]
        public ObservableCollection<Slope> Slopes
        {
            get
            {
                return slopes;
            }
            set
            {
                //if (slopes != value)
                //{
                //    slopes = value;
                //    OnPropertyChanged("Slopes");
                //}
                Set(ref slopes, value);
            }
        }
        [DataMember]
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
                //if(value != totalmixesman)
                //{
                //    totalmixesman = value;
                //    OnPropertyChanged("TotalMixesMan");
                //}
                Set(ref totalmixesman, value);
            }
        }
        [DataMember]
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
                //if (averagemixesprice != value)
                //{
                //    averagemixesprice = value;
                //    OnPropertyChanged("AverageMixesPrice");
                //}
                Set(ref averagemixesprice, value);

            }
        }
        [DataMember]
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
                    RaisePropertyChanged("TotalWeight");
                }
            }
        }
        [DataMember]
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
                    RaisePropertyChanged("TotalFrightCost");
                }
            }
        }
        [DataMember]
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
                    RaisePropertyChanged("LaborCost");

                }
            }
        }
        [DataMember]
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
                    RaisePropertyChanged("MinimumLaborCost");
                }
            }
        }
        [DataMember]
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
                    RaisePropertyChanged("TotalLaborCost");
                }
            }
        }
        [DataMember]
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
                    RaisePropertyChanged("TotalMaterialCost");
                }
            }
        }
        [DataMember]
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
                    RaisePropertyChanged("SumTotal");
                }
            }
        }
        [DataMember]
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

                    RaisePropertyChanged("SumTotalMixes");
                }
            }
        }
        [DataMember]
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
                    RaisePropertyChanged("SumTotalMatExt");
                }
            }
        }
        [DataMember]
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
                    RaisePropertyChanged("SumTotalLaborExt");
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
            if (dbData==null)
            {
                double.TryParse(perMixRates[8][0].ToString(), out minLabVal);
            }
            else
                minLabVal = dbData.SlopeDBData.FirstOrDefault(x => x.SlopeName == "Minimum Slope Labor" && x.SlopeType == "Cement").LaborRate;
            
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
            if (projectName.Contains('.'))
            {
                projectName = projectName.Split('.')[0];
            }

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
            if (pWage!=null)
            {
                double.TryParse(pWage[1][0].ToString(), out deductionOnLargeJob);
            }
 
        }
        
        public virtual void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                if (isApprovedForCement != js.IsApprovedForSandCement)
                {
                    isApprovedForCement = js.IsApprovedForSandCement;
                    SlopeMaterialName = isApprovedForCement ? "Sand and Cement" : js.SlopeMaterialName;
                    
                }

                isPrevailingWage = js.IsPrevalingWage;
                //if (dbData == null)
                //{
                    dbData = js.dbData;
                //}
                if (isPrevailingWage)
                {
                    if (dbData==null)
                    {
                        double.TryParse(freightData[5][0].ToString(), out productionRate);
                        
                    }
                    else
                    {
                        productionRate = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "SlopeProdRate").FactorValue;
                        
                    }
                        

                }
                else
                    productionRate = 0;

                if (dbData==null)
                {
                    if (perMixRates!=null)
                    {
                        double.TryParse(perMixRates[6][1].ToString(), out manualAvgMixPrice);
                    }
                    
                    
                    if (pWage != null)
                    {
                        double.TryParse(pWage[1][0].ToString(), out deductionOnLargeJob);
                    }
                }
                else
                {
                    manualAvgMixPrice = dbData.SlopeDBData.FirstOrDefault(x => x.SlopeName == "Manul Override Avg Labor/mix").PerMixCost;
                    deductionOnLargeJob = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Deduct on Labor for large jobs").Value;
                }
                //laborRate = js.dbData.FreightDBData.First(x=>x.FactorName== "LaborRate").FactorValue;
                hasDiscount = js.HasDiscount;
                materialPerc = getMaterialDiscount(js.ProjectDelayFactor);
                prevailingWage= js.ActualPrevailingWage == 0 ? 0 : (js.ActualPrevailingWage - laborRate) / laborRate;
                
                
                //double.TryParse(pWage[0][0].ToString(), out prevailingWage);
                
                reCalculate();
            }
            
            CalculateAll();

        }
        public double getMaterialDiscount(string delay)
        {
            switch (delay)
            {
                case "0-3 Months":
                    return 0;
                case "3-6 Months":
                    return 0.02;
                case "6-12 Months":
                    return 0.04;
                case ">12 Months":
                    return 0.06;
                default:
                    return 0;
            }
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
            SlopeTotals.MaterialExtTotal = TotalMaterialCost*(1+ materialPerc);
            SlopeTotals.MaterialFreightTotal = TotalFrightCost;
        }
        public virtual void reCalculate()
        {
            foreach (Slope slp in Slopes)
            {
                if (dbData==null)
                {
                    slp.PricePerMix = getPricePerMix(slp.Thickness, isApprovedForCement);
                    slp.GSLaborRate = getGSLaborRate(slp.Thickness, 0);
                }
                else
                {
                    slp.PricePerMix = getPricePerMixDB(slp.Thickness, isApprovedForCement, "Cement");
                    slp.GSLaborRate = getGSLaborRateDB(slp.Thickness, "Cement");
                }
                
            }
            
            CalculateGridTotal();
            CalculateTotalMixes();

            SlopeTotals.LaborExtTotal = TotalLaborCost;
            SlopeTotals.MaterialExtTotal = TotalMaterialCost * (1 + materialPerc);
            SlopeTotals.MaterialFreightTotal = TotalFrightCost;
        }

        public virtual void CalculateTotalMixes()
        {
            double minLabVal = 0;
            if (Slopes.Count > 0)
            {
                LaborCost = Math.Round(SumTotalLaborExt, 2);
                if (dbData==null)
                {
                    double.TryParse(perMixRates[8][0].ToString(), out minLabVal);
                }
                else
                    minLabVal = dbData.SlopeDBData.FirstOrDefault(x => x.SlopeName == "Minimum Slope Labor" && x.SlopeType == "Cement").LaborRate;
               
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
                    return result*(1-productionRate);
                case "1/2 inch Average":
                    double.TryParse(perMixRates[2 + addRow][0].ToString(), out result);
                    return result * (1 - productionRate);
                case "3/4 inch Average":
                    double.TryParse(perMixRates[3 + addRow][0].ToString(), out result);
                    return result * (1 - productionRate);
                case "1 1/4 inch Average":
                    double.TryParse(perMixRates[4 + addRow][0].ToString(), out result);
                    return result * (1 - productionRate);
                case "1 inch Average":
                    double.TryParse(perMixRates[5 + addRow][0].ToString(), out result);
                    return result * (1 - productionRate);
                case "Access":
                    double.TryParse(perMixRates[ addRow][0].ToString(), out result);
                    return result * (1 - productionRate);
                case "1/4 inch Expansion Joints":
                    double.TryParse(perMixRates[1 + addRow][0].ToString(), out result);
                    return result * (1 - productionRate);
                case "Cement Board and screws for stair applications":
                    double.TryParse(perMixRates[2 + addRow][0].ToString(), out result);
                    return result * (1 - productionRate);
                case "1 1/4 inch Mortar Bed with 2x2 or diamond metal lathe":
                    double.TryParse(perMixRates[3 + addRow][0].ToString(), out result);
                    return result * (1 - productionRate);
                default:
                    return 0;
            }
        }

        public virtual double getPricePerMix(string thickness, bool isApproved,int addRow=0)
        {
            double result;
            if (addRow>0)
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
                        if (dbData==null)
                        {
                            double.TryParse(freightData[0][1].ToString(), out factor);
                        }
                        else
                            factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight10000").FactorValue;
                        frCalc =  factor* weight; /*0.03*/
                    }

                    else
                    {
                        if (weight > 5000)
                        {
                            if (dbData == null)
                            {
                                double.TryParse(freightData[1][1].ToString(), out factor);
                            }
                            else
                                factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight5000").FactorValue;
                            
                            frCalc = factor * weight; /*0.04*/
                        }
                        else
                        {
                            if (weight > 2000)
                            {
                                if (dbData == null)
                                {
                                    double.TryParse(freightData[2][1].ToString(), out factor);
                                }
                                else
                                    factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight2000").FactorValue;
                                frCalc = factor * weight; /*0.09*/
                            }
                            else
                            {
                                if (weight > 1000)
                                {
                                    if (dbData == null)
                                    {
                                        double.TryParse(freightData[3][1].ToString(), out factor);
                                    }
                                    else
                                        factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight1000").FactorValue;
                                    frCalc = factor * weight; /*0.12*/
                                }
                                else
                                {
                                    if (weight > 400)
                                    {
                                        if (dbData == null)
                                        {
                                            double.TryParse(freightData[4][1].ToString(), out factor);
                                        }
                                        else
                                            factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight400").FactorValue;
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

        #region DBData
        public DBData dbData;
        public void GetSlopeDetailsDB(string projectName)
        {
            if (projectName.Contains('.'))
            {
                projectName = projectName.Split('.')[0];
            }

            if (dbData == null)
            {
                dbData = DataSerializerService.DSInstance.deserializeDbData(projectName);
            }
            laborRate = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "LaborRate").FactorValue;
            manualAvgMixPrice = dbData.SlopeDBData.FirstOrDefault(x => x.SlopeName == "Manul Override Avg Labor/mix" && x.SlopeType=="Cement").PerMixCost;
            deductionOnLargeJob = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Deduct on Labor for large jobs").Value;
            
            
        }
        public virtual double getGSLaborRateDB(string thickness,string slopeType)
        {
            double result;
            result = dbData.SlopeDBData.FirstOrDefault(x => x.SlopeName == thickness && x.SlopeType==slopeType).LaborRate;
            return result * (1 - productionRate);           
        }

        public virtual double getPricePerMixDB(string thickness, bool isApproved, string slopeType)
        {
            double result;
            
            if (isApproved)
            {
                result = dbData.SlopeDBData.FirstOrDefault(x => x.SlopeName == thickness && x.SlopeType == slopeType).PerMixCost;
            }
            else
            {
                result = dbData.SlopeDBData.FirstOrDefault(x => x.SlopeName.Trim() == "Default" && x.SlopeType == slopeType).PerMixCost;               
            }
            return result;
        }

        public virtual ObservableCollection<Slope> CreateSlopesDB(string slopeType)
        {
            ObservableCollection<Slope> slopes = new ObservableCollection<Slope>();

            slopes.Add(new Slope
            {
                Thickness = "1/4 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRateDB("1/4 inch Average", slopeType),
                LaborRate = laborRate,
                PricePerMix = getPricePerMixDB("1/4 inch Average", isApprovedForCement, slopeType),
                SlopeType = slopeType == "Cement" ? "" : "URI"
            });
            slopes.Add(new Slope
            {
                Thickness = "1/2 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRateDB("1/2 inch Average", slopeType),
                LaborRate = laborRate,
                PricePerMix = getPricePerMixDB("1/2 inch Average", isApprovedForCement, slopeType),
                SlopeType = slopeType == "Cement" ? "" : "URI"
            });

            slopes.Add(new Slope
            {
                Thickness = "3/4 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRateDB("3/4 inch Average", slopeType),
                LaborRate = laborRate,
                PricePerMix = getPricePerMixDB("3/4 inch Average", isApprovedForCement, slopeType),
                SlopeType = slopeType == "Cement" ? "" : "URI"
            });
            slopes.Add(new Slope
            {
                Thickness = "1 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRateDB("1 inch Average", slopeType),
                LaborRate = laborRate,
                PricePerMix = getPricePerMixDB("1 inch Average", isApprovedForCement, slopeType),
                SlopeType = slopeType == "Cement" ? "" : "URI"
            });
            slopes.Add(new Slope
            {
                Thickness = "1 1/4 inch Average",
                DeckCount = 0,
                Sqft = 0,
                GSLaborRate = getGSLaborRateDB("1 1/4 inch Average", slopeType),
                LaborRate = laborRate,
                PricePerMix = getPricePerMixDB("1 1/4 inch Average", isApprovedForCement, slopeType),
                SlopeType = slopeType == "Cement" ? "" : "URI"
            });

            return slopes;
        }
        #endregion

        #region commands
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
