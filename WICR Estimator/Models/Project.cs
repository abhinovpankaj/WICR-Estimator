//using MyToolkit.Data;
using MyToolkit.Model;
using MyToolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using WICR_Estimator.Services;
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
    
    
    public class Project: UndoRedoObservableObject
    {
        private string productVersion;
        public string ProductVersion
        {
            //set
            //{
            //    if (value!=productVersion)
            //    {
            //        productVersion = value;
            //        OnPropertyChanged("ProductVersion");
            //    }
            //}
            set
            {
                Set(ref productVersion, value);
            }
            get
            {
                return productVersion;//"2.1";
            }
        }
        public int ProjectID { get; set; }
        public int EstimateID { get; set; }
        public string CreationDetails { get; set; }

        private int activeTabIndex;
        public int ActiveTabIndex
        {
            get { return activeTabIndex; }
            set
            {
                Set(ref activeTabIndex, value);
            }
        }

        private string originalProjectName;

        public string OriginalProjectName
        {
            get { return originalProjectName; }
            //set
            //{
            //    if (value!=originalProjectName)
            //    {
            //        originalProjectName = value;
            //        OnPropertyChanged("OriginalProjectName");
            //    }
            //}
            set
            {
                Set(ref originalProjectName, value);
            }
        }
        public Dictionary<string, int> lastUsedRows;

        [IgnoreDataMember]
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
                //RegisterForUndoRedo();
            }

            

        }

        

        public void RegisterForUndoRedo(Project prj)
        {
            //string[] ds = { "MetalBaseViewModel","Metals" };
            undoRedoManager = new UndoRedoManager(prj, new MyDispatcher());
            //undoRedoManager.Reset();
            
        }
        private UndoRedoManager undoRedoManager;
        public int CopyCount { get; set; }

        public string Name { get; set; }
        #region MainTable prop
        public void UpdateMainTable()
        {
            //OnPropertyChanged("TotalCost");
            //OnPropertyChanged("MetalCost");
            //OnPropertyChanged("SlopeCost");

            //OnPropertyChanged("SystemNOther");
            //OnPropertyChanged("CostPerSqFoot");
            //OnPropertyChanged("SystemNOther");
            //OnPropertyChanged("LaborCost");
            //OnPropertyChanged("TotalCost");
            //OnPropertyChanged("MaterialCost");
            //OnPropertyChanged("LaborPercentage");

            RaisePropertyChanged("TotalCost");
            RaisePropertyChanged("MetalCost");
            RaisePropertyChanged("SlopeCost");

            RaisePropertyChanged("SystemNOther");
            RaisePropertyChanged("CostPerSqFoot");
            RaisePropertyChanged("SystemNOther");
            RaisePropertyChanged("LaborCost");
            RaisePropertyChanged("TotalCost");
            RaisePropertyChanged("MaterialCost");
            RaisePropertyChanged("LaborPercentage");
        }
        //private double metalCost;
        public double MetalCost
        {
            //set
            //{
            //    if (value!=metalCost)
            //    {
            //        if (MaterialViewModel != null)
            //        {
            //            metalCost= MaterialViewModel.TotalMetalPrice;
            //        }
            //        else
            //            metalCost= 0;
            //    }
            //    OnPropertyChanged("MetalCost");
            //}
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
        //private double slopeCost;
        public double SlopeCost
        {
            //set
            //{
            //    if (value!=slopeCost)
            //    {
            //        if (MaterialViewModel != null)
            //        {
            //            slopeCost= MaterialViewModel.TotalSlopingPrice;
            //        }
            //        else
            //            slopeCost= 0;
            //    }
            //}
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
        //private double systemNOther;
        public double  SystemNOther
        {
            //set
            //{
            //    if (value!=systemNOther)
            //    {
            //        if (MaterialViewModel != null)
            //        {
            //            systemNOther= MaterialViewModel.TotalSystemPrice;
            //        }
            //        else
            //            systemNOther= 0;
            //    }
            //}
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
        //private string workArea;
        public string WorkArea
        {
            //set
            //{
            //    if (value!=workArea)
            //    {
            //        if (ProjectJobSetUp != null)
            //        {
            //            workArea= ProjectJobSetUp.WorkArea;
            //        }
            //        else
            //            workArea= "";
            //    }
            //}
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
        //private double costPerSqft;
        public double CostPerSqFoot
        {
            //set
            //{
            //    if (value!=costPerSqft)
            //    {
            //        if (MaterialViewModel != null)
            //        {
            //            costPerSqft= Math.Round(MaterialViewModel.TotalCostperSqft, 2);
            //        }
            //        else
            //            costPerSqft= 0;
            //    }
            //}
            get
            {

                if (MaterialViewModel != null)
                {
                    return Math.Round(MaterialViewModel.TotalCostperSqft, 2);
                }
                else
                    return 0;
            }
        }
        public double SubContractCost { get; set; }

        //private double laborCost;
        public double LaborCost
        {
            //set
            //{
            //    if (value!=laborCost)
            //    {
            //        if (MaterialViewModel != null)
            //        {
            //            laborCost= MaterialViewModel.AllTabsLaborTotal;
            //        }
            //        else
            //            laborCost= 0;
            //    }
            //}
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
        //private double materialCost;
        public double MaterialCost
        {
            //set
            //{
            //    if (value!=materialCost)
            //    {
            //        if (MaterialViewModel != null)
            //        {
            //            materialCost= MaterialViewModel.AllTabsMaterialTotal;
            //        }
            //        else
            //            materialCost= 0;
            //    }
            //}
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

        //private string laborPerc;
        public string LaborPercentage
        {
            //set
            //{
            //    if (value!=laborPerc)
            //    {
            //        if (MaterialViewModel != null)
            //        {
            //            laborPerc= MaterialViewModel.TotalSale == 0 ? "0" : Math.Round(MaterialViewModel.AllTabsLaborTotal / MaterialViewModel.TotalSale * 100, 2).ToString() + "%";
            //        }
            //        else
            //            laborPerc= "";
            //    }
            //}
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalSale == 0 ? "0" : Math.Round(MaterialViewModel.AllTabsLaborTotal / MaterialViewModel.TotalSale * 100, 2).ToString() + "%";
                }
                else
                    return  "";
            }
        }
       // private double totalCost;
        public double TotalCost
        {
        //    set
        //    {
        //        if (value!=totalCost)
        //        {
        //            if (MaterialViewModel != null)
        //            {
        //                totalCost= MaterialViewModel.TotalSale;
        //            }
        //            else
        //                totalCost= 0;
        //        }
        //    }
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

        #endregion

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
                    //OnPropertyChanged("IsSelectedProject");
                    RaisePropertyChanged("IsSelectedProject");
                    if (OnSelectedProjectChange!=null)
                    {
                        OnSelectedProjectChange(this, EventArgs.Empty);
                    }
                }
            }
        }

        [IgnoreDataMember]
        public int Rank { get; set; }

        public string GrpName { get; set; }
        public string MainGroup { get; set; }

        private JobSetup jobSetup;
        public JobSetup ProjectJobSetUp
        {
            get { return jobSetup; }
            set
            {
                Set(ref jobSetup, value);
                //if (jobSetup!=value)
                //{
                //    jobSetup = value;
                //    RaisePropertyChanged("ProjectJobSetUp");
                //}
            }
        }

        private MetalBaseViewModel metalViewModel;
        
        public MetalBaseViewModel MetalViewModel
        {
            get
            {
                return metalViewModel;
            }
            set
            {
                //if (metalViewModel != value)
                //{
                //    metalViewModel = value;
                //    OnPropertyChanged("MetalViewModel");
                //    //RaisePropertyChanged("MetalViewModel");
                //}

                Set(ref metalViewModel, value);
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
                //if (slopeViewModel != value)
                //{
                //    slopeViewModel = value;
                //    //OnPropertyChanged("SlopeViewModel");
                //    RaisePropertyChanged("SlopeViewModel");
                //}
                Set(ref slopeViewModel, value);
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
                //if (materialViewModel != value)
                //{
                //    materialViewModel = value;
                //    RaisePropertyChanged("MaterialViewModel");
                //}
                Set(ref materialViewModel, value);
            }
        }

        
        public static event EventHandler OnSelectedProjectChange;
        
        private ICommand updatedJobSetup;
        
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

        #region undoredo
        public ICommand RedoCommand
        {
            get
            {
                return new DelegateCommand(Redo, canRedo);
            }
        }

        private void Redo(object obj)
        {
            undoRedoManager.Redo();
        }

        private bool canRedo(object obj)
        {
            return undoRedoManager.CanRedo;
        }

        public ICommand UndoCommand
        {
            get
            {
                return new DelegateCommand(Undo, canUndo);
            }
        }

        private void Undo(object obj)
        {
            undoRedoManager.Undo();
            
        }

        private bool canUndo(object obj)
        {
            return undoRedoManager.CanUndo;
            //if (undoRedoManager.CurrentIndex>3)
            //{
            //    return undoRedoManager.CanUndo;
            //}
            //else
            //    return false;

        }

        #endregion
    }

    public class MyDispatcher : IDispatcher
    {
        public void InvokeAsync(Action action)
        {
            action.Invoke();
        }
    }
}
