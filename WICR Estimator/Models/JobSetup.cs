using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    public class JobSetup:BaseViewModel
    {
        private string ProjectName;
        public static event EventHandler OnJobSetupChange;
        public JobSetup()
        { 
            IsApprovedForSandCement = false;
            IsPrevalingWage = false;
            HasDiscount = false;
            StairWidth = 4.5;
            TotalSqft = 1000;
            RiserCount =30;
            WeatherWearType = "";
            DeckPerimeter = 300;
            WeatherWearType = "Weather Wear";
            DeckCount = 1;
            VendorName = "Chivon";
            MaterialName = "Copper";
            EnableMoreMarkupCommand = new DelegateCommand(CanAddMoreMarkup, canAdd);
            MinMarkUp = -10;
            AllowMoreMarkUp = false;
            HidePasswordSection = System.Windows.Visibility.Hidden;
        }

        private bool canAdd(object obj)
        {
            return true;
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
        public JobSetup(string name)
            :this()
        {
            ProjectName = name;
        }

        public double MinMarkUp { get; set; }

        private DelegateCommand enableMMCommand;
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
                }
            }
        }
        private string weatherWearType;
        public string WeatherWearType
        {
            get { return weatherWearType; }
            set
            {
                if (value!=weatherWearType)
                {
                    weatherWearType = value;
                    OnPropertyChanged("WeatherWearType");
                    OnPropertyChanged("VendorName");
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
                if (ProjectName == "Weather Wear")
                    return "Linear Footage of Deck Perimeter";
                else
                    return "Lf Perimeter for Burlap and Membrane";
            }
        }
        public System.Windows.Visibility ShowWeatherWearDD
        {
            get
            {
                if (ProjectName == "Dexotex Weather Wear")
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
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
                if (laborRate==0)
                {
                    var rate=DataSerializer.DSInstance.deserializeGoogleData(DataType.Rate,"Weather Wear");
                    double.TryParse(rate[0][0].ToString(),out laborRate);
                }
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
    }
}
