using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    [Serializable]
    public class Metal: BaseViewModel
    {
        
        public double LaborRate { set; get; }
        public MetalType Type { get; set; }
        public string Name { get; set; }
        
        public bool IsEditable { get; set; }
        //public string MetalDimensions { get; set; }

        private bool isStairMetalChecked;
        public bool IsStairMetalChecked
        {
            get
            {
                return isStairMetalChecked;
            }
            set
            {
                if (value!= isStairMetalChecked)
                {
                    isStairMetalChecked = value;
                    OnPropertyChanged("IsStairMetalChecked");
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
        private double pr;
        public double ProductionRate
        {
            get { return pr; }
            set
            {
                if (value!=pr)
                {
                    pr = value;
                    OnPropertyChanged("ProductionRate");
                }
            }
        }

        private string size;
        public string Size
        {
            get { return size; }
            set
            {
                if (value!=size)
                {
                    size = value;
                    OnPropertyChanged("Size");
                }
            }
        }
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

        public Metal(string name,string size,double productionRate,double laborRate,double units,double materialPrice,bool isStairMetal,double specialPricing=0)
            :this()
        {
            this.ProductionRate = productionRate;
            this.LaborRate = laborRate;
            this.Units = units;
            this.SpecialMetalPricing = specialPricing;
            this.MaterialPrice = materialPrice;
            this.Name = name;
            this.isStairMetalChecked = isStairMetal;
            this.Size = size;
        }
        public Metal()
        { }
    }


    public class MiscMetal:Metal
    {
        public MiscMetal()
        { }
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
