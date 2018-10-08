using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WICR_Estimator.Models;
using WICR_Estimator.Views;

namespace WICR_Estimator.ViewModels
{
    public class MetalViewModel:BaseViewModel
    {
        private ObservableCollection<Metal> metals;
        private ObservableCollection<MiscMetal> miscMetals;
        private ICommand _addRowCommand;
        private ICommand _calculateCostCommand;
        private ICommand _removeCommand;
        private int AddInt = 2;
        public MetalViewModel()
        {
            Metals = new ObservableCollection<Metal>();
            MiscMetals = new ObservableCollection<MiscMetal>();
            Metals=GetMetals();
            MiscMetals=GetMiscMetals();
            mName = "Copper";
            JobSetup.OnJobSetupChange += JobSetup_OnJobSetupChange;
            updateLaborCost();
            updateMaterialCost();
        }

        private void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js!=null)
            {
                MetalName = Js.MaterialName;
            }
            updateLaborCost();
            updateMaterialCost();

        }
        #region Properties
        private string mName;
        public string MetalName
        {
            get { return mName; }
            set
            {
                if (value!=mName)
                {
                    mName = value;
                    OnPropertyChanged("MetalName");
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
                if (value!=totalLaborCost)
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

        private bool CanRemoveRow(object obj)
        {
            return true;
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

        private void CalculateCost(object obj)
        {
            updateLaborCost();
            updateMaterialCost();
        }
        #endregion
        private bool CanAddRows(object obj)
        {
            return true;
        }

        private void AddRow(object obj)
        {
            AddInt = AddInt + 1;
            MiscMetals.Add(new MiscMetal { Name = "Misc Metal", Units = 1, MaterialPrice = 0, UnitPrice = 0, IsReadOnly = false });          
        }

        public ObservableCollection<Metal> GetMetals()
        {
            ObservableCollection<Metal> met = new ObservableCollection<Metal>();
            met.Add(new Metal("L - METAL / FLASHING", 56, 23.43, 1, 0.912, false));
            met.Add(new Metal("DRIP EDGE METAL", 56, 23.43, 1, 0.564, false));
            met.Add(new Metal("STAIR METAL 4X6", 80, 23.43, 135, 0.912, true));
            met.Add(new Metal("STAIR METAL 3X3", 80, 23.43, 240, 0.54, true));
            met.Add(new Metal("DOOR SADDLES (4 ft.)", 2, 23.43, 1, 30.216, false));
            met.Add(new Metal("DOOR SADDLES (6 ft.)", 2, 23.43, 2, 30.852, false));
            met.Add(new Metal("DOOR SADDLES (8 ft.)", 2, 23.43, 3, 33.072, false));
            met.Add(new Metal("DOOR SADDLES (10 ft.)", 2, 23.43, 6, 50.688, false));
            met.Add(new Metal("INSIDE CORNER", 12, 23.43, 1, 7.5, false));
            met.Add(new Metal("OUTSIDE CORNER", 12, 23.43, 1, 7.5, false));
            met.Add(new Metal("INSIDE EDGE CORNER", 12, 23.43, 1, 5.664, false));
            met.Add(new Metal("OUTSIDE EDGE CORNER", 12, 23.43, 1, 5.028, false));
            met.Add(new Metal("DOOR CORNERS SET OF 2 (L&R)", 12, 23.43, 1, 11.388, false));
            met.Add(new Metal("STRINGER TRANSITION CAP", 12, 23.43, 1, 9.348, false));
            met.Add(new Metal("DRIP TERMINATION", 12, 23.43, 1, 12.468, false));
            met.Add(new Metal("2 inch CHIVON DRAINS", 4, 23.43, 1, 25.44, false));
            met.Add(new Metal("STANDARD SCUPPER 4x4x9", 7, 23.43, 1, 30.852, false));
            met.Add(new Metal("SCUPPER WITH A COLLAR 4x4x9", 4, 23.43, 1, 38.796, false));
            met.Add(new Metal("POST COLLARS 4x4 w/  KERF", 7, 23.43, 1, 12.084, false));
            return met;
        }
        public ObservableCollection<MiscMetal> GetMiscMetals()
        {
            ObservableCollection<MiscMetal> misc = new ObservableCollection<MiscMetal>();
            misc.Add(new MiscMetal { Name = "Pins & Loads for metal over concrete", Units = 1512, UnitPrice = 0, MaterialPrice = 0.25, IsReadOnly = true });
            misc.Add(new MiscMetal { Name = "Nosing for Concrete risers", Units = 30, UnitPrice = 4.69, MaterialPrice = 0, IsReadOnly = true });
            misc.Add(new MiscMetal { Name = "OTHER DRAINS TO BE ITEMIZED", Units = 1, UnitPrice = 8, MaterialPrice = 15, IsReadOnly = false });
            return misc;
        }

        private void updateMaterialCost()
        {
            double stairCost = 0;
            IEnumerable<Metal> stairMetals = metals.Where(x => x.IsStairMetal == false && x.Name.Contains("STAIR"));
            if (stairMetals != null)
            {
                stairCost = stairMetals.Select(x => x.MaterialExtension).Sum();
            }
            if (metals.Count>0 && miscMetals.Count>0)
            {
                double misSum = miscMetals.Select(x => x.MaterialExtension).Sum();
                TotalMaterialCost = metals.Select(x => x.MaterialExtension).Sum() +
                miscMetals.Select(x => x.MaterialExtension).Sum() - stairCost;
            }
            
        }
        private void updateLaborCost()
        {

            double stairCost = 0;
            IEnumerable<Metal> stairMetals = metals.Where(x => x.IsStairMetal == false && x.Name.Contains("STAIR"));
            if (stairMetals != null)
            {
                stairCost = stairMetals.Select(x => x.LaborExtension).Sum();
            }
            if (metals.Count>0 && miscMetals.Count>0)
            {
                double misSum = miscMetals.Select(x => x.LaborExtension).Sum();
                TotalLaborCost = metals.Select(x => x.LaborExtension).Sum() +
                miscMetals.Select(x => x.LaborExtension).Sum() - stairCost;
            }
            
        }
    }
}
