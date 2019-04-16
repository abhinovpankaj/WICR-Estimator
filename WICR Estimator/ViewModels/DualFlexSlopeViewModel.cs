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
    class DualFlexSlopeViewModel : SlopeBaseViewModel
    {
        #region Properties
        private ObservableCollection<Slope> mortarSlopes;
        [DataMember]
        public ObservableCollection<Slope> UrethaneSlopes
        {
            get
            {
                return mortarSlopes;
            }
            set
            {
                if (mortarSlopes != value)
                {
                    mortarSlopes = value;
                    OnPropertyChanged("UrethaneSlopes");
                }
            }
        }

        private double mortarSumTotal;
        [DataMember]
        public double MortarSumTotal
        {
            get
            {
                return mortarSumTotal;
            }
            set
            {
                if (mortarSumTotal != value)
                {
                    mortarSumTotal = value;
                    OnPropertyChanged("MortarSumTotal");
                }
            }
        }
        private double mortarSumTotalMixes;
        [DataMember]
        public double MortarSumTotalMixes
        {
            get
            {
                return mortarSumTotalMixes;
            }
            set
            {
                if (mortarSumTotalMixes != value)
                {
                    mortarSumTotalMixes = value;
                    OnPropertyChanged("MortarSumTotalMixes");
                }
            }
        }
        //  mortarSumTotalLaborExt
        private double mortarSumTotalMatExt;
        [DataMember]
        public double MortarSumTotalMatExt
        {
            get
            {
                return mortarSumTotalMatExt;
            }
            set
            {
                if (mortarSumTotalMatExt != value)
                {
                    mortarSumTotalMatExt = value;
                    OnPropertyChanged("MortarSumTotalMatExt");
                }
            }
        }
        private double mortarSumTotalLaborExt;
        [DataMember]
        public double MortarSumTotalLaborExt
        {
            get
            {
                return mortarSumTotalLaborExt;
            }
            set
            {
                if (mortarSumTotalLaborExt != value)
                {
                    mortarSumTotalLaborExt = value;
                    OnPropertyChanged("MortarSumTotalLaborExt");
                }
            }
        }

        public double MortarMinimumLaborCost { get; private set; }
        #endregion

        private bool easyAccessNoLadder;
        private double totalSqft;
        private double linearCopingFootage;
        private double riserCount;
        private bool hasMortarBed;
        private bool hasQuarterMortarBed;
        
        public DualFlexSlopeViewModel(JobSetup Js)
        {
            IsUrethaneVisible = System.Windows.Visibility.Visible;
            GetSlopeDetailsFromGoogle(Js.ProjectName);

            isApprovedForCement = Js.IsApprovedForSandCement;
            totalSqft = Js.TotalSqft;
            Slopes = CreateSlopes(0);
            UrethaneSlopes = CreateSlopes(9);
            UpdateSpecialSlope();
            CalculateAll();
            Js.JobSetupChange += JobSetup_OnJobSetupChange;
        }
        public DualFlexSlopeViewModel()
        {
        }

        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                linearCopingFootage = js.LinearCopingFootage;
                riserCount = js.RiserCount;
                isApprovedForCement = js.IsApprovedForSandCement;
                easyAccessNoLadder = js.HasEasyAccess;
                totalSqft = js.TotalSqft;
                hasMortarBed = js.HasQuarterLessMortarBed;
                hasQuarterMortarBed = js.HasQuarterMortarBed;
            }
            UpdateSpecialSlope();
            base.JobSetup_OnJobSetupChange(sender, e);
            
        }

        private void UpdateSpecialSlope()
        {
            double val1, val2;
            double.TryParse(perMixRates[13][0].ToString(), out val1);
            double.TryParse(perMixRates[14][0].ToString(), out val2);
            double latherate = hasMortarBed ?val1: val2;
            Slope slp = UrethaneSlopes.FirstOrDefault(x => x.Thickness == "1 1/4 inch Mortar Bed with 2x2 or diamond metal lathe");
            if (slp!=null)
            {
                slp.LaborExtensionSlope = (slp.Sqft / slp.GSLaborRate + slp.Sqft / latherate) * laborRate;
            }
            
        }

        
        public override double getPricePerMix(string thickness, bool isApproved, int addRow = 0)
        {
            double result,val1,val2,val3;

            if (addRow==0)
            {
                return base.getPricePerMix(thickness, isApproved, addRow);
            }
            else
            {
                switch (thickness)
                {
                    case "Access":
                        double.TryParse(perMixRates[ addRow][1].ToString(), out result);
                        return result;
                    case "1/4 inch Expansion Joints":
                        double.TryParse(perMixRates[1+ addRow][1].ToString(), out result);
                        return result;
                    case "Cement Board and screws for stair applications":
                        double.TryParse(perMixRates[2 + addRow][1].ToString(), out result);
                        return result;
                    case "1 1/4 inch Mortar Bed with 2x2 or diamond metal lathe":
                        double.TryParse(perMixRates[3 + addRow][1].ToString(), out result);
                        double.TryParse(perMixRates[4 + addRow][1].ToString(), out val1);
                        double.TryParse(perMixRates[5 + addRow][1].ToString(), out val2);
                        double.TryParse(perMixRates[6 + addRow][1].ToString(), out val3);
                        return CalculateSpecialPriceMix(result,val1,val2,val3);
                    default:
                        return 0;
                }
            }
            
        }

        private double CalculateSpecialPriceMix(double result, double val1, double val2, double val3)
        {
            double mixPrice=0;
            if (isApprovedForCement&&hasQuarterMortarBed)
            {
                mixPrice = result;
            }
            else
            {
                if (isApprovedForCement&&hasMortarBed)
                {
                    mixPrice = val1;
                }
                else
                {
                    if (!isApprovedForCement&&hasQuarterMortarBed)
                    {
                        mixPrice = val2;
                    }
                    else
                    {
                        if (!isApprovedForCement&&hasMortarBed)
                        {
                            mixPrice = val3;
                        }
                    }
                }
            }
            return mixPrice;
        }

        public override ObservableCollection<Slope> CreateSlopes(int RowN = 0)
        {
            if (RowN==9)
            {
                ObservableCollection<Slope> slopes = new ObservableCollection<Slope>();
                slopes.Add(new Slope
                {
                    Thickness = "Access",
                    DeckCount = 1,
                    Sqft = easyAccessNoLadder ?0:totalSqft,
                    GSLaborRate = getGSLaborRate("Access", RowN),
                    LaborRate = laborRate,
                    PricePerMix = getPricePerMix("Access", isApprovedForCement, RowN),
                    SlopeType = RowN == 0 ? "" : "DualFlex"
                });
                slopes.Add(new Slope
                {
                    Thickness = "1/4 inch Expansion Joints",
                    DeckCount = 1,
                    Sqft = totalSqft+linearCopingFootage /120*22,
                    GSLaborRate = getGSLaborRate("1/4 inch Expansion Joints", RowN),
                    LaborRate = laborRate,
                    PricePerMix = getPricePerMix("1/4 inch Expansion Joints", isApprovedForCement, RowN),
                    SlopeType = RowN == 0 ? "" : "DualFlex",                    
                });
                Slope slp=
                new Slope
                {
                    Thickness = "Cement Board and screws for stair applications",
                    DeckCount = 0,
                    Sqft = riserCount*20*3.5 / 24,
                    GSLaborRate = getGSLaborRate("Cement Board and screws for stair applications", RowN),
                    LaborRate = laborRate,
                    PricePerMix = getPricePerMix("Cement Board and screws for stair applications", isApprovedForCement, RowN),
                    SlopeType = RowN == 0 ? "" : "DualFlex"
                };
                slp.TotalMixes = riserCount * (20 / 24 * 3.5 / 15);
                slopes.Add(slp);

                slp=new Slope
                {
                    Thickness = "1 1/4 inch Mortar Bed with 2x2 or diamond metal lathe",
                    DeckCount = 1,
                    Sqft = totalSqft,
                    GSLaborRate = getGSLaborRate("1 1/4 inch Mortar Bed with 2x2 or diamond metal lathe", RowN),
                    LaborRate = laborRate,
                    PricePerMix = getPricePerMix("1 1/4 inch Mortar Bed with 2x2 or diamond metal lathe", isApprovedForCement, RowN),
                    SlopeType = RowN == 0 ? "" : "DualFlex"
                };
                slp.TotalMixes = hasMortarBed ? slp.Total / 22 * 3 : slp.Total / 22 * 5;
                slopes.Add(slp);
                return slopes;
            }
            else
                return base.CreateSlopes(RowN);
        }
        public override void CalculateGridTotal()
        {
            base.CalculateGridTotal();

            if (UrethaneSlopes.Count > 0)
            {
                ///sumtotal              
                MortarSumTotal = Math.Round(UrethaneSlopes.Select(x => x.Total).Sum(), 2);

                ///sumtotalmixes
                MortarSumTotalMixes = Math.Round(UrethaneSlopes.Select(x => x.TotalMixes).Sum(), 2);

                ///sumtotalmatext

                MortarSumTotalMatExt = Math.Round(UrethaneSlopes.Select(x => x.MaterialExtensionSlope).Sum(), 2);
                //sumtotallaborext

                MortarSumTotalLaborExt = Math.Round(UrethaneSlopes.Select(x => x.LaborExtensionSlope).Sum(), 2);

            }
        }

        public override void CalculateTotalMixes()
        {
            base.CalculateTotalMixes();
            double minLabVal = 0;
            double lCost = 0;
            if (UrethaneSlopes.Count > 0)
            {

                lCost = Math.Round(MortarSumTotalLaborExt, 2);
                double.TryParse(perMixRates[16][0].ToString(), out minLabVal);

                MortarMinimumLaborCost = minLabVal * laborRate;

                lCost = MortarSumTotalLaborExt == 0 ? 0 : lCost > MortarMinimumLaborCost ? lCost : MortarMinimumLaborCost;
                if (isPrevailingWage)
                {
                    if (hasDiscount)
                        lCost = lCost * (1 + prevailingWage + deductionOnLargeJob);
                    else
                        lCost = lCost * (1 + prevailingWage);

                }
                else
                {
                    if (hasDiscount)
                        lCost = lCost * (1 + deductionOnLargeJob);

                }
                TotalLaborCost = TotalLaborCost + lCost;
                TotalMaterialCost = TotalMaterialCost + Math.Round(MortarSumTotalMatExt, 2);
                double mortarTotalWeight = Math.Round(50 * MortarSumTotalMixes, 2);
                TotalFrightCost = Math.Round(FreightCalculator(TotalWeight), 2) + Math.Round(FreightCalculator(mortarTotalWeight), 2);
                TotalWeight = TotalWeight + mortarTotalWeight;


            }
        }
    }
}
