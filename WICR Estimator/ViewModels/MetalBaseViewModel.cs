using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class MetalBaseViewModel:BaseViewModel
    {
        public Totals MetalTotals;
        private ObservableCollection<Metal> metals;
        private ObservableCollection<MiscMetal> miscMetals;
        private ObservableCollection<AddOnMetal> addOnMetals;

        private ICommand _addRowCommand;
        
        private ICommand _removeCommand;
        private int AddInt = 2;
        protected IList<IList<object>> pWage;
        protected double prevailingWage;
        protected double deductionOnLargeJob;
        protected bool isPrevailingWage;
        protected bool isDiscount;
        protected string vendorName;
        protected IList<IList<object>> metalDetails;
        protected double laborRate;
        protected double riserCount;
        protected double stairWidth;
        protected bool isFlash;
        public MetalBaseViewModel()
        {
            Metals = new ObservableCollection<Metal>();
            MiscMetals = new ObservableCollection<MiscMetal>();
            MetalTotals = new Totals { TabName = "Metal" };

            MetalName = "16oz Copper";
            vendorName = "Chivon";
            riserCount = 30;
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

            return val;
        }
        protected double getMetalMP(int rowN)
        {
            int colN ;
            double val = 0;
            if (rowN == 37||rowN==38)
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
                        colN = 4;
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
        protected double getUnits(int unitNo)
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
                for (int z = 0; z < 6; z++)
                {
                    if (AddOnMetals[z].IsMetalChecked==true)
                    {
                        addOnMetalUnits = addOnMetalUnits + AddOnMetals[z].Units;
                    }
                }
                
                unit = isFlash ? (Metals[0].Units+ Metals[1].Units + Metals[2].Units + Metals[3].Units
                    + addOnMetalUnits) *stairWidth : 0; 
                //double.TryParse(metalDetails[37][0].ToString(), out unit);
            }
            else if (unitNo == 3)
            {
                unit = isFlash ? riserCount : 0;
                //double.TryParse(metalDetails[38][0].ToString(), out unit);
            }
            return unit;
        }

        public virtual void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            OnJobSetupChange(js);
        }

        public void OnJobSetupChange(JobSetup Js)
        {
              if (Js != null)
               {
                   MetalName = Js.MaterialName;
                   isPrevailingWage = Js.IsPrevalingWage;
                   isDiscount = Js.HasDiscount;
                   vendorName = Js.VendorName;
                   stairWidth = Js.StairWidth;
                   isFlash = Js.IsFlashingRequired;
                   riserCount = Js.RiserCount;
                   if (Js.HasSpecialPricing)
                   {
                       ShowSpecialPriceColumn = System.Windows.Visibility.Visible;
                   }
                   else
                       ShowSpecialPriceColumn = System.Windows.Visibility.Hidden;
               }

               var met = GetMetals();
               for (int i = 0; i < Metals.Count; i++)
               {
                   double units = Metals[i].Units;
                   double sp = Metals[i].SpecialMetalPricing;
                   bool isSelected = Metals[i].IsStairMetalChecked;
                   Metals[i] = met[i];
                   if (!Metals[i].Name.Contains("STAIR METAL"))
                   {
                       Metals[i].Units = units;

                   }
                   else
                   {
                       Metals[i].IsStairMetalChecked = isSelected;
                   }

                   Metals[i].SpecialMetalPricing = sp;
               }
               var addOnMet = GetAddOnMetals();
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
            met.Add(new Metal("2 inch CHIVON DRAINS", "", getMetalPR(29), laborRate, 0, getMetalMP(29), false));
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
            misc.Add(new MiscMetal { Name = "OTHER DRAINS TO BE ITEMIZED", Units = 1, UnitPrice = 8, MaterialPrice = 0, IsEditable = true });
            return misc;
        }


        public virtual ObservableCollection<AddOnMetal> GetAddOnMetals()
        {
            ObservableCollection<AddOnMetal> met = new ObservableCollection<AddOnMetal>();
            met.Add(new AddOnMetal("L - METAL / FLASHING", "4X10", getMetalPR(0), laborRate, 0, getMetalMP(0), false));
            met.Add(new AddOnMetal("L - METAL / FLASHING", "4X8", getMetalPR(1), laborRate, 0, getMetalMP(1), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "4X4", getMetalPR(3), laborRate, 0, getMetalMP(3), false));
            met.Add(new AddOnMetal("DRIP EDGE METAL", "3X4", getMetalPR(4), laborRate, 0, getMetalMP(4), false));
            met.Add(new AddOnMetal("STAIR METAL", "4X10", getMetalPR(6), laborRate, getUnits(1), getMetalMP(6), true));
            met.Add(new AddOnMetal("STAIR METAL", "4X8", getMetalPR(7), laborRate, getUnits(1), getMetalMP(7), true));
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
            if (pWage == null)
            {
                
                GSData gsData = DataSerializer.DSInstance.deserializeGoogleData(projectName);
                pWage = gsData.LaborData;
                double.TryParse(gsData.LaborRate[0][0].ToString(), out laborRate);
                double nails;
                double.TryParse(gsData.MetalData[39][1].ToString(), out nails);
                Nails = nails;
                metalDetails = gsData.MetalData;
            }

            double.TryParse(pWage[0][0].ToString(), out prevailingWage);
            double.TryParse(pWage[1][0].ToString(), out deductionOnLargeJob);

        }

        public void CalculateCost(object obj)
        {
            MiscMetals[0].Units = getUnits(2);
            MiscMetals[1].Units = getUnits(3);
            updateLaborCost();
            updateMaterialCost();
            MetalTotals.MaterialExtTotal = TotalMaterialCost;
            MetalTotals.LaborExtTotal = TotalLaborCost;
        }

        public void updateLaborCost()
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

            normCost = Math.Round(normCost + stairCost + addOnMetalCost+ MiscMetals.Select(x => x.LaborExtension).Sum(),2);

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
        protected void updateMaterialCost()
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

                IEnumerable<AddOnMetal> selectedAddOnMetals = addOnMetals.Where(x => x.IsMetalChecked);
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

                TotalMaterialCost = Math.Round((normCost+stairCost) *(1+nl)+ addOnMetalCost+ MiscMetals.Select(x => x.MaterialExtension).Sum() , 2);
            }

        }
        #region  Temporary
        private ICommand fillValues;
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
            MiscMetals[2].Units = 1;
            MiscMetals[2].UnitPrice = 8;
            MiscMetals[2].MaterialPrice = 15;
        }
        #endregion
    }
}
