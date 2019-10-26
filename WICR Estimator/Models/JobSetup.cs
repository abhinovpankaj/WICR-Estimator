using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    
    public class JobSetup : BaseViewModel
    {

        public bool HasTabSwitched { get; set; }
        private double actualPrevailingWage;
        public double ActualPrevailingWage
        {
            get
            {
                return actualPrevailingWage;
            }
            set
            {
                if (value != actualPrevailingWage)
                {
                    actualPrevailingWage = value;
                    OnPropertyChanged("ActualPrevailingWage");
                    UpdateJobSetup();
                }
            }
        }
        public bool IsProjectIndependent { get; set; }
        private string projectname;

        public string FirstCheckBoxLabel { get; set; }
        public string ProjectName
        {
            get { return projectname; }
            set
            {
                if (value != projectname)
                {
                    projectname = value;
                    OnPropertyChanged("ProjectName");
                    if (OnProjectNameChange != null)
                    {
                        OnProjectNameChange(this, EventArgs.Empty);
                    }                    
                }
            }
        }
        public event EventHandler JobSetupChange;
        
        public event EventHandler OnProjectNameChange;
        public string SlopeMaterialName{get;set;}
        public JobSetup()
        { 
            
        }
        public void UpdateJobSetup()
        {
            if (JobSetupChange != null)
            {
                JobSetupChange(this, EventArgs.Empty);
            }
        }
        private bool canAdd(object obj)
        {
            return true;
        }
        private string jobDate;
        public string JobDate
        {
            get { return jobDate; }
            set
            {
                if (value!=jobDate)
                {
                    jobDate = value;
                    OnPropertyChanged("JobDate");
                }
            }
        }
        public System.Windows.Visibility HidePasswordSection { get; set; }
        public string LoginMessage { get; set; }
        private void CanAddMoreMarkup(object obj)
        {
            var passwordBox = obj as PasswordBox;
            var password = passwordBox.Password;
            if (password == "737373")
            {
                MinMarkUp = -50;
                passwordBox.Password = "";
                OnPropertyChanged("MinMarkUp");
                LoginMessage = "You can add MarkUp less than -10%";
                HidePasswordSection = System.Windows.Visibility.Hidden;
                OnPropertyChanged("HidePasswordSection");
            }
            else
            {
                passwordBox.Password = "";
                LoginMessage = "Incorrect Password.";
                
            }
            OnPropertyChanged("LoginMessage");
        }

        private string projectDelayFactor;
        public string ProjectDelayFactor
        {
            get { return projectDelayFactor; }
            set
            {
                if (value!= projectDelayFactor)
                {
                    projectDelayFactor = value;
                    OnPropertyChanged("ProjectDelayFactor");
                    UpdateJobSetup();
                }
            }
        }
        private string specialProductName;
        public string SpecialProductName
        {
            get { return specialProductName; }
            set
            {
                if (value!=specialProductName)
                {
                    specialProductName = value;
                    OnPropertyChanged("SpecialProductName");
                    
                    if (OnProjectNameChange != null)
                    {
                        OnProjectNameChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        private bool allowMoreMarkup;
        public bool AllowMoreMarkUp
        {
            get { return allowMoreMarkup; }
            set
            {
                if (value!=allowMoreMarkup)
                {
                    allowMoreMarkup = value;
                    OnPropertyChanged("AllowMoreMarkUp");
                    if (!value)
                    {
                        MinMarkUp = -10;
                        LoginMessage = "";
                        OnPropertyChanged("MinMarkUp");
                        OnPropertyChanged("LoginMessage");
                        HidePasswordSection = System.Windows.Visibility.Hidden;
                        
                    }
                    else
                        HidePasswordSection = System.Windows.Visibility.Visible;
                    OnPropertyChanged("HidePasswordSection");
                }
            }
        }

        public string SqftLabel { get; set; }
        private double totalsalesTemp;
        public double TotalSalesCostTemp
        {
            get
            {
                return totalsalesTemp;
            }
            set
            {
                totalsalesTemp = value;
                OnPropertyChanged("TotalSalesCostTemp");
            }
        }

        protected virtual void OnJobSetupChanged(EventArgs e)
        {
            JobSetupChange?.Invoke(this, e);
        }
        public JobSetup(string name)            
        {
            ProjectName = name;
            GetOriginalName();
            if (originalName == "Paraseal")
            {
                SqftLabel = "SQ FT OF VERTICAL CONCRETE WALLS";
            }
            else if (originalName == "Paraseal LG")
            {
                SqftLabel = "SQ FT OF VERTICAL LAGGING WALLS";
            }
            else if (originalName == "201" || originalName == "250 GC"|| originalName=="Xypex")
            {
                SqftLabel = "TOTAL SQ FT HORIZONTAL CONCRETE";
            }
            else if (originalName == "Dual Flex")
                SqftLabel = "TOTAL SQ FT HORIZONTAL (NEOBOND ANTI-FRACTURE)";
            else if (originalName == "860 Carlisle")
                SqftLabel = "TOTAL SQ FT CONCRETE DECKS";
            else if (originalName == "Westcoat Epoxy")
                SqftLabel = "TOTAL SQ FT FLOOR";
            else if (originalName == "Polyurethane Injection Block")
                SqftLabel = "TOTAL SQ FT HORIZONTAL CONCRETE FLOOR";
            else
                SqftLabel = "Total Sqft";

            if (originalName=="Pedestrian System" ||originalName=="Parking Garage"||originalName=="Tufflex" ||originalName=="201" || originalName == "250 GC"
                || originalName == "UPI BT")
            {
                IsNewPlywood = false;
                SqftLabel = "Total Sqft Horizontal Concrete";
            }


            if (originalName=="Desert Crete")
            {
                IsJobSpecifiedByArchitect = true;
                
            }
            HidePasswordSection = System.Windows.Visibility.Collapsed;
            IsApprovedForSandCement = true;
            IsPrevalingWage = false;
            HasDiscount = false;
            StairWidth = 4.5;
            //TotalSqft = 1000;
            //RiserCount = 30;
                       
            //DeckPerimeter = 300;
            ////WeatherWearType = "Weather Wear";
            //DeckCount = 1;
            VendorName = "Chivon";
            MaterialName = "24ga. Galvanized Primed Steel";
            EnableMoreMarkupCommand = new DelegateCommand(CanAddMoreMarkup, canAdd);
            MinMarkUp = -10;
            AllowMoreMarkUp = false;
            FirstCheckBoxLabel = "Approved for Sand & Cement ?";
            ProjectDelayFactor = "0-3 Months";
            UpdateJobSetup();
        }

        public double MinMarkUp { get; set; }
        
        private DelegateCommand enableMMCommand;

        [IgnoreDataMember]
        public DelegateCommand EnableMoreMarkupCommand
        {
            get { return enableMMCommand; }
            set
            {
                if (value!=enableMMCommand)
                {
                    enableMMCommand = value;
                    OnPropertyChanged("EnableMoreMarkupCommand");
                }
            }
        }

        private bool isContingencyEnabled;
        public bool IsContingencyEnabled
        {
            get { return isContingencyEnabled; }
            set
            {
                if (value != isContingencyEnabled)
                {
                    isContingencyEnabled = value;
                    OnPropertyChanged("IsContingencyEnabled");
                    UpdateJobSetup();
                }
            }
        }
        string originalName;
        public void GetOriginalName()
        {
           if (ProjectName.Contains('.'))
                originalName = ProjectName.Split('.')[0];
            else
                originalName = ProjectName;
        }

        public string DeckLabel
        {
            get
            {                
                if (originalName == "Weather Wear" || originalName == "Weather Wear Rehab" || originalName == "Reseal all systems")
                    return "Linear Footage of Deck Perimeter";
                else if (originalName == "Resistite" || originalName == "Multicoat")
                    return "LINEAR FOOTAGE OF DECK TO WALL DETAIL";
                else if (originalName == "Dexcellent II")
                    return "LINEAR FOOTAGE OF DECK TO WALL METAL";
                else if (originalName == "Paraseal")
                    return "LF OF PERIMETER FOOTING (STANDARD PARAGRANULAR DETAIL AND TERM BAR)";
                else if (originalName == "Paraseal LG")
                    return "LF OF PERIMETER FOOTING (adds term bar only )";
                else if (originalName == "Tufflex" || originalName == "201" || originalName == "250 GC" || originalName == "UPI BT")
                    return "LINEAR FOOTAGE OF PERIMETER (DECKS)";
                else if (originalName == "860 Carlisle")
                    return "LINEAR FOOTAGE OF DECK TO WALL METAL(fluid applied detail)";
                else if (originalName == "Dual Flex")
                    return "PERIMETER";
                else if (originalName == "Desert Crete")
                    return "LINEAR FOOTAGE OF DECK TO WALL METAL (COVE BASE)";
                else if (originalName == "Polyurethane Injection Block")
                    return "LINEAR FOOTAGE OF COLD JOINTS";
                else if (originalName == "Westcoat Epoxy")
                    return "LINEAR FOOTAGE OF 3/16 inch COVE BASE";
                else if (originalName == "Block Wall")
                    return "LINEAR FOOTAGE OF INSIDE CORNERS";
                else
                    return "Lf Perimeter for Burlap and Membrane";
            }
        }
        
        public string VerticalSqftLabel
        {
            get
            {
                if (originalName == "Dual Flex")
                {
                    return "TOTAL SQ FT VERTICAL WALLS (MEMBRANE ONLY)";
                }
                else if (originalName == "Polyurethane Injection Block")
                    return "TOTAL SQ FT VERTICAL BLOCK (EXCL 1ST 2 COURSES)";
                else
                    return "TOTAL SQ FT VERTICAL (BLOCK WALL)";
            }
        }
        private string jobName;
        public string JobName
        {
            get
            {
                return jobName;
            }
            set
            {
                if (value!=jobName)
                {
                    jobName = value;
                    OnPropertyChanged("JobName");
                }
            }
        }
        
        private string vendorName;
        public string VendorName
        {
            get
            {
                return vendorName;
            }
            set
            {
                if (value != vendorName)
                {
                    vendorName = value;
                    OnPropertyChanged("VendorName");
                    UpdateJobSetup();

                }
            }
        }
        private string bidBy;
        public string BidBy
        {
            get
            {
                return bidBy;
            }
            set
            {
                if (value != bidBy)
                {
                    bidBy = value;
                    OnPropertyChanged("BidBy");
                }
            }
        }
        private string workArea;
        public string WorkArea
        {
            get
            {
                return workArea;
            }
            set
            {
                if (value!=workArea)
                {

                    workArea = value;
                    OnPropertyChanged("WorkArea");
                    if (OnProjectNameChange != null)
                    {
                        OnProjectNameChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        private DateTime? selecteddate=DateTime.Now;
        public DateTime? SelectedDate
        {
            get
            {
                return selecteddate;
            }
            set
            {
                if (value != selecteddate)
                {
                    selecteddate = value;                   
                    OnPropertyChanged("SelectedDate");
                }
            }
        }
        private double totalSqft;
        public double TotalSqft
        {
            get
            {
                return totalSqft;
            }
            set
            {
                if (value != totalSqft)
                {
                    totalSqft = value;                    
                    if (value>=1000||TotalSqftVertical>=1000 ||TotalSqftPlywood>=1000 )
                    {
                        VHasContingencyDisc = false;
                        IsContingencyEnabled = true;
                    }

                         
                    else
                    {
                        IsContingencyEnabled = false;
                        VHasContingencyDisc = false;
                    }
                    OnPropertyChanged("IsContingencyEnabled");
                    OnPropertyChanged("VHasContingencyDisc");
                    OnPropertyChanged("TotalSqft");
                    UpdateJobSetup();
                   
                }
            }
        }

        private bool vhasContingencyDisc;
        public bool VHasContingencyDisc
        {
            get { return vhasContingencyDisc; }
            set
            {
                if (value != vhasContingencyDisc)
                {
                    vhasContingencyDisc = value;
                    OnPropertyChanged("VHasContingencyDisc");
                    UpdateJobSetup();
                }
            }
        }

        private double deckPerimeter;
        public double DeckPerimeter
        {
            get
            {
                return deckPerimeter;
            }
            set
            {
                if (value != deckPerimeter)
                {
                    deckPerimeter = value;
                    OnPropertyChanged("DeckPerimeter");
                    UpdateJobSetup();
                }
            }
        }
        
        
        private int riserCount;
        public int RiserCount
        {
            get
            {
                return riserCount;
            }
            set
            {
                if (value != riserCount)
                {
                    riserCount = value;
                    OnPropertyChanged("RiserCount");
                    UpdateJobSetup();
                }
            }
        }
        private int deckCount;
        public int DeckCount
        {
            get
            {
                return deckCount;
            }
            set
            {
                if (value != deckCount)
                {
                    deckCount = value;
                    OnPropertyChanged("DeckCount");
                    UpdateJobSetup();
                }
            }
        }
        private bool isApprovedForSandCement;
        public bool IsApprovedForSandCement
        {
            get
            {
                return isApprovedForSandCement;
            }
            set
            {
                if (value != isApprovedForSandCement)
                {
                    isApprovedForSandCement = value;
                    //IsReseal = value;
                    
                    OnPropertyChanged("IsApprovedForSandCement");
                    UpdateJobSetup();
                }
            }
        }
        private bool isPrevalingWage;
        public bool IsPrevalingWage
        {
            get
            {
                return isPrevalingWage;
            }
            set
            {
                if (value != isPrevalingWage)
                {
                    isPrevalingWage = value;
                    OnPropertyChanged("IsPrevalingWage");
                    UpdateJobSetup();
                }
            }
        }
        private double laborRate;
        public double LaborRate
        {
            get
            {
                //if (laborRate==0)
                //{
                //    var rate=DataSerializer.DSInstance.deserializeGoogleData(DataType.Rate,projectname);
                //    double.TryParse(rate[0][0].ToString(),out laborRate);
                //}
                return laborRate;
            }
            set
            {
                if (value!=laborRate)
                {
                    laborRate = value;
                    OnPropertyChanged("LaborRate");
                    UpdateJobSetup();
                }
            }
        }
 
        private bool hasSpecialMaterial;
        public bool HasSpecialMaterial
        {
            get
            {
                return hasSpecialMaterial;
            }
            set
            {
                if (value != hasSpecialMaterial)
                {
                    hasSpecialMaterial = value;
                    OnPropertyChanged("HasSpecialMaterial");
                    UpdateJobSetup();
                }
            }
        }
        private bool isFlashingRequired;
        public bool IsFlashingRequired
        {
            get
            {
                return isFlashingRequired;
            }
            set
            {
                if (value != isFlashingRequired)
                {
                    isFlashingRequired = value;
                    OnPropertyChanged("IsFlashingRequired");
                    UpdateJobSetup();
                }
            }
        }
        private bool hasSpecialPricing;
        public bool HasSpecialPricing
        {
            get
            {
                return hasSpecialPricing;
            }
            set
            {
                if (value != hasSpecialPricing)
                {
                    hasSpecialPricing = value;
                    OnPropertyChanged("HasSpecialPricing");
                    UpdateJobSetup();
                }
            }
        }
        private bool hasDiscount;
        public bool HasDiscount
        {
            get
            {
                return hasDiscount;
            }
            set
            {
                if (value != hasDiscount)
                {
                    hasDiscount = value;
                    OnPropertyChanged("HasDiscount");
                    UpdateJobSetup();
                }
            }
        }
        private double markupPercentage;
        public double MarkupPercentage
        {
            get
            {
                return markupPercentage;
            }
            set
            {
                if (value != markupPercentage)
                {
                    markupPercentage = value;
                    OnPropertyChanged("MarkupPercentage");
                    UpdateJobSetup();
                }
            }
        }
        private string materialName;
        public string MaterialName
        {
            get
            {
                return materialName;
            }
            set
            {
                if (value != materialName)
                {
                    materialName = value;
                    OnPropertyChanged("MaterialName");
                    UpdateJobSetup();

                }
            }
        }
        private double stairWidth;
        public double StairWidth
        {
            get
            {
                return stairWidth;
            }
            set
            {
                if (value != stairWidth)
                {
                    stairWidth = value;
                    OnPropertyChanged("StairWidth");
                    UpdateJobSetup();
                }
            }
        }

        #region DesertCrete
        private bool? isJobSpecifiedByArchitect;
        public bool? IsJobSpecifiedByArchitect
        {
            get { return isJobSpecifiedByArchitect; }
            set
            {
                if (value!=isJobSpecifiedByArchitect)
                {
                    isJobSpecifiedByArchitect = value;
                    OnPropertyChanged("IsJobSpecifiedByArchitect");
                    UpdateJobSetup();
                }
            }
        }
        
        public System.Windows.Visibility IsSystemOverConcreteVisible
        {
            get
            {
                if (originalName == "Dexcellent II"|| originalName == "Endurokote" || originalName == "Pli-Dek" || originalName == "Desert Crete")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        private bool isSystemOverConcrete;
        public bool IsSystemOverConcrete
        {
            get { return isSystemOverConcrete; }
            set
            {
                if (value != isSystemOverConcrete)
                {
                    isSystemOverConcrete = value;
                    OnPropertyChanged("IsSystemOverConcrete");
                    UpdateJobSetup();
                }
            }
        }
        public System.Windows.Visibility IsJobByArchitectVisible
        {
            get
            {
                if (IsJobSpecifiedByArchitect != null)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        #endregion

        #region UPI 
        private bool? isNewPlywood;
        public bool? IsNewPlywood
        {
            get { return isNewPlywood; }
            set
            {
                if (value != isNewPlywood)
                {
                    isNewPlywood = value;
                    OnPropertyChanged("IsNewPlywood");
                    UpdateJobSetup();
                }
            }
        }
        public string StairRiserText
        {
            get
            {
                if (originalName == "Paraseal")
                {
                    return "# PENETRATIONS or DRAINS";
                }
                else if (originalName == "201" || originalName == "250 GC" || originalName == "Dexcellent II" ||
                    originalName == "860 Carlisle" || originalName == "UPI BT" || originalName == "Westcoat Epoxy" ||
                    originalName == "Polyurethane Injection Block" || originalName == "Xypex")
                    return "# RISERS (3.5-4 FT WIDE)";
                else if (originalName == "Paraseal LG")
                    return "TIE BACKS (block outs must be priced separately)";
                else if (originalName == "Dual Flex")
                    return "#  RISERS INCLUDES METAL (3.5-4 FT WIDE)";
                else
                    return "Stair Risers - Confirm stair width";
            }
        }
        public string DeckCountText
        {
            get
            {
                if (originalName == "Paraseal")
                {
                    return "SQ FT OF BETWEEN SLAB MEMBRANE (CONCRETE)";
                }
                else if (originalName == "Paraseal LG")
                {
                    return "ADDITIONAL SUPER STOP FOR COLD JOINTS IN FIELD OR EXTRA LIFTS(LF)";
                }
                else
                    return "# Decks";
            }
        }
        public System.Windows.Visibility IsSandCementVisible
        {
            get
            {
                if (originalName == "Pedestrian System" || originalName == "Parking Garage" || originalName == "Paraseal" || originalName == "Tufflex" ||
                    originalName == "Paraseal LG" || originalName=="860 Carlisle"||originalName=="UPI BT"||originalName=="Westcoat Epoxy"
                    ||originalName== "Polyurethane Injection Block" || originalName == "Xypex")
                {
                    return System.Windows.Visibility.Collapsed;
                }
                else
                    return System.Windows.Visibility.Visible;

            }
        }
        public System.Windows.Visibility IsResealVisible
        {
            get
            {
                if (originalName == "Pedestrian System" || originalName == "Parking Garage")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;

            }
        }
        public System.Windows.Visibility IsSqftConcreteVisible
        {
            get
            {
                if (originalName == "Tufflex"||originalName=="UPI BT")
                {
                    return System.Windows.Visibility.Collapsed;
                }
                else
                    return System.Windows.Visibility.Visible;

            }
        }
        public System.Windows.Visibility IsNewPlywoodVisible
        {
            get
            {
                
                if (isNewPlywood != null)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        
        public System.Windows.Visibility IsPlywoodSqftVisible
        {
            get
            {
                if (originalName == "Pedestrian System" || originalName == "Parking Garage" 
                    || originalName == "Tufflex" || originalName == "201" || originalName == "250 GC"
                    ||originalName=="860 Carlisle"||originalName == "UPI BT")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        private bool isReseal;
        public bool IsReseal
        {
            get { return isReseal; }
            set
            {
                if (value != isReseal)
                {
                    isReseal = value;
                    OnPropertyChanged("IsReseal");
                    UpdateJobSetup();
                }
            }
        }
        private double totalSqftPlywood;
        public double TotalSqftPlywood
        {
            get { return totalSqftPlywood; }
            set
            {
                if (value != totalSqftPlywood)
                {
                    totalSqftPlywood = value;
                    if (value >= 1000 || TotalSqftVertical >= 1000 || TotalSqft >= 1000)
                    {
                        VHasContingencyDisc = false;
                        IsContingencyEnabled = true;
                    }


                    else
                    {
                        IsContingencyEnabled = false;
                        VHasContingencyDisc = false;
                    }
                    OnPropertyChanged("IsContingencyEnabled");
                    OnPropertyChanged("VHasContingencyDisc");
                    OnPropertyChanged("TotalSqftPlywood");
                    UpdateJobSetup(); //JobSetupChange(this, EventArgs.Empty);
                    
                }
            }
        }

        #endregion

        #region Resistite
        public string LinearFootageText
        {
            get
            {
                if (originalName == "Paraseal")
                {
                    return "LINEAR FOOTAGE OF UV PROTECTION AT WALL (801)";
                }
                else if (originalName == "860 Carlisle")
                    return "LINEAR FOOTAGE OF FOOTING";
                else if (originalName == "Dual Flex")
                    return "LINEAR FOOTAGE OF DECK TO WALL";
                else
                    return "LINEAR FOOTAGE OF COPING";
            }
        }
        private double linearCopingFootage;
        public double LinearCopingFootage
        {
            get { return linearCopingFootage; }
            set
            {
                if (value != linearCopingFootage)
                {
                    linearCopingFootage = value;
                    OnPropertyChanged("LinearCopingFootage");
                    UpdateJobSetup();
                }
            }
        }
        public System.Windows.Visibility IslinearCopingFootageVisible
        {
            get
            {
                if (originalName=="Resistite" ||originalName=="Multicoat" || 
                    originalName=="Paraseal"||originalName== "860 Carlisle" || originalName=="Dual Flex")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        
        
        public System.Windows.Visibility Is860SectionVisible
        {
            get
            {
                if (originalName == "860 Carlisle")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        #endregion

        #region Paraseal

        private double additonalTermbarLF;
        public double AdditionalTermBarLF
        {
            get { return additonalTermbarLF; }
            set
            {
                if (value != additonalTermbarLF)
                {
                    additonalTermbarLF = value;
                    OnPropertyChanged("AdditionalTermBarLF");
                    UpdateJobSetup();
                }
            }
        }
        private bool superStopAtFooting;
        public bool SuperStopAtFooting
        {
            get { return superStopAtFooting; }
            set
            {
                if (value != superStopAtFooting)
                {
                    superStopAtFooting = value;
                    OnPropertyChanged("SuperStopAtFooting");
                    UpdateJobSetup();
                }
            }
        }
        private double insideOutsideCornerDetails;
        public double InsideOutsideCornerDetails
        {
            get { return insideOutsideCornerDetails; }
            set
            {
                if (value != insideOutsideCornerDetails)
                {
                    insideOutsideCornerDetails = value;
                    OnPropertyChanged("InsideOutsideCornerDetails");
                    UpdateJobSetup();
                }
            }
        }
        
        public System.Windows.Visibility IsParasealSectionVisible
        {
            get
            {

                if (originalName=="Paraseal")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        #endregion


        #region Paraseal LG
        public System.Windows.Visibility IsParasealLGSectionVisible
        {
            get
            {

                if (originalName == "Paraseal LG")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        private double rakerCornerBases;
        public double RakerCornerBases
        {
            get { return rakerCornerBases; }
            set
            {
                if (value != rakerCornerBases)
                {
                    rakerCornerBases = value;
                    OnPropertyChanged("RakerCornerBases");
                    UpdateJobSetup();
                }
            }
        }
        private double cementBoardDetail;
        public double CementBoardDetail
        {
            get { return cementBoardDetail; }
            set
            {
                if (value != cementBoardDetail)
                {
                    cementBoardDetail = value;
                    OnPropertyChanged("CementBoardDetail");
                    UpdateJobSetup();
                }
            }
        }
        private double rockPockets;
        public double RockPockets
        {
            get { return rockPockets; }
            set
            {
                if (value != rockPockets)
                {
                    rockPockets = value;
                    OnPropertyChanged("RockPockets");
                    UpdateJobSetup();
                }
            }
        }
        private double parasealFoundation;
        public double ParasealFoundation
        {
            get { return parasealFoundation; }
            set
            {
                if (value != parasealFoundation)
                {
                    parasealFoundation = value;
                    UpdateJobSetup();
                }
            }
        }
        private double rearMidLagging;
        public double RearMidLagging
        {
            get { return rearMidLagging; }
            set
            {
                if (value != rearMidLagging)
                {
                    rearMidLagging = value;
                    OnPropertyChanged("RearMidLagging");
                    UpdateJobSetup();
                }
            }
        }
        #endregion
        #region 201
        public System.Windows.Visibility Is201SectionVisible
        {
            get
            {

                if (originalName == "201"||originalName=="250 GC")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        
        public System.Windows.Visibility IsSqftVerticleVisible
        {
            get
            {

                if (originalName == "201" || originalName == "250 GC"||originalName== "860 Carlisle" || originalName=="Dual Flex"||originalName== "Polyurethane Injection Block"
                    ||originalName=="Xypex")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        private double totalSqftVertical;
        public double TotalSqftVertical
        {
            get { return totalSqftVertical; }
            set
            {
                if (value != totalSqftVertical)
                {
                    totalSqftVertical = value;
                    if (value >= 1000||TotalSqft>=1000||TotalSqftPlywood>=1000)
                    {
                        VHasContingencyDisc = false;
                        IsContingencyEnabled = true;
                    }


                    else
                    {
                        IsContingencyEnabled = false;
                        VHasContingencyDisc = false;
                    }
                    OnPropertyChanged("IsContingencyEnabled");
                    OnPropertyChanged("VHasContingencyDisc");
                    OnPropertyChanged("TotalSqftVertical");
                    UpdateJobSetup();
                }
            }
        }

        private double termBarLF;
        public double TermBarLF
        {
            get { return termBarLF; }
            set
            {
                if (value != termBarLF)
                {
                    termBarLF = value;
                    OnPropertyChanged("TermBarLF");
                    UpdateJobSetup();
                }
            }
        }
        private double rebarPrepWallsLF;
        public double RebarPrepWallsLF
        {
            get { return rebarPrepWallsLF; }
            set
            {
                if (value != rebarPrepWallsLF)
                {
                    rebarPrepWallsLF = value;
                    OnPropertyChanged("RebarPrepWallsLF");
                    UpdateJobSetup();
                }
            }
        }
        private double superStopLF;
        public double SuperStopLF
        {
            get { return superStopLF; }
            set
            {
                if (value != superStopLF)
                {
                    superStopLF = value;
                    OnPropertyChanged("SuperStopLF");
                    UpdateJobSetup();
                }
            }
        }
        private double penetrations;
        public double Penetrations
        {
            get { return penetrations; }
            set
            {
                if (value != penetrations)
                {
                    penetrations = value;
                    OnPropertyChanged("Penetrations");
                    UpdateJobSetup();
                }
            }
        }
        #endregion

        #region DualFlex
        public System.Windows.Visibility DualFlexVisible
        {
            get
            {
                if (originalName == "Dual Flex")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }

        private bool hasQuarterMortarBed;
        public bool HasQuarterMortarBed
        {
            get { return hasQuarterMortarBed; }
            set
            {
                if (value != hasQuarterMortarBed)
                {
                    hasQuarterMortarBed = value;
                    OnPropertyChanged("HasQuarterMortarBed");
                    UpdateJobSetup();
                }
            }
        }
        private bool hasQuarterLessMortarBed;
        public bool HasQuarterLessMortarBed
        {
            get { return hasQuarterLessMortarBed; }
            set
            {
                if (value != hasQuarterLessMortarBed)
                {
                    hasQuarterLessMortarBed = value;
                    OnPropertyChanged("HasQuarterLessMortarBed");
                    UpdateJobSetup();
                }
            }
        }
        private bool hasElastex;
        public bool HasElastex
        {
            get { return hasElastex; }
            set
            {
                if (value != hasElastex)
                {
                    hasElastex = value;
                    OnPropertyChanged("HasElastex");
                    UpdateJobSetup();
                }
            }
        }
        private bool hasEasyAccess;
        public bool HasEasyAccess
        {
            get { return hasEasyAccess; }
            set
            {
                if (value != hasEasyAccess)
                {
                    hasEasyAccess = value;
                    OnPropertyChanged("HasEasyAccess");
                    UpdateJobSetup();
                }
            }
        }
        #endregion
    }
}
