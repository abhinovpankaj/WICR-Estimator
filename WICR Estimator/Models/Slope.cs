using System;
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
                //DualFlex Mortar slope 
                if (Thickness== "Cement Board and screws for stair applications")
                {
                    return sqft;
                }
                else
                    return deckCount*sqft;
            }
        }
        private double totalMixes;

        public  double TotalMixes
        {
            set
            {
                if (value != totalMixes)
                {
                    totalMixes = value;
                    OnPropertyChanged("TotalMixes");
                }
            }
            get
            {
                if (SlopeType=="")
                {
                    switch (Thickness)
                    {
                        case "1/4 inch Average":
                            //return Math.Round(Total / 22, 2);
                            return Total / 22;
                        case "1/2 inch Average":
                            return (Total / 22) * 2;
                        case "3/4 inch Average":
                            return (Total / 22) * 3;
                        case "1 1/4 inch Average":
                            return (Total / 22) * 5;
                        case "1 inch Average":
                            return (Total / 22) * 4;
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
                        case "Access":
                        case "1/4 inch Expansion Joints":
                            return Total;
                        case "Cement Board and screws for stair applications":
                            return 0;
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
        private double labrExtnSlope;
        public double LaborExtensionSlope
        {        
            set
            {
                if (value!=labrExtnSlope)
                {
                    labrExtnSlope = value;
                    OnPropertyChanged("LaborExtensionSlope");
                }
            }   
            get
            {
               return CalculateLaborextensionSlope();
            }
        }

        public virtual double CalculateLaborextensionSlope()
        {
            return (Total / GSLaborRate) * LaborRate; ;
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
