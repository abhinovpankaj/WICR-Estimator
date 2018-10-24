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

        public MetalBaseViewModel()
        {
            Metals = new ObservableCollection<Metal>();
            MiscMetals = new ObservableCollection<MiscMetal>();
            MetalTotals = new Totals { TabName = "Metal" };

            MetalName = "Copper";
            vendorName = "Chivon";

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

        protected double getMetalMP(int rowN)
        {
            double val = 0;

            if (rowN == 19 || rowN == 20)
            {
                double.TryParse(metalDetails[rowN][2].ToString(), out val);
            }
            else
                double.TryParse(metalDetails[rowN][7].ToString(), out val);
            return val;
        }
        protected double getUnits(int unitNo)
        {
            double unit = 0;
            if (unitNo == 0)
            {
                double.TryParse(metalDetails[2][0].ToString(), out unit);
            }
            else if (unitNo == 1)
                double.TryParse(metalDetails[3][0].ToString(), out unit);
            else if (unitNo == 2)
            {
                double.TryParse(metalDetails[19][0].ToString(), out unit);
            }
            else if (unitNo == 3)
            {
                double.TryParse(metalDetails[20][0].ToString(), out unit);
            }
            return unit;
        }
        protected double getMetalPR(int rowN)
        {
            int colN = 1;
            double val = 0;
            switch (MetalName)
            {
                case "Copper":
                    colN = 1;
                    break;
                case "Steel":
                    colN = 2;
                    break;
                case "Stainless Steel":
                    colN = 3;
                    break;
                default:
                    colN = 1;
                    break;
            }
            if (vendorName != "Chivon")
            {
                colN = colN + 3;
            }
            double.TryParse(metalDetails[rowN][colN].ToString(), out val);
            return val;
        }



    }
}
