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
        private IList<IList<object>> pWage;
        private double prevailingWage;
        private double deductionOnLargeJob;
        private bool isPrevailingWage;
        private bool isDiscount;
        private string vendorName;
        private IList<IList<object>> metalDetails;
        private double laborRate;
        public MetalViewModel()
        {
            Metals = new ObservableCollection<Metal>();
            MiscMetals = new ObservableCollection<MiscMetal>();
            MetalViewModelAsync();
            Metals =GetMetals();
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
                isPrevailingWage = Js.IsPrevalingWage;
                isDiscount = Js.HasDiscount;
                vendorName = Js.VendorName;
            }
            updateLaborCost();
            updateMaterialCost();

        }
        private void MetalViewModelAsync()
        {
            
            if (pWage == null)
            {
                //pWage = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "E60:E61");
                GSData gsData = DataSerializer.DSInstance.deserializeGoogleData();
                pWage = gsData.LaborData;
                double.TryParse(gsData.LaborRate[0][0].ToString(),out laborRate);
                Nails= double.Parse(gsData.MetalData[21][1].ToString());
                metalDetails = gsData.MetalData;
            }
            
            double.TryParse(pWage[0][0].ToString(), out prevailingWage);    
            double.TryParse(pWage[1][0].ToString(), out deductionOnLargeJob);

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
        private double nails=15;
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
            met.Add(new Metal("L - METAL / FLASHING",getMetalPR(0), laborRate,1, getMetalMP(0), false));
            met.Add(new Metal("DRIP EDGE METAL", getMetalPR(1), laborRate, 1, getMetalMP(1), false));
            met.Add(new Metal("STAIR METAL 4X6", getMetalPR(2), laborRate, getUnits(0), getMetalMP(2), true));
            met.Add(new Metal("STAIR METAL 3X3", getMetalPR(3), laborRate, getUnits(1), getMetalMP(3), true));
            met.Add(new Metal("DOOR SADDLES (4 ft.)", getMetalPR(4), laborRate, 1, getMetalMP(4), false));
            met.Add(new Metal("DOOR SADDLES (6 ft.)", getMetalPR(5) ,laborRate, 2, getMetalMP(5), false));
            met.Add(new Metal("DOOR SADDLES (8 ft.)", getMetalPR(6), laborRate, 3, getMetalMP(6), false));
            met.Add(new Metal("DOOR SADDLES (10 ft.)", getMetalPR(7), laborRate, 6, getMetalMP(7), false));
            met.Add(new Metal("INSIDE CORNER", getMetalPR(8), laborRate, 1, getMetalMP(8), false));
            met.Add(new Metal("OUTSIDE CORNER", getMetalPR(9), laborRate, 1, getMetalMP(9), false));
            met.Add(new Metal("INSIDE EDGE CORNER", getMetalPR(10), laborRate, 1, getMetalMP(10), false));
            met.Add(new Metal("OUTSIDE EDGE CORNER", getMetalPR(11), laborRate, 1, getMetalMP(11), false));
            met.Add(new Metal("DOOR CORNERS SET OF 2 (L&R)", getMetalPR(12), laborRate, 1, getMetalMP(12), false));
            met.Add(new Metal("STRINGER TRANSITION CAP", getMetalPR(13), laborRate, 1, getMetalMP(13), false));
            met.Add(new Metal("DRIP TERMINATION", getMetalPR(14), laborRate, 1, getMetalMP(14), false));
            met.Add(new Metal("2 inch CHIVON DRAINS", getMetalPR(15), laborRate, 1, getMetalMP(15), false));
            met.Add(new Metal("STANDARD SCUPPER 4x4x9", getMetalPR(16), laborRate, 1, getMetalMP(16), false));
            met.Add(new Metal("SCUPPER WITH A COLLAR 4x4x9", getMetalPR(17), laborRate, 1, getMetalMP(17), false));
            met.Add(new Metal("POST COLLARS 4x4 w/  KERF", getMetalPR(18), laborRate, 1, getMetalMP(18), false));       
            return met;
        }
        private double getMetalMP(int rowN)
        {
            double val=0;
            
            if (rowN==19 ||rowN==20)
            {
                double.TryParse(metalDetails[rowN][2].ToString(), out val);
            }
            else
                double.TryParse(metalDetails[rowN][7].ToString(), out val);
            return val;
        }
        private double getUnits(int unitNo)
        {
            double unit=0;
            if (unitNo==0)
            {
                double.TryParse(metalDetails[2][0].ToString(), out unit);
            }
            else if(unitNo==1)
                double.TryParse(metalDetails[3][0].ToString(), out unit);
            else if (unitNo==2)
            {
                double.TryParse(metalDetails[19][0].ToString(), out unit);
            }
            else if (unitNo == 3)
            {
                double.TryParse(metalDetails[20][0].ToString(), out unit);
            }
            return unit;
        }
        private double getMetalPR(int rowN)
        {
            int colN=1;
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
            double.TryParse(metalDetails[rowN][colN].ToString(),out val);
            return val;
        }

        public ObservableCollection<MiscMetal> GetMiscMetals()
        {
            ObservableCollection<MiscMetal> misc = new ObservableCollection<MiscMetal>();
            misc.Add(new MiscMetal { Name = "Pins & Loads for metal over concrete", Units = getUnits(2), UnitPrice = getUnitPrice(0), MaterialPrice = getMetalMP(19), IsReadOnly = true });
            misc.Add(new MiscMetal { Name = "Nosing for Concrete risers", Units = getUnits(3), UnitPrice = getUnitPrice(0), MaterialPrice = getMetalMP(20), IsReadOnly = true });
            misc.Add(new MiscMetal { Name = "OTHER DRAINS TO BE ITEMIZED", Units = 1, UnitPrice = 8, MaterialPrice = 15, IsReadOnly = false });
            return misc;
        }
        private double getUnitPrice(int unit)
        {
            double val = 0;
            if (unit==0)
            {
                double.TryParse(metalDetails[19][3].ToString(), out val);
            }
            else
                double.TryParse(metalDetails[20][3].ToString(), out val);

            return val;
        }
        private void updateMaterialCost()
        {
            double stairCost = 0;
            double nl = Nails / 100;
            IEnumerable<Metal> stairMetals = metals.Where(x => x.IsStairMetal == false && x.Name.Contains("STAIR"));
            if (stairMetals != null)
            {
                stairCost = stairMetals.Select(x => x.MaterialExtension).Sum();
            }
            if (metals.Count>0 && miscMetals.Count>0)
            {
                double misSum = metals.Select(x => x.MaterialExtension).Sum() * nl;
                TotalMaterialCost = ((metals.Select(x => x.MaterialExtension).Sum()) +
                miscMetals.Select(x => x.MaterialExtension).Sum() - stairCost ) + misSum; 
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
            ////Calculate Labor Cost
            double misSum = miscMetals.Select(x => x.LaborExtension).Sum();
            misSum = (metals.Select(x => x.LaborExtension).Sum() +
            miscMetals.Select(x => x.LaborExtension).Sum() - stairCost);
                        
            if (isPrevailingWage  )
            {
                TotalLaborCost = misSum * (1 + prevailingWage + deductionOnLargeJob);
            }
            else
                TotalLaborCost = misSum * (1 + deductionOnLargeJob);

            if (!isDiscount && !isPrevailingWage)
            {
                if (metals.Count > 0 && miscMetals.Count > 0)
                {
                    TotalLaborCost = misSum;
                }
            }
        }
    }
}
