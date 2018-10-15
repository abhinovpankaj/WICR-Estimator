﻿using System;
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

        public Totals MetalTotals { set; get; }

        public Totals SlopeTotals { set; get; }

        #region privatefields
        private ObservableCollection<SystemMaterial> systemMaterials;
        private ObservableCollection<OtherItem> otherMaterials;
        private ObservableCollection<OtherItem> otherLaborMaterials;
        private ObservableCollection<LaborContract> subContractLaborItems;
        private string weatherWearType;
        private double totalSqft;
        private double stairWidth;
        private int riserCount;
        private double deckPerimeter;
        private bool isApprovedforCement;
        private double laborRate;
        private bool isPrevailingWage;
        private bool isSpecialMetal;
        private bool isDiscounted;
        private IList<IList<Object>> laborDetails;
        private IList<IList<object>> materialDetails;
        
        private ICommand _addRowCommand;
        private ICommand _calculateCostCommand;
        private ICommand _removeCommand;
        private int AddInt = 4;


        #endregion

        public MaterialViewModel()
        {
            
            SystemMaterials = new ObservableCollection<SystemMaterial>();
            OtherMaterials = new ObservableCollection<OtherItem>();
            OtherLaborMaterials = new ObservableCollection<OtherItem>();
            SubContractLaborItems = new ObservableCollection<LaborContract>();
            weatherWearType = "Weather Wear";
            totalSqft = 1000;
            stairWidth = 4.5;
            riserCount = 30;
            deckPerimeter = 300;
            isSpecialMetal = false;
            isDiscounted = true;
            isPrevailingWage = true;
            isApprovedforCement = false;
            
            JobSetup.OnJobSetupChange += JobSetup_OnJobSetupChange;
            SystemMaterial.OnQTyChanged += (s, e) => { setExceptionValues(); };
            CheckboxCommand = new DelegateCommand(ApplyCheckUnchecks, canApply);            
        }

        

        public MaterialViewModel(Totals metalTotals,Totals slopeTotals)
            :this()
        {
            MetalTotals = metalTotals;
            SlopeTotals = slopeTotals;
            MetalTotals.OnTotalsChange += MetalTotals_OnTotalsChange;

            SlopeTotals.OnTotalsChange += MetalTotals_OnTotalsChange;
            FetchMaterialValuesAsync();
            calculateLaborHrs();
        }

        private void MetalTotals_OnTotalsChange(object sender, EventArgs e)
        {
            Totals tabTotals = sender as Totals;
            if (tabTotals!=null)
            {
                switch (tabTotals.TabName)
                {
                    case "Metal":
                        MetalTotals = tabTotals;
                        break;
                    case "Slope":
                        SlopeTotals = tabTotals;
                        break;
                    default:
                        break;
                }
            }
            calculateLaborHrs();

        }
        #region public properties

        #region Material
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
                if (value != sumFreight)
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
                    OnPropertyChanged("matTotals");
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

        #region Labor
        public ObservableCollection<OtherItem> OtherLaborMaterials
        {
            get { return otherLaborMaterials; }
            set
            {
                if (value != otherLaborMaterials)
                {
                    otherLaborMaterials = value;
                    OnPropertyChanged("OtherLaborMaterials");                   
                }
            }
        }
        public double TotalHrsLabor { get; set; }
        public double TotalHrsSystemLabor { get; set; }
        public double TotalHrsMetalLabor { get; set; }
        public double TotalHrsSlopeLabor { get; set; }
        public double TotalHrsFreightLabor { get; set; }
        public double TotalHrsDriveLabor { get; set; }

        public double TotalLaborUnitPrice { get; set; }
        public double TotalSetupTimeLabor { get; set; }
        public double TotalLaborExtension { get; set; }
        
        public double TotalSlopingPrice { get; set; }
        public double TotalMetalPrice { get; set; }
        public double TotalSystemPrice { get; set; }
        public double TotalSubcontractLabor { get; set; }
        public double TotalSale { get; set; }

        public double AllTabsLaborTotal { get; set; }
        public double AllTabsMaterialTotal { get; set; }
        public double AllTabsFreightTotal { get; set; }
        public double AllTabsSubContractTotal { get; set; }
        
        private void calculateLaborHrs()
        {
            TotalHrsDriveLabor=totalSqft < 1001 ? 10 : totalSqft / 1000 * 10;
            TotalHrsFreightLabor = SumFreight / laborRate;

            TotalHrsSystemLabor = isPrevailingWage ? (TotalLaborExtension / laborRate) * .445 - TotalHrsDriveLabor :
                                                  (TotalLaborExtension / laborRate) - TotalHrsDriveLabor;
            

            TotalHrsMetalLabor= isPrevailingWage ? (MetalTotals.LaborExtTotal / laborRate) * .445  :
                                                  (MetalTotals.LaborExtTotal / laborRate);
            TotalHrsSlopeLabor= isPrevailingWage ? (SlopeTotals.LaborExtTotal / laborRate) * .445 :
                                                  (SlopeTotals.LaborExtTotal / laborRate);


            TotalHrsLabor = TotalHrsSystemLabor + TotalHrsMetalLabor + TotalHrsSlopeLabor +
                TotalHrsFreightLabor + TotalHrsDriveLabor;

        }


        #endregion

        #endregion

        
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
                    return getCheckboxCheckStatus(obj.ToString());
            }
            else
                return true;
            
        }

        private void ApplyCheckUnchecks(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (obj.ToString() == "Lip Color")
            {
                var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();

                foreach (SystemMaterial mat in materials)
                {
                    if (mat.Name == "Lip Color")
                    {
                        mat.IsMaterialEnabled = false;
                    }

                    if (mat.Name == "Vista Paint Acripoxy" || mat.Name == "Aj-44A Dressing(Sealer)")
                    {
                        mat.IsMaterialChecked = false;
                        mat.IsMaterialEnabled = true;
                    }
                    if (mat.Name == "Resistite Regular Gray")
                    {
                        mat.Name = "Resistite Regular White";
                        mat.MaterialPrice = double.Parse(materialDetails[13][0].ToString());
                    }
                    if (mat.Name == "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)")
                    {
                        mat.Name = "Resistite Regular Or Smooth White(Knock Down Or Smooth)";
                        mat.MaterialPrice = double.Parse(materialDetails[15][0].ToString());
                    }
                    if (mat.Name == "Custom Texture Skip Trowel(Resistite Smooth Gray)")
                    {
                        mat.Name = "Custom Texture Skip Trowel(Resistite Smooth White)";
                        mat.MaterialPrice = double.Parse(materialDetails[21][0].ToString());
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
                    if (mat.Name == "Resistite Regular White")
                    {
                        mat.Name = "Resistite Regular Gray";
                        mat.MaterialPrice = double.Parse(materialDetails[12][0].ToString());
                    }
                    if (mat.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)")
                    {
                        mat.Name = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)";
                        mat.MaterialPrice = double.Parse(materialDetails[14][0].ToString());
                    }
                    if (mat.Name == "Custom Texture Skip Trowel(Resistite Smooth White)")
                    {
                        mat.Name = "Custom Texture Skip Trowel(Resistite Smooth Gray)";
                        mat.MaterialPrice = double.Parse(materialDetails[20][0].ToString());
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
                    if (mat.Name == "Resistite Regular White")
                    {
                        mat.Name = "Resistite Regular Gray";
                        mat.MaterialPrice = double.Parse(materialDetails[12][0].ToString());
                    }
                    if (mat.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)")
                    {
                        mat.Name = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)";
                        mat.MaterialPrice = double.Parse(materialDetails[14][0].ToString());
                    }
                    if (mat.Name == "Custom Texture Skip Trowel(Resistite Smooth White)")
                    {
                        mat.Name = "Custom Texture Skip Trowel(Resistite Smooth Gray)";
                        mat.MaterialPrice = double.Parse(materialDetails[20][0].ToString());
                    }
                }
            }

            if (obj.ToString() == "Neotex Standard Powder(Body Coat)" || obj.ToString() == "Neotex Standard Powder(Body Coat) 1")
            {
                var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();
                bool coat = false, coat1 = false;
                SystemMaterial neomat = null;
                foreach (SystemMaterial mat in materials)
                {
                    if (mat.Name == "Neotex Standard Powder(Body Coat)")
                    {
                        coat = mat.IsMaterialChecked;
                    }
                    if (mat.Name == "Neotex Standard Powder(Body Coat) 1")
                    {
                        coat1 = mat.IsMaterialChecked;
                    }
                    if (mat.Name == "Neotex-38 Paste")
                    {
                        neomat = mat;
                    }
                }
                if (coat == false && coat1 == false)
                {
                    if (neomat != null)
                    {
                        neomat.IsMaterialChecked = false;
                    }

                }
                else
                {
                    if (neomat != null)
                    {
                        neomat.IsMaterialChecked = true;
                    }
                }

            }
            if (obj.ToString() == "Resistite Regular Gray")
            {
                var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();
                bool checkStatus=false;
                foreach (SystemMaterial item in materials)
                {
                    if (item.Name == "Resistite Regular Gray" || item.Name == "Resistite Regular White")
                    {
                        checkStatus = item.IsMaterialChecked;
                        break;
                    }
                }
                foreach (SystemMaterial item in materials)
                {
                    if (item.Name== "Resistite Liquid")
                    {
                        item.IsMaterialChecked = checkStatus;
                    }
                    if (item.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)" || item.Name == "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)")
                    {
                        item.IsMaterialChecked = checkStatus;
                    }
                }
                
            }
        }
        
        #endregion
        //Get data from googlesheets
        private void FetchMaterialValuesAsync()
        {
            if (materialDetails == null)
            {
                //materialDetails = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "H33:K59");
               
                GSData gData = DataSerializer.DSInstance.deserializeGoogleData();
                laborDetails = gData.LaborData;
                materialDetails = gData.MaterialData;
                double.TryParse(gData.LaborRate[0][0].ToString(), out laborRate);
                
            }
            
            SystemMaterials = GetSystemMaterial();
            setExceptionValues();
            setCheckBoxes();
            OtherMaterials = GetOtherMaterials();
            OtherLaborMaterials= GetOtherMaterials(); 
            SubContractLaborItems = GetLaborItems();
            calculateMaterialTotals();
            CalOCTotal();
            CalculateCostBreakup();
            calculateLaborTotals();
            calculateLaborHrs();
            
        }

        private void setExceptionValues()
        {
            if (SystemMaterials.Count!=0)
            {
                SystemMaterial item = SystemMaterials.First(x => x.Name == "Extra Stair Nosing Lf");
                item.StairSqft = item.Qty;
                item = SystemMaterials.First(x => x.Name == "Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
                item.SMSqftH = item.Qty * 32;

                item = SystemMaterials.First(x => x.Name == "Stucco Material Remove And Replace (Lf)");
                item.SMSqftH = item.Qty;
            }
            
        }
        private ObservableCollection<OtherItem> GetOtherMaterials()
        {
            ObservableCollection<OtherItem>  om = new ObservableCollection<OtherItem>();
            om.Add(new OtherItem { Name = "Access issues?", IsReadOnly=true});
            om.Add(new OtherItem { Name = "Additional prep?", IsReadOnly = true });
            om.Add(new OtherItem { Name = "Additional labor?", IsReadOnly = true });
            om.Add(new OtherItem { Name = "Alternate material?", IsReadOnly = true });
            om.Add(new OtherItem { Name = "Additional Move ons?", IsReadOnly = true });
            return om;
        }
        private ObservableCollection<LaborContract> GetLaborItems()
        {
            ObservableCollection<LaborContract> SC = new ObservableCollection<LaborContract>();
            SC.Add(new LaborContract { Name = "" });
            SC.Add(new LaborContract { Name = "" });
            SC.Add(new LaborContract { Name = "" });
            return SC;
        }
        
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
            int index = otherMaterials.IndexOf(obj as OtherItem);
            if (AddInt > 4 && index < otherMaterials.Count)
            {
                OtherMaterials.RemoveAt(AddInt);
                OtherLaborMaterials.RemoveAt(AddInt);
                AddInt = AddInt - 1;
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
            calculateMaterialTotals();
            CalOCTotal();
            CalSCTotal();
            CalculateCostBreakup();
            calculateLaborTotals();
            calculateLaborHrs();
        }

        private bool CanAddRows(object obj)
        {
            return true;
        }

        private void AddRow(object obj)
        {
            AddInt = AddInt + 1;
            OtherMaterials.Add(new OtherItem { Name = ""});
            OtherLaborMaterials.Add(new OtherItem { Name = "" });

        }

        #endregion

        #region methods
        //Event handler to get JobSetup change updates.
        private void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                weatherWearType = js.WeatherWearType;
                totalSqft = js.TotalSqft;
                stairWidth = js.StairWidth;
                riserCount = js.RiserCount;
                deckPerimeter = js.DeckPerimeter;
                isPrevailingWage = js.IsPrevalingWage;
                isSpecialMetal = js.HasSpecialMaterial;
                isDiscounted = js.HasDiscount;
                isApprovedforCement = js.IsApprovedForSandCement;
            }
            FetchMaterialValuesAsync();
        }
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
                    case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                    case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                    case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                    case "CPC MEMBRANE":
                    case "NEOTEX-38 PASTE":
                    case "NEOTEX STANDARD POWDER(BODY COAT)":
                    case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                    case "RESISTITE LIQUID":
                    case "RESISTITE REGULAR GRAY":
                    case "RESISTITE REGULAR WHITE":
                    case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                    //case "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)":
                    case "LIP COLOR":
                    //case "AJ-44A DRESSING (SEALER)":
                    case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                    //case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                    //case "VISTA PAINT ACRIPOXY":
                    case "Stair Nosing From Dexotex":
                    //case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
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
                    case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                    case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                    case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                    case "CPC MEMBRANE":
                    case "NEOTEX STANDARD POWDER(BODY COAT)":
                    case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                    case "RESISTITE REGULAR WHITE":
                    case "LIP COLOR":
                    case "AJ-44A DRESSING(SEALER)":
                    case "VISTA PAINT ACRIPOXY":
                    case "STAIR NOSING FROM DEXOTEX":
                    case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                    case "WEATHER SEAL XL TWO COATS":
                         return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        private double CalculateHrs(double horzSft,double prodHor,double stairSqft,double prodStair)
        {
            double val1 = prodHor != 0 ? horzSft / prodHor : 0;
            double val2= prodStair != 0 ? stairSqft / prodStair : 0;
            return val1 + val2;
        }
        public ObservableCollection<SystemMaterial> GetSystemMaterial()
        {
            
            ObservableCollection<SystemMaterial> smP = new ObservableCollection<SystemMaterial>();
            int cov;
            double mp;
            double w;
            double lfArea;
            double setUpMin = 0; // Setup minimum charges from google sheet, col 6
            double pRateStairs = 0; ///Production rate stairs from google sheet, col 5
            double hprRate = 0;///Horizontal Production rate  from google sheet, col 4
            double vprRate = 0;///Vertical Production rate  from google sheet, col 1
            double sqh = 0;
            double labrExt = 0;
            double calcHrs = 0;
            double sqStairs = 0;
            double qty = 0;
            #region rehab
            if (weatherWearType=="Weather Wear Rehab")
            {
                int.TryParse(materialDetails[0][2].ToString(), out cov);
                double.TryParse(materialDetails[0][0].ToString(), out mp);
                double.TryParse(materialDetails[0][3].ToString(), out w);

                double.TryParse(materialDetails[0][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[0][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[0][4].ToString(), out hprRate);
                sqh= getSqFtAreaH("Light Crack Repair");
                calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
                lfArea = getlfArea("Light Crack Repair");
                
                smP.Add(new SystemMaterial
                {
                    IsWWR = true,
                    IsMaterialChecked = getCheckboxCheckStatus("Light Crack Repair"),
                    IsMaterialEnabled = getCheckboxEnabledStatus("Light Crack Repair"),
                    Name = "Light Crack Repair",
                    SMUnits = "Sq Ft",
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,
                   
                    SMSqftH=sqh,
                    Operation= "CAULK",
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = getSqFtStairs("Light Crack Repair"),
                    SetupMinCharge = setUpMin,
                    Hours= calcHrs,
                    
                    Qty = getQuantity("Light Crack Repair", cov, lfArea),

                });
                int.TryParse(materialDetails[1][2].ToString(), out cov);
                double.TryParse(materialDetails[1][0].ToString(), out mp);
                double.TryParse(materialDetails[1][3].ToString(), out w);
                lfArea = getlfArea("Large Crack Repair");

                double.TryParse(materialDetails[1][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[1][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[1][4].ToString(), out hprRate);
                calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
                sqh = getSqFtAreaH("Large Crack Repair");

                smP.Add(new SystemMaterial
                {
                    IsWWR = true,
                    IsMaterialChecked = getCheckboxCheckStatus("Large Crack Repair"),
                    IsMaterialEnabled = getCheckboxEnabledStatus("Large Crack Repair"),
                    Name = "Large Crack Repair",
                    SMUnits = "LF",
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,
                    Qty = getQuantity("Large Crack Repair", cov, lfArea),

                    SMSqftH = sqh,
                    //Operation = "CAULK",
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = getSqFtStairs("Large Crack Repair"),
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs

                });
                int.TryParse(materialDetails[2][2].ToString(), out cov);
                double.TryParse(materialDetails[2][0].ToString(), out mp);
                double.TryParse(materialDetails[2][3].ToString(), out w);
                lfArea = getlfArea("Bubble Repair(Measure Sq Ft)");
                double.TryParse(materialDetails[2][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[2][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[2][4].ToString(), out hprRate);
                sqh = getSqFtAreaH("Bubble Repair(Measure Sq Ft)");
                sqStairs = 0;
                calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
                smP.Add(new SystemMaterial
                {
                    IsWWR = true,
                    IsMaterialChecked = getCheckboxCheckStatus("Bubble Repair(Measure Sq Ft)"),
                    IsMaterialEnabled = getCheckboxEnabledStatus("Bubble Repair(Measure Sq Ft)"),
                    Name = "Bubble Repair(Measure Sq Ft)",
                    SMUnits = "Sq Ft",
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,
                    Qty = getQuantity("Bubble Repair(Measure Sq Ft)", cov, lfArea),
                    SMSqftH = sqh,
                    Operation = "CAULK",
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = getSqFtStairs("Bubble Repair(Measure Sq Ft)"),
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs

                });
            }
#endregion

            int.TryParse(materialDetails[3][2].ToString(), out cov);
            double.TryParse(materialDetails[3][0].ToString(), out mp);
            double.TryParse(materialDetails[3][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular Over Texture(#55 Bag)");
            double.TryParse(materialDetails[3][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[3][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[3][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Resistite Regular Over Texture(#55 Bag)");
            sqStairs = getSqFtStairs("Resistite Regular Over Texture(#55 Bag)");
            calcHrs = CalculateHrs(sqh,hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate:0;
            qty = getQuantity("Resistite Regular Over Texture(#55 Bag)", cov, lfArea);
            smP.Add(new SystemMaterial
            {

                IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Over Texture(#55 Bag)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Over Texture(#55 Bag)"),
                Name = "Resistite Regular Over Texture(#55 Bag)",
                SMUnits = "55 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "3/32 INCH THICK TROWEL DOWN",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = sqStairs,
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension=mp*qty

            });
            int.TryParse(materialDetails[4][2].ToString(), out cov);
            double.TryParse(materialDetails[4][0].ToString(), out mp);
            double.TryParse(materialDetails[4][3].ToString(), out w);
            lfArea = getlfArea("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale");
            double.TryParse(materialDetails[4][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[4][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[4][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale");
            sqStairs = getSqFtStairs("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "SLIP SHEET",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[5][2].ToString(), out cov);
            double.TryParse(materialDetails[5][0].ToString(), out mp);
            double.TryParse(materialDetails[5][3].ToString(), out w);
            lfArea = getlfArea("Rp Fabric 10 Inch Wide X (300 Lf) From Acme");
            double.TryParse(materialDetails[5][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[5][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[5][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Rp Fabric 10 Inch Wide X (300 Lf) From Acme");
            sqStairs = getSqFtStairs("Rp Fabric 10 Inch Wide X (300 Lf) From Acme");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Rp Fabric 10 Inch Wide X (300 Lf) From Acme", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "DETAIL STAIRS ONLY",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Rp Fabric 10 Inch Wide X (300 Lf) From Acme"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[6][2].ToString(), out cov);
            double.TryParse(materialDetails[6][0].ToString(), out mp);
            double.TryParse(materialDetails[6][3].ToString(), out w);
            lfArea = getlfArea("Glasmat #4 (1200 Sq Ft) From Acme");
            double.TryParse(materialDetails[6][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[6][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[6][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Glasmat #4 (1200 Sq Ft) From Acme");
            sqStairs = getSqFtStairs("Glasmat #4 (1200 Sq Ft) From Acme");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Glasmat #4 (1200 Sq Ft) From Acme", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "INSTALL FIELD GLASS",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Glasmat #4 (1200 Sq Ft) From Acme"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[7][2].ToString(), out cov);
            double.TryParse(materialDetails[7][0].ToString(), out mp);
            double.TryParse(materialDetails[7][3].ToString(), out w);
            double.TryParse(materialDetails[7][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[7][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[7][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Cpc Membrane");
            lfArea = getlfArea("Cpc Membrane");
            sqStairs = getSqFtStairs("Cpc Membrane");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Cpc Membrane", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "SATURATE GLASS & DETAIL PERIMETER",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Cpc Membrane"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[8][2].ToString(), out cov);
            double.TryParse(materialDetails[8][0].ToString(), out mp);
            double.TryParse(materialDetails[8][3].ToString(), out w);
            lfArea = getlfArea("Neotex-38 Paste");
            double.TryParse(materialDetails[8][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[8][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[8][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Neotex-38 Paste");
            sqStairs = getSqFtStairs("Neotex-38 Paste");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Neotex-38 Paste", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ADD TO BODY COAT",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Neotex-38 Paste"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[9][2].ToString(), out cov);
            double.TryParse(materialDetails[9][0].ToString(), out mp);
            double.TryParse(materialDetails[9][3].ToString(), out w);
            lfArea = getlfArea("Neotex Standard Powder(Body Coat)");
            double.TryParse(materialDetails[9][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[9][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[9][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Neotex Standard Powder(Body Coat)");
            sqStairs = getSqFtStairs("Neotex Standard Powder(Body Coat)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Neotex Standard Powder(Body Coat)", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Neotex Standard Powder(Body Coat)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Neotex Standard Powder(Body Coat)"),
                Name = "Neotex Standard Powder(Body Coat)",
                SMUnits = "45 LB BAG",
                SMSqft =lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Neotex Standard Powder(Body Coat)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[10][2].ToString(), out cov);
            double.TryParse(materialDetails[10][0].ToString(), out mp);
            double.TryParse(materialDetails[10][3].ToString(), out w);
            lfArea = getlfArea("Neotex Standard Powder(Body Coat) 1");
            double.TryParse(materialDetails[10][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[10][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[10][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Neotex Standard Powder(Body Coat) 1");
            sqStairs = getSqFtStairs("Neotex Standard Powder(Body Coat) 1");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Neotex Standard Powder(Body Coat) 1", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent=true,
                IsMaterialChecked = getCheckboxCheckStatus("Neotex Standard Powder(Body Coat) 1"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Neotex Standard Powder(Body Coat) 1"),
                Name = "Neotex Standard Powder(Body Coat) 1",
                SMUnits = "45 LB BAG",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Neotex Standard Powder(Body Coat) 1"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[11][2].ToString(), out cov);
            double.TryParse(materialDetails[11][0].ToString(), out mp);
            double.TryParse(materialDetails[11][3].ToString(), out w);
            lfArea = getlfArea("Resistite Liquid");
            double.TryParse(materialDetails[11][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[11][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[11][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Resistite Liquid");
            sqStairs = getSqFtStairs("Resistite Liquid");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Resistite Liquid", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ADD TO TAN FILLER",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Resistite Liquid"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            //int.TryParse(materialDetails[12][2].ToString(), out cov);
            //double.TryParse(materialDetails[12][0].ToString(), out mp);
            //double.TryParse(materialDetails[12][3].ToString(), out w);
            //lfArea = getlfArea("Resistite Regular Gray");
            //smP.Add(new SystemMaterial
            //{
            //    IsCheckboxDependent = true,
            //    IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Gray"),
            //    IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Gray"),
            //    Name = "Resistite Regular Gray",
            //    SMUnits = "55 LB BAG",
            //    SMSqft = lfArea,
            //    Coverage = cov,
            //    MaterialPrice = mp,
            //    Weight = w,
            //    Qty = getQuantity("Resistite Regular Gray", cov, lfArea),
                
            //});
            int.TryParse(materialDetails[13][2].ToString(), out cov);
            double.TryParse(materialDetails[13][0].ToString(), out mp);
            double.TryParse(materialDetails[13][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular White");
            double.TryParse(materialDetails[13][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[13][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[13][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Resistite Regular White");
            sqStairs = getSqFtStairs("Resistite Regular White");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Resistite Regular White", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Resistite Regular White"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            
            
            //int.TryParse(materialDetails[14][2].ToString(), out cov);
            //double.TryParse(materialDetails[14][0].ToString(), out mp);
            //double.TryParse(materialDetails[14][3].ToString(), out w);
            //lfArea=getlfArea("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)");
            //smP.Add(new SystemMaterial
            //{
            //    IsCheckboxDependent=true,
            //    IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"),
            //    IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"),
            //    Name = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)",
            //    SMUnits = "40 LB BAG",
            //    SMSqft = lfArea,
            //    Coverage =cov,
            //    MaterialPrice = mp,
            //    Weight = w,
            //    Qty = getQuantity("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)", cov, lfArea),

            //});
            int.TryParse(materialDetails[15][2].ToString(), out cov);
            double.TryParse(materialDetails[15][0].ToString(), out mp);
            double.TryParse(materialDetails[15][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular Or Smooth White(Knock Down Or Smooth)");
            double.TryParse(materialDetails[15][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[15][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[15][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Resistite Regular Or Smooth White(Knock Down Or Smooth)");
            sqStairs = getSqFtStairs("Resistite Regular Or Smooth White(Knock Down Or Smooth)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Resistite Regular Or Smooth White(Knock Down Or Smooth)", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Resistite Regular Or Smooth White(Knock Down Or Smooth)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[16][2].ToString(), out cov);
            double.TryParse(materialDetails[16][0].ToString(), out mp);
            double.TryParse(materialDetails[16][3].ToString(), out w);
            lfArea = getlfArea("Aj-44A Dressing(Sealer)");
            double.TryParse(materialDetails[16][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[16][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[16][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Aj-44A Dressing(Sealer)");
            sqStairs = getSqFtStairs("Aj-44A Dressing(Sealer)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Aj-44A Dressing(Sealer)", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent=true,
                IsMaterialChecked = false ,//getCheckboxCheckStatus("Aj-44A Dressing(Sealer)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Aj-44A Dressing(Sealer)"),
                Name = "Aj-44A Dressing(Sealer)",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ROLL 2 COATS",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Aj-44A Dressing(Sealer)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[17][2].ToString(), out cov);
            double.TryParse(materialDetails[17][0].ToString(), out mp);
            double.TryParse(materialDetails[17][3].ToString(), out w);
            lfArea = getlfArea("Vista Paint Acripoxy");
            double.TryParse(materialDetails[17][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[17][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[17][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Vista Paint Acripoxy");
            sqStairs = getSqFtStairs("Vista Paint Acripoxy");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Vista Paint Acripoxy", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent=true,
                IsMaterialChecked = false,
                IsMaterialEnabled = getCheckboxEnabledStatus("Vista Paint Acripoxy"),
                Name = "Vista Paint Acripoxy",
                SMUnits = "5 GAL PAIL",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ROLL 2 COATS",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Vista Paint Acripoxy"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[18][2].ToString(), out cov);
            double.TryParse(materialDetails[18][0].ToString(), out mp);
            double.TryParse(materialDetails[18][3].ToString(), out w);
            lfArea = getlfArea("Lip Color");
            double.TryParse(materialDetails[18][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[18][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[18][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Lip Color");
            sqStairs = getSqFtStairs("Lip Color");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Lip Color", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "ROLL 2 COATS",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Lip Color"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });

            int.TryParse(materialDetails[19][2].ToString(), out cov);
            double.TryParse(materialDetails[19][0].ToString(), out mp);
            double.TryParse(materialDetails[19][3].ToString(), out w);
            lfArea = getlfArea("Resistite Universal Primer(Add 50% Water)");
            double.TryParse(materialDetails[19][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[19][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[19][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Resistite Universal Primer(Add 50% Water)");
            sqStairs = getSqFtStairs("Resistite Universal Primer(Add 50% Water)");
            calcHrs =CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Resistite Universal Primer(Add 50% Water)", cov, lfArea);
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
                Qty =qty ,
                SMSqftH = sqh,
                Operation = "PRIMER: SPRAY OR ROLL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Resistite Universal Primer(Add 50% Water)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            //int.TryParse(materialDetails[20][2].ToString(), out cov);
            //double.TryParse(materialDetails[20][0].ToString(), out mp);
            //double.TryParse(materialDetails[20][3].ToString(), out w);
            //lfArea = getlfArea("Custom Texture Skip Trowel(Resistite Smooth Gray)");
            //smP.Add(new SystemMaterial
            //{
            //    IsCheckboxDependent=true,
            //    IsMaterialChecked = getCheckboxCheckStatus("Custom Texture Skip Trowel(Resistite Smooth Gray)"),
            //    IsMaterialEnabled = getCheckboxEnabledStatus("Custom Texture Skip Trowel(Resistite Smooth Gray)"),
            //    Name = "Custom Texture Skip Trowel(Resistite Smooth Gray)",
            //    SMUnits = "Sq Ft",
            //    SMSqft = lfArea,
            //    Coverage = cov,
            //    MaterialPrice = mp,
            //    Weight = w,
            //    Qty = getQuantity("Custom Texture Skip Trowel(Resistite Smooth Gray)", cov, lfArea),
                
            //});
            int.TryParse(materialDetails[21][2].ToString(), out cov);
            double.TryParse(materialDetails[21][0].ToString(), out mp);
            double.TryParse(materialDetails[21][3].ToString(), out w);
            lfArea = getlfArea("Custom Texture Skip Trowel(Resistite Smooth White)");
            double.TryParse(materialDetails[21][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[21][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[21][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Custom Texture Skip Trowel(Resistite Smooth White)");
            sqStairs = getSqFtStairs("Custom Texture Skip Trowel(Resistite Smooth White)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Custom Texture Skip Trowel(Resistite Smooth White)", cov, lfArea);
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
                SMSqftH = sqh,
                Operation = "TROWEL",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Custom Texture Skip Trowel(Resistite Smooth White)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                Qty = qty,
                FreightExtension = w * qty,
                MaterialExtension = mp * qty

            });
            int.TryParse(materialDetails[22][2].ToString(), out cov);
            double.TryParse(materialDetails[22][0].ToString(), out mp);
            double.TryParse(materialDetails[22][3].ToString(), out w);
            lfArea = getlfArea("Weather Seal XL two Coats");
            double.TryParse(materialDetails[22][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[22][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[22][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Weather Seal XL two Coats");
            sqStairs = getSqFtStairs("Weather Seal XL two Coats");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Weather Seal XL two Coats", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Weather Seal XL two Coats"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[23][2].ToString(), out cov);
            double.TryParse(materialDetails[23][0].ToString(), out mp);
            double.TryParse(materialDetails[23][3].ToString(), out w);
            double.TryParse(materialDetails[23][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[23][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[23][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Stair Nosing From Dexotex");
            lfArea = getlfArea("Stair Nosing From Dexotex");
            sqStairs = getSqFtStairs("Stair Nosing From Dexotex");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Stair Nosing From Dexotex", cov, lfArea);
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
                Qty = qty,
                SMSqftH = sqh,
                Operation = "NAIL OR SCREW",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Stair Nosing From Dexotex"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[24][2].ToString(), out cov);
            double.TryParse(materialDetails[24][0].ToString(), out mp);
            double.TryParse(materialDetails[24][3].ToString(), out w);
            lfArea = getlfArea("Extra Stair Nosing Lf");
            
            setUpMin = 0;
            double.TryParse(materialDetails[24][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[24][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Extra Stair Nosing Lf");
            sqStairs = getSqFtStairs("Extra Stair Nosing Lf"); //getvalue from systemMaterial
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
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
                SMSqftH = sqh,
                Operation = "NAIL OR SCREW",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Extra Stair Nosing Lf"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[25][2].ToString(), out cov);
            double.TryParse(materialDetails[25][0].ToString(), out mp);
            double.TryParse(materialDetails[25][3].ToString(), out w);
            lfArea = getlfArea("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
            double.TryParse(materialDetails[25][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[25][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[25][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
            sqStairs = getSqFtStairs("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
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
                SMSqftH = sqh,
                Operation = "Remove and replace dry rot",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });
            int.TryParse(materialDetails[26][2].ToString(), out cov);
            double.TryParse(materialDetails[26][0].ToString(), out mp);
            double.TryParse(materialDetails[26][3].ToString(), out w);
            lfArea = getlfArea("Stucco Material Remove And Replace (Lf)");
            double.TryParse(materialDetails[26][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[26][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[26][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("Stucco Material Remove And Replace (Lf)");
            sqStairs = getSqFtStairs("Stucco Material Remove And Replace (Lf)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
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
                SMSqftH = sqh,
                Operation = "Remove and replace dry rot",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Stucco Material Remove And Replace (Lf)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });

            
            return smP;
        }
        private void setCheckBoxes()
        {
            var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();
            SystemMaterial lipMat=null;
            foreach (SystemMaterial mat in materials)
            {
                if (mat.Name == "Lip Color")
                {
                    lipMat = mat;
                    mat.IsMaterialChecked = true;
                }
                if (mat.Name == "Aj-44A Dressing(Sealer)")
                {
                    mat.IsMaterialChecked = false;
                }
                if (mat.Name == "Vista Paint Acripoxy")
                {
                    mat.IsMaterialChecked = false;
                }
            }
            lipMat.IsMaterialChecked = true;
        }

        
        #region Material
        private double getQuantity(string materialName,double coverage,double lfArea)
        {
            switch (materialName.ToUpper())
            {
                case "LIGHT CRACK REPAIR":
                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                case "GLASMAT #4 (1200 SQ FT) FROM ACME":
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
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                case "WEATHER SEAL XL TWO COATS":
                    return lfArea / coverage;

                case "STAIR NOSING FROM DEXOTEX":
                    return lfArea * stairWidth;
                case "NEOTEX-38 PASTE":
                    return neotaxQty();
                case "RESISTITE LIQUID":
                    return calculateRLqty();
                default:
                    return 0;
            }
        }

        private double calculateRLqty()
        {
            double val1, val2, val3,val4;
            double.TryParse(materialDetails[13][2].ToString(), out val1);
            double.TryParse(materialDetails[15][2].ToString(), out val2);
            double.TryParse(materialDetails[21][2].ToString(), out val3);
            double.TryParse(materialDetails[3][2].ToString(), out val4);
            val1 = getQuantity("RESISTITE REGULAR WHITE", val1, getlfArea("RESISTITE REGULAR WHITE"));
            val2 = getQuantity("RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)", val2, getlfArea("RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)"));
            val3 = getQuantity("CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)", val3, getlfArea("CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)"));
            val4 = getQuantity("RESISTITE REGULAR OVER TEXTURE(#55 BAG)", val4, getlfArea("RESISTITE REGULAR OVER TEXTURE(#55 BAG)"));
            double qty=(val1+val2+val3)*0.33 + val4 / 5;
            return qty;
        }
       
        private double neotaxQty()
        {
            double val1, val2;
            double.TryParse(materialDetails[9][2].ToString(), out val1);
            double.TryParse(materialDetails[10][2].ToString(), out val2);
            val1 = getQuantity("NEOTEX STANDARD POWDER(BODY COAT)", val1, getlfArea("NEOTEX STANDARD POWDER(BODY COAT)"));
            val2 = getQuantity("NEOTEX STANDARD POWDER(BODY COAT) 1", val2, getlfArea("NEOTEX STANDARD POWDER(BODY COAT) 1"));
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
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                case "WEATHER SEAL XL TWO COATS":
                    return (riserCount * stairWidth * 2 ) +totalSqft;
                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    return (riserCount * 4 * 2) + totalSqft;//stairWidth=4
                case "STAIR NOSING FROM DEXOTEX":
                    return riserCount;
                case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                    return deckPerimeter + stairWidth * riserCount * 2;
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
                    TotalOCExtension = OtherMaterials.Select(x => x.Extension).Sum();
                }
            }

        }
        
        /// SubContract Cost
        private void CalSCTotal()
        {

            if (SubContractLaborItems.Count > 0)
            {
                if (SubContractLaborItems.Count > 0)
                {
                    ///sumtotal              
                   TotalSCExtension = SubContractLaborItems.Select(x => x.MaterialExtensionConlbrcst).Sum();
                }
            }

        }
        private void calculateMaterialTotals()
        {
            IEnumerable<SystemMaterial> systemMaterial = systemMaterials.Where(x => x.IsMaterialChecked == true && x.Qty > 0);
            if (systemMaterials.Count > 0)
            {
                SumQty = systemMaterials.Select(x => x.Qty).Sum();
                SumMatPrice = systemMaterials.Select(x => x.MaterialPrice).Sum();
                SumTotalMatExt = systemMaterials.Select(x => x.MaterialExtension).Sum();
                SumWeight = systemMaterials.Select(x => x.FreightExtension).Sum();
                //Total Freight
                        
                SumFreight = FreightCalculator(SumWeight);
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
            result = frCalc;
            return result;
        }
        //this is Material total
        private void CalculateCostBreakup()
        {
            
            IEnumerable<SystemMaterial> systemCBMaterial = systemMaterials.Where(x => x.IsMaterialChecked == true && x.Qty > 0);
            if (systemCBMaterial != null)
            {
                TotalMaterialCostbrkp = (SumTotalMatExt + TotalOCExtension + TotalSCExtension);
                TotalWeightbrkp = SumWeight;
                TotalFreightCostBrkp = FreightCalculator(TotalWeightbrkp);
                TotalSubContractLaborCostBrkp = TotalSCExtension;
            }
        }
        #endregion

        #region LaborSheet
        #region LaborSheet TotalProperties

        #endregion
        private double getSqFtAreaH(string materialName)
        {
            switch (materialName.ToUpper())
            {
                case "LIGHT CRACK REPAIR":
                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                case "CPC MEMBRANE":
                case "NEOTEX STANDARD POWDER(BODY COAT)":
                case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                case "RESISTITE REGULAR WHITE":
                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE REGULAR OR SMOOTH GRAY(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                case "VISTA PAINT ACRIPOXY":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                    return 1000;
                case "LARGE CRACK REPAIR":
                case "BUBBLE REPAIR MAJOR SQFT":
                case "NEOTEX-38 PASTE":
                case "EXTRA STAIR NOSING LF":
                case "RESISTITE LIQUID":
                case "RP FABRIC 10 INCH WIDE X(300 LF) FROM ACME":
                case "STAIR NOSING FROM DEXOTEX":
                      return 0;
                default:
                    return 0;
            }
        }
        private double getSqFtStairs(string materialName)
        {
            switch (materialName.ToUpper())
            {

                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                case "NEOTEX STANDARD POWDER(BODY COAT)":
                case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                case "RESISTITE REGULAR WHITE":
                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE REGULAR OR SMOOTH GRAY(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                case "VISTA PAINT ACRIPOXY":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                case "Rp Fabric 10 Inch Wide X (300 Lf) From Acme":
                    return stairWidth * riserCount * 2;
                case "LARGE CRACK REPAIR":
                case "LIGHT CRACK REPAIR":
                case "BUBBLE REPAIR MAJOR SQFT":
                case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                case "CPC MEMBRANE":
                case "RP FABRIC 10 INCH WIDE X(300 LF) FROM ACME":
                case "Glasmat #4 (1200 Sq Ft) From Acme":
                case "NEOTEX-38 PASTE":
                case "RESISTITE LIQUID":
                    return 0;// from System material


                case "Stair Nosing From Dexotex":
                    return riserCount * 3.5;
                case "Extra Stair Nosing Lf":
                    return 0;// To get from value system material
                default:
                    return 0;
            }
        }
        
        private void calculateLaborTotals()
        {
            double preWage = 0, laborDeduction = 0;
            if (isPrevailingWage)
            {
                double.TryParse(laborDetails[0][0].ToString(), out preWage);
            }
            if (isDiscounted)
            {
                double.TryParse(laborDetails[1][0].ToString(), out laborDeduction);
            }
            IEnumerable<SystemMaterial> selectedLabors = SystemMaterials.Where(x => x.IsMaterialChecked == true).ToList();
            TotalSetupTimeLabor = selectedLabors.Select(x => x.Hours).Sum();

            TotalLaborUnitPrice = selectedLabors.Select(x => x.LaborUnitPrice).Sum() * (1 + preWage + laborDeduction);
            TotalLaborExtension = selectedLabors.Select(x => x.LaborExtension).Sum() * (1 + preWage + laborDeduction);
            if (SlopeTotals!=null && MetalTotals!=null)
            {
                TotalSlopingPrice = getTotals(SlopeTotals.LaborExtTotal, SlopeTotals.MaterialExtTotal, SlopeTotals.MaterialFreightTotal, 0);
                TotalMetalPrice = getTotals(MetalTotals.LaborExtTotal, MetalTotals.MaterialExtTotal, MetalTotals.MaterialFreightTotal, 0);
            }
            
            TotalSystemPrice = getTotals(TotalLaborExtension, TotalMaterialCostbrkp, TotalFreightCostBrkp, TotalSubContractLaborCostBrkp);
            TotalSubcontractLabor = 0;
            TotalSale = TotalSlopingPrice + TotalMetalPrice + TotalSystemPrice+ TotalSubcontractLabor;

            AllTabsLaborTotal = SlopeTotals.LaborExtTotal + MetalTotals.LaborExtTotal + TotalLaborExtension;
            AllTabsMaterialTotal = SlopeTotals.MaterialExtTotal + MetalTotals.MaterialExtTotal + TotalMaterialCostbrkp;
            AllTabsFreightTotal = SlopeTotals.MaterialFreightTotal + MetalTotals.MaterialFreightTotal + TotalFreightCostBrkp;
            AllTabsSubContractTotal= SlopeTotals.SubContractLabor + MetalTotals.SubContractLabor + TotalSubContractLaborCostBrkp;

        }

        private double getTotals(double laborCost, double materialCost, double freightCost, double subcontractLabor)
        {
            double res = 0;
            double slopeTotal = laborCost;

            if (isPrevailingWage)
            {
                double.TryParse(laborDetails[4][0].ToString(), out res);
                slopeTotal = slopeTotal + laborCost * res;
            }
            else
            {
                double.TryParse(laborDetails[2][0].ToString(), out res);
                slopeTotal = slopeTotal + laborCost * res;
                double.TryParse(laborDetails[3][0].ToString(), out res);
                slopeTotal = slopeTotal + laborCost * res;
            }

            double.TryParse(laborDetails[5][0].ToString(), out res);
            slopeTotal = slopeTotal + laborCost * res;
            double.TryParse(laborDetails[6][0].ToString(), out res);
            double tax = res * (freightCost + materialCost) + materialCost + freightCost;//freight+material including tax

            slopeTotal = slopeTotal + tax;
            //subcontrctlabor
            double.TryParse(laborDetails[8][0].ToString(), out res);
            double subCLabor = subcontractLabor * res;
            //profitMargin
            double pmAdd;
            double.TryParse(laborDetails[8][0].ToString(), out pmAdd);
            double profitMarginAdd = (slopeTotal * pmAdd) * (1 + pmAdd);
            //profit margin
            double pm;
            double.TryParse(laborDetails[10][0].ToString(), out pm);
            double specialMetalDeduction = 0;
            if (isSpecialMetal)
            {
                //Profit deduct for special metal
                double.TryParse(laborDetails[9][0].ToString(), out res);
                specialMetalDeduction = materialCost * res;
            }
            double TotalCost = (slopeTotal / pm) + profitMarginAdd + specialMetalDeduction + subCLabor;

            double.TryParse(laborDetails[11][0].ToString(), out res);
            double generalLiability = TotalCost * res / pm;
            double.TryParse(laborDetails[12][0].ToString(), out res);
            double directExpense = TotalCost * res / pm;
            double.TryParse(laborDetails[13][0].ToString(), out res);
            double contigency = TotalCost * res / pm;
            double ins, fuel, addup;
            double.TryParse(laborDetails[14][0].ToString(), out ins);
            double.TryParse(laborDetails[15][0].ToString(), out fuel);
            double.TryParse(laborDetails[16][0].ToString(), out addup);
            double restTotal = TotalCost * (ins + fuel + addup);
            //calculated Profit Margin,currently not being used.
            double ProfitMargin = TotalCost - slopeTotal;
            return TotalCost + generalLiability + directExpense + contigency + restTotal;
        }
        #endregion
        #endregion
    }
}
