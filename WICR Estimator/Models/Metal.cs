using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    public class Metal: BaseViewModel
    {
        private double LaborRate;
        public MetalType Type { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public bool IsReadOnly { get; set; }
        private bool isStairMetal;
        public bool IsStairMetal
        {
            get
            {
                return isStairMetal;
            }
            set
            {
                if (value!=isStairMetal)
                {
                    isStairMetal = value;
                    OnPropertyChanged("IsStairMetal");
                }
            }
        }
        private double units;
        public double Units
        {
            get

            {
                return units;
            }
            set
            {
                if (units != value)
                {
                    units = value;
                    OnPropertyChanged("Units");
                    OnPropertyChanged("LaborExtension");
                    OnPropertyChanged("MaterialExtension");
                }
            }
        }
        public double ProductionRate { get; set; }
        public double NoOfHrs
        {
            get
            {
                return Units / ProductionRate;
            }
        }
        

        public double LaborUnitPrice
        {
            get
            {
                return LaborRate / ProductionRate;
            }
        }
        public virtual  double LaborExtension { get { return Units * LaborUnitPrice; } }
        private double materialPrice;
        public double MaterialPrice
        {
            get
            {
                return materialPrice;
            }
            set
            {
                if (materialPrice!=value)
                {
                    materialPrice = value;
                    OnPropertyChanged("MaterialPrice");
                    OnPropertyChanged("MaterialExtension");
                }
            }
        }

        public double MaterialExtension
        {
            get
            {
                if (SpecialMetalPricing == 0)
                {
                    return MaterialPrice * Units;
                }
                else
                    return SpecialMetalPricing * Units;
            }
        }
        private double specialMetalPricing;
        public double SpecialMetalPricing
        {
            get
            {
                return specialMetalPricing;
            }
            set
            {
                if (specialMetalPricing != value)
                {
                    specialMetalPricing = value;
                    OnPropertyChanged("SpecialMetalPricing");
                    OnPropertyChanged("MaterialExtension");
                }
            }
        }

        public Metal(string name,double productionRate,double laborRate,double units,double materialPrice,bool isStairMetal,double specialPricing=0)
            :this()
        {
            this.ProductionRate = productionRate;
            this.LaborRate = laborRate;
            this.Units = units;
            this.SpecialMetalPricing = specialPricing;
            this.MaterialPrice = materialPrice;
            this.Name = name;
            this.IsStairMetal = isStairMetal;
        }
        public Metal()
        { }
    }


    public class MiscMetal:Metal
    {
        private double unitPrice;
        public double UnitPrice
        {
            get { return unitPrice; }
            set
            {
                if (unitPrice!=value)
                {
                    unitPrice = value;
                    OnPropertyChanged("UnitPrice");
                    OnPropertyChanged("LaborExtension");
                }
               
            }
        }

        public override double LaborExtension
        {
            get
            {
                return Math.Round(UnitPrice * Units, 2);
            }
        }
        public bool CanRemove { get; set; }
    }
    public enum MetalType
    {
        Copper,
        RegularSteel,
        StainlessSteel
    }

    public enum Vendor
    {
        Chivon,
        ThunderBird
    }
}
