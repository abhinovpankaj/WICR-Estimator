using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    
    
    public class JobSetup : BaseViewModel
    {

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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
                }
            }
        }
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
        public event EventHandler OnJobSetupChange;
        
        public event EventHandler OnProjectNameChange;
        public string SlopeMaterialName{get;set;}
        public JobSetup()
        { 
            
        }

        private bool canAdd(object obj)
        {
            return true;
        }
        private string jobDate=DateTime.Today.ToShortDateString();
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    ProjectName = value;
                    if (ProjectName=="")
                    {
                        //ProjectName = "Dexotex Weather Wear";
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
        public JobSetup(string name)            
        {
            ProjectName = name;
            if (name=="Paraseal")
            {
                SqftLabel = "SQ FT OF VERTICAL CONCRETE WALLS";
            }
            else
                SqftLabel = "Total Sqft";

            if (name=="Pedestrian System" ||name=="Parking Garage"||name=="Tufflex")
            {
                IsNewPlywood = false;
                SqftLabel = "Total Sqft Concrete";
            }
            if (name=="Desert Crete")
            {
                IsJobSpecifiedByArchitect = true;
                
            }
            HidePasswordSection = System.Windows.Visibility.Collapsed;
            IsApprovedForSandCement = true;
            IsPrevalingWage = false;
            HasDiscount = false;
            StairWidth = 4.5;
            TotalSqft = 1000;
            RiserCount = 30;
                       
            DeckPerimeter = 300;
            //WeatherWearType = "Weather Wear";
            DeckCount = 1;
            VendorName = "Chivon";
            MaterialName = "24ga. Galvanized Primed Steel";
            EnableMoreMarkupCommand = new DelegateCommand(CanAddMoreMarkup, canAdd);
            MinMarkUp = -10;
            AllowMoreMarkUp = false;
            FirstCheckBoxLabel = "Approved for Sand & Cement ?";          
            
        }

        public double MinMarkUp { get; set; }
        
        private DelegateCommand enableMMCommand;
        [XmlIgnore]
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

        
        private bool hasContingencyDisc;
        public bool HasContingencyDisc
        {
            get { return hasContingencyDisc; }
            set
            {
                if (value!= hasContingencyDisc)
                {
                    hasContingencyDisc = value;
                    OnPropertyChanged("HasContingencyDisc");
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        
        public string DeckLabel
        {
            get
            {
                if (ProjectName == "Weather Wear" || ProjectName == "Weather Wear Rehab")
                    return "Linear Footage of Deck Perimeter";
                else if (ProjectName == "Resistite" || ProjectName == "Multicoat")
                    return "LINEAR FOOTAGE OF DECK TO WALL DETAIL";
                else if (ProjectName == "Paraseal")
                    return "LF OF PERIMETER FOOTING (STANDARD PARAGRANULAR DETAIL AND TERM BAR)";
                else if (ProjectName == "Tufflex")
                    return "LINEAR FOOTAGE OF PERIMETER (DECKS)";
                else
                    return "Lf Perimeter for Burlap and Membrane";
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
                    
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
        private DateTime date;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                if (value != date)
                {
                    date = value;
                    OnPropertyChanged("Date");
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

                    
                    if (value>=1000)
                        HasContingencyDisc = true;
                    else
                        HasContingencyDisc = false;
                    OnPropertyChanged("HasContingencyDisc");
                    OnPropertyChanged("TotalSqft");
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }

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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        public string StairRiserText
        {
            get
            {
                if (ProjectName == "Paraseal")
                {
                    return "# PENETRATIONS or DRAINS";
                }
                else
                    return "Stair Risers - Confirm stair width";
            }
        }
        public string DeckCountText
        {
            get
            {
                if (ProjectName == "Paraseal")
                {
                    return "SQ FT OF BETWEEN SLAB MEMBRANE (CONCRETE)";
                }
                else
                    return "# Decks";
            }
        }
        public System.Windows.Visibility IsSandCementVisible
        {
            get
            {
                if (ProjectName == "Pedestrian System" || ProjectName == "Parking Garage")
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    OnPropertyChanged("TotalSqftPlywood");
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
                }
            }
        }

        #endregion

        #region Resistite
        public string LinearFootageText
        {
            get
            {
                if (ProjectName == "Paraseal")
                {
                    return "LINEAR FOOTAGE OF UV PROTECTION AT WALL (801)";
                }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        public System.Windows.Visibility IslinearCopingFootageVisible
        {
            get
            {
                if (ProjectName=="Resistite" ||ProjectName=="Multicoat" || ProjectName=="Paraseal")
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
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
                    if (OnJobSetupChange != null)
                    {
                        OnJobSetupChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        public System.Windows.Visibility IsParasealSectionVisible
        {
            get
            {

                if (ProjectName=="Paraseal")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        #endregion
    }
}
