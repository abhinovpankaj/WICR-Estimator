using MyToolkit.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    
    public class JobSetup : UndoRedoObservableObject
    {

        #region Formula
        bool calledFromBackend;
        public object this[string propertyName]
        {
            get
            {
                // probably faster without reflection:
                // like:  return Properties.Settings.Default.PropertyValues[propertyName] 
                // instead of the following
                Type myType = typeof(JobSetup);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                Type myType = typeof(JobSetup);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                calledFromBackend = true;
                myPropInfo.SetValue(this, value, null);

            }
        }

        private DataPresentor selectedData;

        public DataPresentor SelectedData
        {
            get { return selectedData; }
            set
            {
                

                
                if (value!=null)
                {
                    //Set(ref selectedData, value);
                    selectedData = value;
                    RaisePropertyChanged("SelectedData");
                    
                    Formula= value.Formula ;
                    
                    Comment = value.Comment;
                    CalculateValue();
                }
                
            }
        }

        private string comment;
        [IgnoreDataMember]
        public string Comment 
        {
            get { return comment; }
            set
            {
                comment = value;
                SelectedData.Comment = value;
                RaisePropertyChanged("Comment");
            }
        }
        
        private string formula;
        [IgnoreDataMember]
        public string Formula
        {
            get { return formula; }
            set
            {
                formula = value;
                RaisePropertyChanged("Formula");
                //Set(ref formula, value);
                if (SelectedData!=null)
                {
                    SelectedData.Formula = value;
                    CalculateValue();
                }
                
                
            }
        }

        public void CalculateValue()
        {
            try
            {
                SelectedData.CalculatedValue = Convert.ToDouble(new DataTable().Compute(SelectedData.Formula ?? "0", null));
            }
            catch (Exception ex)
            {

            }
        }

        private BindingList<DataPresentor> data;

        public BindingList<DataPresentor> ZData
        {
            get { return data; }
            set
            {
                data = value;
                RaisePropertyChanged("ZData");
            }
        }
        private void PopulateCalculatbleTextBox()
        {
            ZData = new BindingList<DataPresentor>
            {
                new DataPresentor { Key = "ActualPrevailingWage"},
                new DataPresentor { Key = "TotalSqftPlywood"},
                new DataPresentor { Key = "TotalSqft"},
                new DataPresentor { Key = "TotalSqftVertical"},
                new DataPresentor { Key = "LinearCopingFootage"},
                new DataPresentor { Key = "DeckPerimeter"},
                new DataPresentor { Key = "RiserCount"},
                new DataPresentor { Key = "DeckCount"},
                new DataPresentor { Key = "AdditionalTermBarLF"},
                new DataPresentor { Key = "InsideOutsideCornerDetails"},
                new DataPresentor { Key = "RakerCornerBases"},
                new DataPresentor { Key = "CementBoardDetail"},
                new DataPresentor { Key = "InsideOutsideCornerDetails"},
                new DataPresentor { Key = "RockPockets"},
                new DataPresentor { Key = "ParasealFoundation"},
                new DataPresentor { Key = "RearMidLagging"},
                new DataPresentor { Key = "TermBarLF"},

                new DataPresentor { Key = "RebarPrepWallsLF"},
                new DataPresentor { Key = "SuperStopLF"},
                new DataPresentor { Key = "Penetrations"},
                new DataPresentor { Key = "ParasealFoundation"},
                new DataPresentor { Key = "RearMidLagging"}
                
            };
        }
        #endregion

        private DBData _dbdata;
        public DBData dbData 
        { get { return _dbdata; }
            set
            {
                if (value != _dbdata)
                {
                    //_dbdata = value;
                    //RaisePropertyChanged("dbData");
                    Set(ref _dbdata, value);
                    UpdateJobSetup();
                    UpdateJobSetup();
                }
                
            }
        }

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
                if (value!=actualPrevailingWage)
                {
                    if (ZData != null)
                    {
                        var myData = ZData.FirstOrDefault(x => x.Key == "ActualPrevailingWage");

                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }

                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key== "ActualPrevailingWage")
                //    {
                //        selectedData.Formula = value.ToString();
                //    }
                    
                //}


                Set(ref actualPrevailingWage, value);
                UpdateJobSetup();
            }
        }

        public bool IsProjectIndependent { get; set; }
        private string projectname;

        public string FirstCheckBoxLabel { get; set; }

        private string notesToBill;

        public string NotesToBill
        {
            get { return notesToBill; }
            set
            {
                //if (value != notesToBill)
                //{
                //    notesToBill = value;
                //    OnPropertyChanged("NotesToBill");                  
                //}
                Set(ref notesToBill, value);
                
            }
        }
        public string ProjectName
        {
            get { return projectname; }
            set
            {
                //if (value != projectname)
                //{
                //    projectname = value;
                //    OnPropertyChanged("ProjectName");
                //    if (OnProjectNameChange != null)
                //    {
                //        OnProjectNameChange(this, EventArgs.Empty);
                //    }                    
                //}
                Set(ref projectname, value);
                if (OnProjectNameChange != null)
                {
                    OnProjectNameChange(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler JobSetupChange;
        
        public event EventHandler OnProjectNameChange;
        public string SlopeMaterialName{get;set;}
        public JobSetup()
        { 
            
        }
        //public string toolVersion;
        public void UpdateJobSetup(string version = "")
        {
            if (JobSetupChange != null)
            {
                //toolVersion = version;
                JobSetupChange(this, EventArgs.Empty);
            }
        }

        public bool canAdd(object obj)
        {
            return true;
        }
        private string jobDate;
        public string JobDate
        {
            get { return jobDate; }
            set
            {
                //if (value!=jobDate)
                //{
                //    jobDate = value;
                //    OnPropertyChanged("JobDate");
                //}
                Set(ref jobDate, value);
            }
        }
        public System.Windows.Visibility HidePasswordSection { get; set; }
        public string LoginMessage { get; set; }
        public void CanAddMoreMarkup(object obj)
        {
            var passwordBox = obj as PasswordBox;
            var password = passwordBox.Password;
            if (password == "737373")
            {
                MinMarkUp = -50;
                passwordBox.Password = "";
                // OnPropertyChanged("MinMarkUp");
                RaisePropertyChanged("MinMarkUp");
                LoginMessage = "You can add MarkUp less than -10%";
                HidePasswordSection = System.Windows.Visibility.Hidden;
                //OnPropertyChanged("HidePasswordSection");
                RaisePropertyChanged("HidePasswordSection");
            }
            else
            {
                passwordBox.Password = "";
                LoginMessage = "Incorrect Password.";
                
            }
            //OnPropertyChanged("LoginMessage");
            RaisePropertyChanged("LoginMessage");
        }

        private string projectDelayFactor;
        public string ProjectDelayFactor
        {
            get { return projectDelayFactor; }
            set
            {
                //if (value!= projectDelayFactor)
                //{
                //    projectDelayFactor = value;
                //    OnPropertyChanged("ProjectDelayFactor");
                //    UpdateJobSetup();
                //}
                Set(ref projectDelayFactor, value);
                UpdateJobSetup();
            }
        }
        private string specialProductName;
        public string SpecialProductName
        {
            get { return specialProductName; }
            set
            {
                //if (value!=specialProductName)
                //{
                //    specialProductName = value;
                //    OnPropertyChanged("SpecialProductName");

                //    if (OnProjectNameChange != null)
                //    {
                //        OnProjectNameChange(this, EventArgs.Empty);
                //    }
                //}
                Set(ref specialProductName, value);
            }
        }
        private bool allowMoreMarkup;
        public bool AllowMoreMarkUp
        {
            get { return allowMoreMarkup; }
            set
            {
                //if (value!=allowMoreMarkup)
                //{
                //    allowMoreMarkup = value;
                //    OnPropertyChanged("AllowMoreMarkUp");
                //    if (!value)
                //    {
                //        MinMarkUp = -10;
                //        LoginMessage = "";
                //        OnPropertyChanged("MinMarkUp");
                //        OnPropertyChanged("LoginMessage");
                //        HidePasswordSection = System.Windows.Visibility.Hidden;

                //    }
                //    else
                //        HidePasswordSection = System.Windows.Visibility.Visible;
                //    OnPropertyChanged("HidePasswordSection");
                //}
                Set(ref allowMoreMarkup, value);
                if (!value)
                {
                    MinMarkUp = -10;
                    LoginMessage = "";
                    RaisePropertyChanged("MinMarkUp");
                    RaisePropertyChanged("LoginMessage");
                    HidePasswordSection = System.Windows.Visibility.Hidden;

                }
                else
                    HidePasswordSection = System.Windows.Visibility.Visible;
                RaisePropertyChanged("HidePasswordSection");
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
                //OnPropertyChanged("TotalSalesCostTemp");
                RaisePropertyChanged("TotalSalesCostTemp");
            }
        }

        protected virtual void OnJobSetupChanged(EventArgs e)
        {
            JobSetupChange?.Invoke(this, e);
        }
        public JobSetup(string name)            
        {
            ProjectName = name;
            PopulateCalculatbleTextBox();
            GetOriginalName();
            if (originalName == "Paraseal")
            {
                SqftLabel = "Sqft of Vertical Concrete Walls";
            }
            else if (originalName == "Paraseal LG" || originalName == "Paraseal GM")
            {
                SqftLabel = "Sqft of Vertical Lagging Walls";
            }
            else if (originalName == "201" || originalName == "250 GC"|| originalName=="Xypex")
            {
                SqftLabel = "Total Sqft Horizontal Concrete";
            }
            else if (originalName == "Dual Flex")
                SqftLabel = "Total Sqft Horizontal (Neobond Anti-Fracture)";
            else if (originalName == "860 Carlisle")
                SqftLabel = "Total Sqft Concrete Decks";
            else if (originalName == "Westcoat Epoxy")
                SqftLabel = "Total Sqft Floor";
            else if (originalName == "Polyurethane Injection Block")
                SqftLabel = "Total Sqft Horizontal Concrete Floor";
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
            //ApplyChangesCommnad = new DelegateCommand(ApplyChanges, canApply);
            MinMarkUp = -10;
            AllowMoreMarkUp = false;
            FirstCheckBoxLabel = "Approved for Sand & Cement ?";
            ProjectDelayFactor = "0-3 Months";
            UpdateJobSetup();

        }

        //private bool canApply(object obj)
        //{
        //    return HomeViewModel.IsDirty;
        //}

        //private void ApplyChanges(object obj)
        //{
        //    UpdateJobSetup();
        //    //HomeViewModel.IsDirty = false;
        //}

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
                    RaisePropertyChanged("EnableMoreMarkupCommand");
                }
            }
        }

        //ApplyChangesCommnad

        //private DelegateCommand applyChangesCommand;

        //[IgnoreDataMember]
        //public DelegateCommand ApplyChangesCommnad
        //{
        //    get { return applyChangesCommand; }
        //    set
        //    {
        //        if (value != applyChangesCommand)
        //        {
        //            applyChangesCommand = value;
        //            OnPropertyChanged("ApplyChangesCommnad");
        //        }
        //    }
        //}

        
        private bool isNewProject;
        public bool IsNewProject
        {
            get { return isNewProject; }
            set
            {
                //if (value != isNewProject)
                //{
                //    isNewProject = value;
                //    OnPropertyChanged("IsNewProject");                  
                //}
                Set(ref isNewProject, value);
            }
        }
        private bool isContingencyEnabled;
        public bool IsContingencyEnabled
        {
            get { return isContingencyEnabled; }
            set
            {
                //if (value != isContingencyEnabled)
                //{
                //    isContingencyEnabled = value;
                //    OnPropertyChanged("IsContingencyEnabled");
                //    UpdateJobSetup();
                //}
                Set(ref isContingencyEnabled, value);
                UpdateJobSetup();
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
                    return "Linear of Deck Perimeter";
                else if (originalName == "Resistite" || originalName == "Multicoat")
                    return "Linear of Deck to Wall Detail";
                else if (originalName == "Dexcellent II")
                    return "Linear of Deck to Wall Metal";
                else if (originalName == "Paraseal")
                    return "LF of Perimeter Footing (Standard Paragranular Detail and Term Bar)";
                else if (originalName == "Paraseal LG"|| originalName == "Paraseal GM")
                    return "LF of Perimeter Footing (adds term bar only )";
                else if (originalName == "Tufflex" || originalName == "201" || originalName == "250 GC" || originalName == "UPI BT")
                    return "Linear of Perimeter (Decks)";
                else if (originalName == "860 Carlisle")
                    return "Linear of Wall Metal(fluid applied detail)";
                else if (originalName == "Dual Flex")
                    return "PERIMETER";
                else if (originalName == "Desert Crete")
                    return "Linear of Deck to Wall Metal (Cove Base)";
                else if (originalName == "Polyurethane Injection Block")
                    return "Linear of Cold Joints";
                else if (originalName == "Westcoat Epoxy")
                    return "Linear of 3/16 inch Cove Base";
                else if (originalName == "Block Wall")
                    return "Linear of Inside Corners";
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
                    return "Total Sqft Vertical Walls (Membrane Only)";
                }
                else if (originalName == "Polyurethane Injection Block")
                    return "Toatl Sqft Vertical Block (Excl 1st 2 Courses)";
                else
                    return "Total Sqft Vertical (Block Wall)";
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
                //if (value!=jobName)
                //{
                //    jobName = value;
                //    OnPropertyChanged("JobName");
                //}
                Set(ref jobName, value);
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
                //if (value != vendorName)
                //{
                //    vendorName = value;
                //    OnPropertyChanged("VendorName");
                //    UpdateJobSetup();

                //}
                Set(ref vendorName, value);
                UpdateJobSetup();
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
                //if (value != bidBy)
                //{
                //    bidBy = value;
                //    OnPropertyChanged("BidBy");
                //}
                Set(ref bidBy, value);
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
                //if (value!=workArea)
                //{

                //    workArea = value;
                //    OnPropertyChanged("WorkArea");
                //    if (OnProjectNameChange != null)
                //    {
                //        OnProjectNameChange(this, EventArgs.Empty);
                //    }
                //}
                Set(ref workArea, value);
                if (OnProjectNameChange != null)
                {
                    OnProjectNameChange(this, EventArgs.Empty);
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
                //if (value != selecteddate)
                //{
                //    selecteddate = value;                   
                //    OnPropertyChanged("SelectedDate");
                //}
                Set(ref selecteddate, value);
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
                if (value!=totalSqft)
                {
                    if (ZData != null)
                    {
                        var myData = ZData.FirstOrDefault(x => x.Key == "TotalSqft");
                        if (!calledFromBackend)
                        {
                            myData.Formula = value.ToString();
                            
                        }
                        
                    }
                    calledFromBackend = false;
                }
               


                Set(ref totalSqft, value);
                //if (selectedData != null)
                //{
                //    if ( selectedData.Key == "TotalSqft")
                //    {
                //        selectedData.Formula = value.ToString();
                //    }
                //}
                setContingency();

                UpdateJobSetup();
            }
        }

        private bool vhasContingencyDisc;
        public bool VHasContingencyDisc
        {
            get { return vhasContingencyDisc; }
            set
            {
                //if (value != vhasContingencyDisc)
                //{
                //    vhasContingencyDisc = value;
                //    OnPropertyChanged("VHasContingencyDisc");
                //    UpdateJobSetup();
                //}
                Set(ref vhasContingencyDisc, value);
                UpdateJobSetup();
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
                if (value!=deckPerimeter)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "DeckPerimeter");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "DeckPerimeter")
                //    { selectedData.Formula = value.ToString();}
                //}
                Set(ref deckPerimeter, value);
                UpdateJobSetup();
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
                if (value!=riserCount)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "RiserCount");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }

                }

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "RiserCount") {selectedData.Formula = value.ToString();}
                //}
                Set(ref riserCount, value);
                UpdateJobSetup();
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
                if (value!=deckCount)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "DeckCount");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "DeckCount")
                //    {
                //        selectedData.Formula = value.ToString();
                //    }
                //}

                Set(ref deckCount, value);
                setContingency();
                UpdateJobSetup();
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
                //if (value != isApprovedForSandCement)
                //{
                //    isApprovedForSandCement = value;
                //    //IsReseal = value;

                //    OnPropertyChanged("IsApprovedForSandCement");
                //    UpdateJobSetup();
                //}
                Set(ref isApprovedForSandCement, value);
                
                UpdateJobSetup();
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
                //if (value != isPrevalingWage)
                //{
                //    isPrevalingWage = value;
                //    OnPropertyChanged("IsPrevalingWage");
                //    UpdateJobSetup();
                //}
                Set(ref isPrevalingWage, value);

                UpdateJobSetup();
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
                //if (value!=laborRate)
                //{
                //    laborRate = value;
                //    OnPropertyChanged("LaborRate");
                //    UpdateJobSetup();
                //}
                Set(ref laborRate, value);

                UpdateJobSetup();
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
                //if (value != hasSpecialMaterial)
                //{
                //    hasSpecialMaterial = value;
                //    OnPropertyChanged("HasSpecialMaterial");
                //    UpdateJobSetup();
                //}
                Set(ref hasSpecialMaterial, value);

                UpdateJobSetup();
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
                //if (value != isFlashingRequired)
                //{
                //    isFlashingRequired = value;
                //    OnPropertyChanged("IsFlashingRequired");
                //    UpdateJobSetup();
                //}
                Set(ref isFlashingRequired, value);

                UpdateJobSetup();
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
                //if (value != hasSpecialPricing)
                //{
                //    hasSpecialPricing = value;
                //    OnPropertyChanged("HasSpecialPricing");
                //    UpdateJobSetup();
                //}
                Set(ref hasSpecialPricing, value);

                UpdateJobSetup();
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
                //if (value != hasDiscount)
                //{
                //    hasDiscount = value;
                //    OnPropertyChanged("HasDiscount");
                //    UpdateJobSetup();
                //}
                Set(ref hasDiscount, value);

                UpdateJobSetup();
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
                //if (value != markupPercentage)
                //{
                //    markupPercentage = value;
                //    OnPropertyChanged("MarkupPercentage");
                //    UpdateJobSetup();
                //}
                Set(ref markupPercentage, value);

                UpdateJobSetup();
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
                //if (value != materialName)
                //{
                //    materialName = value;
                //    OnPropertyChanged("MaterialName");
                //    UpdateJobSetup();

                
                Set(ref materialName, value);

                UpdateJobSetup();
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
                //if (value != stairWidth)
                //{
                //    stairWidth = value;
                //    OnPropertyChanged("StairWidth");
                //    UpdateJobSetup();
                //}
                Set(ref stairWidth, value);

                UpdateJobSetup();
            }
        }

        #region DesertCrete
        private bool? isJobSpecifiedByArchitect;
        public bool? IsJobSpecifiedByArchitect
        {
            get { return isJobSpecifiedByArchitect; }
            set
            {
                //if (value!=isJobSpecifiedByArchitect)
                //{
                //    isJobSpecifiedByArchitect = value;
                //    OnPropertyChanged("IsJobSpecifiedByArchitect");
                //    UpdateJobSetup();
                //}
                Set(ref isJobSpecifiedByArchitect, value);

                UpdateJobSetup();
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
                //if (value != isSystemOverConcrete)
                //{
                //    isSystemOverConcrete = value;
                //    OnPropertyChanged("IsSystemOverConcrete");
                //    UpdateJobSetup();
                //}
                Set(ref isSystemOverConcrete, value);

                UpdateJobSetup();
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
                //if (value != isNewPlywood)
                //{
                //    isNewPlywood = value;
                //    OnPropertyChanged("IsNewPlywood");
                //    UpdateJobSetup();
                //}
                Set(ref isNewPlywood, value);

                UpdateJobSetup();
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
                    return "# RISERS (3.5-4 Ft Wide)";
                else if (originalName == "Paraseal LG" || originalName == "Paraseal GM")
                    return "Tie Backs (block outs must be priced separately)";
                else if (originalName == "Dual Flex")
                    return "#  Risers Includes Metal (3.5-4 Ft Wide)";
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
                    return "Sqft of Between Slab Membrane (Concrete)";
                }
                else if (originalName == "Paraseal LG" || originalName == "Paraseal GM")
                {
                    return "Additional Super Stop for Cold Joints in Field or Extra Lifts(LF)";
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
                    originalName == "Paraseal LG" ||originalName == "Paraseal GM" || originalName=="860 Carlisle"||originalName=="UPI BT"||originalName=="Westcoat Epoxy"
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
                //if (value != isReseal)
                //{
                //    isReseal = value;
                //    OnPropertyChanged("IsReseal");
                //    UpdateJobSetup();
                //}
                Set(ref isReseal, value);

                UpdateJobSetup();
            }
        }
        private double totalSqftPlywood;
        public double TotalSqftPlywood
        {
            get { return totalSqftPlywood; }
            set
            {

                if (value!= totalSqftPlywood)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "TotalSqftPlywood");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "TotalSqftPlywood") {selectedData.Formula = value.ToString();}
                //}

                Set(ref totalSqftPlywood, value);
                setContingency();
                UpdateJobSetup();
            }
        }

        private void setContingency()
        {
            if (originalName== "Paraseal" )
            {
                if (DeckCount >= 1000 || TotalSqftVertical >= 1000 || TotalSqft >= 1000)
                {
                    VHasContingencyDisc = false;
                    IsContingencyEnabled = true;
                }
                else
                {
                    IsContingencyEnabled = false;
                    VHasContingencyDisc = false;
                }
            }
            else
            {
                if (TotalSqftPlywood >= 1000 || TotalSqftVertical >= 1000 || TotalSqft >= 1000)
                {
                    VHasContingencyDisc = false;
                    IsContingencyEnabled = true;
                }
                else
                {
                    IsContingencyEnabled = false;
                    VHasContingencyDisc = false;
                }
            }
            
            RaisePropertyChanged("IsContingencyEnabled");
            RaisePropertyChanged("VHasContingencyDisc");
            
        }
        #endregion

        #region Resistite
        public string LinearFootageText
        {
            get
            {
                if (originalName == "Paraseal")
                {
                    return "Linear Footage of UV Protection at Wall (801)";
                }
                else if (originalName == "860 Carlisle")
                    return "Linear Footage of Footing";
                else if (originalName == "Dual Flex")
                    return "Linera Footage of Deck to Wall";
                else
                    return "Linear Footage of Coping";
            }
        }
        private double linearCopingFootage;
        public double LinearCopingFootage
        {
            get { return linearCopingFootage; }
            set
            {
                
                //if (ZData != null)
                //{
                //    var mydata = ZData.FirstOrDefault(x => x.Key == "LinearCopingFootage");
                //    mydata.Formula = value.ToString();
                //}

                if (selectedData != null)
                {
                    if (selectedData.Formula==""&& selectedData.Key== "LinearCopingFootage"){selectedData.Formula = value.ToString();}
                }
                Set(ref linearCopingFootage, value);
                UpdateJobSetup();
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
                if (value!= additonalTermbarLF)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "AdditionalTermBarLF");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "AdditionalTermBarLF") {selectedData.Formula = value.ToString();}
                //}
                Set(ref additonalTermbarLF, value);
                UpdateJobSetup();
            }
        }
        private bool superStopAtFooting;
        public bool SuperStopAtFooting
        {
            get { return superStopAtFooting; }
            set
            {
                //if (value != superStopAtFooting)
                //{
                //    superStopAtFooting = value;
                //    OnPropertyChanged("SuperStopAtFooting");
                //    UpdateJobSetup();
                //}
                Set(ref superStopAtFooting, value);
                UpdateJobSetup();
            }
        }
        private double insideOutsideCornerDetails;
        public double InsideOutsideCornerDetails
        {
            get { return insideOutsideCornerDetails; }
            set
            {
                if (value!=insideOutsideCornerDetails)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "InsideOutsideCornerDetails");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "InsideOutsideCornerDetails") {selectedData.Formula = value.ToString();}
                //}
                Set(ref insideOutsideCornerDetails, value);
                UpdateJobSetup();
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

                if (originalName == "Paraseal LG" || originalName=="Paraseal GM")
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

                if (value!=rakerCornerBases)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "RakerCornerBases");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }


                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "RakerCornerBases") {selectedData.Formula = value.ToString();}
                //}
                Set(ref rakerCornerBases, value);
                UpdateJobSetup();
            }
        }
        private double cementBoardDetail;
        public double CementBoardDetail
        {
            get { return cementBoardDetail; }
            set
            {
                if (value!=cementBoardDetail)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "CementBoardDetail");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "CementBoardDetail")
                //    {
                //        selectedData.Formula = value.ToString();
                //    }
                //}
                Set(ref cementBoardDetail, value);
                UpdateJobSetup();
            }
        }
        private double rockPockets;
        public double RockPockets
        {
            get { return rockPockets; }
            set
            {
                if (value!=rockPockets)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "RockPockets");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "RockPockets")
                //    {
                //        selectedData.Formula = value.ToString();
                //    }
                //}
                Set(ref rockPockets, value);
                UpdateJobSetup();
            }
        }
        private double parasealFoundation;
        public double ParasealFoundation
        {
            get { return parasealFoundation; }
            set
            {
                if (value!=parasealFoundation)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "ParasealFoundation");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
               

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "ParasealFoundation")
                //    {
                //        selectedData.Formula = value.ToString();
                //    }
                //}
                Set(ref parasealFoundation, value);
                UpdateJobSetup();
            }
        }
        private double rearMidLagging;
        public double RearMidLagging
        {
            get { return rearMidLagging; }
            set
            {
                if (value!=rearMidLagging)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "RearMidLagging");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "RearMidLagging") {selectedData.Formula = value.ToString();}
                //}
                Set(ref rearMidLagging, value);
                UpdateJobSetup();
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
                if (value!=totalSqftVertical)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "TotalSqftVertical");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "TotalSqftVertical") {selectedData.Formula = value.ToString();}
                //}
                Set(ref totalSqftVertical, value);
                setContingency();

                UpdateJobSetup();
            }
        }

        private double termBarLF;
        public double TermBarLF
        {
            get { return termBarLF; }
            set
            {

                if (value!=termBarLF)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "TermBarLF");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "TermBarLF") {selectedData.Formula = value.ToString();}
                //}
                Set(ref termBarLF, value);
                UpdateJobSetup();
            }
        }
        private double rebarPrepWallsLF;
        public double RebarPrepWallsLF
        {
            get { return rebarPrepWallsLF; }
            set
            {

                if (value!=rebarPrepWallsLF)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "RebarPrepWallsLF");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "RebarPrepWallsLF") {selectedData.Formula = value.ToString();}
                //}
                Set(ref rebarPrepWallsLF, value);
                UpdateJobSetup();
            }
        }
        private double superStopLF;
        public double SuperStopLF
        {
            get { return superStopLF; }
            set
            {
                if (value!=superStopLF)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "SuperStopLF");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                

                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "SuperStopLF") {selectedData.Formula = value.ToString();}
                //}
                Set(ref superStopLF, value);
                UpdateJobSetup();
            }
        }
        
        private double penetrations;
        public double Penetrations
        {
            get { return penetrations; }
            set
            {
                if (value!=penetrations)
                {
                    if (ZData != null)
                    {
                        var mydata = ZData.FirstOrDefault(x => x.Key == "Penetrations");
                        if (!calledFromBackend)
                        {
                            mydata.Formula = value.ToString();
                        }
                    }
                }
                
                
                //if (selectedData != null)
                //{
                //    if (selectedData.Formula=="" && selectedData.Key == "Penetrations") {selectedData.Formula = value.ToString();}
                //}
                Set(ref penetrations, value);
                UpdateJobSetup();
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
                
                Set(ref hasQuarterMortarBed, value);
                UpdateJobSetup();
            }
        }
        private bool hasQuarterLessMortarBed;
        public bool HasQuarterLessMortarBed
        {
            get { return hasQuarterLessMortarBed; }
            set
            {
                Set(ref hasQuarterLessMortarBed, value);
                UpdateJobSetup();
            }
        }
        private bool hasElastex;
        public bool HasElastex
        {
            get { return hasElastex; }
            set
            {
                
                Set(ref hasElastex, value);
                UpdateJobSetup();
            }
        }
        private bool hasEasyAccess;
        public bool HasEasyAccess
        {
            get { return hasEasyAccess; }
            set
            {
                
                Set(ref hasEasyAccess, value);
                UpdateJobSetup();
            }
        }
        #endregion
    }
}
