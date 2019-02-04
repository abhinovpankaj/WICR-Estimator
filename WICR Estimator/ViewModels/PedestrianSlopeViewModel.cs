using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    class PedestrianSlopeViewModel:SlopeBaseViewModel
    {
        private double urethaneManualAvgMixPrice;
        private ObservableCollection<Slope> urethaneSlopes;
        public ObservableCollection<Slope> UrethaneSlopes
        {
            get
            {
                return urethaneSlopes;
            }
            set
            {
                if (urethaneSlopes != value)
                {
                    urethaneSlopes = value;
                    OnPropertyChanged("UrethaneSlopes");
                }
            }
        }
        private double urethaneSumTotal;
        public double UrethaneSumTotal
        {
            get
            {
                return urethaneSumTotal;
            }
            set
            {
                if (urethaneSumTotal != value)
                {
                    urethaneSumTotal = value;
                    OnPropertyChanged("UrethaneSumTotal");
                }
            }
        }
        private double urethaneSumTotalMixes;
        public double UrethaneSumTotalMixes
        {
            get
            {
                return urethaneSumTotalMixes;
            }
            set
            {
                if (urethaneSumTotalMixes != value)
                {
                    urethaneSumTotalMixes = value;
                    OnPropertyChanged("UrethaneSumTotalMixes");
                }
            }
        }
        //  UrethaneSumTotalLaborExt
        private double urethaneSumTotalMatExt;
        public double UrethaneSumTotalMatExt
        {
            get
            {
                return urethaneSumTotalMatExt;
            }
            set
            {
                if (urethaneSumTotalMatExt != value)
                {
                    urethaneSumTotalMatExt = value;
                    OnPropertyChanged("UrethaneSumTotalMatExt");
                }
            }
        }
        private double urethaneSumTotalLaborExt;
        public double UrethaneSumTotalLaborExt
        {
            get
            {
                return urethaneSumTotalLaborExt;
            }
            set
            {
                if (urethaneSumTotalLaborExt != value)
                {
                    urethaneSumTotalLaborExt = value;
                    OnPropertyChanged("UrethaneSumTotalLaborExt");
                }
            }
        }
        private double UrethaneMinimumlaborcost;
        public double UrethaneMinimumLaborCost
        {
            get
            {
                return UrethaneMinimumlaborcost;
            }
            set
            {
                if (value != UrethaneMinimumlaborcost)
                {
                    UrethaneMinimumlaborcost = value;
                    OnPropertyChanged("UrethaneMinimumLaborCost");
                }
            }
        }

        private bool urethaneOverrideManually;
        public bool UrethaneOverrideManually
        {
            get { return urethaneOverrideManually; }
            set
            {
                if (value!=urethaneOverrideManually)
                {
                    urethaneOverrideManually = value;
                    OnPropertyChanged("UrethaneOverrideManually");
                }
            }
        }

        private double urethaneTotalMixesMan;
        public double UrethaneTotalMixesMan
        {
            get { return urethaneTotalMixesMan; }
            set
            {
                if (value!=urethaneTotalMixesMan)
                {
                    urethaneTotalMixesMan = value;
                    OnPropertyChanged("UrethaneTotalMixesMan");
                }
            }
        }
        private double urethaneAverageMixesPrice;
        public double UrethaneAverageMixesPrice
        {
            get { return urethaneAverageMixesPrice; }
            set
            {
                if (value != urethaneAverageMixesPrice)
                {
                    urethaneAverageMixesPrice = value;
                    OnPropertyChanged("UrethaneAverageMixesPrice");
                }
            }
        }

        public override void reCalculate()
        {
            foreach (Slope slp in UrethaneSlopes)
            {
                slp.PricePerMix = getPricePerMix(slp.Thickness, isApprovedForCement,9);
            }
            base.reCalculate();
        }
        public PedestrianSlopeViewModel(JobSetup Js)
        {
            IsUrethaneVisible = System.Windows.Visibility.Visible;
            GetSlopeDetailsFromGoogle(Js.ProjectName);
            double.TryParse(perMixRates[15][1].ToString(), out urethaneManualAvgMixPrice);
            isApprovedForCement = Js.IsApprovedForSandCement;
            
            Slopes = CreateSlopes(0);
            UrethaneSlopes = CreateSlopes(9);
            CalculateAll();
            Js.OnJobSetupChange += JobSetup_OnJobSetupChange;
        }
        public PedestrianSlopeViewModel()
        {
        }
        public override void CalculateAll()
        {
            base.CalculateAll();
            if (UrethaneOverrideManually)
            {
                CalculateManualUrethane();
            }

            SlopeTotals.LaborExtTotal = TotalLaborCost;
            SlopeTotals.MaterialExtTotal = TotalMaterialCost;
            SlopeTotals.MaterialFreightTotal = TotalFrightCost;
        }
        public void CalculateManualUrethane()
        {
            //base.CalculateManual();

            double minLabVal = 0;
            UrethaneSumTotalMixes = UrethaneTotalMixesMan;
            UrethaneSumTotalMatExt = UrethaneAverageMixesPrice * UrethaneTotalMixesMan;
            TotalMaterialCost = UrethaneSumTotalMatExt+ SumTotalMatExt;
            double urethaneTotalWeight = Math.Round(50 * UrethaneTotalMixesMan, 2);
            TotalFrightCost = Math.Round(FreightCalculator(TotalWeight), 2) + Math.Round(FreightCalculator(urethaneTotalWeight), 2);
            TotalWeight = urethaneTotalWeight + TotalWeight;

            
            UrethaneSumTotalLaborExt = Math.Round(UrethaneTotalMixesMan * urethaneManualAvgMixPrice, 2);
            double.TryParse(perMixRates[17][0].ToString(), out minLabVal);
            UrethaneMinimumLaborCost = minLabVal * laborRate;
            TotalLaborCost = (UrethaneSumTotalLaborExt == 0 ? 0 : UrethaneMinimumLaborCost > UrethaneSumTotalLaborExt ? UrethaneMinimumLaborCost : UrethaneSumTotalLaborExt)+ TotalLaborCost ;

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
        public override void CalculateGridTotal()
        {
            base.CalculateGridTotal();

            if (UrethaneSlopes.Count > 0)
            {
                ///sumtotal              
                UrethaneSumTotal = Math.Round(UrethaneSlopes.Select(x => x.Total).Sum(), 2);

                if (OverrideManually == false)
                {
                    ///sumtotalmixes
                    UrethaneSumTotalMixes = Math.Round(UrethaneSlopes.Select(x => x.TotalMixes).Sum(), 2);

                    ///sumtotalmatext

                    UrethaneSumTotalMatExt = Math.Round(UrethaneSlopes.Select(x => x.MaterialExtensionSlope).Sum(), 2);
                    //sumtotallaborext

                    UrethaneSumTotalLaborExt = Math.Round(UrethaneSlopes.Select(x => x.LaborExtensionSlope).Sum(), 2);
                }

            }
        }

        public override void CalculateTotalMixes()
        {
            base.CalculateTotalMixes();
            double minLabVal = 0;
            double lCost = 0;
            if (UrethaneSlopes.Count > 0)
            {
                if (UrethaneOverrideManually == false)
                {
                    lCost = Math.Round(UrethaneSumTotalLaborExt, 2);
                    double.TryParse(perMixRates[17][0].ToString(), out minLabVal);

                    UrethaneMinimumLaborCost = minLabVal * laborRate;

                    lCost = UrethaneSumTotalLaborExt == 0 ? 0 : lCost > UrethaneMinimumLaborCost ? lCost : UrethaneMinimumLaborCost;
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
                    TotalMaterialCost = TotalMaterialCost + Math.Round(UrethaneSumTotalMatExt, 2);
                    double urethaneTotalWeight = Math.Round(50 * UrethaneSumTotalMixes, 2);
                    TotalFrightCost = Math.Round(FreightCalculator(TotalWeight), 2)+ Math.Round(FreightCalculator(urethaneTotalWeight), 2);
                    TotalWeight = TotalWeight + urethaneTotalWeight ;
                    
                }
            }
        }
    }
}
