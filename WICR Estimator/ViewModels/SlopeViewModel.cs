﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class SlopeViewModel : SlopeBaseViewModel
    {

        public SlopeViewModel()
        {
            GetSlopeDetailsFromGoogle("Dexotex Weather Wear");
            Slopes = CreateSlopes();
            CalculateAll();        
            JobSetup.OnJobSetupChange += JobSetup_OnJobSetupChange;           
        }

        private void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                isApprovedForCement = js.IsApprovedForSandCement;
                SlopeMaterialName = isApprovedForCement ? "Sand and Cement" : "Dexotex A-81 Underlayment1";
                isPrevailingWage = js.IsPrevalingWage;
                laborRate = js.LaborRate;
                hasDiscount = js.HasDiscount;
            }

            reCalculate();
        }       
   
        private ObservableCollection<Slope> CreateSlopes()
        {
            ObservableCollection<Slope> slopes = new ObservableCollection<Slope>();
            slopes.Add(new Slope
            {
                Thickness = "1/4 inch Average",
                DeckCount = 6,
                Sqft = 1,
                GSLaborRate = getGSLaborRate("1/4 inch Average"),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("1/4 inch Average", isApprovedForCement)
            });
            slopes.Add(new Slope
            {
                Thickness = "1/2 inch Average",
                DeckCount = 7,
                Sqft = 2,
                GSLaborRate = getGSLaborRate("1/2 inch Average"),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("1/2 inch Average", isApprovedForCement)
            });

            slopes.Add(new Slope
            {
                Thickness = "3/4 inch Average",
                DeckCount = 8,
                Sqft = 3,
                GSLaborRate = getGSLaborRate("3/4 inch Average"),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("3/4 inch Average", isApprovedForCement)
            });
            slopes.Add(new Slope
            {
                Thickness = "1 inch Average",
                DeckCount = 9,
                Sqft = 4,
                GSLaborRate = getGSLaborRate("1 inch Average"),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("1 inch Average", isApprovedForCement)
            });
            slopes.Add(new Slope
            {
                Thickness = "1 1/4 inch Average",
                DeckCount = 10,
                Sqft = 10,
                GSLaborRate = getGSLaborRate("1 1/4 inch Average"),
                LaborRate = laborRate,
                PricePerMix = getPricePerMix("1 1/4 inch Average", isApprovedForCement)
            });

            return slopes;
        }

        
    }
}
