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
        public bool IncludeInLaborMinCharge { get; set; }
        public bool IsReadOnly { get; set; }
        public string Operation { get; set; }
        public double VerticalSqft { get; set; }
        public double VerticalProductionRate { get; set; }

        public static event EventHandler OnQTyChanged;
        public static event EventHandler OnUnitChanged;
        public SystemMaterial()
        {
            
        }


        public SystemMaterial(bool ismChecked, string prjestimate, string smunits, double sqft, double coverage,
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
        private double sMSqftH;
        public double SMSqftH
        {
            get { return sMSqftH; }

            set
            {
                if (value != sMSqftH)
                {
                    sMSqftH = value;
                    OnPropertyChanged("SMSqftH");
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
        private double sMSqftV;
        public double SMSqftV
        {
            get { return sMSqftV; }

            set
            {
                if (value != sMSqftV)
                {
                    sMSqftV = value;
                    OnPropertyChanged("SMSqftV");
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
                    OnPropertyChanged("VerticleProductionRate");
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
                    OnPropertyChanged("LaborExtension");
                }
            }
        }
        private double setupMinCharge;
        public double SetupMinCharge
        {
            get { return setupMinCharge; }
            set
            {
                if (value!=setupMinCharge)
                {
                    setupMinCharge = value;
                    OnPropertyChanged("SetupMinCharge");
                }
            }
        }
        private double coverage;
        public double Coverage
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
                    if (Name== "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH)"||
                        Name== "REPAIR AREAS (ENTER SQ FT OF FILL @ 1/4 INCH) UPI 7013 SC BASE COAT"
                        ||Name== "Striping for small cracKs (less than 1/8\")"
                        ||Name== "Route and caulk moving cracks (greater than 1/8\")"
                        ||Name== "SECOND INTERMEDIATE COAT FOR HIGH TRAFFIC")
                    {
                        int unit = 0;
                        Int32.TryParse(value, out unit);
                        if (unit != 0)
                        {
                            IsMaterialChecked = true;
                           
                        }
                        else
                            IsMaterialChecked = false;

                        OnUnitChanged?.Invoke(this.name, EventArgs.Empty);

                    }
                    
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
                    //Paraseal Change
                    if (name== "SUPERSTOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT")
                    {
                        Qty = sqft / coverage;
                        OnPropertyChanged("Qty");
                    }

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
                    FreightExtension = value * Qty;
                    OnPropertyChanged("Weight");
                    OnPropertyChanged("FreightExtension");
                }
            }
        }
        private double extension;
        public double FreightExtension
        {
            get
            {
                return extension;
            }
            set
            {
                if (extension != value)
                {
                    extension = value;
                    OnPropertyChanged("FreightExtension");
                    
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
                    MaterialExtension = Qty * value;
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
                //if (qtysm != value)
                //{

                qtysm = value;
                //if (!IsMaterialEnabled)
                //{
                //    if (qtysm != 0)
                //    {
                //        IsMaterialChecked = true;
                //    }
                //    else
                //        IsMaterialChecked = false;
                //}
                
                OnPropertyChanged("Qty");
                MaterialExtension = value * materialPrice;
                FreightExtension = value * weight;
                
                if (allowHooking(Name))
                {
                    if (qtysm != 0)
                    {
                        IsMaterialChecked = true;
                    }
                    else
                        IsMaterialChecked = false;
                    if (OnQTyChanged != null )
                    {
                        OnQTyChanged(this.Name, EventArgs.Empty);
                    }
                }
                //}
                OnPropertyChanged("MaterialExtension");
                OnPropertyChanged("FreightExtension");
                OnPropertyChanged("LaborExtension");
            }
        }
        private double laborUnitPrice;
        public double LaborUnitPrice
        {
            get
            {
                  return laborUnitPrice;
                
            }

            set
            {
                if (value != laborUnitPrice)
                {
                    if (double.IsNaN(value)||double.IsInfinity(value))
                    {
                        laborUnitPrice = 0;
                        
                    }
                    else
                        laborUnitPrice = value;
                    OnPropertyChanged("LaborUnitPrice");
                }
            }
        }
        private string name;
        public string Name
        { get { return name; }
            set
            {
                if (name!=value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
                
            }
        }
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
                    OnPropertyChanged("SystemMaterials");
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

        private double matExt;
        public double MaterialExtension
        {
            set {
                if (matExt!=value)
                {
                    matExt = value;
                    OnPropertyChanged("MaterialExtension");
                }
            }
            get
            {
                //if (SpecialMaterialPricing != 0)
                //{
                //    matExt = SpecialMaterialPricing * Qty;
                //}
                //else
                //    matExt = MaterialPrice * Qty;
                return matExt;
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
                    if (specialMaterialPricing != 0)
                    {
                        matExt = SpecialMaterialPricing * Qty;
                    }
                    else
                    {
                        matExt = MaterialPrice * Qty;
                        OnQTyChanged(this.Name,null);
                    }
                        
                    
                    OnPropertyChanged("SpecialMaterialPricing");
                    OnPropertyChanged("MaterialExtension");
                }
            }
        }
        private double laborextn;
        public double LaborExtension
        {
            get
            {
                if (laborextn != double.NaN)
                {
                    return laborextn;
                }
                else
                    return 0;
                               
            }       
            set
            {
                if (value!=laborextn)
                {
                    laborextn = value;
                    OnPropertyChanged("LaborExtension");
                }
            }   
        }
        #endregion

        public bool allowHooking(string matName)
        {
            switch (matName)
            {
                case "Bubble Repair(Measure Sq Ft)":
                case "Large Crack Repair":
                case "Extra Stair Nosing Lf":
                case "Extra stair nosing lf":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Plywood 3/4 & Blocking(# Of 4X8 Sheets)":
                case "Stucco Material Remove And Replace (Lf)":
                case "Stucco Material Remove and replace (LF)":
                case "R&R Sealant 1/2 to 3/4 inch control joints (Sonneborn NP-2)":
                case "LARGE CRACK REPAIR":
                case "BUBBLE REPAIR (MEASURE SQ FT)":
                case "System Large cracks Repair":
                case "EXTRA STAIR NOSING":
                case "System bubbled and failed texture repair":
                case "Concrete cracks greater than 1/32 inch (route 1/4 x 1/4) epoxy gel fill, ilica sand, and fiber tape":
                case "Caulk 1/2 to 3/4 inch control joints (SIKA 2C)":
                case "Remove and Replace Expansion joints- backer rod and sealant (SIKA 2C)":
                case "Large cracks with reseal (route, fill with speed bond/sand and spot texture)":
                case "Large cracks with new system (route, fill with speed bond)":
                case "Bubbled and failed textured areas (prep, patch,spot textured with resistite)":
                case "Add for saw cut joints":
                case "Add for removing and replacing concrete (no more than 100 sq ft)":
                case "UNIVERSAL OUTLET":
                case "TOTAL DRAIN 2' x 50' ( In lieu of rock & pipe) \"LINEAR FEET\"":
                case "VISQUINE PROTECTION FOR INCLEMENT WEATHER":
                case "UNIVERSAL OUTLETS":
                case "TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)":
                case "SIDE OUTLET 6\"":
                case "MIRADRAIN HC 1\" DRAIN - PUNCHED 12\" X 100'  (QUICK DRAIN)":
                case "2 COATS VULKEM 350R/951  @ WALL (LF DECK TO WALL)":
                case "ENTER # OF DECKS TO WATER TEST \"NO DAM'S NEEDED\"":
                case "ADD LF FOR DAMMING @ DRIP EDGE":
                case "CALCIUM CHLORIDE TEST (MINIMUM OF 3 FOR EACH JOB)":
                case "PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL WITH SAND BROADCAST":
                case "Add for penetrations  -customer to determine qty":
                    return true;
                default:
                    return false;
            }
        }

        public enum EstimateType
        {
            //["AJ-44A Dressing (Sealer)"]
            //         "Vista Paint Acripoxy",
            // "Lip Color"
        }

        public bool AllEditable { get; set; }
    }
    
    public class OtherItem:BaseViewModel
    {
        public OtherItem()
        { }
        private string name;
        public string Name
        { get { return name; }
            set

            { if (value != name)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }

                    
           }
        }

        public bool IsReadOnly { get; set; }
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
        private double lquantity;
        public double LQuantity
        {
            get { return lquantity; }

            set
            {
                if (value != lquantity)
                {
                    lquantity = value;
                    UpdateUI();
                }
            }

        }
        private double lmaterialPrice;
        public double LMaterialPrice
        {
            get { return lmaterialPrice; }

            set
            {
                if (value != lmaterialPrice)
                {
                    lmaterialPrice = value;
                    UpdateUI();
                }
            }

        }
        public double Extension { get { return Quantity * MaterialPrice; } }
        public double LExtension { get { return LQuantity * LMaterialPrice; } }

        private void UpdateUI()

        {
            OnPropertyChanged("Quantity");
            OnPropertyChanged("MaterialPrice");
            OnPropertyChanged("Extension");
            OnPropertyChanged("LQuantity");
            OnPropertyChanged("LMaterialPrice");
            OnPropertyChanged("LExtension");
        }       
    }

    public class LaborContract : BaseViewModel
    {
        public LaborContract() { }
        private string name;
        public string Name
        {
            get
            { return name; }
            set
            {
                if (name!=value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        
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
