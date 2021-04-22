using MyToolkit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    [Serializable]
    public class Metal: UndoRedoObservableObject
    {
        
        public double LaborRate { set; get; }
        public MetalType Type { get; set; }
        public string Name { get; set; }
        
        public bool IsEditable { get; set; }
        //public string MetalDimensions { get; set; }
        private System.Windows.Visibility isStairMetal;
        public System.Windows.Visibility IsStairMetal
        {
            get
            {
                return isStairMetal;
            }
            set
            {
                if (value != isStairMetal)
                {
                    isStairMetal = value;

                    RaisePropertyChanged("IsStairMetal");

                }
                
            }
        }
        private bool isStairMetalChecked;
        public bool IsStairMetalChecked
        {
            get
            {
                return isStairMetalChecked;
            }
            set
            {
                //if (value != isStairMetalChecked)
                //{
                //    isStairMetalChecked = value;

                //    RaisePropertyChanged("IsStairMetalChecked");

                //    RaisePropertyChanged("LaborExtension");
                //    RaisePropertyChanged("MaterialExtension");
                //}
                Set(ref isStairMetalChecked, value);
                RaisePropertyChanged("LaborExtension");
                RaisePropertyChanged("MaterialExtension");
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
                
                if (Name=="STAIR METAL"|| Name== "Nosing for Concrete risers" || Name== "Pins & Loads for metal over concrete")
                {
                    if (units != value)
                    {
                        units = value;
                        RaisePropertyChanged("Units");
                        RaisePropertyChanged("LaborExtension");
                        RaisePropertyChanged("MaterialExtension");
                    }
                }
                else
                {
                    Set(ref units, value);
                    RaisePropertyChanged("LaborExtension");
                    RaisePropertyChanged("MaterialExtension");
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
                    RaisePropertyChanged("ProductionRate");
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
                    RaisePropertyChanged("Size");
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
        public virtual  double LaborExtension
        {
            get
            {
                if (Name.Contains("STAIR METAL"))
                {
                    return isStairMetalChecked ? Units * LaborUnitPrice : 0;
                }
                else
                    return  Units * LaborUnitPrice;

            }
        }
        private double materialPrice;
        public double MaterialPrice
        {
            get
            {
                return materialPrice;
            }
            set
            {
                //if (materialPrice!=value)
                //{
                //    materialPrice = value;
                //    RaisePropertyChanged("MaterialPrice");
                //    RaisePropertyChanged("MaterialExtension");
                //}
                Set(ref materialPrice, value);
            }
        }

        public double MaterialExtension
        {
            get
            {
                if (Name.Contains("STAIR METAL"))
                {
                    if (SpecialMetalPricing == 0)
                    {
                        return isStairMetalChecked ? MaterialPrice * Units : 0;
                    }
                    else
                        return isStairMetalChecked ? SpecialMetalPricing * Units : 0;
                }
                else
                {
                    if (SpecialMetalPricing == 0)
                    {
                        return MaterialPrice * Units;
                    }
                    else
                        return SpecialMetalPricing * Units ;
                }
                
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
                //if (specialMetalPricing != value)
                //{
                //    specialMetalPricing = value;
                //    RaisePropertyChanged("SpecialMetalPricing");
                //    RaisePropertyChanged("MaterialExtension");
                //}
                Set(ref specialMetalPricing, value);
                RaisePropertyChanged("MaterialExtension");
            }
        }

        public Metal(string name,string size,double productionRate,double laborRate,double units,double materialPrice,bool isStairMetal,double specialPricing=0)
            :this()
        {
            this.ProductionRate = productionRate;
            this.LaborRate = laborRate;
            if (units!=0)
            {
                this.Units = units;
            }
            if (specialMetalPricing!=0)
            {
                this.SpecialMetalPricing = specialPricing;
            }
            
            this.MaterialPrice = materialPrice;
            this.Name = name;
            this.isStairMetalChecked = isStairMetal;
            this.IsStairMetal = isStairMetal? System.Windows.Visibility.Visible: System.Windows.Visibility.Hidden;
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
                //if (unitPrice!=value)
                //{
                //    unitPrice = value;
                //    RaisePropertyChanged("UnitPrice");
                //    RaisePropertyChanged("LaborExtension");
                //}
                Set(ref unitPrice, value);
                RaisePropertyChanged("LaborExtension");
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
