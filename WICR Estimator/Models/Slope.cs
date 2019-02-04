﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    
    public class Slope:BaseViewModel
    {
        public Slope()
        {

        }
        public Slope(string thickNess,double sqft,int deckCount,
            double pricePermix,double laborRate,double LaborRateGS)
        {
            this.Thickness = thickNess;
            this.Sqft = sqft;
            this.DeckCount = deckCount;
            this.PricePerMix = pricePermix;
            this.LaborRate = laborRate;
            this.GSLaborRate = LaborRateGS;           

        }
        public string SlopeType { get; set; }
        public string Thickness { get; set; }
        public double LaborRate { get; set; }
        public double GSLaborRate { get; set; }

        private double sqft;

        public double Sqft
        {
            get
            {
                return sqft;
            }
            set
            {
                if (value != sqft)
                {
                    sqft = value;
                    OnPropertyChanged("Sqft");
                    changeProperty();
                }
            }
        }

        private int deckCount;
        public int DeckCount
        {
            get
            {
                return deckCount;
            }
            set
            {
                if (value != deckCount)
                {
                    deckCount = value;
                    OnPropertyChanged("DeckCount");
                    changeProperty();
                }
            }
        }
        public double Total
        {
            get
            {
                return deckCount*sqft;
            }
        }
        //private double totalMixes;

        public  double TotalMixes
        {
            //set
            //{
            //    if (value!=totalMixes)
            //    {
            //        totalMixes = value;
            //        OnPropertyChanged("TotalMixes");
            //    }
            //}
            get
            {
                if (SlopeType=="")
                {
                    switch (Thickness)
                    {
                        case "1/4 inch Average":
                            return Math.Round(Total / 22, 2);
                        case "1/2 inch Average":
                            return Math.Round((Total / 22) * 2, 2);
                        case "3/4 inch Average":
                            return Math.Round((Total / 22) * 3, 2);
                        case "1 1/4 inch Average":
                            return Math.Round((Total / 22) * 5, 2);
                        case "1 inch Average":
                            return Math.Round((Total / 22) * 4, 2);
                        default:
                            return 0;
                    }
                }
                else
                {
                    switch (Thickness)
                    {
                        case "1/4 inch Average":
                            return Math.Round(Total / 32, 2);
                        case "1/2 inch Average":
                            return Math.Round((Total / 32) * 2, 2);
                        case "3/4 inch Average":
                            return Math.Round((Total / 32) * 3, 2);
                        case "1 1/4 inch Average":
                            return Math.Round((Total / 32) * 5, 2);
                        case "1 inch Average":
                            return Math.Round((Total / 32) * 4, 2);
                        default:
                            return 0;
                    }
                }
                
            }
        }
        private double priceperMix;
        public double PricePerMix
        {
            get { return priceperMix; }
            set
            {
                if (value!=priceperMix)
                {
                    priceperMix = value;
                    OnPropertyChanged("PricePerMix");
                    OnPropertyChanged("MaterialExtensionSlope");
                }
            }
        }

        public double MaterialExtensionSlope
        {
            get
            {
                return TotalMixes * PricePerMix;
            }
        }

        public double LaborExtensionSlope
        {
            
            get
            {
                return (Total / GSLaborRate) * LaborRate;
            }
        }

        private void changeProperty()
        {
            OnPropertyChanged("DeckCount");
            OnPropertyChanged("LaborExtensionSlope");
            OnPropertyChanged("MaterialExtensionSlope");
            OnPropertyChanged("Total");
            OnPropertyChanged("TotalMixes");
            //OnPropertyChanged("AverageMixesPrice");
        }
    }
}
