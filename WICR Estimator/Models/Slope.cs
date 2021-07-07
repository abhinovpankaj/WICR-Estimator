using MyToolkit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    
    public class Slope:UndoRedoObservableObject
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
        public double riserCount { get; set;}
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
                //if (value != sqft)
                //{
                //    sqft = value;
                //    OnPropertyChanged("Sqft");
                //    changeProperty();
                //}
                Set(ref sqft, value);
                changeProperty();

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
                //if (value != deckCount)
                //{
                //    deckCount = value;
                //    OnPropertyChanged("DeckCount");
                //    changeProperty();
                //}
                Set(ref deckCount, value);
                changeProperty();
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
                    RaisePropertyChanged("TotalMixes");
                }
            }
            get
            {
                if (SlopeType == "")
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
                            return Total / 32;
                        case "1/2 inch Average":
                            return(Total / 32) * 2;
                        case "3/4 inch Average":
                            return (Total / 32) * 3;
                        case "1 1/4 inch Average":
                            return (Total / 32) * 5;
                        case "1 inch Average":
                            return(Total / 32) * 4;
                        case "Access":
                        case "1/4 inch Expansion Joints":
                            return Total;
                        case "Cement Board and screws for stair applications":
                            return riserCount * (20.0 / 24 * 3.5 / 15);
                        case "1 1/4 inch Mortar Bed with 2x2 or diamond metal lathe":
                            return  hasMortarBed ? Total / 22 * 3 : Total / 22 * 5;
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
                    RaisePropertyChanged("PricePerMix");
                    RaisePropertyChanged("MaterialExtensionSlope");
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
        public double latheRate { get; set; }

        public double LaborExtensionSlope
        {        
            set
            {
                if (value!=labrExtnSlope)
                {
                    labrExtnSlope = value;
                    RaisePropertyChanged("LaborExtensionSlope");
                }
            }   
            get
            {
               return CalculateLaborextensionSlope();
            }
        }

        public bool hasMortarBed { get; set; }

        public virtual double CalculateLaborextensionSlope()
        {
            if (Thickness== "1 1/4 inch Mortar Bed with 2x2 or diamond metal lathe")
            {
                return (Total / GSLaborRate + Total / latheRate) * LaborRate;
            }
            else
                return (Total / GSLaborRate) * LaborRate; ;
        }
        private void changeProperty()
        {
            RaisePropertyChanged("DeckCount");
            RaisePropertyChanged("LaborExtensionSlope");
            RaisePropertyChanged("MaterialExtensionSlope");
            RaisePropertyChanged("Total");
            RaisePropertyChanged("TotalMixes");
            //OnPropertyChanged("AverageMixesPrice");
        }


    }

    
}
