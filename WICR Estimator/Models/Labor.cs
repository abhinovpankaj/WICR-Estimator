using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;
namespace WICR_Estimator.Models
{
    public class Labor : BaseViewModel
    {
        public Labor()
        {
        }

        public string Name { get; set; }
        public string Operation { get; set; }
        public double VerticalSqft { get; set; }
        public double  VerticalProductionRate { get; set; }
        public double LaborRate { get; set; }
        private double horizontalSqft;
        public double HorizontalSqft
        {
            get { return horizontalSqft; }

            set
            {
                if (value!=horizontalSqft)
                {
                    horizontalSqft = value;
                    OnPropertyChanged("HorizontalSqft");
                }
            }
        }
        private double horizontalProductionRate;
        public double HorizontalProductionRate
        {
            get { return horizontalProductionRate; }

            set
            {
                if (value != horizontalProductionRate)
                {
                    horizontalProductionRate = value;
                    OnPropertyChanged("HorizontalProductionRate");
                }
            }
        }
        private double stairsProductionRate;
        public double StairsProductionRate
        {
            get { return stairsProductionRate; }

            set
            {
                if (value != stairsProductionRate)
                {
                    stairsProductionRate = value;
                    OnPropertyChanged("StairsProductionRate");
                }
            }
        }
        private double stairSqft;
        public double StairSqft
        {
            get { return stairSqft; }

            set
            {
                if (value != stairSqft)
                {
                    stairSqft = value;
                    OnPropertyChanged("StairSqft");
                }
            }
        }
        private double hours;
        public double Hours
        {
            get { return hours; }

            set
            {
                if (value != hours)
                {
                    hours = value;
                    OnPropertyChanged("Hours");
                }
            }
        }
        public double SetupMinCharge { get; set; }
        //private double setupMinCharge;
        //public double SetupMinCharge
        //{
        //    get { return setupMinCharge; }

        //    set
        //    {
        //        if (value != setupMinCharge)
        //        {
        //            setupMinCharge = value;
        //            OnPropertyChanged("SetupMinCharge");
        //        }
        //    }
        //}
        private double laborUnitPrice;
        public double LaborUnitPrice
        {
            get { return laborUnitPrice; }

            set
            {
                if (value != laborUnitPrice)
                {
                    laborUnitPrice = value;
                    OnPropertyChanged("LaborUnitPrice");
                }
            }
        }
        public double LaborExtension
        {
            get
            {
                return (Hours + SetupMinCharge) *LaborRate;
            }

        }

    }
}
