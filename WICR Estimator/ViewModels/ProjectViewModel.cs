using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    class ProjectViewModel : BaseViewModel, IPageViewModel
    {
        
        private ObservableCollection<Project> enabledProjects;
        public ProjectViewModel(ObservableCollection<Project> enabledProjects)
            :this()
        {
            EnabledProjects = enabledProjects;
        }
        public ProjectViewModel()
        {           
            EnabledProjects = new ObservableCollection<Project>();
            EnabledProjects = HomeViewModel.MyselectedProjects;
            if (EnabledProjects!=null)
            {
                foreach (Project prj in EnabledProjects)
                {
                    if (prj.MetalViewModel==null)
                    {
                        prj.MetalViewModel = new MetalViewModel();
                    }
                    if (prj.ProjectJobSetUp==null)
                    {
                        prj.ProjectJobSetUp = new JobSetup();
                    }
                    if (prj.SlopeViewModel == null)
                    {
                        prj.SlopeViewModel = new SlopeViewModel();
                    }
                    if (prj.MaterialViewModel == null)
                    {
                        prj.MaterialViewModel = new MaterialViewModel();
                    }
                    if (prj.LaborViewModel == null)
                    {
                        prj.LaborViewModel = new LaborViewModel();
                    }
                }
            }                                     
        }
        #region Properties
        
        public ObservableCollection<Project> EnabledProjects
        {
            get
            {
                return enabledProjects;
            }
            set
            {
                if (enabledProjects != value)
                {
                    enabledProjects = value;
                    OnPropertyChanged("EnabledProjects");
                }
            }
        }
        #endregion

        #region Methods
        
        #endregion 
       

        public string Name
        {
            get
            {
                return "Project Page";
            }
        }
    }
}
