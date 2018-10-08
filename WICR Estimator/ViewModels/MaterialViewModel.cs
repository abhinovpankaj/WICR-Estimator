using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;
using System.Windows.Input;

namespace WICR_Estimator.ViewModels
{
    public class MaterialViewModel:BaseViewModel
    {
        #region privatefields
        private ObservableCollection<SystemMaterial> systemMaterials;
        private ObservableCollection<OtherItem> otherMaterials;
        private ObservableCollection<LaborContract> subContractLaborItems;
        private string weatherWearType;
        private double totalSqft;
        private double stairWidth;
        private int riserCount;
        private double deckPerimeter;
        private IList<IList<object>> materialDetails;
        ////08-10-18
        private ICommand _addRowCommand;
        private ICommand _calculateCostCommand;
        private ICommand _removeCommand;
        private ICommand _addORowCommand;
        private ICommand _removeOCommand;
        private int AddInt = 4;
        private int AddSInt;
        ////08-10-18
        #endregion


        #region public properties
        public DelegateCommand CheckboxCommand { get; set; }
        public ObservableCollection<LaborContract> SubContractLaborItems
        {
            get { return subContractLaborItems; }
            set
            {
                if (value!=subContractLaborItems)
                {
                    subContractLaborItems = value;
                    OnPropertyChanged("SubContractLaborItems");
                }
            }
        }
        public ObservableCollection<SystemMaterial> SystemMaterials
        {
            get
            {
                return systemMaterials;
            }
            set
            {
                if (value!= systemMaterials)
                {
                    systemMaterials = value;
                    OnPropertyChanged("SystemMaterials");
                }
            }
        }
        public ObservableCollection<OtherItem> OtherMaterials
        {
            get { return otherMaterials; }
            set
            {
                if (value!=otherMaterials)
                {
                    otherMaterials = value;
                    OnPropertyChanged("OtherMaterials");
                    CalOCTotal();
                    }
            }
        }
        private double sumFreight;
        public double SumFreight
        {
            get { return sumFreight; }
            set
            {
                if (value!=sumFreight)
                {
                    sumFreight = value;
                    OnPropertyChanged("SumFreight");
                }
            }
        }
        private double sumWeight;
        public double SumWeight
        {
            get { return sumWeight; }
            set
            {
                if (value != sumWeight)
                {
                    sumWeight = value;
                    OnPropertyChanged("SumWeight");
                }
            }
        }
        private double sumTotalMatExt;
        public double SumTotalMatExt
        {
            get { return sumTotalMatExt; }
            set
            {
                if (value != sumTotalMatExt)
                {
                    sumTotalMatExt = value;
                    OnPropertyChanged("SumTotalMatExt");
                }
            }
        }
        private double sumMatPrice;
        public double SumMatPrice
        {
            get { return sumMatPrice; }
            set
            {
                if (value != sumMatPrice)
                {
                    sumMatPrice = value;
                    OnPropertyChanged("SumMatPrice");
                }
            }
        }
        private double sumQty;
        public double SumQty
        {
            get { return sumQty; }
            set
            {
                if (value != sumQty)
                {
                    sumQty = value;
                    OnPropertyChanged("SumQty");
                }
            }
        }
        private double totalWeightbrkp;
        public double TotalWeightbrkp
        {
            get { return totalWeightbrkp; }
            set
            {
                if (value != totalWeightbrkp)
                {
                    totalWeightbrkp = value;
                    OnPropertyChanged("TotalWeightbrkp");
                }
            }
        }
        private double totalFreightCostBrkp;
        public double TotalFreightCostBrkp
        {
            get { return totalFreightCostBrkp; }
            set
            {
                if (value != totalFreightCostBrkp)
                {
                    totalFreightCostBrkp = value;
                    OnPropertyChanged("TotalFreightCostBrkp");
                }
            }
        }
        private double totalMaterialCostbrkp;
        public double TotalMaterialCostbrkp
        {
            get { return totalMaterialCostbrkp; }
            set
            {
                if (value != totalMaterialCostbrkp)
                {
                    totalMaterialCostbrkp = value;
                    OnPropertyChanged("TotalMaterialCostbrkp");
                }
            }
        }
        private double totalSubContractLaborCostBrkp;
        public double TotalSubContractLaborCostBrkp
        {
            get { return totalSubContractLaborCostBrkp; }
            set
            {
                if (value != totalSubContractLaborCostBrkp)
                {
                    totalSubContractLaborCostBrkp = value;
                    OnPropertyChanged("TotalSubContractLaborCostBrkp");
                }
            }
        }
        private double totalOCExtension;
        public double TotalOCExtension
        {
            get { return totalOCExtension; }
            set
            {
                if (value != totalOCExtension)
                {
                    totalOCExtension = value;
                    OnPropertyChanged("TotalOCExtension");
                }
            }
        }
        private double totalSCExtension;
        public double TotalSCExtension
        {
            get { return totalSCExtension; }
            set
            {
                if (value != totalSCExtension)
                {
                    totalSCExtension = value;
                    OnPropertyChanged("TotalSCExtension");
                }
            }
        }
        #endregion

        public MaterialViewModel()
        {
            SystemMaterials = new ObservableCollection<SystemMaterial>();
            OtherMaterials = new ObservableCollection<OtherItem>();
            SubContractLaborItems = new ObservableCollection<LaborContract>();
            weatherWearType = "Weather Wear";
            totalSqft = 1000;
            stairWidth = 4.5;
            riserCount = 30;
            deckPerimeter = 300;
            FetchMaterialValuesAsync();
            JobSetup.OnJobSetupChange += JobSetup_OnJobSetupChange;
            CheckboxCommand = new DelegateCommand(ApplyCheckUnchecks, canApply);
        }
        #region commands
        private bool canApply(object obj)
        {
            if (obj != null)
            {
                if (obj.ToString() == "Lip Color" || obj.ToString() == "Vista Paint Acripoxy" || obj.ToString() == "Aj-44A Dressing(Sealer)")
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
            
        }

        private void ApplyCheckUnchecks(object obj)
        {
            if (obj==null)
            {
                return;
            }
            if (obj.ToString()=="Lip Color")
            {
                var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();

                foreach (SystemMaterial mat in materials)
                {
                    if (mat.Name == "Lip Color")
                    {
                        mat.IsMaterialEnabled = false;
                    }

                    if (mat.Name== "Vista Paint Acripoxy"|| mat.Name == "Aj-44A Dressing(Sealer)")
                    {
                        mat.IsMaterialChecked = false;
                        mat.IsMaterialEnabled = true;
                    }
                    if (mat.Name== "Resistite Regular White" || mat.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)"
                        ||mat.Name== "Custom Texture Skip Trowel(Resistite Smooth White)")
                    {
                        mat.IsMaterialChecked = true;
                    }
                    if (mat.Name == "Resistite Regular Gray"|| mat.Name == "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"
                        || mat.Name== "Custom Texture Skip Trowel(Resistite Smooth Gray)")
                    {
                        mat.IsMaterialChecked = false;
                    }
                }
            }
            if (obj.ToString() == "Vista Paint Acripoxy")
            {
                var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();
                foreach (SystemMaterial mat in materials)
                {
                    if (mat.Name == "Vista Paint Acripoxy")
                    {
                        mat.IsMaterialEnabled = false;
                    }
                    if (mat.Name == "Lip Color" || mat.Name == "Aj-44A Dressing(Sealer)")
                    {
                        mat.IsMaterialChecked = false;
                        mat.IsMaterialEnabled = true;
                    }
                    if (mat.Name == "Resistite Regular White" || mat.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)"
                        || mat.Name == "Custom Texture Skip Trowel(Resistite Smooth White)")
                    {
                        mat.IsMaterialChecked = false;
                    }
                    if (mat.Name == "Resistite Regular Gray" || mat.Name == "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"
                        || mat.Name == "Custom Texture Skip Trowel(Resistite Smooth Gray)")
                    {
                        mat.IsMaterialChecked = true;
                    }
                }
            }
            if (obj.ToString() == "Aj-44A Dressing(Sealer)")
            {
                
                var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();
                foreach (SystemMaterial mat in materials)
                {
                    if (mat.Name == "Aj-44A Dressing(Sealer)")
                    {
                        mat.IsMaterialEnabled = false;
                    }

                    if (mat.Name == "Lip Color" || mat.Name == "Vista Paint Acripoxy")
                    {
                        mat.IsMaterialChecked = false;
                        mat.IsMaterialEnabled = true;
                    }
                    if (mat.Name == "Resistite Regular White" || mat.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)"
                        || mat.Name == "Custom Texture Skip Trowel(Resistite Smooth White)")
                    {
                        mat.IsMaterialChecked = false;
                    }
                    if (mat.Name == "Resistite Regular Gray" || mat.Name == "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"
                        || mat.Name == "Custom Texture Skip Trowel(Resistite Smooth Gray)")
                    {
                        mat.IsMaterialChecked = true;
                    }
                }
            }

            if (obj.ToString() == "Neotex Standard Powder(Body Coat)" || obj.ToString() == "Neotex Standard Powder(Body Coat 1)")
            {
                var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();
                bool coat, coat1=false;
                SystemMaterial neomat=null;
                foreach (SystemMaterial mat in materials)
                {
                    if (mat.Name== "Neotex Standard Powder(Body Coat)")
                    {
                        coat = mat.IsMaterialChecked;
                    }
                    if (mat.Name == "Neotex Standard Powder(Body Coat) 1")
                    {
                        coat1 = mat.IsMaterialChecked;
                    }
                    if (mat.Name== "Neotex-38 Paste")
                    {
                        neomat = mat;
                    }
                }
                if (coat1==false&&coat1==false)
                {
                    if (neomat!=null)
                    {
                        neomat.IsMaterialChecked = false;
                    }                    
                }            
            }
        }
        
        #endregion
        //Get data from googlesheets
        private async void FetchMaterialValuesAsync()
        {
            if (materialDetails == null)
            {
                materialDetails = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "H33:K59");
            }
            
            SystemMaterials = GetSystemMaterial();
            OtherMaterials = GetOtherMaterials();
            SubContractLaborItems = GetLaborItems();
            calculateTotals();
            CalOCTotal();
            CalculateCostBreakup();
        }

        //private ObservableCollection<LaborContract> GetLaborItems()
        //{
            
        //}

        private ObservableCollection<OtherItem> GetOtherMaterials()
        {
            ObservableCollection<OtherItem>  om = new ObservableCollection<OtherItem>();
            om.Add(new OtherItem { Name = "Access issues?"});
            om.Add(new OtherItem { Name = "Additional prep?"});
            om.Add(new OtherItem { Name = "Additional labor?"});
            om.Add(new OtherItem { Name = "Alternate material?"});
            om.Add(new OtherItem { Name = "Additional Move ons?"});
            return om;
        }
        private ObservableCollection<LaborContract> GetLaborItems()
        {
            ObservableCollection<LaborContract> SC = new ObservableCollection<LaborContract>();
            SC.Add(new LaborContract { Name = "" });          
            return SC;
        }
        //Logic to check uncheck materials
        #region IcommandSection
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
        public ICommand AddORowCommand
        {
            get
            {
                if (_addORowCommand == null)
                {
                    _addORowCommand = new DelegateCommand(AddORow, CanAddRows);
                }

                return _addORowCommand;
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
        public ICommand RemoveOCommand
        {
            get
            {
                if (_removeOCommand == null)
                {
                    _removeOCommand = new DelegateCommand(RemoveORow, CanRemoveRow);
                }

                return _removeOCommand;
            }
        }
        private bool CanRemoveRow(object obj)
        {
            return true;
        }

        private void RemoveRow(object obj)
        {
            int index = otherMaterials.IndexOf(obj as OtherItem);
            if (AddInt > 4 && index < otherMaterials.Count)
            {
                otherMaterials.RemoveAt(AddInt);
                AddInt = AddInt - 1;
            }
        }
        private void RemoveORow(object obj)
        {
            int index = subContractLaborItems.IndexOf(obj as LaborContract);
            if (AddSInt > 0 && index < subContractLaborItems.Count)
            {
                subContractLaborItems.RemoveAt(AddSInt);
                AddSInt = AddSInt - 1;
            }
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
            calculateRLqty();
            neotaxQty();
            calculateTotals();
            CalOCTotal();
            CalculateCostBreakup();
        }

        private bool CanAddRows(object obj)
        {
            return true;
        }

        private void AddRow(object obj)
        {
            AddInt = AddInt + 1;
            otherMaterials.Add(new OtherItem { Name = ""});

        }
        private void AddORow(object obj)
        {
            AddSInt = AddSInt + 1;
            subContractLaborItems.Add(new LaborContract { Name = "" });

        }
        #endregion
        #region methods
        private bool getCheckboxCheckStatus(string materialName)
        {
            if (weatherWearType == "Weather Wear")
            {
                switch (materialName.ToUpper())
                {
                    case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                    case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                    case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                    case "CPC MEMBRANE":
                    case "NEOTEX-38 PASTE":
                    case "NEOTEX STANDARD POWDER(BODY COAT)":
                    case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                    case "RESISTITE LIQUID":
                    case "LIP COLOR":
                    case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                    //case "AJ-44A DRESSING(SEALER)":
                    //case "VISTA PAINT ACRIPOXY":
                    case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                    case "WEATHER SEAL XL TWO COATS":
                    case "STAIR NOSING FROM DEXOTEX":
                        return true;
                    default:
                        return false;
                }
            }
            else if (weatherWearType == "Weather Wear Rehab")
            {
                switch (materialName.ToUpper())
                {
                    case "LIGHT CRACK REPAIR":
                    case "RESISTITE REGULAR OVER TEXTURE (#55 BAG)":
                    case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                    case "RP FABRIC 10 INCH WIDE X(300 LF) FROM ACME":
                    case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                    case "CPC MEMBRANE":
                    case "NEOTEX-38 PASTE":
                    case "NEOTEX STANDARD POWDER (BODY COAT)":
                    case "NEOTEX STANDARD POWDER(BODY COAT)1":
                    case "RESISTITE LIQUID":
                    case "RESISTITE REGULAR GRAY":
                    case "RESISTITE REGULAR WHITE":
                    case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                    //case "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)":
                    case "LIP COLOR":
                    case "AJ-44A DRESSING (SEALER)":
                    case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                    case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)":
                    case "VISTA PAINT ACRIPOXY":
                    case "Stair Nosing From Dexotex":
                    //case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                    case "WEATHER SEAL XL TWO COATS":
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
        private bool getCheckboxEnabledStatus(string materialName)
        {
            if (weatherWearType=="Weather Wear")
            {
                switch (materialName.ToUpper())
                {
                    case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    case "LIP COLOR":
                    case "AJ-44A DRESSING(SEALER)":
                    case "VISTA PAINT ACRIPOXY":
                    case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                    case "WEATHER SEAL XL TWO COATS":
                    case "STAIR NOSING FROM DEXOTEX":
                        return true;
                    default:
                        return false;
                }
            }
            else if (weatherWearType == "Weather Wear Rehab")
            {
                switch (materialName.ToUpper())
                {
                    case "LIGHT CRACK REPAIR":
                    case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                    case "RP FABRIC 10 INCH WIDE X(300 LF) FROM ACME":
                    case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                    case "CPC MEMBRANE":
                    case "NEOTEX STANDARD POWDER(BODY COAT)":
                    case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                    case "RESISTITE REGULAR GRAY":
                    case "LIP COLOR":
                    case "AJ-44A DRESSING(SEALER)":
                    case "VISTA PAINT ACRIPOXY":
                    case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                    case "WEATHER SEAL XL TWO COATS":
                         return true;
                    default:
                        return false;
                }
            }
            return false;
        }


        public ObservableCollection<SystemMaterial> GetSystemMaterial()
        {
            
            ObservableCollection<SystemMaterial> smP = new ObservableCollection<SystemMaterial>();
            int cov;
            double mp;
            double w;
            double lfArea;

            int.TryParse(materialDetails[0][2].ToString(), out cov);
            double.TryParse(materialDetails[0][0].ToString(), out mp);
            double.TryParse(materialDetails[0][3].ToString(), out w);
            lfArea = getlfArea("Light Crack Repair");
            smP.Add(new SystemMaterial
            {
                IsWWR = true,
                IsMaterialChecked = getCheckboxCheckStatus("Light Crack Repair"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Light Crack Repair"),
                Name = "Light Crack Repair",
                SMUnits = "Sq Ft",
                SMSqft =lfArea ,
                Coverage=cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Light Crack Repair", cov, lfArea),
                
            });
            int.TryParse(materialDetails[1][2].ToString(), out cov);
            double.TryParse(materialDetails[1][0].ToString(), out mp);
            double.TryParse(materialDetails[1][3].ToString(), out w);
            lfArea = getlfArea("Large Crack Repair");
            smP.Add(new SystemMaterial
            {
                IsWWR=true,
                IsMaterialChecked = getCheckboxCheckStatus("Large Crack Repair"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Large Crack Repair"),
                Name = "Large Crack Repair",
                SMUnits = "LF",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Large Crack Repair", cov, lfArea),
                
            });
            int.TryParse(materialDetails[2][2].ToString(), out cov);
            double.TryParse(materialDetails[2][0].ToString(), out mp);
            double.TryParse(materialDetails[2][3].ToString(), out w);
            lfArea = getlfArea("Bubble Repair(Measure Sq Ft)");
            smP.Add(new SystemMaterial
            {
                IsWWR = true,
                IsMaterialChecked = getCheckboxCheckStatus("Bubble Repair(Measure Sq Ft)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Bubble Repair(Measure Sq Ft)"),
                Name = "Bubble Repair(Measure Sq Ft)",
                SMUnits = "Sq Ft",
                SMSqft =lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Bubble Repair(Measure Sq Ft)", cov, lfArea),
                
            });
            int.TryParse(materialDetails[3][2].ToString(), out cov);
            double.TryParse(materialDetails[3][0].ToString(), out mp);
            double.TryParse(materialDetails[3][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular Over Texture(#55 Bag)");
            smP.Add(new SystemMaterial
            {

                IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Over Texture(#55 Bag)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Over Texture(#55 Bag)"),
                Name = "Resistite Regular Over Texture(#55 Bag)",
                SMUnits = "55 LB BAG",
                SMSqft = lfArea,
                Coverage =cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Resistite Regular Over Texture(#55 Bag)", cov, lfArea),
               
            });
            int.TryParse(materialDetails[4][2].ToString(), out cov);
            double.TryParse(materialDetails[4][0].ToString(), out mp);
            double.TryParse(materialDetails[4][3].ToString(), out w);
            lfArea = getlfArea("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale");
            smP.Add(new SystemMaterial
            {

                IsMaterialChecked = getCheckboxCheckStatus("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale"),
                IsMaterialEnabled = getCheckboxEnabledStatus("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale"),
                Name = "30# Divorcing Felt (200 Sq Ft) From Ford Wholesale",
                SMUnits = "ROLL",
                SMSqft =lfArea ,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale", cov, lfArea),
                
            });
            int.TryParse(materialDetails[5][2].ToString(), out cov);
            double.TryParse(materialDetails[5][0].ToString(), out mp);
            double.TryParse(materialDetails[5][3].ToString(), out w);
            lfArea = getlfArea("Rp Fabric 10 Inch Wide X (300 Lf) From Acme");
            smP.Add(new SystemMaterial
            {

                IsMaterialChecked = getCheckboxCheckStatus("Rp Fabric 10 Inch Wide X (300 Lf) From Acme"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Rp Fabric 10 Inch Wide X (300 Lf) From Acme"),
                Name = "Rp Fabric 10 Inch Wide X (300 Lf) From Acme",
                SMUnits = "ROLL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Rp Fabric 10 Inch Wide X (300 Lf) From Acme", cov, lfArea),
               
            });
            int.TryParse(materialDetails[6][2].ToString(), out cov);
            double.TryParse(materialDetails[6][0].ToString(), out mp);
            double.TryParse(materialDetails[6][3].ToString(), out w);
            lfArea = getlfArea("Glasmat #4 (1200 Sq Ft) From Acme");
            smP.Add(new SystemMaterial
            {

                IsMaterialChecked = getCheckboxCheckStatus("Glasmat #4 (1200 Sq Ft) From Acme"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Glasmat #4 (1200 Sq Ft) From Acme"),
                Name = "Glasmat #4 (1200 Sq Ft) From Acme",
                SMUnits = "ROLL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Glasmat #4 (1200 Sq Ft) From Acme", cov, lfArea),
                
            });
            int.TryParse(materialDetails[7][2].ToString(), out cov);
            double.TryParse(materialDetails[7][0].ToString(), out mp);
            double.TryParse(materialDetails[7][3].ToString(), out w);
            lfArea = getlfArea("Cpc Membrane");
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Cpc Membrane"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Cpc Membrane"),
                Name = "Cpc Membrane",
                SMUnits = "5 GAL PAIL",
                SMSqft =lfArea ,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Cpc Membrane", cov, lfArea),
                
            });
            int.TryParse(materialDetails[8][2].ToString(), out cov);
            double.TryParse(materialDetails[8][0].ToString(), out mp);
            double.TryParse(materialDetails[8][3].ToString(), out w);
            lfArea = getlfArea("Neotex-38 Paste");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Neotex-38 Paste"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Neotex-38 Paste"),
                Name = "Neotex-38 Paste",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice =mp,
                Weight = w,
                Qty = getQuantity("Neotex-38 Paste", cov, lfArea),
                
            });
            int.TryParse(materialDetails[9][2].ToString(), out cov);
            double.TryParse(materialDetails[9][0].ToString(), out mp);
            double.TryParse(materialDetails[9][3].ToString(), out w);
            lfArea = getlfArea("Neotex Standard Powder(Body Coat)");
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Neotex Standard Powder(Body Coat)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Neotex Standard Powder(Body Coat)"),
                Name = "Neotex Standard Powder(Body Coat)",
                SMUnits = "45 LB BAG",
                SMSqft =lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Neotex Standard Powder(Body Coat)", cov, lfArea),
                
            });
            int.TryParse(materialDetails[10][2].ToString(), out cov);
            double.TryParse(materialDetails[10][0].ToString(), out mp);
            double.TryParse(materialDetails[10][3].ToString(), out w);
            lfArea = getlfArea("Neotex Standard Powder(Body Coat) 1");
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Neotex Standard Powder(Body Coat) 1"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Neotex Standard Powder(Body Coat) 1"),
                Name = "Neotex Standard Powder(Body Coat) 1",
                SMUnits = "45 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Neotex Standard Powder(Body Coat) 1", cov, lfArea),
                
            });
            int.TryParse(materialDetails[11][2].ToString(), out cov);
            double.TryParse(materialDetails[11][0].ToString(), out mp);
            double.TryParse(materialDetails[11][3].ToString(), out w);
            lfArea = getlfArea("Resistite Liquid");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Liquid"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Liquid"),
                Name = "Resistite Liquid",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage =cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Resistite Liquid", cov, lfArea),
                
            });
            int.TryParse(materialDetails[12][2].ToString(), out cov);
            double.TryParse(materialDetails[12][0].ToString(), out mp);
            double.TryParse(materialDetails[12][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular Gray");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Gray"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Gray"),
                Name = "Resistite Regular Gray",
                SMUnits = "55 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Resistite Regular Gray", cov, lfArea),
                
            });
            int.TryParse(materialDetails[13][2].ToString(), out cov);
            double.TryParse(materialDetails[13][0].ToString(), out mp);
            double.TryParse(materialDetails[13][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular White");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular White"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular White"),
                Name = "Resistite Regular White",
                SMUnits = "55 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Resistite Regular White", cov, lfArea),

            });
            int.TryParse(materialDetails[14][2].ToString(), out cov);
            double.TryParse(materialDetails[14][0].ToString(), out mp);
            double.TryParse(materialDetails[14][3].ToString(), out w);
            lfArea=getlfArea("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent=true,
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"),
                Name = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)",
                SMUnits = "40 LB BAG",
                SMSqft = lfArea,
                Coverage =cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)", cov, lfArea),
                
            });
            int.TryParse(materialDetails[15][2].ToString(), out cov);
            double.TryParse(materialDetails[15][0].ToString(), out mp);
            double.TryParse(materialDetails[15][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular Or Smooth White(Knock Down Or Smooth)");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Or Smooth White(Knock Down Or Smooth)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Or Smooth White(Knock Down Or Smooth)"),
                Name = "Resistite Regular Or Smooth White(Knock Down Or Smooth)",
                SMUnits = "40 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Resistite Regular Or Smooth White(Knock Down Or Smooth)", cov, lfArea),

            });
            int.TryParse(materialDetails[16][2].ToString(), out cov);
            double.TryParse(materialDetails[16][0].ToString(), out mp);
            double.TryParse(materialDetails[16][3].ToString(), out w);
            lfArea = getlfArea("Aj-44A Dressing(Sealer)");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent=true,
                IsMaterialChecked = getCheckboxCheckStatus("Aj-44A Dressing(Sealer)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Aj-44A Dressing(Sealer)"),
                Name = "Aj-44A Dressing(Sealer)",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Aj-44A Dressing(Sealer)", cov, lfArea),
                
            });
            int.TryParse(materialDetails[17][2].ToString(), out cov);
            double.TryParse(materialDetails[17][0].ToString(), out mp);
            double.TryParse(materialDetails[17][3].ToString(), out w);
            lfArea = getlfArea("Vista Paint Acripoxy");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent=true,
                IsMaterialChecked = getCheckboxCheckStatus("Vista Paint Acripoxy"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Vista Paint Acripoxy"),
                Name = "Vista Paint Acripoxy",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Vista Paint Acripoxy", cov, lfArea),
                
            });
            int.TryParse(materialDetails[18][2].ToString(), out cov);
            double.TryParse(materialDetails[18][0].ToString(), out mp);
            double.TryParse(materialDetails[18][3].ToString(), out w);
            lfArea = getlfArea("Lip Color");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent=true,
                IsMaterialChecked = getCheckboxCheckStatus("Lip Color"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Lip Color"),
                Name = "Lip Color",
                SMUnits = "Sq Ft",
                SMSqft =lfArea ,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Lip Color", cov, lfArea),
                
            });

            int.TryParse(materialDetails[19][2].ToString(), out cov);
            double.TryParse(materialDetails[19][0].ToString(), out mp);
            double.TryParse(materialDetails[19][3].ToString(), out w);
            lfArea = getlfArea("Resistite Universal Primer(Add 50% Water)");
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Resistite Universal Primer(Add 50% Water)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Universal Primer(Add 50% Water)"),
                Name = "Resistite Universal Primer(Add 50% Water)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Resistite Universal Primer(Add 50% Water)", cov, lfArea),
                
            });
            int.TryParse(materialDetails[20][2].ToString(), out cov);
            double.TryParse(materialDetails[20][0].ToString(), out mp);
            double.TryParse(materialDetails[20][3].ToString(), out w);
            lfArea = getlfArea("Custom Texture Skip Trowel(Resistite Smooth Gray)");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent=true,
                IsMaterialChecked = getCheckboxCheckStatus("Custom Texture Skip Trowel(Resistite Smooth Gray)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Custom Texture Skip Trowel(Resistite Smooth Gray)"),
                Name = "Custom Texture Skip Trowel(Resistite Smooth Gray)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Custom Texture Skip Trowel(Resistite Smooth Gray)", cov, lfArea),
                
            });
            int.TryParse(materialDetails[21][2].ToString(), out cov);
            double.TryParse(materialDetails[21][0].ToString(), out mp);
            double.TryParse(materialDetails[21][3].ToString(), out w);
            lfArea = getlfArea("Custom Texture Skip Trowel(Resistite Smooth White)");
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Custom Texture Skip Trowel(Resistite Smooth White)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Custom Texture Skip Trowel(Resistite Smooth White)"),
                Name = "Custom Texture Skip Trowel(Resistite Smooth White)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Custom Texture Skip Trowel(Resistite Smooth White)", cov, lfArea),

            });
            int.TryParse(materialDetails[22][2].ToString(), out cov);
            double.TryParse(materialDetails[22][0].ToString(), out mp);
            double.TryParse(materialDetails[22][3].ToString(), out w);
            lfArea = getlfArea("Weather Seal XL two Coats");
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Weather Seal XL two Coats"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Weather Seal XL two Coats"),
                Name = "Weather Seal XL two Coats",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Weather Seal XL two Coats", cov, lfArea),

            });
            int.TryParse(materialDetails[23][2].ToString(), out cov);
            double.TryParse(materialDetails[23][0].ToString(), out mp);
            double.TryParse(materialDetails[23][3].ToString(), out w);
            lfArea = getlfArea("Stair Nosing From Dexotex");
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Stair Nosing From Dexotex"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Stair Nosing From Dexotex"),
                Name = "Stair Nosing From Dexotex",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = getQuantity("Stair Nosing From Dexotex", cov, lfArea),
                
            });
            int.TryParse(materialDetails[24][2].ToString(), out cov);
            double.TryParse(materialDetails[24][0].ToString(), out mp);
            double.TryParse(materialDetails[24][3].ToString(), out w);
            lfArea = getlfArea("Extra Stair Nosing Lf");
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Extra Stair Nosing Lf"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Extra Stair Nosing Lf"),
                Name = "Extra Stair Nosing Lf",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight =w,
                
            });
            int.TryParse(materialDetails[25][2].ToString(), out cov);
            double.TryParse(materialDetails[25][0].ToString(), out mp);
            double.TryParse(materialDetails[25][3].ToString(), out w);
            lfArea = getlfArea("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
                Name = "Plywood 3/4 & Blocking(# Of 4X8 Sheets)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice =mp,
                Weight = w,
                
            });
            int.TryParse(materialDetails[26][2].ToString(), out cov);
            double.TryParse(materialDetails[26][0].ToString(), out mp);
            double.TryParse(materialDetails[26][3].ToString(), out w);
            lfArea = getlfArea("Stucco Material Remove And Replace (Lf)");
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Stucco Material Remove And Replace (Lf)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Stucco Material Remove And Replace (Lf)"),
                Name = "Stucco Material Remove And Replace (Lf)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage =cov,
                MaterialPrice =mp,
                Weight = w,
                //Qty = getQuantity("Stucco Material Remove And Replace (Lf)", cov, lfArea),
                
            });
            

            return smP;
        }
        //Event handler to get JobSetup change updates.
        private void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js!=null)
            {
                weatherWearType = js.WeatherWearType;
                totalSqft = js.TotalSqft;
                stairWidth = js.StairWidth;
                riserCount = js.RiserCount;
                deckPerimeter = js.DeckPerimeter;
            }
            FetchMaterialValuesAsync();
            
        }
        private double getQuantity(string materialName,double coverage,double lfArea)
        {
            switch (materialName.ToUpper())
            {
                case "LIGHT CRACK REPAIR":
                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                case "RP FABRIC 10 INCH WIDE X(300 LF) FROM ACME":
                case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                case "CPC MEMBRANE":
                case "NEOTEX STANDARD POWDER(BODY COAT)":
                case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                case "RESISTITE REGULAR WHITE":
                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE REGULAR OR SMOOTH GRAY(KNOCK DOWN OR SMOOTH)":
                case "LIP COLOR":
                case "AJ-44A DRESSING (SEALER)":
                case "VISTA PAINT ACRIPOXY":
                case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                case "WEATHER SEAL XL TWO COATS":
                    return Math.Round(lfArea / coverage,2);

                case "STAIR NOSING FROM DEXOTEX":
                    return Math.Round(lfArea * stairWidth,2);
                case "NEOTEX-38 PASTE":
                    return Math.Round(neotaxQty(),2);
                case "RESISTITE LIQUID":
                    return Math.Round(calculateRLqty());
                default:
                    return 0;
            }
        }

        private double calculateRLqty()
        {
            double val1, val2, val3,val4;
            double.TryParse(materialDetails[12][2].ToString(), out val1);
            double.TryParse(materialDetails[13][2].ToString(), out val2);
            double.TryParse(materialDetails[17][2].ToString(), out val3);
            double.TryParse(materialDetails[3][2].ToString(), out val4);
            double qty=(val1+val2+val3)*0.33 + val4 / 5;
            return qty;
        }
       
        private double neotaxQty()
        {
            double val1, val2;
            double.TryParse(materialDetails[9][2].ToString(), out val1);
            double.TryParse(materialDetails[10][2].ToString(), out val2);
            return (val2 * 1.5 + val1 * 1.25) / 5;
        }

        private double getlfArea(string materialName)
        {
            string upp = materialName.ToUpper();
            switch (materialName.ToUpper())
            {
                case "LIGHT CRACK REPAIR":
                case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                    return totalSqft;
                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                case "CPC MEMBRANE":
                case "NEOTEX STANDARD POWDER(BODY COAT)":
                case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                case "RESISTITE REGULAR WHITE":
                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE REGULAR OR SMOOTH GRAY(KNOCK DOWN OR SMOOTH)":
                case "LIP COLOR":
                case "AJ-44A DRESSING(SEALER)":
                case "VISTA PAINT ACRIPOXY":
                case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                case "WEATHER SEAL XL TWO COATS":
                    return Math.Round((riserCount * stairWidth * 2 ) +totalSqft,2);
                case "STAIR NOSING FROM DEXOTEX":
                    return riserCount;
                case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                    return Math.Round(deckPerimeter + stairWidth * riserCount * 2,2);
                default:
                    return 0;
            }
        }

/// Other Cost Total

        private void CalOCTotal()
        {

            if (OtherMaterials.Count > 0)
            {
                if (OtherMaterials.Count > 0)
                {
                    ///sumtotal              
                    TotalOCExtension = Math.Round(OtherMaterials.Select(x => x.Extension).Sum(), 2);
                }
            }

        }
        /// Other Cost Total
        /// SubContract Cost
        private void CalSCTotal()
        {

            if (SubContractLaborItems.Count > 0)
            {
                if (SubContractLaborItems.Count > 0)
                {
                    ///sumtotal              
                   // TotalSCExtension = Math.Round(SubContractLaborItems.Select(x => x.MaterialExtensionConlbrcst).Sum(), 2);
                }
            }

        }
        private void calculateTotals()
        {
            IEnumerable<SystemMaterial> systemMaterial = systemMaterials.Where(x => x.IsMaterialChecked == true && x.Qty > 0);
            if (systemMaterials.Count > 0)
            {
                SumQty = Math.Round(systemMaterials.Select(x => x.Qty).Sum(),2);
                SumMatPrice = Math.Round(systemMaterials.Select(x => x.MaterialPrice).Sum(), 2);
                SumTotalMatExt = Math.Round(systemMaterials.Select(x => x.MaterialExtension).Sum(), 2);
                SumWeight = Math.Round(systemMaterials.Select(x => x.Weight).Sum(), 2);
                
                //Total Freight
                             
                SumFreight = Math.Round(FreightCalculator(SumWeight), 2);
            }
        }

        private double FreightCalculator(double weight)
        {
            double result;
            double frCalc = 0;
            if (weight != 0)
            {
                if (weight == 0)
                {
                    frCalc = 0;
                }

                else
                {
                    if (weight > 10000)
                    {
                        frCalc = 0.03 * weight;
                    }

                    else
                    {
                        if (weight > 5000)
                        {
                            frCalc = 0.04 * weight;
                        }
                        else
                        {
                            if (weight > 2000)
                            {
                                frCalc = 0.09 * weight;
                            }
                            else
                            {
                                if (weight > 1000)
                                {
                                    frCalc = 0.12 * weight;
                                }
                                else
                                {
                                    if (weight > 400)
                                    {
                                        frCalc = 75;
                                    }
                                    else
                                    {
                                        frCalc = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            result = Math.Round(frCalc,2);
            return result;
        }
        private void CalculateCostBreakup()
        {
            
            IEnumerable<SystemMaterial> systemCBMaterial = systemMaterials.Where(x => x.IsMaterialChecked == true && x.Qty > 0);
            if (systemCBMaterial != null)
            {
                TotalMaterialCostbrkp = Math.Round((SumTotalMatExt + TotalOCExtension + TotalSCExtension),2);
                TotalWeightbrkp = Math.Round(SumWeight,2);
                TotalFreightCostBrkp = Math.Round(FreightCalculator(TotalWeightbrkp),2);
                TotalSubContractLaborCostBrkp = Math.Round(TotalSCExtension, 2);
            }
        }
        #endregion
    }
}
