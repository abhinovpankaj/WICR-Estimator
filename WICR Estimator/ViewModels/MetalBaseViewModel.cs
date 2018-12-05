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
        protected double RiserCount;
        protected double stairWidth;
        protected bool isFlash;
        public MetalBaseViewModel()
        {
            Metals = new ObservableCollection<Metal>();
            MiscMetals = new ObservableCollection<MiscMetal>();
            MetalTotals = new Totals { TabName = "Metal" };

            MetalName = "16oz Copper";
            vendorName = "Chivon";
            RiserCount = 30;
            stairWidth = 4.5;
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
        #endregion
        #region commands
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
                unit = RiserCount * 2.25 * 2;
            }
            else if (unitNo == 1)
                //double.TryParse(metalDetails[3][0].ToString(), out unit);
                unit = RiserCount * stairWidth * 2;
            else if (unitNo == 2)
            {
                unit = isFlash ? (Metals[0].Units+ Metals[1].Units + Metals[2].Units + Metals[3].Units)*stairWidth : 0; 
                //double.TryParse(metalDetails[37][0].ToString(), out unit);
            }
            else if (unitNo == 3)
            {
                unit = isFlash ? RiserCount : 0;
                //double.TryParse(metalDetails[38][0].ToString(), out unit);
            }
            return unit;
        }
        

        public void GetMetalDetailsFromGoogle(string projectName)
        {
            if (pWage == null)
            {
                //pWage = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "E60:E61");
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
            IEnumerable<Metal> stairMetals = Metals.Where(x => x.IsStairMetal == false && x.Name.Contains("STAIR"));
            if (stairMetals != null)
            {
                stairCost = stairMetals.Select(x => x.LaborExtension).Sum();
            }
            ////Calculate Labor Cost
            double misSum = MiscMetals.Select(x => x.LaborExtension).Sum();
            //New Changes for Addon Metals
            IEnumerable<AddOnMetal> selectedAddOnMetals = addOnMetals.Where(x => x.IsMetalChecked);
            if (selectedAddOnMetals.Count()>0)
            {
                addOnMetalCost = selectedAddOnMetals.Select(x => x.LaborExtension).Sum();
            }

            misSum = (Metals.Select(x => x.LaborExtension).Sum() + addOnMetalCost+
            MiscMetals.Select(x => x.LaborExtension).Sum() - stairCost);

            if (isPrevailingWage)
            {
                if (isDiscount)
                {
                    TotalLaborCost = Math.Round(misSum * (1 + prevailingWage + deductionOnLargeJob), 2);
                }
                else
                    TotalLaborCost = Math.Round(misSum * (1 + prevailingWage), 2);

            }
            else
            {
                if (isDiscount)
                {
                    TotalLaborCost = Math.Round(misSum * (1 + deductionOnLargeJob), 2);
                }
                else
                    TotalLaborCost = Math.Round(misSum , 2);
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
            double nl = Nails / 100;
            double addOnMetalCost = 0;
            IEnumerable<Metal> stairMetals = Metals.Where(x => x.IsStairMetal == false && x.Name.Contains("STAIR"));
            if (stairMetals != null)
            {
                stairCost = stairMetals.Select(x => x.MaterialExtension).Sum();
            }
            IEnumerable<AddOnMetal> selectedAddOnMetals = addOnMetals.Where(x => x.IsMetalChecked);
            if (selectedAddOnMetals.Count() > 0)
            {
                addOnMetalCost = selectedAddOnMetals.Select(x => x.MaterialExtension).Sum();
            }
            if (Metals.Count > 0 && MiscMetals.Count > 0)
            {
                double misSum = Metals.Select(x => x.MaterialExtension).Sum() * nl;
                TotalMaterialCost = Math.Round(((Metals.Select(x => x.MaterialExtension).Sum()) + addOnMetalCost+
                MiscMetals.Select(x => x.MaterialExtension).Sum() - stairCost) + misSum, 2);
            }

        }
    }
}
