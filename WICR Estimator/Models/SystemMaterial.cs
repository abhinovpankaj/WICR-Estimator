using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    public class SystemMaterial : BaseViewModel
    {

        private bool ismaterialchecked;
        private bool isMaterialEnabled;
        //public EstimateType Type { get; set; }
        public bool IsWWR { get; set; }
        public bool IsCheckboxDependent { get; set; }
        public bool IsReadOnly { get; set; }
        public SystemMaterial()
        {
            IsMaterialEnabled = true;
            
        }


        public SystemMaterial(bool ismChecked, string prjestimate, string smunits, double sqft, int coverage,
            int qty, double materialPrice, double materialextension, int weight, double specialPricing = 0)
            : this()
        {
            this.Coverage = coverage;
            this.SMSqft = sqft;
            this.Weight = weight;
            this.SpecialMaterialPricing = specialPricing;
            this.MaterialPrice = materialPrice;
            this.Qty = qty;
            this.IsMaterialChecked = ismChecked;
            this.SMUnits = smunits;

        }
        #region Properties
        private int coverage;
        public int Coverage
        {
            get
            {
                return coverage;
            }
            set
            {
                if (coverage != value)
                {
                    coverage = value;
                    OnPropertyChanged("Coverage");
                    OnPropertyChanged("Qty");
                }
            }
        }
        private string smunits;
        public string SMUnits
        {
            get
            {
                return smunits;
            }
            set
            {
                if (smunits != value)
                {
                    smunits = value;
                    OnPropertyChanged("SMUnits");

                }
            }
        }
        private double sqft;
        public double SMSqft
        {
            get
            {
                return sqft;
            }
            set
            {
                if (sqft != value)
                {
                    sqft = value;
                    OnPropertyChanged("SMSqft");
                    OnPropertyChanged("Qty");
                }
            }
        }
        private double weight;
        public double Weight
        {
            get
            {
                return weight;
            }
            set
            {
                if (weight != value)
                {
                    weight = value;
                    OnPropertyChanged("Weight");
                    OnPropertyChanged("MaterialExtension");
                }
            }
        }
        private double extension;
        public double FreightExtension
        {
            get
            {
                extension = Weight * Qty;
                return extension;
            }
            set
            {
                if (extension != value)
                {
                    extension = value;
                    OnPropertyChanged("FreightExtension");
                    OnPropertyChanged("MaterialExtension");
                   
                }
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
                if (materialPrice != value)
                {
                    materialPrice = value;
                    OnPropertyChanged("MaterialPrice");
                    OnPropertyChanged("MaterialExtension");
                }
            }
        }
        private Double qtysm;
        public double Qty
        {
            get

            {
                return qtysm;
            }
            set
            {
                if (qtysm != value)
                {
                    qtysm = value;
                    if (qtysm!= 0)
                    {
                        IsMaterialChecked = true;
                    }
                    else
                        IsMaterialChecked = false;
                    OnPropertyChanged("Qty");
                    OnPropertyChanged("MatExtension");
                    OnPropertyChanged("FreightExtension");
                }
            }
        }
        //public double LaborUnitPrice
        //{
        //    get
        //    {
        //        return LaborRate;/// ProductionRate;
        //    }
        //}
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
                    OnPropertyChanged("IsMaterialChecked");
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
                    OnPropertyChanged("IsMaterialEnabled");
                }
            }
        }
        //public bool IsMaterialVisible
        //{
        //    get
        //    {
        //        return isMaterialVisible;
        //    }
        //    set
        //    {
        //        if (value != isMaterialVisible)
        //        {
        //            isMaterialVisible = value;
        //            OnPropertyChanged("IsMaterialVisible");
        //        }
        //    }
        //}
        //private double materialextension;
        public double MaterialExtension
        {
            get
            {
                if (SpecialMaterialPricing == 0)
                {
                    return MaterialPrice * Qty;
                }
                else
                    return SpecialMaterialPricing * Qty;
            }
        }
        private double specialMaterialPricing;
        //private bool isMaterialVisible;

        public double SpecialMaterialPricing
        {
            get
            {
                return specialMaterialPricing;
            }
            set
            {
                if (specialMaterialPricing != value)
                {
                    specialMaterialPricing = value;
                    OnPropertyChanged("specialMaterialPricing");
                    OnPropertyChanged("MaterialExtension");
                }
            }
        }
        #endregion
        public enum EstimateType
        {
            //["AJ-44A Dressing (Sealer)"]
            //         "Vista Paint Acripoxy",
            // "Lip Color"
        }
    }

    public class OtherItem:BaseViewModel
    {
        public string Name { get; set; }

        private double quantity;
        public double Quantity
        {
            get { return quantity; }

            set
            {
                if (value!=quantity)
                {
                    quantity = value;
                    UpdateUI();
                }
            }

        }
        private double materialPrice;
        public double MaterialPrice
        {
            get { return materialPrice; }

            set
            {
                if (value != materialPrice)
                {
                    materialPrice = value;
                    UpdateUI();
                }
            }

        }
        public double Extension { get { return Quantity * MaterialPrice; } }

        private void UpdateUI()

        {
            OnPropertyChanged("Quantity");
            OnPropertyChanged("MaterialPrice");
            OnPropertyChanged("Extension");
        }       
    }

    public class LaborContract : BaseViewModel
    {
        public string Name { get; set; }

        private double unitconlbrcst;
        public double UnitConlbrcst
        {
            get { return unitconlbrcst; }

            set
            {
                if (value != unitconlbrcst)
                {
                    unitconlbrcst = value;
                    UpdateSC();
                }
            }

        }
        private double unitpriceConlbrcst;
        public double UnitPriceConlbrcst
        {
            get { return unitpriceConlbrcst; }

            set
            {
                if (value != unitpriceConlbrcst)
                {
                    unitpriceConlbrcst = value;
                    UpdateSC();
                }
            }
        }
        public double MaterialExtensionConlbrcst { get { return UnitConlbrcst * UnitPriceConlbrcst; } }
        private void UpdateSC()
        {
            OnPropertyChanged("UnitConlbrcst");
            OnPropertyChanged("UnitPriceConlbrcst");
            OnPropertyChanged("MaterialExtensionConlbrcst");
        }
    }
    }
