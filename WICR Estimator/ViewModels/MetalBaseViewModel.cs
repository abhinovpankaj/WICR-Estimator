using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using WICR_Estimator.Models;
using WICR_Estimator.Services;
using WICR_Estimator.DBModels;

namespace WICR_Estimator.ViewModels
{
    [KnownType(typeof(MetalViewModel))]
    [KnownType(typeof(ZeroMetalViewModel))]
    public class MetalBaseViewModel:BaseViewModel
    {

        public Totals MetalTotals;
        private ObservableCollection<Metal> metals;
        private ObservableCollection<MiscMetal> miscMetals;
        private ObservableCollection<AddOnMetal> addOnMetals;

        private ICommand _addRowCommand;
        
        private ICommand _removeCommand;
        private int AddInt = 2;

        public IList<IList<object>> pWage;
        protected double prevailingWage;
        protected double deductionOnLargeJob;
        protected bool isPrevailingWage;
        protected bool isDiscount;
        protected string vendorName;

        private DBData dbData;
        public IList<IList<object>> freightDetails { get; set; }
        public IList<IList<object>> metalDetails { get; set; }
        
        public IList<FreightDB> freightDBDetails { get; set; }
        public IList<MetalDB> metalDBDetails { get; set; }

        public double laborRate { get; set; }
        protected double riserCount;
        protected double stairWidth;
        protected bool isFlash;
        protected double MaterialPerc;
        public MetalBaseViewModel()
        {
            
            Metals = new ObservableCollection<Metal>();
            MiscMetals = new ObservableCollection<MiscMetal>();
            MetalTotals = new Totals { TabName = "Metal" };

            MetalName = "24ga. Galvanized Primed Steel";
            vendorName = "Chivon";
            riserCount = 0;
            stairWidth = 4.5;
            CheckboxCommand = new DelegateCommand(ApplyCheckUnchecks, canApply);
        }

        public MetalBaseViewModel(DBData dbData)
        {
            this.dbData = dbData;
            Metals = new ObservableCollection<Metal>();
            MiscMetals = new ObservableCollection<MiscMetal>();
            MetalTotals = new Totals { TabName = "Metal" };

            MetalName = "24ga. Galvanized Primed Steel";
            vendorName = "Chivon";
            riserCount = 0;
            stairWidth = 4.5;
            CheckboxCommand = new DelegateCommand(ApplyCheckUnchecks, canApply);
        }



        #region Properties
        private System.Windows.Visibility showSpecialPriceColumn = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility ShowSpecialPriceColumn
        {
            get
            {
                return showSpecialPriceColumn;
            }
            set
            {
                if (showSpecialPriceColumn != value)
                {
                    showSpecialPriceColumn = value;
                    OnPropertyChanged("ShowSpecialPriceColumn");
                }
            }
        }
        private string mName;
        public string MetalName
        {
            get { return mName; }
            set
            {
                if (value != mName)
                {
                    mName = value;
                    OnPropertyChanged("MetalName");
                }
            }
        }
        private double nails = 15;
        public double Nails
        {
            get { return nails; }
            set
            {
                if (value != nails)
                {
                    nails = value;
                    OnPropertyChanged("Nails");
                }
            }
        }
        private double totalLaborCost;
        public double TotalLaborCost
        {
            get
            {
                return totalLaborCost;
            }
            set
            {
                if (value != totalLaborCost)
                {
                    totalLaborCost = value;
                    OnPropertyChanged("TotalLaborCost");
                }
            }
        }


        private double totalMaterialCost;
        public double TotalMaterialCost
        {
            get
            {
                return totalMaterialCost;
            }
            set
            {
                if (value != totalMaterialCost)
                {
                    totalMaterialCost = value;
                    OnPropertyChanged("TotalMaterialCost");
                }
            }
        }


        public ObservableCollection<Metal> Metals
        {
            get
            {
                return metals;
            }
            set
            {
                if (metals != value)
                {
                    metals = value;
                    OnPropertyChanged("Metals");
                }
            }
        }

        public ObservableCollection<AddOnMetal> AddOnMetals
        {
            get
            {
                return addOnMetals;
            }
            set
            {
                if (addOnMetals != value)
                {
                    addOnMetals = value;
                    OnPropertyChanged("AddOnMetals");
                }
            }
        }
        public ObservableCollection<MiscMetal> MiscMetals
        {
            get
            {
                return miscMetals;
            }
            set
            {
                if (miscMetals != value)
                {
                    miscMetals = value;
                    OnPropertyChanged("MiscMetals");
                }
            }
        }
        //[XmlIgnore]
        #endregion

        #region Commands
        [IgnoreDataMember]
        public ICommand AddRowCommand
        {
            get
            {
                if (_addRowCommand == null)
                {
                    _addRowCommand = new DelegateCommand(AddRow, CanAddRows);
                }

                return _addRowCommand;
            }
        }
        //[XmlIgnore]
        [IgnoreDataMember]
        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new DelegateCommand(RemoveRow, CanRemoveRow);
                }

                return _removeCommand;
            }
        }
        private DelegateCommand checkboxCommand;
        //[XmlIgnore]
        [IgnoreDataMember]
        public DelegateCommand CheckboxCommand { get { return checkboxCommand; } private set { checkboxCommand = value; } }

        #endregion
        #region commands
        private void ApplyCheckUnchecks(object obj)
        {
            MiscMetals[0].Units = getUnits(2);
        }

        private bool canApply(object obj)
        {
            return true;
        }

        private bool CanRemoveRow(object obj)
        {
            return true;
        }
        private bool CanAddRows(object obj)
        {
            return true;
        }

        private void AddRow(object obj)
        {
            AddInt = AddInt + 1;
            MiscMetals.Add(new MiscMetal { Name = "Misc Metal", Units = 1, MaterialPrice = 0, UnitPrice = 0, IsEditable = true });
        }
        private void RemoveRow(object obj)
        {
            int index = MiscMetals.IndexOf(obj as MiscMetal);
            if (AddInt > 2 && index < MiscMetals.Count)
            {
                MiscMetals.RemoveAt(AddInt);
                AddInt = AddInt - 1;
            }
            //   MiscMetals.Remove(MiscMetals.Last(x => x.CanRemove == true));
        }
        private ICommand _calculateCostCommand;
        //[XmlIgnore]
        [IgnoreDataMember]
        public ICommand CalculateCostCommand
        {
            get
            {
                if (_calculateCostCommand == null)
                {
                    _calculateCostCommand = new DelegateCommand(CalculateCost, CanCalculate);
                }
                return _calculateCostCommand;
            }
        }
        private bool CanCalculate(object obj)
        {
            return true;
        }
        #endregion

        protected double getMetalPR(int rowN)
        {
            double val = 0;
            double.TryParse(metalDetails[rowN][1].ToString(), out val);
            double prPerc = 0;
            double.TryParse(freightDetails[5][0].ToString(), out prPerc);
            return isPrevailingWage ? val * (1 + prPerc) : val;
        }


        protected double getMetalMP(int rowN)
        {
            int colN;
            double val = 0;
            if (rowN == 37 || rowN == 38)
            {
                double.TryParse(metalDetails[rowN][2].ToString(), out val);
            }

            else
            {
                switch (MetalName)
                {
                    case "16oz Copper":
                        colN = 4;
                        break;
                    case "24ga. Galvanized Primed Steel":
                        colN = 2;
                        break;
                    case "26 ga. Type 304 Stainless Steel":
                        colN = 3;
                        break;
                    default:
                        colN = 2;
                        break;
                }
                if (vendorName != "Chivon")
                {
                    colN = colN + 3;
                }
                double.TryParse(metalDetails[rowN][colN].ToString(), out val);
            }

            return val;
        }

        public virtual double getUnits(int unitNo)
        {
            double unit = 0;
            if (unitNo == 0)
            {
                //double.TryParse(metalDetails[2][0].ToString(), out unit);
                unit = riserCount * 2.25 * 2;
            }
            else if (unitNo == 1)
                //double.TryParse(metalDetails[3][0].ToString(), out unit);
                unit = riserCount * stairWidth * 2;
            else if (unitNo == 2)
            {
                double addOnMetalUnits = 0;
                for (int z = 0; z < AddOnMetals.Count; z++)
                {
                    if (AddOnMetals[z].IsMetalChecked == true)
                    {
                        addOnMetalUnits = addOnMetalUnits + AddOnMetals[z].Units;
                    }
                }
                //changed how pins and load are calculated ,removed Chivon as per mail on 12th Sept 2019.
                //unit = isFlash ? (Metals[0].Units+ Metals[1].Units + Metals[2].Units + Metals[3].Units
                //    + addOnMetalUnits) *stairWidth : 0; 
                unit = isFlash ? (Metals.Sum(x => x.Units) +
                   +addOnMetalUnits) * 4 : 0;
                //double.TryParse(metalDetails[37][0].ToString(), out unit);
            }
            else if (unitNo == 3)
            {
                unit = isFlash ? riserCount : 0;
                //double.TryParse(metalDetails[38][0].ToString(), out unit);
            }
            return unit;
        }

        public double getMaterialDiscount(string delay)
        {
            switch (delay)
            {
                case "0-3 Months":
                    return 0;
                case "3-6 Months":
                    return 0.02;
                case "6-12 Months":
                    return 0.04;
                case ">12 Months":
                    return 0.06;
                default:
                    return 0;
            }
        }
        public virtual void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            //if (dbData == null)
            //{
                dbData = js.dbData;
            //}
            OnJobSetupChange(js);
            

        }

        public void OnJobSetupChange(JobSetup Js)
        {
            if (Js==null)
            {
                return;
            }
            if (Js != null)
            {
                MetalName = Js.MaterialName;
                isPrevailingWage = Js.IsPrevalingWage;
                isDiscount = Js.HasDiscount;
                vendorName = Js.VendorName;
                stairWidth = Js.StairWidth;
                isFlash = Js.IsFlashingRequired;
                riserCount = Js.RiserCount;
                MaterialPerc = getMaterialDiscount(Js.ProjectDelayFactor);
                prevailingWage = Js.ActualPrevailingWage==0?0:(Js.ActualPrevailingWage - laborRate) / laborRate;
                //if (Js.HasSpecialPricing)
                //{
                //    ShowSpecialPriceColumn = System.Windows.Visibility.Visible;
                //}
                //else
                //    ShowSpecialPriceColumn = System.Windows.Visibility.Hidden;
            }

            ObservableCollection<Metal> met=new ObservableCollection<Metal>();
            if (Js.ProjectName== "Paraseal LG")
            {
                if (dbData==null)
                {
                    met = GetMetalsLG();
                }
                else
                    met = GetMetalsDB("LG");
            }
            else
            {
                if (dbData == null)
                {
                    met = GetMetals();
                }
                else
                    met = GetMetalsDB();
            }
               

               for (int i = 0; i < Metals.Count; i++)
               {
                   double units = Metals[i].Units;
                   double sp = Metals[i].SpecialMetalPricing;
                   bool isSelected = Metals[i].IsStairMetalChecked;
                
                   Metals[i] = met[i];
                   if (!Metals[i].Name.Contains("STAIR METAL"))
                   {
                       Metals[i].Units = units;
                        Metals[i].IsStairMetalChecked = isSelected;

                    }
                   else
                   {
                       Metals[i].IsStairMetalChecked = isSelected;
                   }

                   Metals[i].SpecialMetalPricing = sp;
               }
            ObservableCollection<AddOnMetal> addOnMet = new ObservableCollection<AddOnMetal>();
            if (Js.ProjectName == "Paraseal LG")
            {
                
                if (dbData==null)
                {
                    addOnMet = GetAddOnMetalsLG();
                }
                else
                    addOnMet = GetAddOnMetalsDB("LG");
            }
            else
            {
                if (dbData==null)
                {
                    addOnMet = GetAddOnMetals();
                }
                else
                    addOnMet = GetAddOnMetalsDB();
            }
                
             
               for (int i = 0; i < AddOnMetals.Count; i++)
               {
                   double units = AddOnMetals[i].Units;
                   double sp = AddOnMetals[i].SpecialMetalPricing;
                   bool ischecked = AddOnMetals[i].IsMetalChecked;
                   AddOnMetals[i] = addOnMet[i];
                   if (!AddOnMetals[i].Name.Contains("STAIR METAL"))
                   {
                       AddOnMetals[i].Units = units;
                   }
                   AddOnMetals[i].IsMetalChecked = ischecked;
                   AddOnMetals[i].SpecialMetalPricing = sp;
               }

            if (Js!=null)
            {
                if (Js.ProjectName == "Multicoat")
                {
                    MiscMetals.Where(x => x.Name == "Nosing for Concrete risers").FirstOrDefault().Units = 0;
                }
                else
                    MiscMetals[1].Units = getUnits(3);
            }
            if (pWage != null)
                double.TryParse(pWage[1][0].ToString(), out deductionOnLargeJob);
            
            double nails=0;
            if (metalDetails!=null)
            {
                double.TryParse(metalDetails[39][1].ToString(), out nails);
            }
            
            
            Nails = nails;

            if (dbData!=null)
            {
                deductionOnLargeJob = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Deduct on Labor for large jobs").Value;
                Nails = dbData.MetalDBData.FirstOrDefault(x => x.MetalName == "Nails, caulk + overage on metal").ProductionRate;
            }
            CalculateCost(null);                      
        }

        public virtual ObservableCollection<Metal> GetMetals()
        {
            ObservableCollection<Metal> met = new ObservableCollection<Metal>();
            met.Add(new Metal("L - METAL / FLASHING", "4X6", getMetalPR(2), laborRate, 0, getMetalMP(2), false));
            met.Add(new Metal("DRIP EDGE METAL", "2X4", getMetalPR(5), laborRate, 0, getMetalMP(5), false));
            met.Add(new Metal("STAIR METAL", "4X6", getMetalPR(8), laborRate, getUnits(0), getMetalMP(8), true));
            met.Add(new Metal("STAIR METAL", "3X3", getMetalPR(9), laborRate, getUnits(1), getMetalMP(9), true));
            met.Add(new Metal("DOOR SADDLES", "(4 ft.)", getMetalPR(16), laborRate, 0, getMetalMP(16), false));
            met.Add(new Metal("DOOR SADDLES", "(6 ft.)", getMetalPR(17), laborRate, 0, getMetalMP(17), false));
            met.Add(new Metal("DOOR SADDLES", "(8 ft.)", getMetalPR(18), laborRate, 0, getMetalMP(18), false));
            met.Add(new Metal("DOOR SADDLES", "(10 ft.)", getMetalPR(19), laborRate, 0, getMetalMP(19), false));
            met.Add(new Metal("INSIDE CORNER", "", getMetalPR(20), laborRate, 0, getMetalMP(20), false));
            met.Add(new Metal("OUTSIDE CORNER", "", getMetalPR(21), laborRate, 0, getMetalMP(21), false));
            met.Add(new Metal("INSIDE EDGE CORNER", "", getMetalPR(22), laborRate, 0, getMetalMP(22), false));
            met.Add(new Metal("OUTSIDE EDGE CORNER", "", getMetalPR(23), laborRate, 0, getMetalMP(23), false));
            met.Add(new Metal("DOOR CORNERS SET OF 2 (L&R)", "", getMetalPR(24), laborRate, 0, getMetalMP(24), false));
            met.Add(new Metal("STRINGER TRANSITION CAP", "", getMetalPR(25), laborRate, 0, getMetalMP(25), false));
            met.Add(new Metal("DRIP TERMINATION", "", getMetalPR(40), laborRate, 0, getMetalMP(40), false));
            met.Add(new Metal("2 inch DRAINS", "", getMetalPR(29), laborRate, 0, getMetalMP(29), false));//changed name ,removed Chivon as per mail on 12th Sept 2019.
            met.Add(new Metal("STANDARD SCUPPER", "4x4x9", getMetalPR(32), laborRate, 0, getMetalMP(32), false));
            met.Add(new Metal("SCUPPER WITH A COLLAR", "4x4x9", getMetalPR(33), laborRate, 0, getMetalMP(33), false));
            met.Add(new Metal("POST COLLARS w/  KERF", "4x4", getMetalPR(36), laborRate, 0, getMetalMP(36), false));
            return met;
        }

        public virtual ObservableCollection<Metal> GetMetalsLG()
        {
            ObservableCollection<Metal> met = new ObservableCollection<Metal>();
            met.Add(new Metal("L - METAL / FLASHING", "4X6", getMetalPR(2), laborRate, 0, getMetalMP(2), false));
            met.Add(new Metal("DRIP EDGE METAL", "2X4", getMetalPR(5), laborRate, 0, getMetalMP(5), false));
            met.Add(new Metal("STAIR METAL ", "4X6", getMetalPR(8), laborRate, 0, getMetalMP(8), true));
            met.Add(new Metal("STAIR METAL ", "3X3", getMetalPR(9), laborRate, 0, getMetalMP(9), true));
            met.Add(new Metal("DOOR SADDLES", "(4 ft.)", getMetalPR(16), laborRate, 0, getMetalMP(16), false));
            met.Add(new Metal("DOOR SADDLES", "(6 ft.)", getMetalPR(17), laborRate, 0, getMetalMP(17), false));
            met.Add(new Metal("DOOR SADDLES", "(8 ft.)", getMetalPR(18), laborRate, 0, getMetalMP(18), false));
            met.Add(new Metal("DOOR SADDLES", "(10 ft.)", getMetalPR(19), laborRate, 0, getMetalMP(19), false));
            met.Add(new Metal("INSIDE CORNER", "", getMetalPR(20), laborRate, 0, getMetalMP(20), false));
            met.Add(new Metal("OUTSIDE CORNER", "", getMetalPR(21), laborRate, 0, getMetalMP(21), false));
            met.Add(new Metal("INSIDE EDGE CORNER", "", getMetalPR(22), laborRate, 0, getMetalMP(22), false));
            met.Add(new Metal("OUTSIDE EDGE CORNER", "", getMetalPR(23), laborRate, 0, getMetalMP(23), false));
            met.Add(new Metal("DOOR CORNERS SET OF 2 (L&R)", "", getMetalPR(24), laborRate, 0, getMetalMP(24), false));
            met.Add(new Metal("STRINGER TRANSITION CAP", "", getMetalPR(25), laborRate, 0, getMetalMP(25), false));
            met.Add(new Metal("DRIP TERMINATION", "", getMetalPR(40), laborRate, 0, getMetalMP(40), false));
            met.Add(new Metal("2 inch DRAINS", "", getMetalPR(29), laborRate, 0, getMetalMP(29), false)); //changed name ,removed Chivon as per mail on 12th Sept 2019.
            met.Add(new Metal("STANDARD SCUPPER", "4x4x9", getMetalPR(32), laborRate, 0, getMetalMP(32), false));
            met.Add(new Metal("SCUPPER WITH A COLLAR", "4x4x9", getMetalPR(33), laborRate, 0, getMetalMP(33), false));
            met.Add(new Metal("POST COLLARS w/  KERF", "4x4", getMetalPR(36), laborRate, 0, getMetalMP(36), false));
            return met;
        }
        public virtual ObservableCollection<MiscMetal> GetMiscMetals()
        {
            ObservableCollection<MiscMetal> misc = new ObservableCollection<MiscMetal>();
            misc.Add(new MiscMetal { Name = "Pins & Loads for metal over concrete", Units = getUnits(2), UnitPrice = getUnitPrice(0), MaterialPrice = getMetalMP(37), IsEditable = false });
            misc.Add(new MiscMetal { Name = "Nosing for Concrete risers", Units = getUnits(3), UnitPrice = getUnitPrice(1), MaterialPrice = getMetalMP(38), IsEditable = false });
            misc.Add(new MiscMetal { Name = "OTHER DRAINS TO BE ITEMIZED", Units = 0, UnitPrice = 0, MaterialPrice = 0, IsEditable = true });
            return misc;
        }


        public virtual ObservableCollection<AddOnMetal> GetAddOnMetals()
        {
            ObservableCollection<AddOnMetal> met = new ObservableCollection<AddOnMetal>();
            met.Add(new AddOnMetal("L - METAL / FLASHING", "4X10", getMetalPR(0), laborRate, 0, getMetalMP(0), false));
            met.Add(new AddOnMetal("L - METAL / FLASHING", "4X8", getMetalPR(1), laborRate, 0, getMetalMP(1), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "4X4", getMetalPR(3), laborRate, 0, getMetalMP(3), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "3X4", getMetalPR(4), laborRate, 0, getMetalMP(4), false));
            met.Add(new AddOnMetal("STAIR METAL", "4X10", getMetalPR(6), laborRate, getUnits(0), getMetalMP(6), true));
            met.Add(new AddOnMetal("STAIR METAL", "4X8", getMetalPR(7), laborRate, getUnits(0), getMetalMP(7), true));
            met.Add(new AddOnMetal("Open End Stair Metal", "(Set of 2 L&R)", getMetalPR(10), laborRate, 0, getMetalMP(10), false));//Added missed metal ,as per mail on 12th Sept 2019.
            met.Add(new AddOnMetal("Door Pan", "10' - 12'", getMetalPR(11), laborRate, 0, getMetalMP(11), false));
            met.Add(new AddOnMetal("Door Pan", "8'", getMetalPR(12), laborRate, 0, getMetalMP(12), false));
            met.Add(new AddOnMetal("Door Pan", "6'", getMetalPR(13), laborRate, 0, getMetalMP(13), false));
            met.Add(new AddOnMetal("Door Pan", "4'", getMetalPR(14), laborRate, 0, getMetalMP(14), false));
            met.Add(new AddOnMetal("Door Pan", "3'", getMetalPR(15), laborRate, 0, getMetalMP(15), false));
            met.Add(new AddOnMetal("CORNER DRIP TERMINATION", "", getMetalPR(26), laborRate, 0, getMetalMP(26), false));
            met.Add(new AddOnMetal("OFFSET DRIP TERMINATION", "", getMetalPR(27), laborRate, 0, getMetalMP(27), false));
            met.Add(new AddOnMetal("SRAIGHT DRIP TERMINATION", "", getMetalPR(28), laborRate, 0, getMetalMP(28), false));
            met.Add(new AddOnMetal("STANDARD SCUPPER", "2x4x9", getMetalPR(30), laborRate, 0, getMetalMP(30), false));
            met.Add(new AddOnMetal("STANDARD SCUPPER", "3x4x9", getMetalPR(31), laborRate, 0, getMetalMP(31), false));
            met.Add(new AddOnMetal("OVERFLOW SCUPPER", "", getMetalPR(34), laborRate, 0, getMetalMP(34), false));
            met.Add(new AddOnMetal("TWO STAGE SCUPPER", "", getMetalPR(35), laborRate, 0, getMetalMP(35), false));
            return met;
        }
        public virtual ObservableCollection<AddOnMetal> GetAddOnMetalsLG()
        {
            ObservableCollection<AddOnMetal> met = new ObservableCollection<AddOnMetal>();
            met.Add(new AddOnMetal("L - METAL / FLASHING", "4X10", getMetalPR(0), laborRate, 0, getMetalMP(0), false));
            met.Add(new AddOnMetal("L - METAL / FLASHING", "4X8", getMetalPR(1), laborRate, 0, getMetalMP(1), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "4X4", getMetalPR(3), laborRate, 0, getMetalMP(3), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "3X4", getMetalPR(4), laborRate, 0, getMetalMP(4), false));
            met.Add(new AddOnMetal("STAIR METAL ", "4X10", getMetalPR(6), laborRate, 0, getMetalMP(6), true));
            met.Add(new AddOnMetal("STAIR METAL ", "4X8", getMetalPR(7), laborRate, 0, getMetalMP(7), true));
            met.Add(new AddOnMetal("Door Pan", "10' - 12'", getMetalPR(11), laborRate, 0, getMetalMP(11), false));
            met.Add(new AddOnMetal("Door Pan", "8'", getMetalPR(12), laborRate, 0, getMetalMP(12), false));
            met.Add(new AddOnMetal("Door Pan", "6'", getMetalPR(13), laborRate, 0, getMetalMP(13), false));
            met.Add(new AddOnMetal("Door Pan", "4'", getMetalPR(14), laborRate, 0, getMetalMP(14), false));
            met.Add(new AddOnMetal("Door Pan", "3'", getMetalPR(15), laborRate, 0, getMetalMP(15), false));
            met.Add(new AddOnMetal("CORNER DRIP TERMINATION", "", getMetalPR(26), laborRate, 0, getMetalMP(26), false));
            met.Add(new AddOnMetal("OFFSET DRIP TERMINATION", "", getMetalPR(27), laborRate, 0, getMetalMP(27), false));
            met.Add(new AddOnMetal("SRAIGHT DRIP TERMINATION", "", getMetalPR(28), laborRate, 0, getMetalMP(28), false));
            met.Add(new AddOnMetal("STANDARD SCUPPER", "2x4x9", getMetalPR(30), laborRate, 0, getMetalMP(30), false));
            met.Add(new AddOnMetal("STANDARD SCUPPER", "3x4x9", getMetalPR(31), laborRate, 0, getMetalMP(31), false));
            met.Add(new AddOnMetal("OVERFLOW SCUPPER", "", getMetalPR(34), laborRate, 0, getMetalMP(34), false));
            met.Add(new AddOnMetal("TWO STAGE SCUPPER", "", getMetalPR(35), laborRate, 0, getMetalMP(35), false));
            return met;
        }
        public void GetMetalDetailsFromGoogle(string projectName)
        {
            if (projectName.Contains('.'))
            {
                projectName = projectName.Split('.')[0];
            }

            if (pWage == null)
            {
                GSData gsData = DataSerializer.DSInstance.deserializeGoogleData(projectName);
                pWage = gsData.LaborData;
                //pWage = gsData.LaborData.ToArray<object>();
                double lb;
                double.TryParse(gsData.LaborRate[0][0].ToString(), out lb);
                laborRate = lb;
                double nails;
                double.TryParse(gsData.MetalData[39][1].ToString(), out nails);
                Nails = nails;
                metalDetails = gsData.MetalData;
                freightDetails = gsData.FreightData;
            }
            //IList <object> data = pWage[0] as IList<object>;

            //double.TryParse(data.ToArray<object>()[0].ToString(), out prevailingWage);
            //data = pWage[1] as IList<object>;
            //double.TryParse(data.ToArray<object>()[0].ToString(), out deductionOnLargeJob);

            //double.TryParse(pWage[0][0].ToString(), out prevailingWage);
            if (pWage != null)
                double.TryParse(pWage[1][0].ToString(), out deductionOnLargeJob);

        }

        public void CalculateCost(object obj)
        {
            MiscMetals[0].Units = getUnits(2);
            
            updateLaborCost();
            updateMaterialCost();
            MetalTotals.MaterialExtTotal = TotalMaterialCost*(1+MaterialPerc);
            MetalTotals.LaborExtTotal = TotalLaborCost;
        }

        public virtual void updateLaborCost()
        {
            double stairCost = 0;
            double addOnMetalCost = 0;
            double normCost = 0;
            IEnumerable<Metal> stairMetals = Metals.Where(x => x.IsStairMetalChecked == true);
            if (stairMetals.Count()>0)
            {
                stairCost = stairMetals.Select(x => x.LaborExtension).Sum();
            }
            
            IEnumerable<Metal> normMetals = Metals.Where(x => x.Name.Contains("STAIR") == false);
            if (normMetals.Count() > 0)
            {
                normCost = normMetals.Select(x => x.LaborExtension).Sum();
            }
            //New Changes for Addon Metals
            IEnumerable<AddOnMetal> selectedAddOnMetals = addOnMetals.Where(x => x.IsMetalChecked);
            if (selectedAddOnMetals.Count()>0)
            {
                addOnMetalCost = selectedAddOnMetals.Select(x => x.LaborExtension).Sum();
            }
            double misMetalCost = 0;
            misMetalCost = MiscMetals.Select(x => x.LaborExtension).Sum();

            normCost = Math.Round(normCost + stairCost + addOnMetalCost+ misMetalCost, 2);

            if (isPrevailingWage)
            {
                if (isDiscount)
                {
                    TotalLaborCost = Math.Round(normCost * (1 + prevailingWage + deductionOnLargeJob), 2);
                }
                else
                    TotalLaborCost = Math.Round(normCost * (1 + prevailingWage), 2);

            }
            else
            {
                if (isDiscount)
                {
                    TotalLaborCost = Math.Round(normCost * (1 + deductionOnLargeJob), 2);
                }
                else
                    TotalLaborCost = Math.Round(normCost, 2);
            }
    }

        protected double getUnitPrice(int unit)
        {
            double val = 0;
            if (unit == 0)
            {
                double.TryParse(metalDetails[37][3].ToString(), out val);
            }
            else
                double.TryParse(metalDetails[38][3].ToString(), out val);

            return val;
        }
        protected virtual void updateMaterialCost()
        {
            double stairCost = 0;
            double normCost = 0;
            double nl = Nails / 100;
            double addOnMetalCost = 0;
            if (Metals.Count > 0 && MiscMetals.Count > 0)
            {
                IEnumerable<Metal> stairMetals = Metals.Where(x => x.IsStairMetalChecked==true);
                if (stairMetals.Count()>0)
                {
                    stairCost = stairMetals.Select(x => x.MaterialExtension).Sum();
                }
                IEnumerable<AddOnMetal> selectedAddOnMetals = addOnMetals.Where(x => x.IsMetalChecked==true);
                if (selectedAddOnMetals.Count() > 0)
                {
                    addOnMetalCost = selectedAddOnMetals.Select(x => x.MaterialExtension).Sum();
                }

                IEnumerable<Metal> normMetals = Metals.Where(x => x.Name.Contains("STAIR")==false);
                if (normMetals.Count() > 0)
                {
                    normCost = normMetals.Select(x => x.MaterialExtension).Sum();
                }
            
                //double misSum = Metals.Select(x => x.MaterialExtension).Sum() * nl;

                TotalMaterialCost = Math.Round((normCost+stairCost) *(1+nl)+ addOnMetalCost*(1+nl)+ MiscMetals.Select(x => x.MaterialExtension).Sum() , 2);
            }

        }

        #region DBData

        protected double getUnitPrice(string metalName)
        {
            return (double)metalDBDetails.FirstOrDefault(x => x.MetalName == metalName).ProductionRate;
        }

        public void GetMetalDetailsFromDB(string projectName)
        {
            
            if (projectName.Contains('.'))
            {
                projectName = projectName.Split('.')[0];
            }

            if (dbData == null)
            {
                dbData = DataSerializerService.DSInstance.deserializeDbData(projectName);
            }

            //dbData = DataSerializerService.DSInstance.dbData;
            if (dbData.LaborDBData.Count!=0)
            {
                deductionOnLargeJob = dbData.LaborDBData.First(x => x.Name == "Deduct on Labor for large jobs").Value;
            }
               
                //pWage = gsData.LaborData.ToArray<object>();

                laborRate = dbData.FreightDBData.First(x=>x.FreightID==8).FactorValue;
                
                Nails = dbData.MetalDBData.FirstOrDefault(x=>x.MetalName== "Nails, caulk + overage on metal").ProductionRate; //production rate for Nails, caulk + overage on metal
                metalDBDetails = dbData.MetalDBData;
                freightDBDetails = dbData.FreightDBData;
                     
        }

        protected double getMetalPR(string metalName)
        {
            double val = dbData.MetalDBData.FirstOrDefault(x => x.MetalName == metalName).ProductionRate;
            double prPerc = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "SlopeProdRate").FactorValue;

            return isPrevailingWage ? val * (1 + prPerc) : val;
        }
        protected double getMetalMP(string metalName)
        {
            return dbData.MetalDBData.Where(x => x.MetalName == metalName).Where(x => x.MetalType == MetalName).Where(x => x.Vendor == vendorName)
                .Select(s => s.MetalPrice).First();
        }

        public virtual ObservableCollection<Metal> GetMetalsDB(string type="")
        {
            ObservableCollection<Metal> met = new ObservableCollection<Metal>();
            met.Add(new Metal("L - METAL / FLASHING", "4X6", getMetalPR("L-Metal/Flashing 4'X6''"), laborRate, 0, getMetalMP("L-Metal/Flashing 4'X6''"), false));
            met.Add(new Metal("DRIP EDGE METAL", "2X4", getMetalPR("Drip Edge Metal 2\" x 4\""), laborRate, 0, getMetalMP("Drip Edge Metal 2\" x 4\""), false));
            if (type=="")
            {
                met.Add(new Metal("STAIR METAL", "4X6", getMetalPR("Stair Metal 4X6"), laborRate, getUnits(0), getMetalMP("Stair Metal 4X6"), true));
                met.Add(new Metal("STAIR METAL", "3X3", getMetalPR("Stair Metal 3X3"), laborRate, getUnits(1), getMetalMP("Stair Metal 3X3"), true));
            }
            else
            {
                met.Add(new Metal("STAIR METAL ", "4X6", getMetalPR("Stair Metal 4X6"), laborRate, 0, getMetalMP("Stair Metal 4X6"), true));
                met.Add(new Metal("STAIR METAL ", "3X3", getMetalPR("Stair Metal 3X3"), laborRate, 0, getMetalMP("Stair Metal 3X3"), true));
            }
            
            met.Add(new Metal("DOOR SADDLES", "(4 ft.)", getMetalPR("Door Saddles (4 Ft.)"), laborRate, 0, getMetalMP("Door Saddles (4 Ft.)"), false));
            met.Add(new Metal("DOOR SADDLES", "(6 ft.)", getMetalPR("Door Saddles (6Ft)"), laborRate, 0, getMetalMP("Door Saddles (6Ft)"), false));
            met.Add(new Metal("DOOR SADDLES", "(8 ft.)", getMetalPR("Door Saddles (8Ft)"), laborRate, 0, getMetalMP("Door Saddles (8Ft)"), false));
            met.Add(new Metal("DOOR SADDLES", "(10 ft.)", getMetalPR("Door Saddles (10 Ft)"), laborRate, 0, getMetalMP("Door Saddles (10 Ft)"), false));
            met.Add(new Metal("INSIDE CORNER", "", getMetalPR("Inside Corner"), laborRate, 0, getMetalMP("Inside Corner"), false));
            met.Add(new Metal("OUTSIDE CORNER", "", getMetalPR("Outside Corner"), laborRate, 0, getMetalMP("Outside Corner"), false));
            met.Add(new Metal("INSIDE EDGE CORNER", "", getMetalPR("Inside Edge Corner"), laborRate, 0, getMetalMP("Inside Edge Corner"), false));
            met.Add(new Metal("OUTSIDE EDGE CORNER", "", getMetalPR("Outside Edge Corner"), laborRate, 0, getMetalMP("Outside Edge Corner"), false));
            met.Add(new Metal("DOOR CORNERS SET OF 2 (L&R)", "", getMetalPR("Door Corners Set Of 2 (L&R)"), laborRate, 0, getMetalMP("Door Corners Set Of 2 (L&R)"), false));
            met.Add(new Metal("STRINGER TRANSITION CAP", "", getMetalPR("Stringer Transition Cap"), laborRate, 0, getMetalMP("Stringer Transition Cap"), false));
            met.Add(new Metal("DRIP TERMINATION", "", getMetalPR("Drip Termination"), laborRate, 0, getMetalMP("Drip Termination"), false));
            met.Add(new Metal("2 inch DRAINS", "", getMetalPR("2 Inch Chivon Drains"), laborRate, 0, getMetalMP("2 Inch Chivon Drains"), false));//changed name ,removed Chivon as per mail on 12th Sept 2019.
            met.Add(new Metal("STANDARD SCUPPER", "4x4x9", getMetalPR("4\" x 4\" x 9\" standard scupper"), laborRate, 0, getMetalMP("4\" x 4\" x 9\" standard scupper"), false));
            met.Add(new Metal("SCUPPER WITH A COLLAR", "4x4x9", getMetalPR("Scupper With A Collar 4X4X9"), laborRate, 0, getMetalMP("Scupper With A Collar 4X4X9"), false));
            met.Add(new Metal("POST COLLARS w/  KERF", "4x4", getMetalPR("Post Collars 4X4 W/ Kerf"), laborRate, 0, getMetalMP("Post Collars 4X4 W/ Kerf"), false));

            return met;
        }
        public virtual ObservableCollection<AddOnMetal> GetAddOnMetalsDB(string type="")
        {
            ObservableCollection<AddOnMetal> met = new ObservableCollection<AddOnMetal>();
            met.Add(new AddOnMetal("L - METAL / FLASHING", "4X10", getMetalPR("L-Metal/Flashing 4'X10''"), laborRate, 0, getMetalMP("L-Metal/Flashing 4'X10''"), false));
            met.Add(new AddOnMetal("L - METAL / FLASHING", "4X8", getMetalPR("L-Metal/Flashing 4'X8''"), laborRate, 0, getMetalMP("L-Metal/Flashing 4'X8''"), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "4X4", getMetalPR("Drip Edge Metal 4\" x 4\""), laborRate, 0, getMetalMP("Drip Edge Metal 4\" x 4\""), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "3X4", getMetalPR("Drip Edge Metal 3\" x 4\""), laborRate, 0, getMetalMP("Drip Edge Metal 3\" x 4\""), false));
            if (type=="")
            {
                met.Add(new AddOnMetal("STAIR METAL", "4X10", getMetalPR("Stair Metal 4\" x 10\""), laborRate, getUnits(0), getMetalMP("Stair Metal 4\" x 10\""), true));
                met.Add(new AddOnMetal("STAIR METAL", "4X8", getMetalPR("Stair Metal 4\" x 8\""), laborRate, getUnits(0), getMetalMP("Stair Metal 4\" x 8\""), true));
            }
            else
            {
                met.Add(new AddOnMetal("STAIR METAL ", "4X10", getMetalPR("Stair Metal 4\" x 10\""), laborRate, 0, getMetalMP("Stair Metal 4\" x 10\""), true));
                met.Add(new AddOnMetal("STAIR METAL ", "4X8", getMetalPR("Stair Metal 4\" x 8\""), laborRate, 0, getMetalMP("Stair Metal 4\" x 8\""), true));
            }
            
            if(type=="")
                met.Add(new AddOnMetal("Open End Stair Metal", "(Set of 2 L&R)", getMetalPR("Open End Stair Metal (Set of 2 L&R)"), laborRate, 0, getMetalMP("Open End Stair Metal (Set of 2 L&R)"), false));//Added missed metal ,as per mail on 12th Sept 2019.
            
            met.Add(new AddOnMetal("Door Pan", "10' - 12'", getMetalPR("Door Pan 10' - 12'"), laborRate, 0, getMetalMP("Door Pan 10' - 12'"), false));
            met.Add(new AddOnMetal("Door Pan", "8'", getMetalPR("Door Pan 8'"), laborRate, 0, getMetalMP("Door Pan 8'"), false));
            met.Add(new AddOnMetal("Door Pan", "6'", getMetalPR("Door Pan 6'"), laborRate, 0, getMetalMP("Door Pan 6'"), false));
            met.Add(new AddOnMetal("Door Pan", "4'", getMetalPR("Door Pan 4'"), laborRate, 0, getMetalMP("Door Pan 4'"), false));
            met.Add(new AddOnMetal("Door Pan", "3'", getMetalPR("Door Pan 3'"), laborRate, 0, getMetalMP("Door Pan 3'"), false));
            met.Add(new AddOnMetal("CORNER DRIP TERMINATION", "", getMetalPR("Corner Drip Termination"), laborRate, 0, getMetalMP("Corner Drip Termination"), false));
            met.Add(new AddOnMetal("OFFSET DRIP TERMINATION", "", getMetalPR("Offset Drip Termination"), laborRate, 0, getMetalMP("Offset Drip Termination"), false));
            met.Add(new AddOnMetal("STRAIGHT DRIP TERMINATION", "", getMetalPR("Straight Drip Termination"), laborRate, 0, getMetalMP("Straight Drip Termination"), false));
            met.Add(new AddOnMetal("STANDARD SCUPPER", "2x4x9", getMetalPR("2\" x 4\" x 9\" Standard Scupper"), laborRate, 0, getMetalMP("2\" x 4\" x 9\" Standard Scupper"), false));
            met.Add(new AddOnMetal("STANDARD SCUPPER", "3x4x9", getMetalPR("3\" x 4\" x 9\" standard scupper"), laborRate, 0, getMetalMP("3\" x 4\" x 9\" standard scupper"), false));
            met.Add(new AddOnMetal("OVERFLOW SCUPPER", "", getMetalPR("Overflow Scupper"), laborRate, 0, getMetalMP("Overflow Scupper"), false));
            met.Add(new AddOnMetal("TWO STAGE SCUPPER", "", getMetalPR("Two Stage Scupper"), laborRate, 0, getMetalMP("Two Stage Scupper"), false));
            return met;
        }

        public virtual ObservableCollection<MiscMetal> GetMiscMetalsDB()
        {
            ObservableCollection<MiscMetal> misc = new ObservableCollection<MiscMetal>();
            misc.Add(new MiscMetal { Name = "Pins & Loads for metal over concrete", Units = getUnits(2), 
UnitPrice = getUnitPrice("Pins & Loads for metal over concrete"), MaterialPrice = getMetalMP("Pins & Loads for metal over concrete"), IsEditable = false });
            misc.Add(new MiscMetal { Name = "Nosing for Concrete risers", Units = getUnits(3), UnitPrice = getUnitPrice("Nosing for Concrete risers"), MaterialPrice = getMetalMP("Nosing for Concrete risers"), IsEditable = false });
            misc.Add(new MiscMetal { Name = "OTHER DRAINS TO BE ITEMIZED", Units = 0, UnitPrice = 0, MaterialPrice = 0, IsEditable = true });
            return misc;
        }
        #endregion


        #region  Temporary
        private ICommand fillValues;
       

        //[XmlIgnore]
        [IgnoreDataMember]
        public ICommand FillValues
        {
            get
            {
                if (fillValues == null)
                {
                    fillValues = new DelegateCommand(AutoFill, CanAutoFill);
                }

                return fillValues;
            }
        }

        
        private bool CanAutoFill(object obj)
        {
            return true;
        }

        private void AutoFill(object obj)
        {
            int i = 0;
            foreach (Metal item in Metals)
            {
                if (item.Name.Contains("STAIR"))
                {

                }
                
                else
                {
                    item.Units = i + 1;
                    i++;
                }
                

            }
            i = 0;
            foreach (AddOnMetal item in AddOnMetals)
            {
                item.IsMetalChecked = true;
                if (item.Name.Contains("STAIR"))
                {

                }

                else
                {
                    item.Units = i + 1;
                    i++;
                }


            }
            MiscMetals[2].Units = 1;
            MiscMetals[2].UnitPrice = 8;
            MiscMetals[2].MaterialPrice = 15;
            CalculateCost(null);
        }
        #endregion
    }
}
