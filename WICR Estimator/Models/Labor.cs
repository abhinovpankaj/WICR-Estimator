﻿using MyToolkit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;
namespace WICR_Estimator.Models
{
    
    public class Labor : UndoRedoObservableObject
    {
        public Labor()
        {
            IsMaterialEnabled = true;
        }
        public Labor(bool ismChecked, string prjestimate, string operation, double vProdrate, double smSqftV, double hProdrate, double smSqftH, double stairsProdrate,
        double stairSqft, double hours)
            : this()
        {
            this.HorizontalProductionRate = hProdrate;
            this.SMSqftH = smSqftH;
            this.VerticalProductionRate = vProdrate;
            this.SMSqftV = smSqftV;
            this.Operation = operation;
            this.StairsProductionRate = stairSqft;
            this.StairSqft = stairSqft;
            this.Hours = hours;
        }
        //public bool IsChecked { get; set; }
        private bool ismaterialchecked;
        private bool isMaterialEnabled;
        public string Operation { get; set; }
        public double VerticalSqft { get; set; }
        public double VerticalProductionRate { get; set; }
        public double LaborRate { get; set; }

        public string Name { get; set; }
        public bool IsMaterialChecked
        {
            get
            {
                return ismaterialchecked;
            }
            set
            {
                if (value != ismaterialchecked)
                {
                    ismaterialchecked = value;
                    RaisePropertyChanged("IsMaterialChecked");
                }
            }
        }
        public bool IsMaterialEnabled
        {
            get
            {
                return isMaterialEnabled;
            }
            set
            {
                if (value != isMaterialEnabled)
                {
                    isMaterialEnabled = value;
                    RaisePropertyChanged("IsMaterialEnabled");
                }
            }
        }
         
        private double sMSqftH;
        public double SMSqftH
        {
            get { return sMSqftH; }

            set
            {
                if (value != sMSqftH)
                {
                    sMSqftH = value;
                    RaisePropertyChanged("SMSqftH");
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
                    RaisePropertyChanged("HorizontalProductionRate");
                }
            }
        }
        private double sMSqftV;
        public double SMSqftV
        {
            get { return sMSqftV; }

            set
            {
                if (value != sMSqftV)
                {
                    sMSqftV = value;
                    RaisePropertyChanged("SMSqftV");
                }
            }
        }
        private double verticleProductionRate;
        public double VerticleProductionRate
        {
            get { return verticleProductionRate; }

            set
            {
                if (value != verticleProductionRate)
                {
                    verticleProductionRate = value;
                    RaisePropertyChanged("VerticleProductionRate");
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
                    RaisePropertyChanged("StairsProductionRate");
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
                    RaisePropertyChanged("StairSqft");
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
                    RaisePropertyChanged("Hours");
                }
            }
        }

        public double SetupMinCharge { get; set; }
        
        private double laborUnitPrice;
        public double LaborUnitPrice
        {
            get { return laborUnitPrice; }

            set
            {
                if (value != laborUnitPrice)
                {
                    laborUnitPrice = value;
                    RaisePropertyChanged("LaborUnitPrice");
                }
            }
        }
        public double LaborExtension
        {
            get
            {
                return (Hours + SetupMinCharge) * LaborRate;
            }

        }

    }
}
