using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
       
    public class ProjectsTotal:BaseViewModel
    {
        private string name;
        private string workArea;
        private double metalCost;
        private double slopeCost;

        private double systemCost;
        private double materialCost;
        private double laborCost;
        private double totalCost;
        private string laborPerc;



        

        public string Name
        {
            get { return name; }
            set
            {
                if (value!=name)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string WorkArea
        {
            get { return workArea; }
            set
            {
                if (value != workArea)
                {
                    workArea = value;
                    OnPropertyChanged("WorkArea");
                }
            }
        }

        public double MetalCost
        {
            get { return metalCost; }
            set
            {
                if (value != metalCost)
                {
                    metalCost = value;
                    OnPropertyChanged("MetalCost");
                }
            }
        }

        public double SlopeCost
        {
            get { return slopeCost; }
            set
            {
                if (value != slopeCost)
                {
                    slopeCost = value;
                    OnPropertyChanged("SlopeCost");
                }
            }
        }

        public double SystemCost
        {
            get { return systemCost; }
            set
            {
                if (value != systemCost)
                {
                    systemCost = value;
                    OnPropertyChanged("SystemCost");
                }
            }
        }
        public double MaterialCost
        {
            get { return materialCost; }
            set
            {
                if (value != materialCost)
                {
                    materialCost = value;
                    OnPropertyChanged("MaterialCost");
                }
            }
        }
        public double LaborCost
        {
            get { return laborCost; }
            set
            {
                if (value != laborCost)
                {
                    laborCost = value;
                    OnPropertyChanged("LaborCost");
                }
            }
        }

        public double TotalCost
        {
            get { return totalCost; }
            set
            {
                if (value != totalCost)
                {
                    totalCost = value;
                    OnPropertyChanged("TotalCost");
                }
            }
        }
        public string LaborPercentage
        {
            get { return laborPerc; }
            set
            {
                if (value != laborPerc)
                {
                    laborPerc = value;
                    OnPropertyChanged("LaborPercentage");
                }
            }
        }
        

    }
    
    public class Project: BaseViewModel
    {
        
        public string ProductVersion
        {
            set { value = "1.0"; }
            get
            {
                return "1.0";
            }
        }
        public string CreationDetails { get; set; }

        public int ActiveTabIndex { get; set; }
        public string OriginalProjectName { get; set; }
        public Dictionary<string, int> lastUsedRows;

        [XmlIgnore]
        public bool ApplyLatestPrices { get; set; }

        public System.Windows.Visibility ISVisible
        {
            get
            {
                if (Name == "Weather Wear")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Hidden;
            }    
        }
        

        private bool _isExpanded = true;
        public bool IsGroupExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }
        public Project()
        {
            if (updatedJobSetup == null)
            {
                updatedJobSetup = new DelegateCommand(UpdateJobSetUp, canUpdate);
            }
        }
        public int CopyCount { get; set; }

        public string Name { get; set; }
        public void UpdateMainTable()
        {
            OnPropertyChanged("TotalCost");
            OnPropertyChanged("MetalCost");
            OnPropertyChanged("SlopeCost");

            OnPropertyChanged("SystemNOther");
            OnPropertyChanged("CostPerSqFoot");
            OnPropertyChanged("SystemNOther");
            OnPropertyChanged("LaborCost");
            OnPropertyChanged("TotalCost");
            OnPropertyChanged("MaterialCost");
            OnPropertyChanged("LaborPercentage");
        }
        public double MetalCost
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalMetalPrice;
                }
                else
                    return 0;
            }
        }
        public double SlopeCost
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalSlopingPrice;
                }
                else
                    return 0;
            }
        }
        public double  SystemNOther
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalSystemPrice;
                }
                else
                    return 0;
            }
        }
        public string WorkArea
        {
            get
            {
                if (ProjectJobSetUp != null)
                {
                    return ProjectJobSetUp.WorkArea;
                }
                else
                    return "";
            }
        }

        public double CostPerSqFoot
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return Math.Round(MaterialViewModel.TotalCostperSqft,2);
                }
                else
                    return 0;

            }
        }
        public double SubContractCost { get; set; }
        public double LaborCost
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.AllTabsLaborTotal;
                }
                else
                    return 0;
                
            }
        }
        public double MaterialCost
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.AllTabsMaterialTotal;
                }
                else
                    return 0;

            }
        }
        

        public string LaborPercentage
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalSale==0? "0": Math.Round(MaterialViewModel.AllTabsLaborTotal/ MaterialViewModel.TotalSale *100,2).ToString()+"%";
                }
                else
                    return "";
            }
        }
        public double TotalCost {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalSale;
                }
                else
                    return 0;
            }
        }
        private bool isSelectedProject;
        public bool IsSelectedProject
        {
            get
            {
                return isSelectedProject;
            }
            set
            {
                if (isSelectedProject != value)
                {
                    isSelectedProject = value;
                    OnPropertyChanged("IsSelectedProject");
                    if (OnSelectedProjectChange!=null)
                    {
                        OnSelectedProjectChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        public int Rank { get; set; }
        public string GrpName { get; set; }
        public string MainGroup { get; set; }

        public JobSetup ProjectJobSetUp { get; set; }

        private MetalBaseViewModel metalViewModel;
        
        public MetalBaseViewModel MetalViewModel
        {
            get
            {
                return metalViewModel;
            }
            set
            {
                if (metalViewModel != value)
                {
                    metalViewModel = value;
                    OnPropertyChanged("MetalViewModel");

                }
            }
        }
        private SlopeBaseViewModel slopeViewModel;
       
        public SlopeBaseViewModel SlopeViewModel
        {
            get
            {
                return slopeViewModel;
            }
            set
            {
                if (slopeViewModel != value)
                {
                    slopeViewModel = value;
                    OnPropertyChanged("SlopeViewModel");

                }
            }
        }
        private MaterialBaseViewModel materialViewModel;
        
        public MaterialBaseViewModel MaterialViewModel
        {
            get
            {
                return materialViewModel;
            }
            set
            {
                if (materialViewModel != value)
                {
                    materialViewModel = value;
                    OnPropertyChanged("MaterialViewModel");
                }
            }
        }

        
        public static event EventHandler OnSelectedProjectChange;
        
        private ICommand updatedJobSetup;
        //[XmlIgnore]
        [IgnoreDataMember]
        public ICommand UpdatedJobSetup
        {
            set { updatedJobSetup = value; }
            get
            {                
                return updatedJobSetup;
            }
        }

        private bool canUpdate(object obj)
        {
            return true;
        }

        private void UpdateJobSetUp(object obj)
        {
            if(this.MetalViewModel!=null)
             this.MetalViewModel.CalculateCost(null);
        }

        //private void ProjectJobSetUp_OnJobSetupChange(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
        public override string ToString()
        {
            return "Selected Project:"+ Name;
        }

        
    }
}
