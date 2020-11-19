using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using WICR_Estimator.DBModels;
using WICR_Estimator.Models;
using WICR_Estimator.Services;

namespace WICR_Estimator.ViewModels
{
    [DataContract]
    [KnownType(typeof(Carlisle860MaterialViewModel))]
    [KnownType(typeof(DexellentIIMaterialViewModel))]
    [KnownType(typeof(WestcoatdualMaterialViewModel))]
    [KnownType(typeof(DeneefMaterialViewModel))]
    [KnownType(typeof(XypexMaterialViewModel))]
    [KnownType(typeof(DeneefMaterialViewModel))]
    [KnownType(typeof(DexoColorFlakeMaterialViewModel))]
    [KnownType(typeof(DualFlexMaterialViewModel))]
    [KnownType(typeof(WWRehabMaterialViewModel))]
    [KnownType(typeof(MaterialViewModel))]
    [KnownType(typeof(_201MaterialViewModel))]
    [KnownType(typeof(_250MaterialViewModel))]
    [KnownType(typeof(ALXMaterialViewModel))]
    [KnownType(typeof(DesertbrandMaterialViewModel))]
    [KnownType(typeof(DexoMaterialViewModel))]
    [KnownType(typeof(EnduroKoteMaterialViewModel))]
    [KnownType(typeof(MACoatMaterialViewModel))]
    [KnownType(typeof(MulticoatMaterialViewModel))]
    [KnownType(typeof(ParasealLGMaterialViewModel))]
    [KnownType(typeof(ParasealMaterialViewModel))]
    [KnownType(typeof(ParkingMaterialViewModel))]
    [KnownType(typeof(PedestrianMaterialViewModel))]
    [KnownType(typeof(PlideckMaterialViewModel))]
    [KnownType(typeof(WWResealMaterialViewModel))]
    [KnownType(typeof(ResistiteConcreteMaterialViewModel))]
    [KnownType(typeof(TufflexMaterialViewModel))]
    [KnownType(typeof(WestcoatColorMaterialViewModel))]
    [KnownType(typeof(UPIBelowTileMaterialViewModel))]
    [KnownType(typeof(IndependentMaterialViewModel))]
    public class MaterialBaseViewModel:BaseViewModel
    {
        [DataMember]
        public Totals MetalTotals { set; get; }
        [DataMember]
        public Totals SlopeTotals { set; get; }
        [DataMember]
        public double laborRate { get; set; }
        public double prPerc = 0;
        public double totalSqft;
        public double stairWidth;
        public double riserCount;
        public double deckPerimeter;
        public bool isApprovedforCement;
       
        public bool isPrevailingWage;
        public bool isDiscounted;
        public int deckCount;
        public bool hasContingencyDisc;
        [DataMember]
        public IList<IList<Object>> laborDetails { get; set; }
        [DataMember]
        public IList<IList<object>> materialDetails { get; set; }
        [DataMember]
        public IList<IList<object>> freightData { get; set; }


        #region privatefields
        private ObservableCollection<CostBreakup> lCostBreakUp;
        private ObservableCollection<SystemMaterial> systemMaterials;
        private ObservableCollection<OtherItem> otherMaterials;
        private ObservableCollection<OtherItem> otherLaborMaterials;
        private ObservableCollection<LaborContract> subContractLaborItems;
        private string weatherWearType;
        private bool isDataAvailable;
        private double markUpPerc;
        private double previousLaborCharges;
        private bool previousMinLaborCheckboxStatus=true;
        private bool isSpecialMetal;
        private bool hasSpecialPricing;
        
        
        private double costPerSquareFeet;
        private ICommand _addRowCommand;
        private ICommand _calculateCostCommand;
        private ICommand _removeCommand;
        private int AddInt = 4;
        private string projectname;

        #endregion

        public MaterialBaseViewModel()
        { }
        public double preWage = 0;
        public double actualPreWage = 0;
        public MaterialBaseViewModel(JobSetup Js)
        {

            SystemMaterials = new ObservableCollection<SystemMaterial>();
            OtherMaterials = new ObservableCollection<OtherItem>();

            OtherLaborMaterials = new ObservableCollection<OtherItem>();
            SubContractLaborItems = new ObservableCollection<LaborContract>();
            
            weatherWearType = "Weather Wear";
            IncludeDriveHours = true;
            //totalSqft = 1000;
            stairWidth = 4.5;
            //riserCount = 30;
            //deckPerimeter = 300;
            isSpecialMetal = false;
            isDiscounted = false;
            isPrevailingWage = false;
            isApprovedforCement = true;
            //deckCount = 1;
            projectname = Js.ProjectName;
            actualPreWage = Js.ActualPrevailingWage;
            if (!Js.IsProjectIndependent)
            {
                
                Js.JobSetupChange += JobSetup_OnJobSetupChange;
                SystemMaterial.OnQTyChanged += (s, e) => { setExceptionValues(s); };
                CheckboxCommand = new DelegateCommand(ApplyCheckUnchecks, canApply);
            }
            dbData = Js.dbData;
        }


        public MaterialBaseViewModel(Totals metalTotals, Totals slopeTotals,JobSetup Js)
            :this(Js)
        {

            MetalTotals = metalTotals;
            SlopeTotals = slopeTotals;
            if (MetalTotals!=null)
            {
                MetalTotals.OnTotalsChange += MetalTotals_OnTotalsChange;
            }

            if (SlopeTotals!=null)
            {
                SlopeTotals.OnTotalsChange += MetalTotals_OnTotalsChange;
            }
            
            //getDatafromGoogle(Js.ProjectName);          
            getDatafromDB(Js.ProjectName);
        }

        public virtual void setUnitChangeValues()
        {

        }
        public virtual double CalculateLabrExtn(double calhrs,double setupMin,string matName="")
        {
            return (calhrs != 0) ? (setupMin + calhrs) * laborRate : 0;
        }
        public virtual double getLaborUnitPrice(double laborExtension,double riserCount,
            double totalSqft, double sqftVert = 0, double sqftHor = 0,double sqftStairs = 0,string matName="")
        {
            return laborExtension / (riserCount + totalSqft);
        }
        
        public  virtual SystemMaterial getSMObject(int seq, string matName, string unit)
        {
            double cov;
            double mp;
            double w;
            double lfArea=0;
            double setUpMin = 0; // Setup minimum charges from google sheet, col 6
            double pRateStairs = 0; ///Production rate stairs from google sheet, col 5
            double hprRate = 0;///Horizontal Production rate  from google sheet, col 4
            double vprRate = 0;///Vertical Production rate  from google sheet, col 1
            double sqh = 0;
            double sqv = 0;
            double labrExt = 0;
            double calcHrs = 0;
            double sqStairs = 0;
            double qty = 0;
            string operation = "";
            try
            {
                if (isPrevailingWage)
                {
                    double.TryParse(freightData[5][0].ToString(), out prPerc);
                }
                else
                    prPerc = 0;

                //Get Material Data from DB
                ////MaterialDB matdb =  HTTPHelper.GetMaterialByName(matName);
                ////vprRate = matdb.ProdRateVertical;
                ////cov = matdb.Coverage;
                ////mp = matdb.MaterialPrice;
                ////w = matdb.Weight;
                ////setUpMin = matdb.LaborMinCharge;
                ////pRateStairs = matdb.ProdRateStair;
                ////hprRate = matdb.ProdRateHorizontal;
                //

                double.TryParse(materialDetails[seq][1].ToString(), out vprRate);
                double.TryParse(materialDetails[seq][2].ToString(), out cov);
                double.TryParse(materialDetails[seq][0].ToString(), out mp);
                double.TryParse(materialDetails[seq][3].ToString(), out w);

                double.TryParse(materialDetails[seq][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[seq][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[seq][4].ToString(), out hprRate);

                pRateStairs = pRateStairs * (1 + prPerc);
                hprRate = hprRate * (1 + prPerc);
                vprRate = vprRate * (1 + prPerc);

                sqv = getSqftAreaVertical(matName);
                sqh = getSqFtAreaH(matName);
                sqStairs = getSqFtStairs(matName);
                lfArea = getlfArea(matName);

                calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs, sqv, vprRate);

                labrExt = CalculateLabrExtn(calcHrs, setUpMin, matName);

                qty = getQuantity(matName, cov, lfArea);
                if (lfArea == -1)
                {
                    lfArea = qty;
                }
                if (sqh == -1)
                {
                    sqh = qty;
                }
                if (sqStairs == -1)
                {
                    sqStairs = qty;
                }
                operation = GetOperation(matName);
                return (new SystemMaterial
                {
                    Name = matName,
                    SMUnits = unit,
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,
                    Qty = qty,
                    SMSqftH = sqh,
                    Operation = operation,
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = sqStairs,
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs,
                    LaborExtension = labrExt,
                    VerticalProductionRate = vprRate,
                    LaborUnitPrice = getLaborUnitPrice(labrExt, riserCount, totalSqft, sqv, sqh, sqStairs, matName),//labrExt / (riserCount + totalSqft),
                    FreightExtension = w * qty,
                    MaterialExtension = mp * qty,  //chnage for independent projects
                    IsMaterialChecked = getCheckboxCheckStatus(matName),
                    IsMaterialEnabled = getCheckboxEnabledStatus(matName),
                    IncludeInLaborMinCharge = IncludedInLaborMin(matName),
                    IsCheckboxDependent= GetCheckBoxDependency(matName),
                    AllEditable = getEditable()
                });
            }
            catch (Exception e)
            {
                return null;
                
            }
            
        }
        public virtual bool getEditable()
        {
            return false;
        }

        public virtual string GetOperation(string matName)
        {
            return matName;
        }

        public virtual bool IncludedInLaborMin(string matName)
        {
            return true;
        }

        public void  MetalTotals_OnTotalsChange(object sender, EventArgs e)
        {
            Totals tabTotals = sender as Totals;
            if (tabTotals != null)
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
                BaseViewModel.IsDirty = true;
            }
            CalculateCost(null);

        }
        

        #region Material Properties
        [IgnoreDataMember]
        public DelegateCommand CheckboxCommand { get; set; }

        [DataMember]
        public ObservableCollection<LaborContract> SubContractLaborItems
        {
            get { return subContractLaborItems; }
            set
            {
                if (value != subContractLaborItems)
                {
                    subContractLaborItems = value;
                    OnPropertyChanged("SubContractLaborItems");
                }
            }
        }
        [DataMember]
        public ObservableCollection<SystemMaterial> SystemMaterials
        {
            get
            {
                return systemMaterials;
            }
            set
            {
                if (value != systemMaterials)
                {
                    systemMaterials = value;
                    OnPropertyChanged("SystemMaterials");

                }
            }
        }
        [DataMember]
        public ObservableCollection<OtherItem> OtherMaterials
        {
            get { return otherMaterials; }
            set
            {
                if (value != otherMaterials)
                {
                    otherMaterials = value;
                    OnPropertyChanged("OtherMaterials");
                    CalOCTotal();
                }
            }
        }
        [DataMember]
        public double CostPerSquareFeet
        {
            get { return costPerSquareFeet; }
            set
            {
                if (value != costPerSquareFeet)
                {
                    costPerSquareFeet = value;
                    OnPropertyChanged("CostPerSquareFeet");
                }
            }
        }
        private double sumFreight;
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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


        private double subContractMarkup;
        [DataMember]
        public double SubContractMarkup
        {
            get { return subContractMarkup; }
            set
            {
                if (value != subContractMarkup)
                {
                    subContractMarkup = value;
                    populateCalculation();
                    OnPropertyChanged("SubContractMarkup");

                }
            }
        }
        [DataMember]
        
        private double subContractprofitMargin;
        [DataMember]
        public double SubContractProfitMargin
        {
            get { return subContractprofitMargin; }
            set
            {
                if (value != subContractprofitMargin)
                {
                    subContractprofitMargin = value;
                    populateCalculation();
                    OnPropertyChanged("SubContractProfitMargin");

                }
            }
        }

        private double metalMarkup;
        [DataMember]
        public double MetalMarkup
        {
            get { return metalMarkup; }
            set
            {
                if (value != metalMarkup)
                {
                    metalMarkup = value;
                    populateCalculation();
                    OnPropertyChanged("MetalMarkup");

                }
            }
        }
        private double slopeMarkup;
        [DataMember]
        public double SlopeMarkup
        {
            get { return slopeMarkup; }
            set
            {
                if (value != slopeMarkup)
                {
                    slopeMarkup = value;
                    populateCalculation();
                    OnPropertyChanged("SlopeMarkup");

                }
            }
        }
        private double materialMarkup;
        [DataMember]
        public double MaterialMarkup
        {
            get { return materialMarkup; }
            set
            {
                if (value != materialMarkup)
                {
                    materialMarkup = value;
                    populateCalculation();
                    OnPropertyChanged("MaterialMarkup");

                }
            }
        }
        #endregion

        #region Labor Properties
        

        private bool addLaborMinCharge;
        [DataMember]
        public bool ZAddLaborMinCharge
        {
            get { return addLaborMinCharge; }

            set
            {
                //if (value != addLaborMinCharge)
                //{
                    addLaborMinCharge = value;
                    previousMinLaborCheckboxStatus = addLaborMinCharge;
                    OnPropertyChanged("ZAddLaborMinCharge");
                    
                //}
            }
        }
        [DataMember]
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
        [DataMember]
        public ObservableCollection<CostBreakup> LCostBreakUp
        {
            get { return lCostBreakUp; }
            set
            {
                if (value != lCostBreakUp)
                {
                    lCostBreakUp = value;
                    OnPropertyChanged("LCostBreakUp");
                }
            }
        }
        [DataMember]
        public double CostperSqftSlope { get; set; }
        [DataMember]
        public double CostperSqftMetal { get; set; }
        [DataMember]
        public double CostperSqftMaterial { get; set; }
        [DataMember]
        public double CostperSqftSubContract { get; set; }
        [DataMember]
        public double TotalCostperSqft { get; set; }
        [DataMember]
        public double TotalHrsLabor { get; set; }
        [DataMember]
        public double TotalHrsSystemLabor { get; set; }
        [DataMember]
        public double TotalHrsMetalLabor { get; set; }
        [DataMember]
        public double TotalHrsSlopeLabor { get; set; }
        //public double TotalHrsFreightLabor { get; set; }
        [DataMember]
        public double TotalHrsDriveLabor { get; set; }
        [DataMember]
        public double TotalLaborUnitPrice { get; set; }
        [DataMember]
        public double TotalSetupTimeLabor { get; set; }
        [DataMember]
        public double TotalLaborExtension { get; set; }
        [DataMember]
        public double TotalSlopingPrice { get; set; }
        [DataMember]
        public double TotalMetalPrice { get; set; }
        [DataMember]
        public double TotalSystemPrice { get; set; }
        [DataMember]
        public double TotalSubcontractLabor { get; set; }
        [DataMember]
        public double TotalSale { get; set; }
        [DataMember]

        public double AllTabsLaborTotal { get; set; }
        [DataMember]
        public double AllTabsMaterialTotal { get; set; }
        [DataMember]
        public double AllTabsFreightTotal { get; set; }
        [DataMember]
        public double AllTabsSubContractTotal { get; set; }
        [DataMember]
        public double LaborMinChargeHrs { get; set; }
        [DataMember]
        public double LaborMinChargeMinSetup { get; set; }
        [DataMember]
        public double LaborMinChargeLaborExtension { get; set; }
        [DataMember]
        public double LaborMinChargeLaborUnitPrice { get; set; }
        [DataMember]
        public double MarkUpPerc
        {
            get { return markUpPerc; }
            set
            {
                if (value != markUpPerc)
                {
                    markUpPerc = value;
                    OnPropertyChanged("MarkUpPerc");
                }
            }
        }
        

        public virtual void calculateLaborHrs()
        {
            calLaborHrs(10,totalSqft);
        }
        public virtual void driveHrs(int hrs,double tSqft)
        {
            TotalHrsDriveLabor = tSqft < 1001 ? hrs : Math.Ceiling(tSqft / 1000 ) * hrs;
            
            //DriveLaborValue = Math.Round(TotalHrsDriveLabor * laborRate,2);
            //OnPropertyChanged("DriveLaborValue");
            OnPropertyChanged("TotalHrsDriveLabor");
        }
        double tempTotalLaborExtension = 0;
        public void calLaborHrs(int hrs,double tSqft)
        {
            if ( laborRate == 0)
            {
                return;
            }

            TotalHrsSystemLabor = isPrevailingWage ? (tempTotalLaborExtension / actualPreWage) :  //change TotalLaborExtension with private field
                                                  (tempTotalLaborExtension / laborRate) ;
            if (TotalHrsSystemLabor < 0)
            {
                TotalHrsSystemLabor = 0;
            }
            OnPropertyChanged("TotalHrsSystemLabor");
            if (SlopeTotals != null )
            {
                
                TotalHrsSlopeLabor = isPrevailingWage ? (SlopeTotals.LaborExtTotal / actualPreWage) :
                                                      (SlopeTotals.LaborExtTotal / laborRate);
                OnPropertyChanged("TotalHrsSlopeLabor");
            }
            if (MetalTotals!=null)
            {
                TotalHrsMetalLabor = isPrevailingWage ? (MetalTotals.LaborExtTotal / actualPreWage) :
                                                  (MetalTotals.LaborExtTotal / laborRate);
                OnPropertyChanged("TotalHrsMetalLabor");
            }
            TotalHrsDriveLabor = Math.Ceiling((TotalHrsSystemLabor + TotalHrsMetalLabor + TotalHrsSlopeLabor) / 10 * 2);
            if (TotalHrsDriveLabor<4)
            {
                TotalHrsDriveLabor = 4;
            }
            double DHLR = 0;
            if (freightData != null)
            {
                double.TryParse(freightData[6][0].ToString(), out DHLR);
            }
            else
                DHLR=dbData.FreightDBData.First(x => x.FactorName == "DriveHourLaborRate").FactorValue;
            

            DriveLaborValue =IncludeDriveHours ? TotalHrsDriveLabor * DHLR:0;

            TotalHrsLabor = TotalHrsSystemLabor + TotalHrsMetalLabor + TotalHrsSlopeLabor;
           
            OnPropertyChanged("DriveLaborValue");
            OnPropertyChanged("TotalHrsDriveLabor");
            OnPropertyChanged("TotalHrsLabor");

        }
        
        #endregion

        


        #region commands
        public virtual bool canApply(object obj)
        {
            //if (obj != null)
            //{
            //    if (obj.ToString() == "Lip Color" || obj.ToString() == "Vista Paint Acripoxy" || obj.ToString() == "Aj-44A Dressing(Sealer)")
            //    {
            //        return true;
            //    }
            //    else
            //        return getCheckboxCheckStatus(obj.ToString());
            //}
            //else
                return true;
            
        }

        public virtual void ApplyCheckUnchecks(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (obj.ToString() == "Lip Color")
            {
                #region ifLipcolor
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

                    if (mat.Name == "Resistite Regular Gray" ||mat.Name== "Resistite Regular White")
                    {
                        if (dbData == null)
                        {
                            mat.Name = "Resistite Regular White";
                            double matVal = 0;
                            double.TryParse(materialDetails[13][0].ToString(), out matVal);
                            mat.MaterialPrice = matVal;
                            double.TryParse(materialDetails[13][3].ToString(), out matVal);
                            mat.Weight = matVal;
                            double.TryParse(materialDetails[13][4].ToString(), out matVal);
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[13][5].ToString(), out matVal);
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[13][6].ToString(), out matVal);
                            mat.SetupMinCharge = matVal;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        else
                        {
                            mat.Name = "Resistite Regular White";
                            mat.MaterialPrice = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).MaterialPrice;

                            mat.Weight = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).Weight;
                            double matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateHorizontal;
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateStair;

                            mat.StairsProductionRate = matVal * (1 + prPerc);

                            mat.SetupMinCharge = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).LaborMinCharge;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        

                    }
                    if (mat.Name == "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"|| mat.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)")
                    {
                        if (dbData==null)
                        {
                            mat.Name = "Resistite Regular Or Smooth White(Knock Down Or Smooth)";
                            double matVal = 0;
                            double.TryParse(materialDetails[15][0].ToString(), out matVal);
                            mat.MaterialPrice = matVal;
                            double.TryParse(materialDetails[15][3].ToString(), out matVal);
                            mat.Weight = matVal;
                            double.TryParse(materialDetails[15][4].ToString(), out matVal);
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[15][5].ToString(), out matVal);
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[15][6].ToString(), out matVal);
                            mat.SetupMinCharge = matVal;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        else
                        {
                            mat.Name = "Resistite Regular Or Smooth White(Knock Down Or Smooth)";
                            mat.MaterialPrice = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).MaterialPrice;

                            mat.Weight = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).Weight;
                            double matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateHorizontal;
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateStair;

                            mat.StairsProductionRate = matVal * (1 + prPerc);

                            mat.SetupMinCharge = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).LaborMinCharge;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        
                    }
                    if (mat.Name == "Custom Texture Skip Trowel(Resistite Smooth Gray)"|| mat.Name == "Custom Texture Skip Trowel(Resistite Smooth White)")
                    {
                        if (dbData==null)
                        {
                            mat.Name = "Custom Texture Skip Trowel(Resistite Smooth White)";
                            double matVal = 0;
                            double.TryParse(materialDetails[21][0].ToString(), out matVal);
                            mat.MaterialPrice = matVal;
                            double.TryParse(materialDetails[21][3].ToString(), out matVal);
                            mat.Weight = matVal;
                            double.TryParse(materialDetails[21][4].ToString(), out matVal);
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[21][5].ToString(), out matVal);
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[21][6].ToString(), out matVal);
                            mat.SetupMinCharge = matVal;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        else
                        {
                            mat.Name = "Custom Texture Skip Trowel(Resistite Smooth White)";
                            mat.MaterialPrice = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).MaterialPrice;

                            mat.Weight = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).Weight;
                            double matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateHorizontal;
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateStair;

                            mat.StairsProductionRate = matVal * (1 + prPerc);

                            mat.SetupMinCharge = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).LaborMinCharge;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        
                    }

                }
                #endregion
            }
            if (obj.ToString() == "Vista Paint Acripoxy")
            {
                #region ifVista
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
                        if (dbData==null)
                        {
                            mat.Name = "Resistite Regular Gray";
                            double matVal = 0;
                            double.TryParse(materialDetails[12][0].ToString(), out matVal);
                            mat.MaterialPrice = matVal;
                            double.TryParse(materialDetails[12][3].ToString(), out matVal);
                            mat.Weight = matVal;
                            double.TryParse(materialDetails[12][4].ToString(), out matVal);
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[12][5].ToString(), out matVal);
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[12][6].ToString(), out matVal);
                            mat.SetupMinCharge = matVal;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        else
                        {

                            mat.Name = "Resistite Regular Gray";
                            mat.MaterialPrice = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).MaterialPrice;

                            mat.Weight = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).Weight;
                            double matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateHorizontal;
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateStair;

                            mat.StairsProductionRate = matVal * (1 + prPerc);

                            mat.SetupMinCharge = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).LaborMinCharge;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        
                    }
                    if (mat.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)")
                    {
                        if (dbData==null)
                        {
                            mat.Name = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)";
                            double matVal = 0;
                            double.TryParse(materialDetails[14][0].ToString(), out matVal);
                            mat.MaterialPrice = matVal;
                            double.TryParse(materialDetails[14][3].ToString(), out matVal);
                            mat.Weight = matVal;
                            double.TryParse(materialDetails[14][4].ToString(), out matVal);
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[14][5].ToString(), out matVal);
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[14][6].ToString(), out matVal);
                            mat.SetupMinCharge = matVal;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        else
                        {
                            mat.Name = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)";
                            mat.MaterialPrice = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).MaterialPrice;

                            mat.Weight = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).Weight;
                            double matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateHorizontal;
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateStair;

                            mat.StairsProductionRate = matVal * (1 + prPerc);

                            mat.SetupMinCharge = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).LaborMinCharge;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        
                    }
                    if (mat.Name == "Custom Texture Skip Trowel(Resistite Smooth White)")
                    {
                        if (dbData==null)
                        {
                            mat.Name = "Custom Texture Skip Trowel(Resistite Smooth Gray)";
                            double matVal = 0;
                            double.TryParse(materialDetails[20][0].ToString(), out matVal);
                            mat.MaterialPrice = matVal;
                            double.TryParse(materialDetails[20][3].ToString(), out matVal);
                            mat.Weight = matVal;
                            double.TryParse(materialDetails[20][4].ToString(), out matVal);
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[20][5].ToString(), out matVal);
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[20][6].ToString(), out matVal);
                            mat.SetupMinCharge = matVal;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        else
                        {
                            mat.Name = "Custom Texture Skip Trowel(Resistite Smooth Gray)";
                            mat.MaterialPrice = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).MaterialPrice;

                            mat.Weight = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).Weight;
                            double matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateHorizontal;
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateStair;

                            mat.StairsProductionRate = matVal * (1 + prPerc);

                            mat.SetupMinCharge = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).LaborMinCharge;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                    }
                }
                #endregion
            }
            if (obj.ToString() == "Aj-44A Dressing(Sealer)")
            {
                #region Aj44
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
                        if (dbData==null)
                        {
                            mat.Name = "Resistite Regular Gray";
                            double matVal = 0;
                            double.TryParse(materialDetails[12][0].ToString(), out matVal);
                            mat.MaterialPrice = matVal;
                            double.TryParse(materialDetails[12][3].ToString(), out matVal);
                            mat.Weight = matVal;
                            double.TryParse(materialDetails[12][4].ToString(), out matVal);
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[12][5].ToString(), out matVal);
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[12][6].ToString(), out matVal);
                            mat.SetupMinCharge = matVal;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        else
                        {
                            mat.Name = "Resistite Regular Gray";
                            mat.MaterialPrice = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == "RESISTITE REGULAR GRAY").MaterialPrice; 
                            
                            mat.Weight = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == "RESISTITE REGULAR GRAY").Weight; 
                            double matVal= dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == "RESISTITE REGULAR GRAY").ProdRateHorizontal;
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == "RESISTITE REGULAR GRAY").ProdRateStair;
                            
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            
                            mat.SetupMinCharge = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == "RESISTITE REGULAR GRAY").LaborMinCharge ;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                    }
                    if (mat.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)")
                    {
                        if (dbData==null)
                        {
                            mat.Name = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)";
                            double matVal = 0;
                            double.TryParse(materialDetails[14][0].ToString(), out matVal);
                            mat.MaterialPrice = matVal;
                            double.TryParse(materialDetails[14][3].ToString(), out matVal);
                            mat.Weight = matVal;
                            double.TryParse(materialDetails[14][4].ToString(), out matVal);
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[14][5].ToString(), out matVal);
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[14][6].ToString(), out matVal);
                            mat.SetupMinCharge = matVal;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        else
                        {
                            mat.Name = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)";
                            mat.MaterialPrice = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).MaterialPrice;

                            mat.Weight = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).Weight;
                            double matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateHorizontal;
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateStair;

                            mat.StairsProductionRate = matVal * (1 + prPerc);

                            mat.SetupMinCharge = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).LaborMinCharge;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                        }
                        
                    }
                    if (mat.Name == "Custom Texture Skip Trowel(Resistite Smooth White)")
                    {
                        if (dbData==null)
                        {
                            mat.Name = "Custom Texture Skip Trowel(Resistite Smooth Gray)";
                            double matVal = 0;
                            double.TryParse(materialDetails[20][0].ToString(), out matVal);
                            mat.MaterialPrice = matVal;
                            double.TryParse(materialDetails[20][3].ToString(), out matVal);
                            mat.Weight = matVal;
                            double.TryParse(materialDetails[20][4].ToString(), out matVal);
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[20][5].ToString(), out matVal);
                            mat.StairsProductionRate = matVal * (1 + prPerc);
                            double.TryParse(materialDetails[20][6].ToString(), out matVal);
                            mat.SetupMinCharge = matVal;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                            mat.StairSqft = getSqFtStairs(mat.Name);
                        }
                        else
                        {
                            mat.Name = "Custom Texture Skip Trowel(Resistite Smooth Gray)";
                            mat.MaterialPrice = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).MaterialPrice;

                            mat.Weight = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).Weight;
                            double matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateHorizontal;
                            mat.HorizontalProductionRate = matVal * (1 + prPerc);
                            matVal = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).ProdRateStair;

                            mat.StairsProductionRate = matVal * (1 + prPerc);

                            mat.SetupMinCharge = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == mat.Name.ToUpper()).LaborMinCharge;
                            OnPropertyChanged("SetupMinCharge");
                            mat.Hours = CalculateHrs(mat.SMSqftH, mat.HorizontalProductionRate, mat.StairSqft, mat.StairsProductionRate);
                            mat.LaborExtension = (mat.Hours != 0) ? (mat.SetupMinCharge + mat.Hours) * laborRate : 0;
                            mat.LaborUnitPrice = mat.LaborExtension / (riserCount + totalSqft);
                            mat.StairSqft = getSqFtStairs(mat.Name);
                        }
                        
                    }

                }
                #endregion
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

            SystemMaterials.Where(x=>x.Name== "Neotex-38 Paste").FirstOrDefault().Qty=neotaxQty();
            //SystemMaterials.Where(x => x.Name == "Neotex-38 Paste").FirstOrDefault().IsMaterialChecked = true;
        
            if (obj.ToString() == "Resistite Regular Gray"|| obj.ToString() == "Resistite Regular White")
            {
                var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();
                bool checkStatus = false;
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
                    if (item.Name == "Resistite Liquid")
                    {
                        item.IsMaterialChecked = checkStatus;
                    }
                    if (item.Name == "Resistite Regular Or Smooth White(Knock Down Or Smooth)" || item.Name == "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)")
                    {
                        item.IsMaterialChecked = checkStatus;
                    }
                }

            }
            if (obj.ToString()== "Custom Texture Skip Trowel(Resistite Smooth Gray)"||
                obj.ToString()== "Custom Texture Skip Trowel(Resistite Smooth White)"||
                obj.ToString()== "Resistite Regular Over Texture(#55 Bag)" ||
                obj.ToString()== "Resistite Regular Gray")
            {
                calculateRLqty();
            }           
        }

        #endregion
        //Get data from googlesheets
        private void getDatafromGoogle(string prjName)
        {
            if (prjName.Contains('.'))
            {
                prjName = prjName.Split('.')[0];
            }

            double rate;
            isDataAvailable = true;
            if (materialDetails == null)
            {
                //materialDetails = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "H33:K59");
                
                GSData gData = DataSerializer.DSInstance.deserializeGoogleData(prjName);
                laborDetails = gData.LaborData;
                materialDetails = gData.MaterialData;
                freightData = gData.FreightData;
                double.TryParse(gData.LaborRate[0][0].ToString(), out rate);
                laborRate = rate;
                double facVal = 0;
                double.TryParse(laborDetails[7][0].ToString(), out facVal);
                SubContractMarkup = facVal;
                double.TryParse(laborDetails[17][0].ToString(), out facVal);
                MetalMarkup = 1-facVal;
                double.TryParse(laborDetails[19][0].ToString(), out facVal);
                SlopeMarkup = 1-facVal;
                double.TryParse(laborDetails[18][0].ToString(), out facVal);
                MaterialMarkup = 1-facVal;

                double facValue = 0;
                double.TryParse(laborDetails[20][0].ToString(), out facValue);

                SubContractProfitMargin = facValue;

            }
        }


        public virtual void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

                      
            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "Stucco Material Remove And Replace (Lf)" || item.Name == "Plywood 3/4 & Blocking(# Of 4X8 Sheets)" ||
                    item.Name == "Extra Stair Nosing Lf" || item.Name == "Bubble Repair(Measure Sq Ft)" || item.Name == "Large Crack Repair")
                {
                    qtyList.Add(item.Name, item.Qty);
                }

            }
          
            var sysMat = GetSystemMaterial();

            if (hasSetupChanged)
            {
                for (int i = 0; i < SystemMaterials.Count; i++)
                {

                    double sp = SystemMaterials[i].SpecialMaterialPricing;
                    bool iscbChecked = SystemMaterials[i].IsMaterialChecked;
                    bool iscbEnabled = SystemMaterials[i].IsMaterialEnabled;
                    SystemMaterials[i] = sysMat[i];

                    SystemMaterials[i].SpecialMaterialPricing = sp;
                    SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                    SystemMaterials[i].IsMaterialChecked = iscbChecked;
                    if (SystemMaterials[i].Name == "Stucco Material Remove And Replace (Lf)" || SystemMaterials[i].Name == "Plywood 3/4 & Blocking(# Of 4X8 Sheets)" ||
                    SystemMaterials[i].Name == "Extra Stair Nosing Lf" || SystemMaterials[i].Name == "Bubble Repair(Measure Sq Ft)"
                            || SystemMaterials[i].Name == "Large Crack Repair")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            SystemMaterials[i].Qty = qtyList[SystemMaterials[i].Name];

                        }
                    }

                }

            }
            else
            {
                SystemMaterials = sysMat;
                setCheckBoxes();
            }

            foreach (var mat in SystemMaterials)
            {
                if (mat.Name == "Lip Color" || mat.Name == "Aj-44A Dressing(Sealer)" || mat.Name == "Vista Paint Acripoxy")
                {
                    if (mat.IsMaterialChecked)
                    {
                        ApplyCheckUnchecks(mat.Name);
                        break;
                    }
                }
            }

            setExceptionValues(null);
            
            calculateRLqty();

            if (OtherMaterials.Count == 0)
            {
                OtherMaterials = GetOtherMaterials();

                OtherLaborMaterials = OtherMaterials; 
            }


            if (SubContractLaborItems.Count == 0)
            {
                SubContractLaborItems = GetLaborItems();
            }
            
            //CalculateAllMaterial();
            //CalculateLaborMinCharge(hasSetupChanged);
            //calculateLaborTotalsWithMinLabor();
            CalculateCost(null);
            
        }

        public void CalculateAllMaterial()
        {
            calculateMaterialTotals();
            CalOCTotal();
            calculateLaborTotals();
            
            calculateLaborHrs();
            CalculateCostBreakup();            
            populateCalculation();
        }
       

        public virtual void setExceptionValues(object s)
        {
            if (s!=null)
            {
                SystemMaterial item = SystemMaterials.Where(x => x.Name == s.ToString()).FirstOrDefault();
                if (item!=null)
                {
                    item.IsMaterialChecked = item.Qty > 0 ? true : false;
                }
                
            }

            if (SystemMaterials.Count != 0)
            {
                SystemMaterial item = SystemMaterials.Where(x => x.Name == "Extra stair nosing lf").FirstOrDefault();
                if (item!=null)
                {
                    item.StairSqft = item.Qty;
                    item.Hours = CalculateHrs(0, 0, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = (item.Hours + item.SetupMinCharge) * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                    
                }

                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
                if (item!=null)
                {
                    item.SMSqftH = item.Qty * 32;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }

                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
                if (item!=null)
                {
                    item.SMSqftH = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                
                //=======================================
                item = SystemMaterials.Where(x => x.Name == "Extra Stair Nosing Lf").FirstOrDefault();
                if (item!=null)
                {
                    item.StairSqft = item.Qty;
                    item.Hours = CalculateHrs(0, 0, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = (item.Hours + item.SetupMinCharge) * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);
                }
                

                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & Blocking(# Of 4X8 Sheets)").FirstOrDefault();
                if (item!=null)
                {
                    item.SMSqftH = item.Qty * 32;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                

                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove And Replace (Lf)").FirstOrDefault();
                if (item!=null)
                {
                    item.SMSqftH = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);
                    item.LaborExtension = item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                
            }

        }
        public virtual ObservableCollection<OtherItem> GetOtherMaterials()
        {
            ObservableCollection<OtherItem> om = new ObservableCollection<OtherItem>();
            om.Add(new OtherItem { Name = "Access issues?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional prep?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional labor?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Alternate material?", IsReadOnly = false });
            om.Add(new OtherItem { Name = "Additional Move ons?", IsReadOnly = false });

            return om;
        }
        
        public virtual ObservableCollection<LaborContract> GetLaborItems()
        {
            ObservableCollection<LaborContract> SC = new ObservableCollection<LaborContract>();
            SC.Add(new LaborContract { Name = "" });
            SC.Add(new LaborContract { Name = "" });
            SC.Add(new LaborContract { Name = "" });
            SC.Add(new LaborContract { Name = "" });
            SC.Add(new LaborContract { Name = "" });
            return SC;
        }

        #region IcommandSection
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

        private OtherItem newItem;


        private void AddRow(object obj)
        {
            AddInt = AddInt + 1;
            newItem = new OtherItem { Name = "" };
            OtherMaterials.Add(newItem);
            //OtherLaborMaterials.Add(newItem);
        }
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
                //OtherLaborMaterials.RemoveAt(AddInt);
                AddInt = AddInt - 1;
            }
        }
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
        double previousSale = 0;
        public void CalculateCost(object obj)
        {
            
            calculateMaterialTotals();
            CalOCTotal();
            CalSCTotal();

            calculateLaborTotals();

            calculateLaborHrs();
            calculateLaborTotals();
            
            CalculateCostBreakup();
            
            
            CalculateLaborMinCharge(false);
            calculateLaborTotalsWithMinLabor();
            populateCalculation();
            if (TotalSale!=previousSale)
            {
                BaseViewModel.IsDirty = true;
                previousSale = TotalSale;
            }
        }

        

        private bool CanAddRows(object obj)
        {
            return true;
        }


        #endregion

        #region methods
        //Event handler to get JobSetup change updates.
        public virtual void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            //Task task = new Task(() => 
            //{
                JobSetup js = sender as JobSetup;
                if (js != null)
                {
                    //weatherWearType = js.WeatherWearType;
                    totalSqft = js.TotalSqft;
                    stairWidth = js.StairWidth;
                    riserCount = js.RiserCount;
                    deckPerimeter = js.DeckPerimeter;
                    isPrevailingWage = js.IsPrevalingWage;
                    hasSpecialPricing = js.HasSpecialPricing;
                    isDiscounted = js.HasDiscount;
                    isApprovedforCement = js.IsApprovedForSandCement;
                    if (isPrevailingWage)
                    {
                        preWage = js.ActualPrevailingWage == 0 ? 0 : (js.ActualPrevailingWage - laborRate) / laborRate;
                    }
                    else
                        preWage = 0;
                    hasContingencyDisc = js.VHasContingencyDisc;
                    MarkUpPerc = js.MarkupPercentage;
                    deckCount = js.DeckCount;
                    actualPreWage = js.ActualPrevailingWage;
                    MaterialPerc = getMaterialDiscount(js.ProjectDelayFactor);
                }
            if (dbData == null)
            {
                dbData = js.dbData;
            }
            FetchMaterialValuesAsync(true);
            CalculateCost(null);
            js.TotalSalesCostTemp = TotalSale;           
            //});
                    
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
        public virtual void CalculateLaborMinCharge(bool hasSetupChanged)
        {
            //update Add labor for minimum cost
            if (SystemMaterials==null)
            {
                return;
            }
            LaborMinChargeHrs = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
             x.IsMaterialChecked && x.LaborExtension != 0).ToList().Select(x => x.Hours).Sum();
            LaborMinChargeMinSetup = SystemMaterials.Where(x => x.IncludeInLaborMinCharge == true &&
            x.IsMaterialChecked && x.LaborExtension != 0).ToList().Select(x => x.SetupMinCharge).Sum();

            //LaborMinChargeLaborExtension = (LaborMinChargeMinSetup + LaborMinChargeHrs) > 20 ? 0 :
            //                                    (20 - LaborMinChargeMinSetup - LaborMinChargeHrs) * laborRate;

            LaborMinChargeLaborExtension = TotalHrsSystemLabor > 20 ? 0 : isPrevailingWage ? actualPreWage * (20 - TotalHrsSystemLabor) : 
                laborRate * (20 - TotalHrsSystemLabor);

            LaborMinChargeLaborUnitPrice = TotalHrsSystemLabor > 20 ? 0 : (20 - TotalHrsSystemLabor);//(riserCount + totalSqft) == 0 ? 0 : LaborMinChargeLaborExtension / (riserCount + totalSqft);

            if (previousLaborCharges != SystemMaterials.Where(x=>x.IsMaterialChecked).ToList().Select(x => x.MaterialExtension).Sum()+ 
                SystemMaterials.Where(x => x.IsMaterialChecked).ToList().Select(x => x.LaborExtension).Sum())
            {
                //if (LaborMinChargeMinSetup + LaborMinChargeHrs < 20)
                if (TotalHrsSystemLabor < 20)
                {
                    ZAddLaborMinCharge = true;
                }
                else
                    ZAddLaborMinCharge = false;

                previousLaborCharges = SystemMaterials.Where(x => x.IsMaterialChecked).ToList().Select(x => x.MaterialExtension).Sum()+
                                        SystemMaterials.Where(x => x.IsMaterialChecked).ToList().Select(x => x.LaborExtension).Sum();
                previousMinLaborCheckboxStatus = ZAddLaborMinCharge;
            }
            //if (LaborMinChargeMinSetup + LaborMinChargeHrs==0)
            if (TotalHrsSystemLabor ==0)
            {
                ZAddLaborMinCharge = previousMinLaborCheckboxStatus;
            }
            OnPropertyChanged("LaborMinChargeMinSetup");
            OnPropertyChanged("ZAddLaborMinCharge");
            OnPropertyChanged("LaborMinChargeHrs");
            OnPropertyChanged("LaborMinChargeLaborExtension");
            OnPropertyChanged("LaborMinChargeLaborUnitPrice");


        }
        public virtual bool getCheckboxCheckStatus(string materialName)
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
                case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                case "WEATHER SEAL XL TWO COATS":
                case "STAIR NOSING FROM DEXOTEX":
                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR WHITE":
                    return true;
                default:
                    return false;
            }
            
            //else if (weatherWearType == "Weather Wear Rehab")
            //{
            //    switch (materialName.ToUpper())
            //    {
            //        case "LIGHT CRACK REPAIR":
            //        //case "LARGE CRACK REPAIR":
            //        //case "BUBBLE REPAIR(MEASURE SQ FT)":
            //        case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
            //        case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
            //        case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
            //        case "GLASMAT #4 (1200 SQ FT) FROM ACME":
            //        case "CPC MEMBRANE":
            //        case "NEOTEX-38 PASTE":
            //        case "NEOTEX STANDARD POWDER(BODY COAT)":
            //        case "NEOTEX STANDARD POWDER(BODY COAT) 1":
            //        case "RESISTITE LIQUID":
            //        case "RESISTITE REGULAR GRAY":
            //        case "RESISTITE REGULAR WHITE":
            //        case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
            //        //case "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)":
            //        case "LIP COLOR":
            //        //case "AJ-44A DRESSING (SEALER)":
            //        case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
            //        //case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
            //        //case "VISTA PAINT ACRIPOXY":
            //        case "Stair Nosing From Dexotex":
            //        //case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
            //        case "WEATHER SEAL XL TWO COATS":
            //            return true;
            //        default:
            //            return false;
            //    }
            //}
            //return false;
        }
        public virtual bool getCheckboxEnabledStatus(string materialName)
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
                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR WHITE":
                return true;
                default:
                    return false;
            }
           
        }
        
        public virtual double CalculateHrs(double horzSft, double prodHor, double stairSqft, double prodStair, double vertSqft=0, double prodVert=0)
        {
            double val1 = prodHor != 0 ? horzSft / prodHor : 0;
            double val2 = prodStair != 0 ? stairSqft / prodStair : 0;
            double val3=prodVert!=0 ? vertSqft / prodVert : 0;
            return (val1 + val2+val3);
        }
        public virtual ObservableCollection<SystemMaterial> GetSystemMaterial(Dictionary<string, string> materialNames)
        {
            ObservableCollection<SystemMaterial> smCollection = new ObservableCollection<SystemMaterial>();
            int k = 0;
            foreach (string key in materialNames.Keys)
            {
                if (dbData==null)
                {
                    smCollection.Add(getSMObject(k, key, materialNames[key]));
                    double minLCharge = 0;
                    double.TryParse(materialDetails[k][6].ToString(), out minLCharge);
                    LaborMinChargeMinSetup = minLCharge;
                }
                else
                {
                    smCollection.Add(createSMObjectDB(key, materialNames[key]));
                    LaborMinChargeMinSetup = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Minimum Labor charge").Value;
                }
                    
                k++;
            }
            
            

            return smCollection;

        }
        
        private void FillMaterialList()
        {
            Dictionary<string, string> materialNames = new Dictionary<string, string>();
            materialNames.Add("Resistite Regular Over Texture(#55 Bag)", "55 LB BAG");
            materialNames.Add("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale", "ROLL");
            materialNames.Add("Rp Fabric 10 Inch Wide X (300 Lf) From Acme", "ROLL");
            materialNames.Add("Glasmat #4 (1200 Sq Ft) From Acme", "ROLL");
            materialNames.Add("Cpc Membrane", "5 GAL PAIL");
            materialNames.Add("Neotex-38 Paste", "5 GAL PAIL");
            materialNames.Add("Neotex Standard Powder(Body Coat)", "45 LB BAG");
            materialNames.Add("Neotex Standard Powder(Body Coat) 1", "45 LB BAG");
            materialNames.Add("Resistite Liquid", "5 GAL PAIL");
            materialNames.Add("Resistite Regular White", "55 LB BAG");

            materialNames.Add("Resistite Regular Or Smooth White(Knock Down Or Smooth)", "40 LB BAG");
            materialNames.Add("Aj-44A Dressing(Sealer)", "5 GAL PAIL");
            materialNames.Add("Vista Paint Acripoxy", "5 GAL PAIL");
            materialNames.Add("Lip Color", "ROLL 2 COATS");
            materialNames.Add("Resistite Universal Primer(Add 50% Water)", "Sq Ft");
            materialNames.Add("Custom Texture Skip Trowel(Resistite Smooth White)", "Sq Ft");
            materialNames.Add("Weather Seal XL two Coats", "Sq Ft");
            materialNames.Add("Stair Nosing From Dexotex", "Sq Ft");
            materialNames.Add("Extra Stair Nosing Lf", "Sq Ft");
            materialNames.Add("Plywood 3/4 & Blocking(# Of 4X8 Sheets)", "Sq Ft");
            materialNames.Add("Stucco Material Remove And Replace (Lf)", "Sq Ft");

        }
        public bool GetCheckBoxDependency(string matName)
        {
            switch (matName.ToUpper())
            {
                case "NEOTEX-38 PASTE":
                case "NEOTEX STANDARD POWDER(BODY COAT)":
                case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                case "RESISTITE LIQUIDE":

                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR WHITE":
                case "RESISTITE REGULAR OR SMOOTH GRAY(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":

                case "AJ-44A DRESSING(SEALER)":
                case "VISTA PAINT ACRIPOXY":
                case "LIP COLOR":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                    return true;
                default:
                    return false;
            }
        }
        public  virtual ObservableCollection<SystemMaterial> GetSystemMaterial()
        {

            ObservableCollection<SystemMaterial> smP = new ObservableCollection<SystemMaterial>();
            int cov;
            double mp;
            double w;
            double lfArea;
            double setUpMin = 0; // Setup minimum charges from google sheet, col 6
            double pRateStairs = 0; ///Production rate stairs from google sheet, col 5
            double hprRate = 0;///Horizontal Production rate  from google sheet, col 4
            //double vprRate = 0;///Vertical Production rate  from google sheet, col 1
            double sqh = 0;
            double labrExt = 0;
            double calcHrs = 0;
            double sqStairs = 0;
            double qty = 0;

            if (isPrevailingWage) { double.TryParse(freightData[5][0].ToString(), out prPerc); }
            else
                prPerc = 0;
            
            #region rehab
            if (weatherWearType == "Weather Wear Rehab")
            {
                int.TryParse(materialDetails[0][2].ToString(), out cov);
                double.TryParse(materialDetails[0][0].ToString(), out mp);
                double.TryParse(materialDetails[0][3].ToString(), out w);
                double.TryParse(materialDetails[0][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[0][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[0][4].ToString(), out hprRate);
                hprRate = hprRate * (1 + prPerc);
                pRateStairs = pRateStairs * (1 + prPerc);
                sqh = getSqFtAreaH("Light Crack Repair");
                calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
                lfArea = getlfArea("Light Crack Repair");
                sqStairs = getSqFtStairs("Light Crack Repair");

                labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
                qty = getQuantity("Light Crack Repair", cov, lfArea);
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

                    SMSqftH = sqh,
                    Operation = "CAULK",
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = getSqFtStairs("Light Crack Repair"),
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs,
                    Qty = qty,
                    LaborExtension = labrExt,
                    LaborUnitPrice = labrExt / (riserCount + totalSqft),
                    FreightExtension = w * qty,
                    MaterialExtension = mp * qty

                });
                int.TryParse(materialDetails[1][2].ToString(), out cov);
                double.TryParse(materialDetails[1][0].ToString(), out mp);
                double.TryParse(materialDetails[1][3].ToString(), out w);
                lfArea = getlfArea("Large Crack Repair");

                double.TryParse(materialDetails[1][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[1][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[1][4].ToString(), out hprRate);
                hprRate = hprRate * (1 + prPerc);
                pRateStairs = pRateStairs * (1 + prPerc);
                calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
                sqh = getSqFtAreaH("Large Crack Repair");
                sqStairs = getSqFtStairs("Large Crack Repair");

                labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
                qty = getQuantity("Large Crack Repair", cov, lfArea);
                smP.Add(new SystemMaterial
                {
                    IsWWR = true,
                    IsMaterialChecked = getCheckboxCheckStatus("Large Crack Repair"),
                    IsMaterialEnabled = true, /*getCheckboxEnabledStatus("Large Crack Repair"),*/
                    Name = "Large Crack Repair",
                    SMUnits = "LF",
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,
                    Qty = qty,

                    SMSqftH = sqh,
                    //Operation = "CAULK",
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = sqStairs,
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs,
                    LaborExtension = labrExt,
                    LaborUnitPrice = labrExt / (riserCount + totalSqft),
                    FreightExtension = w * qty,
                    MaterialExtension = mp * qty

                });
                int.TryParse(materialDetails[2][2].ToString(), out cov);
                double.TryParse(materialDetails[2][0].ToString(), out mp);
                double.TryParse(materialDetails[2][3].ToString(), out w);
                lfArea = getlfArea("Bubble Repair(Measure Sq Ft)");
                double.TryParse(materialDetails[2][6].ToString(), out setUpMin);
                double.TryParse(materialDetails[2][5].ToString(), out pRateStairs);
                double.TryParse(materialDetails[2][4].ToString(), out hprRate);
                hprRate = hprRate * (1 + prPerc);
                pRateStairs = pRateStairs * (1 + prPerc);
                sqh = getSqFtAreaH("Bubble Repair(Measure Sq Ft)");
                sqStairs = getSqFtStairs("Bubble Repair(Measure Sq Ft)");
                calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
                labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
                qty = getQuantity("Bubble Repair(Measure Sq Ft)", cov, lfArea);
                smP.Add(new SystemMaterial
                {
                    IsWWR = true,
                    IsMaterialChecked = getCheckboxCheckStatus("Bubble Repair(Measure Sq Ft)"),
                    IsMaterialEnabled = true,/*getCheckboxEnabledStatus("Bubble Repair(Measure Sq Ft)"),*/
                    Name = "Bubble Repair(Measure Sq Ft)",
                    SMUnits = "Sq Ft",
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,
                    Qty = qty,
                    SMSqftH = sqh,
                    Operation = "CAULK",
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = sqStairs,
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs,
                    LaborExtension = labrExt,
                    LaborUnitPrice = labrExt / (riserCount + totalSqft),
                    FreightExtension = w * qty,
                    MaterialExtension = mp * qty
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
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Resistite Regular Over Texture(#55 Bag)");
            sqStairs = getSqFtStairs("Resistite Regular Over Texture(#55 Bag)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge=true

            });
            int.TryParse(materialDetails[4][2].ToString(), out cov);
            double.TryParse(materialDetails[4][0].ToString(), out mp);
            double.TryParse(materialDetails[4][3].ToString(), out w);
            lfArea = getlfArea("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale");
            double.TryParse(materialDetails[4][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[4][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[4][4].ToString(), out hprRate);
            sqh = getSqFtAreaH("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale");
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                SMSqft = lfArea,
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[5][2].ToString(), out cov);
            double.TryParse(materialDetails[5][0].ToString(), out mp);
            double.TryParse(materialDetails[5][3].ToString(), out w);
            lfArea = getlfArea("Rp Fabric 10 Inch Wide X (300 Lf) From Acme");
            double.TryParse(materialDetails[5][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[5][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[5][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[6][2].ToString(), out cov);
            double.TryParse(materialDetails[6][0].ToString(), out mp);
            double.TryParse(materialDetails[6][3].ToString(), out w);
            lfArea = getlfArea("Glasmat #4 (1200 Sq Ft) From Acme");
            double.TryParse(materialDetails[6][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[6][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[6][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[7][2].ToString(), out cov);
            double.TryParse(materialDetails[7][0].ToString(), out mp);
            double.TryParse(materialDetails[7][3].ToString(), out w);
            double.TryParse(materialDetails[7][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[7][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[7][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                SMSqft = lfArea,
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[8][2].ToString(), out cov);
            double.TryParse(materialDetails[8][0].ToString(), out mp);
            double.TryParse(materialDetails[8][3].ToString(), out w);
            lfArea = getlfArea("Neotex-38 Paste");
            double.TryParse(materialDetails[8][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[8][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[8][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                MaterialPrice = mp,
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[9][2].ToString(), out cov);
            double.TryParse(materialDetails[9][0].ToString(), out mp);
            double.TryParse(materialDetails[9][3].ToString(), out w);
            lfArea = getlfArea("Neotex Standard Powder(Body Coat)");
            double.TryParse(materialDetails[9][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[9][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[9][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                SMSqft = lfArea,
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[10][2].ToString(), out cov);
            double.TryParse(materialDetails[10][0].ToString(), out mp);
            double.TryParse(materialDetails[10][3].ToString(), out w);
            lfArea = getlfArea("Neotex Standard Powder(Body Coat) 1");
            double.TryParse(materialDetails[10][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[10][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[10][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Neotex Standard Powder(Body Coat) 1");
            sqStairs = getSqFtStairs("Neotex Standard Powder(Body Coat) 1");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Neotex Standard Powder(Body Coat) 1", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[11][2].ToString(), out cov);
            double.TryParse(materialDetails[11][0].ToString(), out mp);
            double.TryParse(materialDetails[11][3].ToString(), out w);
            lfArea = getlfArea("Resistite Liquid");
            double.TryParse(materialDetails[11][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[11][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[11][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                Coverage = cov,
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            
            int.TryParse(materialDetails[13][2].ToString(), out cov);
            double.TryParse(materialDetails[13][0].ToString(), out mp);
            double.TryParse(materialDetails[13][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular White");
            double.TryParse(materialDetails[13][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[13][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[13][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });

            
            int.TryParse(materialDetails[15][2].ToString(), out cov);
            double.TryParse(materialDetails[15][0].ToString(), out mp);
            double.TryParse(materialDetails[15][3].ToString(), out w);
            lfArea = getlfArea("Resistite Regular Or Smooth White(Knock Down Or Smooth)");
            double.TryParse(materialDetails[15][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[15][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[15][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[16][2].ToString(), out cov);
            double.TryParse(materialDetails[16][0].ToString(), out mp);
            double.TryParse(materialDetails[16][3].ToString(), out w);
            lfArea = getlfArea("Aj-44A Dressing(Sealer)");
            double.TryParse(materialDetails[16][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[16][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[16][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Aj-44A Dressing(Sealer)");
            sqStairs = getSqFtStairs("Aj-44A Dressing(Sealer)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Aj-44A Dressing(Sealer)", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = false,//getCheckboxCheckStatus("Aj-44A Dressing(Sealer)"),
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[17][2].ToString(), out cov);
            double.TryParse(materialDetails[17][0].ToString(), out mp);
            double.TryParse(materialDetails[17][3].ToString(), out w);
            lfArea = getlfArea("Vista Paint Acripoxy");
            double.TryParse(materialDetails[17][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[17][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[17][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Vista Paint Acripoxy");
            sqStairs = getSqFtStairs("Vista Paint Acripoxy");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Vista Paint Acripoxy", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });
            int.TryParse(materialDetails[18][2].ToString(), out cov);
            double.TryParse(materialDetails[18][0].ToString(), out mp);
            double.TryParse(materialDetails[18][3].ToString(), out w);
            lfArea = getlfArea("Lip Color");
            double.TryParse(materialDetails[18][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[18][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[18][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Lip Color");
            sqStairs = getSqFtStairs("Lip Color");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Lip Color", cov, lfArea);
            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                IsMaterialChecked = getCheckboxCheckStatus("Lip Color"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Lip Color"),
                Name = "Lip Color",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });

            int.TryParse(materialDetails[19][2].ToString(), out cov);
            double.TryParse(materialDetails[19][0].ToString(), out mp);
            double.TryParse(materialDetails[19][3].ToString(), out w);
            lfArea = getlfArea("Resistite Universal Primer(Add 50% Water)");
            double.TryParse(materialDetails[19][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[19][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[19][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Resistite Universal Primer(Add 50% Water)");
            sqStairs = getSqFtStairs("Resistite Universal Primer(Add 50% Water)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
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
                Qty = qty,
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
                MaterialExtension = mp * qty,
                IncludeInLaborMinCharge = true
            });

            //Add labor for Minimum Charge

            //double.TryParse(materialDetails[26][6].ToString(), out setUpMin);
            LaborMinChargeMinSetup = 6.75;
            IEnumerable<SystemMaterial> selected = smP.Where(x => x.IsMaterialChecked).ToList();
            LaborMinChargeHrs = smP.Where(x => x.IsMaterialChecked).ToList().Select(x => x.Hours).Sum();
            LaborMinChargeLaborExtension = (LaborMinChargeMinSetup + LaborMinChargeHrs) > 20 ? 0 : (20 - (LaborMinChargeMinSetup + LaborMinChargeHrs)) * laborRate;
            LaborMinChargeLaborUnitPrice = (riserCount + totalSqft) == 0 ? 0: LaborMinChargeLaborExtension / (riserCount + totalSqft);

            
            int.TryParse(materialDetails[21][2].ToString(), out cov);
            double.TryParse(materialDetails[21][0].ToString(), out mp);
            double.TryParse(materialDetails[21][3].ToString(), out w);
            lfArea = getlfArea("Custom Texture Skip Trowel(Resistite Smooth White)");
            double.TryParse(materialDetails[21][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[21][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[21][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Custom Texture Skip Trowel(Resistite Smooth White)");
            sqStairs = getSqFtStairs("Custom Texture Skip Trowel(Resistite Smooth White)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = getQuantity("Custom Texture Skip Trowel(Resistite Smooth White)", cov, lfArea);
            bool isch = getCheckboxCheckStatus("Custom Texture Skip Trowel(Resistite Smooth White)");

            smP.Add(new SystemMaterial
            {
                IsCheckboxDependent = true,
                
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
                MaterialExtension = mp * qty,
                IsMaterialChecked = isch,
                IncludeInLaborMinCharge = true

            });
            int.TryParse(materialDetails[22][2].ToString(), out cov);
            double.TryParse(materialDetails[22][0].ToString(), out mp);
            double.TryParse(materialDetails[22][3].ToString(), out w);
            lfArea = getlfArea("Weather Seal XL two Coats");
            double.TryParse(materialDetails[22][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[22][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[22][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                IncludeInLaborMinCharge = true,
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
                MaterialExtension = mp * qty,
                
            });
            int.TryParse(materialDetails[23][2].ToString(), out cov);
            double.TryParse(materialDetails[23][0].ToString(), out mp);
            double.TryParse(materialDetails[23][3].ToString(), out w);
            double.TryParse(materialDetails[23][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[23][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[23][4].ToString(), out hprRate);
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
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
                IncludeInLaborMinCharge = true,
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
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Extra Stair Nosing Lf");
            sqStairs = getSqFtStairs("Extra Stair Nosing Lf"); //getvalue from systemMaterial
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = (calcHrs != 0) ? (setUpMin + calcHrs) * laborRate : 0;
            qty = 0;
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Extra Stair Nosing Lf"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Extra Stair Nosing Lf"),
                Name = "Extra Stair Nosing Lf",
                IncludeInLaborMinCharge = true,
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
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
            sqStairs = getSqFtStairs("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = setUpMin > calcHrs ? setUpMin * laborRate : calcHrs * laborRate;
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
                Name = "Plywood 3/4 & Blocking(# Of 4X8 Sheets)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                IncludeInLaborMinCharge = false,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                SMSqftH = sqh,
                Operation = "Remove and replace dry rot",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                Qty = qty,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / qty,
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
            hprRate = hprRate * (1 + prPerc);
            pRateStairs = pRateStairs * (1 + prPerc);
            sqh = getSqFtAreaH("Stucco Material Remove And Replace (Lf)");
            sqStairs = getSqFtStairs("Stucco Material Remove And Replace (Lf)");
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs);
            labrExt = setUpMin > calcHrs ? setUpMin * laborRate : calcHrs * laborRate;
            smP.Add(new SystemMaterial
            {
                IsMaterialChecked = getCheckboxCheckStatus("Stucco Material Remove And Replace (Lf)"),
                IsMaterialEnabled = getCheckboxEnabledStatus("Stucco Material Remove And Replace (Lf)"),
                Name = "Stucco Material Remove And Replace (Lf)",
                SMUnits = "Sq Ft",
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                IncludeInLaborMinCharge = false,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = "Remove and replace dry rot",
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = getSqFtStairs("Stucco Material Remove And Replace (Lf)"),
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                LaborUnitPrice = labrExt / qty,
                FreightExtension = w * qty,
                MaterialExtension = mp * qty
            });

            //foreach (SystemMaterial item in smP)
            //{
            //    item.HorizontalProductionRate = item.HorizontalProductionRate*(1 + prPerc);
            //    item.StairsProductionRate = item.StairsProductionRate * (1 + prPerc);
            //    item.VerticalProductionRate = item.VerticalProductionRate * (1 + prPerc);
            //}

            return smP;
        }

        public virtual void setCheckBoxes()
        {
            
            var materials = SystemMaterials.Where(x => x.IsCheckboxDependent == true).ToList();
            SystemMaterial lipMat1 = null;
            SystemMaterial lipMat2 = null;
            if (materials.Count == 0)
            {
                return;
            }
            
            foreach (SystemMaterial mat in materials)
            {
                if (mat.Name == "Lip Color")
                {
                    mat.IsMaterialChecked = false;
                }
                if (mat.Name == "Aj-44A Dressing(Sealer)")
                {
                    lipMat1 = mat;
                    mat.IsMaterialChecked = false;
                }
                if (mat.Name == "Vista Paint Acripoxy")
                {
                    lipMat2 = mat;
                    mat.IsMaterialChecked = false;
                }
            }

            if (projectname.Contains("Rehab"))
            {
                lipMat2.IsMaterialChecked = true;
                ApplyCheckUnchecks(lipMat2.Name);
                SystemMaterials.Where(x => x.Name == "Custom Texture Skip Trowel(Resistite Smooth Gray)").FirstOrDefault().IsMaterialEnabled = true;
                
            }
            else
            {
                lipMat1.IsMaterialChecked = true;
                ApplyCheckUnchecks(lipMat1.Name);
            }
            SystemMaterials.Where(x => x.Name == "Neotex-38 Paste").FirstOrDefault().IsMaterialChecked = true;
 
        }


        #region Material
        public virtual double getQuantity(string materialName, double coverage, double lfArea)
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
                
                case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                case "WEATHER SEAL XL TWO COATS":
                    return coverage == 0 ? 0 : lfArea / coverage;
                case "LIP COLOR":
                case "AJ-44A DRESSING(SEALER)":
                case "VISTA PAINT ACRIPOXY":
                    return coverage==0 ? 0.5:lfArea / coverage < 0.5 ? 0.5 : lfArea / coverage;
                case "STAIR NOSING FROM DEXOTEX":
                    return lfArea*stairWidth ;
                case "NEOTEX-38 PASTE":
                    return neotaxQty();
                case "RESISTITE LIQUID":
                    return 0;
                default:
                    return 0;
            }
        }

        public virtual void calculateRLqty()
        {
            double val1=0, val2=0, val3 = 0, val4 = 0;
            double qty = 0;
            SystemMaterial skipMat;
            if (dbData==null)
            {
                double.TryParse(materialDetails[13][2].ToString(), out val1);
                double.TryParse(materialDetails[15][2].ToString(), out val2);
            }
            else
            {
                val1 = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == "RESISTITE REGULAR WHITE").Coverage;
                val2 = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)").Coverage;
            }
                
            val1 = getQuantity("RESISTITE REGULAR WHITE", val1, getlfArea("RESISTITE REGULAR WHITE"));
            val2 = getQuantity("RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)", val2, getlfArea("RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)"));

            skipMat = SystemMaterials.Where(x => x.Name.ToUpper() == "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val3 = skipMat.Qty;
                }
                else
                    val3 = 0;
            }
            skipMat = SystemMaterials.Where(x => x.Name.ToUpper() == "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val3 = skipMat.Qty;
                }
                else
                    val3 = 0;
            }

            skipMat = SystemMaterials.Where(x => x.Name.ToUpper() == "RESISTITE REGULAR OVER TEXTURE(#55 BAG)").FirstOrDefault();
            if (skipMat != null)
            {
                if (skipMat.IsMaterialChecked)
                {
                    val4 = skipMat.Qty;
                }
                else
                    val4 = 0;
            }

            qty = (val1 + val2 + val3) * 0.33 + val4 / 5;

           
            skipMat = SystemMaterials.Where(x => x.Name == "Resistite Liquid").FirstOrDefault();
            if (skipMat != null)
            {
                skipMat.Qty = qty;
            }

            skipMat=SystemMaterials.Where(x => x.Name == "Lip Color").FirstOrDefault();
            if (skipMat!=null)
            {
                bool isChecked = skipMat.IsMaterialChecked;
                skipMat.Qty = val3 + val2;
                skipMat.IsMaterialChecked = isChecked;
            }
        }

        private double neotaxQty()
        {
            double val1, val2;
            if (SystemMaterials.Count==0)
            {
                return 0;
            }
            if (dbData==null)
            {
                double.TryParse(materialDetails[9][2].ToString(), out val1);
                double.TryParse(materialDetails[10][2].ToString(), out val2);
               
            }
            else
            {
                val1= dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == "NEOTEX STANDARD POWDER(BODY COAT)").Coverage;
                val2 = dbData.MaterialDBData.FirstOrDefault(x => x.MaterialName == "NEOTEX STANDARD POWDER(BODY COAT) 1").Coverage;
            }
            val1 = getQuantity("NEOTEX STANDARD POWDER(BODY COAT)", val1, getlfArea("NEOTEX STANDARD POWDER(BODY COAT)"));
            val2 = getQuantity("NEOTEX STANDARD POWDER(BODY COAT) 1", val2, getlfArea("NEOTEX STANDARD POWDER(BODY COAT) 1"));
            val1 = SystemMaterials.Where(x => x.Name == "Neotex Standard Powder(Body Coat)").FirstOrDefault().IsMaterialChecked ? val1 : 0;
            val2 = SystemMaterials.Where(x => x.Name == "Neotex Standard Powder(Body Coat) 1").FirstOrDefault().IsMaterialChecked ? val2 : 0;
            return (val2 * 1.5 + val1 * 1.25) / 5;
        }

        public virtual double getlfArea(string materialName)
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
                    return (riserCount * stairWidth * 2) + totalSqft;
                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    return (riserCount * stairWidth * 2) + totalSqft;//stairWidth=4
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
            if (OtherLaborMaterials != null && OtherMaterials != null)
            {
                if (OtherMaterials.Count > 0)
                {
                    TotalOCExtension = OtherMaterials.Select(x => x.Extension).Sum();               

                }
                if (OtherLaborMaterials.Count > 0)
                {
                    TotalOCLaborExtension = OtherLaborMaterials.Select(x => x.LExtension).Sum();
                    OnPropertyChanged("TotalOCLaborExtension");
                }
            }

        }
        /// SubContract Cost
        private void CalSCTotal()
        {
            if (SubContractLaborItems==null)
            {
                return;
            }
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
            if (systemMaterials==null)
            {
                return;
            }
            IEnumerable<SystemMaterial> selectedSystemMaterials = systemMaterials.Where(x => x.IsMaterialChecked == true);
            if (selectedSystemMaterials.Count() > 0)
            {
                SumQty = selectedSystemMaterials.Select(x => x.Qty).Sum();
                SumMatPrice = selectedSystemMaterials.Select(x => x.MaterialPrice).Sum();
                SumTotalMatExt = selectedSystemMaterials.Select(x => x.MaterialExtension).Sum();
                SumWeight = selectedSystemMaterials.Select(x => x.FreightExtension).Sum();
                //Total Freight

                SumFreight = FreightCalculator(SumWeight);
            }
            else
            {
                SumQty = 0;
                SumMatPrice = 0;
                SumTotalMatExt = 0;
                SumWeight = 0;
                //Total Freight

                SumFreight = FreightCalculator(SumWeight);
            }
        }
        public double TotalMaterialCost { get; set; }

        public double FreightCalculator(double weight)
        {
            //double result;
            double frCalc = 0;
            double factor = 0;
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
                        if (dbData==null)
                        {
                            double.TryParse(freightData[0][1].ToString(), out factor);
                        }
                        else
                            factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight10000").FactorValue;
                        frCalc = factor * weight; /*0.03*/
                    }

                    else
                    {
                        if (weight > 5000)
                        {
                            if (dbData == null)
                            {
                                double.TryParse(freightData[1][1].ToString(), out factor);
                            }
                            else
                                factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight5000").FactorValue;
                            frCalc = factor * weight; /*0.04*/
                        }
                        else
                        {
                            if (weight > 2000)
                            {
                                if (dbData == null)
                                {
                                    double.TryParse(freightData[2][1].ToString(), out factor);
                                }
                                else
                                    factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight2000").FactorValue;
                                frCalc = factor * weight; /*0.09*/
                            }
                            else
                            {
                                if (weight > 1000)
                                {
                                    if (dbData == null)
                                    {
                                        double.TryParse(freightData[3][1].ToString(), out factor);
                                    }
                                    else
                                        factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight1000").FactorValue;
                                    frCalc = factor * weight; /*0.12*/
                                }
                                else
                                {
                                    if (weight > 400)
                                    {
                                        if (dbData == null)
                                        {
                                            double.TryParse(freightData[4][1].ToString(), out factor);
                                        }
                                        else
                                            factor = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "Weight400").FactorValue;
                                        frCalc = factor;/*75*/
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

            return frCalc;
        }
        
        private void CalculateCostBreakup()
        {
            if (SystemMaterials==null)
            {
                return;
            }

            IEnumerable<SystemMaterial> systemCBMaterial = SystemMaterials.Where(x => x.IsMaterialChecked == true);
            if (systemCBMaterial != null)
            {
                TotalMaterialCostbrkp = (SumTotalMatExt + TotalOCExtension);
                
                TotalMaterialCost = TotalMaterialCostbrkp;
                OnPropertyChanged("TotalMaterialCost");
                TotalMaterialCostbrkp = TotalMaterialCostbrkp*(1+MaterialPerc);
                TotalWeightbrkp = SumWeight;
                TotalFreightCostBrkp = FreightCalculator(TotalWeightbrkp);
                TotalSubContractLaborCostBrkp = TotalSCExtension;
                CalculateCostPerSqFT();
            }
        }
        public virtual void CalculateCostPerSqFT()
        {
            CostPerSquareFeet = (totalSqft + deckCount) == 0 ? 0 : Math.Round(TotalMaterialCost / (totalSqft + deckCount), 2);
        }
        #endregion

        #region LaborSheet
        [DataMember]
        public double MaterialPerc { get; set; }
        [DataMember]
        public double LaborPerc { get; set; }
        [DataMember]
        public double TotalOCLaborExtension { get; set; }
        private bool includeDriveHrs;
        [DataMember]
        public bool IncludeDriveHours
        {
            get
            {
                return includeDriveHrs;
            }
            set
            {
                if (value!=includeDriveHrs)
                {
                    includeDriveHrs = value;
                    //if (isDataAvailable)
                    //{
                        CalculateCost(null);
                        CalculateCost(null);
                        //CalculateAllMaterial();
                    //}
                    
                    OnPropertyChanged("IncludeDriveHours");
                }
            }
        }
        public double DriveLaborValue { get; set; }
        public double ProjectProfitMargin { get; set; }

        public virtual double getSqftAreaVertical(string materialName)
        {
            return 0;
        }
        public virtual double getSqFtAreaH(string materialName)
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
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                case "AJ-44A DRESSING(SEALER)":
                case "WEATHER SEAL XL TWO COATS":
                    return totalSqft;
                
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
        public virtual double getSqFtStairs(string materialName)
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
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                case "AJ-44A DRESSING(SEALER)":
                case "WEATHER SEAL XL TWO COATS":
                    return stairWidth * riserCount * 2;
                case "LARGE CRACK REPAIR":
                case "LIGHT CRACK REPAIR":
                case "BUBBLE REPAIR MAJOR SQFT":
                case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                case "CPC MEMBRANE":

                case "Glasmat #4 (1200 Sq Ft) From Acme":
                case "NEOTEX-38 PASTE":
                case "RESISTITE LIQUID":
                    return 0;// from System material


                case "STAIR NOSING FROM DEXOTEX":
                    return riserCount * 3.5;
                //case "EXTRA STAIR NOSING LF":
                //    return 0;// To get from value system material
                default:
                    return 0;
            }
        }

        //New change for LaborUnitPricePer square Feet
        private void calculateLaborTotals()
        {
            if (SystemMaterials==null)
            {
                return;
            }
            double additonalLaborMetalSlope = 0;
            double  laborDeduction = 0;
            AllTabsLaborTotal = 0;
            AllTabsMaterialTotal =0;
            AllTabsFreightTotal = 0;
            AllTabsSubContractTotal = 0;
            if (isPrevailingWage)
            {
                //double.TryParse(laborDetails[0][0].ToString(), out preWage);

            }
            if (isDiscounted)
            {
                double.TryParse(laborDetails[1][0].ToString(), out laborDeduction);
                laborDeduction = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Deduct on Labor for large jobs").Value;
            }
            IEnumerable<SystemMaterial> selectedLabors = SystemMaterials.Where(x => x.IsMaterialChecked == true).ToList();

            //TotalSetupTimeLabor = ZAddLaborMinCharge ? selectedLabors.Select(x => x.Hours).Sum() + LaborMinChargeMinSetup : selectedLabors.Select(x => x.Hours).Sum();

            //TotalLaborUnitPrice = ZAddLaborMinCharge ? (selectedLabors.Select(x => x.LaborUnitPrice).Sum() +
            //    OtherLaborMaterials.Sum(x => x.LMaterialPrice)+
            //    LaborMinChargeLaborUnitPrice) * (1 + preWage + laborDeduction) :
            //    (selectedLabors.Select(x => x.LaborUnitPrice).Sum() + OtherLaborMaterials.Sum(x => x.LMaterialPrice)) * (1 + preWage + laborDeduction);
            
            //new min labor logic 
            //TotalLaborExtension = ZAddLaborMinCharge ? (selectedLabors.Select(x => x.LaborExtension).Sum() + LaborMinChargeLaborExtension) * 
            //    (1 + preWage + laborDeduction) :
            //    selectedLabors.Select(x => x.LaborExtension).Sum() * (1 + preWage + laborDeduction);
            TotalLaborExtension = selectedLabors.Select(x => x.LaborExtension).Sum() * (1 + preWage + laborDeduction);
            //Ends
            
            TotalLaborExtension = TotalLaborExtension + TotalOCLaborExtension * (1 + preWage + laborDeduction);
            tempTotalLaborExtension = TotalLaborExtension;
            if (SlopeTotals != null && MetalTotals != null)
            {
                TotalSlopingPrice = getTotals(SlopeTotals.LaborExtTotal, SlopeTotals.MaterialExtTotal, SlopeTotals.MaterialFreightTotal, 0);
                TotalMetalPrice = getTotals(MetalTotals.LaborExtTotal, MetalTotals.MaterialExtTotal, MetalTotals.MaterialFreightTotal, 0);
                TotalSubcontractLabor = SlopeTotals.SubContractLabor + MetalTotals.SubContractLabor + TotalSubContractLaborCostBrkp;

            }
            //,
            TotalSystemPrice = getTotals(TotalLaborExtension, TotalMaterialCostbrkp, TotalFreightCostBrkp, TotalSubContractLaborCostBrkp);
            
            TotalSale = TotalSlopingPrice + TotalMetalPrice + TotalSystemPrice + TotalSubcontractLabor;
            AllTabsLaborTotal = TotalLaborExtension + DriveLaborValue;
            AllTabsMaterialTotal = TotalMaterialCostbrkp;
            AllTabsFreightTotal = TotalFreightCostBrkp;
            AllTabsSubContractTotal = TotalSubContractLaborCostBrkp;

            if (SlopeTotals != null)
            {
               additonalLaborMetalSlope = (totalSqft + riserCount)==0 ? 0:SlopeTotals.LaborExtTotal / (totalSqft + riserCount);
               AllTabsLaborTotal = SlopeTotals.LaborExtTotal+AllTabsLaborTotal;
                AllTabsMaterialTotal = SlopeTotals.MaterialExtTotal +  AllTabsMaterialTotal;
                AllTabsFreightTotal = SlopeTotals.MaterialFreightTotal +AllTabsFreightTotal;
                AllTabsSubContractTotal = SlopeTotals.SubContractLabor +  AllTabsSubContractTotal;
                TotalLaborUnitPrice = TotalLaborUnitPrice + additonalLaborMetalSlope;
            }
            if (MetalTotals!=null)
            {
                additonalLaborMetalSlope = (totalSqft + riserCount) == 0 ? 0 : MetalTotals.LaborExtTotal / (totalSqft + riserCount);

                AllTabsLaborTotal = AllTabsLaborTotal+ MetalTotals.LaborExtTotal ;
                AllTabsMaterialTotal = AllTabsMaterialTotal+ MetalTotals.MaterialExtTotal ;
                AllTabsFreightTotal = AllTabsFreightTotal+ MetalTotals.MaterialFreightTotal ;
                AllTabsSubContractTotal = AllTabsSubContractTotal+ MetalTotals.SubContractLabor ;
                TotalLaborUnitPrice = TotalLaborUnitPrice + additonalLaborMetalSlope;
            }   

            TotalLaborWithoutDrive = AllTabsLaborTotal - DriveLaborValue;
            //OtherLaborMaterials.FirstOrDefault(x => x.Name == "Access issues?").LMaterialPrice = Math.Round(selectedLabors.Select(x => x.LaborExtension).Sum(),2);
            UpdateUILaborCost();
            UpdateSumOfSqft();
        }

        public void calculateLaborTotalsWithMinLabor()
        {
            double extrHrs = 0;
            double laborDeduction = 0;
            if (isDiscounted)
            {
                //double.TryParse(laborDetails[1][0].ToString(), out laborDeduction);
                laborDeduction = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Deduct on Labor for large jobs").Value;
            }
            if (ZAddLaborMinCharge)
            {

                
                TotalLaborExtension = TotalLaborExtension + LaborMinChargeLaborExtension * (1 + preWage + laborDeduction);
                extrHrs= TotalHrsSystemLabor<20?20 - TotalHrsSystemLabor:0;
                AllTabsLaborTotal = AllTabsLaborTotal + LaborMinChargeLaborExtension * (1 + preWage + laborDeduction); ;
                OnPropertyChanged("AllTabsLaborTotal");
                OnPropertyChanged("TotalLaborExtension");

            }
            TotalHrsSystemLabor = TotalHrsSystemLabor + extrHrs;
            OnPropertyChanged("TotalHrsSystemLabor");
            TotalHrsLabor = TotalHrsSystemLabor + TotalHrsMetalLabor + TotalHrsSlopeLabor;
            OnPropertyChanged("TotalHrsLabor");
        }
        public virtual void UpdateSumOfSqft()
        {
            double sumVal = totalSqft;
            TotalLaborUnitPrice = sumVal == 0 ? 0 : TotalLaborWithoutDrive / sumVal;
            OnPropertyChanged("TotalLaborUnitPrice");
        }

        public double  TotalLaborWithoutDrive { get; set; }

        private void UpdateUILaborCost()
        {
            OnPropertyChanged("TotalLaborWithoutDrive");
            OnPropertyChanged("TotalLaborUnitPrice");
            OnPropertyChanged("TotalLaborExtension");
            OnPropertyChanged("TotalSlopingPrice");

            OnPropertyChanged("TotalMetalPrice");
            OnPropertyChanged("TotalSystemPrice");
            OnPropertyChanged("TotalSale");

            OnPropertyChanged("AllTabsLaborTotal");
            OnPropertyChanged("AllTabsMaterialTotal");
            OnPropertyChanged("AllTabsFreightTotal");
            OnPropertyChanged("AllTabsSubContractTotal");
            OnPropertyChanged("TotalSetupTimeLabor");
        }
        private double getTotals(double laborCost, double materialCost, double freightCost, double subcontractLabor)
        {
            if (dbData==null)
            {
                if (laborDetails == null)
                {
                    return 0;
                }
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
            else
            {
                double res = 0;
                double slopeTotal = laborCost;

                if (isPrevailingWage)
                {
                    // double.TryParse(laborDetails[4][0].ToString(), out res);
                    res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Workers Comp Prevailing Wage").Value;
                    slopeTotal = slopeTotal + laborCost * res;
                }
                else
                {
                    //double.TryParse(laborDetails[2][0].ToString(), out res);
                    res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Workers Comp TL > $24.00").Value;
                    slopeTotal = slopeTotal + laborCost * res;
                    //double.TryParse(laborDetails[3][0].ToString(), out res);
                    res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Workers Comp All < $23.99").Value;
                    slopeTotal = slopeTotal + laborCost * res;
                }

                //double.TryParse(laborDetails[5][0].ToString(), out res);
                res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Payroll Expense (SS, ET, Uemp, Dis, Medicare)").Value;
                slopeTotal = slopeTotal + laborCost * res;
                //double.TryParse(laborDetails[6][0].ToString(), out res);
                res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Tax").Value;
                double tax = res * (freightCost + materialCost) + materialCost + freightCost;//freight+material including tax

                slopeTotal = slopeTotal + tax;
                //subcontrctlabor
                //double.TryParse(laborDetails[8][0].ToString(), out res);
                res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Profit Margin on subcontract labor").Value;
                double subCLabor = subcontractLabor * res;
                //profitMarginAdd
                double pmAdd = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Profit Margin add").Value; 
                
                //double.TryParse(laborDetails[8][0].ToString(), out pmAdd);

                double profitMarginAdd = (slopeTotal * pmAdd) * (1 + pmAdd);
                
                //profit margin
                double pm=dbData.LaborDBData.FirstOrDefault(x => x.Name == "Profit Margin").Value; ;
                ProjectProfitMargin = pm;
                //double.TryParse(laborDetails[10][0].ToString(), out pm);

                double specialMetalDeduction = 0;
                if (isSpecialMetal)
                {
                    //Profit deduct for special metal
                    //double.TryParse(laborDetails[9][0].ToString(), out res);
                    res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Profit deduct for special metal").Value;
                    specialMetalDeduction = materialCost * res;
                }
                double TotalCost = (slopeTotal / pm) + profitMarginAdd + specialMetalDeduction + subCLabor;

                //double.TryParse(laborDetails[11][0].ToString(), out res);
                res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "General Liability (total based on revenue)").Value;
                double generalLiability = TotalCost * res / pm;

                //double.TryParse(laborDetails[12][0].ToString(), out res);
                res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "DirectExpense").Value;
                double directExpense = TotalCost * res / pm;

                //double.TryParse(laborDetails[13][0].ToString(), out res);
                res = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Contingency").Value;

                double contigency = TotalCost * res / pm;
                double ins, fuel, addup;

                //double.TryParse(laborDetails[14][0].ToString(), out ins);

                ins = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Insurance Increase Fund (5 % on total sale)").Value;
                //double.TryParse(laborDetails[15][0].ToString(), out fuel);
                fuel = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Fuel/Sur Chg (1-PRICE OF GAS) /-40").Value;
                //double.TryParse(laborDetails[16][0].ToString(), out addup);
                addup = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Add mark up to total job price").Value;
                double restTotal = TotalCost * (ins + fuel + addup);
                
                //calculated Profit Margin,currently not being used.
                double ProfitMargin = TotalCost - slopeTotal;

                return TotalCost + generalLiability + directExpense + contigency + restTotal;
            }
        }
        #endregion
        #endregion


        public virtual void populateCalculation()
        {
            if (dbData == null)
                return;

            LCostBreakUp = new ObservableCollection<CostBreakup>();
            double facValue = 0;
            double totalJobCostM = 0;
            double totalJobCostS = 0;
            double totalJobCostSy = 0;
            
            
            //double.TryParse(laborDetails[0][0].ToString(), out facValue);
            facValue = Math.Round(preWage,4);
            double fac1 = isPrevailingWage ? facValue : 0;
            if (dbData==null)
            {
                double.TryParse(laborDetails[1][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Deduct on Labor for large jobs").Value;
            double fac2 = isDiscounted ? facValue : 0;
            double calbackfactor = 1 + fac1 + fac2;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Total Labor",
                CalFactor = 0,
                MetalCost = MetalTotals!=null?(MetalTotals.LaborExtTotal / calbackfactor):0,
                SlopeCost = SlopeTotals!=null?(SlopeTotals.LaborExtTotal / calbackfactor):0,
                SystemCost = (TotalLaborExtension / calbackfactor),
                HideCalFactor = System.Windows.Visibility.Hidden
            });

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Prevailing Wage (Including Marina's Salary)",
                CalFactor = fac1,
                MetalCost = MetalTotals != null ? (MetalTotals.LaborExtTotal / calbackfactor) * fac1:0,
                SlopeCost = SlopeTotals!=null?(SlopeTotals.LaborExtTotal / calbackfactor) * fac1:0,
                SystemCost = (TotalLaborExtension / calbackfactor) * fac1
            });



            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Deduct on Labor for large jobs",
                CalFactor = fac2,
                MetalCost = MetalTotals != null ? (MetalTotals.LaborExtTotal / calbackfactor) * fac2:0,
                SlopeCost = SlopeTotals!=null?(SlopeTotals.LaborExtTotal / calbackfactor) * fac2:0,
                SystemCost = (TotalLaborExtension / calbackfactor) * fac2
            });
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Total Labor including prevailing wage",
                CalFactor = 0,
                MetalCost = MetalTotals != null ? MetalTotals.LaborExtTotal:0,
                SlopeCost = SlopeTotals != null ? SlopeTotals.LaborExtTotal:0,
                SystemCost = (TotalLaborExtension+DriveLaborValue),
                HideCalFactor = System.Windows.Visibility.Hidden
            });
            
            if (dbData == null)
            {
                double.TryParse(laborDetails[2][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Workers Comp TL > $24.00").Value;
            

            double actVal = isPrevailingWage ? 0 : facValue;
            double metalLabor = MetalTotals != null ? (MetalTotals.LaborExtTotal / calbackfactor + (MetalTotals.LaborExtTotal / calbackfactor * fac2)):0;
            double slopeLabor = SlopeTotals != null ? (SlopeTotals.LaborExtTotal / calbackfactor + (SlopeTotals.LaborExtTotal / calbackfactor * fac2)):0;
            //double systemLabor = (TotalLaborExtension / calbackfactor + (TotalLaborExtension / calbackfactor * fac2))+DriveLaborValue;
            double systemLabor = TotalLaborExtension + DriveLaborValue;
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Workers Comp TL > $24.00",
                CalFactor = facValue,
                MetalCost = actVal * metalLabor,
                SlopeCost = actVal * slopeLabor,
                SystemCost = actVal * systemLabor
            });

            if (dbData == null)
            {
                double.TryParse(laborDetails[3][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Workers Comp All < $23.99").Value;

            actVal = isPrevailingWage ? 0 : facValue;
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Workers Comp All < $23.99",
                CalFactor = facValue,
                MetalCost = actVal * metalLabor,
                SlopeCost = actVal * slopeLabor,
                SystemCost = actVal * systemLabor
            });

            if (dbData == null)
            {
                double.TryParse(laborDetails[4][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Workers Comp Prevailing Wage").Value;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Workers Comp Prevailing Wage",
                CalFactor = facValue,
                MetalCost = isPrevailingWage && MetalTotals != null  ? facValue * MetalTotals.LaborExtTotal : 0,
                SlopeCost = isPrevailingWage&& SlopeTotals != null ?  facValue * SlopeTotals.LaborExtTotal : 0,
                SystemCost = isPrevailingWage ? facValue * (TotalLaborExtension+DriveLaborValue) : 0
            });
            if (dbData == null)
            {
                double.TryParse(laborDetails[5][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Payroll Expense (SS, ET, Uemp, Dis, Medicare)").Value;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Payroll Expense (SS, ET, Uemp, Dis, Medicare)",
                CalFactor = facValue,
                MetalCost = facValue * metalLabor,
                SlopeCost = facValue * slopeLabor,
                SystemCost = facValue * systemLabor
            });
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Materials",
                CalFactor = 0,
                MetalCost = MetalTotals != null ? MetalTotals.MaterialExtTotal:0,
                SlopeCost = SlopeTotals != null ? SlopeTotals.MaterialExtTotal:0,
                SystemCost = TotalMaterialCostbrkp,
                HideCalFactor = System.Windows.Visibility.Hidden

            });
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Freight",
                CalFactor = 0,
                MetalCost = MetalTotals != null ? MetalTotals.MaterialFreightTotal:0,
                SlopeCost = SlopeTotals != null ? SlopeTotals.MaterialFreightTotal:0,
                SystemCost = TotalFreightCostBrkp,
                HideCalFactor = System.Windows.Visibility.Hidden
            });
            if (dbData == null)
            {
                double.TryParse(laborDetails[6][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Tax").Value;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Tax",
                CalFactor = facValue,
                MetalCost = MetalTotals != null ? facValue * (MetalTotals.MaterialExtTotal + MetalTotals.MaterialFreightTotal):0,
                SlopeCost = SlopeTotals != null ? facValue * (SlopeTotals.MaterialExtTotal + SlopeTotals.MaterialFreightTotal):0,
                SystemCost = facValue * (TotalMaterialCostbrkp + TotalFreightCostBrkp)
            });

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "SubContract Labor",
                CalFactor = 0,
                MetalCost = MetalTotals != null ? MetalTotals.SubContractLabor:0,
                SlopeCost = SlopeTotals != null ? SlopeTotals.SubContractLabor:0,
                SystemCost = 0,
                SubContractLaborCost = TotalSubContractLaborCostBrkp,
                HideCalFactor = System.Windows.Visibility.Hidden
            });
            for (int i = 3; i < LCostBreakUp.Count; i++)
            {
                totalJobCostM = totalJobCostM + LCostBreakUp[i].MetalCost;
                totalJobCostS = totalJobCostS + LCostBreakUp[i].SlopeCost;
                totalJobCostSy = totalJobCostSy + LCostBreakUp[i].SystemCost;

            }
            //totalJobCostM = LCostBreakUp.Select(x => x.MetalCost).Sum()- MetalTotals.LaborExtTotal;
            //totalJobCostS = LCostBreakUp.Select(x => x.SlopeCost).Sum()+SlopeTotals.LaborExtTotal;
            //totalJobCostSy = LCostBreakUp.Select(x => x.SystemCost).Sum()+ TotalLaborExtension;


            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Total Job Cost",
                CalFactor = 0,
                MetalCost = totalJobCostM,
                SlopeCost = totalJobCostS,
                SystemCost = totalJobCostSy,
                HideCalFactor = System.Windows.Visibility.Hidden
            });

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Individual Cost",
                CalFactor = 0,
                MetalCost = 1-MetalMarkup,
                SlopeCost = 1-SlopeMarkup,
                SystemCost = 1-MaterialMarkup,
                HideCalFactor = System.Windows.Visibility.Hidden
            });
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Individual Profit Margin",
                CalFactor = 0,
                MetalCost = MetalMarkup,
                SlopeCost = SlopeMarkup,
                SystemCost = MaterialMarkup,
                SubContractLaborCost= SubContractProfitMargin,
                HideCalFactor = System.Windows.Visibility.Hidden
            });

            double psy1 = SubContractMarkup * (TotalSubContractLaborCostBrkp);
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Profit Margin on subcontract labor",
                CalFactor = SubContractMarkup,
                MetalCost = 0,
                SlopeCost = 0,
                SubContractLaborCost = psy1,
                SystemCost = 0
            });


            if (dbData == null)
            {
                double.TryParse(laborDetails[8][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Profit Margin add").Value;
            
            double pm2, ps2, psy2;
            pm2 = totalJobCostM * facValue * (1 + facValue);
            ps2 = totalJobCostS * facValue * (1 + facValue);
            psy2 = totalJobCostSy * facValue * (1 + facValue);
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Profit Margin add",
                CalFactor = facValue,
                MetalCost = pm2,
                SlopeCost = ps2,
                SystemCost = psy2
            });

            if (dbData == null)
            {
                double.TryParse(laborDetails[9][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Profit deduct for special metal").Value;

            double pm3 = hasSpecialPricing && MetalTotals != null  ? facValue * MetalTotals.MaterialExtTotal : 0;
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Profit deduct for special metal",
                CalFactor = facValue,
                MetalCost = pm3,
                SlopeCost = 0,
                SystemCost = 0
            });
            //double pm;
            //double.TryParse(laborDetails[10][0].ToString(), out pm);
            double totalCostM = 0, totalCostS = 0;
            if (MetalTotals != null )
            {
                totalCostM = (totalJobCostM - MetalTotals.SubContractLabor) / MetalMarkup + (MetalTotals.SubContractLabor +
                pm2 + pm3);
            }
            if (SlopeTotals!=null)
            {
                totalCostS = (totalJobCostS - SlopeTotals.SubContractLabor) / SlopeMarkup + (SlopeTotals.SubContractLabor +
                ps2);
            }
            
            double totalCostSy = (totalJobCostSy) / MaterialMarkup + (psy2); //psy4+ psy6
            double totalCostSbLabor = TotalSubContractLaborCostBrkp + psy1;
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Total Cost",
                CalFactor = 0,
                MetalCost = totalCostM,
                SlopeCost = totalCostS,
                SystemCost = totalCostSy,
                SubContractLaborCost = totalCostSbLabor,
                HideCalFactor = System.Windows.Visibility.Hidden
            });


            if (dbData == null)
            {
                double.TryParse(laborDetails[11][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "General Liability (total based on revenue)").Value;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "General Liability",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue / MetalMarkup,
                SlopeCost = totalCostS * facValue / SlopeMarkup,
                SystemCost = totalCostSy * facValue / MaterialMarkup,
                SubContractLaborCost = (totalCostSbLabor * facValue / SubContractProfitMargin)

            });
            double finalMCost = totalCostM + (totalCostM * facValue / MetalMarkup);
            double finalSCost = totalCostS + (totalCostS * facValue / SlopeMarkup);
            double finalSyCost = totalCostSy + (totalCostSy * facValue / MaterialMarkup);
            double finalSubLabCost = totalCostSbLabor + (totalCostSbLabor * facValue / SubContractProfitMargin);

            if (dbData == null)
            {
                double.TryParse(laborDetails[12][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "DirectExpense").Value;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Direct Expense (Gas, Small Tools, Etc,)",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue / MetalMarkup,
                SlopeCost = totalCostS * facValue / SlopeMarkup,
                SystemCost = totalCostSy * facValue / MaterialMarkup,
                SubContractLaborCost = (totalCostSbLabor * facValue / SubContractProfitMargin)

            });
            finalMCost = finalMCost + (totalCostM * facValue / MetalMarkup);
            finalSCost = finalSCost + (totalCostS * facValue / SlopeMarkup);
            finalSyCost = finalSyCost + (totalCostSy * facValue / MaterialMarkup);
            finalSubLabCost = finalSubLabCost + (totalCostSbLabor * facValue / SubContractProfitMargin);
            if (!hasContingencyDisc)
                if (dbData == null)
                {
                    double.TryParse(laborDetails[13][0].ToString(), out facValue);
                }
                else
                    facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Contingency").Value;

            else
                facValue = 0;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Contingency",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue / MetalMarkup,
                SlopeCost = totalCostS * facValue / SlopeMarkup,
                SystemCost = totalCostSy * facValue / MaterialMarkup,
                SubContractLaborCost = totalCostSbLabor * facValue / SubContractProfitMargin
            });
            finalMCost = finalMCost + (totalCostM * facValue / MetalMarkup);
            finalSCost = finalSCost + (totalCostS * facValue / SlopeMarkup);
            finalSyCost = finalSyCost + (totalCostSy * facValue / MaterialMarkup);
            finalSubLabCost = finalSubLabCost + (totalCostSbLabor * facValue / SubContractProfitMargin);
            if (dbData == null)
            {
                double.TryParse(laborDetails[14][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Insurance Increase Fund (5 % on total sale)").Value;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Insurance Increase Fund (5 % on total sale)",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue,
                SlopeCost = totalCostS * facValue,
                SystemCost = totalCostSy * facValue

            });
            finalMCost = finalMCost + (totalCostM * facValue);
            finalSCost = finalSCost + (totalCostS * facValue);
            finalSyCost = finalSyCost + (totalCostSy * facValue);
            if (dbData == null)
            {
                double.TryParse(laborDetails[15][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Fuel/Sur Chg (1-PRICE OF GAS) /-40").Value;

            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Fuel/Sur Chg (1-PRICE OF GAS) /-40",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue,
                SlopeCost = totalCostS * facValue,
                SystemCost = totalCostSy * facValue
            });
            finalMCost = finalMCost + (totalCostM * facValue);
            finalSCost = finalSCost + (totalCostS * facValue);
            finalSyCost = finalSyCost + (totalCostSy * facValue);
            finalSubLabCost = finalSubLabCost + (totalCostSbLabor * facValue);


            if (dbData == null)
            {
                double.TryParse(laborDetails[16][0].ToString(), out facValue);
            }
            else
                facValue = dbData.LaborDBData.FirstOrDefault(x => x.Name == "Add mark up to total job price").Value;

            if (MarkUpPerc != 0)
            {
                facValue = MarkUpPerc / 100;
            }
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Add mark up to total job price",
                CalFactor = facValue,
                MetalCost = totalCostM * facValue,
                SlopeCost = totalCostS * facValue,
                SystemCost = totalCostSy * facValue,
                SubContractLaborCost = totalCostSbLabor * facValue
            });
            finalMCost = finalMCost + (totalCostM * facValue);
            finalSCost = finalSCost + (totalCostS * facValue);
            finalSyCost = finalSyCost + (totalCostSy * facValue);
            finalSubLabCost = finalSubLabCost + (totalCostSbLabor * facValue);
            LCostBreakUp.Add(new CostBreakup
            {
                Name = "Profit Margin",
                CalFactor = 0,
                MetalCost = totalCostM - totalJobCostM,
                SlopeCost = totalCostS - totalJobCostS,
                SystemCost = totalCostSy - totalJobCostSy,
                HideCalFactor = System.Windows.Visibility.Hidden
            });

            TotalMetalPrice = finalMCost; //*(1+markUpPerc/100);
            TotalSlopingPrice = finalSCost; //* (1 + markUpPerc / 100);
            TotalSystemPrice = finalSyCost; //* (1 + mar2kUpPerc / 100);
            TotalSubcontractLabor = finalSubLabCost; // * (1 + markUpPerc / 100);
            CalculateTotalSqFt();
            TotalSale = TotalMetalPrice + TotalSlopingPrice + TotalSystemPrice + TotalSubcontractLabor;
            LaborPerc= Math.Round(AllTabsLaborTotal / TotalSale,2);
            OnPropertyChanged("MaterialPerc");
            OnPropertyChanged("LaborPerc");
            OnPropertyChanged("TotalMetalPrice");
            OnPropertyChanged("TotalSlopingPrice");
            OnPropertyChanged("TotalSystemPrice");
            OnPropertyChanged("TotalSubcontractLabor");
            OnPropertyChanged("TotalSale");
        }

        public virtual void CalculateTotalSqFt()
        {
            if ((totalSqft + riserCount)==0)
            {
                CostperSqftSlope = 0;
                CostperSqftMetal =0;
                CostperSqftMaterial = 0;
                CostperSqftSubContract =0;
            }
            else
            {
                CostperSqftSlope = TotalSlopingPrice / (totalSqft + riserCount);
                CostperSqftMetal = TotalMetalPrice / (totalSqft + riserCount);
                CostperSqftMaterial = TotalSystemPrice / (totalSqft + riserCount);
                CostperSqftSubContract = TotalSubcontractLabor / (totalSqft + riserCount);
            }           
            TotalCostperSqft = CostperSqftSlope + CostperSqftMetal + CostperSqftMaterial + CostperSqftSubContract;
            OnPropertyChanged("CostperSqftSlope");
            OnPropertyChanged("CostperSqftMetal");
            OnPropertyChanged("CostperSqftMaterial");
            OnPropertyChanged("CostperSqftSubContract");
            OnPropertyChanged("TotalCostperSqft");
        }

        #region DBData
        private DBData dbData;
        private void getDatafromDB(string prjName)
        {
            if (prjName.Contains('.'))
            {
                prjName = prjName.Split('.')[0];
            }

           
            isDataAvailable = true;
            if (dbData == null)
            {
                //materialDetails = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "H33:K59");

                dbData = DataSerializerService.DSInstance.deserializeDbData(prjName);
            }   
                laborRate = dbData.FreightDBData.First(x => x.FreightID == 8).FactorValue;
                
                SubContractMarkup =  dbData.LaborDBData.First(x => x.Name == "Profit Margin on subcontract labor").Value; 
               
                MetalMarkup = 1 - dbData.LaborDBData.First(x => x.Name == "Profit Margin Metal").Value; ;

                SlopeMarkup = 1 - dbData.LaborDBData.First(x => x.Name == "Profit Margin Slope").Value; ;

                MaterialMarkup = 1 - dbData.LaborDBData.First(x => x.Name == "Profit Margin Material").Value; 

                SubContractProfitMargin = dbData.LaborDBData.First(x => x.Name == "Profit Margin SubContract").Value; 

            
        }

        public virtual SystemMaterial createSMObjectDB(string matName,string units)
        {
            double cov;
            double mp;
            double w;
            double lfArea = 0;
            double setUpMin = 0; // Setup minimum charges from google sheet, col 6
            double pRateStairs = 0; ///Production rate stairs from google sheet, col 5
            double hprRate = 0;///Horizontal Production rate  from google sheet, col 4
            double vprRate = 0;///Vertical Production rate  from google sheet, col 1
            double sqh = 0;
            double sqv = 0;
            double labrExt = 0;
            double calcHrs = 0;
            double sqStairs = 0;
            double qty = 0;
            string operation = "";
            try
            {
                if (isPrevailingWage)
                {                    
                    prPerc = dbData.FreightDBData.FirstOrDefault(x => x.FactorName == "SlopeProdRate").FactorValue;
                }
                else
                    prPerc = 0;

                //Get Material Data from DB
                MaterialDB matdb = dbData.MaterialDBData.First(x=>x.MaterialName.ToLower()==matName.ToLower());
                vprRate = matdb.ProdRateVertical;
                cov = matdb.Coverage;
                mp = matdb.MaterialPrice;
                w = matdb.Weight;
                setUpMin = matdb.LaborMinCharge;
                pRateStairs = matdb.ProdRateStair;
                hprRate = matdb.ProdRateHorizontal;
                
                pRateStairs = pRateStairs * (1 + prPerc);
                hprRate = hprRate * (1 + prPerc);
                vprRate = vprRate * (1 + prPerc);

                sqv = getSqftAreaVertical(matName);
                sqh = getSqFtAreaH(matName);
                sqStairs = getSqFtStairs(matName);
                lfArea = getlfArea(matName);

                calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs, sqv, vprRate);

                labrExt = CalculateLabrExtn(calcHrs, setUpMin, matName);

                qty = getQuantity(matName, cov, lfArea);
                if (lfArea == -1)
                {
                    lfArea = qty;
                }
                if (sqh == -1)
                {
                    sqh = qty;
                }
                if (sqStairs == -1)
                {
                    sqStairs = qty;
                }
                operation = GetOperation(matName);
                return (new SystemMaterial
                {
                    Name = matName,
                    SMUnits = units,
                    SMSqft = lfArea,
                    Coverage = cov,
                    MaterialPrice = mp,
                    Weight = w,
                    Qty = qty,
                    SMSqftH = sqh,
                    Operation = operation,
                    HorizontalProductionRate = hprRate,
                    StairsProductionRate = pRateStairs,
                    StairSqft = sqStairs,
                    SetupMinCharge = setUpMin,
                    Hours = calcHrs,
                    LaborExtension = labrExt,
                    VerticalProductionRate = vprRate,
                    LaborUnitPrice = getLaborUnitPrice(labrExt, riserCount, totalSqft, sqv, sqh, sqStairs, matName),//labrExt / (riserCount + totalSqft),
                    FreightExtension = w * qty,
                    MaterialExtension = mp * qty,  //chnage for independent projects
                    IsMaterialChecked = getCheckboxCheckStatus(matName),
                    IsMaterialEnabled = getCheckboxEnabledStatus(matName),
                    IncludeInLaborMinCharge = IncludedInLaborMin(matName),
                    IsCheckboxDependent = GetCheckBoxDependency(matName),
                    AllEditable = getEditable()
                });
            }
            catch (Exception e)
            {
                return null;
            }

        }
        #endregion

        #region  Temporary
        private ICommand fillValues;
        [XmlIgnore]
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
            foreach (LaborContract item in SubContractLaborItems)
            {
                if (i>1)
                {
                    break;
                }
                item.UnitConlbrcst = i + 1;
                item.UnitPriceConlbrcst = i + 3;
                i++;
                item.Name = "SubContractLaborItem " + i;
            }
             i = 0;
            foreach (OtherItem item in OtherMaterials)
            {
                item.Quantity = i + 1;
                item.MaterialPrice = i + 6;
                i++;
            }
            i = 0;
            foreach (OtherItem item in OtherLaborMaterials)
            {
                item.Quantity = i + 1;
                item.MaterialPrice = i + 6;
                i++;
            }
            

        }
        #endregion
    }
}
